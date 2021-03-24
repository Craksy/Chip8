using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Diagnostics;



namespace Chip8.Emulator
{

    /// <summary>
    /// Acts as instruction decoder, and holds the implementation for all of the instructions.
    /// Refer to https://en.wikipedia.org/wiki/CHIP-8#Opcode_table for a more detailed description of each.
    /// 
    /// A reference to the `Emulator` instance is passed to the constructor so that each instruction can access
    /// and modify the rest of the system (ram, registers, timers, etc)
    ///
    /// This is a pretty big class and it can seem a bit bloated when scrolling through, 
    /// even though there are really just two main parts; the instruction decoder 
    /// followed by a series of instruction methods.
    /// </summary>
    class Processor
    {
        private Emulator emulator;
        private Random random;

        public Processor(Emulator emulator) {
            this.emulator = emulator;
            random = new Random(1234); //initializing with a seed for debugging purposes.
        }

        public void DecodeInstruction(short instruction) {
            //nybble the instruction
            byte firstNibble = (byte)(instruction >> 12 & 0xF);
            byte X = (byte)(instruction >> 8 & 0xF);
            byte Y = (byte)(instruction >> 4 & 0xF);
            byte lastNibble = (byte)(instruction & 0xF);

            // Constants and addresses are represented by the 
            // first 8 and 12 bits respectively
            byte NN = (byte)(instruction & 0xFF);
            short NNN = (short)(instruction & 0xFFF);

            switch ((firstNibble, X, Y, lastNibble)){
                case (0,0,0,0): NoOp(); break;
                case (0,0,0xE,0): ClearDisplay(); break;
                case (0,0,0xE,0xE): ReturnFromSubroutine(); break;
                case (1,_,_,_): JumpToAddress(NNN); break;
                case (2,_,_,_): CallSubRoutine(NNN); break;
                case (3,_,_,_): SkipIfEqualConst(X, NN); break;
                case (4,_,_,_): SkipIfNotEqualConst(X, NN); break;
                case (5,_,_,0): SkipIfEqualRegister(X, Y); break;
                case (6,_,_,_): SetRegisterToConst(X, NN); break;
                case (7,_,_,_): AddConstToRegister(X, NN); break;
                case (8,_,_,0): SetRegisterToRegister(X, Y); break;
                case (8,_,_,1): SetRegisterOrRegister(X, Y); break;
                case (8,_,_,2): SetRegisterAndRegister(X, Y); break;
                case (8,_,_,3): SetRegisterXorRegister(X, Y); break;
                case (8,_,_,4): AddRegisterToRegister(X, Y); break;
                case (8,_,_,5): SubtractRegisterFromRegister(X, Y); break;
                case (8,_,_,6): BitshiftRegisterRight(X); break; 
                case (8,_,_,7): SubtractReverseRegisters(X, Y); break;
                case (8,_,_,0xE): BitshiftRegisterLeft(X); break; 
                case (9,_,_,0): SkipIfNotEqualRegister(X,Y); break; 
                case (0xA,_,_,_): SetAddressRegister(NNN); break; 
                case (0xB,_,_,_): JumpToOffsetAddress(NNN); break;
                case (0xC,_,_,_): SetRegisterAndRandom(X,NN); break;
                case (0xD,_,_,_): DrawSprite2(X,Y,lastNibble); break;
                case (0xE,_,9,0xE): SkipIfKeyPressed(X); break;
                case (0xE,_,0xa,1): SkipIfNotKeyPressed(X); break;
                case (0xF,_,0,7): SetRegisterDelayTimer(X); break;
                case (0xF,_,0,0xA): WaitForKeyPress(X); break;
                case (0xF,_,1,5): SetDelayTimerRegister(X); break;
                case (0xF,_,1,8): SetSoundTimerRegister(X); break;
                case (0xF,_,1,0xe): AddRegisterToAddressRegister(X); break;
                case (0xF,_,2,9): PointAddressRegisterToCharacter(X); break;
                case (0xF,_,3,3): BinaryCodedDecimal(X); break; 
                case (0xF,_,5,5): DumpRegisters(X); break;
                case (0xF,_,6,5): LoadToRegisters(X); break;
                default: throw new Exception("Unknown instruction 0x" + instruction.ToString("X4"));
            }
        }


        private void SetRegisterDelayTimer(byte regx) {
            emulator.registers[regx] = (byte)(emulator.delayTimer);
        }

        private void WaitForKeyPress(byte regx) {
            emulator.awaitKeypress = regx;
        }

        private void SetDelayTimerRegister(byte regx) {
            emulator.delayTimer = emulator.registers[regx];
        }

        private void SetSoundTimerRegister(byte regx) {
            emulator.soundTimer = emulator.registers[regx];
        }

        private void AddRegisterToAddressRegister(byte regx) {
            emulator.addressRegister += emulator.registers[regx];
        }

        private void PointAddressRegisterToCharacter(byte regx) {
            byte character = emulator.registers[regx];
            emulator.addressRegister = (short)(character * 5);
        }

        private void BinaryCodedDecimal(byte regx) {
            //REVIEW: I think this might be bugged
            byte[] digits = new byte[3];
            int number = emulator.registers[regx];
            digits[0] = (byte)(number / 100);
            number -= digits[0] * 100;
            digits[1] = (byte)(number / 10);
            number -= digits[1];
            digits[2] = (byte)(number);

            emulator.ram.Write(digits, emulator.addressRegister);

        }

        private void DumpRegisters(byte register) {
            short address = emulator.addressRegister;
            byte[] registers = emulator.registers[0..(register + 1)];
            emulator.ram.Write(registers, address);
        }

        private void LoadToRegisters(byte register) {
            short address = emulator.addressRegister;
            byte[] memoryValues = emulator.ram.Read(address, register + 1);
            for (int i = 0; i < memoryValues.Length; i++) {
                emulator.registers[i] = memoryValues[i];
            }
        }

        private void SkipIfNotKeyPressed(byte regx) {
            byte key = emulator.registers[regx];
            if (!emulator.keystates[key])
                emulator.instructionPointer += 2;
        }

        private void SkipIfKeyPressed(byte regx) {
            byte key = emulator.registers[regx];
            if (emulator.keystates[key])
                emulator.instructionPointer += 2;
        }

        /// <summary>
        /// OPCODE: DXYN
        /// Draw a sprite to the display at the x,y position given by
        /// Registers X and Y.
        /// The sprite is 8 pixels wide, and N+1 pixels tall, and is read
        /// from memory starting at the position pointed to by the address register.
        /// ie. each row of the sprite is one byte read from memory.
        /// 
        /// The sprite is XOR'ed on top of the existing framebuffer meaning that if
        /// we try to set a bit that's already set, it will be flipped to 0 instead.
        /// If any bits are unset this way, set the carry flag.
        /// This is used for collision detection.
        /// </summary>
        /// <param name="regx">Register that holds the X coordinate to draw to</param>
        /// <param name="regy">Register that holds the Y coordinate to draw to</param>
        /// <param name="height">the height of the sprite</param>
        private void DrawSprite2(byte regx, byte regy, byte height) {
            byte x = emulator.registers[regx];
            byte y = emulator.registers[regy];
            int offset = x % 8;
            byte collision = 0;

            for (int row = 0; row < height; row++) {
                byte spriteRow = emulator.ram.Read(emulator.addressRegister + row); //read the next row of the sprite
                int frameBufferIndex = 0xF00 + ((y + row) * 8) + x / 8; // get the location in memory

                if (0xF00 > frameBufferIndex || frameBufferIndex > 0xFFF)
                    continue;

                // If the bit offset is non-zero and we're not writing to the last byte of the frame buffer, 
                // it means that two bytes will be affected.
                if (frameBufferIndex < 0xFFF && offset > 0) {
                    byte[] oldBufferBytes = emulator.ram.Read(frameBufferIndex, 2); //Read 2 bytes from the framebuffer
                    Array.Reverse(oldBufferBytes); // converting to big endian
                    ushort oldBuffer = BitConverter.ToUInt16(oldBufferBytes); //convert to unsigned short
                    if ((oldBuffer & (spriteRow << (8 - offset))) != 0) //check for collision
                        collision = 1; 
                    ushort newBuffer = (ushort)(oldBuffer ^ (spriteRow << (8 - offset))); //XOR the offset sprite data on top of old buffer
                    byte[] newBufferBytes = BitConverter.GetBytes(newBuffer); //convert back to byte array
                    Array.Reverse(newBufferBytes); // convert to big endian
                    emulator.ram.Write(newBufferBytes, frameBufferIndex); //write back to memory
                } else {
                    byte bufferByte = emulator.ram.Read(frameBufferIndex);
                    if ((spriteRow & bufferByte) != 0)
                        collision = 1;
                    emulator.ram.Write((byte)(spriteRow^bufferByte), frameBufferIndex);
                }
            }
            emulator.registers[0xF] = collision; //set the carry flag if there was collision.
        }

        private void SetRegisterAndRandom(byte regx, byte number) {
            byte r = (byte)(random.Next(0, 255) & number);
            emulator.registers[regx] = r;
        }

        private void JumpToOffsetAddress(int address) {
            emulator.instructionPointer = (short)(address + emulator.registers[0]);
        }

        private void SetAddressRegister(short address) {
            emulator.addressRegister = address;
        }

        private void SkipIfNotEqualRegister(byte regx, byte regy) {
            if (emulator.registers[regx] != emulator.registers[regy])
                emulator.instructionPointer += 2;
        }

        private void SubtractRegisterFromRegister(byte regx, byte regy) {
            byte valx = emulator.registers[regx];
            byte valy = emulator.registers[regy];
            emulator.registers[0xF] = (byte)(valx > valy ? 0 : 1);
            emulator.registers[regx] = (byte)(valx - valy);
        }

        private void BitshiftRegisterRight(byte regx) {
            //FIXME: doesn't set the carry flag
            byte newValue = (byte)(emulator.registers[regx] >> 1);
            emulator.registers[regx] = newValue;
        }

        private void SubtractReverseRegisters(byte regx, byte regy) {
            byte valx = emulator.registers[regx];
            byte valy = emulator.registers[regy];
            emulator.registers[0xF] = (byte)(valy > valx ? 0 : 1);
            emulator.registers[regx] = (byte)(valy - valx);
        }

        private void BitshiftRegisterLeft(byte regx) {
            //FIXME: doesn't set the carry flag
            byte newValue = (byte)(emulator.registers[regx] << 1);
            emulator.registers[regx] = newValue;
        }

        private void AddRegisterToRegister(byte regx, byte regy) {
            byte newValue = (byte)(emulator.registers[regx] + emulator.registers[regy]);
            emulator.registers[regx] = newValue;
        }

        private void SetRegisterXorRegister(byte regx, byte regy) {
            byte newValue = (byte)(emulator.registers[regx] & emulator.registers[regy]);
            emulator.registers[regx] = newValue;
        }

        private void SetRegisterAndRegister(byte regx, byte regy) {
            byte newValue = (byte)(emulator.registers[regx] & emulator.registers[regy]);
            emulator.registers[regx] = newValue;
        }

        private void SetRegisterToRegister(byte regx, byte regy) {
            emulator.registers[regx] = emulator.registers[regy];
        }

        private void SetRegisterOrRegister(byte regx, byte regy) {
            byte newValue = (byte)(emulator.registers[regx] | emulator.registers[regy]);
            emulator.registers[regx] = newValue;
        }

        private void SetRegisterToConst(byte register, byte number) {
            emulator.registers[register] = number;
        }

        private void AddConstToRegister(byte register, byte number) {
            byte newValue = (byte)(emulator.registers[register] + number);
            emulator.registers[register] = newValue;
        }

        private void SkipIfEqualRegister(byte regx, byte regy) {
            if (emulator.registers[regx] == emulator.registers[regy])
                emulator.instructionPointer += 2;
        }

        private void SkipIfNotEqualConst(byte register, byte value) {
            if (emulator.registers[register] != value)
                emulator.instructionPointer += 2;
        }

        private void SkipIfEqualConst(byte register, byte value) {
            if (emulator.registers[register] == value)
                emulator.instructionPointer += 2;
        }

        private void CallSubRoutine(short address) {
            emulator.stack.Push(emulator.instructionPointer);
            JumpToAddress(address);
        }

        private void JumpToAddress(short address) {
            emulator.instructionPointer = address;
        }

        private void ReturnFromSubroutine() {
            JumpToAddress(emulator.stack.Pop());
        }

        private void ClearDisplay() {
            emulator.ram.Write(new byte[0xFF], 0xF00);
            emulator.updated = true;
        }

        private void NoOp() {
            return;
        }
    }
}

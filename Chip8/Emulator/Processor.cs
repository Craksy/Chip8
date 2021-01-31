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
    /// This is a pretty big class. CPUs are complex things and they are responsible for pretty much every action
    /// that can be done to data within a computer.
    /// As a result this can seem a bit bloated when scrolling through, even though there are really just
    /// two main parts; the instruction decoder followed by a series of instruction methods.
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
            //this is a bit messy, ngl...

            // First and last 4 bits are used to determine the kind of instruction.
            // Also acts as argument for a few instructions.
            byte firstNibble = (byte)(instruction >> 12 & 0xF);
            byte lastNibble = (byte)(instruction & 0xF);

            // Registers are refered to by the 2nd and 3rd nibbles in all instructions.
            byte regx = (byte)(instruction >> 8 & 0xF);
            byte regy = (byte)(instruction >> 4 & 0xF);

            // Constants and addresses are represented by the 
            // first 8 and 12 bits respectively
            byte number = (byte)(instruction & 0xFF);
            short address = (short)(instruction & 0xFFF);

            // fingers crossed that i got this right so 
            // that I'll never have to touch it again.
            switch (firstNibble) {
                case 0x0:
                    switch (address) {
                        case 0x000:
                            NoOp();
                            break;
                        case 0x0E0:
                            ClearDisplay();
                            break;
                        case 0x0EE:
                            ReturnFromSubroutine();
                            break;
                    }
                    break;
                case 0x1:
                    JumpToAddress(address);
                    break;
                case 0x2:
                    CallSubRoutine(address);
                    break;
                case 0x3:
                    SkipIfEqualConst(regx, number);
                    break;
                case 0x4:
                    SkipIfNotEqualConst(regx, number);
                    break;
                case 0x5:
                    SkipIfEqualRegister(regx, regy);
                    break;
                case 0x6:
                    SetRegisterToConst(regx, number);
                    break;
                case 0x7:
                    AddConstToRegister(regx, number);
                    break;
                case 0x8:
                    switch (lastNibble) {
                        case 0x0:
                            SetRegisterToRegister(regx, regy);
                            break;
                        case 0x1:
                            SetRegisterOrRegister(regx, regy);
                            break;
                        case 0x2:
                            SetRegisterAndRegister(regx, regy);
                            break;
                        case 0x3:
                            SetRegisterXorRegister(regx, regy);
                            break;
                        case 0x4:
                            AddRegisterToRegister(regx, regy);
                            break;
                        case 0x5:
                            SubtractRegisterFromRegister(regx, regy);
                            break;
                        case 0x6:
                            BitshiftRegisterRight(regx);
                            break;
                        case 0x7:
                            SubtractReverseRegisters(regx, regy);
                            break;
                        case 0xE:
                            BitshiftRegisterLeft(regx);
                            break;
                        default:
                            throw new Exception("Unknown instruction " + instruction);
                    }
                    break;
                case 9:
                    SkipIfNotEqualRegister(regx, regy);
                    break;
                case 0xA:
                    SetAddressRegister(address);
                    break;
                case 0xB:
                    JumpToOffsetAddress(address);
                    break;
                case 0xC:
                    SetRegisterAndRandom(regx, number);
                    break;
                case 0xD:
                    DrawSprite2(regx, regy, lastNibble);
                    break;
                case 0xE:
                    if (number == 0x9E)
                        SkipIfKeyPressed(regx);
                    else if (number == 0xA1)
                        SkipIfNotKeyPressed(regx);
                    break;
                case 0xF:
                    switch (number) {
                        case 0x07:
                            SetRegisterDelayTimer(regx);
                            break;
                        case 0x0A:
                            WaitForKeyPress(regx);
                            break;
                        case 0x15:
                            SetDelayTimerRegister(regx);
                            break;
                        case 0x18:
                            SetSoundTimerRegister(regx);
                            break;
                        case 0x1E:
                            AddRegisterToAddressRegister(regx);
                            break;
                        case 0x29:
                            PointAddressRegisterToCharacter(regx);
                            break;
                        case 0x33:
                            BinaryCodedDecimal(regx);
                            break;
                        case 0x55:
                            DumpRegisters(regx);
                            break;
                        case 0x65:
                            LoadToRegisters(regx);
                            break;
                        default:
                            throw new Exception("Unknown instruction " + instruction);
                    }
                    break;

                default:
                    throw new Exception("Unknown instruction " + instruction);
            }

        }

        ////////////////////////////////////////////////////////////
        ///////////// Instruction implementations //////////////////
        ////////////////////////////////////////////////////////////
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
            //TODO: this could be done a lot smoother with a loop
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

        private void DrawSprite(byte regx, byte regy, byte height) {
            //TODO: Rewrite this method. It's a mess.
            short memPosition = emulator.addressRegister;
            byte xpos = emulator.registers[regx];
            byte ypos = emulator.registers[regy];
            int bitOffset = xpos % 8;
            byte collision = 0;

            // For every `row` in range `height` read one byte from memory
            // starting at the position pointed to by the address register.
            for (int row = 0; row < height; row++) {
                if (ypos + row >= 32) // if we try to draw beyond the frame buffer.
                    break;

                int firstByte = 0xF00 + ((ypos + row) * 8) + (xpos / 8); // memory address of the (firs) byte of the framebuffer
                byte spriteData = emulator.ram.Read(memPosition + row);

                // If the bit offset is not 0, it means that two bytes of the
                // framebuffer will be affected.
                if (bitOffset != 0) {
                    //bitmasks to capture the two offset parts of a byte
                    byte mask1 = (byte)(0xFF >> bitOffset);
                    byte mask2 = (byte)~mask1;

                    //making sure that we don't attempt to read beyond the last byte of memory.
                    int numbytes = firstByte < 0xFFF ? 2 : 1;
                    byte[] bufferBytes = emulator.ram.Read(firstByte, numbytes);


                    // If they share any bits between them, there's going to be a 
                    // collision when XOR'ed
                    byte actualByte = numbytes > 1 ? (byte)((bufferBytes[0] & mask1) | (bufferBytes[1] & mask2)) : (byte)(bufferBytes[0] & mask1);
                    if ((actualByte & spriteData) != 0)
                        collision = 1;

                    //Modify the bytes and write them back to memory.
                    bufferBytes[0] ^= (byte)((spriteData >> bitOffset) & 0xFF);
                    if (numbytes == 2)
                        bufferBytes[1] ^= (byte)((spriteData << bitOffset) & 0xFF);
                    emulator.ram.Write(bufferBytes, firstByte);
                } else {
                    byte[] b = new byte[1];
                    b[0] = (byte)(emulator.ram.Read(firstByte) ^ spriteData);
                    emulator.ram.Write(b, firstByte);
                }

                emulator.registers[0xF] = collision;
                emulator.updated = true;
            }
        }


        private void SetRegisterAndRandom(byte regx, byte number) {
            byte r = (byte)(random.Next(0, 255) & number);
            emulator.registers[regx] = r;
        }

        private void JumpToOffsetAddress(int address) {
            throw new NotImplementedException();
        }

        private void SetAddressRegister(short address) {
            emulator.addressRegister = address;
        }

        private void SkipIfNotEqualRegister(byte regx, byte regy) {
            if (emulator.registers[regx] != emulator.registers[regy])
                emulator.instructionPointer += 2;
        }

        private void SubtractRegisterFromRegister(byte regx, byte regy) {
            byte newValue = (byte)(emulator.registers[regx] - emulator.registers[regy]);
            emulator.registers[regx] = newValue;
        }

        private void BitshiftRegisterRight(byte regx) {
            byte newValue = (byte)(emulator.registers[regx] >> 1);
            emulator.registers[regx] = newValue;
        }

        private void SubtractReverseRegisters(byte regx, byte regy) {
            throw new NotImplementedException();
        }

        private void BitshiftRegisterLeft(byte regx) {
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

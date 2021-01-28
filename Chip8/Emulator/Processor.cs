using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Diagnostics;



namespace Chip8.Emulator
{
    class Processor
    {
        private Emulator emulator;
        private Random random;

        public Processor(Emulator emulator)
        {
            this.emulator = emulator;
            random = new Random(1234); //initializing with a seed for debugging purposes.
        }

        public void DecodeInstruction(short instruction)
        {
            // yeah, this is a bit messy, ngl...
            // perhaps i should just pass a byte[] instead. (or at least Int16)

            // First and last 4 bits are used to determine
            // the kind of instruction.
            byte firstNibble = (byte) (instruction >> 12 & 0xF);
            byte lastNibble = (byte) (instruction & 0xF);

            // Registers are refered to by the 2nd and 3rd nibbles in all instructions.
            byte regx = (byte) (instruction >> 8 & 0xF);
            byte regy = (byte) (instruction >> 4 & 0xF);

            // Constants and addresses are represented by the 
            // last 8 and 12 bits respectively
            byte number = (byte)(instruction & 0xFF);
            short address = (short) (instruction & 0xFFF);

            // fingers crossed that i got this right so 
            // that I'll never have to touch it again.
            // TODO: i might as well just compare the entire 
            // instruction instead of having nested switch statements.
            switch (firstNibble)
            {
                case 0x0:
                    switch (address)
                    {
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
                    switch (lastNibble)
                    {
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
                    DrawSprite(regx, regy, lastNibble);
                    break;
                case 0xE:
                    if (number == 0x9E)
                        SkipIfKeyPressed(regx);
                    else if (number == 0xA1)
                        SkipIfNotKeyPressed(regx);
                    break;
                case 0xF:
                    switch (number)
                    {
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
                            DumpRegistersToMemory(regx);
                            break;
                        case 0x65:
                            LoadMemoryIntoRegisters(regx);
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
        private void SetRegisterDelayTimer(byte regx)
        {
            throw new NotImplementedException();
        }

        private void WaitForKeyPress(byte regx)
        {
            throw new NotImplementedException();
        }

        private void SetDelayTimerRegister(byte regx)
        {
            throw new NotImplementedException();
        }

        private void SetSoundTimerRegister(byte regx)
        {
            throw new NotImplementedException();
        }

        private void AddRegisterToAddressRegister(byte regx)
        {
            emulator.addressRegister += emulator.registers[regx];
        }

        private void PointAddressRegisterToCharacter(byte regx)
        {
            throw new NotImplementedException();
        }

        private void BinaryCodedDecimal(byte regx)
        {
            throw new NotImplementedException();
        }

        private void DumpRegistersToMemory(byte regx)
        {
            throw new NotImplementedException();
        }

        private void LoadMemoryIntoRegisters(byte regx)
        {
            throw new NotImplementedException();
        }

        private void SkipIfNotKeyPressed(byte regx)
        {
            throw new NotImplementedException();
        }

        private void SkipIfKeyPressed(byte regx)
        {
            throw new NotImplementedException();
        }

        private void DrawSprite(byte regx, byte regy, byte height)
        {
            /*
             * Draw a sprite to the display at the X,Y position given by
             * Registers `regx` and `regy`.
             * The sprite is 8 pixels wide, and `height` pixels tall, and is read
             * from memory starting at the position pointed to by the address register.
             * ie. each row of the sprite is one byte read from memory.
             * 
             * The sprite is XOR'ed on top of the existing framebuffer meaning that if
             * we try to set a bit that's already set, it will be flipped to 0 instead.
             * If any bits are unset this way, set the carry flag.
             * This is used for collision detection.
             */

            short memPosition = emulator.addressRegister;
            byte xpos = emulator.registers[regx];
            byte ypos = emulator.registers[regy];
            int bitOffset = xpos % 8;
            byte collision = 0;

            Debug.Print("X,Y: {0}, {1}", xpos, ypos);
            Debug.Print("offset, height: {0}, {1}", bitOffset, height);
            Debug.Print("------------------------------");

            // For every `row` in range `height` read one byte from memory
            // starting at the position pointed to by the address register.
            for (int row = 0; row < height; row++)
            {
                if (ypos + row >= 32) // if we try to draw beyond the frame buffer.
                    break;

                int firstByte = 0xF00 + ((ypos+row) * 8) + (xpos / 8); // memory address of the (firs) byte of the framebuffer
                byte spriteData = emulator.ram.Read(memPosition + row)[0];

                // If the bit offset is not 0, it means that two bytes of the
                // framebuffer will be affected.
                if(bitOffset != 0) {
                    //bitmasks to capture the two offset parts of a byte
                    byte mask1 = (byte)(0xFF >> bitOffset);
                    byte mask2 = (byte)~mask1;

                    byte[] bufferBytes = emulator.ram.Read(firstByte, 2);
                    byte actualByte = (byte)((bufferBytes[0] & mask1) | (bufferBytes[1] & mask2));

                    // If they share any bits between them, there's going to be a 
                    // collision when XOR'ed
                    if ((actualByte & spriteData) != 0)
                        collision = 1;

                    //Modify the bytes and write them back to memory.
                    bufferBytes[0] ^= (byte)((spriteData >> bitOffset) & 0xFF);
                    bufferBytes[1] ^= (byte)((spriteData << bitOffset) & 0xFF);
                    emulator.ram.Write(bufferBytes, firstByte);
                }
                else // otherwise just modify the one byte at the current index
                {
                    byte[] b = new byte[1];
                    b[0] = (byte)(emulator.ram.Read(firstByte)[0] ^ spriteData);
                    emulator.ram.Write(b, firstByte);
                }

                emulator.registers[0xF] = collision;
            }
        }

        private void SetRegisterAndRandom(byte regx, byte number)
        {
            byte r = (byte) (random.Next(0, 255) & number);
            emulator.registers[regx] = r;
        }

        private void JumpToOffsetAddress(int address)
        {
            throw new NotImplementedException();
        }

        private void SetAddressRegister(short address)
        {
            emulator.addressRegister = address;
        }

        private void SkipIfNotEqualRegister(byte regx, byte regy)
        {
            if (emulator.ReadRegister(regx) != emulator.ReadRegister(regy))
                emulator.instructionPointer += 2;
        }

        private void SubtractRegisterFromRegister(byte regx, byte regy)
        {
            byte newValue = (byte)(emulator.ReadRegister(regx) - emulator.ReadRegister(regy));
            emulator.WriteRegister(regx, newValue);
        }

        private void BitshiftRegisterRight(byte regx)
        {
            byte newValue = (byte)(emulator.ReadRegister(regx) >> 1);
            emulator.WriteRegister(regx, newValue);
        }

        private void SubtractReverseRegisters(byte regx, byte regy)
        {
            throw new NotImplementedException();
        }

        private void BitshiftRegisterLeft(byte regx)
        {
            byte newValue = (byte)(emulator.ReadRegister(regx) << 1);
            emulator.WriteRegister(regx, newValue);
        }

        private void AddRegisterToRegister(byte regx, byte regy)
        {
            byte newValue = (byte)(emulator.ReadRegister(regx) + emulator.ReadRegister(regy));
            emulator.WriteRegister(regx, newValue);
        }

        private void SetRegisterXorRegister(byte regx, byte regy)
        {
            byte newValue = (byte)(emulator.ReadRegister(regx) & emulator.ReadRegister(regy));
            emulator.WriteRegister(regx, newValue);
        }

        private void SetRegisterAndRegister(byte regx, byte regy)
        {
            throw new NotImplementedException();
        }

        private void SetRegisterToRegister(byte regx, byte regy)
        {
            emulator.WriteRegister(regx, emulator.ReadRegister(regy));
        }

        private void SetRegisterOrRegister(byte regx, byte regy)
        {
            byte newValue = (byte)(emulator.ReadRegister(regx) | emulator.ReadRegister(regy));
            emulator.WriteRegister(regx, newValue);
        }

        private void SetRegisterToConst(byte register, byte number)
        {
            emulator.WriteRegister(register, number);
        }

        private void AddConstToRegister(byte register, byte number)
        {
            byte newValue = (byte)(emulator.ReadRegister(register) + number);
            emulator.WriteRegister(register, newValue);
        }

        private void SkipIfEqualRegister(byte regx, byte regy)
        {
            if (emulator.ReadRegister(regx) == emulator.ReadRegister(regy))
                emulator.instructionPointer += 2;
        }

        private void SkipIfNotEqualConst(byte register, byte value)
        {
            if (emulator.ReadRegister(register) != value)
                emulator.instructionPointer += 2;
        }

        private void SkipIfEqualConst(byte register, byte value)
        {
            if (emulator.ReadRegister(register) == value)
                emulator.instructionPointer += 2;
        }

        private void CallSubRoutine(short address)
        {
            emulator.stack.Push(emulator.instructionPointer);
            JumpToAddress(address);
        }

        private void JumpToAddress(short address)
        {
            emulator.instructionPointer = address;
        }

        private void ReturnFromSubroutine()
        {
            JumpToAddress(emulator.stack.Pop());
        }

        private void ClearDisplay()
        {
            emulator.ram.Write(new byte[0xFF], 0xF00);
        }

        private void NoOp()
        {
            return;
        }
    }
}

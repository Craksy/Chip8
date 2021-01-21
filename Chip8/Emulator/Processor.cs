using System;
using System.Collections.Generic;
using System.Text;


namespace Chip8.Emulator
{
    class Processor
    {
        private Emulator emulator;

        public Processor(Emulator emulator)
        {
            this.emulator = emulator;
        }

        private byte[] ReadRegister(int register)
        {
            return emulator.registers[register].GetValue();
        }

        private void WriteRegister(int register, byte[] data)
        {
            emulator.registers[register].SetValue(data);
        }

        public void DecodeInstruction(int instruction)
        {
            // yeah, this is a bit messy, ngl...
            // perhaps i should just pass a byte[] instead.

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
            int address = instruction & 0xFFF;

            // fingers crossed that i got this right so 
            // that I'll never have to touch it again.
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
            throw new NotImplementedException();
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

        private void DrawSprite(byte regx, byte regy, byte lastNibble)
        {
            throw new NotImplementedException();
        }

        private void SetRegisterAndRandom(byte regx, byte number)
        {
            throw new NotImplementedException();
        }

        private void JumpToOffsetAddress(int address)
        {
            throw new NotImplementedException();
        }

        private void SetAddressRegister(int address)
        {
            throw new NotImplementedException();
        }

        private void SkipIfNotEqualRegister(byte regx, byte regy)
        {
            throw new NotImplementedException();
        }

        private void SubtractRegisterFromRegister(byte regx, byte regy)
        {
            throw new NotImplementedException();
        }

        private void BitshiftRegisterRight(byte regx)
        {
            throw new NotImplementedException();
        }

        private void SubtractReverseRegisters(byte regx, byte regy)
        {
            throw new NotImplementedException();
        }

        private void BitshiftRegisterLeft(byte regx)
        {
            throw new NotImplementedException();
        }

        private void AddRegisterToRegister(byte regx, byte regy)
        {
            throw new NotImplementedException();
        }

        private void SetRegisterXorRegister(byte regx, byte regy)
        {
            throw new NotImplementedException();
        }

        private void SetRegisterAndRegister(byte regx, byte regy)
        {
            throw new NotImplementedException();
        }

        private void SetRegisterToRegister(byte regx, byte regy)
        {
            throw new NotImplementedException();
        }

        private void SetRegisterOrRegister(byte regx, byte regy)
        {
            throw new NotImplementedException();
        }

        private void SetRegisterToConst(byte regx, byte number)
        {
            throw new NotImplementedException();
        }

        private void AddConstToRegister(byte regx, byte number)
        {
            throw new NotImplementedException();
        }

        private void SkipIfEqualRegister(byte regx, byte regy)
        {
            throw new NotImplementedException();
        }

        private void SkipIfNotEqualConst(byte regx, byte number)
        {
            throw new NotImplementedException();
        }

        private void SkipIfEqualConst(byte regx, byte number)
        {
            throw new NotImplementedException();
        }

        private void CallSubRoutine(int address)
        {
            throw new NotImplementedException();
        }

        private void JumpToAddress(int address)
        {
            throw new NotImplementedException();
        }

        private void ReturnFromSubroutine()
        {
            throw new NotImplementedException();
        }

        private void ClearDisplay()
        {
            throw new NotImplementedException();
        }

        private void NoOp()
        {
            throw new NotImplementedException();
        }
    }
}

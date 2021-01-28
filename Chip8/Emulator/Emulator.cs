using System;
using System.Collections.Generic;
using System.Text;

using Chip8.Emulator.Memory;

namespace Chip8.Emulator
{
    class Emulator
    {
        // use public methods instead of exposing these members directly?
        public Ram ram;
        public Stack<short> stack;
        public bool updated;
        
        public byte[] registers;
        public short addressRegister;
        private Processor processor;
        public short instructionPointer;
        private bool run;

        public Emulator()
        {
            ram = new Ram(0x1000);
            //REVIEW: Could probably make this a bit larger.
            stack = new Stack<short>(128);
            registers = new byte[16];
            registers.Initialize();
            addressRegister = 0;
            processor = new Processor(this);
            instructionPointer = 0x200; // Programs traditionally start at mem addr 0x200
            run = true;
        }

        public void Clock()
        {
            byte[] nextInstruction = FetchNextInstruction();
            processor.DecodeInstruction(BitConverter.ToInt16(nextInstruction));
        }

        public byte[] GetFrameBuffer()
        {
            updated = false;
            // There's some off-by-one shit going on here..
            // I feel like the range should be 0xF00 + 0xFF
            // but that leaves me 1 byte short...
            return ram.Read(0xF00, 0x100);
        }

        // REVIEW: is this retarded? just access them via `emulator.registers[N]` instead?
        // I'm struggling to see any advantages of this approach...
        public byte ReadRegister(byte register)
        {
            return registers[register];
        }

        public void WriteRegister(byte register, byte data)
        {
            registers[register] = data;
        }

        private void SwitchEndianess(byte[] data)
        {
            for(int i = 0; i<data.Length-2; i += 2)
            {
                var tmp = data[i + 1];
                data[i + 1] = data[i];
                data[i] = tmp;
            }
        }

        public void ReadROMFromFile(string path)
        {
            byte[] data = System.IO.File.ReadAllBytes(path);
            //SwitchEndianess(data);
            ram.Write(data, 0x200);
        }

        private byte[] FetchNextInstruction()
        {
            byte[] instruction = ram.Read(instructionPointer, 2);

            //swap the two bytes to read them as big endian.
            byte tmp = instruction[0];
            instruction[0] = instruction[1];
            instruction[1] = tmp;
            
            instructionPointer += 2;
            updated = true;
            return instruction;
        }

    }
}

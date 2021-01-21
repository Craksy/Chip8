using System;
using System.Collections.Generic;
using System.Text;

using Chip8.Emulator.Memory;

namespace Chip8.Emulator
{
    class Emulator
    {
        // use public methods instead of exposing this members directly?
        public Ram ram;
        public Stack<byte> stack;
        public Register[] registers;
        public Register addressRegister;
        public bool updated;
        
        private Processor processor;
        private int instructionPointer;
        private bool run;

        public Emulator()
        {
            ram = new Ram(0x1000);
            stack = new Stack<byte>(96); //REVIEW: Could probably make this a bit larger.
            registers = new Register[16];
            registers.Initialize();
            addressRegister = new Register(2);
            processor = new Processor(this);
        }

        public void Start()
        {
            instructionPointer = 0;
            run = true;
            while (run)
            {
                int instruction = BitConverter.ToInt32(FetchNextInstruction());
                processor.DecodeInstruction(instruction);
            }
        }

        public byte[] GetFrameBuffer()
        {
            updated = false;
            return ram.Read(0xF00, 0xFF);
        }

        public void LoadProgram(byte[] data)
        {
            // possibly do some error handling here, to ensure that we
            // don't try to load a program that's too large.
            ram.Write(data, 0x200);
        }

        // REVIEW: Not really sure if this should be it's own function.
        private byte[] FetchNextInstruction()
        {
            byte[] instruction = ram.Read(instructionPointer, 2);
            instructionPointer += 2;
            updated = true;
            return instruction;
        }

    }
}

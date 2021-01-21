using System;
using System.Collections.Generic;
using System.Text;

using Chip8.Emulator.Memory;

namespace Chip8.Emulator
{
    class Emulator
    {
        public Ram ram;
        public Stack<byte> stack;
        public Register[] registers;
        public Register addressRegister;
        private Processor processor;
        private int instructionPointer;

        public Emulator()
        {
            ram = new Ram(0x1000);
            stack = new Stack<byte>(96);
            registers = new Register[16];
            registers.Initialize();
            addressRegister = new Register(2);
            instructionPointer = 0;
            processor = new Processor(this);
        }
    }
}

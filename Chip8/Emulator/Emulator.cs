using System;
using System.Collections.Generic;
using System.Text;

using Chip8.Emulator.Memory;

namespace Chip8.Emulator
{
    class Emulator
    {

        /// <summary>
        /// The "heart" of the CHIP-8 emulation.
        /// This class holds the registers, ram, timers and CPU, and ties it all together to a (hopefully)
        /// functional system. The only things not managed directly by this class is display and input, 
        /// which is handled by the MonoGame library in the main `Game` class.
        /// </summary>

        public Ram ram;
        public Stack<short> stack;
        public bool updated;
        
        public byte[] registers;
        public short addressRegister;
        private Processor processor;
        public short instructionPointer;
        private bool run;

        public double delayTimer, soundTimer;

        public Emulator()
        {
            ram = new Ram(0x1000);
            stack = new Stack<short>(128);
            registers = new byte[16];
            registers.Initialize();
            processor = new Processor(this);
            instructionPointer = 0x200; // Programs traditionally start at mem addr 0x200
            addressRegister = 0;
            delayTimer = 0;
            soundTimer = 0;
            run = true;
        }

        /// <summary>
        /// Update timers and do a single clock cycle.
        /// </summary>
        /// <param name="deltaTime">
        /// time since last call to Update (hopefully this approximates the time since last call to Clock as well)
        /// </param>
        public void Clock(double deltaTime)
        {
            if (delayTimer > 0)
                delayTimer -= deltaTime;
            if (soundTimer > 0)
                soundTimer -= deltaTime;

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

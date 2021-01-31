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
        public bool[] keystates;

        
        public byte[] registers;
        public short addressRegister;
        public short instructionPointer;
        private Processor processor;

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
        }

        /// <summary>
        /// Update timers and keystates before doing a clock cycle.
        /// </summary>
        /// <param name="deltaTime">
        /// time since last call to Update (hopefully this approximates the time since last call to Clock as well)
        /// </param>
        public void Clock(double deltaTime, bool[] keystates)
        {
            this.keystates = keystates;

            if (delayTimer > 0)
                delayTimer -= deltaTime;
            if (soundTimer > 0)
                soundTimer -= deltaTime;

            byte[] nextInstruction = FetchNextInstruction();
            processor.DecodeInstruction(BitConverter.ToInt16(nextInstruction));
        }

        public byte[] GetFrameBuffer() { return ram.Read(0xF00, 0x100); }

        private void SwitchEndianess(byte[] data)
        {
            // TODO: verify that this method is not needed and delete.
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

        public void LoadFontData(string path) {
            byte[] data = System.IO.File.ReadAllBytes(path);
            ram.Write(data, 0);
        }

        private byte[] FetchNextInstruction()
        {
            byte[] instruction = ram.Read(instructionPointer, 2);

            //swap the two bytes to read them as big endian.
            //i'm doing this here, because I'm not sure if we want to be swapping endianess on data bytes.
            byte tmp = instruction[0];
            instruction[0] = instruction[1];
            instruction[1] = tmp;
            
            instructionPointer += 2;
            updated = true;
            return instruction;
        }
    }
}

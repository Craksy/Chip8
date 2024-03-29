﻿using System;
using System.Collections.Generic;


namespace Chip8.Emulator
{
    class Emulator
    {

        /// <summary>
        /// This class holds the registers, ram, timers and CPU, and ties it all together to a (hopefully)
        /// functional system. The only things not managed directly by this class is display and input, 
        /// which is handled by the MonoGame library in the main `Game` class.
        /// </summary>

        public Ram ram;
        private Processor processor;
        public Stack<short> stack;
        public byte[] registers;
        public short addressRegister;
        public short instructionPointer;
        public double delayTimer, soundTimer;

        public bool updated;
        public byte? awaitKeypress; //null if not awaiting keypress, points to the destination register otherwise
        public bool[] keystates;

        public Emulator() {
            ram = new Ram(0x1000);
            stack = new Stack<short>(128);
            registers = new byte[16];
            processor = new Processor(this);
            instructionPointer = 0x200; 
            addressRegister = 0;
            delayTimer = 0;
            soundTimer = 0;
        }

        /// <summary>
        /// Update timers and keystates before doing a clock cycle.
        /// </summary>
        /// <param name="deltaTime">
        /// time since last call to Update
        /// </param>
        public void Clock(double deltaTime, bool[] keystates) {
            this.keystates = keystates;

            if (delayTimer > 0)
                delayTimer -= deltaTime*60;
            if (soundTimer > 0)
                soundTimer -= deltaTime*60;

            if (awaitKeypress == null) {
                byte[] nextInstruction = FetchNextInstruction();
                processor.DecodeInstruction(BitConverter.ToInt16(nextInstruction));
            } else {
                for (int i = 0; i < 16; i++) {
                    if (keystates[i]) {
                        registers[(int)awaitKeypress] = (byte)i;
                        awaitKeypress = null;
                    }
                }
            }
        }

        public byte[] GetFrameBuffer() { return ram.Read(0xF00, 0x100); }

        public void ReadROMFromFile(string path) {
            byte[] data = System.IO.File.ReadAllBytes(path);

            ram.Write(data, 0x200);
        }

        /// <summary>
        /// REVIEW: Let this be a constructor argument instead?
        /// Load font data for character 0-F from a binary file and store them in memory starting at index 0.
        /// </summary>
        /// <param name="path">path to the font data file.</param>
        public void LoadFontData(string path) {
            byte[] data = System.IO.File.ReadAllBytes(path);
            ram.Write(data, 0);
        }

        private byte[] FetchNextInstruction() {
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

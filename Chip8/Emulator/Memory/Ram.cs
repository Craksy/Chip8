using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8.Emulator.Memory
{
    class Ram
    {
        private byte[] memory;
        public Ram(int size)
        {
            memory = new byte[size];
        }

        public byte[] Read(int offset, int count)
        {
            byte[] result = memory[offset..(offset + count)];
            return result;
        }

        public byte Read(int offset) { return memory[offset]; }

        public void Write(byte[] bytes, int offset)
        {
            for (int i = 0; i < bytes.Length; i++) {
                if (offset + i > 0xFFF)
                    break;
                memory[offset + i] = bytes[i];
            }
        }

        public void Write(byte b, int offset) { memory[offset] = b; }
    }
}

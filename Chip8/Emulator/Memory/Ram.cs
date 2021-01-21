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

        public byte[] Read(int offset, int bytes = 1)
        {
            byte[] result = memory[new Range(offset, offset + bytes)];
            return result;
        }

        public void Write(byte[] bytes, int offset)
        {
            for (int i = 0; i < bytes.Length; i++)
                memory[offset + i] = bytes[i];
        }
    }
}

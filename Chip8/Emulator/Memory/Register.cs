using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8.Emulator.Memory
{
    class Register
    {
        //REVIEW: Just use an int here instead, since it's just going to hold single values anyway?
        private byte[] memory; 

        public Register(int bytes = 1)
        {
            memory = new byte[bytes];
        }

        public byte[] GetValue()
        {
            return memory;
        }

        public void SetValue(byte[] data)
        {
            memory = data;
        }
    }
}

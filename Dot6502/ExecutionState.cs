using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502
{
    public enum StateFlag
    {
        Negative = 7,
        Overflow = 6,
        ignored = 5,
        Break = 4,
        Decimal = 3,
        Interrupt = 2,
        Zero = 1,
        Carry = 0
    }

    public class MemoryMap
    {
        public ushort ZeroPage = 0x0000;
        public ushort StackEnd = 0x0100;
        public ushort StackBegin = 0x01FF;
        public ushort Screen = 0x0400; //40*25 characters
        public ushort ProgramStart = 0x1000;
    }

    public class ExecutionState
    {
        //registers:
        public ushort PC; //program counter
        public byte AC; //accumulator
        public byte X; //X register
        public byte Y; //Y register
        public byte SR; //status register [NV-BDIZC]
        public byte SP; //stack pointer
        public byte[] Memory = new byte[65536];
        //FIXME: Consider representing as 256x256 image of RGB8,8,4-colors

        public bool SetFlag(StateFlag flag)
        {
            var prev = TestFlag(flag);
            SR |= (byte)flag;
            return prev;
        }

        public bool ClearFlag(StateFlag flag)
        {
            var prev = TestFlag(flag);
            SR &= (byte)~(flag);
            return prev;
        }

        public bool TestFlag(StateFlag flag)
        {
            return (SR & (sbyte)flag) > 0;
        }

        public byte ReadByte(ushort pos)
        {
            return Memory[pos];
        }

        public sbyte ReadSignedByte(ushort pos)
        {
            unchecked
            {
                return (sbyte)Memory[pos];
            }
        }

        public ushort ReadWord(ushort pos)
        {
            return (ushort)(Memory[pos] << 8 + Memory[pos + 1]);
        }

        public short ReadSignedWord(ushort pos)
        {
            unchecked
            {
                return (short)(Memory[pos] << 8 + Memory[pos + 1]);
            }
        }

    }
}

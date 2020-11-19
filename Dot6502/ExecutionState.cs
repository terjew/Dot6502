using System;
using System.Collections.Generic;
using System.Linq;
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

        private Random random;
        private List<MemoryWatch> watches = new List<MemoryWatch>();

        public ExecutionState()
        {
            random = new Random();
        }

        public void AddMemoryWatch(MemoryWatch watch)
        {
            watches.Add(watch);
        }

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

        public void WriteByte(ushort pos, byte value)
        {
            Memory[pos] = value;
            foreach (var watch in watches.Where(w => w.IsInside(pos)))
            {
                watch.Callback(pos, value);
            }
        }

        public ushort ReadWord(ushort pos)
        {
            return (ushort)((Memory[pos + 1] << 8) + Memory[pos]);
        }

        public void StepExecution()
        {
            var instruction = Decoder.DecodeInstruction(this);
            var offset = instruction.Execute(this);
            PC = (ushort)(PC + offset);
            //Update the random generator number:
            WriteByte(0x00FE, (byte)(random.Next(256)));
        }
    }
}

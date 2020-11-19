using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    public class ExecutionState
    {
        public byte[] Memory = new byte[65536];

        //registers:
        public ushort PC; //program counter
        public byte AC; //accumulator
        public byte X; //X register
        public byte Y; //Y register
        public byte SR; //status register [NV-BDIZC]
        public ushort SP = 0xFF; //stack pointer

        private ushort stackBase = 0x0100;

        private List<MemoryWatch> watches = new List<MemoryWatch>();

        public ExecutionState()
        {
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

        public void WriteWord(ushort pos, ushort word)
        {
            Memory[pos] = (byte)(word & 0xFF);
            Memory[pos + 1] = (byte)(word >> 8);
        }

        public void Push(byte value)
        {
            if (SP == 0) throw new StackOverflowException();
            WriteByte((ushort)(stackBase + SP), value);
            SP--;
        }

        public void Push(ushort word)
        {
            if (SP <= 1) throw new StackOverflowException();
            SP--;
            WriteWord((ushort)(stackBase + SP), word);
            SP--;
        }

        public byte PopByte()
        {
            if (SP == 0) throw new StackOverflowException();
            SP++;
            return ReadByte((ushort)(stackBase + SP));
        }


        public ushort PopWord()
        {
            if (SP == 0) throw new StackOverflowException();
            SP++;
            var word = ReadWord((ushort)(stackBase + SP));
            SP++;
            return word;
        }

        public void StepExecution()
        {
            var instruction = Decoder.DecodeInstruction(this);
            var offset = instruction.Execute(this);
            PC = (ushort)(PC + offset);
        }

        public void LoadHexFile(string filename)
        {
            var hex = File.ReadAllText(filename);
            LoadHexString(hex);
        }

        public void SetNegativeFlag(byte value)
        {
            if (value > 0x7f) SetFlag(StateFlag.Negative);
            else ClearFlag(StateFlag.Negative);
        }

        public void SetZeroFlag(byte value)
        {
            if (value == 0) SetFlag(StateFlag.Zero);
            else ClearFlag(StateFlag.Zero);
        }

        public void LoadHexString(string hexString)
        {
            var byteStrings = hexString.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var bytes = byteStrings.Select(hex => Convert.ToInt32(hex, 16)).Select(i => (byte)i);
            var programAddressBytes = bytes.Take(2).ToArray();
            ushort programAddress = (ushort)((programAddressBytes[1] << 8) | programAddressBytes[0]);
            var programBytes = bytes.Skip(2).ToArray();
            LoadProgram(programBytes, programAddress);
        }

        public void LoadProgram(byte[] program, ushort location)
        {
            Array.Copy(program, 0, Memory, location, program.Length);
            PC = location;
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dot6502
{
    public enum StateFlag
    {
        Negative = 0x80,
        Overflow = 0x40,
        ignored = 0x20,
        Break = 0x10,
        Decimal = 0x08,
        Interrupt = 0x04,
        Zero = 0x02,
        Carry = 0x01
    }

    public class ExecutionState : IDisposable
    {
        public byte[] Memory = new byte[65536];

        //registers:
        public ushort PC; //program counter
        public byte AC; //accumulator
        public byte X; //X register
        public byte Y; //Y register
        public byte SR; //status register [NV-BDIZC]
        public byte SP = 0xFF; //stack pointer

        private ushort stackBase = 0x0100;
        private ushort StartLocation;

        private List<MemoryWatch> watches = new List<MemoryWatch>();
        private bool disposedValue;

        public ExecutionState()
        {
            SetFlag(StateFlag.ignored);
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
            foreach (var watch in watches)
            {
                if (!watch.IsInside(pos)) continue;
                watch.Callback(pos, value);
            }
        }

        public void Reset()
        {
            AC = 0;
            X = 0;
            Y = 0;
            SR = 0x20;
            SP = 0xFF;
            PC = StartLocation;
        }

        public ushort ReadWord(ushort pos)
        {
            return Memory.ReadWord(pos);
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

        public void SetOverflowFlag(bool set)
        {
            if (set) SetFlag(StateFlag.Overflow);
            else ClearFlag(StateFlag.Overflow);
        }

        public void SetNegativeFlag(bool set)
        {
            if (set) SetFlag(StateFlag.Negative);
            else ClearFlag(StateFlag.Negative);
        }

        public void SetNegativeFlag(byte value)
        {
            SetNegativeFlag(value > 0x7f);
        }

        public void SetCarryFlag(bool set)
        {
            if (set) SetFlag(StateFlag.Carry);
            else ClearFlag(StateFlag.Carry);
        }

        public void SetZeroFlag(bool set)
        {
            if (set) SetFlag(StateFlag.Zero);
            else ClearFlag(StateFlag.Zero);
        }

        public void SetZeroFlag(byte value)
        {
            SetZeroFlag(value == 0);
        }

        internal void SetOverflowFlag(byte opa, byte opb, byte result)
        {
            var aNegative = opa > 0x7F;
            var bNegative = opb > 0x7F;
            var resultNegative = result > 0x7F;

            if ((aNegative == bNegative) && aNegative != resultNegative) SetFlag(StateFlag.Overflow);
            else ClearFlag(StateFlag.Overflow);
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
            this.StartLocation = location;
            PC = location;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    watches.Clear();
                }

                Memory = null;
                watches = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

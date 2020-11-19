using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502
{
    public abstract class Pointer
    {
        protected ExecutionState State { get; }
        protected Pointer(ExecutionState state)
        {
            State = state;
        }

        public abstract byte Get();
        public abstract void Set(byte value);
    }

    public class InvalidPointer : Pointer
    {
        public InvalidPointer() : base(null) { }

        public override byte Get()
        {
            throw new NotSupportedException();
        }

        public override void Set(byte value)
        {
            throw new NotSupportedException();
        }
    }

    public class ImmediateValuePointer : Pointer
    {
        public ImmediateValuePointer(ExecutionState state) : base(state)
        {
        }

        public override byte Get()
        {
            return State.ReadByte((ushort)(State.PC + 1));
        }

        public override void Set(byte value)
        {
            throw new NotSupportedException();
        }
    }
    public class MemoryPointer : Pointer
    {
        public ushort Address { get; }

        public MemoryPointer(ExecutionState state, ushort address) : base(state)
        {
            Address = address;
        }

        public override byte Get()
        {
            return State.ReadByte(Address);
        }

        public override void Set(byte value)
        {
            State.WriteByte(Address, value);
        }
    }

    public abstract class RegisterPointer : Pointer
    {
        protected RegisterPointer(ExecutionState state) : base(state) { }
    }

    public class ACPointer : RegisterPointer
    {
        public ACPointer(ExecutionState state) : base(state) { }

        public override byte Get()
        {
            return State.AC;
        }

        public override void Set(byte value)
        {
            State.AC = value;
        }
    }

    public class XPointer : RegisterPointer
    {
        public XPointer(ExecutionState state) : base(state) { }

        public override byte Get()
        {
            return State.X;
        }

        public override void Set(byte value)
        {
            State.X = value;
        }
    }

    public class YPointer : RegisterPointer
    {
        public YPointer(ExecutionState state) : base(state) { }

        public override byte Get()
        {
            return State.Y;
        }

        public override void Set(byte value)
        {
            State.Y = value;
        }
    }

}

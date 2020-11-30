namespace Dot6502.AddressingModes
{
    class Relative : AddressingMode
    {
        public override string Name => "Relative";
        public override string ShortName => "rel";
        public override ushort OperandLength => 1;

        public override Pointer Resolve(ExecutionState state)
        {
            return new ImmediateValuePointer(state);
        }

        public override string Disassemble(byte[] mem, int pc)
        {
            var offset = mem[pc + 1];
            var signedOffset = unchecked((sbyte)offset);
            var dst = pc + 2 + signedOffset;
            var dstStr = dst.ToString("X4");
            var bb = offset.ToString("X2");
            return $"${bb}\t; {dstStr}";
        }
    }
}

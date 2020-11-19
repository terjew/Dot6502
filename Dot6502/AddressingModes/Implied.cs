using System;

namespace Dot6502.AddressingModes
{
    class Implied : AddressingMode
    {
        public override string Name => "Implied";
        public override string ShortName => "impl";
        public override ushort OperandLength => 0;
    }
}

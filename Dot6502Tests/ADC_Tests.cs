using Dot6502;
using Dot6502.AddressingModes;
using Dot6502.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot6502Tests
{
    [TestClass]
    public class ADC_Tests
    {
        private ExecutionState RunAdc(byte AC, byte operand, bool carry = false)
        {
            ExecutionState state = new ExecutionState();
            if (carry) state.SetCarryFlag(true);
            var instruction = new Dot6502.Instructions.ADC(new Immediate());
            state.AC = AC;
            state.WriteByte(1, operand);
            instruction.Execute(state);
            return state;
        }

        [TestMethod]
        public void ADC_00_00()
        {
            var state = RunAdc(0x00, 0x00);
            Assert.AreEqual(0, state.AC);
            Assert.AreEqual(true, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_00_01()
        {
            var state = RunAdc(0x00, 0x01);
            Assert.AreEqual(0x01, state.AC);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_00_7f()
        {
            var state = RunAdc(0x00, 0x7f);
            Assert.AreEqual(0x7f, state.AC);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_00_80()
        {
            var state = RunAdc(0x00, 0x80);
            Assert.AreEqual(0x80, state.AC);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_00_fe()
        {
            var state = RunAdc(0x00, 0xfe);
            Assert.AreEqual(0xfe, state.AC);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_00_ff()
        {
            var state = RunAdc(0x00, 0xff);
            Assert.AreEqual(0xff, state.AC);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_00_ff_c()
        {
            var state = RunAdc(0x00, 0xff, true);
            Assert.AreEqual(0x00, state.AC);
            Assert.AreEqual(true, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_7f_00_c()
        {
            var state = RunAdc(0x7f, 0x00, true);
            Assert.AreEqual(0x80, state.AC);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_7f_00()
        {
            var state = RunAdc(0x7f, 0x00);
            Assert.AreEqual(0x7f, state.AC);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_7f_01()
        {
            var state = RunAdc(0x7f, 0x01);
            Assert.AreEqual(0x80, state.AC);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_7f_80()
        {
            var state = RunAdc(0x7f, 0x80);
            Assert.AreEqual(0xff, state.AC);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }
        [TestMethod]
        public void ADC_7f_80_c()
        {
            var state = RunAdc(0x7f, 0x80, true);
            Assert.AreEqual(0x00, state.AC);
            Assert.AreEqual(true, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_80_80()
        {
            var state = RunAdc(0x80, 0x80);
            Assert.AreEqual(0x00, state.AC);
            Assert.AreEqual(true, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_f0_f0()
        {
            var state = RunAdc(0xf0, 0xf0);
            Assert.AreEqual(0xe0, state.AC);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }

        [TestMethod]
        public void ADC_ff_ff()
        {
            var state = RunAdc(0xff, 0xff);
            Assert.AreEqual(0xfe, state.AC);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Overflow));
        }

    }
}

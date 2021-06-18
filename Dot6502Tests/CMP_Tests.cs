using Dot6502;
using Dot6502.AddressingModes;
using Dot6502.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot6502Tests
{
    [TestClass]
    public class CMP_Tests
    {
        private static ExecutionState RunCmp(byte AC, byte operand)
        {
            ExecutionState state = new();
            var instruction = new Dot6502.Instructions.CMP(new Immediate());
            state.AC = AC;
            state.WriteByte(1, operand);
            instruction.Execute(state);
            return state;
        }

        [TestMethod]
        public void CMP_00_00()
        {
            var state = RunCmp(0x00, 0x00);
            Assert.AreEqual(true, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
        }

        [TestMethod]
        public void CMP_7f_7f()
        {
            var state = RunCmp(0x7F, 0x7F);
            Assert.AreEqual(true, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
        }

        [TestMethod]
        public void CMP_1f_1f()
        {
            var state = RunCmp(0x1F, 0x1F);
            Assert.AreEqual(true, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
        }

        [TestMethod]
        public void CMP_00_7f()
        {
            var state = RunCmp(0x00, 0x7f);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
        }


        [TestMethod]
        public void CMP_00_ff()
        {
            var state = RunCmp(0x00, 0xff);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
        }

        [TestMethod]
        public void CMP_00_80()
        {
            var state = RunCmp(0x00, 0x80);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
        }



        [TestMethod]
        public void CMP_10_ef()
        {
            var state = RunCmp(0x10, 0xef);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
        }

        [TestMethod]
        public void CMP_7f_80()
        {
            var state = RunCmp(0x7f, 0x80);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
        }

        [TestMethod]
        public void CMP_7f_a0()
        {
            var state = RunCmp(0x7F, 0xA0);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Carry));
        }


        [TestMethod]
        public void CMP_7f_00()
        {
            var state = RunCmp(0x7f, 0x00);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
        }


        [TestMethod]
        public void CMP_ff_00()
        {
            var state = RunCmp(0xff, 0x00);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
        }

        [TestMethod]
        public void CMP_80_00()
        {
            var state = RunCmp(0x80, 0x00);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
        }



        [TestMethod]
        public void CMP_ef_10()
        {
            var state = RunCmp(0xef, 0x10);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
        }

        [TestMethod]
        public void CMP_80_7f()
        {
            var state = RunCmp(0x80, 0x7f);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
        }

        [TestMethod]
        public void CMP_a0_7f()
        {
            var state = RunCmp(0xA0, 0x7F);
            Assert.AreEqual(false, state.TestFlag(StateFlag.Zero));
            Assert.AreEqual(false, state.TestFlag(StateFlag.Negative));
            Assert.AreEqual(true, state.TestFlag(StateFlag.Carry));
        }


    }
}

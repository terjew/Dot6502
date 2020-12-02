using Dot6502;
using Dot6502App.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot6502App.Viewmodel
{
    class RegisterViewModel : BindableBase
    {
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string _value;
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }

    class MemoryViewModel : BindableBase
    {
        private EmulationModel executionModel;

        private string offset = "0000";
        public string Offset
        {
            get => offset;
            set {
                if (SetProperty(ref offset, value))
                {
                    Update();
                }
            }
        }

        private bool updateWhilePlaying = false;
        public bool UpdateWhilePlaying
        {
            get => updateWhilePlaying;
            set => SetProperty(ref updateWhilePlaying, value);
        }

        private IEnumerable<string> lines;
        public IEnumerable<string> Lines
        {
            get => lines;
            set => SetProperty(ref lines, value);
        }

        public BindingList<RegisterViewModel> Registers { get; } = new BindingList<RegisterViewModel>();
        public BindingList<RegisterViewModel> Flags { get; } = new BindingList<RegisterViewModel>();

        public IEnumerable<string> Offsets => Enumerable.Range(0, 255).Select(i => i.ToString("X2") + "00");

        public MemoryViewModel(EmulationModel model)
        {
            executionModel = model;
            executionModel.Stopped += ExecutionModel_Stopped;
            executionModel.Loaded += ExecutionModel_Loaded;
            executionModel.Frame += ExecutionModel_Frame;

            Registers.Add(new RegisterViewModel() { Name = "AC" });
            Registers.Add(new RegisterViewModel() { Name = "X" });
            Registers.Add(new RegisterViewModel() { Name = "Y" });
            Registers.Add(new RegisterViewModel() { Name = "SR" });
            Registers.Add(new RegisterViewModel() { Name = "SP" });
            Registers.Add(new RegisterViewModel() { Name = "PC" });

            Flags.Add(new RegisterViewModel() { Name = "Negative" });
            Flags.Add(new RegisterViewModel() { Name = "Overflow" });
            Flags.Add(new RegisterViewModel() { Name = "Break" });
            Flags.Add(new RegisterViewModel() { Name = "Decimal" });
            Flags.Add(new RegisterViewModel() { Name = "Interrupt" });
            Flags.Add(new RegisterViewModel() { Name = "Zero" });
            Flags.Add(new RegisterViewModel() { Name = "Carry" });
        }

        private RegisterViewModel GetRegister(string name)
        {
            return Registers.SingleOrDefault(r => r.Name == name);
        }

        private RegisterViewModel GetFlag(string name)
        {
            return Flags.SingleOrDefault(r => r.Name == name);
        }

        private void ExecutionModel_Frame(object sender, int e)
        {
            if (updateWhilePlaying) Update();
        }

        private void ExecutionModel_Loaded(object sender, EventArgs e)
        {
            Update();
        }

        private void ExecutionModel_Stopped(object sender, EventArgs e)
        {
            Update();
        }

        private void Update()
        {
            GetRegister("AC").Value = executionModel.State.AC.ToString("X2");
            GetRegister("X").Value = executionModel.State.X.ToString("X2");
            GetRegister("Y").Value = executionModel.State.Y.ToString("X2");
            GetRegister("SR").Value = executionModel.State.SR.ToString("X2");
            GetRegister("SP").Value = executionModel.State.SP.ToString("X2");
            GetRegister("PC").Value = executionModel.State.PC.ToString("X4");

            GetFlag("Carry").Value = executionModel.State.TestFlag(StateFlag.Carry) ? "1" : "0";
            GetFlag("Zero").Value = executionModel.State.TestFlag(StateFlag.Zero) ? "1" : "0";
            GetFlag("Interrupt").Value = executionModel.State.TestFlag(StateFlag.Interrupt) ? "1" : "0";
            GetFlag("Decimal").Value = executionModel.State.TestFlag(StateFlag.Decimal) ? "1" : "0";
            GetFlag("Break").Value = executionModel.State.TestFlag(StateFlag.Break) ? "1" : "0";
            GetFlag("Overflow").Value = executionModel.State.TestFlag(StateFlag.Overflow) ? "1" : "0";
            GetFlag("Negative").Value = executionModel.State.TestFlag(StateFlag.Negative) ? "1" : "0";

            byte[] bytes = new byte[256];
            int offset = Convert.ToInt32(Offset, 16);
            Array.Copy(executionModel.State.Memory, offset, bytes, 0, 256);
            var lines = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                var bytesString = string.Join(" ", bytes.Skip(i * 8).Take(8).Select(b => b.ToString("X2")));
                var offsetString = (offset + (i * 8)).ToString("X4");
                lines.Add($"{offsetString}: {bytesString}");
            }
            Lines = lines;
        }
    }
}

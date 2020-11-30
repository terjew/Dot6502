using Dot6502;
using Dot6502App.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dot6502App.Viewmodel
{
    class DisassemblyLine
    {
        public string Text { get; set; }
        public Brush Background { get; set; }
    }

    class DisassemblyViewModel : BindableBase
    {
        private EmulationModel executionModel;

        private bool updateWhilePlaying = false;
        public bool UpdateWhilePlaying
        {
            get => updateWhilePlaying;
            set => SetProperty(ref updateWhilePlaying, value);
        }

        private IEnumerable<DisassemblyLine> lines;
        public IEnumerable<DisassemblyLine> Lines
        {
            get => lines;
            set => SetProperty(ref lines, value);
        }

        public DisassemblyViewModel(EmulationModel model)
        {
            executionModel = model;
            executionModel.Stopped += ExecutionModel_Stopped;
            executionModel.Loaded += ExecutionModel_Loaded;
            executionModel.Frame += ExecutionModel_Frame;
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
            int pc = executionModel.State.PC;
            var mem = executionModel.State.Memory;
            int len = 1;
            var lines = new List<DisassemblyLine>();
            var sb = new StringBuilder();
            for (int i = 0; i < 32; i++)
            {
                var instruction = Dot6502.Decoder.DecodeInstruction(mem[pc]);

                var disassembly = "???";
                len = 1;
                if (instruction != null)
                { 
                    len = instruction.InstructionSize;
                    disassembly = $"{instruction.Name} {instruction.AddressingMode.Disassemble(mem, pc)}";
                }
                
                var pcString = pc.ToString("X4");

                sb.Clear();
                sb.Append(mem[pc].ToString("X2"));
                sb.Append(" ");
                sb.Append(len > 1 ? mem[pc + 1].ToString("X2") : "  ");
                sb.Append(" ");
                sb.Append(len > 2 ? mem[pc + 2].ToString("X2") : "  ");

                lines.Add(new DisassemblyLine() { Text = $"{pcString}: {sb} - {disassembly}", Background = (i == 0) ? Brushes.Orange : Brushes.Transparent });
                pc += len;
            }
            Lines = lines;
        }
    }
}

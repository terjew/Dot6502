using Dot6502;
using Dot6502App.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot6502App.Viewmodel
{
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

        public IEnumerable<string> Offsets => Enumerable.Range(0, 255).Select(i => i.ToString("X2") + "00");

        public MemoryViewModel(EmulationModel model)
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

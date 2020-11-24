using Dot6502;
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
        private int offset;
        private ExecutionState state;

        public int Offset
        {
            get => offset;
            set => SetProperty(ref offset, value);
        }

        public MemoryViewModel(ExecutionState state)
        {
            this.state = state;
        }
    }
}

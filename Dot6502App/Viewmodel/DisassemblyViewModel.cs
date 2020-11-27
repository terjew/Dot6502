using Dot6502App.Model;
using Prism.Mvvm;

namespace Dot6502App.Viewmodel
{
    class DisassemblyViewModel : BindableBase
    {
        private EmulationModel _executionModel;

        public DisassemblyViewModel(EmulationModel executionModel)
        {
            _executionModel = executionModel;
        }
    }
}

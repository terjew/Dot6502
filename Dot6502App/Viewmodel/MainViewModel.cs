using Dot6502;
using Dot6502App.Model;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Dot6502App.Viewmodel
{
    class MainViewModel : BindableBase
    {
        public GraphicsViewModel Graphics {get; private set; }
        public StatusBarViewModel Status { get; private set; }
        public IEnumerable<int> TargetFPSValues { get; } = new[] { 10, 20, 30, 60, 120, 1000, 100000 };

        private int targetFPS;
        public int TargetFPS
        {
            get => targetFPS;
            set {
                if (SetProperty(ref targetFPS, value))
                {
                    _executionModel.TargetFPS = value;
                }
            }
        }

        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand StepCommand { get; private set; }
        public DelegateCommand FrameCommand { get; private set; }

        private ExecutionModel _executionModel;

        public DelegateCommand OpenCommand { get; private set; }

        private bool running;
        public bool Running
        {
            get => running;
            set => SetProperty(ref running, value);
        }

        private bool programLoaded;
        public bool ProgramLoaded
        {
            get => programLoaded;
            set => SetProperty(ref programLoaded, value);
        }

        public MainViewModel()
        {
            Graphics = new GraphicsViewModel(0x200, 32, 32);
            Status = new StatusBarViewModel();

            OpenCommand = new DelegateCommand(() => ShowOpenDialog(), () => !Running).ObservesProperty(() => Running);
            PlayCommand = new DelegateCommand(() => _executionModel.Play(), () => ProgramLoaded && !Running).ObservesProperty(() => ProgramLoaded).ObservesProperty(() => Running);
            PauseCommand = new DelegateCommand(() => _executionModel.Pause(), () => ProgramLoaded && Running).ObservesProperty(() => ProgramLoaded).ObservesProperty(() => Running);
            StepCommand = new DelegateCommand(() => _executionModel.StepInstruction(), () => ProgramLoaded && !Running).ObservesProperty(() => ProgramLoaded).ObservesProperty(() => Running);
            FrameCommand = new DelegateCommand(() => _executionModel.StepFrame(), () => ProgramLoaded && !Running).ObservesProperty(() => ProgramLoaded).ObservesProperty(() => Running);

            _executionModel = new ExecutionModel(LoadedCB, FrameCB, StartCB, StopCB);
            TargetFPS = 60;
        }

        private void StartCB()
        {
            Running = true;
        }

        private void StopCB()
        {
            Running = false;
        }

        private void LoadedCB()
        {
            Graphics.ExecutionModel = _executionModel;
            ProgramLoaded = true;
        }

        private void FrameCB(int instructions)
        {
            Status.Frame(instructions);
            Graphics.Update();
        }

        private void ShowOpenDialog()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".hex"; // Default file extension
            fileDialog.Filter = "Hex files (.hex)|*.hex"; // Filter files by extension
            var result = fileDialog.ShowDialog();
            if (result == true)
            {
                _executionModel.Load(fileDialog.FileName);
            }
        }

    }
}

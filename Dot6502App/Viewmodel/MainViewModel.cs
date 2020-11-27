using Dot6502App.Model;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace Dot6502App.Viewmodel
{
    class MainViewModel : BindableBase
    {
        public GraphicsViewModel Graphics {get; private set; }
        public StatusBarViewModel Status { get; private set; }
        public MemoryViewModel Memory { get; private set; }

        public IEnumerable<int> TargetFPSValues { get; } = new[] { 10, 20, 30, 60, 120, 1000, 100000 };

        private int targetFPS;
        public int TargetFPS
        {
            get => targetFPS;
            set {
                if (SetProperty(ref targetFPS, value))
                {
                    executionModel.TargetFPS = value;
                }
            }
        }

        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand StepCommand { get; private set; }
        public DelegateCommand FrameCommand { get; private set; }
        public DelegateCommand ResetCommand { get; private set; }

        private EmulationModel executionModel;

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
            OpenCommand = new DelegateCommand(() => ShowOpenDialog(), () => !Running).ObservesProperty(() => Running);
            ResetCommand = new DelegateCommand(() => executionModel.Reset(), () => ProgramLoaded).ObservesProperty(() => ProgramLoaded);
            PlayCommand = new DelegateCommand(() => executionModel.Play(), () => ProgramLoaded && !Running).ObservesProperty(() => ProgramLoaded).ObservesProperty(() => Running);
            PauseCommand = new DelegateCommand(() => executionModel.Pause(), () => ProgramLoaded && Running).ObservesProperty(() => ProgramLoaded).ObservesProperty(() => Running);
            StepCommand = new DelegateCommand(() => executionModel.StepInstruction(), () => ProgramLoaded && !Running).ObservesProperty(() => ProgramLoaded).ObservesProperty(() => Running);
            FrameCommand = new DelegateCommand(() => executionModel.StepFrame(), () => ProgramLoaded && !Running).ObservesProperty(() => ProgramLoaded).ObservesProperty(() => Running);

            executionModel = new EmulationModel();
            executionModel.Loaded += ExecutionModel_Loaded;
            executionModel.Started += ExecutionModel_Started;
            executionModel.Stopped += ExecutionModel_Stopped;
            executionModel.Frame += ExecutionModel_Frame;

            Graphics = new GraphicsViewModel(executionModel, 0x200, 32, 32);
            Status = new StatusBarViewModel();
            Memory = new MemoryViewModel(executionModel);

            TargetFPS = 60;
        }

        private void ExecutionModel_Frame(object sender, int instructions)
        {
            Status.Frame(instructions);
        }

        private void ExecutionModel_Stopped(object sender, EventArgs e)
        {
            Running = false;
        }

        private void ExecutionModel_Started(object sender, EventArgs e)
        {
            Running = true;
        }

        private void ExecutionModel_Loaded(object sender, EventArgs e)
        {
            ProgramLoaded = true;
        }

        private void ShowOpenDialog()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".hex"; // Default file extension
            fileDialog.Filter = "Hex files (.hex)|*.hex"; // Filter files by extension
            var result = fileDialog.ShowDialog();
            if (result == true)
            {
                executionModel.Load(fileDialog.FileName);
            }
        }

    }
}

using Dot6502;
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
        private ExecutionState _state;

        public GraphicsViewModel Graphics {get; private set; }
        public StatusBarViewModel Status { get; private set; }

        public ICommand PlayCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StepCommand { get; private set; }
        public ICommand FrameCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }

        private Random random;
        private bool playing;
        private bool frameStepping;

        public MainViewModel()
        {
            random = new Random(0);

            //Graphics = new GraphicsViewModel(96, 64);
            Graphics = new GraphicsViewModel(32, 32, GraphicsMode.RGB332);
            Status = new StatusBarViewModel();

            OpenCommand = new DelegateCommand(() => ShowOpenDialog());
            PlayCommand = new DelegateCommand(() => Play());
            PauseCommand = new DelegateCommand(() => Pause());
            StepCommand = new DelegateCommand(() => Step());
            FrameCommand = new DelegateCommand(() => Frame());
        }

        private void Frame()
        {
            frameStepping = true;
            Play();
        }

        private void Pause()
        {
            frameStepping = true;
            playing = false;
        }

        private void Play()
        {
            playing = true;
            while (playing)
            {
                Step();
            }
        }

        private void Step()
        {
            UpdateInput();
            UpdateRandom();
            _state.StepExecution();
            Status.Instruction();
        }

        private byte[] randomBuffer = new byte[1024];
        private int randomIndex = 1023;

        private void UpdateRandom()
        {
            if (randomIndex >= randomBuffer.Length - 1)
            {
                random.NextBytes(randomBuffer);
                randomIndex = 0;
            }
            //Update the random generator number:
            _state.WriteByte(0x00FE, randomBuffer[randomIndex++]);
        }

        private void UpdateInput()
        {
            //FIXME: Implement
        }

        private void ShowOpenDialog()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".hex"; // Default file extension
            fileDialog.Filter = "Hex files (.hex)|*.hex"; // Filter files by extension
            var result = fileDialog.ShowDialog();
            if (result == true)
            {
                Load(fileDialog.FileName);
            }
        }

        public void Load(string filename)
        {
            if (_state != null) _state.Dispose();

            _state = new ExecutionState();
            _state.LoadHexFile(filename);
            _state.AddMemoryWatch(new MemoryWatch(0xFD, 0xFD, VerticalSync));
        }

        private double frametime = 1000.0 / 20;
        private DateTime nextSync = DateTime.Now;
        private void VerticalSync(ushort arg1, byte arg2)
        {
            if (frameStepping)
            {
                frameStepping = false;
                playing = false;
            }
            else if (playing)
            {
                playing = false;
                Dispatcher.CurrentDispatcher.BeginInvoke(() => Play(), DispatcherPriority.ApplicationIdle);
            }
            Graphics.Update(_state.Memory, 0x200);
            Status.Frame();

            while (DateTime.Now < nextSync)
            {
                Thread.Yield();
            }
            nextSync = DateTime.Now + TimeSpan.FromMilliseconds(frametime);
        }

    }
}

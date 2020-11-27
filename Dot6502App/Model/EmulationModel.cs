using Dot6502;
using System;
using System.Threading;
using System.Windows.Threading;

namespace Dot6502App.Model
{
    class EmulationModel
    {
        public event EventHandler Loaded = delegate { };
        public event EventHandler Started = delegate { };
        public event EventHandler Stopped = delegate { };
        public event EventHandler<int> Frame = delegate { };

        private bool exit = false;
        private Thread thread;
        private DateTime nextSync = DateTime.Now;
        private int instructionCount = 0;

        private bool frameStepping;
        private bool singleStepping;
        private bool playing;
        private bool resetting;
        private Random random;

        private byte[] randomBuffer = new byte[65535];
        private int randomIndex = 65535;
        private bool randomInitialized = false;
        private Dispatcher _dispatcher { get; }
        private ManualResetEventSlim syncEvent = new ManualResetEventSlim(false);

        public ExecutionState State { get; private set; }
        public int TargetFPS { get; set; } = 20;

        public EmulationModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            random = new Random(0);

            thread = new Thread(new ThreadStart(ThreadRun));
            thread.Start();

            _dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            exit = true;
            Stop();
            syncEvent.Set();
        }

        private void ThreadRun()
        {
            while(!exit)
            {
                syncEvent.Wait();
                syncEvent.Reset();
                if (resetting)
                {
                    resetting = false;
                    State.Reset();
                    if (!playing) continue;
                }
                if (!exit)
                {
                    _dispatcher.Invoke(() => Started(this, EventArgs.Empty));
                    while (playing && !exit && !resetting)
                    {
                        StepExecution();
                    }
                    if (!exit) _dispatcher.Invoke(() => Stopped(this, EventArgs.Empty));
                }
            }
        }

        private void StepExecution()
        {
            UpdateInput();
            UpdateRandom();
            State.StepExecution();
            if (singleStepping)
            {
                Stop();
            }
            instructionCount++;
        }

        public void Pause()
        {
            Stop();
        }

        public void Play()
        {
            SignalPlaying();
        }

        internal void Reset()
        {
            resetting = true;
            syncEvent.Set();
        }

        public void StepInstruction()
        {
            singleStepping = true;
            SignalPlaying();
        }

        public void StepFrame()
        {
            frameStepping = true;
            SignalPlaying();
        }

        private void SignalPlaying()
        {
            playing = true;
            syncEvent.Set();
        }

        private void Stop()
        {
            singleStepping = false;
            frameStepping = false;
            playing = false;
        }

        private void UpdateRandom()
        {
            if (!randomInitialized)
            {
                random.NextBytes(randomBuffer);
                randomInitialized = true;
            }
            if (randomIndex >= randomBuffer.Length - 1)
            {
                randomIndex = 0;
            }
            State.Memory[0x00FE] = randomBuffer[randomIndex++];
        }

        private void UpdateInput()
        {
            //FIXME: Implement
        }

        public void Load(string filename)
        {
            if (State != null) State.Dispose();

            State = new ExecutionState();
            State.LoadHexFile(filename);
            State.AddMemoryWatch(new MemoryWatch(0xFD, 0xFD, VerticalSync));
            Loaded(this, EventArgs.Empty);
        }

        private void VerticalSync(ushort arg1, byte arg2)
        {
            _dispatcher.Invoke(() => Frame(this, instructionCount));
            instructionCount = 0;

            if (frameStepping)
            {
                Stop();
                return;
            }

            var frametime = 1000.0 / TargetFPS;
            while (DateTime.Now < nextSync)
            {
                Thread.Yield();
            }
            nextSync = DateTime.Now + TimeSpan.FromMilliseconds(frametime);
        }

    }
}

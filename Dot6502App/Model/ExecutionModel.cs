using Dot6502;
using System;
using System.Threading;
using System.Windows.Threading;

namespace Dot6502App.Model
{
    class ExecutionModel
    {
        private Action _loadedCallback;
        private Action<int> _frameCallback;
        private Action _startCallback;
        private Action _stopCallback;

        private bool exit = false;
        private Thread thread;
        private DateTime nextSync = DateTime.Now;
        private int instructionCount = 0;

        private bool frameStepping;
        private bool singleStepping;
        private bool playing;
        private Random random;

        private byte[] randomBuffer = new byte[65535];
        private int randomIndex = 65535;
        private bool randomInitialized = false;
        private Dispatcher _dispatcher { get; }
        private ManualResetEventSlim playingEvent = new ManualResetEventSlim(false);

        public ExecutionState State { get; private set; }
        public int TargetFPS { get; set; } = 20;

        public ExecutionModel(Action loadedCallback, 
            Action<int> frameCallback, 
            Action startCallback,
            Action stopCallback)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _loadedCallback = loadedCallback;
            _frameCallback = frameCallback;
            _startCallback = startCallback;
            _stopCallback = stopCallback;

            random = new Random(0);

            thread = new Thread(new ThreadStart(ThreadRun));
            thread.Start();

            _dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            exit = true;
            Stop();
            playingEvent.Set();
        }

        private void ThreadRun()
        {
            while(!exit)
            {
                playingEvent.Wait();
                playingEvent.Reset();
                if (!exit)
                {
                    _dispatcher.Invoke(_startCallback);
                    while (playing && !exit)
                    {
                        StepExecution();
                    }
                    if (!exit) _dispatcher.Invoke(_stopCallback);
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
            playingEvent.Set();
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
            _loadedCallback();
        }

        private void VerticalSync(ushort arg1, byte arg2)
        {
            _dispatcher.Invoke(_frameCallback, instructionCount);
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

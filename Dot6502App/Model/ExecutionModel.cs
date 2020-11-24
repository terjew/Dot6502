using Dot6502;
using System;
using System.Threading;
using System.Windows.Threading;

namespace Dot6502App.Model
{
    class ExecutionModel
    {
        public Dispatcher Dispatcher { get; }
        public ExecutionState State { get; private set; }

        private Action _loadedCallback;
        private Action<int> _frameCallback;
        private Action _startCallback;
        private Action _stopCallback;

        private bool exit = false;
        private Thread thread;
        private DateTime nextSync = DateTime.Now;
        private int instructionCount = 0;

        public int TargetFPS { get; set; } = 20;

        public ExecutionModel(Action loadedCallback, 
            Action<int> frameCallback, 
            Action startCallback,
            Action stopCallback)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            _loadedCallback = loadedCallback;
            _frameCallback = frameCallback;
            _startCallback = startCallback;
            _stopCallback = stopCallback;

            random = new Random(0);

            thread = new Thread(new ThreadStart(ThreadRun));
            thread.Start();

            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            exit = true;
            Stop();
        }

        private void ThreadRun()
        {
            while(!exit)
            {
                while (!playing && !exit)
                {
                    Thread.Yield();
                }
                if (!exit) Dispatcher.Invoke(_startCallback);
                while (playing && !exit)
                {
                    StepExecution();
                }
                if (!exit) Dispatcher.Invoke(_stopCallback);
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


        private bool frameStepping;
        private bool singleStepping;
        private bool playing;
        private Random random;

        public void StepFrame()
        {
            frameStepping = true;
            playing = true;
        }

        public void Pause()
        {
            Stop();
        }

        public void Play()
        {
            if (State == null) return;
            playing = true;
        }

        public void StepInstruction()
        {
            singleStepping = true;
            playing = true;
        }

        private void Stop()
        {
            singleStepping = false;
            frameStepping = false;
            playing = false;
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
            Dispatcher.Invoke(_frameCallback, instructionCount);
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

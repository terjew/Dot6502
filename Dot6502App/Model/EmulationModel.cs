using Dot6502;
using System;
using System.Threading;
using System.Windows.Input;
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
            VerticalSync(0, 0);
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
            foreach (var currentKey in Enum.GetValues(typeof(Key)))
            {
                Key key = (Key)currentKey;
                if (key != Key.None)
                {
                    if (Keyboard.IsKeyDown((Key)currentKey))
                    {
                        State.Memory[0xff] = (byte)KeyToChar(key);
                        return;
                    }
                }
            }
        }

        char KeyToChar(Key key)
        {

            if (Keyboard.IsKeyDown(Key.LeftAlt) ||
                Keyboard.IsKeyDown(Key.RightAlt) ||
                Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightAlt))
            {
                return '\x00';
            }

            bool caplock = Console.CapsLock;
            bool shift = Keyboard.IsKeyDown(Key.LeftShift) ||
                                    Keyboard.IsKeyDown(Key.RightShift);
            bool iscap = (caplock && !shift) || (!caplock && shift);

            switch (key)
            {
                case Key.Enter: return '\n';
                case Key.A: return (iscap ? 'A' : 'a');
                case Key.B: return (iscap ? 'B' : 'b');
                case Key.C: return (iscap ? 'C' : 'c');
                case Key.D: return (iscap ? 'D' : 'd');
                case Key.E: return (iscap ? 'E' : 'e');
                case Key.F: return (iscap ? 'F' : 'f');
                case Key.G: return (iscap ? 'G' : 'g');
                case Key.H: return (iscap ? 'H' : 'h');
                case Key.I: return (iscap ? 'I' : 'i');
                case Key.J: return (iscap ? 'J' : 'j');
                case Key.K: return (iscap ? 'K' : 'k');
                case Key.L: return (iscap ? 'L' : 'l');
                case Key.M: return (iscap ? 'M' : 'm');
                case Key.N: return (iscap ? 'N' : 'n');
                case Key.O: return (iscap ? 'O' : 'o');
                case Key.P: return (iscap ? 'P' : 'p');
                case Key.Q: return (iscap ? 'Q' : 'q');
                case Key.R: return (iscap ? 'R' : 'r');
                case Key.S: return (iscap ? 'S' : 's');
                case Key.T: return (iscap ? 'T' : 't');
                case Key.U: return (iscap ? 'U' : 'u');
                case Key.V: return (iscap ? 'V' : 'v');
                case Key.W: return (iscap ? 'W' : 'w');
                case Key.X: return (iscap ? 'X' : 'x');
                case Key.Y: return (iscap ? 'Y' : 'y');
                case Key.Z: return (iscap ? 'Z' : 'z');
                case Key.D0: return (shift ? ')' : '0');
                case Key.D1: return (shift ? '!' : '1');
                case Key.D2: return (shift ? '@' : '2');
                case Key.D3: return (shift ? '#' : '3');
                case Key.D4: return (shift ? '$' : '4');
                case Key.D5: return (shift ? '%' : '5');
                case Key.D6: return (shift ? '^' : '6');
                case Key.D7: return (shift ? '&' : '7');
                case Key.D8: return (shift ? '*' : '8');
                case Key.D9: return (shift ? '(' : '9');
                case Key.OemPlus: return (shift ? '+' : '=');
                case Key.OemMinus: return (shift ? '_' : '-');
                case Key.OemQuestion: return (shift ? '?' : '/');
                case Key.OemComma: return (shift ? '<' : ',');
                case Key.OemPeriod: return (shift ? '>' : '.');
                case Key.OemOpenBrackets: return (shift ? '{' : '[');
                case Key.OemQuotes: return (shift ? '"' : '\'');
                case Key.Oem1: return (shift ? ':' : ';');
                case Key.Oem3: return (shift ? '~' : '`');
                case Key.Oem5: return (shift ? '|' : '\\');
                case Key.Oem6: return (shift ? '}' : ']');
                case Key.Tab: return '\t';
                case Key.Space: return ' ';

                // Number Pad
                case Key.NumPad0: return '0';
                case Key.NumPad1: return '1';
                case Key.NumPad2: return '2';
                case Key.NumPad3: return '3';
                case Key.NumPad4: return '4';
                case Key.NumPad5: return '5';
                case Key.NumPad6: return '6';
                case Key.NumPad7: return '7';
                case Key.NumPad8: return '8';
                case Key.NumPad9: return '9';
                case Key.Subtract: return '-';
                case Key.Add: return '+';
                case Key.Decimal: return '.';
                case Key.Divide: return '/';
                case Key.Multiply: return '*';

                default: return '\x00';
            }
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
            _dispatcher.Invoke(() =>
            {
                Frame(this, instructionCount);
                UpdateInput();
            });
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

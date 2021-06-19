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
        private readonly Thread thread;
        private DateTime nextSync = DateTime.Now;
        private int instructionCount = 0;

        private bool frameStepping;
        private bool singleStepping;
        private bool playing;
        private bool resetting;
        private readonly Random random;

        private readonly byte[] randomBuffer = new byte[65535];
        private int randomIndex = 65535;
        private bool randomInitialized = false;
        private Dispatcher Dispatcher { get; }
        private readonly ManualResetEventSlim syncEvent = new(false);

        public ExecutionState State { get; private set; }
        public int TargetFPS { get; set; } = 20;

        public EmulationModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            random = new Random(0);

            thread = new Thread(new ThreadStart(ThreadRun));
            thread.Start();

            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
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
                    Dispatcher.Invoke(() => Started(this, EventArgs.Empty));
                    while (playing && !exit && !resetting)
                    {
                        StepExecution();
                    }
                    if (!exit) Dispatcher.Invoke(() => Stopped(this, EventArgs.Empty));
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

        static char KeyToChar(Key key)
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

            return key switch
            {
                Key.Enter => '\n',
                Key.A => (iscap ? 'A' : 'a'),
                Key.B => (iscap ? 'B' : 'b'),
                Key.C => (iscap ? 'C' : 'c'),
                Key.D => (iscap ? 'D' : 'd'),
                Key.E => (iscap ? 'E' : 'e'),
                Key.F => (iscap ? 'F' : 'f'),
                Key.G => (iscap ? 'G' : 'g'),
                Key.H => (iscap ? 'H' : 'h'),
                Key.I => (iscap ? 'I' : 'i'),
                Key.J => (iscap ? 'J' : 'j'),
                Key.K => (iscap ? 'K' : 'k'),
                Key.L => (iscap ? 'L' : 'l'),
                Key.M => (iscap ? 'M' : 'm'),
                Key.N => (iscap ? 'N' : 'n'),
                Key.O => (iscap ? 'O' : 'o'),
                Key.P => (iscap ? 'P' : 'p'),
                Key.Q => (iscap ? 'Q' : 'q'),
                Key.R => (iscap ? 'R' : 'r'),
                Key.S => (iscap ? 'S' : 's'),
                Key.T => (iscap ? 'T' : 't'),
                Key.U => (iscap ? 'U' : 'u'),
                Key.V => (iscap ? 'V' : 'v'),
                Key.W => (iscap ? 'W' : 'w'),
                Key.X => (iscap ? 'X' : 'x'),
                Key.Y => (iscap ? 'Y' : 'y'),
                Key.Z => (iscap ? 'Z' : 'z'),
                Key.D0 => (shift ? ')' : '0'),
                Key.D1 => (shift ? '!' : '1'),
                Key.D2 => (shift ? '@' : '2'),
                Key.D3 => (shift ? '#' : '3'),
                Key.D4 => (shift ? '$' : '4'),
                Key.D5 => (shift ? '%' : '5'),
                Key.D6 => (shift ? '^' : '6'),
                Key.D7 => (shift ? '&' : '7'),
                Key.D8 => (shift ? '*' : '8'),
                Key.D9 => (shift ? '(' : '9'),
                Key.OemPlus => (shift ? '+' : '='),
                Key.OemMinus => (shift ? '_' : '-'),
                Key.OemQuestion => (shift ? '?' : '/'),
                Key.OemComma => (shift ? '<' : ','),
                Key.OemPeriod => (shift ? '>' : '.'),
                Key.OemOpenBrackets => (shift ? '{' : '['),
                Key.OemQuotes => (shift ? '"' : '\''),
                Key.Oem1 => (shift ? ':' : ';'),
                Key.Oem3 => (shift ? '~' : '`'),
                Key.Oem5 => (shift ? '|' : '\\'),
                Key.Oem6 => (shift ? '}' : ']'),
                Key.Tab => '\t',
                Key.Space => ' ',
                // Number Pad
                Key.NumPad0 => '0',
                Key.NumPad1 => '1',
                Key.NumPad2 => '2',
                Key.NumPad3 => '3',
                Key.NumPad4 => '4',
                Key.NumPad5 => '5',
                Key.NumPad6 => '6',
                Key.NumPad7 => '7',
                Key.NumPad8 => '8',
                Key.NumPad9 => '9',
                Key.Subtract => '-',
                Key.Add => '+',
                Key.Decimal => '.',
                Key.Divide => '/',
                Key.Multiply => '*',
                _ => '\x00',
            };
        }
        public void Load(string filename)
        {
            if (State != null) State.Dispose();

            State = new ExecutionState();

            State.LoadFile(filename);
            
            State.AddMemoryWatch(new MemoryWatch(0xFD, 0xFD, VerticalSync));
            Loaded(this, EventArgs.Empty);
        }

        private void VerticalSync(ushort arg1, byte arg2)
        {
            Dispatcher.Invoke(() =>
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

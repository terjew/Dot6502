using Dot6502;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dot6502MiniConsole
{
    public class MiniConsole
    {
        const int FPS = 20;
        const int FRAMETIME = 1000 / FPS;
        const int FB_START = 0x0200;

        private int frameCounter;
        private DateTime lastFpsUpdate = DateTime.Now;
        private DateTime nextSync = DateTime.Now;
        private ExecutionState state;
        private Random random;
        private byte[] backbuffer;
        private int width;
        private int height;

        private ConsoleColor[] colors = new ConsoleColor[]
        {
            ConsoleColor.Black,
            ConsoleColor.White,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkCyan,
            ConsoleColor.Magenta,
            ConsoleColor.DarkGreen,
            ConsoleColor.DarkBlue,
            ConsoleColor.Yellow,
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkMagenta,
            ConsoleColor.Red,
            ConsoleColor.DarkGray,
            ConsoleColor.Gray,
            ConsoleColor.Green,
            ConsoleColor.Blue,
            ConsoleColor.Cyan
        };

        public MiniConsole(int width, int height)
        {
            this.width = width;
            this.height = height;

            state = new ExecutionState();
            state.AddMemoryWatch(new MemoryWatch(FB_START, (ushort)(FB_START + (width * height) - 1), UpdateFramebuffer));
            state.AddMemoryWatch(new MemoryWatch(0x00FD, 0x00FD, WaitForVSync));

            random = new Random();
            InitConsole();
        }

        private void InitConsole()
        {
            Console.WindowWidth = width * 2;
            Console.WindowHeight = height;
            Console.BufferWidth = width * 2;
            Console.BufferHeight = height;
            Console.CursorVisible = false;
            Console.BackgroundColor = colors[0];
            Console.SetCursorPosition(0, 0);
            //Start buffer at 0xff to force all to invalidate
            backbuffer = Enumerable.Repeat((byte)0xff, width * height).ToArray();
            for (int i = 0; i < width * height; i++)
            {
                //Clear screen to zero:
                UpdateFramebuffer((ushort)(i + 0x200), 0x00);
            }
        }

        private void WaitForVSync(ushort arg1, byte arg2)
        {
            while (DateTime.Now < nextSync)
            {
                Thread.Yield();
            }
            nextSync = DateTime.Now + TimeSpan.FromMilliseconds(FRAMETIME);
            frameCounter++;
            var elapsedSinceLastFPS = nextSync - lastFpsUpdate;
            if (elapsedSinceLastFPS.TotalSeconds > 1)
            {
                Console.Title = $"FPS: {frameCounter}";
                frameCounter = 0;
                lastFpsUpdate = DateTime.Now;
            }
        }

        private void UpdateFramebuffer(ushort pos, byte value)
        {
            pos -= FB_START;
            if (backbuffer[pos] == value) return;
            backbuffer[pos] = value;
            var Y = pos / width;
            var X = pos % width;
            var color = value & 0x0f;
            Console.BackgroundColor = colors[color];
            Console.SetCursorPosition(X * 2, Y);
            var hex = string.Format("{0:x2}", value);
            Console.Write("  ");
        }

        public void Run()
        {
            while(true)
            {
                UpdateInput();
                UpdateRandom();
                state.StepExecution();
            }
        }

        private void UpdateRandom()
        {
            //Update the random generator number:
            state.WriteByte(0x00FE, (byte)(random.Next(256)));
        }

        private void UpdateInput()
        {
            //FIXME: Implement
        }

        internal void LoadProgram(string filename)
        {
            state.LoadHexFile(filename);
        }
    }
}

using Dot6502;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dot6502MiniConsole
{
    public class MiniConsole
    {
        private ExecutionState state;
        private Random random;
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

        public MiniConsole()
        {
            state = new ExecutionState();
            state.AddMemoryWatch(new MemoryWatch(0x0200, 0x05ff, UpdateFramebuffer));
            Console.WindowWidth = 64;
            Console.WindowHeight = 32;
            Console.BufferWidth = 64;
            Console.BufferHeight = 32;
            random = new Random(2); //reuse seed for repeatable results
            Console.BackgroundColor = colors[0];
            Console.SetCursorPosition(0, 0);
            Console.Write(".");
        }

        private void UpdateFramebuffer(ushort pos, byte value)
        {
            pos -= 0x200;
            var col = pos % 32;
            var row = pos / 32;
            var color = value & 0x0f;
            Console.BackgroundColor = colors[color];
            Console.SetCursorPosition(row * 2, col);
            var hex = string.Format("{0:x2}", value);            
            Console.Write(hex);
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
            //state.WriteByte(0x00FE, (byte)(random.Next(256)));
            state.WriteByte(0x00FE, 0xff);
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

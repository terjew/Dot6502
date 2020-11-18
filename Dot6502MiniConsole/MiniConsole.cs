using Dot6502;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot6502MiniConsole
{
    public class MiniConsole
    {
        private ExecutionState state;

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

//Black ($0)
//White ($1)
//Red ($2)
//Cyan ($3)
//Purple ($4)
//Green ($5)
//Blue ($6)
//Yellow ($7)
//Orange ($8)
//Brown ($9)
//Light red ($a)
//Dark gray ($b)
//Gray ($c)
//Light green ($d)
//Light blue ($e)
//Light gray ($f)

        public MiniConsole()
        {
            state = new ExecutionState();
            state.AddMemoryWatch(new MemoryWatch(0x0200, 0x05ff, UpdateConsole));
            Console.WindowWidth = 64;
            Console.WindowHeight = 32;
            Console.BufferWidth = 64;
            Console.BufferHeight = 32;
        }

        private void UpdateConsole(ushort pos, byte value)
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

        public void LoadProgram(string program, ushort location)
        {
            var byteStrings = program.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var bytes = byteStrings.Select(hex => Convert.ToInt32(hex, 16)).Select(i => (byte)i).ToArray();
            for(ushort i = 0; i < bytes.Length; i++)
            {
                state.WriteByte((ushort)(location + i), bytes[i]);
            }
            state.PC = location;
        }

        public void Run()
        {
            for (; ; )
            {
                state.StepExecution();
            }
        }
    }
}

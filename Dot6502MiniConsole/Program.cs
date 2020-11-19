using System;

namespace Dot6502MiniConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var console = new MiniConsole();
            console.LoadProgram(@"
                AD FE 00 8D 00 00 AD FE
                00 29 03 18 69 02 8D 01
                00 AD FE 00 A0 00 91 00
                4C 37 13
                ", 0x1337);
            console.Run();
        }
    }
}

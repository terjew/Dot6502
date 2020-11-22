using System;

namespace Dot6502MiniConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = new MiniConsole(0x20, 0x20);
            //console.LoadProgram("SamplePrograms/random.hex");
            console.LoadProgram("SamplePrograms/conway.hex");
            //var console = new MiniConsole(0x60, 0x40);
            //console.LoadProgram("SamplePrograms/ball.hex");
            console.Run();
        }
    }
}

using System;

namespace Dot6502MiniConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = new MiniConsole();
            //console.LoadProgram("SamplePrograms/random.hex");
            console.LoadProgram("SamplePrograms/conway.hex");
            console.Run();
        }
    }
}

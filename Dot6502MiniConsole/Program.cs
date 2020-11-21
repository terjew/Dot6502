using System;

namespace Dot6502MiniConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = new MiniConsole(32, 32);
            //console.LoadProgram("SamplePrograms/random.hex");
            console.LoadProgram("SamplePrograms/conway.hex");
            console.Run();
        }
    }
}

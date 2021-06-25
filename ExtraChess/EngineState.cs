using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess
{
    internal static class EngineState
    {
        internal static bool Ready { get; set; } = true;

        internal static void IsReady()
        {
            Task.Run(async () =>
            {
                while (!Ready)
                {
                    await Task.Delay(10);
                }
                Console.WriteLine("readyok");
            }).Wait();
        }
    }
}

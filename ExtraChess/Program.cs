using ExtraChess.UCI;
using System;
using System.Reflection;

namespace ExtraChess
{
    class Program
    {
        static void Main(string[] args)
        {
            EngineState.Initialize();
            EngineState.SetupPosition("startpos");

            Version engineVersion = Assembly.GetEntryAssembly().GetName().Version;
            Console.WriteLine($"Welcome to the ExtraChess v{engineVersion.Major}.{engineVersion.Minor} engine! Please enter your command below.");

            while (true)
            {
                try
                {
                    UCIReceiver.ProcessInstruction(Console.ReadLine());
                }
                catch (UnknownCommandException ex)
                {
                    Console.WriteLine($"The command \"{ex.Command}\" is not supported.");
                }
                catch (NoCommandException)
                {
                    Console.WriteLine($"No command entered.");
                }
                catch (InvalidArgumentsException)
                {
                    Console.WriteLine($"Invalid arguments. Enter the 'help' command to view the manual.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something went wrong: {ex.Message} {ex.StackTrace}");
                }
            }
        }
    }
}

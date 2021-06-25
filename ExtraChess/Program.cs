using ExtraChess.UCI;
using System;
using System.Reflection;

namespace ExtraChess
{
    class Program
    {
        static void Main(string[] args)
        {
            Version engineVersion = Assembly.GetEntryAssembly().GetName().Version;
            Console.WriteLine($"Welcome to the ExtraChess v{engineVersion.Major}.{engineVersion.Minor} engine! Please enter your command below.");

            while (true)
            {
                try
                {
                    string line = Console.ReadLine();
                    string result = UCIProcessor.ProcessInstruction(line);
                    Console.Write(result);
                }
                catch (UnknownCommandException ex)
                {
                    Console.WriteLine($"The command \"{ex.Command}\" is not supported.");
                }
                catch (NoCommandException)
                {
                    Console.WriteLine($"No command entered.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something went wrong: {ex.Message}");
                }
            }
        }
    }
}

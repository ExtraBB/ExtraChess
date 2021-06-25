using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtraChess.UCI
{
    public static class UCIProcessor
    {
        public static string ProcessInstruction(string instruction)
        {
            if(string.IsNullOrEmpty(instruction))
            {
                throw new NoCommandException();
            }
            string[] words = instruction.Split();

            string command = words[0];
            string[] options = words.Skip(1).ToArray();

            switch (command)
            {
                case "uci":
                    {
                        return Uci();
                    }
                case "debug":
                    {
                        throw new NotImplementedException();
                    }
                case "isready":
                    {
                        throw new NotImplementedException();
                    }
                case "setoption":
                    {
                        throw new NotImplementedException();
                    }
                case "register":
                    {
                        throw new NotImplementedException();
                    }
                case "ucinewgame":
                    {
                        throw new NotImplementedException();
                    }
                case "position":
                    {
                        throw new NotImplementedException();
                    }
                case "go":
                    {
                        throw new NotImplementedException();
                    }
                case "stop":
                    {
                        throw new NotImplementedException();
                    }
                case "ponderhit":
                    {
                        throw new NotImplementedException();
                    }
                case "help":
                    {
                        return HelpString;
                    }
                case "quit":
                    {
                        Environment.Exit(0);
                        return "Succesfully exited the application.";
                    }
                default:
                    {
                        throw new UnknownCommandException(command);
                    }
            }
        }

		private static string Uci()
        {
			Version engineVersion = Assembly.GetEntryAssembly().GetName().Version;

			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"id name ExtraChess v{engineVersion.Major}.{engineVersion.Minor}");
			stringBuilder.AppendLine($"id author Bruno Carvalhal");

			// TODO: Print options here

			stringBuilder.AppendLine("uciok");

			return stringBuilder.ToString();
		}

		private static string HelpString =
@"	* uci
		tell engine to use the uci (universal chess interface),
		this will be send once as a first command after program boot
		to tell the engine to switch to uci mode.
		After receiving the uci command the engine must identify itself with the 'id' command
		and sent the 'option' commands to tell the GUI which engine settings the engine supports if any.
		After that the engine should sent 'uciok' to acknowledge the uci mode.
		If no uciok is sent within a certain time period, the engine task will be killed by the GUI.

	* debug [ on | off ]
		switch the debug mode of the engine on and off.
		In debug mode the engine should sent additional infos to the GUI, e.g. with the 'info string' command,
		to help debugging, e.g. the commands that the engine has received etc.
		This mode should be switched off by default and this command can be sent
		any time, also when the engine is thinking.

	* isready
		this is used to synchronize the engine with the GUI. When the GUI has sent a command or
		multiple commands that can take some time to complete,
		this command can be used to wait for the engine to be ready again or
		to ping the engine to find out if it is still alive.
		E.g. this should be sent after setting the path to the tablebases as this can take some time.
		This command is also required once before the engine is asked to do any search
		to wait for the engine to finish initializing.
		This command must always be answered with 'readyok' and can be sent also when the engine is calculating
		in which case the engine should also immediately answer with 'readyok' without stopping the search.

	* setoption name  [value ]
		this is sent to the engine when the user wants to change the internal parameters
		of the engine. For the 'button' type no value is needed.
		One string will be sent for each parameter and this will only be sent when the engine is waiting.
		The name of the option in  should not be case sensitive and can inludes spaces like also the value.
		The substrings 'value' and 'name' should be avoided in  and  to allow unambiguous parsing,
		for example do not use  = 'draw value'.
		Here are some strings for the example below:
			'setoption name Nullmove value true\n'
			'setoption name Selectivity value 3\n'
			'setoption name Style value Risky\n'
			'setoption name Clear Hash\n'
			'setoption name NalimovPath value c:\chess\tb\4;c:\chess\tb\5\n'

	* register
		this is the command to try to register an engine or to tell the engine that registration
		will be done later. This command should always be sent if the engine	has send 'registration error'
		at program startup.
		The following tokens are allowed:
		* later
			the user doesn't want to register the engine now.
		* name 
			the engine should be registered with the name 
		* code 
			the engine should be registered with the code 
		Example:
			'register later'
			'register name Stefan MK code 4359874324'

	* ucinewgame
		this is sent to the engine when the next search (started with 'position' and 'go') will be from
		a different game. This can be a new game the engine should play or a new game it should analyse but
		also the next position from a testsuite with positions only.
		If the GUI hasn't sent a 'ucinewgame' before the first 'position' command, the engine shouldn't
		expect any further ucinewgame commands as the GUI is probably not supporting the ucinewgame command.
		So the engine should not rely on this command even though all new GUIs should support it.
		As the engine's reaction to 'ucinewgame' can take some time the GUI should always send 'isready'
		after 'ucinewgame' to wait for the engine to finish its operation.
   
	* position [fen  | startpos ]  moves  .... 
		set up the position described in fenstring on the internal board and
		play the moves on the internal chess board.
		if the game was played  from the start position the string 'startpos' will be sent
		Note: no 'new' command is needed. However, if this position is from a different game than
		the last position sent to the engine, the GUI should have sent a 'ucinewgame' inbetween.

	* go
		start calculating on the current position set up with the 'position' command.
		There are a number of commands that can follow this command, all will be sent in the same string.
		If one command is not send its value should be interpreted as it would not influence the search.
		* searchmoves  .... 
			restrict search to this moves only
			Example: After 'position startpos' and 'go infinite searchmoves e2e4 d2d4'
			the engine should only search the two moves e2e4 and d2d4 in the initial position.
		* ponder
			start searching in pondering mode.
			Do not exit the search in ponder mode, even if it's mate!
			This means that the last move sent in in the position string is the ponder move.
			The engine can do what it wants to do, but after a 'ponderhit' command
			it should execute the suggested move to ponder on. This means that the ponder move sent by
			the GUI can be interpreted as a recommendation about which move to ponder. However, if the
			engine decides to ponder on a different move, it should not display any mainlines as they are
			likely to be misinterpreted by the GUI because the GUI expects the engine to ponder
			on the suggested move.
		* wtime 
			white has x msec left on the clock
		* btime 
			black has x msec left on the clock
		* winc 
			white increment per move in mseconds if x > 0
		* binc 
			black increment per move in mseconds if x > 0
		* movestogo 
			there are x moves to the next time control,
			this will only be sent if x > 0,
			if you don't get this and get the wtime and btime it's sudden death
		* depth 
			search x plies only.
		* nodes 
			search x nodes only,
		* mate 
			search for a mate in x moves
		* movetime 
			search exactly x mseconds
		* infinite
			search until the 'stop' command. Do not exit the search without being told so in this mode!
    
	* stop
		stop calculating as soon as possible,
		don't forget the 'bestmove' and possibly the 'ponder' token when finishing the search

	* ponderhit
		the user has played the expected move. This will be sent if the engine was told to ponder on the same move
		the user has played. The engine should continue searching but switch from pondering to normal search.

	* quit
		quit the program as soon as possible
";
	}
}

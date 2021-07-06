﻿using ExtraChess.Models;
using ExtraChessUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChessUI.Utils
{
    public class EngineProcess : IDisposable
    {
        private Process process;

        public delegate void OutputReceivedEventHandler(string line);
        public event OutputReceivedEventHandler OutputReceived;

        public delegate void MoveReceivedEventHandler(Move move);
        public event MoveReceivedEventHandler MoveReceived;

        public EngineProcess(string fileName)
        {
            process = new Process()
            {
                StartInfo =
                {
                    FileName = fileName,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            process.OutputDataReceived += Process_OutputDataReceived;
            process.Start();
            process.BeginOutputReadLine();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OutputReceived?.Invoke(e.Data);

            string[] split = e.Data.Split();
            if(split[0] == "bestmove")
            {
                MoveReceived?.Invoke(Move.UCIMoveToMove(Game.PossibleMoves, split[1]));
            }
        }

        public void SendMessage(string message)
        {
            process.StandardInput.WriteLine(message);
            process.StandardInput.Flush();
        }

        public void Dispose()
        {
            process.OutputDataReceived -= Process_OutputDataReceived;
            process.Kill();
            process.Dispose();
        }
    }
}

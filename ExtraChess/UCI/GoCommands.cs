using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess.UCI
{
    public enum GoCommands
    {
        SearchMoves,
        Ponder,
        WTime,
        BTime,
        WInc,
        BInc,
        MovesToGo,
        Depth,
        Nodes,
        Mate,
        MoveTime,
        Infinite
    }
}

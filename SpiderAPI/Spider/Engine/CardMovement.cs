using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderAPI.Spider.Engine
{
    public class CardMovement
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public CardRank Rank { get; set; }
        public CardSuite Suite { get; set; }
        public int MoveSize { get; set; }
        public bool TimeDelay { get; set; } = false;
    }
}

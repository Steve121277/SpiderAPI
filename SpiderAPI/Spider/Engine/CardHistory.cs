using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderAPI.Spider.Engine
{
    internal class CardHistory
    {
        public int FromRow { get; set; }
        public int FromColumn { get; set; }
        public int OldRow { get; set; }
        public int OldColumn { get; set; }
        public int Dot { get; set; }
    }
}

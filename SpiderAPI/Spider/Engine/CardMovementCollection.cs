using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderAPI.Spider.Engine
{
    internal class CardMovementCollection
    {
        List<CardMovement> collection = new List<CardMovement>();

        public CardMovementCollection() { }

        public CardMovement this[int index]
        {
            get { return collection[index]; }
            set { collection[index] = value; }
        }

        public int Add(int Row, int Column, CardRank Rank, CardSuite Suite, int MoveSize)
        {
            collection.Add (new CardMovement {  Row = Row, Column = Column, Rank = Rank, Suite = Suite, MoveSize = MoveSize });
            
            return collection.Count;
        }

        public int AddTimeDelay()
        {
            collection.Add(new CardMovement { TimeDelay = true });

            return collection.Count;
        }

        public int RemoveLast()
        {
            if (collection.Count == 0)
                return 0;

            collection.RemoveAt(collection.Count - 1);

            return collection.Count;
        }

        public void RemoveAll()
        {
            collection.Clear ();
        }

        public int Count
        {
            get { return collection.Count;}
        }

        public bool IsTimeDelayOfLastItem
        {
            get
            {
                if (collection.Count == 0)
                {
                    return false;
                }

                return collection.Last().TimeDelay;
            }
        }

        public bool IsTimeDelayAt(int pos)
        {
            if (pos >=  collection.Count)
                return false;

            return collection[pos].TimeDelay;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SpiderAPI.Spider.Engine
{
    public enum CardSuite : int
    {
        None = 0,
        Spade = 1,
        Clubs = 2,
        Heart = 3,
        Diamond = 4,
    }

    public enum CardRank : int
    {
        None = 0,
        A = 1,
        _2 = 2,
        _3 = 3,
        _4 = 4,
        _5 = 5,
        _6 = 6,
        _7 = 7,
        _8 = 8,
        _9 = 9,
        _10 = 10,
        J = 11,
        Q = 12,
        K = 13,
        FaceDown = 15,
    }

    public class Card : ICloneable, IEquatable<Card>
    {
        private static string[] honours = { "J", "Q", "K" };
        private static string[] fills = { "White", "Black", "CornflowerBlue", "OrangeRed", "Orange" };
        private static char[] symbs = { ' ', '\u2660', '\u2663', '\u2665', '\u2666' };

        int cardDeck;
        CardSuite suite;
        CardRank rank;

        //Place hodler of cards
        //internal PlaceHolerCtrl PlaceHoderWindow { get; set; }
        internal CardSuite Suite 
        { 
            get { return suite; }
            set { suite = value; } 
        }
        internal CardRank Rank
        {
            get 
            { 
                if (this.FaceDown)
                {
                    return CardRank.FaceDown;
                }

                return rank; 
            }
            set
            {
                if (value == CardRank.None  ||
                    value == CardRank.FaceDown)
                {
                    throw new Exception("Check symbol logic");
                }

                rank = value;
            }
        }

        internal bool FaceDown { get; set; } = false;
        internal bool IsNone { get { return Rank == CardRank.None; } }
        internal bool IsNotNone { get { return Rank != CardRank.None || this.FaceDown; } }
        internal bool IsNotNoneFaceDown { get { return Rank != CardRank.None && !this.FaceDown; } }

        internal string SymbolChar { get { return Card.GetSymbolChar(this.Rank); } }
        internal char SuiteChar { get { return Card.GetSuiteChar(this.Suite); } }
        internal string SuiteColor { get { return Card.GetSuiteColor(this.Suite); } }
        internal int Top { get;set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public bool Equals(Card? other)
        {
            if (other == null)
                return false;

            return (this.Rank == other.Rank &&
                this.Suite == other.Suite);
        }

        internal int CardDeck
        {
            get { return cardDeck; }
            set
            {
                cardDeck = value;
                Suite = (CardSuite)(value % 4 + 1);
                Rank = (CardRank)(value % 13 + 1);
            }
        }

        internal Card(/*PlaceHolerCtrl PlaceHoderWindow*/)
        {
            //this.PlaceHoderWindow = PlaceHoderWindow;
            this.Suite = CardSuite.None;
            this.SetNone();
        }

        internal Card(/*PlaceHolerCtrl PlaceHoderWindow, */CardSuite Kind, CardRank Rank)
        {
            //this.PlaceHoderWindow = PlaceHoderWindow;
            this.Suite = Kind;
            this.Rank = Rank;
        }

        internal void MoveFrom(Card card)
        {
            this.Suite = card.Suite;
            this.rank = card.rank; //not use Rank
            this.FaceDown = card.FaceDown;

            card.SetNone();
            //card.Suite = CardSuite.None;
            card.FaceDown = false;
        }

        internal void SetNone()
        {
            this.rank = CardRank.None;
            this.FaceDown = false;
        }

        static internal string GetSymbolChar(CardRank Symbol)
        {
            if (Symbol == CardRank.A)
            {
                return "A";
            }
            else if ((int)Symbol > 10)
            {
                return honours[(int)Symbol - 11];
            }
            else
            {
                return ((int)Symbol).ToString();
            }
        }

        static internal char GetSuiteChar(CardSuite Suite)
        {
            return symbs[(int)Suite];
        }

        static internal string GetSuiteColor(CardSuite Suite)
        {
            return fills[(int)Suite];
        }

        static internal bool IsNext(CardRank symbolOrg, CardRank symbolNext)
        {
            return ((int)symbolOrg + 1) == (int)symbolNext;
        }
    }
}

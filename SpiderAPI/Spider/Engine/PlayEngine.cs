using NumpyDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SpiderAPI.Spider.Engine
{
    public class PlayEngine
    {
        const int TOTAL_CARDS_COUNT = 52;
        const int SIDE_CARDS_COUNT = 13;
        const int KIND_CARDS_COUNT = 4;
        const int MAX_ROW_COUNT = 80;

        const int MAX_PLAY_COLUMNS = 17;
        const int PLAY_COLUMNS = 10;
        const int ROW_BASE = 5;
        const int ROW_LIMIT_SIZE = 30;
        const int DealNext10Limit = 5;

        const int TimeDelayer = PLAY_COLUMNS;
        //string MarkOfpp = "pp";
        //const int facedown = 15;
        //const int HISTORY_SIZE = 1000;


        public string ID { get; private set; }
        protected int Width { get; set; } = 800;
        protected int Height { get; set; } = 500;
        public int Difficulty { get; set; } = 0;

        int widthPerCard = (int)(800 / 10.3);
        int heightPerCard = 500 / 33;
        int h33t30 = 500;   // h33 * 30
        //const int StepLimit = 0;

        Stack<CardHistory> cardsHistory = new Stack<CardHistory>();  

        //int col = 0;
        //int ia = 0;
        //int ijj = 0;
        //int ijoins,iijoins;
        //int j = 0;
        //int blankcolumns = 0;
        //int movecol = 0;
        //int n = 0;
        ////int rowe = 0;
        //int cc = 0;
        //int ct = 0;
        int dot = 0;
        int dott = 0;

        //int fragment_length = 0;
        //int xa = 0;
        //int ya = 0;
        //int jx = 0;
        //int DisplayCounter { get; set; } = 0;
        int OldColumn { get; set; } = 0;
        int OldRow { get; set; } = 0;
        //int cardrownumber = 0;
        //int cardcolumn = 0;
        //int cl = 0;
        //int rw = 0;
        //int column;
        //int row;
        bool GameStarted = false;
        //int RedAlert = 0;

        int oldr = 0;
        int oldc = 0;
        //int columnold = 0;
        //int rowold = 0;
        //int rowfordisplay = 0;
        //int columnfordisplay = 0;
        int fromcolumn = 0;
        int fromrow = 0;
        //int colautostart = 0;
        //int rowautostart = 0;  // card variables
        
        //int suitremoved = 0;
        //int DisplayCount = 0;
        
        //int columnformove = 0;
        //int totya = 0;
        int expander = PLAY_COLUMNS;

        //ndarray u = np.zeros(18, np.Int32);
        ndarray gaps = np.zeros(PLAY_COLUMNS, np.Int32);
        //ndarray dims = np.zeros(18, np.Int32); //not necessary, i => i
        List<CardSuite> removedsuit = new List<CardSuite>(); //CardSuite[] removedsuit = ZerosCardSuite(9);

        //card height on current column.
        ndarray compressor = np.zeros(PLAY_COLUMNS, np.Int32);
        //ndarray lastcard = np.zeros(PLAY_COLUMNS, np.Int32);
        //ndarray firstcard = np.zeros(PLAY_COLUMNS, np.Int32);
        //ndarray suitcard = np.zeros(PLAY_COLUMNS, np.Int32);
        int[] colmoves = InitInt(PLAY_COLUMNS);

        //ndarray histrec = np.zeros(  (1000, 5), np.Int32);
//        ndarray xi = np.zeros(23, np.Int32);
        ndarray by = np.zeros(  23 + 1, np.Int32);
        ndarray lth = np.zeros(  23 + 1, np.Int32);
        ndarray lx = np.zeros(  23 + 1, np.Int32);
        ndarray ly = np.zeros(  23 + 1, np.Int32);
        ndarray blanks_program = np.zeros(  23, np.Int32);

        ndarray ledgerow = np.zeros(  23, np.Int32);
        ndarray ledgecolumn = np.zeros(  22, np.Int32);
        //ndarray blankcolumn = np.zeros(  23, np.Int32);
        //ndarray movesz = np.zeros(  200, np.Int32);

        //ndarray cardsarray = np.zeros(  (80, 10, 2), np.Int32);  // Deal layout array
        //ndarray colms = np.zeros(  (MAX_ROW_COUNT, 10), np.Int32);
        //ndarray cardstore = np.zeros(  (200 + 400, 2), np.Int32);
        //ndarray positionstore = np.zeros(  (200 + 400, 2), np.Int32);
        CardMovementCollection cardMovements = new CardMovementCollection();

        //ndarray caxsx = np.zeros(  (12, 10, 2), np.Int32);
        Card[,] cards;// new Card[80, 10];
        
        int DealNext10 = 0;
        //int historycount = 0;
        int ilptr = 0;
        int endptr = 0;
        int expptr = 3 * 500 / 10;
        //int color = 0;
        int htx3div10 = (int)(3 * 500 / 10);
        int htdiv20 = (int)(500 / 20);
        //ndarray cds = np.zeros(53, np.Int32);
        ndarray dck = np.zeros(105, np.Int32);
        //ndarray deck = np.zeros(105, np.Int32);

        int oldmasthead = -1;
        //int repsuity = 0;

        //PlaceHolerCtrl wdow;

        public PlayEngine(string ID)
        {
            this.ID = ID;

            //this.Width = this.Width;
            //this.Height = this.Height;

            CalcLayout();

            this.cards = InitCard(/*wdow, */MAX_ROW_COUNT, PLAY_COLUMNS);

            Difficulty = 9;
        }

        public PlayEngine(/*PlaceHolerCtrl _wdow, */int width, int height)
        {
            //wdow = _wdow;

            this.Width = width;
            this.Height = height;

            CalcLayout();

            //for (int jm = 0;jm < MAX_PLAY_COLUMNS; jm++)
            //{
            //    if(jm < PLAY_COLUMNS)
            //    {
            //        dims[jm] = jm;
            //    }
            //    else
            //    {
            //        dims[jm] = 0;
            //    }
            //}

            this.cards = InitCard(/*wdow, */MAX_ROW_COUNT, PLAY_COLUMNS);

            Difficulty = 9;
        }

        int GetDim(int column)
        {
            while (column < 0)
            {
                column += 18;
            }

            return column >= PLAY_COLUMNS ? 0 : column;
        }

        void CalcLayout()
        {
            this.widthPerCard = (int)(this.Width / (PLAY_COLUMNS + .3f));
            this.heightPerCard = this.Height / 33;
            this.h33t30 = this.Height;   // h33 * 30

            this.htx3div10 = (int)(3 * this.Height / 10);
            this.htdiv20 = (int)(this.Height / 20);
        }

        public void _New(APIInitData data)
        {
            //wdow.CreateRectangle(0, 0, Width, Height, "Green");
            data.Columns = PLAY_COLUMNS;

            InitNextCard(); //nextcard = 0;
            this.cardsHistory.Clear();
            DealNext10 = 0;

            Shuffle(data);

            int row;
            int i80;

            removedsuit.Clear();

            for (int col=0;col< PLAY_COLUMNS;col++)
            {
                compressor[col] = heightPerCard;
                gaps[col] = htdiv20;

                row = ROW_BASE;
                i80 = GetDim(col);// (int)dims[i];
                
                while (cards[row, i80].IsNotNone)//(int)cardsarray[row,i80, 0] > 0)
                {
                    storefuturecardmovement(row, col, 1);
                    row ++;
                }
                
                timedelay();
            }

            APIMoveData moveData = new APIMoveData() { ID = data.ID };

            DisplayAll(moveData);

            data.movement.AddRange(moveData.movement);
            GameStarted = true;
        }

        private void Shuffle(APIInitData data)
        {
            int [] newdig = new int[SIDE_CARDS_COUNT + 1];
            List<int> _card = new List<int>();
            List<int> _dack = new List<int>();

            int n = 0;

            for (int kind = 0; kind < KIND_CARDS_COUNT; kind++)
            {
                for(int j =1;j< SIDE_CARDS_COUNT + 1; j++)
                {
                    n ++;
                    _card.Add(n);
                    newdig[j-1] = 0;
                }
            }
            
            int _u;
            int newrand;

#if DEBUG
            Random rnd = new Random(78); // random.randint(0, 100)  #78,45,1,22,27,43,80,84,31,46,84,49,90,89,28,16,60,51,22,6,27,77,87,96,48
#else
            Random rnd = new Random(); // random.randint(0, 100)  #78,45,1,22,27,43,80,84,31,46,84,49,90,89,28,16,60,51,22,6,27,77,87,96,48
#endif
            while (_card.Count > 0)
            {
                newrand = 0;
                
                while(newrand == 0)
                {
                    n = rnd.Next(0, _card.Count - 1);
                    _u = _card[n] % 13; // card[n] is original ordered pack

                    if (_card.Count > TOTAL_CARDS_COUNT - (Difficulty+1) )
                    {
                        if ((int)newdig[_u] > 0)
                        {
                            newrand = 0;
                        }
                        else
                        {
                            newdig[_u] = _u;    // selects first n cards to be different
                            newrand = 1;
                            _dack.Add(_card[n]);

                            _card[n] = _card.Last();
                            _card.RemoveAt(_card.Count - 1);
                        }
                        newrand = 1;
                    }
                    else
                    {
                        _dack.Add(_card[n]);

                        _card[n] = _card.Last();
                        _card.RemoveAt(_card.Count - 1);

                        newrand = 1;
                    }
                }
            }

            for(int k =1;k< TOTAL_CARDS_COUNT + 1; k++)
            {
                dck[k] = _dack[k-1];
                dck[k+52] = (int)_dack[k-1];
            }

            for (int k = 1; k <11; k++)
            {
                dck[64+ k] = (int)dck[k + 44];
                dck[44+ k ] = (int)_dack[k-1]; // initial exposed cards
                dck[k] = (int)_dack[k+12-1]; // 64
            }

            InitNextCard(); //nextcard = 0;
            
            int il;
            for(int jl =0;jl < PLAY_COLUMNS; jl++)
            {
                il = ROW_BASE;
                while (cards[il, jl].IsNotNone) // (int)cardsarray[il, jl, 0] > 0)
                {
                    cards[il, jl].SetNone();    //cardsarray[il, jl, 0] = 0; // card space empty
                    il ++;
                }
                il = ROW_BASE;
                
                while(il < MAX_ROW_COUNT)
                {
                    cards[il, jl].Top = heightPerCard * (il - ROW_BASE) + ROW_BASE; // set card display spacing
                    il ++;
                }

                //if(jl < 4)
                //{
                //    lastcard[jl] = ROW_BASE + 5;
                //}
                //else
                //{
                //    lastcard[jl] = ROW_BASE + 4;
                //}
            }
            
            int rowdepth = ROW_BASE;
            int jlo;

            while (rowdepth < ROW_BASE + 7)
            {
                for (int jl = 0; jl < PLAY_COLUMNS; jl++)
                {
                    jlo = jl;
                    if(NextCard < 44 || (NextCard < 54 &&  rowdepth == ROW_BASE + 7))
                    {
                        //cardsarray[rowdepth, jlo, 0] = facedown;
                        //caxsx[rowdepth, jlo, 0] = (int)dck[nextcard + 1] % 13 + 1;
                        //caxsx[rowdepth, jlo, 1] = (int)dck[nextcard + 1] % 4 + 1;
                        cards[rowdepth, jlo].CardDeck = (int)dck[NextCard + 1];
                        cards[rowdepth, jlo].FaceDown = true;
                        
                        IncrementNextCard();// nextcard  ++;
                    }else if(NextCard < 54)
                    {
                        //cardsarray[rowdepth, jl, 0] = (int)dck[nextcard + 1] % 13 + 1;
                        //cardsarray[rowdepth, jl, 1] = (int)dck[nextcard + 1] % 4 + 1;
                        cards[rowdepth, jl].CardDeck = (int)dck[NextCard + 1];
                        cards[rowdepth, jlo].FaceDown = false;

                        IncrementNextCard();//nextcard ++;
                    }
                }
                rowdepth ++;
            }

            data.DealNext10 = this.DealNext10;
            //wdow.CreateRectangle(Width - 40, 5, Width - 5, 30, "Green");
            //display remain card count.
            //wdow.CreateText(Width - 25, 15, "White", string.Format("{0}", (50 - 10 * DealNext10)), "Times", 15, true);
        }

        //void lstcard(int rw,int cl)
        //{
        //    lastcard[cl] = rw;
        //}

        int GetLastCardPos(int Column)
        {
            if (Column >= PLAY_COLUMNS)
            {
                return -1;
            }

            for(int row = ROW_BASE;row < MAX_ROW_COUNT;row++)
            {
                if (this.cards[row, Column].IsNone)
                {
                    return row - 1;
                }
            }

            return ROW_BASE - 1;
        }

        CardSuite GetLastCardSuite(int Column)
        {
            int row = GetLastCardPos(Column);
            
            return cards[row, Column].Suite;
        }

        bool IsColumnBlank(int Column)
        {
            return GetLastCardPos(Column) == ROW_BASE - 1;
        }

        bool IsColumnNotBlank(int Column)
        {
            return GetLastCardPos(Column) > ROW_BASE - 1;
        }

        int GetFirstCardPos(int Column)
        {
            int findRow = GetLastCardPos(Column);
            CardSuite suite = cards[findRow, Column].Suite;

            if (cards[ROW_BASE, Column].IsNotNone)
            {
                while (cards[findRow - 1, Column].Suite == suite && Card.IsNext(cards[findRow - 1, Column].Rank, cards[findRow, Column].Rank))
                {
                    findRow --;
                }
            }

            return findRow;
        }

        void storefuturecardmovement(int rowfordisply,int columnfordisply,int movesiz)
        {
            int a = GetDim(columnfordisply);//(int)dims[columnfordisply];
            
            if (cardMovements.Count > 150)
            {
                //a = a;
            }

            //DisplayCounter++;
            //Debug.Print("storefuturecardmovement {0}", cardMovements.Count);
            //positionstore[DisplayCounter, 0] = rowfordisply; 
            //positionstore[DisplayCounter, 1] = columnfordisply;
            //cardsarray[rowfordisply, a];
            //cardstore[DisplayCounter, 0] = cards[rowfordisply, a].Rank;
            //cardstore[DisplayCounter, 1] = cards[rowfordisply, a].Suite;
            //movesz[DisplayCounter] = movesiz;

            cardMovements.Add(rowfordisply, columnfordisply, cards[rowfordisply, a].Rank, cards[rowfordisply, a].Suite, movesiz);
            
            Debug.Print("storefuturecardmovement {0}", cardMovements.Count);
        }

        void timedelay()
        {
            //if ((int)positionstore[DisplayCounter,1] != 10 && DisplayCounter < 199)
            if (cardMovements.IsTimeDelayOfLastItem && cardMovements.Count < 199)
            {
                //DisplayCounter++;

                cardMovements.AddTimeDelay();
                Debug.Print("timedelay {0}", cardMovements.Count);

                //positionstore[DisplayCounter, 1] = TimeDelayer;
            }
        }

        void DisplayRedAlert(int rowfordisplay, int columnfordisplay)
        {
            int columnfordisplayw10;
            //bool isJumper;
            int displayCount = 0;
            int RedAlert = 1;

        //afterDelay:
            //isJumper = false;
            //if ((int)positionstore[displayCount, 1] == TimeDelayer)

            while (RedAlert>0)
            {
                if (cardMovements.IsTimeDelayAt(displayCount))
                {
                    displayCount++;
                }

                columnfordisplayw10 = columnfordisplay * widthPerCard;
          
                if(RedAlert == 1 && cards[rowfordisplay, columnfordisplay].IsNotNoneFaceDown)//(int)cardsarray[rowfordisplay,columnfordisplay,0] != 0 && (int)cardsarray[rowfordisplay,columnfordisplay,0] != facedown)
                {
                    //wdow.CreateRectangle(columnfordisplayw10 + 7, cards[rowfordisplay, columnfordisplay].Top, columnfordisplayw10 + widthPerCard - 3, cards[rowfordisplay, columnfordisplay].Top + 13, "Red");
                    RedAlert = 2;
                    //System.Threading.Thread.Sleep(200);
                    continue;
                }
                else if (RedAlert == 2)
                {
                    //wdow.CreateRectangle(columnfordisplayw10 + 4, cards[rowfordisplay, columnfordisplay].Top, columnfordisplayw10 + widthPerCard, cards[rowfordisplay, columnfordisplay].Top + 15, "White");

                    string color = cards[rowfordisplay, columnfordisplay].SuiteColor;//(int)cardsarray[rowfordisplay, columnfordisplay, 1];
                    string suiteChar = cards[rowfordisplay, columnfordisplay].SuiteChar.ToString();

                    //draw when click
                    //wdow.CreateText(columnfordisplay * widthPerCard + 8, cards[rowfordisplay, columnfordisplay].Top - 1, color, cards[rowfordisplay, columnfordisplay].SymbolChar, "Times", 10, true);
                    //wdow.CreateText(columnfordisplay * widthPerCard + 20, cards[rowfordisplay, columnfordisplay].Top - 2, color, suiteChar, "Times", 12, true);

                    if (expander != columnfordisplay && cards[rowfordisplay + 1, columnfordisplay].IsNotNone)//(int)cardsarray[rowfordisplay + 1, columnfordisplay, 0] > 0)
                    {
                        //wdow.CreateLine(columnfordisplay * widthPerCard + 2, cards[rowfordisplay, columnfordisplay].Top + 14, columnfordisplay * widthPerCard + MAX_ROW_COUNT, cards[rowfordisplay, columnfordisplay].Top + 14,
                        //                1, "Black");
                    }
                    RedAlert = 0;
                    //System.Threading.Thread.Sleep(20);
                    continue;
                }
                else
                {
                    break;
                }
            }
        }

        void DisplayAll(APIMoveData? data)
        {
            if (data != null)
            {
                for (int i = 0; i < cardMovements.Count; i++)
                    data.movement.Add(cardMovements[i]);
            }

            int displayCount = 0;

            //afterDelay:
            //if ((int)positionstore[displayCount, 1] == TimeDelayer)
            if (cardMovements.IsTimeDelayAt(displayCount))
            {
                displayCount++;
            }

            //while (displayCount < DisplayCounter)
            while (displayCount < cardMovements.Count)
            {
                if (cardMovements.IsTimeDelayAt(displayCount))
                {
                    displayCount++;

                    if (displayCount >= cardMovements.Count)
                        break;
                }

                //if ((int)positionstore[displayCount, 1] == 10)
                if (cardMovements.IsTimeDelayAt(displayCount))
                {
                    displayCount++;
                    //System.Threading.Thread.Sleep(20);
                    continue;
                }

                //while ((int)positionstore[displayCount, 1] < 10 && displayCount < DisplayCounter + 1)
                while (!cardMovements.IsTimeDelayAt(displayCount) && displayCount < cardMovements.Count)
                {
                    //int cardrownumber = (int)positionstore[displayCount, 0];
                    //int cardcolumn = (int)positionstore[displayCount, 1];
                    //if ((int)cardstore[displayCount, 0] != 0 || (int)cardstore[displayCount, 1] != 0)
                    if (cardMovements[displayCount].Suite != CardSuite.None || cardMovements[displayCount].Rank != CardRank.None)
                    {
                        //displaycard(cardrownumber, cardcolumn, (CardRank)cardstore[displayCount, 0], (CardSuite)cardstore[displayCount, 1], (int)movesz[displayCount]);
                        DisplayCard(cardMovements[displayCount].Row, cardMovements[displayCount].Column, cardMovements[displayCount].Rank, cardMovements[displayCount].Suite, cardMovements[displayCount].MoveSize);
                        //displaycard(cardrownumber, cardcolumn, (CardSymbol)cardstore[DisplayCount, 0], (CardSuite)cardstore[DisplayCount, 1], (int)movesz[DisplayCount]);
                    }
                    displayCount++;
                }
            }

            cardMovements.RemoveAll();
        }

        //void storefuturecardmovementx(int rowfordisply,int columnfordisply,int movesiz)
        //{
        //    int a = GetDim(columnfordisply);// (int)dims[columnfordisply];
        //    //displaycard(rowfordisply, columnfordisply,(int)cardsarray[rowfordisply, a, 0],(int)cardsarray[rowfordisply, a, 1], movesiz);
        //    displaycard(rowfordisply, columnfordisply, cards[rowfordisply, a].Rank, cards[rowfordisply, a].Suite, movesiz);
        //}

        void DisplayCard(int Row, int Column, CardRank Rank, CardSuite Suite, int MoveSize)
        {
            for (int i = 0; i < MoveSize;i++)
            {
                PositionCardOnTable(Row + i, Column, Rank - i, Suite);
            }
        }

        void PositionCardOnTable(int CardRow, int CardColumn, CardRank Rank, CardSuite Suite)
        {
            int cdclw10 = CardColumn * widthPerCard;
            
            if(Rank == CardRank.None)
            {
                //wdow.CreateRectangle(cdclw10 + 3, cards[CardRow, CardColumn].Top, cdclw10 + widthPerCard + 2, Height + 20, "Green");
            }
            else
            {
                //wdow.CreateRectangle(cdclw10 + 3, cards[CardRow, CardColumn].Top - 1, cdclw10 + widthPerCard, cards[CardRow,CardColumn].Top + 100, "White",1);
                
                if(Rank == CardRank.FaceDown)//(rank == facedown)
                {
                    //wdow.CreateRectangle(cdclw10 + 7, cards[CardRow, CardColumn].Top + 5, cdclw10 + 73, cards[CardRow, CardColumn].Top + 20, "Brown", 1);
                }
                else
                {
                    CardDetail(CardRow, CardColumn, Rank, Suite);
                }
            }
        }

        //Display at firt draw
        void CardDetail(int Row, int Column, CardRank Rank, CardSuite Suite)
        {
            int cdclw10 = Column * widthPerCard;

            string color = Card.GetSuiteColor(Suite);
            string suite = Card.GetSuiteChar(Suite).ToString();

            //wdow.CreateText(cdclw10 + 20, cards[Row, Column].Top - 3, color, suite, "Times", 12, true);
            //wdow.CreateText(cdclw10 + 36, cards[Row, Column].Top + 25, color, suite, "Times", 25, true);

            string p = Card.GetSymbolChar(Rank);

            if (Rank == CardRank._10)
            {
                //wdow.CreateText(cdclw10 + 11, cards[Row, Column].Top - 1, color, "0", "Times", 10, true);
                //wdow.CreateText(cdclw10 + 6, cards[Row, Column].Top - 1, color, "1", "Times", 10, true);
                //wdow.CreateText(cdclw10 + 14, cards[Row, Column].Top + 25, color, "0", "Times", 25, true);
                //wdow.CreateText(cdclw10 + 1, cards[Row, Column].Top + 25, color, "1", "Times", 25, true);
            }
            else
            {
                //wdow.CreateText(cdclw10 + 8, cards[Row, Column].Top - 1, color, p, "Times", 10, true);
                //wdow.CreateText(cdclw10 + 9, cards[Row, Column].Top + 25, color, p, "Times", 25, true);
            }
        }

        void ExpandColumn(int arownumb, int acolumnnumber, int aheight)
        {
            gaps[acolumnnumber] = aheight;
            int a80 = GetDim(acolumnnumber);//(int)dims[acolumnnumber];
            int a = a80;
            int ij;

            if (arownumb == ROW_BASE)  // no expansion but maybe compression
            {
                ij = arownumb;
                while(ij < MAX_ROW_COUNT)
                {
                    cards[ij, a].Top = aheight * (ij - ROW_BASE) + ROW_BASE;
                    ij ++;
                }
                ij = arownumb;
                while (cards[ij, a].IsNotNone)//(int)cardsarray[ij,a,0]>0)
                {
                    PositionCardOnTable(ij, acolumnnumber, cards[ij, a].Rank, cards[ij, a].Suite);//(int)cardsarray[ij, a, 0], (int)cardsarray[ij, a, 1]);
                    ij ++;
                }
                while (cards[ij,a].Top < Height)
                {
                    PositionCardOnTable(ij, acolumnnumber, CardRank.None, cards[ij, a].Suite);//0, (int)cardsarray[ij, a, 1]);
                    ij ++;
                }
            }
            else
            {
                ij =  ROW_BASE;
                while (cards[ij,a].Top < expptr)
                {
                    ij ++;
                }
                ij --;
                int ij13 = ij + 14;

                while (ij < ij13)
                {
                    cards[ij + 1, a].Top = cards[ij, a].Top + htdiv20;
                    PositionCardOnTable(ij, acolumnnumber, cards[ij, a].Rank, cards[ij, a].Suite);//(int)cardsarray[ij, a, 0], (int)cardsarray[ij, a, 1]);
                    ij ++;
                }
            }
        }
    
        internal void OnLeftClick(int X, int Y, APIMoveData moveData)
        {
            if (!this.GameStarted)
            {
                return;
            }

            if (Y < 5)
            {
                Y = 5;
            }
            
            int col = (int)X / widthPerCard;
            int rw = ROW_BASE; 
            int rowe = ROW_BASE;

            //bug hit. exceed when above 10
            if (col >= PLAY_COLUMNS)
            {
                return;
            }

            //      if (expander == column) deexpand();
            if (expander < PLAY_COLUMNS && col != expander) //if a different column is expanded, then deexpand it
            {
                deexpand();
            }

            int i = GetLastCardPos(col);//(int)lastcard[cl]; 
            if (cards[i, col].Top > Height - 30)
            {
                int h33t30cardrownumber = h33t30 / i;
                ExpandColumn(ROW_BASE, col, h33t30cardrownumber);
                compressor[col] = h33t30cardrownumber;
                rw = ROW_BASE;
            }
            else
            {
                int mposy = Y;
                if (expander == PLAY_COLUMNS && (int)compressor[col] == heightPerCard)
                {
                    rw = (int)((mposy + 69) / 15);
                    i = 6;
                    while(i > 0 && cards[rw, col].IsNone)//(int)cardsarray[rw, cl, 0] == 0)
                    {
                        i --;
                        rw --;
                    }
                    //          int row1 = lastcard(cl);
                    //        if (rw < row1 + 6) rw = row1;
                    if (rw < ROW_BASE)
                    {
                        rw = ROW_BASE;
                    }
                    cardfrontclick(rw, col, moveData);
                }
                else
                {
                    if(expander == col) //if column expanded
                    {
                        if(Y < expptr)
                        {
                            rw = ROW_BASE;
                        }
                        else
                        {
                            rw = (Y - htx3div10) / (int)gaps[col] + ilptr;
                        }

                        //CurrentColumn = col;
                        //int row1 = rw;
                        deexpand();
                        autos(rw, col, moveData);
                        DisplayAll(moveData);
                        col = -1;
                    }
                    else
                    {
                        while(Y > cards[rowe, col].Top)
                        {
                            rowe ++;
                        }
                        rw = rowe - 1;
                        while(cards[rw, col].IsNone && rw + 7 > rowe)//((int)cardsarray[rw, cl, 0] == 0 && rw +7 > rowe)
                        {
                            rw --;
                        }
                    }
                }

                if (col > -1)
                {
                    cardfrontclick(rw, col, moveData);
                }
            }
        }

        void deexpand()
        {
            if(expander < PLAY_COLUMNS)
            {
                int ah;
                int j = expander;
                int il = endptr;

                if(il > 30)
                {
                    ah = heightPerCard * 30 / il;
                }
                else
                {
                    ah = heightPerCard;
                }

                compressor[j] = ah;
                ExpandColumn(ROW_BASE, j, ah);
                gaps[j] = ah;
                expander = PLAY_COLUMNS;
            }
        }

        int findEmptyRow(int row,int col) //find0
        {
            while (cards[row, col].IsNotNone)//(int)cardsarray[row2,_col,0] != 0)
            {
                row++;
            }
            return row;
        }
    
        void autos(int _row, int column, APIMoveData moveData)
        {
            ndarray xi = np.zeros(23, np.Int32);
            bool suitremoved = false;
            int kill = 0;
            CardSuite samesuit = 0;
            int ia = 0;
            int colautostart = column;
            int r80 = GetDim(column);//(int)dims[CurrentColumn];
            int rowautostart = _row; // card variables
            List<int> blankcolumn = new List<int>();
   //         if((column == 0 ) && (_row < 7))
   //         { 
   //             ia = ia; 
   //         }

            if (cards[ROW_BASE, column].IsNotNone)//(int)cardsarray[ROW_BASE,CurrentColumn,0] != 0)
            {
                ia = GetLastCardPos(r80);// (int)lastcard[r80];
                int samesuitlgth = 1;
                int cc = _row;
                
                samesuit = cards[cc, column].Suite;//(int)cardsarray[cc, CurrentColumn, 1];
                
                while (cc<ia && Card.IsNext(cards[cc + 1, column].Rank, cards[cc, column].Rank))//(int)cardsarray[cc+1,CurrentColumn,0] +1 == (int)cardsarray[cc,CurrentColumn,0])
                {
                    samesuitlgth ++;
                    cc ++;
                    
                    //if ((int)cardsarray[cc, CurrentColumn, 1] != samesuit) samesuit = 0;
                    if (cards[cc, column].Suite != samesuit)
                    {
                        samesuit = CardSuite.None;
                    }
                }

                if (cc != ia)
                {
                    kill = 1;
                }

                if(ia - _row == 12 && kill == 0)
                {
                    suitremoved = RemoveSuite(_row, column, moveData);
      //              if ((int)compressor[column] > h33) deexpand();
                }
                if (suitremoved)
                {
                    if ((int)compressor[column] > heightPerCard)
                    {
                        deexpand();
                    }

                    column = -1;
                    kill = 1;
                }
            }
            else
            {
                kill = 1;
            }

            fromrow = _row;
            fromcolumn = column;

            CardRank fromrank = cards[fromrow, fromcolumn].Rank;//(int)cardsarray[fromrow, fromcolumn, 0];
            if (kill ==  0)
            {
                column = repeatcol(column);
            }
            r80 = GetDim(column);//(int)dims[CurrentColumn];
            if (column == -1)
            {
                kill = 1;
            }
            else
            {
                int cji;
                if(cards[ROW_BASE, column].IsNone)//(int)cardsarray[ROW_BASE, CurrentColumn, 0] == 0)
                {
                    _row = ROW_BASE - 1;
                    cji = (int)CardRank.FaceDown;//facedown;
                }
                else
                {
                    _row = GetLastCardPos(r80);//(int)lastcard[r80];
                    //cji = (int)cardsarray[_row, CurrentColumn, 0] - 1;
                    cji = (int)cards[_row, column].Rank - 1;
                }

                //if (_row > ROW_BASE - 1 || (int)cardsarray[fromrow, fromcolumn, 0] == 0)
                //{
                //    ct = fromrow;
                //}
                int found;
                //if(((int)cardsarray[fromrow, fromcolumn, 0] == cji || (int)cardsarray[_row, CurrentColumn, 0] == 0) && kill == 0)
                if (((int)cards[fromrow, fromcolumn].Rank == cji || cards[_row, column].IsNone) && kill == 0)
                {
                    found = 1;
                }
                else
                {
                    kill = 1;
                    found = 0;
                }
                
                if (suitremoved)
                {
                    found = 0;
                }

                if(found ==1)
                {
                    dott = 0;
                    int blankcolumns = FindLedges(column, blankcolumn);
                    
                    ledgecolumn[(int)cards[fromrow, fromcolumn].Rank + 1] = column;//ledgecolumn[(int)cardsarray[fromrow, fromcolumn, 0] + 1] = CurrentColumn;
                    ia = blankcolumns * blankcolumns - blankcolumns + 1;  // dynamic potential of blank columns

                    if (blankcolumns == 0)
                    {
                        ia = 0;
                    }
                    //cc = fromrow;
                    //while(cc<=ct)
                    //{
                    //    cc += 1;
                    //}
                    int xa = 1;
                    int ya = 0;

                    lx[xa] = fromrow;
                    ly[xa] = fromcolumn;

                    cutstep();

                    found = steps(ia, found, ref xa, ref ya);

                    //if (ya > 1 && totya < StepLimit + 1) totya += ya;
                    if (found == 0) kill = 1;
                    if (found == 1)
                    {
                        oldr = _row + 1;
                        oldc = column;
                    }
                    int _short = GetLastCardPos(fromcolumn) - fromrow + 1;//(int)lastcard[fromcolumn] - fromrow + 1;
                    if (found == 1)
                    {
                        if (samesuit != samesuit)
                        {

                        }
                        else
                        {
                            for(int jk=1;jk<ya+1;jk++)
                            {
                                blanks_program[jk] = 0;
                                xi[jk] = 0;
                            }

                            int yy = ya;
                            
                            by[0] = 1;
                            ly[0] = 10;
                            xa = 1;
                            
                            while(xa < ya)
                            {
                                int j = 0;
                                ia = 0;
                                while((int)by[xa] == 0)
                                {
                                    ia ++;
                                    j += (int)lth[xa];
                                    xi[ia] = xa;
                                    xa ++;
                                }
                                if(ia == blankcolumns + 1)
                                {
                                    blanks_program[(int)xi[2]] = 1;
                                }else if (ia == blankcolumns + 2)
                                {
                                    blanks_program[(int)xi[2]] = 1;
                                    blanks_program[(int)xi[4]] = 1;
                                }
                                else if (ia == blankcolumns + 3)
                                {
                                    blanks_program[(int)xi[2]] = 1;
                                    blanks_program[(int)xi[4]] = 2;
                                }
                                else if (ia == blankcolumns + 4)
                                {
                                    blanks_program[(int)xi[2]] = 1;
                                    blanks_program[(int)xi[4]] = 2;
                                    blanks_program[(int)xi[6]] = 1;
                                }
                                else if (ia == blankcolumns + 5)
                                {
                                    blanks_program[(int)xi[2]] = 1;
                                    blanks_program[(int)xi[4]] = 2;
                                    blanks_program[(int)xi[6]] = 1;
                                    blanks_program[(int)xi[8]] = 1;
                                }
                                else if (ia == blankcolumns + 6)
                                {
                                    blanks_program[(int)xi[2]] = 1;
                                    blanks_program[(int)xi[4]] = 2;
                                    blanks_program[(int)xi[6]] = 1;
                                    blanks_program[(int)xi[8]] = 2;
                                }
                                else if (ia == blankcolumns + 7)
                                {
                                    blanks_program[(int)xi[2]] = 1;
                                    blanks_program[(int)xi[4]] = 2;
                                    blanks_program[(int)xi[6]] = 1;
                                    blanks_program[(int)xi[8]] = 2;
                                    blanks_program[(int)xi[10]] = 1;
                                }
                                else if (ia == blankcolumns + 8)
                                {
                                    blanks_program[(int)xi[3]] = 1;
                                    blanks_program[(int)xi[5]] = 2;
                                    blanks_program[(int)xi[6]] = 3;
                                    blanks_program[(int)xi[7]] = 1;
                                    blanks_program[(int)xi[9]] = 2;
                                    blanks_program[(int)xi[11]] = 1;
                                }
                                xa ++;
                            }
                            
                            xa = 0;
                            
                            while(xa < yy)
                            {
                                xa ++;
                                int columnformove = GetDim((int)ly[xa]);//(int)dims[ly[xa]];
                                //OldColumn = (int)ledgecolumn[(int)cardsarray[lx[xa], columnformove, 0] + 1];
                                OldColumn = (int)ledgecolumn[(int)cards[(int)lx[xa], columnformove].Rank + 1];
                                if (OldColumn == -1)
                                {
                                    xa = mptyshlf(xa, blankcolumn);
                                    timedelay();
                                }
                                else
                                {
                                    OldRow = (int)ledgerow[OldColumn] - 1;
                                    movecard(xa, OldRow, OldColumn);
                                    timedelay();
                                    OldRow += (int)lth[xa];
                                    int xxx = xa;
                                    while((int)by[xa - 1] == 0)
                                    {
                                        xa = fillshlf(xa, blankcolumn);
                                        timedelay();
                                    }
                                    xa = xxx;
                                }
                            }

                            int olru = OldColumn;
                            int olcu = OldRow;

                            while(xa > 0)
                            {
                                while (xa > 0 && (int)ly[xa] == OldColumn)
                                {
                                    xa--; //-= 1;
                                }
                                int xx = xa;
                                if (xa > 0 && (int)ly[xa] != (int)ly[xa - 1])
                                {
                                    OldColumn = olru;
                                    OldRow = olcu;
                                    OldRow = GetLastCardPos(GetDim(OldColumn));//(int)lastcard[GetDim(OldColumn)];//(int)dims[OldColumn]];
                                    movecard(xa, OldRow, OldColumn);
                                    timedelay();
                                    OldRow+= (int)lth[xa];
                                }
                                else if (xa > 0)
                                {
                                    
                                    while ((int)ly[xx] == (int)ly[xa] && xa > 0)
                                    {
                                        xa--; //-= 1;
                                    }
                                    xa++;//+= 1;
                                    while(xa < xx)
                                    {
                                        xa = mptyshlf(xa, blankcolumn);
                                        timedelay();
                                        xa ++;
                                    };
                                    OldColumn = olru;
                                    int ru80 = GetDim(OldColumn);//(int)dims[OldColumn];
                                    OldRow = olcu;

                                    OldRow = GetLastCardPos(ru80);//(int)lastcard[ru80];
                                    movecard(xa, OldRow, OldColumn);
                                    timedelay();
                                    OldRow += (int)lth[xa];
                                    while((int)by[xa - 1] == 0)
                                    {
                                        xa = fillshlf(xa, blankcolumn);
                                    }
                                    timedelay();
                                    OldColumn = olru;
                                }
                            }
                            //lastcard[CurrentColumn] =(int) lastcard[CurrentColumn] + (int)lastcard[colautostart] - fromrow + 1;
                            ///lastcard[fromcolumn] = fromrow - 1;
                            tidyup(column);
                            //int n = 1;
                         
                            //cc = _row;
                            if(found == 1 && !suitremoved)
                            {
                                history(fromrow, fromcolumn, _row, OldColumn, dott);
                            }
                        }
                    }
                }
            }

            if(kill == 1)
            {
                if(expander == PLAY_COLUMNS)
                {
                    //rowfordisplay = rowautostart;
                    //columnfordisplay = colautostart;
                    DisplayRedAlert(rowautostart, colautostart);
                }
                else
                {
                    ExpandColumn(rowautostart, colautostart, heightPerCard);
                }
            }
        }

        void tidyup(int column)
        {
            int cardrownumber = GetLastCardPos(column); //(int)lastcard[CurrentColumn];

            if(cardrownumber > ROW_LIMIT_SIZE)
            {
                int h33t30cardrownumber = h33t30/ cardrownumber;
                ExpandColumn(ROW_BASE, column, h33t30cardrownumber);
                compressor[column] = h33t30cardrownumber;
            }
            else if((int)compressor[column] > heightPerCard)
            {
                ExpandColumn(ROW_BASE, column, heightPerCard);
                compressor[column] = heightPerCard;
            }

            if (dott == 1)
            {
                dot = 1;
            }
        }

        void cardfrontclick(int row, int column, APIMoveData? moveData)
        {
            //CurrentColumn = _cl;
            //CurrentRow = _rw;
            //int ia = CurrentRow; 
            int cardcolumn = column;
            //int r80 = (int)dims[_cl];
            int ia = GetLastCardPos(column) + 1; //(int)lastcard[_cl] + 1; // find address of bottom card plus 1
            endptr = ia;
            if ((int)compressor[column] == heightPerCard)
            {
                autos(row, column, moveData); //compute whether move legal
            }
            else
            {
                //if bottom card clicked or blank column
                if (row > ia - 2 || cards[row, column].FaceDown) //(int)cardsarray[CurrentRow, _cl, 0] == facedown) 
                {
                    //then process
                    autos(row, column, moveData);
                    //if column was expanded then restore column to normal spacing or compression
                    deexpand(); 
                }
                else
                {
                    //note column to be expanded
                    expander = cardcolumn;
                    //f clicked card not within 13 of bottom card
                    if (ia > row + 13)
                    {//show where expansion will start
                        ia = row;
                    }
                    else
                    {//else bottom 13 cards will be expanded
                        ia -= 13;
                    }

                    // store pointer to start of expansion
                    ilptr = ia;
                
                    int il = ia; //set counter  1682-1692 indented 07/12/23
                    
                    // set gap for expansion to the form height divided by 20
                    gaps[cardcolumn] = htdiv20;
                    //reset the form array pointers to allow expansion
                    while (cards[il, cardcolumn].IsNotNone)//((int)cardsarray[il, cardcolumn, 0] > 0)
                    {
                        cards[il, cardcolumn].Top = (int)gaps[cardcolumn] * (il - ia) + htx3div10;
                        il ++;
                    }
                    
                    il = ia; //set counter  
                    //store expansion until display
                    while (cards[il, cardcolumn].IsNotNone && il < ia + 14)//((int)cardsarray[il, cardcolumn, 0] > 0 && il < ia + 14)
                    {
                        PositionCardOnTable(il, cardcolumn, cards[il, cardcolumn].Rank, cards[il, cardcolumn].Suite);// (int)cardsarray[il, cardcolumn, 0], (int)cardsarray[il, cardcolumn, 1]);
                        il ++;
                    }
                }
            }

            //suitremoved = 0;
            DisplayAll(moveData);
        }
    
        bool RemoveSuite(int _row, int colum, APIMoveData moveData)
        {
            bool isSuiteRemoved = false;
            CardSuite suite = cards[_row, colum].Suite;//(int)cardsarray[_row, colum, 1];
            
            int ij = 0;
            while (cards[_row + ij, colum].Suite == suite && ij < SIDE_CARDS_COUNT)//(int)cardsarray[_row+ij,colum,1] == clr && ij<13)
            {
                ij ++;
            }

            if(ij == SIDE_CARDS_COUNT)
            {
                int rowfordisplay =  0, columnfordisplay = 0;

                removedsuit.Add(suite);//removedsuit[ij] = suite;
                showicon(suite);

                OldColumn = removedsuit.Count - 1 + 10;
                
                for(ij =0; ij< SIDE_CARDS_COUNT; ij++)
                {
                    cards[_row + 12 - ij, colum].SetNone();////cardsarray[_row + 12 - ij, colum, 0] = 0;
                    rowfordisplay = _row + 12 - ij;
                    columnfordisplay = colum;
                    storefuturecardmovement(rowfordisplay, columnfordisplay, 1);
                    timedelay();
                }
                isSuiteRemoved = true;
                //lastcard[colum] = _row - 1;
                timedelay();
                if(ij == SIDE_CARDS_COUNT)
                {
                    if (cards[_row - 1,colum].FaceDown)//(int)cardsarray[_row-1,colum,0] == facedown)
                    {
                        //DisplayCounter -= 1;
                        cardMovements.RemoveLast();
                        cards[_row - 1, colum].FaceDown = false;//cardsarray[_row - 1, colum] = caxsx[_row - 1, colum];
                        rowfordisplay = _row - 1;
                        columnfordisplay = colum;
                        storefuturecardmovement(rowfordisplay, columnfordisplay, 1);
                        dot = 1;
                    }
                    else if (cards[_row - 1,colum].IsNotNone)//(int)cardsarray[_row-1,colum,0] >0)
                    {
                        //DisplayCounter -= 1;
                        cardMovements.RemoveLast();
                        rowfordisplay = _row - 1;
                        columnfordisplay = colum;
                        storefuturecardmovement(rowfordisplay, columnfordisplay, 1);
                    }
                }

                history(_row, colum, 0, OldColumn, dot);
                timedelay();
                DisplayAll(moveData);
            }

            return isSuiteRemoved;
        }
    
        void showicon(CardSuite suite)
        {
            int i = removedsuit.Count;
            int j = 32;
            
            if (suite == CardSuite.Clubs || suite == CardSuite.Heart) j = 36 - (int)suite;

            if (suite == CardSuite.None)
            {
                //wdow.CreateRectangle(Width - 30, 70 + i * 30, Width - 1, 100 + i * 30, "Green");
            }
            else
            {
                //wdow.CreateText(Width - j, 30 + i * 30, Card.GetSuiteColor(suite), Card.GetSuiteChar(suite).ToString(), "Times", 30);
            } 
        }
    
        void history(int _fromrow,int _fromcolumn,int _oldrow,int _oldcolumn,int _dot)
        {
            if(_oldcolumn > -1)
            {
                //historycount += 1;
                //if (historycount == HISTORY_SIZE + 1) historycount = 1;
                //histrec[historycount, 0] = _oldrow;
                //histrec[historycount, 1] = _oldcolumn;
                //histrec[historycount, 2] = _fromrow;
                //histrec[historycount, 3] = _fromcolumn;
                //histrec[historycount, 4] = _dot;
                this.cardsHistory.Push(new CardHistory()
                {
                    FromRow = _fromrow,
                    FromColumn = _fromcolumn,
                    OldRow = _oldrow,
                    OldColumn = _oldcolumn,
                    Dot = dot
                });
            }
        }
    
        int repeatcol(int column)
        {
            int blnk = -1;
            int masthead = -1;
            int m, j;
            int repsuity = -1;

            //confused?
            //if (oldr != fromrow || oldc != CurrentColumn)
            if (oldc != column || oldr != fromrow)
            {
                //repsuity = -1;
                for(int jl = 0; jl < PLAY_COLUMNS; jl++)
                {
                    colmoves[jl] = 0;
                }

                int reprank = (int)cards[fromrow, column].Rank + 1;//(int)cardsarray[fromrow, CurrentColumn, 0] + 1;
                int repsuit = (int)cards[fromrow, column].Suite;//(int)cardsarray[fromrow, CurrentColumn, 1];
                
                oldmasthead = column;
                
                int k = column + 1;
                
                if (k == PLAY_COLUMNS)
                {
                    k = 0;
                }
                
                while(k != column)
                {
                    j = GetLastCardPos(k); ////(int)lastcard[k];
                    if (j == ROW_BASE - 1) //empty column
                    {
                        blnk = k;
                    }
                    else
                    {
                        m = (int)cards[j, k].Rank;//(int)cardsarray[j, k, 0];
                        if (m == reprank)
                        {
                            masthead = k;
                            colmoves[k] = 1;
                            if(repsuity == -1 && (int)cards[j, k].Suite == repsuit)//(int)cardsarray[j, k, 1] == repsuit)
                            {
                                repsuity = k;
                            }
                        }
                    }
                    
                    k ++;
                    
                    if (k == PLAY_COLUMNS)
                    {
                        k = 0;
                    }
                }
                
                //if repeated suite exist
                if (repsuity > -1)
                {
                    masthead = repsuity;
                }
                
                if(masthead > -1)
                {
                    if (fromrow == ROW_BASE)
                    {
                        colmoves[column] = 0;
                    }
                    
                    colmoves[masthead] = 2;
                    
                    return masthead;
                }

                return blnk;
            }
            else //same card was clicked again
            {
                //repsuity = -1;
                blnk = -1;
                int k = GetLastCardPos(oldmasthead); //(int)lastcard[oldmasthead];
                //if((int)cardsarray[fromrow, CurrentColumn, 0] == (int)cardsarray[k, oldmasthead, 0] - 1)
                if (Card.IsNext(cards[k, oldmasthead].Rank, cards[fromrow, column].Rank))
                {
                    colmoves[oldmasthead] = 1;
                }

                k = column - 1;

                if (k == -1)
                {
                    k = PLAY_COLUMNS - 1;
                }

                j = 0;
                while(j != PLAY_COLUMNS - 1)
                {
                    //if ((int)lastcard[k] == ROW_BASE - 1)
                    if(IsColumnBlank(k))//if (GetLastCardPos(k) == ROW_BASE - 1)
                    {
                        blnk = k;
                    }

                    if((int)colmoves[k] == 1)
                    {
                        if(masthead == -1)
                        {
                            colmoves[k] = 2;
                            masthead = k;
                        }
                    }
                    
                    k --;

                    if (k == -1)
                    {
                        k = PLAY_COLUMNS - 1;
                    }

                    j++;
                }

                if (masthead > -1)
                {
                    //if ((int)lastcard[oldmasthead] > 4)
                    if(IsColumnNotBlank(oldmasthead))//if (GetLastCardPos(oldmasthead) > ROW_BASE - 1)
                    {
                        oldmasthead = masthead;
                        colmoves[k] = 2;
                    }
                    return masthead;
                }
                else
                {
                    if (fromrow == ROW_BASE)
                    {
                        blnk = -1;
                    }
                    k = -1;
                    if (blnk > -1)
                    {
                        for (int jl = 0; jl < PLAY_COLUMNS; jl++)
                        {
                            if ((int)colmoves[jl] == 2)
                            {
                                colmoves[jl] = 1;
                                k = jl;
                            }
                        }
                        return blnk;
                    }
                    for (int jl = 0; jl < PLAY_COLUMNS; jl++)
                    {
                        if ((int)colmoves[jl] == 2 && jl != column)
                        {
                            colmoves[jl] = 1;
                            oldmasthead = jl;
                        }
                    }
                    if(oldmasthead > -1)
                    {
                        return oldmasthead;
                    }else
                    {
                        return blnk;
                    }
                }
            }
        }

        /*int checkcol(int column)
        {
            rw = (int)lastcard[(int)dims[column]];
            if ((int)cardsarray[rw,column,0] == (int)cardsarray[fromrow,fromcolumn,0]+1 && (int)usedcolumns[column] == 1)
            {
                columnold = column;rowold = rw;
            }
            if ((int)cardsarray[rw, column, 0] == (int)cardsarray[fromrow, fromcolumn, 0] + 1 && (int)usedcolumns[column] == 0)
            {
                rankjoin = column;
                if ((int)cardsarray[rw, column, 1] ==(int)cardsarray[fromrow, fromcolumn, 1]) samesuitjoin = column;
            }

            return rankjoin;
        }*/

        //find ledges and count available blank columns
        int FindLedges(int findColumn, List<int> blankColumns)
        {
            for(int jl = 0;jl < 22;jl++)
            {
                ledgecolumn[jl] = -1;
            }
            int ll,i;

            blankColumns.Clear();

            for (int jl = 0;jl < PLAY_COLUMNS;jl++)
            {
                ll = GetLastCardPos(jl) + 1; //(int)lastcard[jl] + 1;

                i = (int)cards[ll - 1, jl].Rank;//(int)cardsarray[ll - 1, jl, 0];
                ledgecolumn[i] = jl;

                ledgerow[jl] = ll;
                if (ll == ROW_BASE && jl != findColumn)
                {
                    blankColumns.Add(jl);
                }
            }

            return blankColumns.Count;
        }
    
        int cutstep()
        {
            int cc = fromrow;
            int ro80 = GetDim(fromcolumn);//(int)dims[fromcolumn];
            int n;
            cc = GetLastCardPos(ro80); //(int)lastcard[ro80];
            int ct = cc;
            while (ct >fromrow)
            {
                //while (ct>fromrow && (int)cardsarray[ct,fromcolumn,1] == (int)cardsarray[ct-1,fromcolumn,1])
                while (ct > fromrow && cards[ct, fromcolumn].Suite == cards[ct - 1, fromcolumn].Suite)
                {
                    ct --;
                }
                n = ct;
                //while ((int)ledgecolumn[(int)cardsarray[n,fromcolumn,0]+1] == -1 && n<cc)
                while ((int)ledgecolumn[(int)cards[n, fromcolumn].Rank + 1] == -1 && n < cc)
                {
                    n ++;
                }

                while(n<cc)
                {
                    n ++;
                    ledgecolumn[(int)cards[n, fromcolumn].Rank + 1] = -1;//ledgecolumn[(int)cardsarray[n, fromcolumn, 0] + 1] = -1;
                }

                ct --;
                cc = ct;
            }

            return cc;
        }

        // find available shelves for card movements
        int steps(int _ia,int fnd, ref int xa, ref int ya)
        {
            int find = fnd;

            fromrow = (int)lx[xa];
            fromcolumn = (int)ly[xa];
            int addr_end_fragment = fromrow;
            int ro80 = GetDim(fromcolumn);//(int)dims[fromcolumn];
            
            while (cards[addr_end_fragment, fromcolumn].IsNotNone)//(int)cardsarray[addr_end_fragment,fromcolumn,0] != 0)
            {
                addr_end_fragment ++;
            }

            addr_end_fragment --;
            //ct = addr_end_fragment;
            int n = 0;
            xa = 0;
            ya = 0;
            int fragment_length = 1;

            while (fromrow < addr_end_fragment)
            {
                //while ((int)ledgecolumn[(int)cardsarray[addr_end_fragment,fromcolumn,0] +1] != -1 && fromrow <addr_end_fragment)
                while ((int)ledgecolumn[(int)cards[addr_end_fragment, fromcolumn].Rank + 1] != -1 && fromrow < addr_end_fragment)
                {
                    ya ++;
                    lx[ya] = addr_end_fragment;
                    ly[ya] = fromcolumn;
                    by[ya] = 1;
                    lth[ya] = fragment_length;
                    fragment_length = 1;
                    addr_end_fragment --;
                    n = 0;
                }

                //if ((int)cardsarray[addr_end_fragment,fromcolumn,1] != (int)cardsarray[addr_end_fragment - 1, fromcolumn, 1] && fromrow<addr_end_fragment)
                if (cards[addr_end_fragment, fromcolumn].Suite != cards[addr_end_fragment - 1, fromcolumn].Suite && fromrow < addr_end_fragment)
                {
                    n ++;
                    ya ++;
                    lx[ya] = addr_end_fragment;
                    ly[ya] = fromcolumn;
                    by[ya] = 0;
                    lth[ya] = fragment_length;
                    fragment_length = 1;
                }
                else
                {
                    if(fromrow<addr_end_fragment)
                    {
                        fragment_length ++;
                    }
                }
                if (n > _ia) find = 0;
                addr_end_fragment --;
            }
            ya ++;
            lx[ya] = fromrow;
            ly[ya] = fromcolumn;
            by[ya] = 1;
            lth[ya] = fragment_length;

            return find;
        }

        // empty temporary shelf
        int mptyshlf(int xa, List<int> blankcolumn)
        {
            spacefind(blankcolumn);
            movecard(xa, OldRow, OldColumn);
            if ((int)blanks_program[xa] == 1)
            {
                OldRow += (int)lth[xa];
                xa --;
                movecard(xa, OldRow, OldColumn);
                xa ++;
            }

            if ((int)blanks_program[xa] == 2)
            {
                OldRow += (int)lth[xa];
                xa --;
                movecard(xa, OldRow, OldColumn);
                xa -= 2;
                spacefind(blankcolumn);
                movecard(xa, OldRow, OldColumn);
                xa ++;
                OldColumn = (int)ly[xa + 1];
                OldRow = (int)lx[xa + 1] + (int)lth[xa + 1] - 1;
                movecard(xa, OldRow, OldColumn);
                OldRow += (int)lth[xa];
                xa --;
                movecard(xa, OldRow, OldColumn);
                xa += 3;
            }

            if ((int)blanks_program[xa] == 3)
            {
                OldRow = (int)lx[xa-4] + (int)lth[xa-4]-1;
                OldColumn = (int)ly[xa -4];
                xa -= 5;
                movecard(xa, OldRow, OldColumn);
                xa += 5;
            }

            return xa;
        }
    
        void movecard(int xa, int OldRow, int OldColumn)
        {
            int columnformove = (int)ly[xa];
            int columnformove80 = GetDim(columnformove);//(int)dims[columnformove];
            int ru80 = GetDim(OldColumn);//(int)dims[OldColumn];
            OldRow = findEmptyRow(OldRow + 1, ru80) - 1;
            int ll = 1;
            int rowfordisplay, columnfordisplay;

            if (!cards[(int)lx[xa] - 1, columnformove].FaceDown)//(int)cardsarray[(int)lx[xa]-1,columnformove,0] != facedown)
            {
                dot = 0;
            }
            else
            {
                cards[(int)lx[xa] - 1, columnformove].FaceDown = false;//cardsarray[(int)lx[xa] - 1, columnformove] = caxsx[(int)lx[xa] - 1, columnformove];

                columnfordisplay = fromcolumn;
                rowfordisplay = fromrow - 1;
                storefuturecardmovement(rowfordisplay, columnfordisplay, 1);
                dot = 1;
                dott = 1;
            }
            rowfordisplay = (int)lx[xa];
            columnfordisplay = columnformove;
            while (ll < (int)lth[xa] + 1)
            {
                int lxxal = rowfordisplay + ll;

                cards[OldRow + ll, OldColumn].MoveFrom(cards[(lxxal - 1), columnformove]);//cardsarray[OldRow + ll, OldColumn] = cardsarray[(lxxal - 1), columnformove];
                //cards[lxxal - 1, columnformove].SetNone();//cardsarray[lxxal - 1, columnformove, 0] = 0;

                ll ++;
            }
            int movesize = ll - 1;
            storefuturecardmovement(rowfordisplay, columnfordisplay, 1);
            //if((int) cardsarray[rowfordisplay - 1, columnfordisplay, 0] > 0 && (int)cardsarray[rowfordisplay - 1, columnfordisplay, 0] != 15)
            if (cards[rowfordisplay - 1, columnfordisplay].IsNotNoneFaceDown)
            {
                storefuturecardmovement(rowfordisplay - 1, columnfordisplay, 1);
            }
            rowfordisplay = OldRow + 1;
            columnfordisplay = OldColumn;
            storefuturecardmovement(rowfordisplay, columnfordisplay, movesize);
            lx[xa] = OldRow + 1;
            ly[xa] = OldColumn;
        }
    
        void spacefind(List<int> blankcolumn)
        {
            foreach(int column in blankcolumn)
            {
                if (column < PLAY_COLUMNS && cards[ROW_BASE, column].IsNone)//(int)cardsarray[ROW_BASE, blankcolumn[jmx], 0] == 0)
                {
                    OldColumn = column;
                    OldRow = (int)ledgerow[OldColumn] - 1;
                    break;
                }
            }
        }

        // fill temporary shelf
        int fillshlf(int xa, List<int> blankcolumn)
        {
            OldRow = GetLastCardPos(OldColumn); //(int)lastcard[OldColumn];
            xa --;
            if((int)blanks_program[xa] == 1)
            {
                xa --;
                spacefind(blankcolumn);
                movecard(xa, OldRow, OldColumn) ;
                xa ++;
            }
            if ((int)blanks_program[xa] == 2)
            {
                xa -= 3;
                spacefind(blankcolumn);
                movecard(xa, OldRow, OldColumn);
                xa ++;
                spacefind(blankcolumn);
                movecard(xa, OldRow, OldColumn);
                OldRow += (int)lth[xa];
                xa --;
                movecard(xa, OldRow, OldColumn);
                xa+= 2;
                spacefind(blankcolumn);
                movecard(xa, OldRow, OldColumn);
                xa ++;
            }

            if ((int)blanks_program[xa] == 3)
            {
                xa -= 5;
                spacefind(blankcolumn);
                movecard(xa, OldRow, OldColumn);
                xa += 5;
            }

            OldColumn = (int)ly[xa + 1];
            OldRow = (int)lx[xa + 1] + (int)lth[xa + 1] - 1;
            movecard(xa, OldRow, OldColumn);
            OldRow += (int)lth[xa];

            return xa;
        }
    
        public void SetDifficult(int difficulty)
        {
            Difficulty = 9 - difficulty;
            //New();
        }
    
        void tencards(APIStackData data)
        {
            //data.DCK = new int[PLAY_COLUMNS];

            for (int it = 0; it < PLAY_COLUMNS;it++)
            {
                int tenct = GetLastCardPos(it); //(int)lastcard[it];
                //cardsarray[tenct + 1, it, 0] = (int)dck[nextcard + 1] % 13 + 1;
                //cardsarray[tenct + 1, it, 1] = (int)dck[nextcard + 1] % 4 + 1;
                cards[tenct + 1, it].CardDeck = (int)dck[NextCard + 1];
                //data.DCK[it] = (int)dck[NextCard + 1];
                //lastcard[it] = tenct + 1;
                storefuturecardmovement(tenct + 1, it, 1);
                timedelay();
                IncrementNextCard(); // nextcard++;
                //System.Threading.Thread.Sleep(20);
            }
        }
    
        internal void stackclick(APIStackData data)
        {
            deexpand();
            oldc = -1;
            if(DealNext10 < DealNext10Limit)
            {
                //newtencards = 1;
                joinz(true);

                tencards(data);

                //newtencards = 0;
                DealNext10++;
                data.DealNext10 = DealNext10;
                //wdow.CreateRectangle(Width - 30, 5, Width - 2, 40, "Green");
                //wdow.CreateText(Width - 26, 15, "White", (50 - 10 * DealNext10).ToString(), "Times", 15, true);
                history(0, 0, 0, 1, 0);

                APIMoveData moveData = new APIMoveData() { ID = data.ID };

                DisplayAll(moveData);

                data.movement.AddRange(moveData.movement);
            }
        }

        void joinz(bool NewTenCards)
        {
            deexpand();

            //for(int ijr = 0;ijr < PLAY_COLUMNS;ijr++)
            //{
            //    col = ijr;
            //    //describe(col);
            //}
            bool movecol;
            do
            {
                movecol = false;
                if (!NewTenCards)
                {
                    movecol = movecol || joinsuits();
                }

                movecol = movecol || fillspaces();
            } while (movecol);
        }

        //void describe(int icol)
        //{
        //    int rowe = GetLastCardPos(icol); //(int)lastcard[icol];
        //    suitcard[icol] = cards[rowe, col].Suite;//cardsarray[rowe, col, 1];
        //    int suitcardcol = (int)suitcard[icol];
            
        //    if(cards[ROW_BASE, col].IsNotNone)//(int)cardsarray[ROW_BASE, col, 0] > 0)
        //    {
        //        //while((int)cardsarray[rowe - 1, col, 1] == suitcardcol && (int)cardsarray[rowe, col, 0] == (int)cardsarray[rowe - 1, col, 0] - 1)
        //        while ((int)cards[rowe - 1, col].Suite == suitcardcol && Card.IsNext(cards[rowe - 1, col].Rank, cards[rowe, col].Rank))
        //        {
        //            rowe -= 1;
        //        }
        //    }
            
        //    firstcard[icol] = rowe;
        //}

        //void findsuittop(int icol)
        //{
        //    int rowe = (int)lastcard[icol];
        //    suitcard[icol] = cardsarray[rowe, col, 1];
        //    int suitcardcol = (int)suitcard[icol];
        //    if((int)cardsarray[ROW_BASE, col, 0] > 0)
        //    {
        //        while((int)cardsarray[rowe, col, 1] == suitcardcol && (int)cardsarray[rowe - 1, col, 1] == suitcardcol && (int)cardsarray[rowe, col, 0] == (int)cardsarray[rowe - 1, col, 0] - 1)
        //        {
        //            rowe -= 1;
        //        }
        //    }
        //    firstcard[icol] = rowe;
        //}
    
        //void findcolumnbottomcard(int icol)
        //{
        //    int rowe = ROW_BASE;

        //    if (rowe > ROW_BASE) rowe -= 1;
        //    lstcard(rowe, icol);
        //}
    
        void undoes()
        {
            deexpand();
            if(this.cardsHistory.Count > 0)
            {
                CardHistory history = this.cardsHistory.Peek();

                OldRow = history.OldRow;// (int)histrec[historycount, 0];
                OldColumn = history.OldColumn;// (int)histrec[historycount, 1];
                fromrow = history.FromRow; //(int)histrec[historycount, 2];
                fromcolumn = history.FromColumn;//(int)histrec[historycount, 3];
                dot = history.Dot;// (int)histrec[historycount, 4];
                
                if(history.OldColumn >= PLAY_COLUMNS && history.OldRow == -1)
                {

                }
                else
                {
                    this.cardsHistory.Pop();
                }
                
                int kkk = 0;

                if(dot == 1  && fromrow + kkk > ROW_BASE)
                {
                    cards[fromrow + kkk - 1, fromcolumn].FaceDown = true;//cardsarray[fromrow + kkk - 1, fromcolumn, 0] = facedown;
                    storefuturecardmovement(fromrow + kkk - 1, fromcolumn, 1);
                }

                if (OldRow == 0 && OldColumn == 1)
                {
                    DealNext10 --;
                    for (int jm =0;jm<PLAY_COLUMNS;jm++)
                    {
                        int rowfordisplay = GetLastCardPos(jm); //(int)lastcard[i];
                        int columnfordisplay = jm;
                        cards[rowfordisplay, jm].SetNone();//cardsarray[rowfordisplay, i, 0] = 0;
                        storefuturecardmovement(rowfordisplay, columnfordisplay, 1);
                        rowfordisplay--;
                        storefuturecardmovement(rowfordisplay, columnfordisplay, 1);
                        //wdow.CreateRectangle(Width - 30, 5, Width - 5, 35,"Green");
                        //wdow.CreateText(Width - 26, 15, "White", (50 - 10 * DealNext10).ToString(), "Times", 15, true);
                        DecrementNextCard(); //nextcard --;
                        //lastcard[i] = rowfordisplay;
                    }
                }
                else
                {
                    if(OldRow == 0)
                    {
                        for (int jm = 0; jm < 13; jm++)
                        {
                            cards[fromrow + jm, fromcolumn].Rank = (CardRank)(14 - jm - 1);////cardsarray[fromrow + i-1,fromcolumn, 0] = 14 - i;
                            cards[fromrow + jm, fromcolumn].Suite = removedsuit.Last();//cardsarray[fromrow + i - 1, fromcolumn, 1] = removedsuit[kk];

                            storefuturecardmovement(fromrow + jm, fromcolumn, 1);
                        }
                        //lastcard[fromcolumn] = fromrow + 12;
                        removedsuit.RemoveAt(removedsuit.Count - 1);
                    }
                    else
                    {
                        //while((int)cardsarray[OldRow + kkk + 1, OldColumn, 0] != 0 && (int)cardsarray[fromrow + kkk, fromcolumn, 0] == 0)
                        while (cards[OldRow + kkk + 1, OldColumn].IsNotNone && cards[fromrow + kkk, fromcolumn].IsNone)
                        {
                            cards[fromrow + kkk, fromcolumn].MoveFrom(cards[(OldRow + kkk + 1), OldColumn]);//cardsarray[fromrow + kkk, fromcolumn] = cardsarray[(OldRow + kkk + 1), OldColumn];
                            //cards[OldRow + kkk + 1, OldColumn].SetNone();//cardsarray[OldRow + kkk + 1, OldColumn, 0] = 0;

                            int rowfordisplay = fromrow + kkk;
                            int columnfordisplay = fromcolumn;
                            storefuturecardmovement(rowfordisplay, columnfordisplay, 1);
                            if(OldColumn > 9 && OldRow +kkk + 1 != 6)
                            {
                                rowfordisplay = OldRow + kkk + 1 - 1;
                            }
                            else
                            {
                                rowfordisplay = OldRow + kkk + 1;
                            }
                            columnfordisplay = OldColumn;
                            storefuturecardmovement(rowfordisplay, columnfordisplay, 1);
                            //if ((int)cardsarray[rowfordisplay - 1 - kkk, OldColumn, 0] > 0 && (int)cardsarray[rowfordisplay - 1 - kkk, OldColumn, 0] != 15)
                            if (cards[rowfordisplay - 1 - kkk, OldColumn].IsNotNoneFaceDown)
                            {
                                storefuturecardmovement(rowfordisplay - 1 - kkk, columnfordisplay, 1);
                            }

                            kkk ++;
                        }
                        //lastcard[fromcolumn] = fromrow + kkk - 1;
                        storefuturecardmovement(OldRow, OldColumn, 1);
                        //lastcard[OldColumn] = OldRow;
                    }
                }
            }
        }

        public void undo(APIUndoData undoData)
        {
            oldc = -1;
            undoes();
            undoData.DealNext10 = DealNext10;
            timedelay();

            showicon(0);

            APIMoveData moveData = new APIMoveData() { ID = undoData.ID };

            DisplayAll(moveData);

            undoData.movement.AddRange(moveData.movement);
        }
    
        void movecards(int jx, int ijj, int rowe, int col)
        {
            //if(col < 10 || col > 9)
            {
                OldRow = rowe;
                fromrow = jx;
                fromcolumn = ijj;
                //lastcard[fromcolumn] = fromrow - 1;
                //if((int)lastcard[fromcolumn] != fromrow - 1 || fromcolumn > 9)
                //{
                //    fromrow = fromrow;
                //}
                //while((int)cardsarray[jx, fromcolumn, 0] != 0)
                while (cards[jx, fromcolumn].IsNotNone)
                {
                    rowe++;
                    // cardsarray[rowe, col] = (int)cardsarray[jx, fromcolumn];
                    //cardsarray[rowe, col, 0] = (int)cardsarray[jx, fromcolumn, 0];
                    //cardsarray[rowe, col, 1] = (int)cardsarray[jx, fromcolumn, 1];
                    cards[rowe, col].MoveFrom(cards[jx, fromcolumn]);
                    //cards[jx, fromcolumn].SetNone();//cardsarray[jx, fromcolumn, 0] = 0;
                    jx++;
                }

                //movecol = true;
                //lastcard[col] = rowe;
                storefuturecardmovement(fromrow, fromcolumn, 1);
                dot = 0;
                if(cards[fromrow - 1, fromcolumn].IsNotNone)//(int)cardsarray[fromrow - 1, fromcolumn, 0] > 0)
                {
                    if(cards[fromrow - 1, fromcolumn].FaceDown)//(int)cardsarray[fromrow - 1, fromcolumn, 0] == facedown)
                    {
                        dot = 1;
                        cards[fromrow - 1, fromcolumn].FaceDown = false;//cardsarray[fromrow - 1, fromcolumn] = caxsx[fromrow - 1, fromcolumn];
                    }
                    storefuturecardmovement(fromrow - 1, fromcolumn, 1);
                }
                
                history(fromrow, fromcolumn, OldRow, col, dot);

                int movesize = jx - fromrow;
                
                storefuturecardmovement(OldRow + 1, col, movesize);
                timedelay();
                //describe(col);
                //col = fromcolumn;
                //describe(col);
            }
        }
    
        bool joinsuits()
        {
            //for(int ijoins = 0; ijoins < PLAY_COLUMNS - 1; ijoins++)
            //{
            //    int iijoins = -1;
            //    ijoins = 0;

            //    while(joins(ref iijoins, ref ijoins, ref movecol))
            //    {
            //    }
            //}
            return joins();
        }
    
        bool joins()
        {
            bool movecol = false;

            for (int ijoins = 0; ijoins < PLAY_COLUMNS; ijoins++)
            {
                for (int iijoins = 0; iijoins < PLAY_COLUMNS; iijoins++)
                {
                    int lastCardPos_ijoins = GetLastCardPos(ijoins);
                    int lastCardPos_iijoins = GetLastCardPos(iijoins);
                    int firstCardPos_ijoins = GetFirstCardPos(ijoins);
                    int firstCardPos_iijoins = GetFirstCardPos(iijoins);

                    //if(iijoins != ijoins && (int)suitcard[ijoins] == (int)suitcard[iijoins] && (int)cardsarray[(int)firstcard[ijoins], ijoins, 0] > (int)cardsarray[(int)firstcard[iijoins], iijoins, 0] && (int)cardsarray[(int)lastcard[ijoins], ijoins, 0] <= (int)cardsarray[(int)firstcard[iijoins], iijoins, 0] + 1 && (int)cardsarray[(int)lastcard[ijoins], ijoins, 0] > (int)cardsarray[(int)lastcard[iijoins], iijoins, 0])
                    if (iijoins != ijoins &&
                        GetLastCardSuite(ijoins) == GetLastCardSuite(iijoins) &&//(int)suitcard[ijoins] == (int)suitcard[iijoins] && 
                        cards[firstCardPos_ijoins, ijoins].Rank > cards[firstCardPos_iijoins, iijoins].Rank &&
                        cards[lastCardPos_ijoins, ijoins].Rank <= cards[firstCardPos_iijoins, iijoins].Rank + 1 &&
                        cards[lastCardPos_ijoins, ijoins].Rank > cards[lastCardPos_iijoins, iijoins].Rank)
                    {
                        int jx = 0;
                        //while((int)cardsarray[(int)firstcard[iijoins] + jx, iijoins, 0] >= (int)cardsarray[(int)lastcard[ijoins], ijoins, 0])
                        while (cards[firstCardPos_iijoins + jx, iijoins].Rank >= cards[lastCardPos_ijoins, ijoins].Rank)
                        {
                            jx++;
                        }
                        int rowe = lastCardPos_ijoins;// (int)lastcard[ijoins];
                        jx = firstCardPos_iijoins + jx;
                        int col = ijoins;
                        //ijj = iijoins;
                        movecards(jx, iijoins, rowe, col);
                        movecol = true;
                        //System.Threading.Thread.Sleep(20);
                        continue;
                    }
                }
            }

            return movecol;
        }

        bool fillspaces()
        {
            int col = PLAY_COLUMNS;
            int rowe = ROW_BASE;
            bool movecol = false;

            for(int ijk=0; ijk < PLAY_COLUMNS; ijk++)
            {
                if (cards[ROW_BASE, ijk].IsNone)
                {
                    col = ijk;
                }
            }
                
            if(col < PLAY_COLUMNS)
            {
                int jx;
                int ijj = 0;

                if (DealNext10 < DealNext10Limit)
                {
                    jx = MAX_ROW_COUNT;//100
                    for(int ij=0; ij < PLAY_COLUMNS; ij++)
                    {
                        int firstCardIdx = GetFirstCardPos(ij);

                        //if((int)firstcard[ij] < jx && (int)firstcard[ij] > ROW_BASE)
                        if (firstCardIdx < jx && firstCardIdx > ROW_BASE)
                        {
                            jx = firstCardIdx; 
                            ijj = ij;
                        }
                    }
                }
                else
                {
                    jx = ROW_BASE;
                    for (int ij = 0; ij < PLAY_COLUMNS; ij++)
                    {
                        int firstCardIdx = GetFirstCardPos(ij);

                        //if ((int)firstcard[ij] > jx)
                        if (firstCardIdx > jx)
                        {
                            jx = firstCardIdx;
                            ijj = ij;
                        }
                    }
                }

                rowe = ROW_BASE - 1;

                if(jx > ROW_BASE && jx != MAX_ROW_COUNT/*100*/)
                {
                    movecards(jx, ijj, rowe,col);
                    movecol = true;
                    //col = ijj;
                }
            }

            return movecol;
        }
    
        public void solve(APIMoveData data)
        {
            oldc = -1;
            joinz(false);
            timedelay();

            DisplayAll(data);
        }  

        public static Card[,] InitCard(/*PlaceHolerCtrl _wdow, */int rows, int cols)
        {
            Card[,] card = new Card[rows, cols];

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    card[row, col] = new Card(/*_wdow*/);

            return card;
        }

        public static int[] InitInt(int size)
        {
            int[] data = new int[size];

            for(int i = 0;i < size;i++)
                data[i] = 0;

            return data;
        }

        #region Next Card Maniplation
        int NextCard { get; set; } = 0;

        void InitNextCard()
        {
            this.NextCard = 0;
        }

        void IncrementNextCard()
        {
            this.NextCard++;
        }

        void DecrementNextCard()
        {
            this.NextCard--;
        }
        #endregion
    }
}

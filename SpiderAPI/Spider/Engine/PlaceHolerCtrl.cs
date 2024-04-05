using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proAmazingSpider
{
    public partial class PlaceHolerCtrl : PictureBox
    {
        int HolderWidth;
        int HolderHeight;

        public PlaceHolerCtrl()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);


        }

        internal void CreateRectangle(int Left, int Top, int Right, int Bottom, string FillColor, int BorderWidth = 0, string BorderColor = "Black")
        {
            this.HolderWidth = Right - Left;
            this.HolderHeight = Bottom - Top;

            Rectangle rt = new Rectangle(Left, Top, this.HolderWidth, this.HolderHeight);

            using (Graphics g = this.CreateGraphics())
            {
                using (SolidBrush brush = new SolidBrush(Color.FromName(FillColor)))
                    g.FillRectangle(brush, rt);

                if (BorderWidth > 0)
                {
                    using (Pen pen = new Pen(Color.FromName(BorderColor), BorderWidth))
                        g.DrawRectangle(pen, rt);
                }
            }
        }

        internal void CreateText(int X, int Y, string FontColor, string Content, string FontName, int FontSize, bool IsBold = false)
        {
            using(Graphics g = this.CreateGraphics())
            using (SolidBrush brush = new SolidBrush(Color.FromName(FontColor)))
            using (Font _font = IsBold ? new Font(FontName, FontSize, FontStyle.Bold) : new Font(FontName, FontSize))
                g.DrawString(Content, _font, brush, new Point(X, Y));
        }

        internal void CreateLine(int X1, int Y1, int X2, int Y2, int Width, string FillColor)
        {
            using (Graphics g = this.CreateGraphics())
            using (SolidBrush brush = new SolidBrush(Color.FromName(FillColor)))
            using (Pen p = new Pen(brush, Width))
                g.DrawLine(p, X1, Y1, X2, Y2);
        }

    }
}

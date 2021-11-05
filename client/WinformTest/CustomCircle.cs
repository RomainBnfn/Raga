using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformTest
{
    public partial class CustomCircle : Control
    {
        public CustomCircle()
        {
            InitializeComponent();
            this.BackColor = Color.Blue;

        }

        public CustomCircle(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            /*using (Brush brush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillEllipse(brush, this.ClientRectangle);
            }*/
            e.Graphics.Clear(this.BackColor);

            int size = (this.Width-15)/5 + 4;
            this.Font = new Font("Jokerman", size) ;
            using (Brush stylo = new SolidBrush(Color.Black))
            {
                SizeF taille = e.Graphics.MeasureString(this.Text, this.Font);
                e.Graphics.DrawString(this.Text, this.Font, stylo, new RectangleF(new PointF((Width - taille.Width) / 2, (Height - taille.Height) / 2), taille));
            }
        }

        [Localizable(true), Bindable(true)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (this.Text != value)
                {
                    base.Text = value;
                    this.Invalidate();
                }
            }
        }

        [Localizable(true), Bindable(true)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                if (this.BackColor != value)
                {
                    base.BackColor = value;
                    this.Invalidate();
                }
            }
        }
    }
}

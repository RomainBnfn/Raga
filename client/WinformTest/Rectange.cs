using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformTest
{
    public partial class Rectange : Control
    {
        public Rectange()
        {
            InitializeComponent();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            System.Drawing.Drawing2D.GraphicsPath maForme =
                            new System.Drawing.Drawing2D.GraphicsPath();

            System.Drawing.Rectangle newRectangle = this.ClientRectangle;


            using (Brush brush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
            maForme.AddRectangle(newRectangle);

            Region = new System.Drawing.Region(maForme);
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

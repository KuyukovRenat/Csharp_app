using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tmp1
{
    public partial class Form1 : Form
    {
        List<Ellipse> ellipses;
        bool runnable;

        public Form1()
        {
            InitializeComponent();
            runnable = true;
            DoubleBuffered = true;
            ellipses = new List<Ellipse>();
            Ellipse b = new Ellipse(new Rectangle(this.Width - 80, this.Height - 300, 80, 100), Pens.Black, new Point(0, 0), false);
            Paint += b.draw;
            ellipses.Add(b); 
            //поток добавления новых и удаления старых эллипсов
            new Thread(
                () =>
                {
                while (runnable) //пока приложение работает
                {
                    lock (ellipses) //синхронизация
                    {
                        Ellipse y = new Ellipse(new Rectangle(this.Width - 80, this.Height - 300, 80, 100), Pens.Orange, new Point(-2, 0), true);
                        b.Ev += y.evYellow;
                        y.Ev += b.evBlack;
                        y.Created();
                        Paint += y.draw;
                        ellipses.Add(y);//добаление нового желтого эллипса в правой стороне
                        }
                        Thread.Sleep(3000);//ждем 3 сек
                    }
                }).Start();
            //поток перемещения эллипсов
            new Thread(() =>
            {
                while (runnable) //пока приложение работает
                {
                    lock (ellipses) //синхронизация
                    {
                        ellipses.ForEach((Ellipse el) =>
                        {
                            if (el.Rect.Y > 0)
                            {
                                el.move();
                                Invalidate();
                                Thread.Sleep(10);
                            }
                        });
                    }
                }
            }).Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            runnable = false;
        }

        class Ellipse
        {
            public delegate void DlHandler(object sender, EventArgs e);
            public event DlHandler Ev;
            public Rectangle Rect;
            public Pen Color { get; }
            public Point Vector { get; set; }
            public bool Visible { get; set; }
            public int count;

            public Ellipse(Rectangle r, Pen c, Point v, bool vis)
            {
                Rect = r;
                Color = c;
                Vector = v;
                Visible = vis;
            }
           
           public void Created()
            {
                if (Ev != null)
                    Ev(this, null);
            }

            public void move()
            {
                Rect.Offset(Vector);
            }

            public void evBlack(object sender, EventArgs e)
            {
                count++;
                if (count >= 10)
                {
                    Visible = true;
                    Vector = new Point(-2, 0);
                    if (Ev != null)
                        Ev(this, null);
                }
            }

            public void evYellow(object sender, EventArgs e)
            {
                Visible = false; //убираем видимость желтых эллипсов
            }

            public void draw(object sender, PaintEventArgs e)
            {
                if (Visible)
                {
                    e.Graphics.DrawEllipse(Color, Rect);
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Color Colour = Color.Blue;
            Rectangle r1 = new Rectangle(0, 170, 902, 15);
            e.Graphics.DrawRectangle(new Pen(Colour), r1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {}
        private void label1_Click(object sender, EventArgs e)
        {}
    }
}
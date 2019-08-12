using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graphical_connect4
{
    public partial class Form1 : Form
    {
        Color lightRed = Color.FromArgb(100, 255, 0, 0);
        Color red = Color.FromArgb(255, 255, 0, 0);

        Color lightBlue = Color.FromArgb(100, 0, 0, 255);
        Color blue = Color.FromArgb(255, 0, 0, 255);

        SolidBrush lightRedBrush;
        Pen redPen;
        SolidBrush lightBlueBrush;
        Pen bluePen;
        List<Disc> discList = new List<Disc>();

        Timer drawTimer = new Timer();

        public Form1()
        {
            InitializeComponent();
            //drawScreen(this.CreateGraphics());
            //this.Paint += new PaintEventHandler(drawScreen);
            lightRedBrush = new SolidBrush(lightRed);
            lightBlueBrush = new SolidBrush(lightBlue);
            redPen = new Pen(red, 3);
            bluePen = new Pen(blue, 3);

            Disc newDisc = new Disc(new Point(50, 50), Color.Red);
            discList.Add(newDisc);

            drawTimer.Interval = 16;
            drawTimer.Tick += new EventHandler(drawTimerElapsed);
            drawTimer.Enabled = true;

        }

        public void drawTimerElapsed(object sender, System.EventArgs e)
        {
            drawScreen();
        }


        public void drawScreen()
        {
            Bitmap bufl = new Bitmap(this.Width, this.Height);
            using (Graphics g = Graphics.FromImage(bufl))
            {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, Width, Height));
                updateBoard();
                drawBoard(g);
                this.CreateGraphics().DrawImageUnscaled(bufl, 0, 0);
            }
        }


        public void drawBoard(Graphics g)
        {
            foreach (Disc d in discList)
            {
                d.Draw(g);
            }
        }

        public void updateBoard()
        {
            foreach (Disc d in discList)
            {
                d.UpdatePosition();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            discList[0].StartMove(new Point(100, 100));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            discList[0].StartMove(new Point(50, 50));
        }
    }

    public class Disc
    {
        private Point _pos;
        private float _width;
        private float _height;
        private double _vel;
        private double _accel;
        private double _decel;
        private double _maxVel;
        private Color _color;
        private Color _lightColor;
        private SolidBrush _brush;
        private Pen _pen;
        private Boolean _inMove;
        private Point _movePoint;
        private Vect _moveDir;

        public Point Pos { get => _pos; set => _pos = value; }
        public float Width { get => _width; set => _width = value; }
        public float Height { get => _height; set => _height = value; }
        public double Vel { get => _vel; set => _vel = value; }
        public double Accel { get => _accel; set => _accel = value; }
        public double Decel { get => _decel; set => _decel = value; }
        public double MaxVel { get => _maxVel; set => _maxVel = value; }
        public Color Color { get => _color; set => _color = value; }
        public Color LightColor { get => _lightColor; set => _lightColor = value; }
        public SolidBrush Brush { get => _brush; set => _brush = value; }
        public Pen Pen { get => _pen; set => _pen = value; }
        public bool InMove { get => _inMove; set => _inMove = value; }
        public Point MovePoint { get => _movePoint; set => _movePoint = value; }
        public Vect MoveDir { get => _moveDir; set => _moveDir = value; }

        public struct Vect
        {
            private double _x;
            private double _y;
            private double _length;

            public double X { get => _x; set => _x = value; }
            public double Y { get => _y; set => _y = value; }
            public double Length { get => _length; set => _length = value; }

            public Vect(double X, double Y)
            {
                _x = X;
                _y = Y;
                _length = Math.Sqrt(Math.Pow(_x, 2) + Math.Pow(_y, 2));
            }
        }

        public Disc(Point p, Color c, float r = 50, double v = 10, double a = 1, double d = 1)
        {
            Width = r;
            Height = r;
            MaxVel = v;
            Color = c;
            Pos = p;
            Accel = a;
            Decel = d;
            Vel = 0;
            InMove = false;

            Pen = new Pen(Color);
            Brush = new SolidBrush(Color);
        }

        public void UpdatePosition()
        {
            if(InMove)
            {
                if (Vel < MaxVel)
                {
                    Vel += Accel;
                    if (Vel > MaxVel)
                    {
                        Vel = MaxVel;
                    }
                }
                _pos.X = (int)Math.Ceiling(Pos.X + (MoveDir.X * Vel));
                _pos.Y = (int)Math.Ceiling(Pos.Y + (MoveDir.Y * Vel));
                if (
                    (MoveDir.X >= 0) && (_pos.X > _movePoint.X) ||
                    (MoveDir.X < 0) && (_pos.X < _movePoint.X) ||
                    (MoveDir.Y >= 0) && (_pos.Y > _movePoint.Y) ||
                    (MoveDir.Y < 0) && (_pos.Y < _movePoint.Y)
                    )
                {
                    _pos.X = MovePoint.X;
                    _pos.Y = _movePoint.Y;
                    _inMove = false;
                }
            }
            else
            {
                Vel = 0;
            }
        }

        
        public void Draw(Graphics g)
        {
            try
            {
                g.DrawEllipse(Pen, Pos.X, Pos.Y, Width, Height);
                g.FillEllipse(Brush, Pos.X, Pos.Y, Width, Height);
            }
            catch (Exception ex) {

            }
            
        }

        public void StartMove(Point to)
        {
            InMove = true;
            MovePoint = to;
            Vect fullVect = new Vect(to.X - Pos.X, to.Y - Pos.Y);
            double moveLength = Math.Sqrt(Math.Pow(fullVect.X, 2) + Math.Pow(fullVect.Y, 2));
            MoveDir = new Vect(fullVect.X / moveLength, fullVect.X / moveLength);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;



namespace graphical_connect4
{
	public partial class Form1 : Form
	{
		
		List<Disc> discList = new List<Disc>();
		readonly Timer drawTimer = new Timer();
		readonly int FrameRate = 30;
		String boardFilename = "boardUnit.png";
		Bitmap boardUnitBMP;
		Bitmap boardBMP;
		int numCol = 7;
		int numRow = 6;
		int formerWidth;
		int formerHeight;

		public Form1()
		{
			InitializeComponent();

			Disc newDisc = new Disc(new Point(5, 5), Color.Red, 90);
			discList.Add(newDisc);

			boardUnitBMP = new Bitmap(Directory.GetCurrentDirectory() + "\\..\\..\\Resources\\" + boardFilename);
			boardBMP = CreateBoardBMP(boardUnitBMP);
			drawTimer.Interval = 1000/FrameRate;
			drawTimer.Tick += new EventHandler(drawTimerElapsed);
			drawTimer.Enabled = true;

			//this.ResizeBegin += new EventHandler(Form1_ResizeBegin);
			//this.ResizeEnd += new EventHandler(Form1_ResizeEnd);
		}

		public void Form1_ResizeBegin(object sender, System.EventArgs e)
		{
			formerWidth = this.Width;
			formerHeight = this.Height;
		}

		public void Form1_ResizeEnd(object sender, System.EventArgs e)
		{
			foreach (Disc d in discList)
			{
				double screenPercentX = (double)d.Pos.X / formerWidth;
				double screenPercentY = (double)d.Pos.Y / formerHeight;
				d.Pos = new Point((int)(Width * screenPercentX), (int)(Height * screenPercentY));

				screenPercentX = (double)d.CurrentMoveDest.X / formerWidth;
				screenPercentY = (double)d.CurrentMoveDest.Y / formerHeight;
				d.CurrentMoveDest = new Point((int)(Width * screenPercentX), (int)(Height * screenPercentY));

				for (int i = 0; i < d.MoveList.Count; i++)
				{
					screenPercentX = (double)d.MoveList[i].X / formerWidth;
					screenPercentY = (double)d.MoveList[i].Y / formerHeight;
					d.MoveList[i] = new Point((int)(Width * screenPercentX), (int)(Height * screenPercentY));
				}
			}
		}

		public void drawTimerElapsed(object sender, System.EventArgs e)
		{
			drawScreen();
		}



		public void drawScreen()
		{
			SplitterPanel boardPanel = splitContainer1.Panel2;
			Bitmap bufl = new Bitmap(boardPanel.Width * 2, boardPanel.Height * 2);
			bufl.SetResolution(boardUnitBMP.HorizontalResolution, boardUnitBMP.VerticalResolution);
			using (Graphics g = Graphics.FromImage(bufl))
			{
				//g.ScaleTransform(0.5f, 0.5f);
				
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				g.FillRectangle(Brushes.White, new Rectangle(0, 0, Width, Height));
				UpdateBoard();
				DrawPieces(g);
				DrawBoard(g);
				//Form1.splitContainer1.Panel2.CreateGraphics().DrawImageUnscaled(bufl, 0, 0);

				boardPanel.CreateGraphics().DrawImageUnscaled(bufl, 0, 0);
				bufl.Dispose();
			}
		}


		public void DrawPieces(Graphics g)
		{
			foreach (Disc d in discList)
			{
				d.Draw(g);
			}
		}

		public void DrawBoard(Graphics g)
		{
			g.DrawImage(boardBMP, new Point(0, boardUnitBMP.Height));
		}

		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, long dwRop);

		private Bitmap CreateBoardBMP(Bitmap unit)
		{
			Bitmap bmp = new Bitmap(unit.Width * numCol, unit.Height * numRow);
			bmp.SetResolution(unit.HorizontalResolution, unit.VerticalResolution);
			using (Graphics bmpGrf = Graphics.FromImage(bmp))
			{
				bmpGrf.ScaleTransform(0.5f, 0.5f);
				bmpGrf.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				for (int r = 0; r < numRow; r++)
				{
					for (int c = 0; c < numCol - 0; c++)
					{
						bmpGrf.DrawImage(unit, unit.Width * c, unit.Height * r);
					}
				}
			}

			return bmp;
		}


		public void UpdateBoard()
		{
			foreach (Disc d in discList)
			{
				d.UpdatePosition();
			}
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			List<Point> moves = new List<Point>();
			moves.Add(new Point(5, 105));
			moves.Add(new Point(105, 105));
			moves.Add(new Point(105, 5));
			moves.Add(new Point(5, 5));
			discList[0].StartMove(moves);
		}

		private void Button2_Click(object sender, EventArgs e)
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
		private Boolean _moveComplete;
		private List<Point> _moveList;
		private Point _currentMoveDest;
		private Vect _currentMoveVect;

		public Point Pos { get => _pos; set => _pos = value; }
		public float Width { get => _width; set => _width = value; }
		public float Height { get => _height; set => _height = value; }
		public double Vel { get => _vel; set => _vel = value; }
		public double Accel { get => _accel; set => _accel = value; }
		public double Decel { get => _decel; set => _decel = value; }
		public double MaxVel { get => _maxVel; set => _maxVel = value; }
		public Color DarkColor { get => _color; set => _color = value; }
		public Color LightColor { get => _lightColor; set => _lightColor = value; }
		public SolidBrush Brush { get => _brush; set => _brush = value; }
		public Pen Pen { get => _pen; set => _pen = value; }
		public bool MoveComplete { get => _moveComplete; set => _moveComplete = value; }
		public Vect CurrentMoveVect { get => _currentMoveVect; set => _currentMoveVect = value; }
		public List<Point> MoveList { get => _moveList; set => _moveList = value; }
		public Point CurrentMoveDest { get => _currentMoveDest; set => _currentMoveDest = value; }

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

		public Disc(Point p, Color c, float r = 50, double v = 5, double a = 1, double d = 1)
		{
			Width = r;
			Height = r;
			MaxVel = v;
			DarkColor = c;
			Pos = p;
			Accel = a;
			Decel = d;
			Vel = 0;
			MoveComplete = true;
			LightColor = Color.FromArgb(DarkColor.A / 2, DarkColor.R, DarkColor.G, DarkColor.B);
			MoveList = new List<Point>();

			Pen = new Pen(DarkColor, 3);
			Brush = new SolidBrush(LightColor);
		}

		public void UpdatePosition()
		{
			if (!MoveComplete)
			{
				Double distanceRemaining = Math.Sqrt(Math.Pow(CurrentMoveDest.X - Pos.X, 2) + Math.Pow(CurrentMoveDest.Y - Pos.Y, 2));
				int stepsToStop = (int)Math.Ceiling(Vel / Accel);
				double distanceToStop = Vel * stepsToStop + 0.5 * Accel * Math.Pow(stepsToStop, 2);

				if (distanceToStop > distanceRemaining)
				{
					Vel -= Accel;
					if (Vel < 0)
					{
						Vel = 0;
					}
				}
				else {
					if (Vel < MaxVel)
					{
						Vel += Accel;
						if (Vel > MaxVel)
						{
							Vel = MaxVel;
						}
					}
				}
				
				_pos.X = (int)Math.Ceiling(Pos.X + (CurrentMoveVect.X * Vel));
				_pos.Y = (int)Math.Ceiling(Pos.Y + (CurrentMoveVect.Y * Vel));
				if (
					(CurrentMoveVect.X > 0) && (_pos.X > CurrentMoveDest.X) ||
					(CurrentMoveVect.X < 0) && (_pos.X < CurrentMoveDest.X) ||
					(CurrentMoveVect.Y > 0) && (_pos.Y > CurrentMoveDest.Y) ||
					(CurrentMoveVect.Y < 0) && (_pos.Y < CurrentMoveDest.Y)
					)
				{
					_pos.X = CurrentMoveDest.X;
					_pos.Y = CurrentMoveDest.Y;
					_moveComplete = true;

					if (MoveList.Count > 0)
					{
						CurrentMoveDest = MoveList[0];
						MoveList.Remove(CurrentMoveDest);
						CurrentMoveVect = GetCurrentMoveVect();
						MoveComplete = false;
					}
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
			catch (Exception ex)
			{

			}
		}

		public void StartMove(Point dest)
		{
			try
			{
				MoveComplete = false;
				MoveList.Add(dest);
				CurrentMoveDest = MoveList[0];
				MoveList.Remove(CurrentMoveDest);
				CurrentMoveVect = GetCurrentMoveVect();
			}
			catch (Exception ex)
			{

			}
		}

		public void StartMove(List<Point> dests)
		{
			try
			{
				if (dests.Count > 0)
				{
					MoveComplete = false;
					foreach (Point dest in dests)
					{
						MoveList.Add(dest);
					}
					CurrentMoveDest = MoveList[0];
					MoveList.Remove(CurrentMoveDest);
					CurrentMoveVect = GetCurrentMoveVect();
				}
			}
			catch (Exception ex)
			{

			}
		}

		private Vect GetCurrentMoveVect()
		{
			if (CurrentMoveDest == Pos)
			{
				return new Vect(0, 0);
			}
			else
			{
				Vect fullVect = new Vect(CurrentMoveDest.X - Pos.X, CurrentMoveDest.Y - Pos.Y);
				return new Vect(fullVect.X / fullVect.Length, fullVect.Y / fullVect.Length);
			}
			
		}
	}
}

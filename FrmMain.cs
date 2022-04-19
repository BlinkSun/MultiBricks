using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiBricks
{
    enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public partial class FrmMain : Form
    {
        private float gravity = 0.98F;

        private int boardWidth = 400;
        private int boardHeight = 600;

        private GameState gameState = GameState.MainMenu;



        private Timer TimerMoveBalls;
        private List<Ball> ballList = new List<Ball>();
        private Player player = new Player();
        private MainMenu mainMenu;

        //public FrmMain()
        //{
        //    InitializeComponent();
        //    this.DoubleBuffered = true;
        //    this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        //    this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        //    this.SetStyle(ControlStyles.UserPaint, true);
        //    this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        //    this.BackColor = Color.Transparent;
        //    this.TransparencyKey = Color.Transparent;
        //    this.FormBorderStyle = FormBorderStyle.None;
        //    this.WindowState = FormWindowState.Maximized;
        //    this.KeyPreview = true;
        //    this.KeyDown += FrmMain_KeyDown;
        //    this.KeyUp += FrmMain_KeyUp;
        //    this.MouseDown += FrmMain_MouseDown;
        //    this.MouseUp += FrmMain_MouseUp;
        //    this.MouseMove += FrmMain_MouseMove;
        //    this.Paint += FrmMain_Paint;
        //    this.TimerMoveBalls = new Timer();
        //    this.TimerMoveBalls.Interval = 10;
        //    this.TimerMoveBalls.Tick += TimerMoveBalls_Tick;
        //    this.TimerMoveBalls.Start();
        //}


        private bool canMove = false;
        private int lastMouseX = 0;

        private Random rnd = new Random();
        public FrmMain()
        {
            InitializeComponent();
            this.components = new Container();
            TimerMoveBalls = new(this.components);
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.DoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);
            this.UpdateStyles();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            // Generate MainMenu
            mainMenu = new MainMenu(this.boardWidth, this.boardHeight);
            
            // Generate Walls Area
            //wallList.Add(new("Top", 0, 0, boardWidth, 0));
            //wallList.Add(new("Right", boardWidth - 1, 0, 0, boardHeight));
            //wallList.Add(new("Bottom", 0, boardHeight - 1, boardWidth, 0));
            //wallList.Add(new("Left", 0, 0, 0, boardHeight));
            
            // Generate Player
            player = new("Player", (this.ClientSize.Width / 2) - (50 / 2), this.ClientSize.Height - 75, 50, 10);
            
            // Generate Balls
            Ball b = new Ball((this.ClientSize.Width / 2) - (10 / 2), this.ClientSize.Height - 75 - 10, 5, -5, true);
            ballList.Add(b);

            TimerMoveBalls.Tick += new EventHandler(this.TimerMoveBalls_Tick);
            TimerMoveBalls.Interval = 1;
            TimerMoveBalls.Start();
        }

        private void TimerMoveBalls_Tick(object sender, EventArgs e)
        {
            foreach (Ball ball in ballList)
            {
                if (ball.isSticked) continue;
                // gravity
                //ball.vy += gravity;

                ball.x += ball.vx;
                if (ball.x <= 0)
                {
                    ball.vx = -ball.vx;
                    //Boing();
                }
                else if (ball.x + ball.width >= ClientSize.Width)
                {
                    ball.vx = -ball.vx;
                    //Boing();
                }

                ball.y += ball.vy;
                if (ball.y <= 0)
                {
                    ball.vy = -ball.vy;
                    //Boing();
                }
                else if (ball.y + ball.height > ClientSize.Height)
                {
                    ball.isDead = true;
                    //ball.vy = -ball.vy;
                }
                
                if (ball.y + ball.height >= player.y &&
                    (ball.x > player.x && ball.x + ball.width < player.x + player.width))
                {
                    //ball.isSticked = true;
                    ball.y = player.y - ball.height;
                    ball.vy = -ball.vy;
                }
            }
            ballList.RemoveAll(ball => ball.isDead);
            if (ballList.Count == 0)
            {
                SpawnNewBall();
            }
            Refresh();
        }

        private void SpawnNewBall()
        {
            Ball b = new Ball((this.ClientSize.Width / 2) - (10 / 2), this.ClientSize.Height - 75 - 10, 5, -5, true);
            ballList.Add(b);
        }

        private void FrmMain_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.CornflowerBlue);
            switch (gameState)
            {
                case GameState.MainMenu:
                    mainMenu.Draw(e.Graphics);
                    break;
                case GameState.Playing:
                    player.Draw(e.Graphics);
                    e.Graphics.DrawRectangle(Pens.Red, 0, 0, boardWidth - 1, boardHeight - 1);
                    ballList.ForEach(ball => ball.Draw(e.Graphics));
                    break;
                case GameState.Paused:
                    break;
                case GameState.GameOver:
                    break;
                default:
                    break;
            }
        }

        private void FrmMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (canMove)
            {
                player.x += (e.X - lastMouseX);
                foreach (Ball ball in ballList)
                    if (ball.isSticked) ball.x += (e.X - lastMouseX);
                //if (player.x <= 0)
                //{
                //    player.x = 0;
                //}
                //else if (player.x + player.width >= ClientSize.Width)
                //{
                //    player.x = ClientSize.Width - player.width;
                //}
                lastMouseX = e.X;
            }
            
        }

        private void FrmMain_MouseUp(object sender, MouseEventArgs e)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    gameState = GameState.Playing;
                    break;
                case GameState.Playing:
                    if (e.Button == MouseButtons.Left)
                    {
                        lastMouseX = e.X;
                        canMove = !canMove;
                    }
                    if (e.Button == MouseButtons.Right && canMove) ballList.ForEach(ball => ball.isSticked = false);
                    break;
                case GameState.Paused:
                    break;
                case GameState.GameOver:
                    break;
                default:
                    break;
            }
        }
    }

    public class Ball
    {
        public int width = 10;
        public int height = 10;
        public float x, y;
        public float vx, vy;

        public bool isSticked = false;
        public bool isDead = false;

        public Ball(float x = 0, float y = 0, float vx = 0, float vy = 0, bool isSticked = false)
        {
            this.x = x;
            this.y = y;
            this.vx = vx;
            this.vy = vy;
            this.isSticked = isSticked;
        }

        public bool IsColliding(Rectangle other) => GetRectangle().IntersectsWith(other);
        
        public RectangleF GetRectangle() => new(x, y, width, height);
        
        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Yellow, GetRectangle());
            g.DrawEllipse(Pens.Black, GetRectangle());
        }
    }

    public class Wall
    {
        public string name;
        public int x, y;
        public int width, height;

        public Wall(string name = "", int x = 0, int y = 0, int width = 0, int height = 0)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Rectangle GetRectangle() => new(x, y, width, height);

        public void Draw(Graphics g)
        {
            g.DrawRectangle(Pens.Red, GetRectangle());
        }
    }
    
    public class Player
    {
        public string name;
        public int x, y;
        public int width, height;

        public Player(string name = "", int x = 0, int y = 0, int width = 0, int height = 0)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Rectangle GetRectangle() => new(x, y, width, height);
        
        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Red, x, y, width, height);
            g.DrawRectangle(Pens.Black, x, y, width, height);
        }
    }

    public class MainMenu
    {
        private int boardWidth;
        private int boardHeight;
        private Font mainFont;

        public MainMenu(int boardWidth, int boardHeight)
        {
            this.boardWidth = boardWidth;
            this.boardHeight = boardHeight;
            this.mainFont = new Font("Consolas", 20);
        }

        public void Draw(Graphics g)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;      // Horizontal Alignment
            stringFormat.LineAlignment = StringAlignment.Center;  // Vertical Alignment
            int tmpX = (boardWidth / 2);
            int tmpY = (boardHeight / 4);
            g.DrawString("Multi Bricks", mainFont, Brushes.Black, tmpX, tmpY, stringFormat);
            tmpX = (boardWidth / 2);
            tmpY = (boardHeight / 4 * 2);
            g.DrawString("Click to Start", mainFont, Brushes.Red, tmpX, tmpY, stringFormat);
        }
    }
}

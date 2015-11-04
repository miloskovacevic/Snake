using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        List<Circle> snake = new List<Circle>();
        Circle food = new Circle();

        public Form1()
        {
            InitializeComponent();

            //setovanje settings na nulu...
            new Settings();

            //setuj brzinu igre i startuj tajmer
            gameTimer.Interval = 2000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            //start new game
            StartGame();
        }

        private void StartGame()
        {
            
            lblGameOver.Visible = false;

            //setovanje settings na nulu...
            new Settings();

            // iz nekog razloga ovo gore ne vraca GameOver na false pa moram ovde...
            Settings.GameOver = false;

            //ocisti zmiju iz predjasnje igre... i kreiraj novog player-a...
            snake.Clear();

            Circle head = new Circle();
            head.X = 10;
            head.Y = 5;
            snake.Add(head);

            lblScore.Text = Settings.Score.ToString();

            GenerateFood();

        }

        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle();
            food.X = random.Next(0, maxXPos);
            food.Y = random.Next(0, maxYPos);
        }

        private void UpdateScreen(object sender, EventArgs e)
        { 
            // provjeri game over...
            if (Settings.GameOver == true)
            {
                // check if enter is pressed...
                if (Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                {
                    Settings.direction = Direction.Right;
                }
                else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                {
                    Settings.direction = Direction.Left;
                }
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                {
                    Settings.direction = Direction.Up;
                }
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                {
                    Settings.direction = Direction.Down;
                }

                MovePlayer();
            }

            pbCanvas.Invalidate();


        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (Settings.GameOver == false)
            {
                // boja zmije
                Brush snakeColour;


                // nacrtaj zmiju
                for (int i = 0; i < snake.Count; i++)
                { 
                    //crtaj glavu...
                    if (i == 0)
                    {
                        snakeColour = Brushes.Black;
                    }
                    else
                    {
                        snakeColour = Brushes.Green;
                    }

                    canvas.FillEllipse(snakeColour, new Rectangle(snake[i].X * Settings.Width, 
                                                    snake[i].Y * Settings.Height,
                                                    Settings.Width, Settings.Height));
                    
                    // crtamo hranu
                    canvas.FillEllipse(Brushes.Red, new Rectangle(food.X * Settings.Width,
                                                    food.Y * Settings.Height, 
                                                    Settings.Width, Settings.Height));
                }
            }
            else
            {
                string gameOver = "Game over \n Your score is : " + Settings.Score + "\nPress enter to try again!";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;
            }

        }

        private void MovePlayer()
        {
            for (int i = snake.Count - 1; i >= 0; i--)
            { 
                //move head
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            snake[i].X++;
                            break;
                        case Direction.Left:
                            snake[i].X--;
                            break;
                        case Direction.Up:
                            snake[i].Y--;
                            break;
                        case Direction.Down:
                            snake[i].Y++;
                            break;
                    }

                    // vrati max x i y poziciju...
                    int maxXPos = pbCanvas.Width / Settings.Width;
                    int maxYPos = pbCanvas.Height / Settings.Height;
                    
                    //detect collisions with game borders...
                    if (snake[i].X < 0 || snake[i].Y < 0
                        || snake[i].Y >= maxYPos || snake[i].X >= maxXPos)
                    {
                       Die();
                    }

                    //detect  self-collision with body...
                    for (int j = 1; j < snake.Count; j++)
                    {
                        if (snake[i].X == snake[j].X && snake[i].Y == snake[j].Y)
                        {
                           Die();
                        }
                    }

                    //detect collision with food piece...
                    if (snake[0].X == food.X && snake[0].Y == food.Y)
                    {
                       Eat();
                    }
                    

                }
                else
                { 
                    //move body
                    snake[i].X = snake[i - 1].X;
                    snake[i].Y = snake[i - 1].Y;
                }
            }
        }

        private void Eat()
        {
            //add circle to body
            Circle food = new Circle();
            food.X = snake[snake.Count - 1].X;
            food.Y = snake[snake.Count - 1].Y;
            snake.Add(food);

            //update score
            Settings.Score += Settings.Points;
            lblScore.Text = Settings.Score.ToString();

            GenerateFood();
        }

        private void Die()
        {
            Settings.GameOver = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }
    }
}

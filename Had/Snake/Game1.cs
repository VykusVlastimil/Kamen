using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Snake
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Game objects
        private List<Vector2> _snake;
        private Vector2 _food;
        private Vector2 _direction;
        private bool _gameOver;
        private bool _victory;

        // Timing for slower movement
        private float _timer;
        private const float MOVE_DELAY = 0.15f; // Time in seconds between moves (adjust this to make faster/slower)

        // Game constants
        private const int CELL_SIZE = 20;
        private const int GRID_WIDTH = 30;
        private const int GRID_HEIGHT = 20;
        private readonly int MAX_SNAKE_LENGTH = 30 * 20;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 400;
            _graphics.ApplyChanges();

            ResetGame();

            base.Initialize();
        }

        private void ResetGame()
        {
            _snake = new List<Vector2> { new Vector2(5, 5) };
            _direction = Vector2.UnitX;
            _gameOver = false;
            _victory = false;
            _timer = 0f;
            SpawnFood();
        }

        private void SpawnFood()
        {
            var possiblePositions = new List<Vector2>();

            for (int x = 0; x < GRID_WIDTH; x++)
            {
                for (int y = 0; y < GRID_HEIGHT; y++)
                {
                    var pos = new Vector2(x, y);
                    if (!_snake.Contains(pos))
                    {
                        possiblePositions.Add(pos);
                    }
                }
            }

            if (possiblePositions.Count > 0)
            {
                _food = possiblePositions[Random.Shared.Next(possiblePositions.Count)];
            }
            else
            {
                _victory = true;
                _gameOver = true;
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_gameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    ResetGame();
                return;
            }

            // Input handling (always responsive)
            if (Keyboard.GetState().IsKeyDown(Keys.W) && _direction != Vector2.UnitY) _direction = -Vector2.UnitY;
            if (Keyboard.GetState().IsKeyDown(Keys.S) && _direction != -Vector2.UnitY) _direction = Vector2.UnitY;
            if (Keyboard.GetState().IsKeyDown(Keys.A) && _direction != Vector2.UnitX) _direction = -Vector2.UnitX;
            if (Keyboard.GetState().IsKeyDown(Keys.D) && _direction != -Vector2.UnitX) _direction = Vector2.UnitX;

            // Update timer with elapsed time
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Only move snake when timer reaches the delay
            if (_timer >= MOVE_DELAY)
            {
                _timer = 0f; // Reset timer

                // Move snake
                Vector2 newHead = _snake[0] + _direction;

                // Check collisions
                if (newHead.X < 0 || newHead.X >= GRID_WIDTH ||
                    newHead.Y < 0 || newHead.Y >= GRID_HEIGHT ||
                    _snake.Contains(newHead))
                {
                    _gameOver = true;
                    _victory = false;
                    return;
                }

                _snake.Insert(0, newHead);

                // Check food collision
                if (newHead == _food)
                {
                    if (_snake.Count >= MAX_SNAKE_LENGTH)
                    {
                        _victory = true;
                        _gameOver = true;
                    }
                    else
                    {
                        SpawnFood();
                    }
                }
                else
                {
                    _snake.RemoveAt(_snake.Count - 1);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Draw snake
            foreach (var segment in _snake)
            {
                DrawRectangle((int)segment.X * CELL_SIZE, (int)segment.Y * CELL_SIZE, CELL_SIZE, CELL_SIZE, Color.Lime);
            }

            // Draw food (if game is still running)
            if (!_gameOver)
            {
                DrawRectangle((int)_food.X * CELL_SIZE, (int)_food.Y * CELL_SIZE, CELL_SIZE, CELL_SIZE, Color.Red);
            }

            // Draw victory/loss indicators
            if (_gameOver)
            {
                if (_victory)
                {
                    // Victory - draw green rectangle covering most of the screen
                    DrawRectangle(50, 50, 500, 300, new Color(0, 255, 0, 128));
                }
                else
                {
                    // Loss - draw red triangle
                    DrawTriangle(200, 100, 400, 100, 300, 300, Color.Red);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { color });
            _spriteBatch.Draw(texture, new Rectangle(x, y, width, height), color);
        }

        private void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { color });

            int minX = System.Math.Min(x1, System.Math.Min(x2, x3));
            int minY = System.Math.Min(y1, System.Math.Min(y2, y3));
            int maxX = System.Math.Max(x1, System.Math.Max(x2, x3));
            int maxY = System.Math.Max(y1, System.Math.Max(y2, y3));

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (IsPointInTriangle(x, y, x1, y1, x2, y2, x3, y3))
                    {
                        _spriteBatch.Draw(texture, new Rectangle(x, y, 1, 1), color);
                    }
                }
            }
        }

        private bool IsPointInTriangle(int x, int y, int x1, int y1, int x2, int y2, int x3, int y3)
        {
            float denominator = ((y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3));
            float a = ((y2 - y3) * (x - x3) + (x3 - x2) * (y - y3)) / denominator;
            float b = ((y3 - y1) * (x - x3) + (x1 - x3) * (y - y3)) / denominator;
            float c = 1 - a - b;

            return a >= 0 && a <= 1 && b >= 0 && b <= 1 && c >= 0 && c <= 1;
        }
    }
}
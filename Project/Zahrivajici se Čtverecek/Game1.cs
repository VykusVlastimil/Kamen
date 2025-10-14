using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slippysquare

{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Square properties
        private Rectangle square;
        private Vector2 velocity;
        private Vector2 acceleration;
        private float maxSpeed = 60f;
        private float accelerationRate = 0.9988988989898f;
        private float friction = 0.95f;

        // Color properties
        private Color squareColor;
        private float colorIntensity = 0f; // 0 = black, 1 = red
        private float colorTransitionSpeed = 1f;

        // Wall properties
        private Rectangle topWall;
        private Rectangle bottomWall;
        private const int WALL_WIDTH = 50;
        private const int GAP_SIZE = 150;
        private float wallSpeed = 3f;
        private bool wallMovingDown = true;

        // Input
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        // Texture for drawing
        private Texture2D pixel;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Set resolution to 1920x1080
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            // Initialize square
            square = new Rectangle(100, 540, 60, 60);
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            squareColor = Color.Black;

            // Initialize walls
            UpdateWalls();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a 1x1 white pixel texture for drawing
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            // Handle input
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            HandleInput();
            UpdatePhysics();
            UpdateColor();
            UpdateWall();
            CheckCollisions();

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            acceleration = Vector2.Zero;

            // Apply acceleration based on input
            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A))
                acceleration.X -= accelerationRate;
            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D))
                acceleration.X += accelerationRate;
            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W))
                acceleration.Y -= accelerationRate;
            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S))
                acceleration.Y += accelerationRate;
        }

        private void UpdatePhysics()
        {
            // Apply acceleration to velocity
            velocity += acceleration;

            // Limit maximum speed
            if (velocity.Length() > maxSpeed)
                velocity = Vector2.Normalize(velocity) * maxSpeed;

            // Apply friction (slippery movement)
            velocity *= friction;

            // Stop movement if velocity is very small
            if (velocity.Length() < 0.1f)
                velocity = Vector2.Zero;

            // Update position
            square.X += (int)velocity.X;
            square.Y += (int)velocity.Y;

            // Keep square within screen bounds
            square.X = MathHelper.Clamp(square.X, 0, 1920 - square.Width);
            square.Y = MathHelper.Clamp(square.Y, 0, 1080 - square.Height);
        }

        private void UpdateColor()
        {
            // Calculate color intensity based on current speed
            float targetIntensity = velocity.Length() / maxSpeed;

            // Smoothly transition towards target intensity
            if (targetIntensity > colorIntensity)
                colorIntensity = targetIntensity;
            else
                colorIntensity = MathHelper.Lerp(colorIntensity, targetIntensity, colorTransitionSpeed);

            // Set color from black to red based on intensity
            squareColor = Color.Lerp(Color.Black, Color.Red, colorIntensity);
        }

        private void UpdateWall()
        {
            // Move wall up and down
            if (wallMovingDown)
            {
                if (topWall.Bottom < 1080 - GAP_SIZE)
                    UpdateWallsPosition(wallSpeed);
                else
                    wallMovingDown = false;
            }
            else
            {
                if (topWall.Bottom > GAP_SIZE)
                    UpdateWallsPosition(-wallSpeed);
                else
                    wallMovingDown = true;
            }
        }

        private void UpdateWallsPosition(float movement)
        {
            // Update wall rectangles based on movement
            int wallMiddle = 1920 / 2 - WALL_WIDTH / 2;

            topWall = new Rectangle(wallMiddle, 0, WALL_WIDTH,
                (int)(topWall.Height + movement));

            bottomWall = new Rectangle(wallMiddle, topWall.Height + GAP_SIZE,
                WALL_WIDTH, 1080 - (topWall.Height + GAP_SIZE));
        }

        private void UpdateWalls()
        {
            int wallMiddle = 1920 / 2 - WALL_WIDTH / 2;
            topWall = new Rectangle(wallMiddle, 0, WALL_WIDTH, 400);
            bottomWall = new Rectangle(wallMiddle, 400 + GAP_SIZE, WALL_WIDTH, 1080 - (400 + GAP_SIZE));
        }

        private void CheckCollisions()
        {
            // Check collision with walls
            if (square.Intersects(topWall) || square.Intersects(bottomWall))
            {
                // Simple collision response - push square out
                if (square.Intersects(topWall))
                {
                    square.Y = topWall.Bottom;
                    velocity.Y = MathHelper.Clamp(velocity.Y, 0, maxSpeed);
                }
                if (square.Intersects(bottomWall))
                {
                    square.Y = bottomWall.Top - square.Height;
                    velocity.Y = MathHelper.Clamp(velocity.Y, -maxSpeed, 0);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();

            // Draw square
            _spriteBatch.Draw(pixel, square, squareColor);

            // Draw walls
            _spriteBatch.Draw(pixel, topWall, Color.DarkBlue);
            _spriteBatch.Draw(pixel, bottomWall, Color.DarkBlue);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
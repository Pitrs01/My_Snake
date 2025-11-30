using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Snake
{
    public class Snake : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D snakeTexture;
        private Apple apple;
        private Pear pear;
        private List<Vector2> snakeBody;
        private Vector2 direction;
        private Vector2 nextDirection;
        private int cellSize = 20;
        private float moveTimer;
        private float moveInterval = 0.15f;
        private int score = 0;
        private int level = 1;
        private int targetLength = 6;
        private const int maxLevel = 20;
        private Random random;
        private bool gameOver;

        public Snake()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "Snake Game";
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            random = new Random();
            snakeBody = new List<Vector2>();

            // Startovní pozice hada uprostřed obrazovky
            int startX = (_graphics.PreferredBackBufferWidth / 2 / cellSize) * cellSize;
            int startY = (_graphics.PreferredBackBufferHeight / 2 / cellSize) * cellSize;

            snakeBody.Add(new Vector2(startX, startY));
            snakeBody.Add(new Vector2(startX - cellSize, startY));
            snakeBody.Add(new Vector2(startX - cellSize * 2, startY));

            direction = new Vector2(1, 0);
            nextDirection = direction;
            gameOver = false;
            level = 1;
            targetLength = 6;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Vytvoření textury pro hada
            snakeTexture = new Texture2D(GraphicsDevice, cellSize, cellSize);
            Color[] data = new Color[cellSize * cellSize];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.LightGreen;
            }
            snakeTexture.SetData(data);

            // Vytvoření hrušky PŘED jablkem
            pear = new Pear(GraphicsDevice, cellSize);

            // Vytvoření jablka
            apple = new Apple(GraphicsDevice, cellSize);
            SpawnApple();

            // Spawn první hrušky hned na začátku
            if (random.Next(0, 100) < 100)
            {
                SpawnPear();
            }
        }

        private void SpawnApple()
        {
            int maxX = _graphics.PreferredBackBufferWidth / cellSize;
            int maxY = (_graphics.PreferredBackBufferHeight - 40) / cellSize;

            Vector2 newPosition;
            do
            {
                newPosition = new Vector2(
                    random.Next(0, maxX) * cellSize,
                    random.Next(2, maxY) * cellSize + 40
                );
            } while (snakeBody.Contains(newPosition) || (pear.IsActive && newPosition == pear.Position));

            apple.Position = newPosition;
        }

        private void SpawnPear()
        {
            int maxX = _graphics.PreferredBackBufferWidth / cellSize;
            int maxY = (_graphics.PreferredBackBufferHeight - 40) / cellSize;

            Vector2 newPosition;
            do
            {
                newPosition = new Vector2(
                    random.Next(0, maxX) * cellSize,
                    random.Next(2, maxY) * cellSize + 40
                );
            } while (snakeBody.Contains(newPosition) || newPosition == apple.Position);

            pear.Position = newPosition;
            pear.IsActive = true;
        }

        private void CheckLevelCompletion()
        {
            if (snakeBody.Count == targetLength)
            {
                level++;

                if (level > maxLevel)
                {
                    gameOver = true;
                    return;
                }

                // Střídavě větší a menší
                if (level % 2 == 0)
                {
                    targetLength = Math.Max(3, snakeBody.Count - (2 + level / 3));
                }
                else
                {
                    targetLength = snakeBody.Count + (3 + level / 2);
                }

                score += 50;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (gameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    Initialize();
                    score = 0;
                    LoadContent();
                }
                return;
            }

            KeyboardState stavKlavesnice = Keyboard.GetState();

            if (stavKlavesnice.IsKeyDown(Keys.A) || stavKlavesnice.IsKeyDown(Keys.Left))
            {
                if (direction.X == 0)
                    nextDirection = new Vector2(-1, 0);
            }
            else if (stavKlavesnice.IsKeyDown(Keys.D) || stavKlavesnice.IsKeyDown(Keys.Right))
            {
                if (direction.X == 0)
                    nextDirection = new Vector2(1, 0);
            }
            else if (stavKlavesnice.IsKeyDown(Keys.W) || stavKlavesnice.IsKeyDown(Keys.Up))
            {
                if (direction.Y == 0)
                    nextDirection = new Vector2(0, -1);
            }
            else if (stavKlavesnice.IsKeyDown(Keys.S) || stavKlavesnice.IsKeyDown(Keys.Down))
            {
                if (direction.Y == 0)
                    nextDirection = new Vector2(0, 1);
            }

            moveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (moveTimer >= moveInterval)
            {
                moveTimer = 0;
                MoveSnake();
            }

            base.Update(gameTime);
        }

        private void MoveSnake()
        {
            direction = nextDirection;
            Vector2 newHead = snakeBody[0] + direction * cellSize;

            if (newHead.X < 0 || newHead.X >= _graphics.PreferredBackBufferWidth ||
                newHead.Y < 40 || newHead.Y >= _graphics.PreferredBackBufferHeight)
            {
                gameOver = true;
                return;
            }

            if (snakeBody.Contains(newHead))
            {
                gameOver = true;
                return;
            }

            snakeBody.Insert(0, newHead);

            if (newHead == apple.Position)
            {
                score += 10;
                SpawnApple();
                CheckLevelCompletion();
            }
            else if (pear.IsActive && newHead == pear.Position)
            {
                score -= 5;
                pear.IsActive = false;

                if (snakeBody.Count > 2)
                {
                    snakeBody.RemoveAt(snakeBody.Count - 1);
                    snakeBody.RemoveAt(snakeBody.Count - 1);
                }
                else
                {
                    gameOver = true;
                    return;
                }

                if (random.Next(0, 100) < 100)
                {
                    SpawnPear();
                }

                CheckLevelCompletion();
            }
            else
            {
                snakeBody.RemoveAt(snakeBody.Count - 1);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            apple.Draw(_spriteBatch);
            pear.Draw(_spriteBatch);

            for (int i = 0; i < snakeBody.Count; i++)
            {
                Color color = i == 0 ? Color.DarkGreen : Color.White;
                _spriteBatch.Draw(snakeTexture, snakeBody[i], color);
            }

            DrawGameInfo();

            if (gameOver)
            {
                Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.Black });
                _spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 600), Color.Black * 0.7f);

                bool won = level > maxLevel;
                if (won)
                {
                    _spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 600), Color.Green * 0.3f);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGameInfo()
        {
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            _spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 40), Color.Black * 0.5f);

            int currentLength = snakeBody.Count;
            int barWidth = 200;
            int barHeight = 20;
            int barX = 10;
            int barY = 10;

            _spriteBatch.Draw(pixel, new Rectangle(barX, barY, barWidth, barHeight), Color.DarkGray);

            float progress;
            Color barColor;

            if (targetLength > currentLength)
            {
                progress = (float)currentLength / targetLength;
                barColor = Color.LimeGreen;
            }
            else if (targetLength < currentLength)
            {
                progress = 1f - ((float)(currentLength - targetLength) / currentLength);
                barColor = Color.Orange;
            }
            else
            {
                progress = 1f;
                barColor = Color.Gold;
            }

            int fillWidth = (int)(barWidth * progress);
            _spriteBatch.Draw(pixel, new Rectangle(barX, barY, fillWidth, barHeight), barColor);

            DrawRectangleOutline(pixel, new Rectangle(barX, barY, barWidth, barHeight), Color.White, 2);

            int infoX = barX + barWidth + 20;

            DrawInfoBox(pixel, infoX, barY, 80, barHeight, Color.Blue);
            DrawInfoBox(pixel, infoX + 90, barY, 120, barHeight,
                currentLength == targetLength ? Color.Gold : (targetLength > currentLength ? Color.Green : Color.Orange));
            DrawInfoBox(pixel, infoX + 220, barY, 100, barHeight, Color.Purple);
        }

        private void DrawInfoBox(Texture2D pixel, int x, int y, int width, int height, Color color)
        {
            _spriteBatch.Draw(pixel, new Rectangle(x, y, width, height), color * 0.3f);
            DrawRectangleOutline(pixel, new Rectangle(x, y, width, height), color, 2);
        }

        private void DrawRectangleOutline(Texture2D pixel, Rectangle rect, Color color, int thickness)
        {
            _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
            _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            _spriteBatch.Draw(pixel, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
        }
    }
}
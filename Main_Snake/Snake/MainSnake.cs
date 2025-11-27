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
        private SpriteFont font;
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

            direction = new Vector2(1, 0); // Pohyb doprava
            nextDirection = direction; // Inicializace nextDirection
            gameOver = false;

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

            // Vytvoření hrušky PŘED jablkem (kvůli kontrole v SpawnApple)
            pear = new Pear(GraphicsDevice, cellSize);

            // Vytvoření jablka
            apple = new Apple(GraphicsDevice, cellSize);
            SpawnApple();

            // Spawn první hrušky hned na začátku (100% šance)
            if (random.Next(0, 100) < 100)
            {
                SpawnPear();
            }
        }

        private void SpawnApple()
        {
            int maxX = _graphics.PreferredBackBufferWidth / cellSize;
            int maxY = _graphics.PreferredBackBufferHeight / cellSize;

            Vector2 newPosition;
            do
            {
                newPosition = new Vector2(
                    random.Next(0, maxX) * cellSize,
                    random.Next(0, maxY) * cellSize
                );
            } while (snakeBody.Contains(newPosition) || (pear.IsActive && newPosition == pear.Position));

            apple.Position = newPosition;
        }

        private void SpawnPear()
        {
            int maxX = _graphics.PreferredBackBufferWidth / cellSize;
            int maxY = _graphics.PreferredBackBufferHeight / cellSize;

            Vector2 newPosition;
            do
            {
                newPosition = new Vector2(
                    random.Next(0, maxX) * cellSize,
                    random.Next(0, maxY) * cellSize
                );
            } while (snakeBody.Contains(newPosition) || newPosition == apple.Position);

            pear.Position = newPosition;
            pear.IsActive = true;
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
                }
                return;
            }

            KeyboardState stavKlavesnice = Keyboard.GetState();

            // Ovládání směru (nelze otočit o 180°)
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

            // Pohyb hada s časovačem
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
            // Aplikace nextDirection na direction před pohybem
            direction = nextDirection;

            // Nová pozice hlavy
            Vector2 newHead = snakeBody[0] + direction * cellSize;

            // Kontrola kolize se zdí
            if (newHead.X < 0 || newHead.X >= _graphics.PreferredBackBufferWidth ||
                newHead.Y < 0 || newHead.Y >= _graphics.PreferredBackBufferHeight)
            {
                gameOver = true;
                return;
            }

            // Kontrola kolize se sebou samým
            if (snakeBody.Contains(newHead))
            {
                gameOver = true;
                return;
            }

            // Přidání nové hlavy
            snakeBody.Insert(0, newHead);

            // Kontrola kolize s jablkem
            if (newHead == apple.Position)
            {
                score += 10;
                SpawnApple();
                // Had se prodlouží (neodstraníme ocas)
            }
            // Kontrola kolize s hruškou
            else if (pear.IsActive && newHead == pear.Position)
            {
                score -= 5;
                pear.IsActive = false;

                // Had se zmenší o 2 (normální pohyb + extra zkrácení)
                if (snakeBody.Count > 2)
                {
                    snakeBody.RemoveAt(snakeBody.Count - 1); // První zkrácení
                    snakeBody.RemoveAt(snakeBody.Count - 1); // Druhé zkrácení
                }
                else
                {
                    // Had je moc krátký - game over
                    gameOver = true;
                    return;
                }

                // 90% šance na spawn nové hrušky po snědení
                if (random.Next(0, 100) < 90)
                {
                    SpawnPear();
                }
            }
            else
            {
                // Normální pohyb - odstranění konce hada
                snakeBody.RemoveAt(snakeBody.Count - 1);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Vykreslení jablka
            apple.Draw(_spriteBatch);

            // Vykreslení hrušky
            pear.Draw(_spriteBatch);

            // Vykreslení hada
            for (int i = 0; i < snakeBody.Count; i++)
            {
                Color color = i == 0 ? Color.DarkGreen : Color.White; // Hlava tmavší
                _spriteBatch.Draw(snakeTexture, snakeBody[i], color);
            }

            if (gameOver)
            {
                // Zobrazení game over zprávy (bez fontu, jen barevný overlay)
                Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.Black });
                _spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 600), Color.Black * 0.7f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
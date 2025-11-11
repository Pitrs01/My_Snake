using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Snake
{
    public class Snake : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D snakeTexture;
        int x, y;
        int height, width;
        int speed = 2;

        public Snake()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "Snake";
        }

        protected override void Initialize()
        {

            width = height = 20;
            x = _graphics.PreferredBackBufferWidth / 2;
            y = _graphics.PreferredBackBufferHeight / 2;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            snakeTexture = new Texture2D(GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.LightGreen;
            }
        snakeTexture.SetData(data);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState stavKlavesnice = Keyboard.GetState();

            if (stavKlavesnice.IsKeyDown(Keys.A))
            {
                x -= speed;
            }

            else if (stavKlavesnice.IsKeyDown(Keys.D))
            {
                x += speed;
            }

            else if (stavKlavesnice.IsKeyDown(Keys.W))
            {
                y -= speed;
            }

            else if (stavKlavesnice.IsKeyDown(Keys.S))
            {   
                y += speed;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(snakeTexture, new Vector2(x, y), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake
{
    public class Apple
    {
        private Texture2D texture;
        public Vector2 Position { get; set; }
        private int size;

        public Apple(GraphicsDevice graphicsDevice, int cellSize)
        {
            size = cellSize;

            // Vytvoření červené textury pro jablko
            texture = new Texture2D(graphicsDevice, cellSize, cellSize);
            Color[] data = new Color[cellSize * cellSize];

            // Vytvoření kruhu pro jablko
            int center = cellSize / 2;
            int radius = cellSize / 2 - 2;

            for (int x = 0; x < cellSize; x++)
            {
                for (int y = 0; y < cellSize; y++)
                {
                    int index = x + y * cellSize;
                    int dx = x - center;
                    int dy = y - center;

                    if (dx * dx + dy * dy <= radius * radius)
                    {
                        data[index] = Color.Red;
                    }
                    else
                    {
                        data[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(data);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
        }
    }
}
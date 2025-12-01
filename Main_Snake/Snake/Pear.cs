using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake
{
    public class Pear : ICollectible
    {
        private Texture2D texture;
        public Vector2 Position { get; set; }
        public bool IsActive { get; set; }
        public int ScoreValue => -5;
        public int LengthChange => -1;
        private int size;

        public Pear(GraphicsDevice graphicsDevice, int cellSize)
        {
            size = cellSize;
            IsActive = false;

            // Vytvoření žluté textury pro hrušku
            texture = new Texture2D(graphicsDevice, cellSize, cellSize);
            Color[] data = new Color[cellSize * cellSize];

            // Vytvoření tvaru hrušky (širší dole, užší nahoře)
            int centerX = cellSize / 2;

            for (int x = 0; x < cellSize; x++)
            {
                for (int y = 0; y < cellSize; y++)
                {
                    int index = x + y * cellSize;
                    int dx = x - centerX;

                    // Horní část hrušky (menší)
                    if (y < cellSize / 3)
                    {
                        int radiusTop = cellSize / 4;
                        int dy = y - cellSize / 4;
                        if (dx * dx + dy * dy <= radiusTop * radiusTop)
                        {
                            data[index] = Color.Yellow;
                        }
                        else
                        {
                            data[index] = Color.Transparent;
                        }
                    }
                    // Dolní část hrušky (větší)
                    else
                    {
                        int radiusBottom = cellSize / 2 - 2;
                        int dy = y - cellSize * 2 / 3;
                        if (dx * dx + dy * dy <= radiusBottom * radiusBottom)
                        {
                            data[index] = Color.Yellow;
                        }
                        else
                        {
                            data[index] = Color.Transparent;
                        }
                    }
                }
            }

            texture.SetData(data);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(texture, Position, Color.White);
            }
        }
    }
}
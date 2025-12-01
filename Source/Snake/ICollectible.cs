using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake
{
    /// <summary>
    /// Rozhraní pro objekty, které může had sebrat
    /// </summary>
    public interface ICollectible
    {
        /// <summary>
        /// Pozice objektu na herní ploše
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Zda je objekt aktivní (viditelný a sbíratelný)
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Bodová hodnota objektu (může být i záporná)
        /// </summary>
        int ScoreValue { get; }

        /// <summary>
        /// Změna délky hada při sebrání (-1 = zmenší, +1 = zvětší, 0 = beze změny)
        /// </summary>
        int LengthChange { get; }

        /// <summary>
        /// Vykreslení objektu na obrazovku
        /// </summary>
        void Draw(SpriteBatch spriteBatch);
    }
}
// Implementing the blast bar to destroy on-screen obstacles

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace Avoid.Sprites
{
    class BlastBar
    {
        // Variables 
        private Texture2D blastBarSpriteSheetImage;
        private SpriteSheet blastBarSpriteSheet;
        private GraphicsDevice graphics_device;
        private float localBlast = 0;
        private Vector2 blastBarPosition = new Vector2(440, 1080);

        // Variables to deal with shaking
        // The blast bar will shake when blast is used
        // This is the exact same as the screen shake that occurs when the void turns on
        private bool shakeViewport = false;
        private float shakeStartAngle = 0;
        private float shakeRadius = 10;
        Random rand = new Random();
        public BlastBar (GraphicsDevice _graphics_device)
        {
            graphics_device = _graphics_device;
        }
        public void LoadContent(ContentManager _content)
        {
            blastBarSpriteSheetImage = _content.Load<Texture2D>("images/blast_spritesheet");
            blastBarSpriteSheet = new SpriteSheet(blastBarSpriteSheetImage, 4, (1040, 100), 1);
            blastBarSpriteSheet.frame = 3;
        }

        // Update based on changes from GameState
        // If a change is detected, update the graphics of the blast bar (change the yellow portions to grey by updating the spritesheet frame)
        // As for the actual blast logic, that is dealt with in ObstacleManager
        public void Update(float blast)
        {
            if (blast < localBlast)
            {
                blastBarSpriteSheet.UpdateFrame();
                shakeViewport = true;
                localBlast = blast;

            }
            // Only show the blast bar if there is blast available
            // If blast becomes available, it should slide up to its position rather than abruptly appear
            // The inverse goes for when blast becomes empty
            else if (blast > localBlast)
            {
                if (localBlast > 0)
                {
                    shakeViewport = true;
                }
                blastBarSpriteSheet.frame = 0;
                blastBarPosition.Y -= 10;
                if (blastBarPosition.Y <= 890)
                    localBlast = blast;
            }
            else
            {
                if (blast == 0)
                {
                    if (shakeRadius == 10)
                    {
                        if (blastBarPosition.Y < 1080)
                            blastBarPosition.Y += 10;
                    }
                }
            }
        }

        // Draw blast bar and offset it to shake if necessary
        public void Draw(SpriteBatch _spriteBatch)
        {
            Rectangle sourceRect = blastBarSpriteSheet.FrameRect();
            Vector2 offset = Vector2.Zero;
            if (shakeViewport)
            {
                offset = new Vector2((float)(Math.Sin(shakeStartAngle) * shakeRadius), (float)(Math.Cos(shakeStartAngle) * shakeRadius));
                shakeRadius -= 0.25f;
                shakeStartAngle += (150 + rand.Next(60));
                if (shakeRadius <= 0)
                {
                    shakeViewport = false;
                    shakeStartAngle = 0;
                    shakeRadius = 10;
                }
                _spriteBatch.End();
                _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(offset.X, offset.Y, 0));
            }
            _spriteBatch.Draw(blastBarSpriteSheetImage, blastBarPosition, sourceRect, Color.White);
            if (shakeViewport)
            {
                _spriteBatch.End();
                _spriteBatch.Begin();
            }
        }
    }
}

// Implementing the health bar

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace Avoid.Sprites
{
    class HealthBar
    {

        // The health bar is very similar to BlastBar

        private Texture2D healthBarSpriteSheetImage;
        private SpriteSheet healthBarSpriteSheet;
        private GraphicsDevice graphics_device;
        private float localHealth = 5;

        private bool shakeViewport = false;
        private float shakeStartAngle = 0;
        private float shakeRadius = 10;
        Random rand = new Random();
        public HealthBar (GraphicsDevice _graphics_device)
        {
            graphics_device = _graphics_device;
        }

        // Instead of 4 frames like in the blast bar, the health bar uses 6 frames
        public void LoadContent(ContentManager _content)
        {
            healthBarSpriteSheetImage = _content.Load<Texture2D>("images/health_bar_spritesheet");
            healthBarSpriteSheet = new SpriteSheet(healthBarSpriteSheetImage, 6, (560, 100), 1);
            healthBarSpriteSheet.UpdateFrame();
        }

        public void Update(float health)
        {
            if (health < localHealth)
            {
                healthBarSpriteSheet.UpdateFrame();
                shakeViewport = true;
            }
            if (health > localHealth)
                healthBarSpriteSheet.UpdateFrame(-1);
            localHealth = health;
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            Rectangle sourceRect = healthBarSpriteSheet.FrameRect();
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
            _spriteBatch.Draw(healthBarSpriteSheetImage, new Vector2(680, 50), sourceRect, Color.White);
            if (shakeViewport)
            {
                _spriteBatch.End();
                _spriteBatch.Begin();
            }
        }
    }
}

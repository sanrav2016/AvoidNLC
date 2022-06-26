// Implement a player

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;

namespace Avoid.Sprites
{
    public class Player
    {
        // The player (the rocket) is the centerpiece of the game!
        // It must respond to input, move between orbit rings accordingly through an animation, animate smoke from the booster, and draw a shield if necessary

        public SpriteSheet playerSpriteSheet;
        private Texture2D playerSpriteSheetImage;
        private Texture2D shieldBubbleImage;
        public Vector2 playerPosition;
        private GraphicsDevice graphics_device;
        private int animateToPosition = -1;
        public float scale = 0.3f;
        public bool shieldBubble = false;
        private float shieldStartTime = -1;

        private Vector2[] positions =
        {
            new Vector2(960, 300),
            new Vector2(960, 450),
            new Vector2(960, 600)
        };

        public Player(GraphicsDevice _graphics_device)
        {
            graphics_device = _graphics_device;
        }
        public void LoadContent(ContentManager content)
        {
            playerPosition = positions[1];
            playerSpriteSheetImage = content.Load<Texture2D>("images/player_spritesheet");
            playerSpriteSheet = new SpriteSheet(playerSpriteSheetImage, 60, (600, 556));
            shieldBubbleImage = content.Load<Texture2D>("images/shield_bubble");
        }

        // In Update, take input from the keyboard and gamepad
        // Use this to slowly move from one orbit ring to the next or the previous
        public void Update(GameTime gameTime, KeyboardStateExtended keyState, GamePadState gamePadState)
        {
            float delta = ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f);
            float speed = (int)((2000 * delta) / 5.0) * 5;
            if (animateToPosition > -1)
            {
                Vector2 toPos = positions[animateToPosition];
                if (playerPosition.Y > toPos.Y)
                {
                    playerPosition.Y -= speed;
                } else if (playerPosition.Y < toPos.Y)
                {
                    playerPosition.Y += speed;
                } else
                {
                    animateToPosition = -1;
                }
            }
            // With a gamepad, the left joystick controls the movement
            if (gamePadState.IsConnected)
            {
                if (gamePadState.ThumbSticks.Left.Y < -0.5f)
                    animateToPosition = 2;
                else if (gamePadState.ThumbSticks.Left.Y > 0.5f)
                    animateToPosition = 0;
                else if (gamePadState.ThumbSticks.Left.Y >= -0.5f && gamePadState.ThumbSticks.Left.Y <= 0.5f)
                    animateToPosition = 1;
            }
            // With a keyboard, the up and down or W and S keys do
            if (keyState.WasKeyJustDown(Keys.Up) || keyState.WasKeyJustDown(Keys.W))
            {
                if (playerPosition == positions[1] || animateToPosition == 1) animateToPosition = 0;
                else if (playerPosition == positions[2] || animateToPosition == 2) animateToPosition = 1;

            }
            if (keyState.WasKeyJustDown(Keys.Down) || keyState.WasKeyJustDown(Keys.S))
            {
                if (playerPosition == positions[0] || animateToPosition == 0) animateToPosition = 1;
                else if (playerPosition == positions[1] || animateToPosition == 1) animateToPosition = 2;
            }
            // Update the smoke animation
            playerSpriteSheet.UpdateFrame();
        }

        // Draw the player
        // If the shield is on, draw the shield and scale it periodically
        public void Draw(GameTime _gameTime, SpriteBatch _spriteBatch)
        {
            Rectangle sourceRect = playerSpriteSheet.FrameRect();
            _spriteBatch.Draw(playerSpriteSheetImage, playerPosition, sourceRect, Color.White, (float)(Math.PI / 2), new Vector2(600/2, 556/2), new Vector2(scale, scale), SpriteEffects.None, 0f);
            if (shieldBubble)
            {
                if (shieldStartTime == -1)
                    shieldStartTime = (float)_gameTime.TotalGameTime.TotalMilliseconds;
                float v = 0.2f * (float)Math.Sin((float)((_gameTime.TotalGameTime.TotalMilliseconds - shieldStartTime) / 1500)) + 1f;
                Vector2 shieldCenter = new Vector2(shieldBubbleImage.Width, shieldBubbleImage.Height) / 2;
                _spriteBatch.Draw(shieldBubbleImage, playerPosition, null, Color.White, 0, shieldCenter, new Vector2(v, v), SpriteEffects.None, 0f);
            } else
            {
                if (shieldStartTime != -1)
                    shieldStartTime = -1;
            }
        }
    }
}
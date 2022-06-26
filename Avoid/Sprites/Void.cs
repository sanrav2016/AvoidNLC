// Implenting the void
// aka the giant space laser

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Avoid.States;

namespace Avoid.Sprites
{
    class Void
    {
        private State game;
        private Texture2D voidSpriteSheetImage;
        private SpriteSheet voidSpriteSheet;
        private GraphicsDevice graphics_device;
        // The void needs access to the level data in order to know when to turn on and off
        // In the level data, void toggle is represented using an x
        private List<string> levelData = new List<string>();
        public bool on = false;
        private int counter = 0;
        private int delay;
        private Color voidColor;
        public Void(GraphicsDevice _graphics_device, List<string> _levelData, State _game, int _delay, Color _color)
        {
            graphics_device = _graphics_device;
            levelData = _levelData;
            delay = _delay;
            game = _game;
            voidColor = _color;
        }

        // Load the void spritesheet
        public void LoadContent(ContentManager content)
        {
            voidSpriteSheetImage = content.Load<Texture2D>("images/void_spritesheet");
            voidSpriteSheet = new SpriteSheet(voidSpriteSheetImage, 5, (360, 1200));
        }
        
        // Updating the void graphics
        // The void logic (player health loss) is implemented elsewhere in GameState
        public void Update()
        {
            if (levelData[3].Length > 0)
            {
                counter += 1;
                if (counter == delay)
                {
                    char x = levelData[3][0];
                    levelData[3] = levelData[3].Substring(1);
                    if (x == 'x')
                    {
                        if (on)
                        {
                            on = false;
                            GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                        } else
                        {
                            on = true;
                            // If the user has a gamepad, they will feel it vibrate when the void is on!
                            GamePad.SetVibration(PlayerIndex.One, 1f, 1f);
                        }
                    }
                    counter = 0;
                }
            }
            voidSpriteSheet.UpdateFrame();
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            Rectangle sourceRect = voidSpriteSheet.FrameRect();
            _spriteBatch.Draw(voidSpriteSheetImage, new Vector2(780, -100), sourceRect, voidColor);
        }
    }

}
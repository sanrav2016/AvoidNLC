// State class
// This class defines a game state and is instantiated for every page of the game
// It is analogous to a template for specific game states to be built off of

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Avoid.Sprites;

namespace Avoid.States
{
    public abstract class State
    {
        protected Game1 game;
        protected GraphicsDeviceManager graphics;
        protected GraphicsDevice graphics_device;
        protected ContentManager content;
        // These below variables are specific to the GameState
        public float health, score, blast;
        public bool shieldOn, backgroundMusicPlaying;
        public Player player;
        public State(Game1 _game, ContentManager _content)
        {
            game = _game;
            graphics = _game.graphics;
            graphics_device = _game.GraphicsDevice;
            content = _content;
        }
        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}

// Instructions state
// This is a simple state; it only renders the instructions image and the menu button to screen

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Avoid.Sprites;

namespace Avoid.States
{
    class Instructions : State
    {
        private Texture2D backgroundImage;
        private Texture2D instructionsImage;
        private Button menuButton;
        public Instructions(Game1 game, ContentManager content) : base(game, content)
        {

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundImage, Vector2.Zero, new Color(90, 90, 90));
            spriteBatch.Draw(instructionsImage, Vector2.Zero, Color.White);
            menuButton.Draw(spriteBatch);
        }

        public override void LoadContent()
        {
            backgroundImage = content.Load<Texture2D>("images/background2");
            instructionsImage = content.Load<Texture2D>("images/instructions");
            menuButton = new Button(game, "Menu", new Vector2(50, 50), true);
            menuButton.LoadContent(content);
        }
        public override void Update(GameTime gameTime)
        {
            if (menuButton.Clicked() || game.gamePadState.IsButtonDown(Buttons.Back))
            {
                game.ChangeState(game.Menu());
                game.nextState.backgroundMusicPlaying = true;
            }
        }
    }
}

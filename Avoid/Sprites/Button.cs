// Button is a class used to create in-game buttons. 
// I created this class myself since Monogame does not provide a solution for buttons, and even if it did, they probably wouldn't be as customizable as I would like.
// In addition, the use of a class like this helps avoid repeating code.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Avoid.Sprites
{
    class Button
    {
        // Variables
        private Game1 game;
        private Texture2D buttonImage;
        private Rectangle buttonRect;
        public string text;
        private Vector2 position;
        private bool small = false;
        private bool medium = false;
        public Button(Game1 _game, string _text, Vector2 _position, bool _small = false, bool _medium = false)
        {
            game = _game;
            text = _text;
            position = _position;
            small = _small;
            medium = _medium;
            // By default, the button is large (800x200). It can be set to small (250x100) or medium (700x150)
            int w = 800, h = 200;
            if (small)
            {
                w = 250;
                h = 100;
            }
            if (medium)
            {
                w = 700;
                h = 150;
            }
            buttonRect = new Rectangle((int)position.X, (int)position.Y, w, h);
        }

        // Load the correct image/Texture2D, based on the button size
        public void LoadContent(ContentManager content)
        {
            if (small)
                buttonImage = content.Load<Texture2D>("images/small_button_frame");
            else if (medium)
                buttonImage = content.Load<Texture2D>("images/medium_button_frame");
            else
                buttonImage = content.Load<Texture2D>("images/button_frame");
        }

        // Draw the button. Change the font size as necessary
        // In addition, change the button's tint and font color if it is being hovered over
        public void Draw(SpriteBatch spriteBatch)
        {
            Color buttonColor = Color.White;
            Color textColor = Color.Black;
            if (Hover())
            {
                buttonColor = Color.LightGray;
                textColor = Color.DarkMagenta;
            }
            spriteBatch.Draw(buttonImage, position, buttonColor);
            SpriteFont textFont = game.ArcadePI50;
            if (small) 
                textFont = game.ArcadePI30;
            if (medium)
                textFont = game.ArcadePI45;
            Vector2 stringSize = textFont.MeasureString(text);
            spriteBatch.DrawString(textFont, text, position + new Vector2(buttonImage.Width / 2, buttonImage.Height / 2) - stringSize / 2, textColor);
        }

        // This function returns whether the button has just been clicked
        public bool Clicked()
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            bool click = buttonRect.Contains(mousePosition) && mouseState.LeftButton == ButtonState.Pressed;
            return click;
        }

        // This function returns whether the button is being hovered over
        public bool Hover()
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            bool hover = buttonRect.Contains(mousePosition);
            return hover;
        }

    }
}

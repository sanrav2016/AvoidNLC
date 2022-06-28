// Menu state
// aka the title slide
// This is what the player sees when they boot up the game

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Avoid.Sprites;
using System.Threading;
using System;

namespace Avoid.States
{
    class Menu : State
    {
        private Texture2D menuControlsIcons;
        private Song backgroundMusic, ding;
        private Texture2D backgroundImage;
        private Texture2D titleImage;
        private Button playButton;
        private Button leaderboardButton;
        private Button instructionsButton;
        private float menuStartTime = -1;
        public Menu(Game1 game, ContentManager content) : base(game, content)
        {
        }

        // Draw all menu elements
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw the title image slowly scaling
            float c = (float)Math.Cos((float)((gameTime.TotalGameTime.TotalMilliseconds - menuStartTime) / 3000));
            int v = (int)(40 * c) + 100;
            float z = -0.1f * c + 1;
            Vector2 titleImageVector = new Vector2(titleImage.Width, titleImage.Height);
            spriteBatch.Draw(backgroundImage, Vector2.Zero, new Color(v, v, v));
            spriteBatch.Draw(titleImage, new Vector2(1294, 267) - (z * titleImageVector) / 2, null, Color.White, 0, Vector2.Zero, new Vector2(z, z), SpriteEffects.None, 0f);
            playButton.Draw(spriteBatch);
            leaderboardButton.Draw(spriteBatch);
            instructionsButton.Draw(spriteBatch);
            spriteBatch.DrawString(game.ArcadePI30, "a game by Sanjay R", new Vector2(170, 970), Color.Gray);
            game.levelManager.Draw(gameTime, spriteBatch);
            if (game.gamePadState.IsConnected)
                spriteBatch.Draw(menuControlsIcons, new Vector2(1680, 475), Color.White);
        }

        public override void LoadContent()
        {
            menuControlsIcons = content.Load<Texture2D>("images/menu_controls_icons");
            backgroundMusic = content.Load<Song>("audio/lobby");
            ding = content.Load<Song>("audio/ding");
            backgroundImage = content.Load<Texture2D>("images/background2");
            titleImage = content.Load<Texture2D>("images/title");
            playButton = new Button(game, "Start", new Vector2(950, 475), false, true);
            playButton.LoadContent(content);
            leaderboardButton = new Button(game, "Leaderboard", new Vector2(950, 650), false, true);
            leaderboardButton.LoadContent(content);
            instructionsButton = new Button(game, "How to Play", new Vector2(950, 825), false, true);
            instructionsButton.LoadContent(content);
            game.levelManager.LoadContent(content);
            GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
        }

        // Assign functionality to all the different on screen buttons
        public override void Update(GameTime gameTime)
        {
            if (menuStartTime == -1)
                menuStartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            // If a gamepad has been connected, add instructions for how to select on screen buttons to the button text
            if (playButton.Clicked() || game.gamePadState.IsButtonDown(Buttons.A))
            {
                MediaPlayer.Play(ding);
                MediaPlayer.IsRepeating = false;
                Thread.Sleep(1000);
                game.ChangeState(game.GameState());
            }
            if (leaderboardButton.Clicked() || game.gamePadState.IsButtonDown(Buttons.X))
                game.ChangeState(game.Leaderboard());
            if (instructionsButton.Clicked() || game.gamePadState.IsButtonDown(Buttons.Y))
                game.ChangeState(game.Instructions());
            if (!backgroundMusicPlaying)
            {
                MediaPlayer.Play(backgroundMusic, new TimeSpan());
                MediaPlayer.IsRepeating = true;
                backgroundMusicPlaying = true;
            }
            game.levelManager.Update(gameTime);
        }
    }
}
// Level manager
// We have levels, but now we need a level manager!
// This renders the level select box on the menu screen
// It also creates and stores the specific level data

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System.Collections.Generic;

namespace Avoid.States
{
    public class LevelManager
    {
        private Game1 game;
        private Texture2D arrow;
        private Texture2D levelSelectBackgroundImage;
        public List<Level> levels = new List<Level>();
        public int currentLevelId;
        public bool secretLevelPlayed = false;
        private bool showUpArrow = true, showDownArrow = true;
        private bool hoverUpArrow = false, hoverDownArrow = false;
        public LevelManager(Game1 _game)
        {
            game = _game;
            // Here is the data for all the levels, represented using strings
            // Representations are based on first letters (o -> obstacle, c -> coin, etc.)
            List<string> levelData = new List<string>();
            levelData.Add("            o     o                 cccc        oo                     oo  oo  oo");
            levelData.Add("         o        o       h       c cccc      s oo              b      oo  oo  oo");
            levelData.Add("              o   o                 cccc        oo                     oo  oo  oo");
            levelData.Add("                                                             x  x                ");
            levels.Add(new Level(0, "Demo", Color.White, levelData, 40, 3500, "audio/demo"));
            levelData = new List<string>();
            levelData.Add("oooo   c s ooo   o     o  o o   s           oooooooooo    ");
            levelData.Add("     b ooo    o          o  o            c  boooooooo   oo");
            levelData.Add("oooo   o   o    oo    o  o           h  c   oooo        oo");
            levelData.Add("              x x       x x              x  x             ");
            levels.Add(new Level(1, "Level 1", Color.Red, levelData, 25, 800, "audio/level1"));
            levelData = new List<string>();
            levelData.Add("oo  b oo ooooo     o   o   o    o       o     o     o     ");
            levelData.Add("   oo oo oosoo      o oho oco   o s  o     so    o      oo");
            levelData.Add("oo oo    ooooo       o   o   o  o       o     o  c  o   oo");
            levelData.Add("                x  x                   x x    x x         ");
            levels.Add(new Level(2, "Level 2", Color.Orange, levelData, 25, 650, "audio/level2"));
            levelData = new List<string>();
            levelData.Add("ooo oob oooocoooooc    s      o     os     ooooc   ooooooooo    oo");
            levelData.Add("oso ooo o h      os    o      o    ho           oo     ooos  oo  o");
            levelData.Add("ooo ooo ooooooooooo    o      s     o      oooooooooo       o  o  ");
            levelData.Add("          x   x    x x    x x    x x       x x                x  x");
            levels.Add(new Level(3, "Level 3", Color.Yellow, levelData, 15, 600, "audio/level3"));
            levelData = new List<string>();
            levelData.Add("o   o   o      oho  ooo soooo o o oco");
            levelData.Add(" o o o o ob    ooo  ooo ooooo  h o o ");
            levelData.Add("  o   o        oho  ooo ooooo c o o o");
            levelData.Add("                          x   x      ");
            levels.Add(new Level(4, "Level 4", Color.LimeGreen, levelData, 25, 450, "audio/level4"));
            levelData = new List<string>();
            levelData.Add("oo  oo c oosooooo     h c ooooo o oo   oo");
            levelData.Add("  oo  oo    oo    oooooob ooo so     oo  ");
            levelData.Add("oo coo   oo    oo   ooooo ooooooo oo  hoo");
            levelData.Add("              x     x            x    x  ");
            levels.Add(new Level(5, "Level 5", Color.Cyan, levelData, 30, 300, "audio/level5"));
            levelData = new List<string>();
            levelData.Add("oooo  o   o   o   o   o   o   o        oo   oo   oo  o   o     ccccccccccccccccccccccccccccccccc");
            levelData.Add("oooo   o o o o o o  h  o o o o o    b  oo   oo   oo   o o o    ccccccccccccccccccccccccccccccccc");
            levelData.Add("oooo    o   o   o       o   o   o      oo   oo   oo    o   o   ccccccccccccccccccccccccccccccccc");
            levelData.Add("                         x          x                                                           ");
            levels.Add(new Level(6, "SECRET", Color.Magenta, levelData, 20, 300, "audio/secret"));
        }
        public Level CurrentLevel()
        {
            Level level = new Level(levels[currentLevelId]);
            return level;
        }
        public void LoadContent(ContentManager content)
        {
            levelSelectBackgroundImage = content.Load<Texture2D>("images/level_background");
            arrow = content.Load<Texture2D>("images/arrow");
        }

        // Determine whether the arrows should be shown (if the player is on the first level in the list, don't show the up arrow. The inverse also goes)
        // Change the current level if the arrows have been clicked 
        public void Update(GameTime gameTime)
        {
            showUpArrow = showDownArrow = true;
            if (currentLevelId == 0)
                showUpArrow = false;
            int x = 1;
            if (secretLevelPlayed)
                x = 0;
            if (currentLevelId == levels.Count - 1 - x)
                showDownArrow = false;
            MouseStateExtended mouseState = MouseExtended.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            hoverUpArrow = new Rectangle(160, 150, 540, 300).Contains(mousePosition);
            bool upArrowClick = hoverUpArrow && mouseState.WasButtonJustUp(MouseButton.Left);
            hoverDownArrow = new Rectangle(160, 580, 540, 300).Contains(mousePosition);
            bool downArrowClick = hoverDownArrow && mouseState.WasButtonJustUp(MouseButton.Left);
            if (showUpArrow && (upArrowClick || (game.gamePadState.DPad.Up == ButtonState.Pressed && game.prevGamePadState.DPad.Up == ButtonState.Released)))
                currentLevelId -= 1;
            if (showDownArrow && (downArrowClick || (game.gamePadState.DPad.Down == ButtonState.Pressed && game.prevGamePadState.DPad.Down == ButtonState.Released)))
                currentLevelId += 1;
        }

        // Draw the level select box
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Level level = CurrentLevel();
            spriteBatch.Draw(levelSelectBackgroundImage, new Vector2(160, 150), Color.White);
            Vector2 stringSize = game.ArcadePI50.MeasureString(level.name);
            spriteBatch.DrawString(game.ArcadePI50, level.name, new Vector2(430, 513) - stringSize / 2, Color.DarkSlateBlue);
            if (showUpArrow)
            {
                Color color = Color.White;
                // Turn the arrows blue if they are being hovered over
                if (hoverUpArrow)
                    color = Color.SlateBlue;
                spriteBatch.Draw(arrow, new Vector2(330, 250), color);
            }
            if (showDownArrow)
            {
                Color color = Color.White;
                if (hoverDownArrow)
                    color = Color.SlateBlue;
                spriteBatch.Draw(arrow, new Vector2(330, 680), null, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.FlipVertically, 0);
            }
        }
    }
}

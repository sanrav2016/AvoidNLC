// The game state
// This is the actual game; it compiles all game elements and turns them into a playable game
// This is different from Game1 in that Game1 manages everything (the whole app), while GameState only manages the game portion
// GameState also manages the win and lose screens

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System.Collections.Generic;
using Avoid.Sprites;
using System.Threading;
using System;

namespace Avoid.States
{
    class GameState : State
    {

        // variables. many, many variables.
        private Song backgroundMusic, loseSoundEffect, winSoundEffect, introSoundEffect;
        private Texture2D arrows;
        private Texture2D spaceBar;
        private Texture2D backgroundImage;
        private Texture2D bigCircle, middleCircle, smallCircle;
        private Button menuButton;
        private Button tryAgainButton;
        private Texture2D voidCircle;
        private Vector2 voidCirclePosition = new Vector2(710, 290);
        private HealthBar healthBar;
        private BlastBar blastBar;
        private Avoid.Sprites.Void gameVoid;
        private ObstacleManager obstacleManager;
        private bool gameLost = false;
        private bool gameWon = false;
        private List<string> levelData = new List<string>();
        private float voidCircleAngle = 0;
        private int counter = 0;
        private int delay;
        private int timeFactor;
        private Color color;
        private bool turnOffShieldPending = false;
        private bool hurtByVoid = false;
        private bool slept = false;
        private string levelName;
        private bool shakeViewport = false;
        private float shakeStartAngle = 0;
        private float shakeRadius = 10;
        Random rand = new Random();
        private GameTime shakeStart;
        private bool introInProgress;
        private float gameStartTime = -1;
        private float outroOffset = 0;
        private Level origLevel;
        private bool musicPlaying = false, winMusicPlaying = false, introMusicPlaying = false;
        // Consolation and celebratory messages for if the player loses or wins a level. 
        private string[] consolationMessages =
        {
            "The void compels you.", "Mastery requires patience.",
            "Do not fret, not just yet.", "You were supposed to avoid it.",
            "Click to play again. You know you want to.",
            "Keep calm and carry on.",
            "So close, yet so far.",
            "*generic consolation message*" 
        };
        private string[] celebratoryMessages =
        {
            "The void compels you.",
            "Another one for the books.",
            "A well deserved victory.",
            "Nice job, you avoided it.",
            "Click Menu to play again. You know you want to.",
            "Don't let it get to your head.",
            "*generic celebratory message*"
        };
        private string celebratoryMessage, consolationMessage;
        public GameState(Game1 game, ContentManager content) : base(game, content) {
            health = 5;
            score = 0;
            blast = 0;
            shieldOn = false;
            introInProgress = true;
            origLevel = game.levelManager.levels[game.levelManager.currentLevelId];
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (gameWon || gameLost)
                origLevel.ResetHighScore((int)score);
            Vector2 offset = Vector2.Zero;
            // Shake if the void is on
            if (shakeViewport)
            {
                offset = new Vector2((float)(Math.Sin(shakeStartAngle) * shakeRadius), (float)(Math.Cos(shakeStartAngle) * shakeRadius));
                shakeRadius -= 0.25f;
                shakeStartAngle += (150 + rand.Next(60));
                if (gameTime.TotalGameTime.TotalSeconds - shakeStart.TotalGameTime.TotalSeconds > 1 || shakeRadius <= 0)
                {
                    shakeViewport = false;
                    shakeStart = null;
                    shakeStartAngle = 0;
                    shakeRadius = 10;
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(offset.X, offset.Y, 0));
            }
            // Draw game elements
            if (!gameLost)
            {
                if (!introInProgress)
                {
                    // Do NOT draw game elements if the game has been won, except for a select few
                    if (gameWon) {
                        outroOffset -= 10;
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(outroOffset, 0, 0));
                    }
                    int v = (int)(60 * Math.Abs((float)Math.Sin((float)((gameTime.TotalGameTime.TotalMilliseconds - gameStartTime) / 1000))));
                    if (gameVoid.on) v = 150;
                    spriteBatch.Draw(backgroundImage, Vector2.Zero, new Color(v, v, v));
                    Vector2 stringSize = game.ArcadePI75.MeasureString(((int)score).ToString());
                    spriteBatch.DrawString(game.ArcadePI75, ((int)score).ToString(), new Vector2(1880, 150) - stringSize, Color.Yellow);
                    spriteBatch.Draw(smallCircle, new Vector2(960, 1080) - new Vector2(smallCircle.Width, smallCircle.Height) / 2, Color.Gray);
                    spriteBatch.Draw(middleCircle, new Vector2(960, 1080) - new Vector2(middleCircle.Width, middleCircle.Height) / 2, Color.Gray);
                    spriteBatch.Draw(bigCircle, new Vector2(960, 1080) - new Vector2(bigCircle.Width, bigCircle.Height) / 2, Color.Gray);
                    if (gameVoid.on) gameVoid.Draw(spriteBatch);
                    healthBar.Draw(spriteBatch);
                    obstacleManager.Draw(gameTime, spriteBatch);
                    if (gameWon)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin();
                        spriteBatch.Draw(backgroundImage, Vector2.Zero, new Color(v, v, v));
                    }
                    player.Draw(gameTime, spriteBatch);
                    // Draw images used in the demo level. These show the player the controls and how the game works.
                    if (origLevel.id == 0)
                    {
                        float timeSpan = (float)gameTime.TotalGameTime.TotalMilliseconds - gameStartTime;
                        float l = 0.1f * (float)Math.Sin((float)timeSpan / 1000) + 0.7f;
                        if (timeSpan > 1000 && timeSpan < 8000)
                            spriteBatch.Draw(arrows, new Vector2(960, 700) - (l * new Vector2(800, 400)) / 2, null, Color.White, 0, Vector2.Zero, new Vector2(l, l), SpriteEffects.None, 0f);
                        if (timeSpan > 48000 && timeSpan < 60000 && !gameWon)
                        {
                            spriteBatch.Draw(spaceBar, new Vector2(960, 700) - (l * new Vector2(800, 200)) / 2, null, Color.White, 0, Vector2.Zero, new Vector2(l, l), SpriteEffects.None, 0f);
                        }
                    }
                } else
                {
                    Vector2 stringSize = game.ArcadePI75.MeasureString(levelName);
                    spriteBatch.DrawString(game.ArcadePI75, levelName, new Vector2(960, 150) - stringSize / 2, color);
                }
                if (gameWon)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(outroOffset, 0, 0));
                }
                voidCircleAngle -= (float)(Math.PI / timeFactor / 6);
                spriteBatch.Draw(voidCircle, voidCirclePosition + new Vector2(500 / 2, 500 / 2), null, color, voidCircleAngle, new Vector2(500 / 2, 500 / 2), Vector2.One, SpriteEffects.None, 0f);
                if (gameWon)
                {
                    // Win screen
                    spriteBatch.Draw(smallCircle, new Vector2(960, 1080) - new Vector2(smallCircle.Width, smallCircle.Height) / 2, Color.Gray);
                    spriteBatch.Draw(middleCircle, new Vector2(960, 1080) - new Vector2(middleCircle.Width, middleCircle.Height) / 2, Color.Gray);
                    spriteBatch.Draw(bigCircle, new Vector2(960, 1080) - new Vector2(bigCircle.Width, bigCircle.Height) / 2, Color.Gray);
                    spriteBatch.End();
                    spriteBatch.Begin();
                    Vector2 stringSize = game.ArcadePI75.MeasureString("Level Complete");
                    spriteBatch.DrawString(game.ArcadePI75, "Level Complete", new Vector2(960, 150) - stringSize / 2, Color.Green);
                    stringSize = game.ArcadePI75.MeasureString("SCORE: " + ((int)score).ToString());
                    spriteBatch.DrawString(game.ArcadePI75, "SCORE: " + ((int)score).ToString(), new Vector2(960, 780) - stringSize / 2, Color.Green);
                    stringSize = game.ArcadePI75.MeasureString("HIGH SCORE: " + origLevel.highScore.ToString());
                    spriteBatch.DrawString(game.ArcadePI75, "HIGH SCORE: " + origLevel.highScore.ToString(), new Vector2(960, 930) - stringSize / 2, Color.Yellow);
                    stringSize = game.ArcadePI30.MeasureString(celebratoryMessage);
                    spriteBatch.DrawString(game.ArcadePI30, celebratoryMessage, new Vector2(960, 670) - stringSize / 2, Color.Gray);
                }
                if (!introInProgress && !gameWon)
                {
                    blastBar.Draw(spriteBatch);
                }
            } else
            {
                if (!slept)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    slept = true;
                    MediaPlayer.Stop();
                    Thread.Sleep(1000);
                    MediaPlayer.Play(loseSoundEffect);
                    MediaPlayer.IsRepeating = false;
                }
                // Lose screen
                Vector2 stringSize = game.ArcadePI75.MeasureString("Game Over");
                spriteBatch.DrawString(game.ArcadePI75, "Game Over", new Vector2(960, 200) - stringSize / 2, Color.Red);
                stringSize = game.ArcadePI75.MeasureString("SCORE: " + ((int)score).ToString());
                spriteBatch.DrawString(game.ArcadePI75, "SCORE: " + ((int)score).ToString(), new Vector2(960, 340) - stringSize / 2, Color.Red);
                stringSize = game.ArcadePI75.MeasureString("HIGH SCORE: " + origLevel.highScore.ToString());
                spriteBatch.DrawString(game.ArcadePI75, "HIGH SCORE: " + origLevel.highScore.ToString(), new Vector2(960, 490) - stringSize / 2, Color.Yellow);
                stringSize = game.ArcadePI30.MeasureString(consolationMessage);
                spriteBatch.DrawString(game.ArcadePI30, consolationMessage, new Vector2(960, 690) - stringSize / 2, Color.Gray);
                tryAgainButton.Draw(spriteBatch);
            }
            // Always keep the menu button at the top left
            menuButton.Draw(spriteBatch);
        }

        // Load all necessary elements
        // This function draws heavily from the files in the Sprites folder
        public override void LoadContent()
        {
            Level level = game.levelManager.CurrentLevel();
            levelName = level.name;
            color = level.color;
            levelData = level.levelData;
            delay = level.delay;
            timeFactor = level.timeFactor;
            backgroundImage = content.Load<Texture2D>("images/background");
            menuButton = new Button(game, "Menu", new Vector2(50, 50), true);
            menuButton.LoadContent(content);
            tryAgainButton = new Button(game, "Try Again", new Vector2(560, 750));
            tryAgainButton.LoadContent(content);
            List<string> levelDataCopy = new List<string>(levelData);
            player = new Player(graphics_device);
            player.LoadContent(content);
            healthBar = new HealthBar(graphics_device);
            healthBar.LoadContent(content);
            blastBar = new BlastBar(graphics_device);
            blastBar.LoadContent(content);
            gameVoid = new Avoid.Sprites.Void(graphics_device, levelDataCopy, this, delay, color);
            gameVoid.LoadContent(content);
            obstacleManager = new ObstacleManager(graphics_device, player, this, levelDataCopy, delay, timeFactor);
            obstacleManager.LoadContent(content);
            voidCircle = content.Load<Texture2D>("images/void_circle");
            bigCircle = content.Load<Texture2D>("images/big_circle");
            middleCircle = content.Load<Texture2D>("images/middle_circle");
            smallCircle = content.Load<Texture2D>("images/small_circle");
            arrows = content.Load<Texture2D>("images/up_down_arrows");
            spaceBar = content.Load<Texture2D>("images/space_bar");
            celebratoryMessage = celebratoryMessages[rand.Next(celebratoryMessages.Length)];
            // Hint at how to get to the secret level if the player has completed the fifth level
            if (origLevel.id == 5)
                celebratoryMessage = "Say, have you ever heard of the Konami Code?";
            if (origLevel.id == 6)
                celebratoryMessage = "Might it have been a pyrrhic victory?";
            consolationMessage = consolationMessages[rand.Next(consolationMessages.Length)];
            // Load audio
            // These audio are Songs, which play using the system's own media player
            backgroundMusic = content.Load<Song>(origLevel.musicFileName);
            loseSoundEffect = content.Load<Song>("audio/lose");
            winSoundEffect = content.Load<Song>("audio/win");
            introSoundEffect = content.Load<Song>("audio/level_intro");
        }

        // Update the game
        public override void Update(GameTime gameTime)
        {
            // Go back to menu if the user has requested it
            if (menuButton.Clicked() || game.gamePadState.IsButtonDown(Buttons.Back))
            {
                game.ChangeState(game.Menu());
            }
            // Game intro
            if (gameStartTime == -1) gameStartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            if (introInProgress)
            {
                if (!introMusicPlaying)
                {
                    MediaPlayer.Play(introSoundEffect, new TimeSpan());
                    MediaPlayer.IsRepeating = false;
                    introMusicPlaying = true;
                }
                voidCirclePosition.Y = 290 + (float)Math.Pow(2, (gameTime.TotalGameTime.TotalMilliseconds - gameStartTime) / 200);
                if (voidCirclePosition.Y >= 830)
                {
                    introInProgress = false;
                    gameStartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                }
                return;
            }
            if (game.gamePadState.IsConnected)
            {
                tryAgainButton.text = "Try Again (A)";
            }
            if (!introInProgress && !musicPlaying)
            {
                MediaPlayer.Play(backgroundMusic, new TimeSpan());
                MediaPlayer.IsRepeating = true;
                musicPlaying = true;
            }
            // Determine if the player is still playing, has won, or has lost
            if (health > 0)
            {
                if (levelData[0].Length == 0)
                {
                    if (obstacleManager.obstacles.Count == 0)
                    {
                        gameWon = true;
                    }
                } else
                {
                    counter += 1;
                    if (counter == delay)
                    {
                        levelData[0] = levelData[0].Substring(1);
                        counter = 0;
                    }
                }
            } else
            {
                gameLost = true;
            }
            // If the game is still going, handle the shield and the void
            // The shield should turn on when it collides with a shield icon
            // It should remain until the end of the next void
            if (!gameLost && !gameWon)
            {
                if (shieldOn)
                {
                    if (gameVoid.on)
                    {
                        turnOffShieldPending = true;
                    }
                    else
                    {
                        if (turnOffShieldPending)
                        {
                            turnOffShieldPending = false;
                            shieldOn = false;
                            player.shieldBubble = false;
                        }
                    }
                }
                else
                {
                    if (gameVoid.on)
                    {
                        if (!hurtByVoid)
                        {
                            health -= 1;
                            hurtByVoid = true;
                        }
                    }
                    else
                    {
                        if (hurtByVoid)
                            hurtByVoid = false;
                    }
                }
                if (gameVoid.on && !shakeViewport)
                {
                    shakeViewport = true;
                    shakeStart = gameTime;
                }
                // Handle blasts
                bool blastHappened = false;
                KeyboardStateExtended keyState = KeyboardExtended.GetState();
                if (keyState.WasKeyJustDown(Keys.Space) || (game.gamePadState.Buttons.B == ButtonState.Pressed && game.prevGamePadState.Buttons.B == ButtonState.Released))
                {
                    if (blast > 0)
                    {
                        blast -= 1;
                        blastHappened = true;
                    }
                }
                score += 0.1f;
                player.Update(gameTime, keyState, game.gamePadState);
                obstacleManager.Update(gameTime, blastHappened);
                healthBar.Update(health);
                blastBar.Update(blast);
                gameVoid.Update();
            } else
            {
                shieldOn = false;
                player.shieldBubble = false;
            }
            // If the game has been won, play the win music
            if (gameWon)
            {
                if (!winMusicPlaying)
                {
                    winMusicPlaying = true;
                    MediaPlayer.Play(winSoundEffect);
                    MediaPlayer.IsRepeating = false;
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                }
                player.playerSpriteSheet.UpdateFrame();
            }
            // If the game has been lost, add functionality to the try again button
            if (gameLost)
            {
                if (tryAgainButton.Clicked() || game.gamePadState.IsButtonDown(Buttons.A))
                {
                    game.ChangeState(game.GameState());
                }
            }
        }
    }
}

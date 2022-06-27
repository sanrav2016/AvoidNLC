// This is the primary program. It connects to all the subcomponents of the project and comprises everything into a playable application.
// I have broken the game up into many different files that all reference each other.
// We could technically create the whole game in this file, but that would be tedious and hard to manage.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Avoid.States;
using System.IO;
using System;

namespace Avoid
{
    public class Game1 : Game
    {

        // Everytime the game boots up, a player ID is assigned based on previous history of players. 
        public int playerId;
        public bool leaderboardKeyed = false;

        // The game will update its logic at 60 frames per second, even if it doesn't draw at that rate due to system limitations.
        // Ideally, both update and draw rate will be at 60 fps.
        // The game is 1920x1080, but it will be scaled to fit the device's screen size.
        private float FPS = 60f;
        private int width = 1920;
        private int height = 1080;

        // Important global variables that are used all throughout the game
        public GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public KeyboardState keyState;
        private KeyboardState prevKeyState;
        public GamePadState gamePadState;
        public GamePadState prevGamePadState;

        // The Konami Code, a hallmark of arcade games! These variables help keep track of whether the user has pressed it
        // There are two collections of variables here: one for the keyboard and one for the gamepad (Xbox controller). 
        private List<Keys> konamiCodeKeyboard = new List<Keys>();
        private List<Buttons> konamiCodeGamePad = new List<Buttons>();
        private List<Buttons> konamiCodeButtonList = new List<Buttons>();
        private List<Keys> pressedKeys = new List<Keys>();
        private List<Buttons> pressedButtons = new List<Buttons>();
        private int currentKonamiIndex = 0;

        public State currentState;
        public State nextState;

        // Game state constructors
        // Each game state is a different page or "slide" of the game; for example, the menu, the leaderboard, or the actual game
        public LevelManager levelManager;
        public State GameState()
        {
            return new GameState(this, Content);
        }
        public State Menu()
        {
            return new Menu(this, Content);
        }
        public State Instructions()
        {
            return new Instructions(this, Content);
        }
        public State Leaderboard()
        {
            return new Leaderboard(this, Content);
        }

        // Fonts are built using SpriteFont variables
        // The font used in Avoid is ArcadePix
        public SpriteFont ArcadePI30;
        public SpriteFont ArcadePI45;
        public SpriteFont ArcadePI50;
        public SpriteFont ArcadePI75;

        // Constructor
        public Game1() { 
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        // Initialize is the first function called in this program, after the constructor of course
        protected override void Initialize()
        {
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / FPS);
            // Fixed timestep ensures steady updating, even if drawing falls behind
            IsFixedTimeStep = true;
            base.Initialize();
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            // Full screen for a more immersive experience!
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }

        // LoadContent gathers the necessary files using a ContentManager and initializes them to the proper variables
        // This makes it possible to draw them and use them in logic
        protected override void LoadContent()
        {
            // All things to draw to the screen are loaded into what is known as a SpriteBatch
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ArcadePI30 = Content.Load<SpriteFont>("fonts/ArcadePI30");
            ArcadePI45 = Content.Load<SpriteFont>("fonts/ArcadePI45");
            ArcadePI50 = Content.Load<SpriteFont>("fonts/ArcadePI50");
            ArcadePI75 = Content.Load<SpriteFont>("fonts/ArcadePI75");
            levelManager = new LevelManager(this);
            currentState = Menu();
            currentState.LoadContent();
            nextState = null;
            // Assign player ID based on previous players in the leaderboard
            // The leaderboard is a simple text file to save space; nevertheless, it can hold all the information we need it to
            string leaderboardText = File.ReadAllText("leaderboard.txt");
            string[] string_scores = leaderboardText.Split(',');
            playerId = 1;
            foreach (string s in string_scores)
            {
                if (!string.IsNullOrEmpty(s))
                    playerId += 1;
            }
            konamiCodeKeyboard.AddRange(new List<Keys> { Keys.Up, Keys.Up, Keys.Down, Keys.Down, Keys.Left, Keys.Right, Keys.Left, Keys.Right, Keys.B, Keys.A });
            konamiCodeGamePad.AddRange(new List<Buttons> { Buttons.DPadUp, Buttons.DPadUp, Buttons.DPadDown, Buttons.DPadDown, Buttons.DPadLeft, Buttons.DPadRight, Buttons.DPadLeft, Buttons.DPadRight, Buttons.B, Buttons.A });
            konamiCodeButtonList.AddRange(new List<Buttons> { Buttons.DPadDown, Buttons.DPadLeft, Buttons.DPadRight, Buttons.DPadUp, Buttons.A, Buttons.B });
        }

        // Update is called continuously to the rate of the FPS, or in this case, 60 times a second.
        // It updates the game's logic, such as handling input, changing sprite positions, etc.
        protected override void Update(GameTime gameTime)
        {
            // If escape is pressed, exit the game
            keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Escape))
                Exit();
            if (prevKeyState.IsKeyDown(Keys.LeftAlt) && keyState.IsKeyUp(Keys.Enter) && prevKeyState.IsKeyDown(Keys.Enter))
                graphics.ToggleFullScreen();
            // Check if gamepad is connected
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);
            if (capabilities.IsConnected)
            {
                GamePadState state = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
                if (capabilities.GamePadType == GamePadType.GamePad)
                {
                    gamePadState = state;
                }
            }
                if (nextState != null)
            {
                currentState = nextState;
                currentState.LoadContent();
                nextState = null;
            }
            currentState.Update(gameTime);
            // Validate whether the Konami Code is being or has been pressed
            // If it has, open the secret level, which is not normally visible
            if (gamePadState.IsConnected)
            {
                foreach (Buttons button in konamiCodeButtonList)
                {
                    if (gamePadState.IsButtonDown(button) && prevGamePadState.IsButtonUp(button))
                        pressedButtons.Add(button);
                }
                foreach (Buttons button in pressedButtons)
                {
                    if (button == konamiCodeGamePad[currentKonamiIndex])
                        currentKonamiIndex += 1;
                    else
                        currentKonamiIndex = 0;
                    if (currentKonamiIndex == konamiCodeGamePad.Count)
                    {
                        currentKonamiIndex = 0;
                        pressedButtons = new List<Buttons>();
                        levelManager.currentLevelId = 6;
                        levelManager.secretLevelPlayed = true;
                        ChangeState(new GameState(this, Content));
                    }
                }
            } else
            {
                foreach (Keys key in keyState.GetPressedKeys())
                {
                    if (prevKeyState.IsKeyUp(key))
                        pressedKeys.Add(key);
                }
                foreach (Keys key in pressedKeys)
                {
                    if (key == konamiCodeKeyboard[currentKonamiIndex])
                        currentKonamiIndex += 1;
                    else
                        currentKonamiIndex = 0;
                    if (currentKonamiIndex == konamiCodeKeyboard.Count)
                    {
                        currentKonamiIndex = 0;
                        pressedKeys = new List<Keys>();
                        levelManager.currentLevelId = 6;
                        levelManager.secretLevelPlayed = true;
                        ChangeState(new GameState(this, Content));
                    }
                }
            }
            base.Update(gameTime);
            prevKeyState = keyState;
            prevGamePadState = gamePadState;
        }

        // ChangeState is called when switching between game pages
        public void ChangeState(State state)
        {
            nextState = state;
        }

        // Draw is called continuously, similar to Update.
        // Ideally, it should be called just after Update every time Update runs, but if the system is slow, it may lag.
        // Again, this is why we implemented Fixed Time Step above. It ensures that even if there is lag, the game will update and run at the right speed.
        // As for Update itself lagging, there are other measures implemented in the game to deal with that.
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            // Draw the contents of the current state to the sprite batch
            currentState.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

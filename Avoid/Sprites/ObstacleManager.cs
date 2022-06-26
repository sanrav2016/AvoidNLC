// Implementing a way to create, draw, and remove obstacles

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Avoid.States;
using System;

namespace Avoid.Sprites
{
    class ObstacleManager
    {
        // An obstacle class is not enough; we also need an obstacle manager!

        private Player player;
        private State GameState;
        private Texture2D heartImage;
        private Texture2D shieldImage;
        private Texture2D coinImage;
        private Texture2D blastImage;
        private Texture2D obstacleImage;
        private GraphicsDevice graphics_device;
        // This list will be initialized with the level's obstacle data, provided as an array of strings
        private List<String> levelData = new List<String>();
        // This list will contain obstacles to be rendered on screen
        // When an obstacle is to enter the screen, it will be added to this list
        // When an obstacle exits the screen, it will be removed from this list
        // In this way, obstacles off screen are not drawn and do not waste processing power
        public List<Obstacle> obstacles = new List<Obstacle>();
        private int delay;
        private int counter = 0;
        private int timeFactor;
        public ObstacleManager(GraphicsDevice _graphics_device, Player _player, GameState _gameState, List<string> _levelData, int _delay, int _timeFactor)
        {
            graphics_device = _graphics_device;
            player = _player;
            GameState = _gameState;
            levelData = _levelData;
            delay = _delay;
            timeFactor = _timeFactor;
        }

        // Load the images for all the obstacles
        public void LoadContent(ContentManager content)
        {
            heartImage = content.Load<Texture2D>("images/heart");
            shieldImage = content.Load<Texture2D>("images/shield");
            coinImage = content.Load<Texture2D>("images/coin");
            blastImage = content.Load<Texture2D>("images/blast");
            obstacleImage = content.Load<Texture2D>("images/obstacle");
        }
        public void Update(GameTime gameTime, bool blastHappened)
        {
            // Obstacles that are to be deleted will be added to this list
            List<Obstacle> toBeRemoved = new List<Obstacle>();
            // Loop through every obstacle and see what to do with it
            foreach (Obstacle obstacle in obstacles)
            {
                float t = -(float)(gameTime.TotalGameTime.TotalMilliseconds - obstacle.startingTime) / timeFactor;
                int radius = 0;
                // Figure out where to draw the obstacle based on whether it is in the outer, middle or inner ring or orbit around the void
                switch (obstacle.level)
                {
                    case 0:
                        radius = 780;
                        break;
                    case 1:
                        radius = 630;
                        break;
                    case 2:
                        radius = 480;
                        break;
                }
                obstacle.position = new Vector2((float)(960 + radius * Math.Cos(t)), (float)(1080 + radius * Math.Sin(t)));
                // Implementing collision logic, with inspiration from W3schools
                Vector2 playerPos = player.playerPosition + player.scale * new Vector2(310, 160);
                float playerLeft = playerPos.X;
                float playerRight = playerPos.X + (280 * player.scale);
                float playerTop = playerPos.Y;
                float playerBottom = playerPos.Y + (246 * player.scale);
                float obstacleLeft = obstacle.position.X;
                float obstacleRight = obstacle.position.X + 125;
                float obstacleTop = obstacle.position.Y;
                float obstacleBottom = obstacle.position.Y + 125;
                bool crash = true;
                if ((playerBottom < obstacleTop) || (playerTop > obstacleBottom) || (playerRight < obstacleLeft) || (playerLeft > obstacleRight))
                    crash = false;
                // If crash, remove obstacle and have it take effect
                if (crash)
                {
                    toBeRemoved.Add(obstacle);
                    obstacle.TakeEffect(GameState);
                }
                // If obstacle exits screen, remove
                if (obstacle.position.Y > 1200)
                    toBeRemoved.Add(obstacle);
                // If obstacle is blasted and is on screen, remove
                if (obstacle.type == 'o' && blastHappened && new Rectangle(0, 0, 1920, 1080).Contains(obstacle.position))
                    toBeRemoved.Add(obstacle);
            }
            foreach (Obstacle obstacle in toBeRemoved)
            {
                obstacles.RemoveAll(p => p == obstacle);
            }
            // Create new obstacles based on the level data
            if (levelData[0].Length > 0)
            {
                counter += 1;
                if (counter == delay) {
                    char top = levelData[0][0];
                    char middle = levelData[1][0];
                    char bottom = levelData[2][0];
                    ValidateCharacter(top, gameTime, 0);
                    ValidateCharacter(middle, gameTime, 1);
                    ValidateCharacter(bottom, gameTime, 2);
                    levelData[0] = levelData[0].Substring(1);
                    levelData[1] = levelData[1].Substring(1);
                    levelData[2] = levelData[2].Substring(1);
                    counter = 0;
                }
            }
        }

        // The function which creates new obstacles
        // In the level data strings, obstacles are represented by the first letters of their names (e.g. heart -> h)
        // If there is a space, nothing will be drawn
        public void ValidateCharacter(char c, GameTime gameTime, int level)
        {
            Vector2 position;
            int radius = 0;
            switch (level)
            {
                case 0:
                    radius = 780;
                    break;
                case 1:
                    radius = 630;
                    break;
                case 2:
                    radius = 480;
                    break;
            }
            position = new Vector2((float)(960 + radius), 1080f);
            if (c == 'h' || c == 's' || c == 'c' || c == 'o' || c == 'b') 
                obstacles.Add(new Obstacle(c, position, level, gameTime.TotalGameTime.TotalMilliseconds));

        }

        // Draw the obstacles
        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            foreach (Obstacle obstacle in obstacles) { 
                float t = (float)Math.PI / 2 + -(float)(gameTime.TotalGameTime.TotalMilliseconds - obstacle.startingTime) / timeFactor;
                Texture2D obstacleImageToDraw = heartImage;
                // Give the illusion of the obstacle rotating by scaling it periodically
                Vector2 scaleVector = new Vector2(Math.Abs((float)Math.Sin((gameTime.TotalGameTime.TotalMilliseconds) / 1000)), 1);
                switch (obstacle.type)
                {
                    case 'h':
                        obstacleImageToDraw = heartImage;
                        break;
                    case 's':
                        obstacleImageToDraw = shieldImage;
                        break;
                    case 'c':
                        obstacleImageToDraw = coinImage;
                        break;
                    case 'b':
                        obstacleImageToDraw = blastImage;
                        break;
                    case 'o':
                        obstacleImageToDraw = obstacleImage;
                        scaleVector = Vector2.One;
                        break;
                }
                _spriteBatch.Draw(obstacleImageToDraw, obstacle.position, null, Color.White, t, new Vector2(62, 62), scaleVector, SpriteEffects.None, 0f);
            }
        }
    }
}
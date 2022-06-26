// Leaderboard state
// What's an arcade game without a leaderboard? 
// Leaderboard data is stored in a text file as a list of scores
// The scores are ordered by player. The first person to play is player 1, the second is player 2, etc.
// A player's score will only be stored if they open the leaderboard. This prevents those who open the game but do not play from taking up an empty spot

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Avoid.Sprites;
using System.IO;

namespace Avoid.States
{
    class Leaderboard : State
    {
        private List<int[]> scores = new List<int[]>();
        private Texture2D backgroundImage;
        private Button menuButton;
        private int playerId;
        private float leaderboardStartTime = -1;
        private bool playerScoreRegistered = false;
        private int playerScore = 0;
        public Leaderboard(Game1 game, ContentManager content) : base(game, content)
        {
            playerId = game.playerId;
            if (!game.leaderboardKeyed)
            {
                File.AppendAllText("leaderboard.txt", "0,");
                game.leaderboardKeyed = true;
            }
        }

        // Draw the top 5 players as well as the current player
        // Include rank number, player ID and score
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundImage, Vector2.Zero, new Color(90, 90, 90));
            menuButton.Draw(spriteBatch);
            Vector2 stringSize = game.ArcadePI75.MeasureString("Leaderboard");
            spriteBatch.DrawString(game.ArcadePI75, "Leaderboard", new Vector2(960, 200) - stringSize / 2, Color.White);
            int count = 0;
            foreach (int[] scoreArray in scores)
            {
                if (scoreArray[2] <= 5 || scoreArray[0] == playerId) {
                    int player = scoreArray[0], score = scoreArray[1], rank = scoreArray[2];
                    if (count == 5)
                    {
                        spriteBatch.DrawString(game.ArcadePI50, "---", new Vector2(900, 300 + count * 75), Color.White);
                        count += 1;
                    }
                    string text = "Player " + player.ToString();
                    Color color = Color.White;
                    // Designate the current player's score info using yellow
                    if (player == playerId)
                    {
                        text += " (You)";
                        color = Color.Yellow;
                    }
                    Vector2 rankSize = game.ArcadePI50.MeasureString(rank.ToString());
                    spriteBatch.DrawString(game.ArcadePI50, rank.ToString(), new Vector2(480 - rankSize.X, 300 + count * 75), color);
                    spriteBatch.DrawString(game.ArcadePI50, text, new Vector2(540, 300 + count * 75), color);
                    spriteBatch.DrawString(game.ArcadePI50, score.ToString(), new Vector2(1300, 300 + count * 75), color);
                    count += 1;
                }
            }
        }
        public override void LoadContent()
        {
            backgroundImage = content.Load<Texture2D>("images/background2");
            menuButton = new Button(game, "Menu", new Vector2(50, 50), true);
            menuButton.LoadContent(content);
        }

        // Read the leaderboard data from the text file
        // Update it with the current player's score 
        // Make sure to only do this once or there will be unnecessary and continous calls to open the text file
        public override void Update(GameTime gameTime)
        {
            if (leaderboardStartTime == -1)
                leaderboardStartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            if (menuButton.Clicked() || game.gamePadState.IsButtonDown(Buttons.Back))
            {
                game.ChangeState(game.Menu());
                game.nextState.backgroundMusicPlaying = true;
            }
            if (!playerScoreRegistered)
            {
                foreach (Level level in game.levelManager.levels)
                {
                    if (level.id != 0)
                        playerScore += level.highScore;
                }
                int playerCounter = 1;
                string leaderboardText = File.ReadAllText("leaderboard.txt");
                string[] string_scores = leaderboardText.Split(',');
                string_scores[playerId - 1] = playerScore.ToString();
                string stringToWrite = "";
                foreach (string s in string_scores)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        stringToWrite += int.Parse(s) + ",";
                        int[] scoreArray = { 0, 0, 0 };
                        scoreArray[0] = playerCounter;
                        scoreArray[1] = int.Parse(s);
                        scores.Add(scoreArray);
                        playerCounter += 1;
                    }
                }
                File.WriteAllText("leaderboard.txt", stringToWrite);
                scores.Sort((x, y) => y[1].CompareTo(x[1]));
                int rank = 1;
                foreach (int[] s in scores)
                {
                    s[2] = rank;
                    rank += 1;
                }
                playerScoreRegistered = true;
            }
        }
    }
}

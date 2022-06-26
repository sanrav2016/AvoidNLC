// Implement levels
// This class is used to represent a level

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Avoid.States
{
    public class Level
    {
        public int id;
        public string name;
        public Color color;
        // Level data is the most important; it tells the game how to render obstacles and when to activate the void
        public List<string> levelData;
        // Delay and time factor set how fast the obstacles come onto screen
        // The faster the level, the harder the level!
        public int delay;
        public int timeFactor;
        public int highScore = 0;
        public string musicFileName;
        public Level(int _id, string _name, Color _color, List<string> _data, int _delay, int _time, string _musicFileName)
        {
            id = _id;
            name = _name;
            color = _color;
            levelData = _data;
            delay = _delay;
            timeFactor = _time;
            musicFileName = _musicFileName;
        }

        // Second constructor to create a copy of a level
        // This is to prevent altering the original level data, since data will need be altered
        public Level(Level level)
        {
            id = level.id;
            name = level.name;
            color = level.color;
            levelData = new List<string>(level.levelData);
            delay = level.delay;
            timeFactor = level.timeFactor;
            musicFileName = level.musicFileName;
        }

        // Reset the high score 
        public void ResetHighScore(int score)
        {
            if (score > highScore)
                highScore = score;
        }
    }
}

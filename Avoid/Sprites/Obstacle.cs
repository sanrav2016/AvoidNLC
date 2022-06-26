// Implementing obstacles

using Microsoft.Xna.Framework;
using Avoid.States;

namespace Avoid.Sprites
{
    class Obstacle
    {
        public char type;
        public Vector2 position;
        public int level;
        public float startingTime;

        // This class is used to represent a single in-game item, such as an obstacle, heart, coin, shield, or blast
        // It does NOT only represent "bad" obstacles, contrary to the filename
        // For namesake, all of these items (obstacle, heart, coin, shield, blast) will be referred to as obstacles
        public Obstacle(char _type, Vector2 _position, int _level, double _startingTime)
        {
            type = _type;
            position = _position;
            level = _level;
            // It is important to keep track of when the obstacle was created, as it will help render rotation correctly
            startingTime = (float)_startingTime;
        }

        // TakeEffect is called when the obstacle is collided with. 
        // The obstacles edit the GameState directly
        public void TakeEffect(State game)
        {
            switch (type)
            {
                case 'h':
                    if (game.health < 5) {
                        game.health += 1;
                    }
                    break;
                case 'c':
                    game.score += 100;
                    break;
                case 's':
                    game.shieldOn = true;
                    game.player.shieldBubble = true;
                    break;
                case 'o':
                    if (!game.shieldOn)
                        game.health -= 1;
                    break;
                case 'b':
                    game.blast = 3;
                    break;
            }
        }
     }
}

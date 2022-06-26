// Implementing spritesheets
// Spritesheets are useful when animating characters or game elements
// A spritesheet is an image that contains every frame of an animation
// Such images can get quite large (for instance, the player spritesheet is almost 5000px in width and height)
// However, they are still more efficient and processor-friendly than storing each frame as a separate image

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avoid.Sprites
{
    public class SpriteSheet
    {
        private Texture2D spriteSheet;
        private int singleW, singleH;
        public int frame = -1;
        private int totalFrames;
        private int delay = 3;

        // When creating a spritesheet, we must specify the actual spritesheet image, the number of frames in the sheet, the size of an individual frame, and the delay
        // The delay determines how often new frames are drawn
        // This is necessary since the game FPS is so fast; a delay makes the animation viewable and appreciable
        public SpriteSheet(Texture2D _spritesheet, int _totalFrames, (int, int) _dim, int _delay = 3)
        {
            spriteSheet = _spritesheet;
            totalFrames = _totalFrames * delay - 1;
            (singleW, singleH) = _dim;
            delay = _delay;
        }

        // The spritesheet is a huge image. When drawing, we will only draw a portion of the image that contains the necessary frame
        // This function determines that portion
        public Rectangle FrameRect()
        {
            int _frame = frame;
            _frame /= delay;
            int x = 0, y = 0;
            while (_frame > 0)
            {
                x += singleW;
                if (x >= spriteSheet.Width)
                {
                    x = 0;
                    y += singleH;
                }
                _frame -= 1;
            }
            return new Rectangle(x, y, singleW, singleH);
        }

        // This function, when called, increases the frame
        // If necessary, it loops back to the start
        // The dir parameter allows frame updating to run backwards (puts the animation in reverse)
        public int UpdateFrame(int dir = 1)
        {
            if (frame < 0)
            {
                frame = 0;
            }
            else
            {
                if (dir == 1)
                {
                    if (frame < totalFrames) frame += 1;
                    else frame = 0;
                } else
                {
                    if (frame > 0) frame -= 1;
                    else frame = totalFrames - 1;
                }
            }
            return frame;
        }
    }
}

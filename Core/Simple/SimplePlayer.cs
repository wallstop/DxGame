using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Simple
{
    public class SimplePlayer : SimpleSprite
    {
        private const string playerName_ = "Player";
        private SimpleMap map_;

        private const char PLAYER_CHARACTER = 'Z';

        public SimplePlayer(SimpleMap map)
           : base(playerName_)
        {
            map_ = map;
            position_.X = map.GetPlayerPosition().X * BLOCK_WIDTH;
            position_.Y = map.GetPlayerPosition().Y * BLOCK_WIDTH;
        }

        private void Move(int x, int y)
        {
            float updatedX = position_.X + x;
            float updatedY = position_.Y + y;

            if ((updatedX > 0) && (updatedX < map_.GetMapSize().X))
            {
                position_.X = updatedX;
            }
            else if (updatedX <= 0)
            {
                position_.X = 0;
            }
            else
            {
                position_.X = map_.GetMapSize().X - space_.Width;
            }

            if ((updatedY > 0) && ((updatedY + space_.Height) < map_.GetMapSize().Y))
            {
                position_.Y = updatedY;
            }
            else if (updatedY <= 0)
            {
                position_.Y = 0;
            }
            else
            {
                position_.Y = map_.GetMapSize().Y - space_.Height;
            }
        }

        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Keys [] pressedKeys = keyboardState.GetPressedKeys();
            foreach (Keys key in pressedKeys)
            {
                switch (key)
                {
                case Keys.Left:
                    Move(-4, 0);
                    break;
                case Keys.Right:
                    Move(4, 0);
                    break;
                case Keys.Up:
                    Move(0, -4);
                    break;
                case Keys.Down:
                    Move(0, 4);
                    break;
                }
            }
        }

        public static bool CanCreateFrom(char character)
        {
            return character == PLAYER_CHARACTER;
        }

        public bool Update()
        {
            HandleInput();
            return true;
        }
    }
}

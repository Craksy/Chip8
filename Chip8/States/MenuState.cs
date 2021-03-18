using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Chip8.States 
{
    public class MenuState : State
    {
        public MenuState(Game1 game, GraphicsDevice screen, SpriteBatch spriteBatch) : base(game, screen, spriteBatch)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            screen.Clear(Color.Tomato);
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
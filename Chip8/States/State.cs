using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chip8.States
{
    public abstract class State 
    {
        protected GraphicsDevice screen;
        protected Game1 game;
        protected SpriteBatch spriteBatch;
        
        public abstract void Draw(GameTime gameTime);
        public abstract void Update(GameTime gameTime);
        public State(Game1 game){
            this.game = game;
            this.screen = game.GraphicsDevice;
            this.spriteBatch = game.spriteBatch;
        }
    }
}
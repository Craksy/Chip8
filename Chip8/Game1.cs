using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.IO;

using Chip8.Emulator;
using Chip8.States;

namespace Chip8
{
    public class Game1 : Game
    {
        public SpriteBatch spriteBatch;

        private const int SCREEN_WIDTH = 1024;
        private const int SCREEN_HEIGHT = 512;
        private GraphicsDeviceManager graphics;

        private State currentState;


        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize() {
            // Monogame specific initialization
            base.Initialize();
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();
        }

        protected override void LoadContent() {
            // HACK: initializing first state here so that the content libarary is sure been loaded. 
            spriteBatch = new SpriteBatch(GraphicsDevice);
            currentState = new MenuState(this);
        }

        protected override void Update(GameTime gameTime) {
            currentState.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            currentState.Draw(gameTime);
            base.Draw(gameTime);
        }

        public void ChangeState(State state){
            currentState = state;
        }
    }
}

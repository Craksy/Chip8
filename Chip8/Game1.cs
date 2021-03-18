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
        private const int SCREEN_WIDTH = 1024;
        private const int SCREEN_HEIGHT = 512;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private State nextState;
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
            //Im loading the first state here, as the content library must be available
            //before trying to load sprites etc.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            string pathToRom = @"./ROMS/Tetris [Fran Dachille, 1991].ch8";
            currentState = new EmulatorState(this, GraphicsDevice, spriteBatch, pathToRom);
            nextState = null;
        }

        protected override void Update(GameTime gameTime) {
            if(nextState != null){
                currentState = nextState;
                nextState = null;
            }

            currentState.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            currentState.Draw(gameTime);
            base.Draw(gameTime);
        }

        public void ChangeState(State state){
            nextState = state;
        }
    }
}

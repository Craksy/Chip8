using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Chip8
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D pixelSprite;

        private byte[] screenBuffer; //TODO: read the upper 256 bytes of the emulator RAM instead of having a local variable.
        private int pixelSize;
        private Color screenColor;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void RenderBuffer(byte[] data)
        {
            spriteBatch.Begin();
            for(int i = 0; i < 256; i++) //for every byte in the screen buffer
            {
                byte byt = data[i];
                for (int j = 0; j < 8; j++) //for every bit in that byte
                {
                    if (((byt>>j) & 1) == 1)
                    {
                        //Calculate x and y coordinates based on bit position.
                        int x = i % 8 * 8 * pixelSize + j * pixelSize;
                        int y = i / 8 * pixelSize;
                        spriteBatch.Draw(pixelSprite, new Vector2(x, y), screenColor);
                    }
                }
            }
            spriteBatch.End();
        }

        protected override void Initialize()
        {
            base.Initialize();
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 512;
            graphics.ApplyChanges();
            screenBuffer = new byte[256];
            screenColor = Color.Red;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixelSprite = this.Content.Load<Texture2D>("pixel16");
            pixelSize = 16;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}

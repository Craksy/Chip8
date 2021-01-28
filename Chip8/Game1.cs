﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using Chip8.Emulator;

namespace Chip8
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D pixelSprite;

        private int pixelSize;
        private Color pixelColor;
        private Color screenBackground;
        private Emulator.Emulator emulator; //note to self: don't name classes after their namespace

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
                    if (((byt>>7-j) & 1) == 1)
                    {
                        //Calculate x and y coordinates based on bit position.
                        int x = i % 8 * 8 * pixelSize + j * pixelSize;
                        int y = i / 8 * pixelSize;
                        spriteBatch.Draw(pixelSprite, new Vector2(x, y), pixelColor);
                    }
                }
            }
            spriteBatch.End();
        }

        protected override void Initialize()
        {
            // Monogame specific initialization
            base.Initialize();
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 512;
            graphics.ApplyChanges();
            pixelColor = Color.Green;
            screenBackground = Color.Black;

            // Emulator initialization
            emulator = new Emulator.Emulator();

            //TODO: actually map file locations to some program names, to make it easier to load different 
            //programs when debugging

            //emulator.ReadROMFromFile(@"C:\Users\Amnesia\source\repos\Chip8\Chip8\ROMS\IBM Logo.ch8");
            //emulator.ReadROMFromFile(@"C:\Users\Amnesia\source\repos\Chip8\Chip8\ROMS\Maze [David Winter, 199x].ch8");
            emulator.ReadROMFromFile(@"C:\Users\Amnesia\source\repos\Chip8\Chip8\ROMS\Zero Demo [zeroZshadow, 2007].ch8");

            //__DebugRendering();
        }

        private void __DebugRendering()
        {
            // just a function that writes some random stuff to the framebuffer
            // so i can see that everything works as expected
            byte[] randinit = { 15, 240, 255};
            byte[] randData = {255, 255, 255, 255 ,1, 1, 1, 240 };
            byte[] rand3 = { 3, 192 };
            emulator.ram.Write(randData, 0xFF8);
            emulator.ram.Write(randinit, 0xF00);
            emulator.ram.Write(rand3, 0xF00+8);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // REVIEW: this is kinda silly.
            // It's probably possible to use OpenGL primitives or something instead.
            pixelSprite = this.Content.Load<Texture2D>("pixel16");
            pixelSize = 16; // a pixel is 16 pixels, lol
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            emulator.Clock(gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(screenBackground);
            byte[] frameBuffer = emulator.GetFrameBuffer();
            RenderBuffer(frameBuffer);

            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Chip8.Emulator;
using Microsoft.Xna.Framework.Input;

namespace Chip8.States 
{
    public class EmulatorState : State
    {
        private Texture2D pixelSprite;
        private int pixelSize;
        private Color pixelColor;
        private Color screenBackground;
        private Emulator.Emulator emulator;
        private Keys[] keybindings;
        private bool[] keystates;


        public EmulatorState(Game1 game, string rom) 
        : base(game)
        {
            pixelColor = Color.Green;
            screenBackground = Color.Black;

            //loads a 16x16 white sprite to act as 'pixel'
            pixelSprite = game.Content.Load<Texture2D>("pixel16"); 
            pixelSize = 16;

            //ready input
            keystates = new bool[16];
            //Temporry solution. eventually read from a config file or something
            keybindings = new Keys[] { 
                Keys.OemOpenBrackets, Keys.M, Keys.OemComma, Keys.OemPeriod,
                Keys.J, Keys.K, Keys.L, Keys.U, Keys.I, Keys.O, Keys.N,
                Keys.H, Keys.Y, Keys.Enter, Keys.Space, Keys.P
            };

            //initialize the actual emulator and load fonts
            emulator = new Emulator.Emulator();
            emulator.LoadFontData(@"./font-data.bin");

            emulator.ReadROMFromFile(rom);
        }

        public override void Draw(GameTime gameTime)
        {
            if (emulator.updated) {
                screen.Clear(screenBackground);
                RenderBuffer();
                emulator.updated = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.ChangeState(new MenuState(game));


            emulator.Clock(gameTime.ElapsedGameTime.TotalSeconds, GetKeyStates());
        }

        private void RenderBuffer() {
            byte[] data = emulator.GetFrameBuffer();
            spriteBatch.Begin();
            for (int i = 0; i < 256; i++) {
                byte byt = data[i];
                for (int j = 0; j < 8; j++) {
                    if (((byt >> 7 - j) & 1) == 1) {
                        //Calculate x and y coordinates and draw a pixel if the bit is on.
                        int x = i % 8 * 8 * pixelSize + j * pixelSize;
                        int y = i / 8 * pixelSize;
                        spriteBatch.Draw(pixelSprite, new Vector2(x, y), pixelColor);
                    }
                }
            }
            spriteBatch.End();
        }

        private bool[] GetKeyStates() {
            KeyboardState state = Keyboard.GetState();
            for (int i = 0; i < 16; i++)
                keystates[i] = state.IsKeyDown(keybindings[i]);
            return keystates;
        }
    }
}
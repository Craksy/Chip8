using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Collections.Generic;

using Chip8.States;

namespace Chip8.Components.Menu 
{
    public class Menu : DrawableGameComponent
    {
        public float x {get; set;}
        public float y {get; set;}

        protected string[] menuItems;
        protected string title;
        protected MenuState menuState;
        protected Game1 game;

        private SpriteFont font;
        private SpriteBatch spriteBatch;
        private int currentIndex;
        private Keys[] previousPressedKeys;
        private int previousPressedCount;

        public Menu(Game1 game, MenuState menuState) 
        : base(game) {
            this.menuState = menuState;
            this.game = game;
            spriteBatch = game.spriteBatch;
            font = menuState.font;
            //HACK: intitializing like this so that the enter key wont carry over between menu changes
            previousPressedKeys = new Keys[]{Keys.Enter}; 
            previousPressedCount = 1;
            currentIndex = 0;
            x = 0;
            y = 0;
        }

        public override void Update(GameTime gameTime) {
            //TODO: Possibly make a proper input manager instead of this
            KeyboardState kb = Keyboard.GetState();
            int pressedKeyCount = kb.GetPressedKeyCount();

            if(previousPressedCount != pressedKeyCount){
                IEnumerable<Keys> pressedKeys = kb.GetPressedKeys().Except(previousPressedKeys);
                if(pressedKeys.Contains(Keys.Down))
                    currentIndex++;
                if(pressedKeys.Contains(Keys.Up))
                    currentIndex--;
                if(pressedKeys.Contains(Keys.Enter))
                    OnItemSelected(currentIndex);
                int numItems = menuItems.Length;
                currentIndex = (currentIndex % numItems + numItems) % numItems;

                previousPressedKeys = kb.GetPressedKeys();
                previousPressedCount = kb.GetPressedKeyCount();
            }
            base.Update(gameTime);
        }

        protected virtual void OnItemSelected(int index){ }

        public override void Draw(GameTime gameTime) {
            game.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, title, new Vector2(x, y), Color.Green);
            for(int i=0; i<menuItems.Length; i++){
                spriteBatch.DrawString(
                    font, 
                    menuItems[i], 
                    new Vector2(x, y+80+i*50), 
                    currentIndex == i ? Color.Green : Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
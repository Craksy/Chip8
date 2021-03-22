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
        public string[] menuItems;
        public string title;

        public Action<int> onItemSelected;

        private SpriteFont font;
        protected SpriteBatch spriteBatch;
        protected Game1 game;
        private int currentIndex;

        private Keys[] previousPressedKeys;
        private int previousPressedCount;
        protected MenuState menuState;

        public Menu(Game1 game, MenuState menuState) 
        : base(game) {
            this.menuState = menuState;
            this.spriteBatch = game.spriteBatch;
            this.game = game;
            currentIndex = 0;
            previousPressedKeys = new Keys[]{Keys.Enter};
            previousPressedCount = 1;
            x = 0;
            y = 0;
            font = Game.Content.Load<SpriteFont>("menuFont");
        }

        public override void Update(GameTime gameTime) {
            //TODO: Possibly make a proper event driven input API instead of this
            KeyboardState kb = Keyboard.GetState();
            if(previousPressedCount != kb.GetPressedKeyCount()){
                IEnumerable<Keys> pressedKeys = kb.GetPressedKeys().Except(previousPressedKeys);
                previousPressedKeys = kb.GetPressedKeys();
                previousPressedCount = kb.GetPressedKeyCount();
                if(pressedKeys.Contains(Keys.Down))
                    currentIndex++;
                if(pressedKeys.Contains(Keys.Up))
                    currentIndex--;
                if(pressedKeys.Contains(Keys.Enter))
                    onItemSelected(currentIndex);
                int numItems = menuItems.Length;
                currentIndex = (currentIndex % numItems + numItems) % numItems;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
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
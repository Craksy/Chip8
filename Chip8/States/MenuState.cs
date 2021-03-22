using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Chip8.Components.Menu;

namespace Chip8.States 
{
    public class MenuState : State
    {

        private SpriteFont font;
        public Menu currentMenu;

        public MenuState(Game1 game) 
        : base(game)
        {
            font = game.Content.Load<SpriteFont>("menuFont");
            currentMenu = new MainMenu(game, this);
            game.Components.Add(currentMenu);
        }

        public void ChangeMenu(Menu menu){
            game.Components.Remove(currentMenu);
            currentMenu = menu;
            game.Components.Add(currentMenu);
        }

        public override void Draw(GameTime gameTime)
        {
            screen.Clear(Color.Black);
            currentMenu.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
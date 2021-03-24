using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Chip8.States;
namespace Chip8.Components.Menu{
    public class MainMenu : Menu
    {
        public MainMenu(Game1 game, MenuState menuState) 
        : base(game, menuState) {
            title = "Main Menu";
            menuItems = new string[] {"Load ROM", "Settings", "Exit"};
        }

        protected override void OnItemSelected(int index) {
            base.OnItemSelected(index);
            switch(index){
                case 0:
                    menuState.ChangeMenu(new LoadRomMenu(game, menuState, @"./ROMS/"));
                    break;
                case 1:
                    menuState.ChangeMenu(new SettingsMenu(game, menuState));
                    break;
                case 2: 
                    game.Exit();
                    break;
            }
                
        }
    }
}
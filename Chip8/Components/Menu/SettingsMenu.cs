using Chip8.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chip8.Components.Menu{
    public class SettingsMenu : Menu
    {
        public SettingsMenu(Game1 game, MenuState menuState) 
        : base(game, menuState)
        {
            title = "Settings";
            menuItems = new string[] { 
                "Colors", 
                "Screen Size", 
                "Keybindings", 
                "Back"
                };
        }

        protected override void OnItemSelected(int index) {
            switch (index) {
                case 0:
                    menuState.ChangeMenu(new ColorSettingsMenu(game, menuState));
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    menuState.ChangeMenu(new MainMenu(game, menuState));
                    break;
            }
        }
    }
}
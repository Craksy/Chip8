using Microsoft.Xna.Framework;
using Chip8.States;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Chip8.Components.Menu
{
    public class ColorSettingsMenu : Menu
    {
        public ColorSettingsMenu(Game1 game, MenuState menuState) 
        : base(game, menuState) {
            title = "Color Settings";
            
            menuItems = new string[] {"Foreground", "Background", "Go Back"};
            onItemSelected = (index) => {
                switch(index){
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        menuState.ChangeMenu(new SettingsMenu(game, menuState));
                        break;
                }
            };
        }
    }
}
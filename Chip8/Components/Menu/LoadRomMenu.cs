using Chip8.States;
using Microsoft.Xna.Framework.Graphics;

using System.IO;
using System.Linq;


namespace Chip8.Components.Menu{
    public class LoadRomMenu : Menu
    {

        private string basePath;
        public LoadRomMenu(Game1 game, MenuState menuState, string path) 
        : base(game, menuState) {
            basePath = path;
            title = "Load ROM";
            
            menuItems = Directory.GetFileSystemEntries(basePath)
            .Select(f => Path.GetFileName(f))
            .ToArray<string>();
        }

        protected override void OnItemSelected(int index){
            string path = Path.Join(basePath, menuItems[index]);
            if(File.GetAttributes(path).HasFlag(FileAttributes.Directory)){
                menuState.ChangeMenu(new LoadRomMenu(game, menuState, path));
            }else{
                //HACK: cleaning up components should be handled by the state manager instead of this.
                game.Components.Clear();
                game.ChangeState(new States.EmulatorState(game, path));
            }
        }
    }
}
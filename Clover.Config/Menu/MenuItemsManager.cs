using Clover.Core.Configuration;

namespace Clover.Config.Menu
{

    
    
    
    public sealed class MenuItemsManager : DefaultConfigFileManager<MenuItems>
    {
        
        
        
        const string CONST_DEFAULTPATH = "~/config/MenuItemsA.config";

         
        
        
        public MenuItemsManager()
            : base(CONST_DEFAULTPATH)
        {            
        }

        
        
        
        
        public MenuItemsManager(string configfilepath)
            : base(configfilepath)
        {
          
        }     
    }
}

using Clover.Core.Configuration;
using StructureMap;

namespace Clover.Config
{
    
    
    
    public abstract class ConfigCenter<T1,T2> where T1:IConfigFileManager<T2> where T2 : IConfigInfo
    {
       
        private static T1 m_manager;

        
        
        
        
        public static void AddConfigChangeEvent(Clover.Core.Base.TAction<T2> f)
        { 
            m_manager.ChangeConfigEvent += f;
        }


        
        
        
        
        public static void RemoveConfigChangeEvent(Clover.Core.Base.TAction<T2> f)
        {
            m_manager.ChangeConfigEvent -= f;
        } 

        
        
        
        static ConfigCenter()
        {
            m_manager = ObjectFactory.GetInstance<T1>();

            m_manager.LoadConfig();
        }

        
        
        
        public static void ResetConfig()
        {
          m_manager.LoadConfig();
        }

        
        
        
        public static void SaveConfig()
        {
            m_manager.SaveConfig();
        }


        public static T2 Config
		{
            get
            {
                return m_manager.ConfigInfo;
            }
            set {
                m_manager.ConfigInfo = value;
            }
  		}
    }
}




using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Clover.Core.IO
{

    public abstract class PathTool {

        public abstract String CombineAbs( String[] arrPath );
        public abstract String Map( String path );

        
        
        
        
        public static PathTool getInstance() {
            if (GenericContext.IsWindows) return new WindowsPath();
            return new LinuxPath();
        }

        
        
        
        
        public static String GetBinDirectory() {
            if (GenericContext.IsWeb)
            {
                return HttpRuntime.BinDirectory;
            }

            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }

    }

}

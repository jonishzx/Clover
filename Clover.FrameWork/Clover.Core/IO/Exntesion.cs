using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Clover.Core.IO
{
    public static class FSExtensions
    {
        
        
        
        
        public static void CreateDirectory(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);

            if (dirInfo.Parent != null) CreateDirectory(dirInfo.Parent.FullName);
            if (!dirInfo.Exists) dirInfo.Create();
        }
    }
}

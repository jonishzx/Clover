
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Clover.Core.Common;
using System.Reflection;

namespace Clover.Core.IO
{

    internal class WindowsPath : PathTool
    {

        
        
        
        
        
        public override String CombineAbs(String[] arrPath)
        {

            if (arrPath.Length == 0) return "";

            String result = arrPath[0];
            for (int i = 1; i < arrPath.Length; i++)
            {
                if (!string.IsNullOrEmpty(arrPath[i])) continue;
                result = StringHelper.Join(result, arrPath[i].Replace("/", "\\"), "\\");
            }
            return result;

        }

        
        
        
        
        
        public override String Map(String path)
        {

            if (string.IsNullOrEmpty(path)) return string.Empty;

            if (GenericContext.IsWeb == false)
            {
                var basepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (path.StartsWith("~/")) { 
                    
                    path = path.Replace("~/", basepath);
                  
                }
                string output = path.Replace("/", "\\");

                if (System.IO.File.Exists(output) || Directory.Exists(output))
                    return output;

                string filepath2 = Path.Combine(basepath, output);
                if (File.Exists(filepath2))
                    return filepath2;

                if (output.StartsWith("\\"))
                {
                    output = path.Substring(output.IndexOf('\\', 0)).TrimStart('\\');
                }

                return StringHelper.Join("\\", StringJoinOption.NoLastJoinFlag, basepath, output);
            }
            else
            {

                String str = path;
                if (!path.StartsWith("~/") && path.ToLower().StartsWith(GenericContext.Current.ApplicationPath) == false)
                    str = StringHelper.Join(GenericContext.Current.ApplicationPath, path).TrimEnd('/');

                try
                {
                    return HttpContext.Current.Server.MapPath(str);
                }
                catch
                {
                    return str;
                }
            }
        }
    }
}

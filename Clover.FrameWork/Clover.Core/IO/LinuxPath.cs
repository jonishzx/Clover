


using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Clover.Core.Common;

namespace Clover.Core.IO
{

    internal class LinuxPath : PathTool
    {

        public override String CombineAbs(String[] arrPath)
        {

            if (arrPath.Length == 0) return "";

            String result = arrPath[0];
            for (int i = 1; i < arrPath.Length; i++)
            {
                if (string.IsNullOrEmpty(arrPath[i])) continue;
                result = StringHelper.Join(result, arrPath[i].Replace("\\", "/"));
            }
            return result;

        }

        public override String Map(String path)
        {

            if (string.IsNullOrEmpty(path)) return "";

            if (GenericContext.IsWeb == false)
            {

                return StringHelper.Join(AppDomain.CurrentDomain.BaseDirectory, path);
            }
            else
            {

                String str = path;
                if (path.ToLower().StartsWith(GenericContext.Current.ApplicationPath) == false)
                    str = StringHelper.Join(GenericContext.Current.ApplicationPath, path);

                return HttpContext.Current.Server.MapPath(path);
            }
        }
    }

}

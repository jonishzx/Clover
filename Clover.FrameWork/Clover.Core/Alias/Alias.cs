using System;
using System.Collections.Generic;
using System.Text;
using StructureMap;

namespace Clover.Core.Alias
{
    
    
    
    public sealed class ch : Clover.Core.Common.ConvertHelper
    {

    }

    
    
    
    public sealed class sh : Clover.Core.Common.StringHelper
    {

    }

    
    
    
    public sealed class xmlh<T> : Clover.Core.Common.XMLSeaializeHelper<T>
    {

    }

    
    
    
    public sealed class dth : Clover.Core.Extension.DataTableHelper
    {

    }

    [PluginFamily("Default")]
    
    
    
    public sealed class log : Clover.Core.Logging.LogCentral
    {

    }
    
}

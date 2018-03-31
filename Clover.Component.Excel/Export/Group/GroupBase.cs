using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;

namespace Clover.Component.Excel.Export.Group
{
    
   public abstract class GroupBase
    {
       
       
       
       public ISheet hsheet { get; set; }
       
       
       
       
       public abstract void DoGroup();

    }
}

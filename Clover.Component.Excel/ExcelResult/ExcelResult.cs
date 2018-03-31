using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clover.Component.Excel
{
    public class ExcelResult
    {
        
        
        
        public ExcelResultType Result { get; set; }

        
        
        
        public ExcelMessageCollection ExcelMessages { get; set; }

        #region 初始化

        public ExcelResult()
        {
            Result = ExcelResultType.Succeed;
            ExcelMessages = new ExcelMessageCollection();
        }

        #endregion

    }
}

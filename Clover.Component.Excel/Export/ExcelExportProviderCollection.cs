using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clover.Component.Excel.Export
{
    public class ExcelExportProviderCollection : List<ExcelExportProvider>
    {
        public ExcelExportProvider this[string typeName]
        {
            get
            {
                foreach (ExcelExportProvider eep in this)
                {
                    if (string.Compare(eep.TypeName, typeName, true) == 0)
                        return eep;
                }
                return null;
            }
        }
    }
}

using System.Collections.Generic;

namespace Clover.Component.Excel.Import
{
    public class ExcelImportProviderCollection : List<ExcelImportProvider>
    {
        public ExcelImportProvider this[string typeName]
        {
            get
            {
                foreach (ExcelImportProvider scp in this)
                {
                    if (string.Compare(scp.TypeName, typeName, true) == 0)
                        return scp;
                }
                return null;
            }
        }
    }
}

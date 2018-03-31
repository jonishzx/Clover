using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using NPOI.SS.UserModel;
using Clover.Component.Excel.Export.Group;
using Clover.Component.Excel.Export.Merged;
using Clover.Component.Excel.Common;
namespace Clover.Component.Excel.Export
{
    public class Worksheet
    {
        #region 属性定义

        
        
        
        public string Name { get; protected set; }

        
        
        
        public string Title { get; set; }

        
        
        
        public int Row { get; protected set; }

        
        
        
        public int Column { get; protected set; }

        
        
        
        public ExcelExportProvider ExcelExportProvider { get; set; }
        
        
        
        public string ID { get; protected set; }

        
        
        
        public string RefSheetId { get; set; }

        
        
        
        public string RefSheetName { get; set; }

        private List<Area> _area;
        public List<Area> Areas
        {
            get
            {
                return _area;
            }
            set
            {
                _area = value;
            }
        }

        
        
        
        private List<Formula> _formulas;
        public List<Formula> Formulas
        {
            get
            {
                return _formulas;
            }
            set
            {
                _formulas = value;
            }
        }

        
        
        
        public List<MergedConfig> MergedConfigs { get; set; }

        
        
        
        public List<GroupConfig> GroupConfigs { get; set; }

        public ISheet ISheet { get; set; }

        
        
        
        public string AutoSizeColumns { get; protected set; }
        
        
        
        protected int[] AutoSizeColumnStart { get; set; }
        
        
        
        protected int[] AutoSizeColumnEnd { get; set; }

        public XmlNode HSSFSheetXmlNode { get; set; }

        #endregion

        #region ctor

        public Worksheet(XmlNode worksheetNode, ExcelExportProvider provider)
            : this(worksheetNode,provider,"","","","")
        {
        }


        public Worksheet(XmlNode worksheetNode, ExcelExportProvider provider,string refWorksheetId,string refWorkSheetName,string newWorkSheetId,string newWorkSheetName)
        {
            Name = XmlUtility.getNodeAttributeStringValue(worksheetNode, "name");
            Title = XmlUtility.getNodeAttributeStringValue(worksheetNode, "title");
            Row = XmlUtility.getNodeAttributeIntValue(worksheetNode, "row");
            Column = XmlUtility.getNodeAttributeIntValue(worksheetNode, "col");
            ID = XmlUtility.getNodeAttributeStringValue(worksheetNode, "id");

            RefSheetId = refWorksheetId;
            RefSheetName = refWorkSheetName;
            if(!string.IsNullOrEmpty(newWorkSheetId))
            {
                ID = newWorkSheetId;
            }
            if (!string.IsNullOrEmpty(newWorkSheetName))
            {
                Name = newWorkSheetName;
            }
            GetMergedConfigs(worksheetNode);
            GetGroupConfigs(worksheetNode);
            InitAutoSizeColumn(worksheetNode);
            ExcelExportProvider = provider;
            Formulas = GetFormulas(worksheetNode);
            Areas = GetAreas(worksheetNode);

            HSSFSheetXmlNode = worksheetNode;
        }

        
        
        
        
        private void GetMergedConfigs(XmlNode worksheetNode)
        {
            MergedParse parse = new MergedParse(worksheetNode);
            MergedConfigs = parse.ParseMergedConfig();
        }

        
        
        
        
        private void GetGroupConfigs(XmlNode worksheetNode)
        {
            GroupParse parse = new GroupParse(worksheetNode);
            GroupConfigs = parse.ParseGroup();
        }

        
        
        
        
        
        public List<Area> GetAreas(XmlNode worksheetNode)
        {
            List<Area> areas = new List<Area>();
            XmlNodeList xNodes = worksheetNode.ChildNodes;
            if (xNodes != null && xNodes.Count > 0)
            {
                foreach (XmlNode xNode in xNodes)
                {
                    
                    if (xNode.Name == "StaticArea")
                    {
                        Area area = new StaticArea(xNode, this);
                        areas.Add(area);
                    }
                    else if (xNode.Name == "RepeatArea")
                    {
                        Area area = new RepeatArea(xNode, this);
                        areas.Add(area);
                    }
                }
            }
            return areas;
        }

        private List<Formula> GetFormulas(XmlNode worksheetNode)
        {
            List<Formula> formulas = new List<Formula>();
            XmlNodeList xNodes = worksheetNode.SelectNodes("Formula");
            if (xNodes != null && xNodes.Count > 0)
            {
                foreach (XmlNode xNode in xNodes)
                {
                    Formula formula = new Formula(xNode);
                    formulas.Add(formula);
                }
            }
            return formulas;
        }

        
        
        
        
        public void AddFormulas(XmlNodeList formuls)
        {
            if(Formulas==null)
            {
                Formulas = new List<Formula>();
            }
            foreach (XmlNode formulNode in formuls)
            {
                Formulas.Add(new Formula(formulNode));
            }
            foreach(Area area in Areas)
            {
                area.InitArea();
            }
        }

        private void InitAutoSizeColumn(XmlNode worksheetNode)
        {
            AutoSizeColumns = XmlUtility.getNodeAttributeStringValue(worksheetNode, "autosizecolumns", "");
            if (!string.IsNullOrEmpty(AutoSizeColumns))
            {
                string[] args = AutoSizeColumns.Split(',');
                AutoSizeColumnStart = new int[args.Length];
                AutoSizeColumnEnd = new int[args.Length];
                for (int iLoop = 0; iLoop < args.Length; iLoop++)
                {
                    string argument = args[iLoop].Trim();
                    
                    if (argument.Contains("~"))
                    {
                        int start = -1, end = -1;
                        string[] autoSizeCols = argument.Split('~');
                        if (int.TryParse(autoSizeCols[0], out start) && int.TryParse(autoSizeCols[1], out end))
                        {
                            if (start < 0 || end < 0)
                                throw (new Exception("AutoSizeColumns的开始或结束列未配置或小于0"));
                            int temp;
                            if (start > end)
                            {
                                temp = start;
                                start = end;
                                end = temp;
                            }
                            AutoSizeColumnStart[iLoop] = start;
                            AutoSizeColumnEnd[iLoop] = end;
                        }
                    }
                    else
                    {
                        int start = -1;
                        if (int.TryParse(argument, out start))
                        {
                            if (start > 0)
                            {
                                AutoSizeColumnStart[iLoop] = start;
                                AutoSizeColumnEnd[iLoop] = -1;
                            }
                            else
                            {
                                AutoSizeColumnStart[iLoop] = -1;
                                AutoSizeColumnEnd[iLoop] = -1;
                                throw (new Exception("AutoSizeColumns的开始或结束列未配置或小于0"));
                            }
                        }
                    }
                }
            }
        }

        public void AutoSizeColumn(ISheet hssfSheet)
        {
            if (AutoSizeColumnStart != null)
            {
                for (int nLoop = 0; nLoop < AutoSizeColumnStart.Length; nLoop++)
                {
                    if (AutoSizeColumnStart[nLoop] == -1)
                        continue;
                    else if (AutoSizeColumnEnd[nLoop] == -1)
                    {
                        hssfSheet.AutoSizeColumn(AutoSizeColumnStart[nLoop]);
                    }
                    else
                    {
                        for (int iLoop = AutoSizeColumnStart[nLoop]; iLoop < AutoSizeColumnEnd[nLoop]; iLoop++)
                        {
                            hssfSheet.AutoSizeColumn(iLoop);
                        }
                    }
                }
            }
        }

        #endregion
        


    }
}

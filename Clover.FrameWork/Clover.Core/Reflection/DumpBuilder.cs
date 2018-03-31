namespace Clover.Core.Reflection
{
	#region 引用的命名空间.
	

	using System;
	using System.Configuration;
	using System.Collections;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using Clover.Core.Common;
	using System.Data;
	using System.Data.OleDb;
	using Clover.Core.Properties;
	using System.Reflection;
	using System.ComponentModel;
	using System.Collections.Generic;

	
	#endregion

	

	
	
	
	public sealed class DumpBuilder
	{
        #region 公共构造函数.
        

        
        
        
        public DumpBuilder()
            :
            this(0, false, null)
        {
        }

        
        
        
        
        public DumpBuilder(
            int indentLevel)
            :
            this(indentLevel, false, null)
        {
        }

        
        
        
        
        
        public DumpBuilder(
            int indentLevel,
            bool deep)
            :
            this(indentLevel, deep, null)
        {
        }

        
        
        
        
        
        
        public DumpBuilder(
            int indentLevel,
            bool deep,
            Type typeToDump)
        {
            this.indentLevel = indentLevel;
            this.deep = deep;

            if (typeToDump != null)
            {
                string s = string.Format(
                    @"反射类型 '{0}':",
                    typeToDump.FullName);

                lines.Add(s);
            }
        }

        
        #endregion

        #region 公共方法.
        

        
        
        
        
        
        public void AddLine(
            string name,
            object value)
        {
            lines.Add(MakeStringToAdd(name, value));
        }

        
        
        
        
        public void AddLine(
            string text)
        {
            lines.Add(text);
        }

        
        
        
        
        
        
        public void InsertLine(
            int index,
            string name,
            object value)
        {
            lines.Insert(index, MakeStringToAdd(name, value));
        }


        
        
        
        
        
        public void InsertLine(
            int index,
            string text)
        {
            lines.Insert(index, text);
        }

        
        
        
        
        
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    sb.AppendLine(DoIndent(line.TrimEnd()));
                }
            }

            return sb.ToString().TrimEnd();
        }

        
        #endregion

        #region 公共属性.
        

        
        
        
        
        public bool IsDeep
        {
            get
            {
                return deep;
            }
        }

        
        #endregion

        #region 静态方法(预定项).
        

        
        
        
        
        
        
        public static string Dump(
            Exception x)
        {
            StringBuilder sb = new StringBuilder();
            Reflect(sb, x);

            return sb.ToString();
        }


        
        
        
        
        
        public static string Dump(
            object x)
        {
            StringBuilder sb = new StringBuilder();
            Reflect(sb, x);

            return sb.ToString();
        }

        
        #endregion

        #region 反射输出.
        

        
        
        
        
        
        private static void Reflect(
            StringBuilder sb,
            object obj)
        {
            Reflect(sb, new GraphRef(null, obj, null), 0);
        }

        
        
        
        
        
        
        private static void Reflect(
            StringBuilder sb,
            GraphRef obj,
            int indent)
        {
            const int maxDepth = 3;

            
            if (!(obj.Value is ValueType)) 
            {
                GraphRef parentRef = obj.Parent; 
                while (parentRef != null) 
                {
                    if (parentRef.Value == obj.Value) 
                        return; 

                    parentRef = parentRef.Parent; 
                }
            }

            sb.Append('\t', indent);

            
            if (!String.IsNullOrEmpty(obj.PropName))
            {
                sb.Append(obj.PropName);
                sb.Append("=");
            }

            int childIndent = indent + 1;

            
            if (obj.Value == null)
            {
                sb.Append("null");
            }
            
            else if (obj.Value is string)
            {
                sb.Append("\"" + Escape((string)obj.Value) + "\"");
            }
            
            else if (obj.Value is char)
            {
                sb.Append("\'" + Escape(new String((char)obj.Value, 1)) + "\'");
            }
            
            else if (obj.Value is Array)
            {
                Array arr = (Array)obj.Value;
                sb.Append("\r\n");
                sb.Append('\t', indent);
                sb.Append("[\r\n");
                for (int i = 0; i < arr.Length; i++)
                {
                    Reflect(sb, new GraphRef(obj, arr.GetValue(i), null), childIndent);
                    if (i < arr.Length - 1)
                        sb.Append(',');
                    sb.Append("\r\n");
                }
                sb.Append('\t', indent);
                sb.Append("]\r\n");
            }
            
            else if (obj.Value is Type)
            {
                sb.Append("Type: ");
                sb.Append(((Type)obj.Value).FullName);
            }
            
            else if (obj.Value is MemberInfo)
            {
                sb.Append(obj.Value.GetType().Name);
                sb.Append(": ");
                sb.Append(((MemberInfo)obj.Value).Name);
            }
            
            else if (Convert.GetTypeCode(obj.Value) == TypeCode.Object)
            {

                Type type = obj.Value.GetType();
                sb.Append(type.Name); 
                if (indent <= maxDepth)
                {
                    sb.Append("\r\n");
                    sb.Append('\t', indent);
                    sb.Append("{\r\n");
                    
                    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    
                    for (int i = 0; i < fields.Length; i++)
                    {
                        FieldInfo pi = fields[i];
                       
                        try
                        {
                            Reflect(sb, new GraphRef(obj, pi.GetValue(obj.Value), pi.Name), childIndent);
                        }
                        catch (Exception e)
                        {
                            sb.Append("<失败获取属性值 (");
                            sb.Append(e.GetType().Name);
                            sb.Append(")>");
                        }
                        if (i < fields.Length - 1)
                            sb.Append(',');
                        sb.Append("\r\n");
                   
                    }

                    PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    
                    for (int i = 0; i < props.Length; i++)
                    {
                        PropertyInfo pi = props[i];
                        if (pi.GetIndexParameters().Length == 0) 
                        {
                            try
                            {
                                Reflect(sb, new GraphRef(obj, pi.GetValue(obj.Value, null), pi.Name), childIndent);
                            }
                            catch (Exception e)
                            {
                                sb.Append("<失败获取属性值 (");
                                sb.Append(e.GetType().Name);
                                sb.Append(")>");
                            }
                            if (i < props.Length - 1)
                                sb.Append(',');
                            sb.Append("\r\n");
                        }
                    }
                    
                    if (obj is IList)
                    {
                        IList list = (IList)obj.Value;
                        sb.Append("\r\n");
                        sb.Append('\t', indent);
                        sb.Append("[\r\n");
                        for (int i = 0; i < list.Count; i++)
                        {
                            Reflect(sb, new GraphRef(obj, list[i], null), childIndent);
                            if (i < list.Count - 1)
                                sb.Append(',');
                            sb.Append("\r\n");
                        }
                        sb.Append('\t', indent);
                        sb.Append("]\r\n");
                    }
                    sb.Append('\t', indent);
                    sb.Append("}");
                }
            }
            
            else
            {
                sb.Append(obj.Value.ToString());
            }

        }

        
        #endregion

        #region 过滤字符.
        

        static string[] escapeChars = new string[] { "\r", "\n", "\t", "\"", "\'", "\\" };
        static string[] escapeCharReplacements = new string[]
		{
			"\\r","\\n","\\t","\\\"","\\\'","\\\\"
		};

        
        
        
        
        
        static string Escape(string input)
        {
            StringBuilder sb = new StringBuilder(input);
            for (int i = 0; i < escapeChars.Length; i++)
                sb.Replace(escapeChars[i], escapeCharReplacements[i]);
            return sb.ToString();
        }

        
        #endregion

        #region 私有的反射帮助类.
        

        
        
        
        private class GraphRef
        {
            #region 公共方法.

            
            
            
            
            
            
            public GraphRef(
                GraphRef parent,
                object obj,
                string propName)
            {
                this.parent = parent;
                this.value = obj;
                this.propName = propName;
            }

            #endregion

            #region 公共属性.

            
            
            
            
            public object Value
            {
                get
                {
                    return value;
                }
            }

            
            
            
            
            public GraphRef Parent
            {
                get
                {
                    return parent;
                }
            }

            
            
            
            
            public string PropName
            {
                get
                {
                    return propName;
                }
            }

            #endregion

            #region 私有变量.

            private object value;
            private GraphRef parent;
            private string propName;

            #endregion
        }

        
        #endregion

        #region 私有方法.
        

        
        
        
        
        
        
        private static string MakeStringToAdd(
            string name,
            object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(
                    "Can not dump the name",
                    @"name");
            }
            else if (name.Length <= 0)
            {
                throw new ArgumentException(
                    "Can not dump the name",
                    @"name");
            }
            else
            {
                string result = string.Empty;

                result += string.Format(
                    @"{0}: '{1}'",
                    name,
                    value == null ? @"(null)" : value.ToString());

                return result;
            }
        }

        
        
        
        
        
        private string DoIndent(
            string text)
        {
            StringBuilder result = new StringBuilder();

            result.Append('\t', indentLevel);
            result.Append(text.TrimEnd());

            return result.ToString();
        }

        
        #endregion

        #region 私有变量.
        

        private List<string> lines = new List<string>();

        private readonly int indentLevel = 0;
        private readonly bool deep = false;

        
        #endregion
	}

	
}
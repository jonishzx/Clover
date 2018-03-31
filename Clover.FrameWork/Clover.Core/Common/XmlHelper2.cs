namespace Clover.Core.Common
{
	#region 引用的命名空间.
	

	using System;
	using System.Globalization;
	using System.Xml;
	using System.IO;
	using System.Text;

	
	#endregion

	

	
	
	
	public sealed class XmlHelper2
	{
		#region Miscellaneous routines.
		

		
		
		
		
		
		public static XmlDocument CreateDocument()
		{
			return CreateDocument( @"unicode" );
		}

		
		
		
		
		
		
		public static XmlDocument CreateDocument(
			string encoding )
		{
			XmlDocument doc = new XmlDocument();

			doc.AppendChild(
				doc.CreateProcessingInstruction(
				@"xml",
				string.Format(
				@"version='1.0' encoding='{0}'",
				encoding ) ) );

			return doc;
		}

		
		
		
		
		
		
		public static XmlDocument CreateDocument(
			Encoding encoding )
		{
			return CreateDocument( encoding.BodyName );
		}

		
		#endregion

		#region Reading attributes.
		

		
		
		
		
		
		public static void ReadAttribute(
			out string result,
			XmlAttribute attribute )
		{
			ReadAttribute(
				out result,
				attribute,
				null );
		}

		
		
		
		
		
		
		public static void ReadAttribute(
			out string result,
			XmlAttribute attribute,
			string defaultResult )
		{
			if ( attribute == null )
			{
				result = defaultResult;
			}
			else
			{
				result = attribute.Value;
			}
		}

		
		
		
		
		
		public static void ReadAttribute(
			out DirectoryInfo result,
			XmlAttribute attribute )
		{
			ReadAttribute(
				out result,
				attribute,
				null );
		}

		
		
		
		
		
		
		public static void ReadAttribute(
			out DirectoryInfo result,
			XmlAttribute attribute,
			DirectoryInfo defaultResult )
		{
			if ( attribute == null )
			{
				result = defaultResult;
			}
			else
			{
				result = new DirectoryInfo( attribute.Value );
			}
		}

		
		
		
		
		
		public static void ReadAttribute<T>(
			out T result,
			XmlAttribute attribute )
		{
			ReadAttribute<T>(
				out result,
				attribute,
				default( T ) );
		}

		
		
		
		
		
		
		public static void ReadAttribute<T>(
			out T result,
			XmlAttribute attribute,
			T defaultResult )
		{
			if ( attribute == null )
			{
				result = defaultResult;
			}
			else
			{
				result = ConvertHelper.ToT<T>( attribute.Value, defaultResult );
			}
		}

		
		
		
		
		
		public static void ReadAttribute(
			out FileInfo result,
			XmlAttribute attribute )
		{
			ReadAttribute(
				out result,
				attribute,
				null );
		}

		
		
		
		
		
		
		public static void ReadAttribute(
			out FileInfo result,
			XmlAttribute attribute,
			FileInfo defaultResult )
		{
			if ( attribute == null )
			{
				result = defaultResult;
			}
			else
			{
				result = new FileInfo( attribute.Value );
			}
		}

		
		
		
		
		
		public static void ReadAttribute(
			out Guid result,
			XmlAttribute attribute )
		{
			ReadAttribute(
				out result,
				attribute,
				Guid.Empty );
		}

		
		
		
		
		
		
		public static void ReadAttribute(
			out Guid result,
			XmlAttribute attribute,
			Guid defaultResult )
		{
			if ( attribute == null )
			{
				result = defaultResult;
			}
			else
			{
				result = new Guid( attribute.Value );
			}
		}

		
		
		
		
		
		public static void ReadAttribute(
			out double result,
			XmlAttribute attribute )
		{
			ReadAttribute(
				out result,
				attribute,
				0 );
		}

		
		
		
		
		
		
		public static void ReadAttribute(
			out double result,
			XmlAttribute attribute,
			double defaultResult )
		{
			if ( attribute == null || attribute.Value == null )
			{
				result = defaultResult;
			}
			else
			{
				try
				{
					result = double.Parse( attribute.Value );
				}
				catch ( FormatException )
				{
					result = defaultResult;
				}
				catch ( OverflowException )
				{
					result = defaultResult;
				}
			}
		}

		
		
		
		
		
		public static void ReadAttribute(
			out int result,
			XmlAttribute attribute )
		{
			ReadAttribute(
				out result,
				attribute,
				0 );
		}

		
		
		
		
		
		
		public static void ReadAttribute(
			out int result,
			XmlAttribute attribute,
			int defaultResult )
		{
			if ( attribute == null || attribute.Value == null )
			{
				result = defaultResult;
			}
			else
			{
				try
				{
					result = int.Parse( attribute.Value );
				}
				catch ( FormatException )
				{
					result = defaultResult;
				}
				catch ( OverflowException )
				{
					result = defaultResult;
				}
			}
		}

		
		
		
		
		
		public static void ReadAttribute(
			out DateTime result,
			XmlAttribute attribute )
		{
			ReadAttribute(
				out result,
				attribute,
				DateTime.MinValue );
		}

		
		
		
		
		
		
		public static void ReadAttribute(
			out DateTime result,
			XmlAttribute attribute,
			DateTime defaultResult )
		{
			if ( attribute == null || attribute.Value == null )
			{
				result = defaultResult;
			}
			else
			{
				try
				{
					result = DateTime.Parse( attribute.Value );
				}
				catch ( FormatException )
				{
					result = defaultResult;
				}
				catch ( OverflowException )
				{
					result = defaultResult;
				}
			}
		}

		
		
		
		
		
		public static void ReadAttribute(
			out bool result,
			XmlAttribute attribute )
		{
			ReadAttribute(
				out result,
				attribute,
				false );
		}

		
		
		
		
		
		
		public static void ReadAttribute(
			out bool result,
			XmlAttribute attribute,
			bool defaultResult )
		{
			if ( attribute == null || attribute.Value == null )
			{
				result = defaultResult;
			}
			else
			{
				if ( ConvertHelper.IsBoolean( attribute.Value ) )
				{
					result = ConvertHelper.ToBoolean(
						attribute.Value,
						defaultResult );
				}
				else
				{
					if ( attribute.Value == null )
					{
						result = defaultResult;
					}
					else
					{
						if ( attribute.Value == @"0" )
						{
							result = false;
						}
						else if ( attribute.Value == @"-1" || 
							attribute.Value == @"1" )
						{
							result = true;
						}
						else
						{
							result = defaultResult;
						}
					}
				}
			}
		}

		
		
		
		
		
		public static void ReadAttribute(
			out decimal result,
			XmlAttribute attribute )
		{
			ReadAttribute(
				out result,
				attribute,
				decimal.Zero );
		}

		
		
		
		
		
		
		public static void ReadAttribute(
			out decimal result,
			XmlAttribute attribute,
			decimal defaultResult )
		{
			if ( attribute == null || attribute.Value == null )
			{
				result = defaultResult;
			}
			else
			{
				try
				{
					result = decimal.Parse(
						attribute.Value,
						CultureInfo.InvariantCulture );
				}
				catch ( FormatException )
				{
					result = defaultResult;
				}
			}
		}

		
		#endregion

		#region Reading nodes.
		

		
		
		
		
		
		public static void ReadNode(
			out string result,
			XmlNode node )
		{
			ReadNode(
				out result,
				node,
				null );
		}

		
		
		
		
		
		
		public static void ReadNode(
			out string result,
			XmlNode node,
			string defaultResult )
		{
			if ( node == null )
			{
				result = defaultResult;
			}
			else
			{
				result = node.InnerText;
			}
		}

		
		
		
		
		
		public static void ReadNode(
			out int result,
			XmlNode node )
		{
			ReadNode(
				out result,
				node,
				0 );
		}

		
		
		
		
		
		
		public static void ReadNode(
			out int result,
			XmlNode node,
			int defaultResult )
		{
			if ( node == null || node.InnerText == null )
			{
				result = defaultResult;
			}
			else
			{
				try
				{
					result = int.Parse( node.InnerText );
				}
				catch ( FormatException )
				{
					result = defaultResult;
				}
				catch ( OverflowException )
				{
					result = defaultResult;
				}
			}
		}

		
		
		
		
		
		public static void ReadNode(
			out bool result,
			XmlNode node )
		{
			ReadNode(
				out result,
				node,
				false );
		}

		
		
		
		
		
		
		public static void ReadNode(
			out bool result,
			XmlNode node,
			bool defaultResult )
		{
			if ( node == null || node.InnerText == null )
			{
				result = defaultResult;
			}
			else
			{
				try
				{
					result = bool.Parse( node.InnerText );
				}
				catch ( FormatException )
				{
					if ( node.InnerText == @"0" )
					{
						result = false;
					}
					else if ( node.InnerText == @"-1" || 
						node.InnerText == @"1" )
					{
						result = true;
					}
					else
					{
						result = defaultResult;
					}
				}
			}
		}

		
		
		
		
		
		public static void ReadNode(
			out decimal result,
			XmlNode node )
		{
			ReadNode(
				out result,
				node,
				decimal.Zero );
		}

		
		
		
		
		
		
		public static void ReadNode(
			out decimal result,
			XmlNode node,
			decimal defaultResult )
		{
			if ( node == null || node.InnerText == null )
			{
				result = defaultResult;
			}
			else
			{
				try
				{
					result = decimal.Parse(
						node.InnerText,
						CultureInfo.InvariantCulture );
				}
				catch ( FormatException )
				{
					result = defaultResult;
				}
			}
		}

		
		#endregion

		#region Private routines.
		

		
		
		
		public XmlHelper2()
		{
		}

		
		#endregion
	}

	
}
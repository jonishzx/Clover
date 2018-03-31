using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml;
namespace Clover.Core.Common
{
    /// <summary>
    /// 用于序列化和反序列化CDATA标记的内容
    /// </summary>
    [Serializable]
    public class XmlCDataString : IXmlSerializable
    {
        private string _strValue = null;

        public XmlCDataString()
        {
        }

        public XmlCDataString(string strValue)
        {
            _strValue = strValue;
        }

        public string StringValue
        {
            get { return _strValue; }
            set { _strValue = value; }
        }

        public static implicit operator XmlCDataString(string strValue)
        {
            return new XmlCDataString(strValue);
        }

        public static explicit operator string(XmlCDataString cdata)
        {
            return cdata.StringValue;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            // TODO
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlDocument doc = null;
            XmlCDataSection xmlCData = null;
            XmlSerializer serializer = null;

            doc = new XmlDataDocument();
            xmlCData = doc.CreateCDataSection(_strValue);
            serializer = new XmlSerializer(typeof(XmlCDataSection));
            serializer.Serialize(writer, xmlCData);

        }
    }
}
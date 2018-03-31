using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clover.Component.Excel.Common
{
    public class XmlUtility
    {
        
        public static XmlAttribute addAttribute(XmlNode parentNode, string attName, string attValue)
        {
            if (parentNode == null)
            {
                return null;
            }
            XmlAttribute node = parentNode.Attributes[attName];
            if (node == null)
            {
                node = parentNode.OwnerDocument.CreateAttribute(attName);
                parentNode.Attributes.Append(node);
            }
            node.Value = attValue;
            return node;
        }

        public static XmlAttribute addAttribute(XmlNode parentNode, string attName, string attValue, string nodeNameSpace)
        {
            if (parentNode == null)
            {
                return null;
            }
            string namespaceOfPrefix = parentNode.GetNamespaceOfPrefix(nodeNameSpace);
            XmlAttribute attribute = parentNode.Attributes[attName];
            if (attribute == null)
            {
                XmlNode node = parentNode.OwnerDocument.CreateNode(XmlNodeType.Attribute, attName, namespaceOfPrefix);
                node.Value = attValue;
                parentNode.Attributes.SetNamedItem(node);
                return (XmlAttribute)node;
            }
            attribute.Value = attValue;
            return attribute;
        }

        public static XmlNode addChildNode(XmlNode parentNode, string nodeName, string nodeValue)
        {
            return addChildNode(parentNode, nodeName, nodeValue, string.Empty);
        }

        public static XmlNode addChildNode(XmlNode parentNode, XmlNode outXmlNode, bool userOuterXml)
        {
            if (userOuterXml)
            {
                XmlNode newChild = parentNode.OwnerDocument.ImportNode(outXmlNode, true);
                parentNode.AppendChild(newChild);
                return newChild;
            }
            foreach (XmlNode node2 in outXmlNode.ChildNodes)
            {
                addChildNode(parentNode, node2, true);
            }
            return parentNode;
        }

        public static XmlNode addChildNode(XmlNode parentNode, string nodeName, string innerXml, bool setInnerTextOrInnerXml)
        {
            return addChildNode(parentNode, nodeName, innerXml, string.Empty, setInnerTextOrInnerXml);
        }

        public static XmlNode addChildNode(XmlNode parentNode, string nodeName, string nodeValue, string nodeNameSpace)
        {
            return addChildNode(parentNode, nodeName, nodeValue, nodeNameSpace, false);
        }

        public static XmlNode addChildNode(XmlNode parentNode, string nodeName, string innerXml, string nodeNameSpace, bool setToInnerXml)
        {
            XmlNode newChild = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, nodeNameSpace);
            if (setToInnerXml)
            {
                newChild.InnerXml = innerXml;
            }
            else
            {
                newChild.InnerText = innerXml;
            }
            parentNode.AppendChild(newChild);
            return newChild;
        }

        public static XmlNamespaceManager createXmlNameSpace(XmlDocument xml)
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(xml.NameTable);
            manager.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
            manager.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
            manager.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
            manager.AddNamespace("html", "http://www.w3.org/TR/REC-html40");
            return manager;
        }

        public static bool getNodeAttributeBooleanValue(XmlNode node, string attr)
        {
            return getNodeAttributeStringValue(node, attr, string.Empty).ToLower().Equals(bool.TrueString.ToLower());
        }

        public static double getNodeAttributedoubleValue(XmlNode node, string attr)
        {
            return getNodeAttributedoubleValue(node, attr, 0.0);
        }

        public static double getNodeAttributedoubleValue(XmlNode node, string attr, double defaultValue)
        {
            double num;
            if ((node.Attributes[attr] != null) && double.TryParse(node.Attributes[attr].Value, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static int getNodeAttributeIntValue(XmlNode node, string attr)
        {
            return getNodeAttributeIntValue(node, attr, 0);
        }

        public static int getNodeAttributeIntValue(XmlNode node, string attr, int defaultValue)
        {
            int num;
            if ((node.Attributes[attr] != null) && int.TryParse(node.Attributes[attr].Value, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static string getNodeAttributeStringValue(XmlNode node, string attr)
        {
            return getNodeAttributeStringValue(node, attr, null);
        }

        public static string getNodeAttributeStringValue(XmlNode node, string attr, string defaultValue)
        {
            if ((node == null) || (node.Attributes[attr] == null))
            {
                return defaultValue;
            }
            return node.Attributes[attr].Value;
        }

        public static double getNodedoubleValue(XmlNode node)
        {
            return getNodedoubleValue(node, 0.0);
        }

        public static double getNodedoubleValue(XmlNode node, double defaultValue)
        {
            double num;
            if ((node != null) && double.TryParse(node.InnerText, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static int getNodeIntValue(XmlNode node)
        {
            return getNodeIntValue(node, 0);
        }

        public static int getNodeIntValue(XmlNode node, int defaultValue)
        {
            int num;
            if ((node != null) && int.TryParse(node.InnerText, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static string getNodeStringValue(XmlNode node)
        {
            if (node == null)
            {
                return null;
            }
            return node.InnerText;
        }

        public static XmlNode getSubNode(XmlNode parent, string xPath)
        {
            return parent.SelectSingleNode(xPath);
        }

        public static XmlNodeList getSubNodeList(XmlNode parent, string xPath)
        {
            return parent.SelectNodes(xPath);
        }
    }
}

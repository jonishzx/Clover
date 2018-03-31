namespace Clover.Core.Common
{
    #region 命名空间
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Collections;
    #endregion

    
    
    
    
    public class XMLSeaializeHelper<T>
    {
        XmlSerializer mySerializer;
        String ClassName;
        String BaseDirectory;

        
        
        
        public XMLSeaializeHelper()
        {
            mySerializer = new XmlSerializer(typeof(T));
            BaseDirectory = "";
        }

        
        
        
        
        public XMLSeaializeHelper(String myBaseDirectory)
        {
            mySerializer = new XmlSerializer(typeof(T));
            BaseDirectory = myBaseDirectory;
        }

        
        
        
        
        
        public String FileName(Type myObj)
        {
            ClassName = myObj.Name;
            return BaseDirectory + "\\" + ClassName + ".xml";
        }

        
        
        
        
        public String GetCustXMLPath(Type myObj) 
        {
            ClassName = myObj.Name;
            String TaskFile = BaseDirectory + "\\" +"CustConfig\\"+ ClassName + ".xml";
            if (!System.IO.Directory.Exists(Path.GetDirectoryName(TaskFile)))
            {
               System.IO.Directory.CreateDirectory(Path.GetDirectoryName(TaskFile));
            }
            return TaskFile;
        }

        
        
        
        
        public void Save(T myObj)
        {
            
            TextWriter myWriter = new StreamWriter(GetCustXMLPath(myObj.GetType()));
            mySerializer.Serialize(myWriter, myObj);
            myWriter.Close();
        }

        public void Save(T myObj, string FilePath)
        {
            
            TextWriter myWriter = new StreamWriter(FilePath, false); 
            mySerializer.Serialize(myWriter, myObj);
            myWriter.Close();
        }

        
        
        
        
        
        public string SaveToString(T myObj)
        {
            string rtn = null;

            using (MemoryStream ms = new MemoryStream())
            {

                mySerializer.Serialize(ms, myObj);

                ms.Seek(0, SeekOrigin.Begin);

                using (StreamReader sr = new StreamReader(ms))
                {
                    rtn = sr.ReadToEnd();
                    sr.Close();
                }
                ms.Close();
            }

            return rtn;
        }

     

        
        
        
        
        
        
        public T Load(string XMLData)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            StringReader sr = new StringReader(XMLData);
            T NewObject = (T)mySerializer.Deserialize(sr);
            sr.Close();
            return NewObject;
        }


        public T Load()
        {
          
            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            
            TextReader myReader = new StreamReader(GetCustXMLPath(typeof(T)));
            T NewObject = (T)mySerializer.Deserialize(myReader);
            myReader.Close();
            return NewObject;
        }

        
        
        
        
        
        public T LoadFile(string filepath)
        {
            if (File.Exists(filepath))
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(T));
                
                TextReader myReader = new StreamReader(filepath);
                T NewObject = (T)mySerializer.Deserialize(myReader);
                myReader.Close();
                return NewObject;
            }
            else
                return default(T);
        }

    }
}

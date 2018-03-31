
namespace Clover.Core.IO
{
    using System;
    using System.IO;
    using ICSharpCode.SharpZipLib.Zip;
    using Clover.Core.Logging;


    
    
    
    public sealed class Zipper
    {
        static Zipper()
        {
        }

        
        
        
        
        
        public static System.IO.MemoryStream ZipFiles(string[] fList)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(ms);
            System.IO.FileStream fs;
            for (int i = 0; i < fList.Length; i++)
            {
                fs = new System.IO.FileStream(fList[i], System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(System.IO.Path.GetFileName(fList[i]));
                zipOStream.PutNextEntry(entry);
                zipOStream.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
            zipOStream.Finish();
            zipOStream.Flush();
            zipOStream.Close();
            return ms;
        }

        
        
        
        
        
        
        public static bool ZipFiles(string[] fList, string zipPath)
        {
            bool rValue = true;
            System.IO.FileStream ms = new System.IO.FileStream(zipPath, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite, System.IO.FileShare.Delete);
            ZipOutputStream zipOStream = new ZipOutputStream(ms);
            try
            {
                System.IO.FileStream fs;
                for (int i = 0; i < fList.Length; i++)
                {
                    fs = new System.IO.FileStream(fList[i], System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ZipEntry entry = new ZipEntry(System.IO.Path.GetFileName(fList[i]));
                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(buffer, 0, buffer.Length);
                    fs.Close();
                }
                zipOStream.Finish();
                zipOStream.Flush();
            }
            catch(Exception ex)
            {
                LogCentral.Current.Error(
                    string.Format(
                    @"压缩以下文件 '{0}' 时出现异常 [{1}]",
                    Common.StringHelper.Join(",", fList),
                    ex.Message));

                rValue = false;
            }
            finally
            {
                zipOStream.Close();
                ms.Close();
            }
            return rValue;
        }

        
        
        
        
        
        
        public static bool Unzip(string sourcePath, string destPath, string prefix)
        {
            bool rValue = false;
            ZipInputStream s = null;
            ZipEntry theEntry;
            string fileName = "";
            try
            {
                s = new ZipInputStream(File.OpenRead(sourcePath));
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.IsDirectory)
                    {
                        string path = destPath + "\\" + theEntry.Name.Replace("/", @"\\");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                    }
                    else
                    {
                        fileName = destPath + "\\" + theEntry.Name.Replace("/", @"\\");
                        if (prefix != "")
                        {
                            fileName = System.IO.Path.GetDirectoryName(fileName) + "\\" + prefix + System.IO.Path.GetFileName(fileName);
                        }
                        FileStream sw = File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                sw.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        sw.Close();
                        File.SetCreationTime(fileName, theEntry.DateTime);
                    }
                }
                rValue = true;
            }
            catch (Exception ex)
            {
                LogCentral.Current.Error(
                    string.Format(
                    @"解压以下文件 '{0}' 时出现异常 [{1}]",
                    sourcePath,
                    ex.Message));
                rValue = false;
                throw ex;
            }
            finally
            {
                if (s != null)
                    s.Close();
            }
            return rValue;
        }

        
        
        
        
        
        public static string[] GetFileNameListFromZipFile(byte[] zipData)
        {
            System.Collections.Generic.List<string> rValue = new System.Collections.Generic.List<string>();
            System.IO.MemoryStream ms = new MemoryStream(zipData);
            ZipInputStream s = null;
            ZipEntry theEntry;
            string fileName = "";
            try
            {
                s = new ZipInputStream(ms);
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (!theEntry.IsDirectory)
                    {
                        fileName = theEntry.Name.Replace("/", @"\\");
                        rValue.Add(System.IO.Path.GetFileNameWithoutExtension(fileName));
                    }
                }
            }
            catch (Exception ex)
            {
                LogCentral.Current.Error(
                     string.Format(
                     @"从zip压缩包的流中获取文件名列表时出现异常 [{1}]",
                    
                     ex.Message));
                throw ex;
            }
            finally
            {
                if (s != null)
                    s.Close();
            }
            return rValue.ToArray();
        }
    }
}
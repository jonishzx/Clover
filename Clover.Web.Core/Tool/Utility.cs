using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;


using Clover.Core.IO;
using Clover.Core.Common;

namespace Clover.Web.Core
{
    
    
    
    public static class CacheHelper
    {

        
        
        
        public static int Seconds = 600;

        
        
        
        
        public static void CacheThis(HttpResponse response)
        {
            
            
            response.Cache.SetExpires(DateTime.Now.AddSeconds(Seconds));
            response.Cache.SetMaxAge(new TimeSpan(0, 0, Seconds));
            response.Cache.SetCacheability(HttpCacheability.Public);
        
            response.Cache.SetValidUntilExpires(true);

            
            
            response.Cache.SetSlidingExpiration(true);
            response.Cache.SetETagFromFileDependencies();
        }

        
        
        
        public static void ClearPageCache()
        {
            List<string> keys = new List<string>();

            IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();

            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }

            for (int i = 0; i < keys.Count; i++)
            {
                HttpContext.Current.Cache.Remove(keys[i]);
            }
        }
    }

    
    
    
    public class Utility
    {
        public static bool DownloadFile(HttpRequest request, HttpResponse response, string fullPath, string oFileName, int downloadKbps)
        {
            try
            {
                if (fullPath.IndexOf("~") == 0)
                    fullPath = HttpContext.Current.Server.MapPath(fullPath);

                FileInfo finfo = new FileInfo(fullPath);

                FileStream myFile = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(myFile);

                string[] strsplit = fullPath.Split('.');

                try
                {
                    response.AddHeader("Accept-Ranges", "bytes");

                    response.Buffer = false;

                    long fileLength = myFile.Length;

                    long startBytes = 0;

                    int pack = 10240; 

                    

                    
                    int sleep = (int)Math.Floor((double)(1000 * pack / downloadKbps)) + 1;

                    if (request.Headers["Range"] != null) 
                    {
                        response.StatusCode = 206; 
                        string[] range = response.Headers["Range"].Split(new char[] { '=', '-' });
                        startBytes = Convert.ToInt64(range[1]);
                    }

                    response.AddHeader("Content-Length", (fileLength - startBytes).ToString());

                    if (startBytes != 0) 
                    {
                        response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }

                    response.AddHeader("Connection", "Keep-Alive"); 

                    
                    response.ContentType = ConvertFileType(finfo.Extension.Replace(",", ""));

                    response.AppendHeader("Content-Disposition", "attachment;filename=\"" + HttpUtility.UrlEncode(oFileName, System.Text.Encoding.UTF8) + "\"");

                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);

                    int maxCount = (int)Math.Floor((double)((fileLength - startBytes) / pack)) + 1;

                    for (int i = 0; i < maxCount; i++)
                    {
                        if (response.IsClientConnected)
                        {
                            response.BinaryWrite(br.ReadBytes(pack)); 
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            i = maxCount;
                        }
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    br.Close(); 
                    myFile.Close();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        


        
        
        
        
        
        public static void AddAutoIdToTable(string fldname,int startNbr,DataTable dt)
        {
            if(dt.Columns.Contains(fldname))
                throw new ArgumentException(string.Format("{0}字段已经存在表中",fldname));

            dt.Columns.Add(fldname,typeof(System.Int16));


            for (int i = 0; i < dt.Rows.Count;i++ )
            {
                dt.Rows[i][fldname] = startNbr + i;
            }

        }

        public static string GetSessionObject(string key, string defvalue)
        {           
            if (System.Web.HttpContext.Current.Session[key] != null)
                return System.Web.HttpContext.Current.Session[key].ToString();
            else
                return defvalue;
        }

        public static string GetSessionObject(string key)
        {
            return GetSessionObject(key, string.Empty);
        }

        public static string FormatStringLengh(object Content, int lengh)
        {
            Regex reg = new Regex("<(.|\n)*>");
            string temp = Content.ToString();

            if (reg.IsMatch(temp))
            {
                temp = reg.Replace(Content.ToString(), "");
            }

            
            if (temp.Length > lengh)
            {
                return temp.Substring(0, (lengh - 1)) + "..";
            }
            else
            {
                return temp;
            }
        }
       
        public static void SetSessionObject(string key, string value)
        {
            System.Web.HttpContext.Current.Session[key] = value;
        }

        public static void RunClientScript(Page page, string func)
        {
            string strKey = DateTime.Now.ToString();
            if (!page.ClientScript.IsClientScriptBlockRegistered(strKey))
            {
                for (var i = 0; i < 100; i++)
                {
                    strKey += i.ToString();
                    if (!page.ClientScript.IsClientScriptBlockRegistered(strKey))
                        break;
                }
            }

            System.Web.UI.ScriptManager.RegisterClientScriptBlock(page, page.GetType(), strKey, func, true);
        }   

        public static void ScriptAlert(Page page, string message)
        {
            RunClientScript(page, "window.alert('" + message + "');");
        }

        public static void ScriptAlert(Page page, string message, string url)
        {
            RunClientScript(page, "window.alert('" + message + "');window.location='" + url + "';");
        }
    
        
        
        
        
        public static string GetViewerIP()
        {
            
            string result = String.Empty;

            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (string.IsNullOrEmpty(result)) 
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }

            if (string.IsNullOrEmpty(result) || ! StringHelper.IsValidIP(result))
            {
                return "127.0.0.1";
            }

            return result;
        }

        
        
        
        
        
        public static string formatParam(string parameter)
        {
            if (parameter != null)
            {
                return parameter.Replace("'", "''");
            }
            else
            {
                return "";
            }
        }

        public static string FnCent(string s)
        {
            return s.Replace("\r\n", "<br>")
                    .Replace("\r \n", "<br>")
                    .Replace("\n", "<br>")
                    .Replace("\"", "“")
                    .Replace("\r", "<br>");
        }

        
        
        
        
        
        
        
        public static void addOtherDll(ListControl ddl, string Pading, int DirId, DataTable datatable, string idfield, string namefd, string parentfd, int deep, bool addparent)
        {
            if (addparent && ddl.Items.FindByValue(string.Empty) == null)
            {
                DataRow[] parentrows = datatable.Select(idfield + "='" + DirId + "'");
                ListItem li = new ListItem("|--" + parentrows[0][namefd].ToString(), string.Empty);
                ddl.Items.Add(li);
            }

            DataRow[] rowlist = datatable.Select(parentfd + "='" + DirId + "'");
            foreach (DataRow row in rowlist)
            {
                string strPading = "";
                for (int j = 0; j < deep; j++)
                {
                    strPading += "　";         
                }
                
                ListItem li = new ListItem(strPading + "|--" + row[namefd].ToString(), row[idfield].ToString());
                ddl.Items.Add(li);
                
                addOtherDll(ddl, strPading, Convert.ToInt32(row[idfield]), datatable, idfield, namefd, parentfd, deep + 1, addparent);
            }
        }


        
        
        
        
        
        
        public static void FindControlsByType(ControlCollection cc, Type type, List<Control> ctrl)
        {

            foreach (Control c in cc)
            {

                if (c.GetType() == type)
                {
                    ctrl.Add(c);
                }
                else
                    FindControlsByType(c.Controls, type, ctrl);   
            }
        }
 
        
        
        
        
        
        
        public static void FindControlById(ControlCollection cc, string id, ref Control ctrl)
        {

            foreach (Control c in cc)
            {

                if (c.ID == id)
                {
                    ctrl = c;

                    break;
                }
                else
                {
                    if (c.HasControls())
                    {
                        FindControlById(c.Controls, id, ref ctrl);
                    }
                }
            }

        }

        
        
        
        
        
        
        public static bool ExistControlById(ControlCollection cc, string id)
        {

            foreach (Control c in cc)
            {

                if (c.ID == id)
                {
                    return true;

                    break;
                }
                else
                {
                    if (c.HasControls())
                    {
                        return ExistControlById(c.Controls, id);
                    }
                }
            }

            return false;
        }

        
        
        
        
        
        public static bool ValidateEmailAddress(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }


        public static bool CheckInput(string val, string fieldname, bool required, int minlen, int maxlen, ref StringBuilder errorsb)
        {

            if (required && string.IsNullOrEmpty(val))
            {
                errorsb.AppendLine(fieldname + " 为必选字段");
                return false;
            }

            if (minlen != 0 && val.Length < minlen)
            {
                errorsb.AppendLine(fieldname + string.Format(" 需要输入一个长度最少是 {0} 的字符串", minlen));
                return false;
            }

            if (maxlen != 0 && val.Length > maxlen)
            {
                errorsb.AppendLine(fieldname + string.Format(" 请输入一个长度最多是 {0} 的字符串", maxlen));
                return false;
            }

            if (maxlen != 0 && maxlen != 0 && val.Length > maxlen && val.Length < minlen)
            {
                errorsb.AppendLine(fieldname + string.Format("请输入一个长度介于 {0} 和 {1} 之间的字符串", minlen, maxlen));
                return false;
            }
            return true;
        }

        public static string GetParm(string pname, string defaultVal)
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString[pname]))
                return HttpContext.Current.Request.QueryString[pname];
            else
                return defaultVal;
        }

        public static string GetFormParm(string pname, string defaultVal)
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form[pname]))
                return HttpContext.Current.Request.Form[pname];
            else
                return defaultVal;
        }

        public static string GetParm(string pname)
        {
            return GetParm(pname, "");
        }

        public static string GetFormParm(string pname)
        {
            return GetFormParm(pname, "");
        }

        public static int GetIntParm(string pname)
        {
            return GetIntParm(pname, 0);
        }

        public static int GetIntParm(string pname, int defaultVal)
        {
            string val = GetParm(pname);
            int rtn = 0;

            int.TryParse(val, out rtn);
            if (rtn == 0)
                rtn = defaultVal;
            return rtn;
        }
        public static int GetFormIntParm(string pname, int defaultVal)
        {
            string val = GetFormParm(pname);
            int rtn = 0;

            int.TryParse(val, out rtn);
            if (rtn == 0)
                rtn = defaultVal;
            return rtn;
        }
        public static int GetFormIntParm(string pname)
        {
            return GetFormIntParm(pname, 0);
        }


        
        
        
        
        
        public static void SetDDLSelectByValue(ListControl dl, string val)
        {
            
            foreach (ListItem it in dl.Items)
            {
                it.Selected = false;                 
            }
            foreach (ListItem it in dl.Items)
            {
                if (it.Value == val)
                {
                    it.Selected = true;
                    break;
                }
                else
                {
                    it.Selected = false;
                }
            }
        }

        
        
        
        
        
        public static void SetDDLSelectByText(ListControl dl, string text)
        {

            foreach (ListItem it in dl.Items)
            {
                it.Selected = false;
            }
            foreach (ListItem it in dl.Items)
            {
                if (it.Text == text)
                {
                    it.Selected = true;
                    break;
                }
                else
                {
                    it.Selected = false;
                }
            }
        }

          
        
        
        
        
        public static void SetCBLSelectByValue(ListControl dl, string val)
        {
            SetCBLSelectByValue(dl,val,",");
        }

        
        
        
        
        
        
        public static void SetCBLSelectByValue(ListControl dl, string val, string splitter)
        {
            List<string> vals = new List<string>(StringHelper.SplitString(val, splitter));
       
            foreach (ListItem it in dl.Items)
            {
                if (vals.Contains(it.Value))
                {
                    it.Selected = true;
                }
                else {
                    it.Selected = false;
                }
            }
        }


        
        
        
        
        
        public static bool IsListControlAllChecked(ListControl dl)
        {
            bool returnval = true;
            for (int i = 0; i < dl.Items.Count; i++)
            {
                if (!dl.Items[i].Selected)
                {
                    returnval = false;
                    break;
                }

            }
            return returnval;
        }

        
        
        
        
        
        public static string GetListControlCheckedItemString(ListControl dl)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < dl.Items.Count; i++)
            {
                if (dl.Items[i].Selected)
                {
                    sb.Append(dl.Items[i].Value);
                    sb.Append(",");
                }

            }
            return sb.ToString();
        }

        
        
        
        
        
        public static List<string> GetListControlCheckedItemList(ListControl dl)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < dl.Items.Count; i++)
            {
                if (dl.Items[i].Selected)
                {
                    list.Add(dl.Items[i].Value);
                }

            }
            return list;
        }

        
        
        
        
        
        
        
        public static List<string> GetCheckedItemFormRepeater(Repeater rp, string ckid, string valctrlId)
        {
            List<string> list = new List<string>();
            foreach (RepeaterItem rpit in rp.Items)
            {
                CheckBox chkbox = rpit.FindControl(ckid) as CheckBox;
                Label lb = rpit.FindControl(valctrlId) as Label;
                if (chkbox != null && lb != null)
                {
                    if (chkbox.Checked)
                    {
                        list.Add(lb.Text);
                    }
                }
            }
            return list;
        }

        
        
        
        
        
        public static string ConvertAbsoulteUrl(string relativeUrl)
        {
            string rtnurl = relativeUrl;

            if (relativeUrl.IndexOf("?") >= 0)
            {
                string url = System.Text.RegularExpressions.Regex.Match(relativeUrl, @"(\S)*\?").Value.Replace("?", "");
                string query = System.Text.RegularExpressions.Regex.Replace(relativeUrl, @"(\S)*\?", "");

                rtnurl = VirtualPathUtility.ToAbsolute(url) + "?" + query;
            }
            else
            {
                try
                {
                    rtnurl = VirtualPathUtility.ToAbsolute(relativeUrl);
                }
                catch
                {
                }

            }
            return rtnurl;
        }

        
        
        
        
        
        public static string ConvertPsyPath(string strPath)
        {
            string output = string.Empty;

            if (HttpContext.Current != null || strPath.IndexOf("~")==0)
            {
                try{
                    output =  HttpContext.Current.Server.MapPath(strPath);
                }catch{}
            }

            if (System.IO.File.Exists(output) || System.IO.Directory.Exists(output))
            {
                return output;
            }
            else 
            {
                output = strPath.Replace("/", "\\");
                if (output.StartsWith("\\"))
                {
                    output = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
                }
                output = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, output);
            }

            return output;
        }

        #region  生成缩略图
        
        
        
        
        
        
        
        
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode,string watermark, out string outthumbnailPath)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
        

            switch (mode)
            {
                case "HW":
                    break;
                case "W":
                    toheight =  (int)Math.Round((decimal)(originalImage.Height * width / originalImage.Width));
                    break;
                case "H":
                    towidth = (int)Math.Round((decimal)(originalImage.Width * height / originalImage.Height));
                    break;
                case "R":
                    if (ow > oh)
                    {
                        
                        toheight = (int)Math.Round((decimal)originalImage.Height * toheight / originalImage.Width);    
                    }
                    else
                    {
                        
                        towidth = (int)Math.Round((decimal)originalImage.Width * toheight / originalImage.Height);                                                                
                    }
                    break;
                case "Cut":
                    if (ow * toheight > towidth * oh)
                    {
                        
                        oh = toheight;
                        ow = (int)Math.Round((decimal)originalImage.Width * towidth / originalImage.Height);

                        y = 0;
                        x = (int)Math.Round((decimal)(originalImage.Width - ow) / 2);
                    }
                    else
                    {
                        
                        ow = towidth;
                        oh = (int)Math.Round((decimal)originalImage.Height * towidth / originalImage.Width);

                        x = 0;
                        y = (int)Math.Round((decimal)(originalImage.Height - oh) / 2);
                    }
                    break;
                default:
                    break;
            }

            
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            
            g.Clear(Color.White);
            
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            
            if (!string.IsNullOrEmpty(watermark))
                g.DrawString(watermark, new Font("Courier New", 11), new SolidBrush(Color.LightGray), 0, toheight - 30);


            try
            {
                
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                outthumbnailPath = thumbnailPath;
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
        #endregion

        #region 内容类型标记转换
        
        
        
        
        
        public static string ConvertFileType(string ext)
        {
            string contenttype = string.Empty;
            if (ext.IndexOf(".") == 0)
                ext = ext.TrimStart('.');

            switch (ext)
            {
                case "ez": contenttype = "application/andrew-inset"; break;
                case "hqx": contenttype = "application/mac-binhex40"; break;
                case "cpt": contenttype = "application/mac-compactpro"; break;
                case "doc": contenttype = "application/msword"; break;
                case "bin": contenttype = "application/octet-stream"; break;
                case "dms": contenttype = "application/octet-stream"; break;
                case "lha": contenttype = "application/octet-stream"; break;
                case "lzh": contenttype = "application/octet-stream"; break;
                case "exe": contenttype = "application/octet-stream"; break;
                case "class": contenttype = "application/octet-stream"; break;
                case "so": contenttype = "application/octet-stream"; break;
                case "dll": contenttype = "application/octet-stream"; break;
                case "oda": contenttype = "application/oda"; break;
                case "pdf": contenttype = "application/pdf"; break;
                case "ai": contenttype = "application/postscript"; break;
                case "eps": contenttype = "application/postscript"; break;
                case "ps": contenttype = "application/postscript"; break;
                case "smi": contenttype = "application/smil"; break;
                case "smil": contenttype = "application/smil"; break;
                case "mif": contenttype = "application/vnd.mif"; break;
                case "xls": contenttype = "application/vnd.ms-excel"; break;
                case "ppt": contenttype = "application/vnd.ms-powerpoint"; break;
                case "wbxml": contenttype = "application/vnd.wap.wbxml"; break;
                case "wmlc": contenttype = "application/vnd.wap.wmlc"; break;
                case "wmlsc": contenttype = "application/vnd.wap.wmlscriptc"; break;
                case "bcpio": contenttype = "application/x-bcpio"; break;
                case "vcd": contenttype = "application/x-cdlink"; break;
                case "pgn": contenttype = "application/x-chess-pgn"; break;
                case "cpio": contenttype = "application/x-cpio"; break;
                case "csh": contenttype = "application/x-csh"; break;
                case "dcr": contenttype = "application/x-director"; break;
                case "dir": contenttype = "application/x-director"; break;
                case "dxr": contenttype = "application/x-director"; break;
                case "dvi": contenttype = "application/x-dvi"; break;
                case "spl": contenttype = "application/x-futuresplash"; break;
                case "gtar": contenttype = "application/x-gtar"; break;
                case "hdf": contenttype = "application/x-hdf"; break;
                case "js": contenttype = "application/x-javascript"; break;
                case "skp": contenttype = "application/x-koan"; break;
                case "skd": contenttype = "application/x-koan"; break;
                case "skt": contenttype = "application/x-koan"; break;
                case "skm": contenttype = "application/x-koan"; break;
                case "latex": contenttype = "application/x-latex"; break;
                case "nc": contenttype = "application/x-netcdf"; break;
                case "cdf": contenttype = "application/x-netcdf"; break;
                case "sh": contenttype = "application/x-sh"; break;
                case "shar": contenttype = "application/x-shar"; break;
                case "swf": contenttype = "application/x-shockwave-flash"; break;
                case "sit": contenttype = "application/x-stuffit"; break;
                case "sv4cpio": contenttype = "application/x-sv4cpio"; break;
                case "sv4crc": contenttype = "application/x-sv4crc"; break;
                case "tar": contenttype = "application/x-tar"; break;
                case "tcl": contenttype = "application/x-tcl"; break;
                case "tex": contenttype = "application/x-tex"; break;
                case "texinfo": contenttype = "application/x-texinfo"; break;
                case "texi": contenttype = "application/x-texinfo"; break;
                case "t": contenttype = "application/x-troff"; break;
                case "tr": contenttype = "application/x-troff"; break;
                case "roff": contenttype = "application/x-troff"; break;
                case "man": contenttype = "application/x-troff-man"; break;
                case "me": contenttype = "application/x-troff-me"; break;
                case "ms": contenttype = "application/x-troff-ms"; break;
                case "ustar": contenttype = "application/x-ustar"; break;
                case "src": contenttype = "application/x-wais-source"; break;
                case "xhtml": contenttype = "application/xhtml+xml"; break;
                case "xht": contenttype = "application/xhtml+xml"; break;
                case "zip": contenttype = "application/zip"; break;
                case "au": contenttype = "audio/basic"; break;
                case "snd": contenttype = "audio/basic"; break;
                case "mid": contenttype = "audio/midi"; break;
                case "midi": contenttype = "audio/midi"; break;
                case "kar": contenttype = "audio/midi"; break;
                case "mpga": contenttype = "audio/mpeg"; break;
                case "mp2": contenttype = "audio/mpeg"; break;
                case "mp3": contenttype = "audio/mpeg"; break;
                case "mp4": contenttype = "video/mp4"; break;
                case "flv": contenttype = "video/x-flv"; break;
                case "vob": contenttype = "video/vob"; break;
                case "aif": contenttype = "audio/x-aiff"; break;
                case "aiff": contenttype = "audio/x-aiff"; break;
                case "aifc": contenttype = "audio/x-aiff"; break;
                case "m3u": contenttype = "audio/x-mpegurl"; break;
                case "ram": contenttype = "audio/x-pn-realaudio"; break;
                case "rm": contenttype = "audio/x-pn-realaudio"; break;
                case "rmvb": contenttype = "audio/x-pn-realaudio"; break;
                case "rpm": contenttype = "audio/x-pn-realaudio-plugin"; break;
                case "ra": contenttype = "audio/x-realaudio"; break;
                case "wav": contenttype = "audio/x-wav"; break;
                case "pdb": contenttype = "chemical/x-pdb"; break;
                case "xyz": contenttype = "chemical/x-xyz"; break;
                case "bmp": contenttype = "image/bmp"; break;
                case "gif": contenttype = "image/gif"; break;
                case "ief": contenttype = "image/ief"; break;
                case "jpeg": contenttype = "image/jpeg"; break;
                case "jpg": contenttype = "image/jpeg"; break;
                case "jpe": contenttype = "image/jpeg"; break;
                case "png": contenttype = "image/png"; break;
                case "tiff": contenttype = "image/tiff"; break;
                case "tif": contenttype = "image/tiff"; break;
                case "djvu": contenttype = "image/vnd.djvu"; break;
                case "djv": contenttype = "image/vnd.djvu"; break;
                case "wbmp": contenttype = "image/vnd.wap.wbmp"; break;
                case "ras": contenttype = "image/x-cmu-raster"; break;
                case "pnm": contenttype = "image/x-portable-anymap"; break;
                case "pbm": contenttype = "image/x-portable-bitmap"; break;
                case "pgm": contenttype = "image/x-portable-graymap"; break;
                case "ppm": contenttype = "image/x-portable-pixmap"; break;
                case "rgb": contenttype = "image/x-rgb"; break;
                case "xbm": contenttype = "image/x-xbitmap"; break;
                case "xpm": contenttype = "image/x-xpixmap"; break;
                case "xwd": contenttype = "image/x-xwindowdump"; break;
                case "igs": contenttype = "model/iges"; break;
                case "iges": contenttype = "model/iges"; break;
                case "msh": contenttype = "model/mesh"; break;
                case "mesh": contenttype = "model/mesh"; break;
                case "silo": contenttype = "model/mesh"; break;
                case "wrl": contenttype = "model/vrml"; break;
                case "vrml": contenttype = "model/vrml"; break;
                case "css": contenttype = "text/css"; break;
                case "html": contenttype = "text/html"; break;
                case "htm": contenttype = "text/html"; break;
                case "asc": contenttype = "text/plain"; break;
                case "txt": contenttype = "text/plain"; break;
                case "rtx": contenttype = "text/richtext"; break;
                case "rtf": contenttype = "text/rtf"; break;
                case "sgml": contenttype = "text/sgml"; break;
                case "sgm": contenttype = "text/sgml"; break;
                case "tsv": contenttype = "text/tab-separated-values"; break;
                case "wml": contenttype = "text/vnd.wap.wml"; break;
                case "wmls": contenttype = "text/vnd.wap.wmlscript"; break;
                case "etx": contenttype = "text/x-setext"; break;
                case "xsl": contenttype = "text/xml"; break;
                case "xml": contenttype = "text/xml"; break;
                case "mpeg": contenttype = "video/mpeg"; break;
                case "mpg": contenttype = "video/mpeg"; break;
                case "mpe": contenttype = "video/mpeg"; break;
                case "qt": contenttype = "video/quicktime"; break;
                case "mov": contenttype = "video/quicktime"; break;
                case "mxu": contenttype = "video/vnd.mpegurl"; break;
                case "avi": contenttype = "video/x-msvideo"; break;
                case "movie": contenttype = "video/x-sgi-movie"; break;
                case "ice": contenttype = "x-conference/x-cooltalk"; break;
                default:
                    contenttype = "application/octet-stream";
                    break;

            }

            return contenttype;

        }

        #endregion
        #region 字符串转换为数据表
        
        
        
        
        
        
        
        public static DataTable StrToDataTable(string srcStr, string rowFlag, string colFlag)
        {
            string[] rows = srcStr.Split(new string[] { rowFlag}, StringSplitOptions.RemoveEmptyEntries);
            if(rows.Length > 0)
            {
                string[] cols = rows[0].Split(new string[] { colFlag }, StringSplitOptions.RemoveEmptyEntries);

                DataTable dt = new DataTable();

                for (int i = 0; i < cols.Length; i++)
                {
                    dt.Columns.Add("col" + i.ToString());
                }

                foreach (string rowstr in rows)
                {
                    DataRow dr = dt.NewRow();
                    cols = rowstr.Split(new string[] { colFlag }, StringSplitOptions.RemoveEmptyEntries);
                    for(int j=0;j<cols.Length && j<dt.Columns.Count;j++)
                    {
                        dr[j] = cols[j];
                    }

                    dt.Rows.Add(dr);
                }

                return dt;
            }

            return null;
            
        }
        #endregion
    }    
}
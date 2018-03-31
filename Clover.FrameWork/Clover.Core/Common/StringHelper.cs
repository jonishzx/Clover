namespace Clover.Core.Common
{
    #region 使用的命名空间.
    

    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;
    using Clover.Core.Logging;
    using Clover.Core.Collection;
    using Clover.Core.Properties;

    
    #endregion

    [Flags]
    
    
    
    public enum StringJoinOption
    {
        
        
        
        None = 0,
        
        
        
        RemoveEmpty = 1,

        
        
        
        Reverse = 2,

        
        
        
        NoLastJoinFlag = 4,

        
        
        
        CheckStringCombieChar = 8
    }
    

    
    
    
    public partial class StringHelper
    {
        
        
        
        
        
        public static Boolean IsEmpty(String target)
        {
            return string.IsNullOrEmpty(target) || target.Trim().Length == 0;
        }

        
        
        
        public static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        
        
        
        
        
        public static String addEndSlash(String appPath)
        {
            if (!appPath.EndsWith("/")) return appPath + "/";
            return appPath;
        }

        #region 特殊的字符串转换.
        

        
        
        
        
        
        
        
        public static string SerializeToString(
            object o)
        {
            if (o == null)
            {
                return null;
            }
            else
            {
                StringDictionary dic = o as StringDictionary;

                
                if (dic != null)
                {
                    Hashtable ht = new Hashtable();

                    foreach (string key in dic.Keys)
                    {
                        ht[key] = dic[key];
                    }

                    
                    ht[stringDictionaryKey] = true;

                    o = ht;
                }

                IFormatter f = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    f.Serialize(stream, o);

                    byte[] buf = stream.ToArray();
                    return Convert.ToBase64String(buf);
                }
            }
        }

        
        
        
        private const string stringDictionaryKey =
            @"__is_StringDictionary_wrapper__";

        
        
        
        
        
        public static object DeserializeFromString(
            string s)
        {
            return DeserializeFromString(
                s,
                true);
        }

        
        
        
        
        
        
        public static object DeserializeFromString(
            string s,
            bool ignoreSerializationExceptions)
        {
            if (s == null || s.Length <= 0)
            {
                return null;
            }
            else
            {
                try
                {
                    IFormatter f = new BinaryFormatter();

                    using (MemoryStream stream = new MemoryStream(
                        Convert.FromBase64String(s)))
                    {
                        object o = f.Deserialize(stream);

                        
                        if (o is Hashtable)
                        {
                            Hashtable ht = o as Hashtable;

                            if (ht.ContainsKey(stringDictionaryKey))
                            {
                                ht.Remove(stringDictionaryKey);

                                StringDictionary dic =
                                    new StringDictionary();

                                foreach (string key in ht.Keys)
                                {
                                    object ob = ht[key];

                                    if (ob == null)
                                    {
                                        dic[key] = null;
                                    }
                                    else
                                    {
                                        dic[key] = ob.ToString();
                                    }
                                }

                                return dic;
                            }
                            else
                            {
                                return o;
                            }
                        }
                        else
                        {
                            return o;
                        }
                    }
                }
                catch (FormatException x)
                {
                    if (ignoreSerializationExceptions)
                    {
                        
                        
                        LogCentral.Current.Warn(
                            string.Format(
                            @" 反序列化时发生 FormatException" +
                            @"字符  {0} 字符 长度 ('{1}'). " +
                            @"返回 NULL.",
                            s.Length,
                            s), x);

                        return null;
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (SerializationException x)
                {
                    if (ignoreSerializationExceptions)
                    {

                        LogCentral.Current.Warn(
                            string.Format(
                            @"反序列化时发生SerializationException " +
                            @"字符 {0} 字符 长度 ('{1}'). " +
                            @"返回 NULL.",
                            s.Length,
                            s), x);

                        return null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        
        #endregion

        #region 杂项 程序.
        

        
        
        
        
        
        
        
        
        public static string GetEnumDescription(
            Enum value)
        {
            string result;
            if (recentEnumDescriptions.TryGetValue(value, out result))
            {
                return result;
            }
            else
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());

                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

                if (attributes != null &&
                    attributes.Length > 0)
                {
                    result = attributes[0].Description;
                }
                else
                {
                    result = value.ToString();
                }

                recentEnumDescriptions[value] = result;
                return result;
            }
        }

        
        
        
        
        
        public static string GenerateHash(
            string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            else
            {
                return input.GetHashCode().ToString(
                    @"X",
                    CultureInfo.InvariantCulture);
            }
        }

        
        
        
        
        
        public static string FormatFileSize(
            int fileSize)
        {
            return FormatFileSize((long)fileSize);
        }

        
        
        
        
        
        public static string FormatFileSize(
            long fileSize)
        {
            const long fileSize1KB = 1024;
            const long fileSize100KB = 102400;
            const long fileSize1MB = 1048576;
            const long fileSize1GB = 1073741824;
            const long fileSize1TB = 1099511627776;

            if (fileSize < fileSize1KB)
            {
                return string.Format(
                    Thread.CurrentThread.CurrentCulture,
                    @"{0} {1}",
                    fileSize,
                    Resources.Str_FormatFileSize_Bytes);
            }
            else if (fileSize < fileSize100KB)
            {
                return string.Format(
                    Thread.CurrentThread.CurrentCulture,
                    @"{0:F1} {1}",
                    (double)fileSize / (double)fileSize1KB,
                    Resources.Str_FormatFileSize_KB);
            }
            else if (fileSize < fileSize1MB)
            {
                return string.Format(
                    Thread.CurrentThread.CurrentCulture,
                    @"{0} {1}",
                    fileSize / fileSize1KB,
                    Resources.Str_FormatFileSize_KB);
            }
            else if (fileSize < fileSize1GB)
            {
                return string.Format(
                    Thread.CurrentThread.CurrentCulture,
                    @"{0:F1} {1}",
                    (double)fileSize / (double)fileSize1MB,
                    Resources.Str_FormatFileSize_MB);
            }
            else if (fileSize < fileSize1TB)
            {
                return string.Format(
                    Thread.CurrentThread.CurrentCulture,
                    @"{0:F2} {1}",
                    (double)fileSize / (double)fileSize1GB,
                    Resources.Str_FormatFileSize_GB);
            }
            else
            {
                return string.Format(
                    Thread.CurrentThread.CurrentCulture,
                    @"{0:F2} {1}",
                    (double)fileSize / (double)fileSize1TB,
                    Resources.Str_FormatFileSize_TB);
            }
        }

        
        
        
        
        
        
        public static string EscapeRXCharacters(
            string text,
            params char[] ignoreChars)
        {
            Set<char> ignores = new Set<char>(ignoreChars);

            
            if (!ignores.Contains('\\'))
            {
                text = text.Replace(@"\", @"\\");
            }

            if (!ignores.Contains('+'))
            {
                text = text.Replace(@"+", @"\+");
            }
            if (!ignores.Contains('+'))
            {
                text = text.Replace(@"?", @"\?");
            }
            if (!ignores.Contains('.'))
            {
                text = text.Replace(@".", @"\.");
            }
            if (!ignores.Contains('*'))
            {
                text = text.Replace(@"*", @"\*");
            }
            if (!ignores.Contains('^'))
            {
                text = text.Replace(@"^", @"\^");
            }
            if (!ignores.Contains('$'))
            {
                text = text.Replace(@"$", @"\$");
            }
            if (!ignores.Contains('('))
            {
                text = text.Replace(@"(", @"\(");
            }
            if (!ignores.Contains(')'))
            {
                text = text.Replace(@")", @"\)");
            }
            if (!ignores.Contains('['))
            {
                text = text.Replace(@"[", @"\[");
            }
            if (!ignores.Contains(']'))
            {
                text = text.Replace(@"]", @"\]");
            }
            if (!ignores.Contains('{'))
            {
                text = text.Replace(@"{", @"\{");
            }
            if (!ignores.Contains('}'))
            {
                text = text.Replace(@"}", @"\}");
            }
            if (!ignores.Contains('|'))
            {
                text = text.Replace(@"|", @"\|");
            }

            return text;
        }


        
        
        
        
        
        
        public static string[] SplitExtended(
            string s,
            string separator)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            else
            {
                List<string> list = new List<string>();

                int lastPos = 0;
                int pos = 0;
                while ((pos = s.IndexOf(separator, lastPos)) >= 0)
                {
                    list.Add(s.Substring(lastPos, pos - lastPos + 1)); 

                    lastPos = pos + separator.Length; 
                }

                if (lastPos < s.Length - 1) 
                {
                    list.Add(s.Substring(lastPos));
                }

                return list.ToArray();
            }
        }

        
        
        
        
        
        
        public static string[] SplitExtended(
            string s,
            params char[] separator)
        {
            string[] t = s.Split(separator);

            
            List<string> list = new List<string>();
            foreach (string u in t)
            {
                if (u.Length > 0)
                    list.Add(u);
            }

            return list.ToArray();
        }


        
        
        
        
        
        
        
        
        public static string AddZerosPrefix(
            object text,
            int length)
        {
            string s = Convert.ToString(text);

            if (!string.IsNullOrEmpty(s) && s.Length < length)
            {
                StringBuilder sb = new StringBuilder(s);
                sb.Append('0', length - s.Length);

                return sb.ToString();
            }
            else
            {
                return s;
            }
        }

        
        
        
        
        
        
        public static string Left(
            string s,
            int count)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            else
            {
                return s.Substring(0, Math.Min(count, s.Length));
            }
        }

        
        
        
        
        
        
        public static string Right(
            string s,
            int count)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            else
            {
                int length = s.Length;

                if (s.Length <= count)
                {
                    return s;
                }
                else
                {
                    return s.Substring(length - count);
                }
            }
        }

        
        #endregion

        #region 常用的格式检查方法
        

        
        
        
        
        
        public static bool IsValidEmail(string matchcode)
        {
            return RXTest(matchcode, RegexResource.RegxForEmail, "m");
        }

        
        
        
        
        
        public static bool IsValidIP(string matchcode)
        {
            return RXTest(matchcode, RegexResource.RegxForIP, "m");
        }

        
        
        
        
        
        public static bool IsValidUrl(string matchcode)
        {
            return RXTest(matchcode, RegexResource.RegxForUrl, "m");
        }

        
        
        
        
        
        public static bool IsValidAccount(string matchcode)
        {
            return RXTest(matchcode, RegexResource.RegxForAccount, "m");
        }

        
        
        
        
        
        public static bool IsValidChinese(string matchcode)
        {
            return RXTest(matchcode, RegexResource.RegxForChinese, "m");
        }



        
        
        
        
        
        public static int GetUnicodeStrLength(string matchcode)
        {
            return System.Text.UnicodeEncoding.Unicode.GetBytes(matchcode).Length;
        }


        
        
        
        
        
        public static int GetUTF8StrLength(string matchcode)
        {
            return System.Text.UTF8Encoding.UTF8.GetBytes(matchcode).Length;
        }


        
        #endregion

        #region 正则表达式方法.
        

        
        
        
        
        
        
        
        
        
        public static string RXReplace(
            string text,
            string pattern,
            string replacement,
            string flags)
        {
            RegexOptions options =
                ConvertRXOptionsFromString(flags);

            Regex rx = new Regex(pattern, options);
            if (flags.Contains(@"g"))
            {
                return rx.Replace(text, replacement);
            }
            else
            {
                return rx.Replace(text, replacement, 1);
            }
        }

        
        
        
        
        
        
        
        public static bool RXTest(
            string text,
            string pattern,
            string flags)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            else
            {
                RegexOptions options =
                    ConvertRXOptionsFromString(flags);

                Regex rx = new Regex(
                    pattern,
                    options);
                return rx.IsMatch(text);
            }
        }

        
        
        
        
        
        
        
        public static int RXTestCount(
            string text,
            string pattern,
            string flags)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }
            else
            {
                RegexOptions options =
                    ConvertRXOptionsFromString(flags);

                Regex rx = new Regex(
                    pattern,
                    options);
                return rx.Matches(text).Count;
            }
        }

        
        
        
        
        
        public static RegexOptions ConvertRXOptionsFromString(
            string flags)
        {
            RegexOptions options;

            if (recentRegexOptions.TryGetValue(flags, out options))
            {
                return options;
            }
            else
            {
                options = RegexOptions.None;

                if (flags.Contains(@"i"))
                {
                    options |= RegexOptions.IgnoreCase;
                }
                if (flags.Contains(@"x"))
                {
                    options |= RegexOptions.IgnorePatternWhitespace;
                }
                if (flags.Contains(@"m"))
                {
                    options |= RegexOptions.Multiline;
                }
                if (flags.Contains(@"s"))
                {
                    options |= RegexOptions.Singleline;
                }

                recentRegexOptions[flags] = options;
                return options;
            }
        }

        
        
        
        
        
        public static Dictionary<string, List<string>> GetKeyValueFormString(string source)
        {
            Dictionary<string, List<string>> rst = new Dictionary<string, List<string>>(10);

            string pair = "((?:\\\\.|[^=,]+)*)=(\"(?:\\\\.|[^\"\\\\]+)*\"|(?:\\\\.|[^,\"\\\\]+)*)";
            
            var matches = Regex.Matches(source, pair,
    RegexOptions.IgnoreCase
    | RegexOptions.CultureInvariant
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled);

            foreach (Match match in matches) { 
                string[] vals = match.Value.Split(new char[]{'='}, StringSplitOptions.RemoveEmptyEntries);

                if (rst.ContainsKey(vals[0])) {
                    rst.Add(vals[0], 
                        new List<string>(SplitString(Regex.Replace(vals[1],"^(\\\")|(\\\")$", ""), ",")));
                }
            }

            return rst;
        }
        
        #endregion

        #region 私有方法.
        

        
        
        
        private static Dictionary<Enum, string> recentEnumDescriptions =
            new Dictionary<Enum, string>();

        
        
        
        private static Dictionary<string, RegexOptions> recentRegexOptions =
            new Dictionary<string, RegexOptions>();

        
        #endregion

        
        
        
        
        
        
        
        public static string[] SplitString(string input, string strSplit)
        {

            if (!string.IsNullOrEmpty(input))
            {
                if (input.IndexOf(strSplit) < 0)
                {
                    string[] tmp = { input };
                    return tmp;
                }
                return Regex.Split(input, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
            {
                return new string[0] { };
            }
        }

        
        
        
        
        
        
        public static string Join(string combieChar, params string[] strparams)
        {
            return Join(combieChar, string.Empty, StringJoinOption.RemoveEmpty, strparams);
        }

        
        
        
        
        
        
        
        public static string Join(string combieChar, StringJoinOption options, params string[] strparams)
        {
            return Join(combieChar, string.Empty, options, strparams);
        }

        
        
        
        
        
        
        
        
        public static string Join(string combieChar, string cSymbol, StringJoinOption options, params string[] strparams)
        {
            string[] rst = new string[strparams.Length];
            Array.Copy(strparams, rst, strparams.Length);

            
            if ((options & StringJoinOption.Reverse) != 0)
                Array.Reverse(rst);

            bool removeEmpty = (options & StringJoinOption.RemoveEmpty) != 0;
            bool noLastJoinflag = (options & StringJoinOption.NoLastJoinFlag) != 0;
            bool checkstringcombiechar = (options & StringJoinOption.CheckStringCombieChar) != 0;
            bool cSymbolEmpty = IsEmpty(cSymbol);

            if (options == StringJoinOption.None || (!removeEmpty && !noLastJoinflag && !checkstringcombiechar))
            {
                return string.Join(combieChar, rst) + combieChar;
            }
            else if (cSymbolEmpty && options == StringJoinOption.NoLastJoinFlag)
            {
                return string.Join(combieChar, rst);
            }
            else
            {
                List<string> list = new List<string>(strparams);
                StringBuilder sb = new StringBuilder(strparams.Length * 10);
                for (int i = 0; i < rst.Length; i++)
                {
                    if (!(removeEmpty && IsEmpty(rst[i])))
                    {
                        if (!cSymbolEmpty)
                        {
                            sb.Append(cSymbol);
                            sb.Append(rst[i]);
                            sb.Append(cSymbol);
                        }
                        else
                        {
                            sb.Append(rst[i]);
                        }

                        
                        if (!(checkstringcombiechar && cSymbolEmpty && rst[i].EndsWith(combieChar)))
                        {
                            sb.Append(combieChar);
                        }
                    }
                }

                if (noLastJoinflag)
                    sb.Remove(sb.Length - 1, combieChar.Length);

                return sb.ToString();
            }

        }

        
        
        
        
        
        
        public static bool InIpArea(string ip, string[] iparray)
        {
            string[] userip = SplitString(ip, @".");

            for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                string[] tmpip = SplitString(iparray[ipIndex], @"-"); 
                string[] tmpip1 = SplitString(tmpip[0], @"."); 
                string[] tmpip2 = SplitString(tmpip[1], @".");
                int r = 0;
                for (int i = 0; i < tmpip1.Length; i++)
                {

                    
                    if (int.Parse(userip[i]) < int.Parse(tmpip1[i]) || int.Parse(userip[i]) > int.Parse(tmpip2[i]))
                    {
                        break;
                    }
                    else if (int.Parse(userip[i]) > int.Parse(tmpip1[i]) && int.Parse(userip[i]) < int.Parse(tmpip2[i])) 
                    {
                        return true;
                    }
                    else
                    {
                        r++;
                    }
                }
                if (r == 4)
                {
                    return true;
                }
            }
            return false;
        }

        
        
        
        
        
        public static string NoHTML(string Htmlstring)
        {

            

            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            

            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"([rn])[s]+", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "xa1", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "xa2", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "xa3", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");

            Htmlstring.Replace(">", "");

            Htmlstring.Replace("rn", "");

            Htmlstring = System.Web.HttpUtility.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;

        }

        
        
        
        
        
        public static string FmtDecimal(int val)
        {
            return val.ToString("###,###");
        }
        
        
        
        
        
        public static string FmtDecimal(decimal val, int digitCount)
        {
            return val.ToString("###,##0" + (digitCount > 0 ? "." + "".PadLeft(digitCount, '0') : ""));
        }
        
        
        
        
        
        public static string FmtDecimal(double val, int digitCount)
        {
            return val.ToString("###,##0" + (digitCount > 0 ? "." + "".PadLeft(digitCount, '0') : ""));

        }
        
        
        
        
        
        public static decimal GetFmtDecimal(string val)
        {
            return decimal.Parse(val, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
        }
        
        
        
        
        
        public static int GetFmtInt(string val)
        {
            return int.Parse(val, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
        }

    }

    
}
namespace Clover.Core.Common
{
    #region 引用的控件.
    

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Globalization;
    using System.Text;

    
    #endregion

    

    
    
    
    public class ConvertHelper
    {
        #region 各式各样的转换方法.
        

        
        
        
        
        
        public static CultureInfo ToCultureInfo(
            string languageCode)
        {
            return ToCultureInfo(
                languageCode,
                CultureInfo.InvariantCulture);
        }

        
        
        
        
        
        
        public static CultureInfo ToCultureInfo(
            string languageCode,
            CultureInfo fallbackTo)
        {
            if (string.IsNullOrEmpty(languageCode) ||
                languageCode.Trim().Length < 2)
            {
                return fallbackTo;
            }
            else
            {
                string c4;
                string c2;

                if (languageCode.Length == 2)
                {
                    c2 = languageCode;
                    c4 = c2 + @"-" + c2;
                }
                else if (languageCode.Length == 4)
                {
                    c2 = languageCode.Substring(0, 2);
                    c4 = languageCode;
                }
                else
                {
                    c2 = languageCode.Substring(0, 2);
                    c4 = c2 + @"-" + c2;
                }

                try
                {
                    CultureInfo info = new CultureInfo(
                        c4);
                    return info;
                }
                catch (ArgumentException)
                {
                    try
                    {
                        
                        CultureInfo info = new CultureInfo(
                            c2);
                        return info;
                    }
                    catch 
                        
                    {
                        
                        
                        
                        
                        
                      
                        return fallbackTo;
                    }
                }
            }
        }

        
        
        
        
        
        
        
        public static string ToString(
            byte[] buffer)
        {
            if (buffer == null)
            {
                return null;
            }
            else if (buffer.Length <= 0)
            {
                return null;
            }
            else
            {
                StringBuilder s = new StringBuilder();

                foreach (byte b in buffer)
                {
                    if (s.Length > 0)
                    {
                        s.Append(@"-");
                    }

                    s.Append(Convert.ToString(b));
                }

                return s.ToString();
            }
        }

        
        #endregion

        #region 有默认输出的转换方法
        

        
        
        
        
        
        public static T ToT<T>(
            object o)
        {
            return ToT<T>(o, default(T));
        }

        
        
        
        
        
        public static string ToString(
            object o)
        {
            return ToString(o, (string)null);
        }

        
        
        
        
        
        
        public static string ToString(
            object o,
            IFormatProvider provider)
        {
            return ToString(o, null, provider);
        }

        
        
        
        
        
        public static double ToDouble(
            object o)
        {
            return ToDouble(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        public static double ToDouble(
            object o,
            IFormatProvider provider)
        {
            return ToDouble(o, 0.0, provider);
        }

        
        
        
        
        
        public static int ToInt32(
            object o)
        {
            return ToInt32(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        public static int ToInt32(
            object o,
            IFormatProvider provider)
        {
            return ToInt32(o, 0, provider);
        }

        
        
        
        
        
        public static long ToInt64(
            object o)
        {
            return ToInt64(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        public static long ToInt64(
            object o,
            IFormatProvider provider)
        {
            return ToInt64(o, 0, provider);
        }

        
        
        
        
        
        public static decimal ToDecimal(
            object o)
        {
            return ToDecimal(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        public static decimal ToDecimal(
            object o,
            IFormatProvider provider)
        {
            return ToDecimal(o, decimal.Zero, provider);
        }

        
        
        
        
        
        public static DateTime ToDateTime(
            object o)
        {
            return ToDateTime(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        public static DateTime ToDateTime(
            object o,
            IFormatProvider provider)
        {
            return ToDateTime(o, DateTime.MinValue, provider);
        }

        
        
        
        
        
        public static bool ToBoolean(
            object o)
        {
            return ToBoolean(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        public static bool ToBoolean(
            object o,
            IFormatProvider provider)
        {
            return ToBoolean(o, false, provider);
        }

        
        
        
        
        
        public static Guid ToGuid(
            object o)
        {
            return ToGuid(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        public static Guid ToGuid(
            object o,
            IFormatProvider provider)
        {
            return ToGuid(o, Guid.Empty, provider);
        }

        
        #endregion

        #region 用户自定义返回的转换方法.
        

        
        
        
        
        
        
        public static T ToT<T>(
            object o,
            T fallbackTo)
        {
            if (o == null)
            {
                return fallbackTo;
            }
            
            
            else if (o.GetType() == typeof(T))
            {
                return (T)o;
            }
            else if (typeof(T).IsEnum) 
            {
                if (Enum.IsDefined(typeof(T), o))
                {
                    return (T)Enum.Parse(typeof(T), o.ToString(), true);
                }
                else
                {
                    return fallbackTo;
                }
            }
            else
            {
                return fallbackTo;
            }
        }

        
        
        
        
        
        
        public static string ToString(
            object o,
            string fallbackTo)
        {
            return ToString(o, fallbackTo, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        public static string ToString(
            object o,
            string fallbackTo,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return fallbackTo;
            }
            
            
            else if (o.GetType() == typeof(string))
            {
                return (string)o;
            }
            else
            {
                return Convert.ToString(o, provider);
            }
        }

        
        
        
        
        
        
        public static double ToDouble(
            object o,
            double fallbackTo)
        {
            return ToDouble(o, fallbackTo, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        public static double ToDouble(
            object o,
            double fallbackTo,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return fallbackTo;
            }
            
            
            else if (o.GetType() == typeof(double))
            {
                return (double)o;
            }
            else if (IsFloat(o, provider))
            {
                return Convert.ToDouble(o, provider);
            }
            else
            {
                return fallbackTo;
            }
        }

        
        
        
        
        
        
        public static int ToInt32(
            object o,
            int fallbackTo)
        {
            return ToInt32(o, fallbackTo, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        public static int ToInt32(
            object o,
            int fallbackTo,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return fallbackTo;
            }
            
            
            else if (o.GetType() == typeof(int))
            {
                return (int)o;
            }
            else if (IsInteger(o, provider))
            {
                return Convert.ToInt32(o, provider);
            }
            else if (o is Enum)
            {
                return (int)o;
            }
            else
            {
                return fallbackTo;
            }
        }

        
        
        
        
        
        
        public static long ToInt64(
            object o,
            long fallbackTo)
        {
            return ToInt64(o, fallbackTo, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        public static long ToInt64(
            object o,
            long fallbackTo,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return fallbackTo;
            }
            
            
            else if (o.GetType() == typeof(long))
            {
                return (long)o;
            }
            else if (IsInt64(o, provider))
            {
                return Convert.ToInt64(o, provider);
            }
            else
            {
                return fallbackTo;
            }
        }

        
        
        
        
        
        
        public static decimal ToDecimal(
            object o,
            decimal fallbackTo)
        {
            return ToDecimal(o, fallbackTo, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        public static decimal ToDecimal(
            object o,
            decimal fallbackTo,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return fallbackTo;
            }
            
            
            else if (o.GetType() == typeof(decimal))
            {
                return (decimal)o;
            }
            else if (IsDecimal(o, provider))
            {
                return Convert.ToDecimal(o, provider);
            }
            else
            {
                return fallbackTo;
            }
        }

        
        
        
        
        
        
        
        public static DateTime ToDateTime(
            object o,
            DateTime fallbackTo)
        {
            return ToDateTime(o, fallbackTo, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        public static DateTime ToDateTime(
            object o,
            DateTime fallbackTo,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return fallbackTo;
            }
            
            
            else if (o.GetType() == typeof(DateTime))
            {
                return (DateTime)o;
            }
            else if (IsDateTime(o, provider))
            {
                return Convert.ToDateTime(o, provider);
            }
            else
            {
                return fallbackTo;
            }
        }

        
        
        
        
        
        
        public static bool ToBoolean(
            object o,
            bool fallbackTo)
        {
            return ToBoolean(o, fallbackTo, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        public static bool ToBoolean(
            object o,
            bool fallbackTo,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return fallbackTo;
            }
            
            
            else if (o.GetType() == typeof(bool))
            {
                return (bool)o;
            }
            else if (IsBoolean(o, provider))
            {
                try
                {
                    string s =
                        Convert.ToString(o, provider).Trim().ToLowerInvariant();

                    if (s.Length <= 0)
                    {
                        return fallbackTo;
                    }
                    else if (
                        string.Compare(s, bool.TrueString, true) == 0 ||
                        s == @"1" ||
                        s == @"-1")
                    {
                        return true;
                    }
                    else if (
                        string.Compare(s, bool.FalseString, true) == 0 ||
                       s == @"0")
                    {
                        return false;
                    }
                    else
                    {
                        return bool.Parse(Convert.ToString(o, provider));
                    }
                }
                catch (FormatException)
                {
                    return fallbackTo;
                }
            }
            else
            {
                return fallbackTo;
            }
        }

        
        
        
        
        
        
        public static Guid ToGuid(
            object o,
            Guid fallbackTo)
        {
            return ToGuid(o, fallbackTo, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        public static Guid ToGuid(
            object o,
            Guid fallbackTo,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return fallbackTo;
            }
            
            else if (o.GetType() == typeof(Guid))
            {
                return (Guid)o;
            }
            else if (IsGuid(o, provider))
            {
                if (o is byte[])
                {
                    return new Guid(o as byte[]);
                }
                else
                {
                    return new Guid(Convert.ToString(o, provider));
                }
            }
            else
            {
                return fallbackTo;
            }
        }
        
        #endregion

        #region 转换检查方法.
        
        
        
        
        
        
        
        
        public static bool IsBoolean(
            object o)
        {
            return IsBoolean(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        public static bool IsBoolean(
            object o,
            IFormatProvider provider)
        {
           
            if (o == null) 
            {
                return false;
            }
            
            
            else if (o.GetType() == typeof(bool)) 
            {
                return true;
            }
            else
            {
                string s =
                    Convert.ToString(o, provider).Trim().ToLowerInvariant();

                if (s.Length <= 0) 
                {
                    return false;
                }
                else if (o is bool) 
                {
                    return true;
                }
                else if (
                    string.Compare(s, bool.TrueString, true) == 0 ||
                    s == @"1" ||
                    s == @"-1")
                {
                    return true;
                }
                else if (
                   string.Compare(s, bool.FalseString, true) == 0 ||
                   s == @"0")
                {
                    return true;
                }
                else
                {
                    bool a;
                    return bool.TryParse(Convert.ToString(o, provider), out a);
                }
            }
        }

        
        
        
        
        
        
        
        public static bool IsDateTime(
            object o)
        {
            return IsDateTime(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        public static bool IsDateTime(
            object o,
            IFormatProvider provider)
        {
            if (o == null ||
                Convert.ToString(o, provider).Trim().Length <= 0)
            {
                return false;
            }
            
            
            else if (o.GetType() == typeof(DateTime)) 
            {
                return true;
            }
            else
            {
                DateTime r;

                return DateTime.TryParse(
                    Convert.ToString(o, provider),
                    provider,
                    DateTimeStyles.None,
                    out r);
            }
        }

        
        
        
        
        
        
        
        public static bool IsNumeric(
            object o)
        {
            return IsNumeric(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        
        public static bool IsNumeric(
            object o,
            IFormatProvider provider)
        {
            return DoIsNumeric(o,
                floatNumberStyle,
                provider);
        }

        
        
        
        
        
        
        
        
        public static bool IsDecimal(
            object o)
        {
            return IsDecimal(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        
        public static bool IsDecimal(
            object o,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return false;
            }
            
            
            else if (o.GetType() == typeof(decimal))
            {
                return true;
            }
            else
            {
                return DoIsNumeric(
                    o,
                    floatNumberStyle,
                    provider);
            }
        }

        
        
        
        
        
        
        
        
        public static bool IsFloat(
            object o)
        {
            return IsFloat(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        
        public static bool IsFloat(
            object o,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return false;
            }
            
            
            else if (o.GetType() == typeof(float))
            {
                return true;
            }
            else
            {
                return DoIsNumeric(o,
                    floatNumberStyle,
                    provider);
            }
        }

        
        
        
        
        
        
        
        public static bool IsDouble(
            object o)
        {
            return IsDouble(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        
        public static bool IsDouble(
            object o,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return false;
            }
            
            
            else if (o.GetType() == typeof(double))
            {
                return true;
            }
            
            
            else if (o.GetType() == typeof(float))
            {
                return true;
            }
            else
            {
                return DoIsNumeric(o,
                    floatNumberStyle,
                    provider);
            }
        }

        
        
        
        
        
        
        
        
        public static bool IsInteger(
            object o)
        {
            return IsInteger(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        
        public static bool IsInteger(
            object o,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return false;
            }
            
            
            else if (o.GetType() == typeof(int))
            {
                return true;
            }
            
            
            else if (o.GetType() == typeof(long))
            {
                return true;
            }
            else if (o is Enum)
            {
                return true;
            }
            else
            {
                return DoIsNumeric(o, NumberStyles.Integer, provider);
            }
        }

        
        
        
        
        
        
        
        
        public static bool IsInt32(
            object o)
        {
            return IsInt32(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        
        public static bool IsInt32(
            object o,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return false;
            }
            
            
            else if (o.GetType() == typeof(Int32))
            {
                return true;
            }
            else
            {
                return DoIsNumeric(o, NumberStyles.Integer, provider);
            }
        }

        
        
        
        
        
        
        
        
        public static bool IsInt64(
            object o)
        {
            return IsInt64(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        
        public static bool IsInt64(
            object o,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return false;
            }
            
            
            else if (o.GetType() == typeof(Int64))
            {
                return true;
            }
            else
            {
                return DoIsNumeric(o, NumberStyles.Integer, provider);
            }
        }

        
        
        
        
        
        
        
        
        public static bool IsCurrency(
            object o)
        {
            return IsCurrency(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        
        public static bool IsCurrency(
            object o,
            IFormatProvider provider)
        {
            return DoIsNumeric(o, NumberStyles.Currency, provider);
        }

        
        
        
        
        
        
        
        private static bool DoIsNumeric(
            object o,
            NumberStyles styles,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return false;
            }
            else if (Convert.ToString(o, provider).Length <= 0)
            {
                return false;
            }
            else
            {
                double result;
                return double.TryParse(
                    o.ToString(),
                    styles,
                    provider,
                    out result);
            }
        }

        
        
        
        
        
        
        
        public static bool IsGuid(
            object o)
        {
            return IsGuid(o, CultureInfo.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        public static bool IsGuid(
            object o,
            IFormatProvider provider)
        {
            if (o == null)
            {
                return false;
            }
            
            
            else if (o.GetType() == typeof(Guid))
            {
                return true;
            }
            else if (o is byte[])
            {
                try
                {
                    Guid ignore = new Guid(o as byte[]);
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    Guid ignore = new Guid(o.ToString());
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
        }

        
        #endregion

        #region 金额格式化方法
        
        
        
        
        
        public static string FormatCurrency_Chinese(decimal num)
        {
            string strNumber = "零壹贰叁肆伍陆柒捌玖";            
            string strDigit = "万仟佰拾亿仟佰拾万仟佰拾圆角分"; 

            string strDStrFormat = "";    
            System.Text.StringBuilder result = new System.Text.StringBuilder();  

            int maxdigit;    
            int currdigit = 0; 
            int nzero = 0;  
            int temp;            

            num = Math.Round(Math.Abs(num), 2);    
            strDStrFormat = ((long)(num * 100)).ToString();        
            maxdigit = strDStrFormat.Length;      
            if (maxdigit > 15) { return "溢出"; }
            strDigit = strDigit.Substring(15 - maxdigit);   


            
            for (int i = 0; i < maxdigit; i++)
            {
                currdigit = maxdigit - i; 

                temp = Convert.ToInt32(strDStrFormat[i].ToString());      

                if (currdigit != 3 && currdigit != 7 && currdigit != 11 && currdigit != 15)
                {
                    if (temp == 0) 
                    {
                        nzero++;
                    }
                    else
                    {

                        if (temp != 0 && nzero != 0) 
                        {
                            
                            result.Append(strNumber[0]);

                            
                            result.Append(strNumber[temp]);

                            
                            result.Append(strDigit[i]);

                            nzero = 0;
                        }
                        else if (temp != 0 && nzero == 0) 
                        {
                            
                            result.Append(strNumber[temp]);

                            
                            result.Append(strDigit[i]);

                            nzero = 0;
                        }
                        else if (temp == 0 && nzero != 0) 
                        {
                            
                        }
                    }
                }
                else
                {
                    
                    if (nzero != 0 && temp != 0)
                        
                        result.Append(strNumber[0]);

                    if (temp != 0) 
                        
                        result.Append(strNumber[temp]);


                    nzero = 0;

                    
                    if (currdigit >= 3 && currdigit < 5) 
                        result.Append(strDigit[i]);
                    else if (currdigit >= 5 && currdigit < 11) 
                        result.Append(strDigit[i]);
                    else if (currdigit >= 11) 
                        result.Append(strDigit[i]);
                }

                if (i == maxdigit - 1 && temp == 0)
                {
                    
                    result.Append('整');
                }
            }

            if (num == 0) 
            {
                result.Append("零圆整");
            }

            return result.ToString();
        }


        

        
        
        
        
        
        public static string FormatCurrency(
            decimal val)
        {
            
            return FormatCurrency(val, Thread.CurrentThread.CurrentCulture);
        }

        
        
        
        
        
        
        public static string FormatCurrency(
            decimal val,
            IFormatProvider provider)
        {
            
            return val.ToString(@"C", provider);
        }

        
        
        
        
        
        
        
        public static string FormatCurrency(
            decimal val,
            int precision)
        {
            return FormatCurrency(
                val,
                precision,
                Thread.CurrentThread.CurrentCulture);
        }

        
        
        
        
        
        
        
        public static string FormatCurrency(
            decimal val,
            int precision,
            IFormatProvider provider)
        {
            NumberFormatInfo nfi =
                (provider.GetFormat(typeof(NumberFormatInfo)) as
                NumberFormatInfo).Clone() as NumberFormatInfo;
            nfi.CurrencyDecimalDigits = precision;

            
            return val.ToString(@"C", nfi);
        }

        
        
        
        
        
        
        public static string FormatCurrency(
            decimal val,
            bool addCurrencySymbol)
        {
            return FormatCurrency(
                val,
                addCurrencySymbol,
                Thread.CurrentThread.CurrentCulture);
        }

        
        
        
        
        
        
        
        public static string FormatCurrency(
            decimal val,
            bool addCurrencySymbol,
            IFormatProvider provider)
        {
            if (addCurrencySymbol)
            {
                return FormatCurrency(val);
            }
            else
            {
                return val.ToString(@"n", provider);
            }
        }

        
        
        
        
        
        
        
        public static string FormatCurrency(
            decimal val,
            int precision,
            bool addCurrencySymbol)
        {
            return FormatCurrency(
                val,
                precision,
                addCurrencySymbol,
                Thread.CurrentThread.CurrentCulture);
        }

        
        
        
        
        
        
        
        
        public static string FormatCurrency(
            decimal val,
            int precision,
            bool addCurrencySymbol,
            IFormatProvider provider)
        {
            if (addCurrencySymbol)
            {
                return FormatCurrency(val, precision, provider);
            }
            else
            {
                NumberFormatInfo nfi =
                    (provider.GetFormat(typeof(NumberFormatInfo)) as
                    NumberFormatInfo).Clone() as NumberFormatInfo;
                nfi.NumberDecimalDigits = precision;

                return val.ToString(@"n", nfi);
            }
        }

        
        
        
        
        
        public static string FormatDecimal(
            decimal val)
        {
            return FormatDecimal(val, Thread.CurrentThread.CurrentCulture);
        }

        
        
        
        
        
        
        public static string FormatDecimal(
            decimal val,
            IFormatProvider provider)
        {
            return val.ToString(@"D", provider);
        }

        
        #endregion

        #region 私有变量
        

        
        
        
        private static readonly NumberStyles floatNumberStyle =
            NumberStyles.Float |
            NumberStyles.Number |
            NumberStyles.AllowThousands |
            NumberStyles.AllowDecimalPoint |
            NumberStyles.AllowLeadingSign |
            NumberStyles.AllowLeadingWhite |
            NumberStyles.AllowTrailingWhite;

        
        #endregion
    }

    
}
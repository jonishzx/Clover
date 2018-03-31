using System;
using System.ComponentModel;
using Clover.Data.Properties;
using Clover.Data.SmartField;

namespace Clover.Data
{
    
    
    
    
    
    
    
    
    [Serializable]
    [TypeConverter(typeof (SmartDateConverter))]
    public struct SmartDate : ISmartField,
        IComparable, IConvertible, IFormattable
    {
        private static string _defaultFormat;

        #region EmptyValue enum

        
        
        
        
        public enum EmptyValue
        {
            
            
            
            
            MinDate,

            
            
            
            
            MaxDate
        }

        #endregion

        #region Constructors

        static SmartDate()
        {
            _defaultFormat = "d";
        }

        
        
        
        
        public SmartDate(bool emptyIsMin)
        {
            _emptyValue = GetEmptyValue(emptyIsMin);
            _format = null;
            _initialized = false;
            
            _date = DateTime.MinValue;
            SetEmptyDate(_emptyValue);
        }

        
        
        
        
        public SmartDate(EmptyValue emptyValue)
        {
            _emptyValue = emptyValue;
            _format = null;
            _initialized = false;
            
            _date = DateTime.MinValue;
            SetEmptyDate(_emptyValue);
        }

        
        
        
        
        
        
        
        
        public SmartDate(DateTime value)
        {
            _emptyValue = EmptyValue.MinDate;
            _format = null;
            _initialized = false;
            _date = DateTime.MinValue;
            Date = value;
        }

        
        
        
        
        
        public SmartDate(DateTime value, bool emptyIsMin)
        {
            _emptyValue = GetEmptyValue(emptyIsMin);
            _format = null;
            _initialized = false;
            _date = DateTime.MinValue;
            Date = value;
        }

        
        
        
        
        
        public SmartDate(DateTime value, EmptyValue emptyValue)
        {
            _emptyValue = emptyValue;
            _format = null;
            _initialized = false;
            _date = DateTime.MinValue;
            Date = value;
        }

        
        
        
        
        
        
        public SmartDate(DateTime value, EmptyValue emptyValue, DateTimeKind kind)
        {
            _emptyValue = emptyValue;
            _format = null;
            _initialized = false;
            _date = DateTime.MinValue;
            Date = DateTime.SpecifyKind(value, kind);
        }

        
        
        
        
        
        
        
        
        public SmartDate(DateTime? value)
        {
            _emptyValue = EmptyValue.MinDate;
            _format = null;
            _initialized = false;
            _date = DateTime.MinValue;
            if (value.HasValue)
                Date = value.Value;
        }

        
        
        
        
        
        public SmartDate(DateTime? value, bool emptyIsMin)
        {
            _emptyValue = GetEmptyValue(emptyIsMin);
            _format = null;
            _initialized = false;
            _date = DateTime.MinValue;
            if (value.HasValue)
                Date = value.Value;
        }

        
        
        
        
        
        public SmartDate(DateTime? value, EmptyValue emptyValue)
        {
            _emptyValue = emptyValue;
            _format = null;
            _initialized = false;
            _date = DateTime.MinValue;
            if (value.HasValue)
                Date = value.Value;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public SmartDate(DateTimeOffset value)
        {
            _emptyValue = EmptyValue.MinDate;
            _format = null;
            _initialized = false;
            _date = DateTime.MinValue;
            Date = value.DateTime;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public SmartDate(DateTimeOffset value, bool emptyIsMin)
        {
            _emptyValue = GetEmptyValue(emptyIsMin);
            _format = null;
            _initialized = false;
            _date = DateTime.MinValue;
            Date = value.DateTime;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public SmartDate(DateTimeOffset value, EmptyValue emptyValue)
        {
            _emptyValue = emptyValue;
            _format = null;
            _initialized = false;
            _date = DateTime.MinValue;
            Date = value.DateTime;
        }

        
        
        
        
        
        
        
        
        public SmartDate(string value)
        {
            _emptyValue = EmptyValue.MinDate;
            _format = null;
            _initialized = true;
            _date = DateTime.MinValue;
            Text = value;
        }

        
        
        
        
        
        public SmartDate(string value, bool emptyIsMin)
        {
            _emptyValue = GetEmptyValue(emptyIsMin);
            _format = null;
            _initialized = true;
            _date = DateTime.MinValue;
            Text = value;
        }

        
        
        
        
        
        public SmartDate(string value, EmptyValue emptyValue)
        {
            _emptyValue = emptyValue;
            _format = null;
            _initialized = true;
            _date = DateTime.MinValue;
            Text = value;
        }

        private static EmptyValue GetEmptyValue(bool emptyIsMin)
        {
            if (emptyIsMin)
                return EmptyValue.MinDate;
            return EmptyValue.MaxDate;
        }

        private void SetEmptyDate(EmptyValue emptyValue)
        {
            if (emptyValue == EmptyValue.MinDate)
                Date = DateTime.MinValue;
            else
                Date = DateTime.MaxValue;
        }

        #endregion

        #region Text Support

        
        
        
        
        
        
        
        
        
        public string FormatString
        {
            get
            {
                if (_format == null)
                    _format = _defaultFormat;
                return _format;
            }
            set { _format = value; }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public string Text
        {
            get { return DateToString(Date, FormatString, _emptyValue); }
            set { Date = StringToDate(value, _emptyValue); }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static void SetDefaultFormatString(string formatString)
        {
            _defaultFormat = formatString;
        }

        #endregion

        #region Date Support

        
        
        
        public DateTime Date
        {
            get
            {
                if (!_initialized)
                {
                    _date = DateTime.MinValue;
                    _initialized = true;
                }
                return _date;
            }
            set
            {
                _date = value;
                _initialized = true;
            }
        }

        
        
        
        public DateTimeOffset ToDateTimeOffset()
        {
            return new DateTimeOffset(Date);
        }

        
        
        
        public DateTime? ToNullableDate()
        {
            if (IsEmpty)
                return new DateTime?();
            return Date;
        }

        #endregion

        #region System.Object overrides

        
        
        
        public override string ToString()
        {
            return Text;
        }

        
        
        
        
        
        
        public string ToString(string format)
        {
            if (string.IsNullOrEmpty(format))
                return ToString();
            return DateToString(Date, format, _emptyValue);
        }

        
        
        
        
        
        public override bool Equals(object obj)
        {
            if (obj is SmartDate)
            {
                var tmp = (SmartDate) obj;
                if (IsEmpty && tmp.IsEmpty)
                    return true;
                return Date.Equals(tmp.Date);
            }
            if (obj is DateTime)
                return Date.Equals((DateTime) obj);
            if (obj is string)
                return (CompareTo(obj.ToString()) == 0);
            return false;
        }

        
        
        
        public override int GetHashCode()
        {
            return Date.GetHashCode();
        }

        #endregion

        #region DBValue

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public object DBValue
        {
            get
            {
                if (IsEmpty)
                    return DBNull.Value;
                return Date;
            }
        }

        #endregion

        #region Empty Dates

        
        
        
        
        
        
        
        
        
        public bool EmptyIsMin
        {
            get { return (_emptyValue == EmptyValue.MinDate); }
        }

        
        
        
        public bool IsEmpty
        {
            get
            {
                if (_emptyValue == EmptyValue.MinDate)
                    return Date.Equals(DateTime.MinValue);
                return Date.Equals(DateTime.MaxValue);
            }
        }

        #endregion

        #region Conversion Functions

        
        
        
        
        
        
        
        
        public static SmartDate Parse(string value)
        {
            return new SmartDate(value);
        }

        
        
        
        
        
        
        public static SmartDate Parse(string value, EmptyValue emptyValue)
        {
            return new SmartDate(value, emptyValue);
        }

        
        
        
        
        
        
        public static SmartDate Parse(string value, bool emptyIsMin)
        {
            return new SmartDate(value, emptyIsMin);
        }

        
        
        
        
        
        
        public static bool TryParse(string value, ref SmartDate result)
        {
            return TryParse(value, EmptyValue.MinDate, ref result);
        }

        
        
        
        
        
        
        
        public static bool TryParse(string value, EmptyValue emptyValue, ref SmartDate result)
        {
            DateTime dateResult = DateTime.MinValue;
            if (TryStringToDate(value, emptyValue, ref dateResult))
            {
                result = new SmartDate(dateResult, emptyValue);
                return true;
            }
            return false;
        }

        
        
        
        
        
        
        
        
        
        public static DateTime StringToDate(string value)
        {
            return StringToDate(value, true);
        }

        
        
        
        
        
        
        
        
        
        
        
        public static DateTime StringToDate(string value, bool emptyIsMin)
        {
            return StringToDate(value, GetEmptyValue(emptyIsMin));
        }

        
        
        
        
        
        
        
        
        
        
        
        public static DateTime StringToDate(string value, EmptyValue emptyValue)
        {
            DateTime result = DateTime.MinValue;
            if (TryStringToDate(value, emptyValue, ref result))
                return result;
            throw new ArgumentException(Resources.StringToDateException);
        }

        private static bool TryStringToDate(string value, EmptyValue emptyValue, ref DateTime result)
        {
            DateTime tmp;
            if (String.IsNullOrEmpty(value))
            {
                if (emptyValue == EmptyValue.MinDate)
                {
                    result = DateTime.MinValue;
                    return true;
                }
                result = DateTime.MaxValue;
                return true;
            }
            if (DateTime.TryParse(value, out tmp))
            {
                result = tmp;
                return true;
            }
            string ldate = value.Trim().ToLower();
            if (ldate == Resources.SmartDateT ||
                ldate == Resources.SmartDateToday ||
                ldate == ".")
            {
                result = DateTime.Now;
                return true;
            }
            if (ldate == Resources.SmartDateY ||
                ldate == Resources.SmartDateYesterday ||
                ldate == "-")
            {
                result = DateTime.Now.AddDays(-1);
                return true;
            }
            if (ldate == Resources.SmartDateTom ||
                ldate == Resources.SmartDateTomorrow ||
                ldate == "+")
            {
                result = DateTime.Now.AddDays(1);
                return true;
            }
            return false;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public static string DateToString(
            DateTime value, string formatString)
        {
            return DateToString(value, formatString, true);
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        public static string DateToString(
            DateTime value, string formatString, bool emptyIsMin)
        {
            return DateToString(value, formatString, GetEmptyValue(emptyIsMin));
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        public static string DateToString(
            DateTime value, string formatString, EmptyValue emptyValue)
        {
            if (emptyValue == EmptyValue.MinDate)
            {
                if (value == DateTime.MinValue)
                    return string.Empty;
            }
            else
            {
                if (value == DateTime.MaxValue)
                    return string.Empty;
            }
            return string.Format("{0:" + formatString + "}", value);
        }

        #endregion

        #region Manipulation Functions

        
        
        
        
        
        
        
        
        
        
        int IComparable.CompareTo(object value)
        {
            if (value is SmartDate)
                return CompareTo((SmartDate) value);
            throw new ArgumentException(Resources.ValueNotSmartDateException);
        }

        
        
        
        
        
        
        
        
        
        
        public int CompareTo(SmartDate value)
        {
            if (IsEmpty && value.IsEmpty)
                return 0;
            return _date.CompareTo(value.Date);
        }

        
        
        
        
        
        public int CompareTo(string value)
        {
            return Date.CompareTo(StringToDate(value, _emptyValue));
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public int CompareTo(DateTimeOffset value)
        {
            return Date.CompareTo(value.DateTime);
        }

        
        
        
        
        
        public int CompareTo(DateTime value)
        {
            return Date.CompareTo(value);
        }

        
        
        
        
        public DateTime Add(TimeSpan value)
        {
            if (IsEmpty)
                return Date;
            return Date.Add(value);
        }

        
        
        
        
        public DateTime Subtract(TimeSpan value)
        {
            if (IsEmpty)
                return Date;
            return Date.Subtract(value);
        }

        
        
        
        
        
        
        
        
        
        
        
        public TimeSpan Subtract(DateTimeOffset value)
        {
            if (IsEmpty)
                return TimeSpan.Zero;
            return Date.Subtract(value.DateTime);
        }

        
        
        
        
        public TimeSpan Subtract(DateTime value)
        {
            if (IsEmpty)
                return TimeSpan.Zero;
            return Date.Subtract(value);
        }

        #endregion

        #region Operators

        
        
        
        
        
        
        public static bool operator ==(SmartDate obj1, SmartDate obj2)
        {
            return obj1.Equals(obj2);
        }

        
        
        
        
        
        
        public static bool operator !=(SmartDate obj1, SmartDate obj2)
        {
            return !obj1.Equals(obj2);
        }

        
        
        
        
        public static implicit operator string(SmartDate obj1)
        {
            return obj1.Text;
        }

        
        
        
        
        public static implicit operator DateTime(SmartDate obj1)
        {
            return obj1.Date;
        }

        
        
        
        
        public static implicit operator DateTime?(SmartDate obj1)
        {
            return obj1.ToNullableDate();
        }

        
        
        
        
        public static implicit operator DateTimeOffset(SmartDate obj1)
        {
            return obj1.ToDateTimeOffset();
        }

        
        
        
        
        public static explicit operator SmartDate(string dateValue)
        {
            return new SmartDate(dateValue);
        }

        
        
        
        
        public static implicit operator SmartDate(DateTime dateValue)
        {
            return new SmartDate(dateValue);
        }

        
        
        
        
        public static implicit operator SmartDate(DateTime? dateValue)
        {
            return new SmartDate(dateValue);
        }

        
        
        
        
        public static explicit operator SmartDate(DateTimeOffset dateValue)
        {
            return new SmartDate(dateValue);
        }

        
        
        
        
        
        
        public static bool operator ==(SmartDate obj1, DateTime obj2)
        {
            return obj1.Equals(obj2);
        }

        
        
        
        
        
        
        public static bool operator !=(SmartDate obj1, DateTime obj2)
        {
            return !obj1.Equals(obj2);
        }

        
        
        
        
        
        
        public static bool operator ==(SmartDate obj1, string obj2)
        {
            return obj1.Equals(obj2);
        }

        
        
        
        
        
        
        public static bool operator !=(SmartDate obj1, string obj2)
        {
            return !obj1.Equals(obj2);
        }

        
        
        
        
        
        
        public static SmartDate operator +(SmartDate start, TimeSpan span)
        {
            return new SmartDate(start.Add(span), start.EmptyIsMin);
        }

        
        
        
        
        
        
        public static SmartDate operator -(SmartDate start, TimeSpan span)
        {
            return new SmartDate(start.Subtract(span), start.EmptyIsMin);
        }

        
        
        
        
        
        
        public static TimeSpan operator -(SmartDate start, SmartDate finish)
        {
            return start.Subtract(finish.Date);
        }

        
        
        
        
        
        
        public static bool operator >(SmartDate obj1, SmartDate obj2)
        {
            return obj1.CompareTo(obj2) > 0;
        }

        
        
        
        
        
        
        public static bool operator <(SmartDate obj1, SmartDate obj2)
        {
            return obj1.CompareTo(obj2) < 0;
        }

        
        
        
        
        
        
        public static bool operator >(SmartDate obj1, DateTime obj2)
        {
            return obj1.CompareTo(obj2) > 0;
        }

        
        
        
        
        
        
        public static bool operator <(SmartDate obj1, DateTime obj2)
        {
            return obj1.CompareTo(obj2) < 0;
        }

        
        
        
        
        
        
        public static bool operator >(SmartDate obj1, string obj2)
        {
            return obj1.CompareTo(obj2) > 0;
        }

        
        
        
        
        
        
        public static bool operator <(SmartDate obj1, string obj2)
        {
            return obj1.CompareTo(obj2) < 0;
        }

        
        
        
        
        
        
        public static bool operator >=(SmartDate obj1, SmartDate obj2)
        {
            return obj1.CompareTo(obj2) >= 0;
        }

        
        
        
        
        
        
        public static bool operator <=(SmartDate obj1, SmartDate obj2)
        {
            return obj1.CompareTo(obj2) <= 0;
        }

        
        
        
        
        
        
        public static bool operator >=(SmartDate obj1, DateTime obj2)
        {
            return obj1.CompareTo(obj2) >= 0;
        }

        
        
        
        
        
        
        public static bool operator <=(SmartDate obj1, DateTime obj2)
        {
            return obj1.CompareTo(obj2) <= 0;
        }

        
        
        
        
        
        
        public static bool operator >=(SmartDate obj1, string obj2)
        {
            return obj1.CompareTo(obj2) >= 0;
        }

        
        
        
        
        
        
        public static bool operator <=(SmartDate obj1, string obj2)
        {
            return obj1.CompareTo(obj2) <= 0;
        }

        #endregion

        #region  IConvertible

        TypeCode IConvertible.GetTypeCode()
        {
            return ((IConvertible) _date).GetTypeCode();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToBoolean(provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToByte(provider);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToChar(provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToDateTime(provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToDecimal(provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToDouble(provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToInt16(provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToInt32(provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToInt64(provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToSByte(provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToSingle(provider);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return Text.ToString(provider);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType.Equals(typeof (string)))
                return ((IConvertible) Text).ToType(conversionType, provider);
            if (conversionType.Equals(typeof (SmartDate)))
                return this;
            return ((IConvertible) _date).ToType(conversionType, provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToUInt16(provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToUInt32(provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible) _date).ToUInt64(provider);
        }

        #endregion

        #region IFormattable Members

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(format);
        }

        #endregion

        private readonly EmptyValue _emptyValue;
        private DateTime _date;
        private string _format;
        private bool _initialized;
    }
}
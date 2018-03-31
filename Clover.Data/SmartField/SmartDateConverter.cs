using System;
using System.ComponentModel;
using System.Globalization;

namespace Clover.Data.SmartField
{
    
    
    
    public class SmartDateConverter : TypeConverter
    {
        
        
        
        
        
        
        
        
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (string))
                return true;
            if (sourceType == typeof (DateTime))
                return true;
            if (sourceType == typeof (DateTimeOffset))
                return true;
            if (sourceType == typeof (DateTime?))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        
        
        
        
        
        
        
        
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return new SmartDate(Convert.ToString(value));
            if (value is DateTime)
                return new SmartDate(Convert.ToDateTime(value));
            if (value == null)
                return new SmartDate();
            if (value is DateTime?)
                return new SmartDate((DateTime?) value);
            if (value is DateTimeOffset)
                return new SmartDate(((DateTimeOffset) value).DateTime);
            return base.ConvertFrom(context, culture, value);
        }

        
        
        
        
        
        
        
        
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (string))
                return true;
            if (destinationType == typeof (DateTime))
                return true;
            if (destinationType == typeof (DateTimeOffset))
                return true;
            if (destinationType == typeof (DateTime?))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        
        
        
        
        
        
        
        
        
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            var sd = (SmartDate) value;
            if (destinationType == typeof (string))
                return sd.Text;
            if (destinationType == typeof (DateTime))
                return sd.Date;
            if (destinationType == typeof (DateTimeOffset))
                return new DateTimeOffset(sd.Date);
            if (destinationType == typeof (DateTime?))
                return sd.Date;
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
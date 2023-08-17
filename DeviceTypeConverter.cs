using ModbusRegisterMap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorControl.ModbusRegisterMap
{
    public class DevFloatConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var casted = value as string;
            return casted != null
                ? (DevFloat)float.Parse(casted)
                : base.ConvertFrom(context, culture, value);
        }
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            var casted = value as DevFloat;
            return destinationType == typeof(string) && casted != null
                ? casted.ToString(null, culture)
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
    public class DevUShortConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var casted = value as string;
            return casted != null
                ? (DevUShort)ushort.Parse(casted)
                : base.ConvertFrom(context, culture, value);
        }
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            var casted = value as DevUShort;
            return destinationType == typeof(string) && casted != null
                ? casted.ToString(null, culture)
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
    public class DevULongConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var casted = value as string;
            return casted != null
                ? (DevULong)ulong.Parse(casted)
                : base.ConvertFrom(context, culture, value);
        }
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            var casted = value as DevULong;
            return destinationType == typeof(string) && casted != null
                ? casted.ToString(null, culture)
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

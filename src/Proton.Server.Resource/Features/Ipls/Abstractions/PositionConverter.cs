using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AltV.Net.Data;

namespace Proton.Server.Resource.Features.Ipls.Abstractions;

public sealed class PositionConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);
    public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType) => destinationType == typeof(Position);
    // public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    // {
    // }
    // public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    // {
    //     return base.ConvertTo(context, culture, value, destinationType);
    // }
}

using System.ComponentModel;
using System.Reflection;

namespace DockerProject.Models;

public static class Helper
{
    public static string GetEnumDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }
    
    public static string GetContrastColor(string hexColor)
    {
        if (string.IsNullOrEmpty(hexColor)) return "#000000";
        var cleanHex = hexColor.Replace("#", "");
        if (cleanHex.Length != 6) return "#000000";
        var r = Convert.ToInt32(cleanHex.Substring(0, 2), 16);
        var g = Convert.ToInt32(cleanHex.Substring(2, 2), 16);
        var b = Convert.ToInt32(cleanHex.Substring(4, 2), 16);
        var yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;
        return (yiq >= 128) ? "#000000" : "#FFFFFF";
    }
}
using System.ComponentModel;
using System.Reflection;

namespace SampleProject.Domain.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// 取得 Enum 的 Description Attribute 值
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null)
            return value.ToString();

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// 取得 Enum 的名稱（英文）
    /// </summary>
    public static string GetName(this Enum value)
    {
        return value.ToString();
    }
}

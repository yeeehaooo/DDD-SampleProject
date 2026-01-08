using System.ComponentModel;

namespace SampleProject.Domain.Enums;

/// <summary>
/// 預設規格類型
/// </summary>
public enum SpecificationType
{
    [Description("尺寸")]
    Size = 1,

    [Description("顏色")]
    Color = 2,

    [Description("容量")]
    Capacity = 3,

    [Description("重量")]
    Weight = 4,

    [Description("材質")]
    Material = 5,

    [Description("品牌")]
    Brand = 6
}

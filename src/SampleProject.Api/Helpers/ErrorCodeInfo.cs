namespace SampleProject.Api.Helpers;

/// <summary>
/// 錯誤代碼資訊 - 包含錯誤代碼和描述
/// </summary>
public class ErrorCodeInfo
{
    public string Code { get; }
    public string Description { get; }

    public ErrorCodeInfo(string code, string description)
    {
        Code = code;
        Description = description;
    }

    /// <summary>
    /// 隱式轉換為字串（方便向後相容）
    /// </summary>
    public static implicit operator string(ErrorCodeInfo errorCodeInfo) => errorCodeInfo.Code;

    public override string ToString() => Code;
}

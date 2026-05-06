namespace EasyWeChatModels.Enums;

/// <summary>
/// 审批模式枚举
/// </summary>
public enum ExamineMode
{
    /// <summary>依次审批</summary>
    Sequential = 1,
    /// <summary>会签（需全部通过）</summary>
    CounterSign = 2,
    /// <summary>或签（任一通过即可）</summary>
    Or = 3
}
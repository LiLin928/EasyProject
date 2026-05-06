namespace EasyWeChatModels.Enums;

/// <summary>
/// 审批人设置类型枚举
/// </summary>
public enum ApproverSetType
{
    /// <summary>固定用户</summary>
    FixedUser = 1,
    /// <summary>直接主管</summary>
    Supervisor = 2,
    /// <summary>发起人自选</summary>
    InitiatorSelect = 4,
    /// <summary>发起人自己</summary>
    InitiatorSelf = 5,
    /// <summary>多级主管</summary>
    MultiSupervisor = 7,
    /// <summary>指定角色</summary>
    Role = 8,
    /// <summary>表单字段</summary>
    FormField = 9
}
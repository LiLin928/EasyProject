namespace CommonManager.Helper;

/// <summary>
/// 密码帮助类
/// </summary>
/// <remarks>
/// 使用 BCrypt 算法进行密码哈希和验证，提供安全的密码存储方案。
/// BCrypt 是专门为密码存储设计的哈希算法，具有以下特点：
/// - 自带盐值，无需单独存储
/// - 计算成本可调，防止暴力破解
/// - 抗彩虹表攻击
/// </remarks>
public static class PasswordHelper
{
    /// <summary>
    /// 哈希密码
    /// </summary>
    /// <param name="password">原始密码</param>
    /// <returns>哈希后的密码字符串</returns>
    /// <remarks>
    /// 使用 BCrypt 自动生成盐值并哈希密码。
    /// 哈希结果格式：$2a$10$...
    /// - $2a$: BCrypt 版本标识
    /// - $10$: 工作因子（计算成本）
    /// - 后面: 盐值和哈希值
    /// </remarks>
    /// <example>
    /// <code>
    /// var hashedPassword = PasswordHelper.HashPassword("123456");
    /// // 存储 hashedPassword 到数据库
    /// </code>
    /// </example>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="password">待验证的原始密码</param>
    /// <param name="hashedPassword">存储的哈希密码</param>
    /// <returns>密码是否匹配</returns>
    /// <remarks>
    /// BCrypt 会从哈希字符串中提取盐值，重新计算哈希并比对。
    /// </remarks>
    /// <example>
    /// <code>
    /// var storedHash = user.Password; // 从数据库获取
    /// bool isValid = PasswordHelper.VerifyPassword(inputPassword, storedHash);
    /// if (isValid) {
    ///     // 登录成功
    /// }
    /// </code>
    /// </example>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
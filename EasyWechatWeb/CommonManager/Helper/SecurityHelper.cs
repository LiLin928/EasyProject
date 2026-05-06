using System.Security.Cryptography;
using System.Text;

namespace CommonManager.Helper;

/// <summary>
/// 安全加密帮助类，提供常用的加密、编码和随机字符串生成方法
/// </summary>
/// <remarks>
/// 该类提供 MD5、SHA256 哈希加密，Base64 编解码，以及随机字符串生成等功能。
/// 注意：MD5 和 SHA256 是单向哈希，不可解密，适用于数据完整性验证和密码存储。
/// </remarks>
/// <example>
/// <code>
/// // 密码加密
/// var hashedPassword = SecurityHelper.Md5(password);
///
/// // 生成随机验证码
/// var code = SecurityHelper.RandomString(6);
///
/// // Base64 编码
/// var encoded = SecurityHelper.Base64Encode("Hello World");
/// </code>
/// </example>
public static class SecurityHelper
{
    /// <summary>
    /// 计算 MD5 哈希值
    /// </summary>
    /// <param name="input">要加密的字符串</param>
    /// <returns>32 位小写 MD5 哈希字符串</returns>
    /// <remarks>
    /// MD5 是单向哈希算法，不可解密。
    /// 常用于：
    /// - 密码存储（建议加盐后再哈希）
    /// - 文件完整性校验
    /// - 数据去重（根据哈希值判断）
    /// 注意：MD5 存在碰撞风险，高安全性场景建议使用 SHA256。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 简单密码哈希
    /// var password = "123456";
    /// var hashed = SecurityHelper.Md5(password);
    /// // 输出: e10adc3949ba59abbe56e057f20f883e
    ///
    /// // 加盐密码哈希（更安全）
    /// var salt = SecurityHelper.RandomString(8);
    /// var saltedPassword = password + salt;
    /// var hashedPassword = SecurityHelper.Md5(saltedPassword);
    ///
    /// // 文件完整性校验
    /// var fileContent = File.ReadAllText("data.txt");
    /// var checksum = SecurityHelper.Md5(fileContent);
    /// </code>
    /// </example>
    public static string Md5(string input)
    {
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    /// <summary>
    /// 计算 SHA256 哈希值
    /// </summary>
    /// <param name="input">要加密的字符串</param>
    /// <returns>64 位小写 SHA256 哈希字符串</returns>
    /// <remarks>
    /// SHA256 比 MD5 更安全，碰撞概率更低。
    /// 常用于：
    /// - 高安全性密码存储
    /// - JWT Token 密钥生成
    /// - 数字签名
    /// - 区块链等安全要求高的场景
    /// </remarks>
    /// <example>
    /// <code>
    /// // 高安全性密码哈希
    /// var password = "MySecretPassword";
    /// var hashed = SecurityHelper.Sha256(password);
    /// // 输出: 89e6e8f5a4f1c6d7b8e9f0a1b2c3d4e5...
    ///
    /// // JWT 密钥（建议使用足够长度的随机字符串）
    /// var securityKey = SecurityHelper.Sha256(Guid.NewGuid().ToString());
    /// </code>
    /// </example>
    public static string Sha256(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    /// <summary>
    /// 生成指定长度的随机字符串
    /// </summary>
    /// <param name="length">字符串长度</param>
    /// <returns>由大小写字母和数字组成的随机字符串</returns>
    /// <remarks>
    /// 字符集：A-Z、a-z、0-9（共 62 个字符）。
    /// 常用于：
    /// - 验证码生成
    /// - 临时密码生成
    /// - 盐值（Salt）生成
    /// - 随机文件名生成
    /// 注意：使用 Random 类，不适合高安全性随机数需求。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 生成 6 位验证码
    /// var verifyCode = SecurityHelper.RandomString(6);
    /// Console.WriteLine($"验证码: {verifyCode}");
    ///
    /// // 生成 16 位临时密码
    /// var tempPassword = SecurityHelper.RandomString(16);
    ///
    /// // 生成密码盐值
    /// var salt = SecurityHelper.RandomString(8);
    /// var hashedPassword = SecurityHelper.Md5(password + salt);
    ///
    /// // 生成随机文件名
    /// var fileName = SecurityHelper.RandomString(12) + ".txt";
    /// </code>
    /// </example>
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Base64 编码
    /// </summary>
    /// <param name="input">要编码的字符串</param>
    /// <returns>Base64 编码后的字符串</returns>
    /// <remarks>
    /// Base64 是一种编码方式，不是加密，可以解码还原。
    /// 常用于：
    /// - URL 安全传输
    /// - 邮件附件编码
    /// - 图片数据内嵌（Data URL）
    /// - 简单数据隐藏
    /// </remarks>
    /// <example>
    /// <code>
    /// // 编码字符串
    /// var encoded = SecurityHelper.Base64Encode("Hello World");
    /// Console.WriteLine(encoded);  // SGVsbG8gV29ybGQ=
    ///
    /// // 编码 JSON 数据
    /// var json = JsonHelper.ToJson(user);
    /// var encodedJson = SecurityHelper.Base64Encode(json);
    ///
    /// // URL 传输编码
    /// var urlSafeData = SecurityHelper.Base64Encode(queryParams);
    /// </code>
    /// </example>
    public static string Base64Encode(string input)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
    }

    /// <summary>
    /// Base64 解码
    /// </summary>
    /// <param name="input">Base64 编码的字符串</param>
    /// <returns>解码后的原始字符串</returns>
    /// <remarks>
    /// 解码 Base64 编码的字符串，还原为原始内容。
    /// 如果输入不是有效的 Base64 字符串，会抛出 FormatException。
    /// </remarks>
    /// <exception cref="FormatException">输入不是有效的 Base64 字符串</exception>
    /// <example>
    /// <code>
    /// // 解码字符串
    /// var decoded = SecurityHelper.Base64Decode("SGVsbG8gV29ybGQ=");
    /// Console.WriteLine(decoded);  // Hello World
    ///
    /// // 解码 JSON 数据
    /// var encodedJson = "...";
    /// var json = SecurityHelper.Base64Decode(encodedJson);
    /// var user = JsonHelper.ToObject&lt;User&gt;(json);
    /// </code>
    /// </example>
    public static string Base64Decode(string input)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(input));
    }
}
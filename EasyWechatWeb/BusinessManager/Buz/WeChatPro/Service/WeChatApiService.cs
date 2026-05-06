using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 微信 API 服务
/// </summary>
/// <remarks>
/// 负责调用微信官方 API，包括登录认证、获取 AccessToken 等。
/// 当 UseMock=true 时，使用模拟数据，无需真实凭证。
/// 当 UseMock=false 时，调用真实微信 API。
/// </remarks>
public class WeChatApiService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatApiService> _logger { get; set; } = null!;
    /// <summary>
    /// 微信配置选项
    /// </summary>
    public IOptions<WeChatOptions> _options { get; set; } = null!;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// 初始化微信 API 服务
    /// </summary>
    public WeChatApiService()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// 通过 code 获取微信用户 OpenID
    /// </summary>
    /// <param name="code">小程序登录 code</param>
    /// <returns>微信会话信息；失败返回 null</returns>
    /// <remarks>
    /// 当 UseMock=true 时，返回模拟数据（使用 code 的 MD5 作为 OpenId）。
    /// 当 UseMock=false 时，调用真实的 jscode2session API。
    ///
    /// code 有效期：5 分钟，且只能使用一次。
    /// </remarks>
    public async Task<WxCode2SessionResponse?> Code2SessionAsync(string code)
    {
        if (_options.Value.UseMock)
        {
            _logger.LogInformation("使用 Mock 模式登录，code: {Code}", code);

            // Mock: 使用 code 的 MD5 作为 OpenId
            var openId = ComputeMd5(code);
            return new WxCode2SessionResponse
            {
                OpenId = openId,
                SessionKey = ComputeMd5(code + "_session"),
                UnionId = null,
                ErrCode = 0
            };
        }

        // 真实 API 调用
        var url = $"https://api.weixin.qq.com/sns/jscode2session" +
                  $"?appid={_options.Value.AppId}" +
                  $"&secret={_options.Value.AppSecret}" +
                  $"&js_code={code}" +
                  $"&grant_type=authorization_code";

        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<WxCode2SessionResponse>(response);

            if (result?.ErrCode != 0)
            {
                _logger.LogError("微信登录失败: {ErrCode} - {ErrMsg}", result?.ErrCode, result?.ErrMsg);
                return null;
            }

            _logger.LogInformation("微信登录成功，OpenId: {OpenId}", result?.OpenId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用微信 API 失败");
            return null;
        }
    }

    /// <summary>
    /// 获取微信 AccessToken
    /// </summary>
    /// <returns>AccessToken；失败返回 null</returns>
    /// <remarks>
    /// AccessToken 用于调用微信后台大部分接口。
    /// 有效期 2 小时，建议使用中控服务器缓存管理。
    /// </remarks>
    public async Task<string?> GetAccessTokenAsync()
    {
        if (_options.Value.UseMock)
        {
            _logger.LogInformation("使用 Mock 模式获取 AccessToken");
            return "mock_access_token_" + Guid.NewGuid().ToString("N");
        }

        var url = $"https://api.weixin.qq.com/cgi-bin/token" +
                  $"?grant_type=client_credential" +
                  $"&appid={_options.Value.AppId}" +
                  $"&secret={_options.Value.AppSecret}";

        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<WxAccessTokenResponse>(response);

            if (result?.ErrCode != 0)
            {
                _logger.LogError("获取 AccessToken 失败: {ErrCode} - {ErrMsg}", result?.ErrCode, result?.ErrMsg);
                return null;
            }

            return result?.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取 AccessToken 失败");
            return null;
        }
    }

    /// <summary>
    /// 解密微信返回的加密手机号数据
    /// </summary>
    /// <param name="sessionKey">会话密钥</param>
    /// <param name="encryptedData">加密数据</param>
    /// <param name="iv">初始向量</param>
    /// <returns>解密后的手机号信息；失败返回 null</returns>
    public WxPhoneInfoDto? DecryptPhoneNumber(string sessionKey, string encryptedData, string iv)
    {
        if (_options.Value.UseMock)
        {
            _logger.LogInformation("使用 Mock 模式解密手机号");
            return new WxPhoneInfoDto
            {
                PhoneNumber = "+8613800138000",
                PurePhoneNumber = "13800138000",
                CountryCode = "86"
            };
        }

        try
        {
            // AES-128-CBC 解密
            var sessionKeyBytes = Convert.FromBase64String(sessionKey);
            var encryptedBytes = Convert.FromBase64String(encryptedData);
            var ivBytes = Convert.FromBase64String(iv);

            using var aes = Aes.Create();
            aes.Key = sessionKeyBytes;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            var json = Encoding.UTF8.GetString(decryptedBytes);

            _logger.LogInformation("解密手机号成功，JSON: {Json}", json);

            var result = JsonSerializer.Deserialize<WxPhoneInfoDto>(json);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解密手机号失败");
            return null;
        }
    }

    /// <summary>
    /// 计算 MD5 哈希值
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>MD5 哈希字符串</returns>
    private static string ComputeMd5(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = MD5.HashData(inputBytes);
        return Convert.ToHexString(hashBytes).ToLower();
    }
}
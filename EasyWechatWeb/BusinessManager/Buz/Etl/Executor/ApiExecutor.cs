using BusinessManager.Buz.Etl.Engine;
using EasyWeChatModels.Models.Etl;
using EasyWeChatModels.Models.Etl.NodeConfigs;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace BusinessManager.Buz.Etl.Executor;

/// <summary>
/// API节点执行器
/// 调用外部API接口
/// </summary>
public class ApiExecutor : BaseExecutor
{
    private readonly HttpClient _httpClient = new();

    /// <summary>
    /// 节点类型
    /// </summary>
    public override string NodeType => "api";

    /// <summary>
    /// 执行API调用
    /// </summary>
    public override async Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node)
    {
        var startTime = DateTime.Now;

        try
        {
            var config = ParseConfig<ApiNodeConfig>(node.Config);

            // 替换URL中的变量
            var upstreamVars = GetUpstreamVariables(context, node.Id);
            var apiUrl = ReplaceVariables(config.ApiUrl, upstreamVars);
            var apiBody = ReplaceVariables(config.ApiBody, upstreamVars);

            // 设置超时
            var timeout = config.Timeout ?? 30;
            _httpClient.Timeout = TimeSpan.FromSeconds(timeout);

            // 构建请求
            var request = BuildRequest(config, apiUrl, apiBody);

            // 发送请求
            var response = await _httpClient.SendAsync(request);

            // 读取响应
            var responseContent = await response.Content.ReadAsStringAsync();

            // 解析响应
            var responseData = ParseResponse(responseContent, config);

            // 构建输出
            var outputs = new Dictionary<string, object>
            {
                [config.OutputVariable] = responseData,
                ["_statusCode"] = response.StatusCode.ToString(),
                ["_isSuccess"] = response.IsSuccessStatusCode,
                ["_responseTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            if (!response.IsSuccessStatusCode)
            {
                return CreateFailResult($"API调用失败: {response.StatusCode}");
            }

            return CreateSuccessResult(outputs);
        }
        catch (Exception ex)
        {
            return CreateFailResult($"执行API调用失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 验证配置
    /// </summary>
    public override string? ValidateConfig(DagNode node)
    {
        var config = ParseConfig<ApiNodeConfig>(node.Config);

        if (string.IsNullOrEmpty(config.ApiUrl))
            return "API地址不能为空";

        if (string.IsNullOrEmpty(config.OutputVariable))
            return "输出变量名不能为空";

        return null;
    }

    private HttpRequestMessage BuildRequest(ApiNodeConfig config, string apiUrl, string? apiBody)
    {
        var method = config.ApiMethod.ToUpper() switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            _ => HttpMethod.Get
        };

        var request = new HttpRequestMessage(method, apiUrl);

        // 设置请求头
        if (!string.IsNullOrEmpty(config.ApiHeaders))
        {
            try
            {
                var headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(config.ApiHeaders);
                if (headers != null)
                {
                    foreach (var (key, value) in headers)
                    {
                        request.Headers.TryAddWithoutValidation(key, value);
                    }
                }
            }
            catch { }
        }

        // 设置请求体
        if (!string.IsNullOrEmpty(apiBody) && method != HttpMethod.Get)
        {
            request.Content = new StringContent(apiBody, Encoding.UTF8, "application/json");
        }

        return request;
    }

    private object ParseResponse(string responseContent, ApiNodeConfig config)
    {
        // 尝试解析为JSON
        try
        {
            if (responseContent.StartsWith("{") || responseContent.StartsWith("["))
            {
                return JsonConvert.DeserializeObject<object>(responseContent) ?? responseContent;
            }
        }
        catch { }

        return responseContent;
    }

    private string ReplaceVariables(string? template, Dictionary<string, object> variables)
    {
        if (string.IsNullOrEmpty(template))
            return template ?? string.Empty;

        var result = template;
        foreach (var (key, value) in variables)
        {
            var placeholder = $"${{{key}}}";
            if (result.Contains(placeholder))
            {
                result = result.Replace(placeholder, value?.ToString() ?? string.Empty);
            }
        }
        return result;
    }
}
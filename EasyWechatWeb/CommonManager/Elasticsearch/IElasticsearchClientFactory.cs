using System.Collections.Generic;
using Elastic.Clients.Elasticsearch;

namespace CommonManager.Elasticsearch;

/// <summary>
/// Elasticsearch 客户端工厂接口
/// 支持多环境连接管理
/// </summary>
public interface IElasticsearchClientFactory
{
    /// <summary>
    /// 获取指定环境的 Elasticsearch 客户端
    /// 客户端按环境缓存，避免重复创建
    /// </summary>
    /// <param name="environment">环境名称，如 Development、Production</param>
    /// <returns>Elasticsearch 客户端实例</returns>
    /// <exception cref="CommonManager.Error.BusinessException">环境不存在时抛出</exception>
    ElasticsearchClient GetClient(string environment);

    /// <summary>
    /// 获取所有可用的环境名称列表
    /// </summary>
    /// <returns>环境名称列表</returns>
    List<string> GetAvailableEnvironments();
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CommonManager.Error;
using EasyWeChatModels.Options;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;

namespace CommonManager.Elasticsearch;

/// <summary>
/// Elasticsearch 客户端工厂实现
/// 按环境缓存客户端实例，支持多环境动态切换
/// </summary>
public class ElasticsearchClientFactory : IElasticsearchClientFactory
{
    private readonly ElasticsearchQueryOptions _options;
    private readonly ConcurrentDictionary<string, ElasticsearchClient> _clients = new();

    public ElasticsearchClientFactory(IOptions<ElasticsearchQueryOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// 获取或创建指定环境的 Elasticsearch 客户端
    /// </summary>
    public ElasticsearchClient GetClient(string environment)
    {
        if (!_options.Environments.ContainsKey(environment))
        {
            throw new BusinessException($"Elasticsearch 环境 '{environment}' 不存在，可用环境: {string.Join(", ", _options.Environments.Keys)}");
        }

        return _clients.GetOrAdd(environment, env =>
        {
            var config = _options.Environments[env];
            var settings = new ElasticsearchClientSettings(new Uri(config.Url))
                .DefaultIndex(config.IndexPrefix)
                .RequestTimeout(TimeSpan.FromMilliseconds(config.DefaultTimeout));

            return new ElasticsearchClient(settings);
        });
    }

    /// <summary>
    /// 获取所有可用环境名称
    /// </summary>
    public List<string> GetAvailableEnvironments()
    {
        return _options.Environments.Keys.ToList();
    }
}
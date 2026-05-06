using System.Linq.Expressions;
using SqlSugar;

namespace CommonManager.Base;

/// <summary>
/// 非泛型服务基类，提供数据库上下文注入
/// </summary>
/// <remarks>
/// 适用于不需要泛型 CRUD 操作的服务，如多表查询、复杂业务逻辑等。
/// 数据库上下文 _db 通过属性注入方式提供。
/// </remarks>
/// <example>
/// <code>
/// public class MyService : BaseService, IMyService
/// {
///     public async Task&lt;List&lt;MyDto&gt;&gt; GetComplexDataAsync()
///     {
///         // 多表关联查询
///         return await _db.Queryable&lt;TableA, TableB&gt;((a, b) => new JoinQueryInfos(
///             JoinType.Left, a.Id == b.AId
///         ))
///         .Select((a, b) => new MyDto { ... })
///         .ToListAsync();
///     }
/// }
/// </code>
/// </example>
public class BaseService
{
    /// <summary>
    /// 数据库上下文（通过属性注入）
    /// </summary>
    /// <remarks>
    /// 使用 SqlSugar 的 ISqlSugarClient 接口，支持多种数据库操作。
    /// 通过 Autofac 的属性注入功能自动赋值。
    /// </remarks>
    public ISqlSugarClient _db { get; set; } = null!;
}

/// <summary>
/// 泛型仓储基类，提供通用的 CRUD 操作方法
/// </summary>
/// <typeparam name="T">实体类型，必须具有无参构造函数</typeparam>
/// <remarks>
/// 该类封装了常用的数据库操作方法，包括查询、插入、更新、删除等。
/// 继承此类的 Service 可以直接使用这些方法，减少重复代码。
/// 数据库上下文 _db 通过属性注入方式提供。
/// </remarks>
/// <example>
/// <code>
/// public class UserService : BaseService&lt;User&gt;
/// {
///     // 直接使用基类方法
///     public async Task&lt;User?&gt; GetByName(string name)
///     {
///         return await GetFirstAsync(u => u.Name == name);
///     }
/// }
/// </code>
/// </example>
public class BaseService<T> where T : class, new()
{
    /// <summary>
    /// 数据库上下文（通过属性注入）
    /// </summary>
    /// <remarks>
    /// 使用 SqlSugar 的 ISqlSugarClient 接口，支持多种数据库操作。
    /// 通过 Autofac 的属性注入功能自动赋值。
    /// </remarks>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>
    /// 查询所有记录
    /// </summary>
    /// <returns>包含所有实体的列表</returns>
    /// <remarks>
    /// 注意：数据量大时慎用，可能导致性能问题。
    /// 建议使用分页查询 GetPageListAsync 替代。
    /// </remarks>
    /// <example>
    /// <code>
    /// var allUsers = await userService.GetListAsync();
    /// </code>
    /// </example>
    public virtual async Task<List<T>> GetListAsync()
    {
        return await _db.Queryable<T>().ToListAsync();
    }

    /// <summary>
    /// 根据条件查询记录列表
    /// </summary>
    /// <param name="whereExpr">查询条件表达式</param>
    /// <returns>满足条件的实体列表</returns>
    /// <remarks>
    /// 支持复杂的 Lambda 表达式查询条件。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 查询状态为活跃的用户
    /// var activeUsers = await userService.GetListAsync(u => u.Status == "Active");
    ///
    /// // 查询创建时间在某个范围内的用户
    /// var recentUsers = await userService.GetListAsync(u => u.CreateTime >= startDate);
    /// </code>
    /// </example>
    public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpr)
    {
        return await _db.Queryable<T>().Where(whereExpr).ToListAsync();
    }

    /// <summary>
    /// 分页查询记录列表
    /// </summary>
    /// <param name="pageIndex">页码，从 1 开始</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="whereExpr">查询条件表达式，可选</param>
    /// <param name="orderBy">排序表达式，可选</param>
    /// <param name="isAsc">是否升序排序，默认为降序</param>
    /// <returns>包含分页信息的 PageResponse 对象</returns>
    /// <remarks>
    /// 支持条件筛选和排序，排序默认为降序。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 基础分页查询
    /// var page1 = await userService.GetPageListAsync(1, 10);
    ///
    /// // 带条件的分页查询
    /// var activePage = await userService.GetPageListAsync(
    ///     1, 10,
    ///     whereExpr: u => u.Status == "Active",
    ///     orderBy: u => u.CreateTime,
    ///     isAsc: false);
    /// </code>
    /// </example>
    public virtual async Task<PageResponse<T>> GetPageListAsync(
        int pageIndex,
        int pageSize,
        Expression<Func<T, bool>>? whereExpr = null,
        Expression<Func<T, object>>? orderBy = null,
        bool isAsc = false)
    {
        var query = _db.Queryable<T>();

        if (whereExpr != null)
            query = query.Where(whereExpr);

        if (orderBy != null)
            query = isAsc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

        var total = new RefAsync<int>();
        var items = await query.ToPageListAsync(pageIndex, pageSize, total);

        return PageResponse<T>.Create(items, total.Value, pageIndex, pageSize);
    }

    /// <summary>
    /// 根据 ID 查询单条记录
    /// </summary>
    /// <param name="id">实体主键 ID</param>
    /// <returns>查询到的实体对象，不存在则返回 null</returns>
    /// <remarks>
    /// 使用主键查询，是最高效的查询方式。
    /// 实体类需正确配置主键特性（[SugarColumn(IsPrimaryKey = true)]）。
    /// </remarks>
    /// <example>
    /// <code>
    /// var user = await userService.GetByIdAsync(Guid.Parse("..."));
    /// if (user != null)
    /// {
    ///     Console.WriteLine($"用户名: {user.Name}");
    /// }
    /// </code>
    /// </example>
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _db.Queryable<T>().InSingleAsync(id);
    }

    /// <summary>
    /// 根据条件查询单条记录
    /// </summary>
    /// <param name="whereExpr">查询条件表达式</param>
    /// <returns>满足条件的第一个实体对象，不存在则返回 null</returns>
    /// <remarks>
    /// 返回满足条件的第一个记录，适用于唯一性查询（如用户名、邮箱）。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 根据用户名查询
    /// var user = await userService.GetFirstAsync(u => u.UserName == "admin");
    ///
    /// // 根据邮箱查询
    /// var userByEmail = await userService.GetFirstAsync(u => u.Email == "test@example.com");
    /// </code>
    /// </example>
    public virtual async Task<T?> GetFirstAsync(Expression<Func<T, bool>> whereExpr)
    {
        return await _db.Queryable<T>().FirstAsync(whereExpr);
    }

    /// <summary>
    /// 插入单条记录并返回生成的ID
    /// </summary>
    /// <param name="entity">要插入的实体对象</param>
    /// <returns>新插入记录的主键 ID</returns>
    /// <remarks>
    /// 适用于Guid主键的表，返回插入后的ID。
    /// 实体的主键字段会在插入前自动生成Guid。
    /// </remarks>
    /// <example>
    /// <code>
    /// var newUser = new User { Name = "张三", Email = "zhangsan@example.com" };
    /// var newId = await userService.InsertAsync(newUser);
    /// Console.WriteLine($"新用户 ID: {newId}");
    /// </code>
    /// </example>
    public virtual async Task<Guid> InsertAsync(T entity)
    {
        await _db.Insertable(entity).ExecuteCommandAsync();
        // 对于Guid主键，实体在插入前已经设置了Guid值
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null && idProperty.PropertyType == typeof(Guid))
        {
            return (Guid)idProperty.GetValue(entity)!;
        }
        return Guid.Empty;
    }

    /// <summary>
    /// 批量插入记录
    /// </summary>
    /// <param name="entities">要插入的实体列表</param>
    /// <returns>受影响的行数</returns>
    /// <remarks>
    /// 批量插入比逐条插入效率更高，适用于大量数据导入。
    /// </remarks>
    /// <example>
    /// <code>
    /// var users = new List&lt;User&gt;
    /// {
    ///     new User { Name = "用户1" },
    ///     new User { Name = "用户2" },
    ///     new User { Name = "用户3" }
    /// };
    /// var affectedRows = await userService.InsertRangeAsync(users);
    /// </code>
    /// </example>
    public virtual async Task<int> InsertRangeAsync(List<T> entities)
    {
        return await _db.Insertable(entities).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新单条记录的所有字段
    /// </summary>
    /// <param name="entity">要更新的实体对象（需包含主键）</param>
    /// <returns>受影响的行数</returns>
    /// <remarks>
    /// 更新实体所有字段，未修改的字段也会被更新。
    /// 如需只更新部分字段，请使用 UpdateAsync(entity, columns) 方法。
    /// </remarks>
    /// <example>
    /// <code>
    /// var user = await userService.GetByIdAsync(1);
    /// if (user != null)
    /// {
    ///     user.Name = "新名字";
    ///     user.UpdateTime = DateTime.Now;
    ///     await userService.UpdateAsync(user);
    /// }
    /// </code>
    /// </example>
    public virtual async Task<int> UpdateAsync(T entity)
    {
        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新实体的指定字段
    /// </summary>
    /// <param name="entity">要更新的实体对象（需包含主键）</param>
    /// <param name="columns">要更新的字段表达式</param>
    /// <returns>受影响的行数</returns>
    /// <remarks>
    /// 只更新指定的字段，其他字段保持不变，性能更优。
    /// </remarks>
    /// <example>
    /// <code>
    /// var user = new User { Id = 1, Name = "新名字" };
    /// // 只更新 Name 字段
    /// await userService.UpdateAsync(user, u => u.Name);
    ///
    /// // 更新多个字段
    /// await userService.UpdateAsync(user, u => new User { Name = u.Name, UpdateTime = u.UpdateTime });
    /// </code>
    /// </example>
    public virtual async Task<int> UpdateAsync(T entity, Expression<Func<T, object>> columns)
    {
        return await _db.Updateable(entity).UpdateColumns(columns).ExecuteCommandAsync();
    }

    /// <summary>
    /// 根据 ID 删除记录
    /// </summary>
    /// <param name="id">要删除的记录主键 ID</param>
    /// <returns>受影响的行数（成功删除返回 1，不存在返回 0）</returns>
    /// <remarks>
    /// 物理删除，记录将从数据库中永久移除。
    /// </remarks>
    /// <example>
    /// <code>
    /// var affectedRows = await userService.DeleteAsync(Guid.Parse("..."));
    /// if (affectedRows > 0)
    /// {
    ///     Console.WriteLine("删除成功");
    /// }
    /// </code>
    /// </example>
    public virtual async Task<int> DeleteAsync(Guid id)
    {
        return await _db.Deleteable<T>().In(id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 根据条件删除记录
    /// </summary>
    /// <param name="whereExpr">删除条件表达式</param>
    /// <returns>受影响的行数</returns>
    /// <remarks>
    /// 批量删除满足条件的所有记录，谨慎使用。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 删除所有状态为"已注销"的用户
    /// await userService.DeleteAsync(u => u.Status == "Cancelled");
    /// </code>
    /// </example>
    public virtual async Task<int> DeleteAsync(Expression<Func<T, bool>> whereExpr)
    {
        return await _db.Deleteable<T>().Where(whereExpr).ExecuteCommandAsync();
    }

    /// <summary>
    /// 根据 ID 列表批量删除记录
    /// </summary>
    /// <param name="ids">要删除的记录 ID 列表</param>
    /// <returns>受影响的行数</returns>
    /// <remarks>
    /// 批量删除指定的多条记录。
    /// </remarks>
    /// <example>
    /// <code>
    /// var idsToDelete = new List&lt;Guid&gt; { Guid.Parse("..."), Guid.Parse("...") };
    /// var affectedRows = await userService.DeleteRangeAsync(idsToDelete);
    /// </code>
    /// </example>
    public virtual async Task<int> DeleteRangeAsync(List<Guid> ids)
    {
        return await _db.Deleteable<T>().In(ids).ExecuteCommandAsync();
    }

    /// <summary>
    /// 判断是否存在满足条件的记录
    /// </summary>
    /// <param name="whereExpr">查询条件表达式</param>
    /// <returns>存在返回 true，不存在返回 false</returns>
    /// <remarks>
    /// 比 Count > 0 更高效，只判断存在性而不计算总数。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 检查用户名是否已存在
    /// if (await userService.ExistsAsync(u => u.UserName == "admin"))
    /// {
    ///     Console.WriteLine("用户名已被使用");
    /// }
    /// </code>
    /// </example>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> whereExpr)
    {
        return await _db.Queryable<T>().AnyAsync(whereExpr);
    }

    /// <summary>
    /// 统计满足条件的记录数量
    /// </summary>
    /// <param name="whereExpr">查询条件表达式，可选（不提供则统计全部）</param>
    /// <returns>记录数量</returns>
    /// <remarks>
    /// 不提供条件时统计表中所有记录数。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 统计所有用户数量
    /// var total = await userService.CountAsync();
    ///
    /// // 统计活跃用户数量
    /// var activeCount = await userService.CountAsync(u => u.Status == "Active");
    /// </code>
    /// </example>
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? whereExpr = null)
    {
        var query = _db.Queryable<T>();
        if (whereExpr != null)
            query = query.Where(whereExpr);
        return await query.CountAsync();
    }

    /// <summary>
    /// 在事务中执行多个操作
    /// </summary>
    /// <param name="action">要执行的操作委托</param>
    /// <returns>事务是否成功完成</returns>
    /// <remarks>
    /// 所有操作在同一事务中执行，任一操作失败则全部回滚。
    /// 适用于需要保证数据一致性的场景。
    /// </remarks>
    /// <exception cref="Exception">操作失败时抛出异常，事务自动回滚</exception>
    /// <example>
    /// <code>
    /// // 转账示例：同时更新两个用户的余额
    /// await userService.ExecuteTransactionAsync(async () =>
    /// {
    ///     await UpdateAsync(fromUser);
    ///     await UpdateAsync(toUser);
    ///     await InsertAsync(transactionLog);
    /// });
    /// </code>
    /// </example>
    public virtual async Task<bool> ExecuteTransactionAsync(Func<Task> action)
    {
        try
        {
            _db.Ado.BeginTran();
            await action();
            _db.Ado.CommitTran();
            return true;
        }
        catch
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }
}
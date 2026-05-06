using MiniExcelLibs;
using MiniExcelLibs.OpenXml;

namespace CommonManager.Helper;

/// <summary>
/// Excel 导入导出帮助类，基于 MiniExcel 实现
/// </summary>
/// <remarks>
/// 该类提供 Excel 文件的读取和写入功能，支持泛型操作。
/// 基于 MiniExcel 库实现，具有低内存占用、高性能的特点，适合处理大量数据。
///
/// 特点：
/// - 支持 .xlsx 格式文件
/// - 自动根据类型属性名生成列标题
/// - 支持动态类型和泛型操作
/// - 内存占用低，适合大数据量处理
/// </remarks>
/// <example>
/// <code>
/// // 导出数据到文件
/// var users = new List&lt;User&gt; { new User { Name = "张三", Age = 25 } };
/// ExcelHelper.SaveToExcel("users.xlsx", users);
///
/// // 从文件读取数据
/// var userList = ExcelHelper.ReadFromExcel&lt;User&gt;("users.xlsx");
///
/// // 导出为字节数组（适合 Web API 返回）
/// var bytes = ExcelHelper.SaveToExcelBytes(users);
///
/// // 从字节数组读取（适合 Web API 上传）
/// var importedUsers = ExcelHelper.ReadFromExcelBytes&lt;User&gt;(bytes);
/// </code>
/// </example>
public static class ExcelHelper
{
    /// <summary>
    /// 将数据集合保存到 Excel 文件
    /// </summary>
    /// <typeparam name="T">数据类型，类的属性名将作为 Excel 列标题</typeparam>
    /// <param name="filePath">Excel 文件保存路径，必须是 .xlsx 格式</param>
    /// <param name="data">要保存的数据集合</param>
    /// <remarks>
    /// 使用 MiniExcel 的 SaveAs 方法保存数据。
    /// Excel 列标题根据类型属性的名称自动生成，可通过 [Description] 特性自定义列标题。
    ///
    /// 性能说明：
    /// - 使用流式写入，内存占用低
    /// - 适合导出大量数据（百万级）
    /// - 自动处理 null 值
    /// </remarks>
    /// <exception cref="ArgumentNullException">filePath 或 data 为 null 时抛出</exception>
    /// <exception cref="ArgumentException">filePath 格式不正确时抛出</exception>
    /// <example>
    /// <code>
    /// // 定义数据模型
    /// public class UserExportDto
    /// {
    ///     [Description("用户名")]
    ///     public string UserName { get; set; }
    ///
    ///     [Description("真实姓名")]
    ///     public string RealName { get; set; }
    ///
    ///     [Description("手机号")]
    ///     public string Phone { get; set; }
    /// }
    ///
    /// // 导出数据
    /// var users = new List&lt;UserExportDto&gt;
    /// {
    ///     new UserExportDto { UserName = "admin", RealName = "管理员", Phone = "13800138000" },
    ///     new UserExportDto { UserName = "user1", RealName = "用户1", Phone = "13900139000" }
    /// };
    ///
    /// ExcelHelper.SaveToExcel("D:\\exports\\users.xlsx", users);
    /// </code>
    /// </example>
    public static void SaveToExcel<T>(string filePath, IEnumerable<T> data)
    {
        var config = new OpenXmlConfiguration
        {
            TableStyles = TableStyles.Default
        };
        MiniExcel.SaveAs(filePath, data, configuration: config);
    }

    /// <summary>
    /// 从 Excel 文件读取数据
    /// </summary>
    /// <typeparam name="T">目标数据类型，必须是无参构造函数的类</typeparam>
    /// <param name="filePath">Excel 文件路径，必须是 .xlsx 格式</param>
    /// <returns>读取的数据集合，文件不存在或为空时返回空集合</returns>
    /// <remarks>
    /// 使用 MiniExcel 的 Query 方法读取数据。
    /// Excel 列标题与类型属性名进行匹配（不区分大小写）。
    ///
    /// 注意事项：
    /// - 类型 T 必须有无参构造函数
    /// - 只读取第一个工作表
    /// - 自动跳过空行
    /// - 类型不匹配时返回默认值
    /// </remarks>
    /// <exception cref="ArgumentNullException">filePath 为 null 时抛出</exception>
    /// <exception cref="FileNotFoundException">文件不存在时抛出</exception>
    /// <example>
    /// <code>
    /// // 读取用户数据
    /// var users = ExcelHelper.ReadFromExcel&lt;UserImportDto&gt;("D:\\imports\\users.xlsx");
    ///
    /// foreach (var user in users)
    /// {
    ///     Console.WriteLine($"用户名: {user.UserName}, 姓名: {user.RealName}");
    /// }
    ///
    /// // 处理读取结果
    /// if (users.Any())
    /// {
    ///     // 数据导入处理...
    /// }
    /// </code>
    /// </example>
    public static IEnumerable<T> ReadFromExcel<T>(string filePath) where T : class, new()
    {
        var config = new OpenXmlConfiguration
        {
            FillMergedCells = true
        };
        return MiniExcel.Query<T>(filePath, configuration: config);
    }

    /// <summary>
    /// 将数据集合导出为 Excel 字节数组
    /// </summary>
    /// <typeparam name="T">数据类型，类的属性名将作为 Excel 列标题</typeparam>
    /// <param name="data">要导出的数据集合</param>
    /// <returns>Excel 文件的字节数组，可用于 HTTP 响应返回</returns>
    /// <remarks>
    /// 使用内存流生成 Excel 文件字节数组。
    /// 适合 Web API 场景，直接返回文件内容而不保存到磁盘。
    ///
    /// 使用场景：
    /// - Web API 文件下载
    /// - 内存缓存
    /// - 临时导出
    ///
    /// 性能说明：
    /// - 内存占用较高，不适合超大数据量
    /// - 建议数据量超过 10 万条时使用文件方式
    /// </remarks>
    /// <exception cref="ArgumentNullException">data 为 null 时抛出</exception>
    /// <example>
    /// <code>
    /// // 在 Web API 控制器中使用
    /// [HttpGet("export")]
    /// public IActionResult ExportUsers()
    /// {
    ///     var users = _userService.GetList();
    ///     var bytes = ExcelHelper.SaveToExcelBytes(users);
    ///     return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
    /// }
    /// </code>
    /// </example>
    public static byte[] SaveToExcelBytes<T>(IEnumerable<T> data)
    {
        using var stream = new MemoryStream();
        var config = new OpenXmlConfiguration
        {
            TableStyles = TableStyles.Default
        };
        stream.SaveAs(data, configuration: config);
        return stream.ToArray();
    }

    /// <summary>
    /// 从 Excel 字节数组读取数据
    /// </summary>
    /// <typeparam name="T">目标数据类型，必须是无参构造函数的类</typeparam>
    /// <param name="bytes">Excel 文件的字节数组，来自上传或其他来源</param>
    /// <returns>读取的数据集合，字节数组为空时返回空集合</returns>
    /// <remarks>
    /// 使用内存流读取 Excel 文件字节数组。
    /// 适合 Web API 场景，处理上传的 Excel 文件。
    ///
    /// 使用场景：
    /// - Web API 文件上传导入
    /// - 内存数据处理
    /// - 接收外部系统数据
    ///
    /// 注意事项：
    /// - 类型 T 必须有无参构造函数
    /// - 只读取第一个工作表
    /// - 自动跳过空行
    /// </remarks>
    /// <exception cref="ArgumentNullException">bytes 为 null 时抛出</exception>
    /// <exception cref="ArgumentException">字节数组不是有效 Excel 格式时抛出</exception>
    /// <example>
    /// <code>
    /// // 在 Web API 控制器中使用
    /// [HttpPost("import")]
    /// public IActionResult ImportUsers(IFormFile file)
    /// {
    ///     using var stream = file.OpenReadStream();
    ///     var bytes = new byte[stream.Length];
    ///     stream.Read(bytes, 0, bytes.Length);
    ///
    ///     var users = ExcelHelper.ReadFromExcelBytes&lt;UserImportDto&gt;(bytes);
    ///
    ///     // 处理导入数据...
    ///     return Ok(new { Count = users.Count() });
    /// }
    /// </code>
    /// </example>
    public static IEnumerable<T> ReadFromExcelBytes<T>(byte[] bytes) where T : class, new()
    {
        using var stream = new MemoryStream(bytes);
        var config = new OpenXmlConfiguration
        {
            FillMergedCells = true
        };
        return stream.Query<T>(configuration: config);
    }
}
using EasyWeChatModels.Dto.AntWorkflow;

namespace BusinessManager.Buz.IService;

/// <summary>
/// Ant流程表单字段服务接口
/// </summary>
public interface IAntWorkflowFormFieldService
{
    /// <summary>
    /// 获取业务类型的表单字段列表
    /// </summary>
    Task<List<AntFormFieldDto>> GetListByBusinessTypeAsync(string businessType);

    /// <summary>
    /// 添加表单字段
    /// </summary>
    Task<Guid> AddAsync(CreateFormFieldDto dto);

    /// <summary>
    /// 更新表单字段
    /// </summary>
    Task<int> UpdateAsync(UpdateFormFieldDto dto);

    /// <summary>
    /// 删除表单字段
    /// </summary>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 批量更新字段顺序
    /// </summary>
    Task<int> UpdateOrderAsync(List<FormFieldOrderDto> orders);
}
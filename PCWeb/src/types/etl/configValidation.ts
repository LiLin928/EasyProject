/**
 * ETL 配置验证模块
 * 用于验证各种节点配置的有效性
 */
import type { DagNode } from './pipeline'
import type {
  DataSourceNodeConfig,
  SqlNodeConfig,
  TransformNodeConfig,
  OutputNodeConfig,
  ApiNodeConfig,
  FileNodeConfig,
  ScriptNodeConfig,
  ConditionNodeConfig,
  ParallelNodeConfig,
  NotificationNodeConfig,
  SubflowNodeConfig,
} from './taskNode'
import { TaskNodeType } from './taskNode'

/**
 * 验证结果接口
 */
export interface ValidationResult {
  valid: boolean
  errors: string[]
}

/**
 * 验证公共配置
 * @param node DAG 节点
 * @returns 验证结果
 */
export function validateBaseConfig(node: DagNode): ValidationResult {
  const errors: string[] = []

  // name: 不能为空
  if (!node.name || node.name.trim() === '') {
    errors.push('节点名称不能为空')
  }

  // timeout: 不能小于 10 秒
  if (node.timeout !== undefined && node.timeout < 10) {
    errors.push('超时时间不能小于 10 秒')
  }

  // retryTimes: 不能超过 10 次
  if (node.retryTimes !== undefined && node.retryTimes > 10) {
    errors.push('重试次数不能超过 10 次')
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证数据源节点配置
 * @param config 数据源节点配置
 * @returns 验证结果
 */
export function validateDataSourceConfig(config: DataSourceNodeConfig): ValidationResult {
  const errors: string[] = []

  // datasourceId: 必填
  if (!config.datasourceId || config.datasourceId.trim() === '') {
    errors.push('数据源不能为空')
  }

  // queryType === 'table' 时 tableName 必填
  if (config.queryType === 'table') {
    if (!config.tableName || config.tableName.trim() === '') {
      errors.push('查询类型为表查询时，表名不能为空')
    }
  }

  // queryType === 'sql' 时 sql 必填
  if (config.queryType === 'sql') {
    if (!config.sql || config.sql.trim() === '') {
      errors.push('查询类型为 SQL 查询时，SQL 语句不能为空')
    }
  }

  // outputVariable: 必填
  if (!config.outputVariable || config.outputVariable.trim() === '') {
    errors.push('输出变量名不能为空')
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证 SQL 节点配置
 * @param config SQL 节点配置
 * @returns 验证结果
 */
export function validateSqlConfig(config: SqlNodeConfig): ValidationResult {
  const errors: string[] = []

  // datasourceId: 必填
  if (!config.datasourceId || config.datasourceId.trim() === '') {
    errors.push('数据源不能为空')
  }

  // sqlType: 必填
  if (!config.sqlType) {
    errors.push('SQL 类型不能为空')
  }

  // sql: 必填
  if (!config.sql || config.sql.trim() === '') {
    errors.push('SQL 语句不能为空')
  }

  // sqlType === 'query' 时 outputVariable 必填
  if (config.sqlType === 'query') {
    if (!config.outputVariable || config.outputVariable.trim() === '') {
      errors.push('查询类型 SQL 必须指定输出变量名')
    }
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证数据转换节点配置
 * @param config 转换节点配置
 * @returns 验证结果
 */
export function validateTransformConfig(config: TransformNodeConfig): ValidationResult {
  const errors: string[] = []

  // transformType: 必填
  if (!config.transformType) {
    errors.push('转换类型不能为空')
  }

  // inputVariable: 必填
  if (!config.inputVariable || config.inputVariable.trim() === '') {
    errors.push('输入变量名不能为空')
  }

  // outputVariable: 必填
  if (!config.outputVariable || config.outputVariable.trim() === '') {
    errors.push('输出变量名不能为空')
  }

  // transformType === 'mapping' 时 fieldMapping 不能为空
  if (config.transformType === 'mapping') {
    if (!config.fieldMapping || config.fieldMapping.length === 0) {
      errors.push('映射转换类型必须配置字段映射')
    }
  }

  // transformType === 'aggregate' 时 aggregateConfig.aggregations 不能为空
  if (config.transformType === 'aggregate') {
    if (!config.aggregateConfig || !config.aggregateConfig.aggregations || config.aggregateConfig.aggregations.length === 0) {
      errors.push('聚合转换类型必须配置聚合字段')
    }
  }

  // transformType === 'script' 时 script 必填
  if (config.transformType === 'script') {
    if (!config.script || config.script.trim() === '') {
      errors.push('脚本转换类型必须配置转换脚本')
    }
  }

  // transformType === 'filter' 时 filterExpression 必填
  if (config.transformType === 'filter') {
    if (!config.filterExpression || config.filterExpression.trim() === '') {
      errors.push('过滤转换类型必须配置过滤表达式')
    }
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证数据输出节点配置
 * @param config 输出节点配置
 * @returns 验证结果
 */
export function validateOutputConfig(config: OutputNodeConfig): ValidationResult {
  const errors: string[] = []

  // datasourceId: 必填
  if (!config.datasourceId || config.datasourceId.trim() === '') {
    errors.push('数据源不能为空')
  }

  // outputType: 必填
  if (!config.outputType) {
    errors.push('输出类型不能为空')
  }

  // tableName: 必填
  if (!config.tableName || config.tableName.trim() === '') {
    errors.push('目标表名不能为空')
  }

  // inputVariable: 必填
  if (!config.inputVariable || config.inputVariable.trim() === '') {
    errors.push('输入变量名不能为空')
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证 API 节点配置
 * @param config API 节点配置
 * @returns 验证结果
 */
export function validateApiConfig(config: ApiNodeConfig): ValidationResult {
  const errors: string[] = []

  // apiUrl: 必填且格式正确
  if (!config.apiUrl || config.apiUrl.trim() === '') {
    errors.push('API 地址不能为空')
  } else {
    // 验证 URL 格式
    try {
      new URL(config.apiUrl)
    } catch {
      errors.push('API 地址格式不正确')
    }
  }

  // apiMethod: 必填
  if (!config.apiMethod) {
    errors.push('请求方法不能为空')
  }

  // apiBodyType === 'json' 时 apiBody 必须是有效 JSON
  if (config.apiBodyType === 'json' && config.apiBody) {
    try {
      JSON.parse(config.apiBody)
    } catch {
      errors.push('请求体不是有效的 JSON 格式')
    }
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证文件节点配置
 * @param config 文件节点配置
 * @returns 验证结果
 */
export function validateFileConfig(config: FileNodeConfig): ValidationResult {
  const errors: string[] = []

  // fileOperation: 必填
  if (!config.fileOperation) {
    errors.push('文件操作类型不能为空')
  }

  // fileType: 必填
  if (!config.fileType) {
    errors.push('文件类型不能为空')
  }

  // filePath: 必填（对于 read, write, delete 操作）
  if (['read', 'write', 'delete'].includes(config.fileOperation)) {
    if (!config.filePath || config.filePath.trim() === '') {
      errors.push('文件路径不能为空')
    }
  }

  // 对于写入操作，需要 inputVariable
  if (config.fileOperation === 'write') {
    if (!config.inputVariable || config.inputVariable.trim() === '') {
      errors.push('写入操作需要指定输入变量')
    }
  }

  // 对于读取操作，需要 outputVariable
  if (config.fileOperation === 'read') {
    if (!config.outputVariable || config.outputVariable.trim() === '') {
      errors.push('读取操作需要指定输出变量')
    }
  }

  // 对于 move, copy 操作，需要 localPath 或目标路径
  if (['move', 'copy'].includes(config.fileOperation)) {
    if (!config.localPath || config.localPath.trim() === '') {
      errors.push('移动/复制操作需要指定目标路径')
    }
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证脚本节点配置
 * @param config 脚本节点配置
 * @returns 验证结果
 */
export function validateScriptConfig(config: ScriptNodeConfig): ValidationResult {
  const errors: string[] = []

  // scriptType: 必填
  if (!config.scriptType) {
    errors.push('脚本类型不能为空')
  }

  // script 或 scriptPath: 必须有一个
  const hasScript = config.script && config.script.trim() !== ''
  const hasScriptPath = config.scriptPath && config.scriptPath.trim() !== ''

  if (!hasScript && !hasScriptPath) {
    errors.push('脚本内容或脚本路径必须指定一个')
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证条件节点配置
 * @param config 条件节点配置
 * @returns 验证结果
 */
export function validateConditionConfig(config: ConditionNodeConfig): ValidationResult {
  const errors: string[] = []

  // conditionType: 必填
  if (!config.conditionType) {
    errors.push('条件类型不能为空')
  }

  // branches: 不能为空
  if (!config.branches || config.branches.length === 0) {
    errors.push('分支配置不能为空')
  }

  // conditionType === 'simple' 时 conditions.rules 不能为空
  if (config.conditionType === 'simple') {
    if (!config.conditions || !config.conditions.rules || config.conditions.rules.length === 0) {
      errors.push('简单条件类型必须配置条件规则')
    }
  }

  // conditionType === 'expression' 时 expression 必填
  if (config.conditionType === 'expression') {
    if (!config.expression || config.expression.trim() === '') {
      errors.push('表达式条件类型必须配置条件表达式')
    }
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证并行节点配置
 * @param config 并行节点配置
 * @returns 验证结果
 */
export function validateParallelConfig(config: ParallelNodeConfig): ValidationResult {
  const errors: string[] = []

  // parallelType: 必填
  if (!config.parallelType) {
    errors.push('并行类型不能为空')
  }

  // parallelType === 'fixed' 时 branches 不能为空
  if (config.parallelType === 'fixed') {
    if (!config.branches || config.branches.length === 0) {
      errors.push('固定并行类型必须配置分支')
    }
  }

  // parallelType === 'dynamic' 时 dynamicSource 必填
  if (config.parallelType === 'dynamic') {
    if (!config.dynamicSource || config.dynamicSource.trim() === '') {
      errors.push('动态并行类型必须配置动态数据源')
    }
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证通知节点配置
 * @param config 通知节点配置
 * @returns 验证结果
 */
export function validateNotificationConfig(config: NotificationNodeConfig): ValidationResult {
  const errors: string[] = []

  // notificationType: 必填
  if (!config.notificationType) {
    errors.push('通知类型不能为空')
  }

  // triggerOn: 必填
  if (!config.triggerOn) {
    errors.push('触发条件不能为空')
  }

  // notificationType === 'email' 时 recipients 和 subject 必填
  if (config.notificationType === 'email') {
    if (!config.emailConfig) {
      errors.push('邮件通知配置不能为空')
    } else {
      if (!config.emailConfig.recipients || config.emailConfig.recipients.length === 0) {
        errors.push('邮件收件人不能为空')
      }
      if (!config.emailConfig.subject || config.emailConfig.subject.trim() === '') {
        errors.push('邮件主题不能为空')
      }
    }
  }

  // notificationType === 'webhook' 时 url 必填
  if (config.notificationType === 'webhook') {
    if (!config.webhookConfig) {
      errors.push('Webhook 通知配置不能为空')
    } else {
      if (!config.webhookConfig.url || config.webhookConfig.url.trim() === '') {
        errors.push('Webhook URL 不能为空')
      }
    }
  }

  // notificationType === 'message' 时 recipients 和 title 必填
  if (config.notificationType === 'message') {
    if (!config.messageConfig) {
      errors.push('消息通知配置不能为空')
    } else {
      if (!config.messageConfig.recipients || config.messageConfig.recipients.length === 0) {
        errors.push('消息接收人不能为空')
      }
      if (!config.messageConfig.title || config.messageConfig.title.trim() === '') {
        errors.push('消息标题不能为空')
      }
    }
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证子流程节点配置
 * @param config 子流程节点配置
 * @returns 验证结果
 */
export function validateSubflowConfig(config: SubflowNodeConfig): ValidationResult {
  const errors: string[] = []

  // pipelineId: 必填
  if (!config.pipelineId || config.pipelineId.trim() === '') {
    errors.push('子流程不能为空')
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 验证函数映射
 */
const validationMap: Record<TaskNodeType, (config: unknown) => ValidationResult> = {
  [TaskNodeType.DATASOURCE]: (config) => validateDataSourceConfig(config as DataSourceNodeConfig),
  [TaskNodeType.SQL]: (config) => validateSqlConfig(config as SqlNodeConfig),
  [TaskNodeType.TRANSFORM]: (config) => validateTransformConfig(config as TransformNodeConfig),
  [TaskNodeType.OUTPUT]: (config) => validateOutputConfig(config as OutputNodeConfig),
  [TaskNodeType.API]: (config) => validateApiConfig(config as ApiNodeConfig),
  [TaskNodeType.FILE]: (config) => validateFileConfig(config as FileNodeConfig),
  [TaskNodeType.SCRIPT]: (config) => validateScriptConfig(config as ScriptNodeConfig),
  [TaskNodeType.CONDITION]: (config) => validateConditionConfig(config as ConditionNodeConfig),
  [TaskNodeType.PARALLEL]: (config) => validateParallelConfig(config as ParallelNodeConfig),
  [TaskNodeType.NOTIFICATION]: (config) => validateNotificationConfig(config as NotificationNodeConfig),
  [TaskNodeType.SUBFLOW]: (config) => validateSubflowConfig(config as SubflowNodeConfig),
}

/**
 * 综合验证函数
 * 同时验证公共配置和特定节点配置
 * @param node DAG 节点
 * @returns 验证结果
 */
export function validateNodeConfig(node: DagNode): ValidationResult {
  const errors: string[] = []

  // 验证公共配置
  const baseResult = validateBaseConfig(node)
  errors.push(...baseResult.errors)

  // 验证特定节点配置
  const validator = validationMap[node.type]
  if (validator) {
    const configResult = validator(node.config)
    errors.push(...configResult.errors)
  } else {
    errors.push(`未知的节点类型: ${node.type}`)
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

/**
 * 批量验证多个节点
 * @param nodes DAG 节点数组
 * @returns 验证结果映射（节点 ID -> 验证结果）
 */
export function validateNodes(nodes: DagNode[]): Record<string, ValidationResult> {
  const results: Record<string, ValidationResult> = {}

  for (const node of nodes) {
    results[node.id] = validateNodeConfig(node)
  }

  return results
}

/**
 * 检查是否有任何验证错误
 * @param results 验证结果映射
 * @returns 是否有错误
 */
export function hasValidationErrors(results: Record<string, ValidationResult>): boolean {
  return Object.values(results).some((result) => !result.valid)
}

/**
 * 获取所有验证错误信息
 * @param nodes DAG 节点数组
 * @param results 验证结果映射
 * @returns 错误信息数组（包含节点名称）
 */
export function getValidationErrors(
  nodes: DagNode[],
  results: Record<string, ValidationResult>,
): Array<{ nodeId: string; nodeName: string; errors: string[] }> {
  const errors: Array<{ nodeId: string; nodeName: string; errors: string[] }> = []

  for (const node of nodes) {
    const result = results[node.id]
    if (result && !result.valid) {
      errors.push({
        nodeId: node.id,
        nodeName: node.name,
        errors: result.errors,
      })
    }
  }

  return errors
}
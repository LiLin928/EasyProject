/**
 * 数据源类型定义
 */

/**
 * 数据源类型
 */
export type DatasourceType = 'mysql' | 'postgresql' | 'oracle' | 'sqlserver' | 'clickhouse' | 'mongodb' | 'redis' | 'elasticsearch' | 'http_api' | 'ftp' | 'sftp' | 'hive'

/**
 * 数据源类型枚举值（用于常量引用）
 */
export const DatasourceType = {
  MYSQL: 'mysql' as DatasourceType,
  POSTGRESQL: 'postgresql' as DatasourceType,
  ORACLE: 'oracle' as DatasourceType,
  SQLSERVER: 'sqlserver' as DatasourceType,
  CLICKHOUSE: 'clickhouse' as DatasourceType,
  MONGODB: 'mongodb' as DatasourceType,
  REDIS: 'redis' as DatasourceType,
  ELASTICSEARCH: 'elasticsearch' as DatasourceType,
  HTTP_API: 'http_api' as DatasourceType,
  FTP: 'ftp' as DatasourceType,
  SFTP: 'sftp' as DatasourceType,
  HIVE: 'hive' as DatasourceType,
}

/**
 * 数据源状态
 */
export type DatasourceStatus = 'connected' | 'disconnected' | 'error'

/**
 * 数据源状态枚举值
 */
export const DatasourceStatus = {
  ACTIVE: 'connected' as DatasourceStatus,
  INACTIVE: 'disconnected' as DatasourceStatus,
  ERROR: 'error' as DatasourceStatus,
}

/**
 * 数据源信息
 */
export interface DataSource {
  id: string
  name: string
  type: DatasourceType
  host?: string
  port?: number
  database?: string
  username?: string
  status: DatasourceStatus
  lastConnectionTime?: string
  description?: string
  createTime?: string
  updateTime?: string
  creatorId?: string
  creatorName?: string
  /** 扩展配置（用于非数据库类型的数据源） */
  config?: Record<string, any>
}

/**
 * 数据源类型别名
 */
export type Datasource = DataSource

/**
 * 数据源列表查询参数
 */
export interface DataSourceListParams {
  pageIndex?: number
  pageSize?: number
  name?: string
  type?: DatasourceType
}

/**
 * 创建数据源参数
 */
export interface CreateDataSourceParams {
  name: string
  type: DatasourceType
  host: string
  port: number
  database: string
  username: string
  password: string
  description?: string
}

/**
 * 更新数据源参数
 */
export interface UpdateDataSourceParams {
  id: string
  name?: string
  host?: string
  port?: number
  database?: string
  username?: string
  password?: string
  description?: string
}

/**
 * 数据库类型信息
 */
export interface DbTypeInfo {
  code: string
  name: string
  defaultPort: number
}

/**
 * 测试连接结果
 */
export interface TestConnectionResult {
  success: boolean
  message: string
  serverVersion?: string
  connectionTime?: number
}
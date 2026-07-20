// 基础类型
export * from './response'
export * from './user'
export * from './menu'
export * from './global'
export * from './tagsView'

// 报表和列模板
export * from './report'
export * from './columnTemplate'

// 业务模块类型（重命名以避免冲突）
export type { ReviewStatus as ProductReviewStatus } from './product'
export * from './product'

// 工作流类型（重命名以避免冲突）
export type { ApproverSetType, ConditionBranch, ConditionOperator, ExamineMode, FlowPermission, NodeUser, NotificationRecipient, ParallelBranch } from './flowNode'
export * from './flowNode'

// 订单和客户
export * from './order'
export * from './customer'
export * from './banner'

// 枚举和ETL
export * from './enums'
export * from './etl'

// 工作流和审计
export * from './antWorkflow'
export * from './operateLog'
export * from './task'
export * from './announcement'
export * from './businessAuditPoint'

// 桌面组件（重命名以避免冲突）
export type { DataSourceType as DesktopDataSourceType } from './desktopWidget'
export * from './desktopWidget'

// 大屏（重命名以避免冲突）
export type { DataSourceType as ScreenDataSourceType, DataSource as ScreenDataSource } from './screen'
export * from './screen'
/**
 * Mock 模块配置
 * true: 使用 Mock 数据
 * false: 使用真实后端 API
 *
 * 切换说明：
 * 1. 将对应模块设置为 false 即可切换到真实 API
 * 2. 真实 API 需要后端实现对应的接口
 * 3. Mock 数据响应格式与真实 API 保持一致，便于无缝切换
 */
export interface MockConfig {
  auth: boolean      // 认证模块（建议 true）
  basic: boolean     // 用户/角色/菜单/字典
  product: boolean   // 商品管理
  order: boolean     // 订单管理（含退款）
  customer: boolean  // 客户管理
  banner: boolean    // 轮播图管理
  antWorkflow: boolean // Ant Workflow（DAG 工作流）
  report: boolean    // 报表
  screen: boolean    // 大屏
  etl: boolean       // ETL
  upload: boolean    // 文件上传
  logQuery: boolean  // 日志查询
  task: boolean      // 定时任务管理
}

const mockConfig: MockConfig = {
  auth: false,        // 认证使用真实后端 API
  basic: false,       // 基础管理使用真实 API
  product: false,     // 商品管理使用真实 API
  order: false,       // 订单管理使用真实 API
  customer: false,    // 客户管理使用真实 API
  banner: false,      // 轮播图使用真实 API
  antWorkflow: false, // Ant Workflow 使用真实 API
  report: false,      // 报表使用真实 API
  screen: false,      // 大屏使用真实 API
  etl: false,         // ETL 使用真实 API
  upload: false,      // 上传使用真实 API
  logQuery: false,    // 日志查询使用真实 API
  task: false,        // 定时任务使用真实 API
}

export default mockConfig
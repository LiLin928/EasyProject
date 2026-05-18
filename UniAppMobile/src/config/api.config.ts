// config/api.config.ts

/** API 路径配置 */
export const API_PATHS = {
  // 认证
  AUTH_LOGIN: '/api/auth/login',
  AUTH_LOGOUT: '/api/auth/logout',
  USER_INFO: '/api/user/info',

  // 工作流
  WORKFLOW_TODO_LIST: '/api/workflow/todo/list',
  WORKFLOW_DONE_LIST: '/api/workflow/done/list',
  WORKFLOW_CC_LIST: '/api/workflow/cc/list',
  WORKFLOW_DETAIL: '/api/workflow/detail',
  WORKFLOW_APPROVE: '/api/workflow/approve',

  // 商品
  PRODUCT_LIST: '/api/product/list',
  PRODUCT_DETAIL: '/api/product/detail',
  PRODUCT_ADD: '/api/product/add',
  PRODUCT_UPDATE: '/api/product/update',
  PRODUCT_DELETE: '/api/product/delete',
  STOCK_LIST: '/api/product/stock/list',

  // 基础管理
  USER_LIST: '/api/user/list',
  ROLE_LIST: '/api/role/list',
  DEPARTMENT_LIST: '/api/department/list',

  // 报表
  REPORT_LIST: '/api/report/list',
  REPORT_CATEGORIES: '/api/report/categories',
  REPORT_DETAIL: '/api/report/detail',
  REPORT_DATA: '/api/report/data',

  // 大屏
  SCREEN_LIST: '/api/screen/list',
  SCREEN_DETAIL: '/api/screen/detail',

  // 日志
  LOG_LIST: '/api/log/list',
  LOG_DETAIL: '/api/log/detail',
  LOG_CLEAR: '/api/log/clear',
} as const
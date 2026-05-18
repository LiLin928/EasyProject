// config/api.config.ts

/** API 路径配置 */
export const API_PATHS = {
  // 认证
  AUTH_LOGIN: '/api/auth/login',
  AUTH_LOGOUT: '/api/auth/logout',
  AUTH_REFRESH: '/api/auth/refresh',

  // 用户
  USER_INFO: '/api/user/info',        // 获取当前用户信息
  USER_LIST: '/api/user/list',        // 用户列表
  USER_DETAIL: '/api/user/detail',    // 用户详情
  USER_ADD: '/api/user/add',          // 添加用户
  USER_UPDATE: '/api/user/update',    // 更新用户
  USER_DELETE: '/api/user/delete',    // 删除用户

  // 工作流（对应后端 AntWorkflow 模块）
  // 任务相关
  WORKFLOW_TODO_LIST: '/api/ant-workflow/task/todo',
  WORKFLOW_DONE_LIST: '/api/ant-workflow/task/done',
  WORKFLOW_CC_LIST: '/api/ant-workflow/task/cc',
  WORKFLOW_TASK_DETAIL: '/api/ant-workflow/task/detail',
  WORKFLOW_APPROVE: '/api/ant-workflow/task/approve',
  WORKFLOW_REJECT: '/api/ant-workflow/task/reject',
  WORKFLOW_TRANSFER: '/api/ant-workflow/task/transfer',
  WORKFLOW_ADD_SIGNER: '/api/ant-workflow/task/add-signer',
  WORKFLOW_CC_READ: '/api/ant-workflow/task/cc-read',
  // 流程运行时
  WORKFLOW_START: '/api/ant-workflow/runtime/start',
  WORKFLOW_MY_INSTANCES: '/api/ant-workflow/runtime/my-instances',
  WORKFLOW_INSTANCE_DETAIL: '/api/ant-workflow/runtime/detail',
  WORKFLOW_CANCEL: '/api/ant-workflow/runtime/cancel',
  WORKFLOW_LOGS: '/api/ant-workflow/runtime/logs',

  // 商品
  PRODUCT_LIST: '/api/product/list',
  PRODUCT_DETAIL: '/api/product/detail',
  PRODUCT_ADD: '/api/product/add',
  PRODUCT_UPDATE: '/api/product/update',
  PRODUCT_DELETE: '/api/product/delete',
  PRODUCT_CATEGORY_LIST: '/api/product/category/list',
  PRODUCT_STOCK_LIST: '/api/product/stock/list',
  STOCK_LIST: '/api/product/stock/list',

  // 基础管理
  ROLE_LIST: '/api/role/list',
  ROLE_DETAIL: '/api/role/detail',
  ROLE_ADD: '/api/role/add',
  ROLE_UPDATE: '/api/role/update',
  ROLE_DELETE: '/api/role/delete',

  DEPARTMENT_TREE: '/api/department/tree',
  DEPARTMENT_DETAIL: '/api/department/detail',
  DEPARTMENT_ADD: '/api/department/add',
  DEPARTMENT_UPDATE: '/api/department/update',
  DEPARTMENT_DELETE: '/api/department/delete',

  MENU_LIST: '/api/menu/list',
  MENU_USER_MENU: '/api/menu/user-menu',
  MENU_DETAIL: '/api/menu/detail',

  // 报表
  REPORT_LIST: '/api/report/list',
  REPORT_CATEGORIES: '/api/report/categories',
  REPORT_DETAIL: '/api/report/detail',
  REPORT_DATA: '/api/report/preview',

  // 大屏
  SCREEN_LIST: '/api/screen/list',
  SCREEN_DETAIL: '/api/screen/detail',

  // 日志
  LOG_QUERY: '/api/log/query',
  LOG_DETAIL: '/api/log/detail',
  LOG_ENVIRONMENTS: '/api/log/environments',

  // 文件
  FILE_UPLOAD: '/api/file/upload',
  FILE_DOWNLOAD: '/api/file/download',
} as const
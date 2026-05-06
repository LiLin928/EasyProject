// 菜单数据 Mock（字段名与后端 MenuDto 一致）

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * Mock 菜单类型（与后端 MenuDto 字段对应）
 */
interface MockMenu {
  id: string
  parentId: string | null
  menuName: string          // 显示标题
  menuCode: string | null   // 路由名称
  path: string | null
  component: string | null
  icon: string | null
  sort: number
  type: number              // 1-菜单，2-按钮
  status: 0 | 1
  hidden: 0 | 1
  affix: 0 | 1
  createTime: string
  updateTime: string | null
  children?: MockMenu[]
}

/**
 * 格式化当前时间
 */
function formatNow(): string {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  const hours = String(now.getHours()).padStart(2, '0')
  const minutes = String(now.getMinutes()).padStart(2, '0')
  const seconds = String(now.getSeconds()).padStart(2, '0')
  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`
}

// 初始菜单数据（使用 GUID，字段名与后端一致）
const menus: MockMenu[] = [
  // 工作台（固定标签页）
  {
    id: '11111111-2222-3333-4444-555555555101',
    parentId: null,
    menuName: '工作台',
    menuCode: 'Desktop',
    path: '/desktop',
    component: 'desktop/index',
    icon: 'House',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // 基础管理
  {
    id: '11111111-2222-3333-4444-555555555102',
    parentId: null,
    menuName: '基础管理',
    menuCode: 'Basic',
    path: '/basic',
    component: null,
    icon: 'Setting',
    sort: 2,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555121',
    parentId: '11111111-2222-3333-4444-555555555102',
    menuName: '用户管理',
    menuCode: 'User',
    path: '/basic/user',
    component: 'basic/user/index',
    icon: 'User',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555122',
    parentId: '11111111-2222-3333-4444-555555555102',
    menuName: '角色管理',
    menuCode: 'Role',
    path: '/basic/role',
    component: 'basic/role/index',
    icon: 'UserFilled',
    sort: 2,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555123',
    parentId: '11111111-2222-3333-4444-555555555102',
    menuName: '菜单管理',
    menuCode: 'Menu',
    path: '/basic/menu',
    component: 'basic/menu/index',
    icon: 'Menu',
    sort: 3,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555124',
    parentId: '11111111-2222-3333-4444-555555555102',
    menuName: '字典管理',
    menuCode: 'Dict',
    path: '/basic/dict',
    component: 'basic/dict/index',
    icon: 'Collection',
    sort: 4,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // 业务管理
  {
    id: '11111111-2222-3333-4444-555555555103',
    parentId: null,
    menuName: '业务管理',
    menuCode: 'Buz',
    path: '/buz',
    component: null,
    icon: 'Shop',
    sort: 3,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // 商品管理（父级）
  {
    id: '11111111-2222-3333-4444-555555555107',
    parentId: '11111111-2222-3333-4444-555555555103',
    menuName: '商品管理',
    menuCode: 'Product',
    path: '/buz/product',
    component: null,
    icon: 'Goods',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555171',
    parentId: '11111111-2222-3333-4444-555555555107',
    menuName: '商品列表',
    menuCode: 'ProductList',
    path: '/buz/product/list',
    component: 'buz/product/ProductList',
    icon: 'List',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555173',
    parentId: '11111111-2222-3333-4444-555555555107',
    menuName: '商品分类',
    menuCode: 'CategoryList',
    path: '/buz/product/category',
    component: 'buz/product/CategoryList',
    icon: 'Grid',
    sort: 2,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555174',
    parentId: '11111111-2222-3333-4444-555555555107',
    menuName: '库存管理',
    menuCode: 'StockList',
    path: '/buz/product/stock',
    component: 'buz/product/StockList',
    icon: 'Box',
    sort: 3,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555175',
    parentId: '11111111-2222-3333-4444-555555555107',
    menuName: '库存流水',
    menuCode: 'StockRecordList',
    path: '/buz/product/stock-record',
    component: 'buz/product/StockRecordList',
    icon: 'Tickets',
    sort: 4,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // 订单管理
  {
    id: '11111111-2222-3333-4444-555555555108',
    parentId: '11111111-2222-3333-4444-555555555103',
    menuName: '订单管理',
    menuCode: 'Order',
    path: '/buz/order',
    component: 'buz/order/OrderList',
    icon: 'ShoppingCart',
    sort: 2,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // 退款管理
  {
    id: '11111111-2222-3333-4444-555555555109',
    parentId: '11111111-2222-3333-4444-555555555103',
    menuName: '退款管理',
    menuCode: 'Refund',
    path: '/buz/refund',
    component: 'buz/refund/RefundList',
    icon: 'RefreshLeft',
    sort: 3,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // 客户管理（父级）
  {
    id: '11111111-2222-3333-4444-555555555110',
    parentId: '11111111-2222-3333-4444-555555555103',
    menuName: '客户管理',
    menuCode: 'Customer',
    path: '/buz/customer',
    component: null,
    icon: 'Avatar',
    sort: 4,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555201',
    parentId: '11111111-2222-3333-4444-555555555110',
    menuName: '客户列表',
    menuCode: 'CustomerList',
    path: '/buz/customer/list',
    component: 'buz/customer/index',
    icon: 'List',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555202',
    parentId: '11111111-2222-3333-4444-555555555110',
    menuName: '会员等级',
    menuCode: 'MemberLevel',
    path: '/buz/customer/level',
    component: 'buz/customer/level/index',
    icon: 'Medal',
    sort: 2,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555203',
    parentId: '11111111-2222-3333-4444-555555555110',
    menuName: '积分记录',
    menuCode: 'PointsLog',
    path: '/buz/customer/points',
    component: 'buz/customer/points/index',
    icon: 'Star',
    sort: 3,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // 供应商管理
  {
    id: '11111111-2222-3333-4444-555555555176',
    parentId: '11111111-2222-3333-4444-555555555103',
    menuName: '供应商管理',
    menuCode: 'SupplierList',
    path: '/buz/supplier',
    component: 'buz/supplier/SupplierList',
    icon: 'OfficeBuilding',
    sort: 5,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // 小程序管理（父级）
  {
    id: '11111111-2222-3333-4444-555555555111',
    parentId: '11111111-2222-3333-4444-555555555103',
    menuName: '小程序管理',
    menuCode: 'MiniApp',
    path: '/miniapp',
    component: null,
    icon: 'Iphone',
    sort: 6,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555211',
    parentId: '11111111-2222-3333-4444-555555555111',
    menuName: '轮播图管理',
    menuCode: 'BannerList',
    path: '/miniapp/banner',
    component: 'buz/banner/BannerList',
    icon: 'Picture',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // 报表管理
  {
    id: '11111111-2222-3333-4444-555555555105',
    parentId: null,
    menuName: '报表管理',
    menuCode: 'Report',
    path: '/report',
    component: null,
    icon: 'DataAnalysis',
    sort: 4,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555151',
    parentId: '11111111-2222-3333-4444-555555555105',
    menuName: '报表列表',
    menuCode: 'ReportList',
    path: '/report/list',
    component: 'report/ReportList',
    icon: 'Document',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555154',
    parentId: '11111111-2222-3333-4444-555555555105',
    menuName: '数据源管理',
    menuCode: 'DataSourceList',
    path: '/report/datasource',
    component: 'report/DataSourceList',
    icon: 'Coin',
    sort: 2,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555155',
    parentId: '11111111-2222-3333-4444-555555555105',
    menuName: '列配置模板',
    menuCode: 'ColumnTemplateList',
    path: '/report/column-template',
    component: 'report/ColumnTemplateList',
    icon: 'Grid',
    sort: 3,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // 大屏管理
  {
    id: '11111111-2222-3333-4444-555555555106',
    parentId: null,
    menuName: '大屏管理',
    menuCode: 'Screen',
    path: '/screen',
    component: null,
    icon: 'DataBoard',
    sort: 5,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555161',
    parentId: '11111111-2222-3333-4444-555555555106',
    menuName: '大屏列表',
    menuCode: 'ScreenList',
    path: '/screen/list',
    component: 'screen/index',
    icon: 'DataBoard',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // ETL 管理
  {
    id: '11111111-2222-3333-4444-555555555112',
    parentId: null,
    menuName: 'ETL管理',
    menuCode: 'Etl',
    path: '/etl',
    component: null,
    icon: 'Connection',
    sort: 6,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555221',
    parentId: '11111111-2222-3333-4444-555555555112',
    menuName: '数据源管理',
    menuCode: 'EtlDatasource',
    path: '/etl/datasource',
    component: 'etl/datasource/index',
    icon: 'Coin',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555222',
    parentId: '11111111-2222-3333-4444-555555555112',
    menuName: '管道管理',
    menuCode: 'EtlPipeline',
    path: '/etl/pipeline',
    component: 'etl/pipeline/index',
    icon: 'Sort',
    sort: 2,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555223',
    parentId: '11111111-2222-3333-4444-555555555112',
    menuName: '调度管理',
    menuCode: 'EtlSchedule',
    path: '/etl/schedule',
    component: 'etl/schedule/index',
    icon: 'Timer',
    sort: 3,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555224',
    parentId: '11111111-2222-3333-4444-555555555112',
    menuName: '监控管理',
    menuCode: 'EtlMonitor',
    path: '/etl/monitor',
    component: 'etl/monitor/index',
    icon: 'View',
    sort: 4,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  // 工作流管理
  {
    id: '11111111-2222-3333-4444-555555555113',
    parentId: null,
    menuName: '工作流管理',
    menuCode: 'AntWorkflow',
    path: '/ant_workflow',
    component: null,
    icon: 'Share',
    sort: 7,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2026-04-13 10:00:00',
    updateTime: '2026-04-13 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555231',
    parentId: '11111111-2222-3333-4444-555555555113',
    menuName: '流程列表',
    menuCode: 'AntWorkflowList',
    path: '/ant_workflow/list',
    component: 'ant_workflow/list/index',
    icon: 'List',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2026-04-13 10:00:00',
    updateTime: '2026-04-13 10:00:00',
  },
  // 运行时管理（父级）
  {
    id: '11111111-2222-3333-4444-555555555233',
    parentId: '11111111-2222-3333-4444-555555555113',
    menuName: '运行时管理',
    menuCode: 'AntWorkflowRuntime',
    path: '/ant_workflow/runtime',
    component: null,
    icon: 'Setting',
    sort: 2,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2026-04-13 10:00:00',
    updateTime: '2026-04-13 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555331',
    parentId: '11111111-2222-3333-4444-555555555233',
    menuName: '待办任务',
    menuCode: 'AntWorkflowTodo',
    path: '/ant_workflow/runtime/todo',
    component: 'ant_workflow/runtime/todo/index',
    icon: 'List',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2026-04-13 10:00:00',
    updateTime: '2026-04-13 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555332',
    parentId: '11111111-2222-3333-4444-555555555233',
    menuName: '已办任务',
    menuCode: 'AntWorkflowDone',
    path: '/ant_workflow/runtime/done',
    component: 'ant_workflow/runtime/done/index',
    icon: 'Finished',
    sort: 2,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2026-04-13 10:00:00',
    updateTime: '2026-04-13 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555333',
    parentId: '11111111-2222-3333-4444-555555555233',
    menuName: '我发起的',
    menuCode: 'AntWorkflowMy',
    path: '/ant_workflow/runtime/my',
    component: 'ant_workflow/runtime/my/index',
    icon: 'User',
    sort: 3,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2026-04-13 10:00:00',
    updateTime: '2026-04-13 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555334',
    parentId: '11111111-2222-3333-4444-555555555233',
    menuName: '抄送我的',
    menuCode: 'AntWorkflowCc',
    path: '/ant_workflow/runtime/cc',
    component: 'ant_workflow/runtime/cc/index',
    icon: 'Message',
    sort: 4,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2026-04-13 10:00:00',
    updateTime: '2026-04-13 10:00:00',
  },
  // 运维监控
  {
    id: '11111111-2222-3333-4444-555555555900',
    parentId: null,
    menuName: '运维监控',
    menuCode: 'Ops',
    path: '/ops',
    component: null,
    icon: 'Monitor',
    sort: 8,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2026-04-13 10:00:00',
    updateTime: '2026-04-13 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555901',
    parentId: '11111111-2222-3333-4444-555555555900',
    menuName: 'ES日志查询',
    menuCode: 'LogQuery',
    path: '/ops/log',
    component: 'ops/LogQuery',
    icon: 'Document',
    sort: 1,
    type: 1,
    status: 1,
    hidden: 0,
    affix: 0,
    createTime: '2026-04-13 10:00:00',
    updateTime: '2026-04-13 10:00:00',
  },
  // 个人中心（隐藏）
  {
    id: '11111111-2222-3333-4444-555555555104',
    parentId: null,
    menuName: '个人中心',
    menuCode: 'Person',
    path: '/person',
    component: 'person/index',
    icon: 'User',
    sort: 99,
    type: 1,
    status: 1,
    hidden: 1,
    affix: 0,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
]

/**
 * 将扁平数组转换为树形结构
 */
function buildMenuTree(menuList: MockMenu[]): MockMenu[] {
  const menuMap = new Map<string, MockMenu>()
  const tree: MockMenu[] = []

  // 先将所有菜单存入 Map
  menuList.forEach((menu) => {
    menuMap.set(menu.id, { ...menu, children: [] })
  })

  // 构建树形结构
  menuList.forEach((menu) => {
    const node = menuMap.get(menu.id)!
    if (menu.parentId === null) {
      // 根节点
      tree.push(node)
    } else {
      // 子节点
      const parent = menuMap.get(menu.parentId)
      if (parent) {
        parent.children = parent.children || []
        parent.children.push(node)
      }
    }
  })

  // 排序（按 sort 升序）
  const sortChildren = (nodes: MockMenu[]) => {
    nodes.sort((a, b) => a.sort - b.sort)
    nodes.forEach((node) => {
      if (node.children && node.children.length > 0) {
        sortChildren(node.children)
      }
    })
  }

  sortChildren(tree)

  // 移除空的 children 数组
  const cleanChildren = (nodes: MockMenu[]) => {
    nodes.forEach((node) => {
      if (node.children && node.children.length === 0) {
        delete node.children
      } else if (node.children) {
        cleanChildren(node.children)
      }
    })
  }

  cleanChildren(tree)

  return tree
}

export default [
  // 获取菜单树（只返回启用且未隐藏的菜单）
  {
    url: '/api/menu/list',
    method: 'get',
    response: () => {
      const filteredMenus = menus.filter((menu) => menu.status === 1)
      const tree = buildMenuTree(filteredMenus)

      return {
        code: 200,
        message: '成功',
        data: tree,
      }
    },
  },

  // 获取当前用户的菜单
  {
    url: '/api/menu/user-menu',
    method: 'get',
    response: () => {
      const filteredMenus = menus.filter((menu) => menu.status === 1)
      const tree = buildMenuTree(filteredMenus)

      return {
        code: 200,
        message: '成功',
        data: tree,
      }
    },
  },

  // 获取菜单详情（路径参数）
  {
    url: /\/api\/menu\/detail\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const menu = menus.find((m) => m.id === id)

      if (!menu) {
        return {
          code: 404,
          message: '菜单不存在',
          data: null,
        }
      }

      return {
        code: 200,
        message: '成功',
        data: menu,
      }
    },
  },

  // 新增菜单
  {
    url: '/api/menu/add',
    method: 'post',
    response: ({ body }: { body: Partial<MockMenu> }) => {
      const { parentId, menuName, menuCode, path, component, icon, sort, type, status, hidden, affix } = body

      if (!menuName) {
        return {
          code: 400,
          message: '菜单名称不能为空',
          data: null,
        }
      }

      const now = formatNow()
      const newMenu: MockMenu = {
        id: generateGuid(),
        parentId: parentId ?? null,
        menuName,
        menuCode: menuCode ?? null,
        path: path ?? null,
        component: component ?? null,
        icon: icon ?? null,
        sort: sort ?? 0,
        type: type ?? 1,
        status: status ?? 1,
        hidden: hidden ?? 0,
        affix: affix ?? 0,
        createTime: now,
        updateTime: now,
      }

      menus.push(newMenu)

      return {
        code: 200,
        message: '创建成功',
        data: newMenu,
      }
    },
  },

  // 编辑菜单（PUT方法）
  {
    url: '/api/menu/update',
    method: 'put',
    response: ({ body }: { body: Partial<MockMenu> & { id: string } }) => {
      const { id, parentId, menuName, menuCode, path, component, icon, sort, type, status, hidden, affix } = body

      const menuIndex = menus.findIndex((m) => m.id === id)

      if (menuIndex === -1) {
        return {
          code: 404,
          message: '菜单不存在',
          data: null,
        }
      }

      const menu = menus[menuIndex]

      // 更新字段
      if (parentId !== undefined) menu.parentId = parentId
      if (menuName !== undefined) menu.menuName = menuName
      if (menuCode !== undefined) menu.menuCode = menuCode
      if (path !== undefined) menu.path = path
      if (component !== undefined) menu.component = component
      if (icon !== undefined) menu.icon = icon
      if (sort !== undefined) menu.sort = sort
      if (type !== undefined) menu.type = type
      if (status !== undefined) menu.status = status
      if (hidden !== undefined) menu.hidden = hidden
      if (affix !== undefined) menu.affix = affix
      menu.updateTime = formatNow()

      menus[menuIndex] = menu

      return {
        code: 200,
        message: '更新成功',
        data: menu,
      }
    },
  },

  // 删除菜单（单个，DELETE方法）
  {
    url: /\/api\/menu\/delete\/[\w-]+/,
    method: 'delete',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()

      // 检查是否有子菜单
      const hasChildren = menus.some((m) => m.parentId === id)
      if (hasChildren) {
        const menu = menus.find((m) => m.id === id)
        return {
          code: 400,
          message: `菜单"${menu?.menuName || id}"存在子菜单，不能删除`,
          data: null,
        }
      }

      const index = menus.findIndex((m) => m.id === id)
      if (index !== -1) {
        menus.splice(index, 1)
      }

      return {
        code: 200,
        message: '删除成功',
        data: 1,
      }
    },
  },

  // 分配角色权限
  {
    url: '/api/menu/assign-roles',
    method: 'post',
    response: ({ body }: { body: { menuId: string; roleIds: string[] } }) => {
      const { menuId, roleIds } = body

      if (!menuId) {
        return {
          code: 400,
          message: '菜单ID不能为空',
          data: null,
        }
      }

      const menu = menus.find((m) => m.id === menuId)
      if (!menu) {
        return {
          code: 404,
          message: '菜单不存在',
          data: null,
        }
      }

      return {
        code: 200,
        message: '分配成功',
        data: roleIds?.length || 0,
      }
    },
  },
] as MockMethod[]
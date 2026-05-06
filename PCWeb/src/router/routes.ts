import type { RouteRecordRaw } from 'vue-router'
import antWorkflowRoutes from './antWorkflowRoutes'
import opsRoutes from './opsRoutes'

// 公共路由（不需要登录）
export const publicRoutes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/login/index.vue'),
    meta: { title: '登录', hidden: true }, // hidden: true 不显示在标签页中
  },
  {
    path: '/404',
    name: 'NotFound',
    component: () => import('@/views/error/404.vue'),
    meta: { title: '404', hidden: true },
  },
]

// 公告管理静态路由（添加到基础布局路由的 children 中）
export const announcementRoutes: RouteRecordRaw[] = [
  {
    path: 'basic/announcement',
    name: 'Announcement',
    redirect: '/basic/announcement/list',
    meta: { title: '公告管理', icon: 'Bell' },
    children: [
      {
        path: 'list',
        name: 'AnnouncementList',
        component: () => import('@/views/basic/announcement/index.vue'),
        meta: { title: '公告列表' }
      },
      {
        path: 'edit/:id?',
        name: 'AnnouncementEdit',
        component: () => import('@/views/basic/announcement/AnnouncementEdit.vue'),
        meta: { title: '编辑公告', hidden: true }
      },
      {
        path: 'preview/:id',
        name: 'AnnouncementPreview',
        component: () => import('@/views/basic/announcement/AnnouncementPreview.vue'),
        meta: { title: '查看公告', hidden: true }
      }
    ]
  }
]

// 独立页面路由（不需要主布局，全屏展示）
export const standaloneRoutes: RouteRecordRaw[] = [
  // 大屏预览页面（独立全屏）
  {
    path: '/screen/view/:id',
    name: 'ScreenView',
    component: () => import('@/views/screen/preview/index.vue'),
    meta: { title: '大屏预览', hidden: true },
  },
  // 大屏设计器（独立全屏）
  {
    path: '/screen/design/:id?',
    name: 'ScreenDesign',
    component: () => import('@/views/screen/design/index.vue'),
    meta: { title: '大屏设计器', hidden: true },
  },
  // 发布后的大屏查看页面
  {
    path: '/screen/publish/:publishId',
    name: 'ScreenPublish',
    component: () => import('@/views/screen/publish/index.vue'),
    meta: { title: '大屏查看', hidden: true },
  },
  // 发布后的报表查看页面（公开访问，不需要登录）
  {
    path: '/report/publish/:id',
    name: 'ReportPublish',
    component: () => import('@/views/report/publish/index.vue'),
    meta: { title: '报表查看', hidden: true },
  },
  // ETL DAG 设计器（独立全屏）
  {
    path: '/etl/design',
    name: 'EtlDesign',
    component: () => import('@/views/etl/design/index.vue'),
    meta: { title: 'DAG设计器', hidden: true },
  },
  // Ant Workflow 设计器（蚂蚁风格，独立全屏）
  // 注意：使用 /workflow-ant-design 路径，避免与 /ant_workflow 动态路由前缀冲突
  {
    path: '/workflow-ant-design/:id?',
    name: 'AntWorkflowDesign',
    component: () => import('@/views/ant_workflow/design/index.vue'),
    meta: { title: '流程设计器', hidden: true },
  },
]

// 基础布局路由（动态路由的容器）
export const baseRoute: RouteRecordRaw = {
  path: '/',
  name: 'Layout',
  component: () => import('@/views/layout/index.vue'),
  redirect: '/desktop',
  children: [
    // 工作台页面（静态路由，避免依赖动态路由）
    {
      path: '/desktop',
      name: 'Desktop',
      component: () => import('@/views/desktop/index.vue'),
      meta: { title: '工作台', icon: 'House', affix: true },
    },
    // 列配置模板编辑页面（静态路由，避免动态路由参数匹配问题）
    // {
    //   path: '/report/column-template/edit/:id',
    //   name: 'ColumnTemplateEdit',
    //   component: () => import('@/views/report/ColumnTemplateEdit.vue'),
    //   meta: { title: '列配置模板编辑', hidden: true },
    // },
    // {
    //   path: '/report/column-template/create',
    //   name: 'ColumnTemplateCreate',
    //   component: () => import('@/views/report/ColumnTemplateEdit.vue'),
    //   meta: { title: '列配置模板创建', hidden: true },
    // },
    // // 商品编辑页面（静态路由，避免动态路由参数匹配问题）
    // {
    //   path: '/product/edit/:id',
    //   name: 'ProductEditById',
    //   component: () => import('@/views/buz/product/ProductEdit.vue'),
    //   meta: { title: '编辑商品', hidden: true },
    // },
    // {
    //   path: '/product/edit',
    //   name: 'ProductEdit',
    //   component: () => import('@/views/buz/product/ProductEdit.vue'),
    //   meta: { title: '新增商品', hidden: true },
    // },
    // // 商品评价管理
    // {
    //   path: '/product/review',
    //   name: 'ProductReview',
    //   component: () => import('@/views/buz/product/ReviewList.vue'),
    //   meta: { title: '商品评价', hidden: true },
    // },
    // // 商品统计报表
    // {
    //   path: '/product/stats',
    //   name: 'ProductStats',
    //   component: () => import('@/views/buz/product/StatsReport.vue'),
    //   meta: { title: '统计报表' },
    // },
    // // 库存变动记录
    // {
    //   path: '/buz/product/stock-record',
    //   name: 'StockRecord',
    //   component: () => import('@/views/buz/product/StockRecordList.vue'),
    //   meta: { title: '库存变动记录', hidden: true },
    // },
    // // 订单创建页面
    // {
    //   path: '/order/create',
    //   name: 'OrderCreate',
    //   component: () => import('@/views/buz/order/OrderEdit.vue'),
    //   meta: { title: '新建订单', hidden: true },
    // },
    // // 订单编辑页面
    // {
    //   path: '/order/edit/:id',
    //   name: 'OrderEdit',
    //   component: () => import('@/views/buz/order/OrderEdit.vue'),
    //   meta: { title: '编辑订单', hidden: true },
    // },
    // // 小程序管理路由
    // {
    //   path: '/miniapp/banner',
    //   name: 'BannerList',
    //   component: () => import('@/views/buz/banner/BannerList.vue'),
    //   meta: { title: '轮播图管理', icon: 'Picture' },
    // },

    // 任务编辑页面（静态路由，避免动态路由参数匹配问题）
    {
      path: '/ops/task/edit/:id?',
      name: 'TaskEdit',
      component: () => import('@/views/ops/TaskEdit.vue'),
      meta: { title: '任务设置', hidden: true },
    },
    // 公告管理路由（静态路由）
    ...announcementRoutes,
    // 部门管理路由（静态路由）
    {
      path: 'basic/department',
      name: 'Department',
      component: () => import('@/views/basic/department/index.vue'),
      meta: { title: '部门管理', icon: 'OfficeBuilding' },
    },
  ], // 动态路由将添加到这里
}

// 常量路由（用于初始化固定标签）
export const constantRoutes: RouteRecordRaw[] = [...publicRoutes, ...standaloneRoutes, baseRoute]

// 初始路由
export const routes: RouteRecordRaw[] = [...publicRoutes, ...standaloneRoutes, baseRoute]

// 404 兜底路由（动态路由加载后添加）
export const notFoundRoute: RouteRecordRaw = {
  path: '/:pathMatch(.*)*',
  redirect: '/404',
}

// 导出 ant_workflow 路由
export { antWorkflowRoutes }

// 导出 ops 路由
export { opsRoutes }
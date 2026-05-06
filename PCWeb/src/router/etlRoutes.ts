import type { RouteRecordRaw } from 'vue-router'

const etlRoutes: RouteRecordRaw[] = [
  {
    path: '/etl',
    name: 'Etl',
    redirect: '/etl/datasource',
    meta: { title: 'ETL管理', icon: 'DataAnalysis' },
    children: [
      {
        path: 'datasource',
        name: 'EtlDatasource',
        component: () => import('@/views/etl/datasource/index.vue'),
        meta: { title: '数据源管理' },
      },
      {
        path: 'pipeline',
        name: 'EtlPipeline',
        component: () => import('@/views/etl/pipeline/index.vue'),
        meta: { title: '任务流管理' },
      },
      {
        path: 'schedule',
        name: 'EtlSchedule',
        component: () => import('@/views/etl/schedule/index.vue'),
        meta: { title: '调度管理' },
      },
      {
        path: 'monitor',
        name: 'EtlMonitor',
        component: () => import('@/views/etl/monitor/index.vue'),
        meta: { title: '执行监控' },
      },
      // 注意：design 路由已移至 standaloneRoutes（独立全屏页面），此处不再重复配置
    ],
  },
]

export default etlRoutes
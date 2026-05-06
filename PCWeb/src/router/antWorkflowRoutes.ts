import type { RouteRecordRaw } from 'vue-router'

/**
 * Ant Design 风格工作流路由配置
 */
const antWorkflowRoutes: RouteRecordRaw[] = [
  {
    path: '/ant_workflow',
    name: 'AntWorkflow',
    redirect: '/ant_workflow/list',
    meta: {
      title: '工作流管理',
      icon: 'Share',
    },
    children: [
      {
        path: 'list',
        name: 'AntWorkflowList',
        component: () => import('@/views/ant_workflow/list/index.vue'),
        meta: {
          title: '流程列表',
          icon: 'List',
        },
      },
      // 设计页面已移至 standaloneRoutes（独立全屏）
      {
        path: 'runtime',
        name: 'AntWorkflowRuntime',
        redirect: '/ant_workflow/runtime/todo',
        meta: {
          title: '运行时管理',
          icon: 'Setting',
        },
        children: [
          {
            path: 'todo',
            name: 'AntWorkflowTodo',
            component: () => import('@/views/ant_workflow/runtime/todo/index.vue'),
            meta: {
              title: '待办任务',
              icon: 'List',
            },
          },
          {
            path: 'done',
            name: 'AntWorkflowDone',
            component: () => import('@/views/ant_workflow/runtime/done/index.vue'),
            meta: {
              title: '已办任务',
              icon: 'Finished',
            },
          },
          {
            path: 'my',
            name: 'AntWorkflowMy',
            component: () => import('@/views/ant_workflow/runtime/my/index.vue'),
            meta: {
              title: '我发起的',
              icon: 'User',
            },
          },
          {
            path: 'cc',
            name: 'AntWorkflowCc',
            component: () => import('@/views/ant_workflow/runtime/cc/index.vue'),
            meta: {
              title: '抄送我的',
              icon: 'Message',
            },
          },
        ],
      },
      // 业务审核点管理
      {
        path: 'audit-point',
        name: 'BusinessAuditPoint',
        component: () => import('@/views/workflow/businessAuditPoint/index.vue'),
        meta: {
          title: '审核点管理',
          icon: 'Stamp',
        },
      },
    ],
  },
]

export default antWorkflowRoutes
// src/router/opsRoutes.ts

import type { RouteRecordRaw } from 'vue-router'

/**
 * 运维监控路由
 */
const opsRoutes: RouteRecordRaw[] = [
  {
    path: '/ops',
    name: 'Ops',
    redirect: '/ops/log',
    meta: {
      title: '运维监控',
      icon: 'Monitor',
    },
    children: [
      {
        path: 'log',
        name: 'LogQuery',
        component: () => import('@/views/ops/LogQuery.vue'),
        meta: {
          title: 'ES 日志查询',
          icon: 'Document',
        },
      },
      {
        path: 'operate-log',
        name: 'OperateLog',
        component: () => import('@/views/ops/OperateLog.vue'),
        meta: {
          title: '操作日志查询',
          icon: 'List',
        },
      },
      {
        path: 'task',
        name: 'TaskList',
        component: () => import('@/views/ops/TaskList.vue'),
        meta: {
          title: '定时任务管理',
          icon: 'Timer',
        },
      },
      {
        path: 'task/edit/:id?',
        name: 'TaskEdit',
        component: () => import('@/views/ops/TaskEdit.vue'),
        meta: {
          title: '任务设置',
          icon: 'Edit',
          hidden: true,
        },
      },
      {
        path: 'task-log',
        name: 'TaskLog',
        component: () => import('@/views/ops/TaskLog.vue'),
        meta: {
          title: '任务执行日志',
          icon: 'Clock',
        },
      },
    ],
  },
]

export default opsRoutes
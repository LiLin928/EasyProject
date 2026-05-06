// Mock 配置入口

import auth from './auth'
import user from './user'
import menu from './menu'
import role from './role'
import dict from './dict'
import report from './report'
import datasource from './datasource'
import screen from './screen'
import columnTemplate from './columnTemplate'
import product from './product'
import order from './order'
import upload from './upload'
import review from './review'
import stats from './stats'
import memberLevel from './memberLevel'
import customer from './customer'
import points from './points'
import banner from './banner'
import etl from './etl'
import antWorkflow from './ant_workflow'
import logQuery from './logQuery'
import task from './task'

import mockConfig from '@/config/mock.config'
import type { MockConfig } from '@/config/mock.config'

// 为每个模块添加 module 标识
const allMocks = [
  ...auth.map(m => ({ ...m, module: 'auth' })),
  ...user.map(m => ({ ...m, module: 'basic' })),
  ...role.map(m => ({ ...m, module: 'basic' })),
  ...menu.map(m => ({ ...m, module: 'basic' })),
  ...dict.map(m => ({ ...m, module: 'basic' })),
  ...product.map(m => ({ ...m, module: 'product' })),
  ...review.map(m => ({ ...m, module: 'product' })),
  ...stats.map(m => ({ ...m, module: 'product' })),
  ...order.map(m => ({ ...m, module: 'order' })),
  ...customer.map(m => ({ ...m, module: 'customer' })),
  ...memberLevel.map(m => ({ ...m, module: 'customer' })),
  ...points.map(m => ({ ...m, module: 'customer' })),
  ...banner.map(m => ({ ...m, module: 'banner' })),
  ...report.map(m => ({ ...m, module: 'report' })),
  ...datasource.map(m => ({ ...m, module: 'report' })),
  ...columnTemplate.map(m => ({ ...m, module: 'report' })),
  ...screen.map(m => ({ ...m, module: 'screen' })),
  ...etl.map(m => ({ ...m, module: 'etl' })),
  ...antWorkflow.map(m => ({ ...m, module: 'antWorkflow' })),
  ...upload.map(m => ({ ...m, module: 'upload' })),
  ...logQuery.map(m => ({ ...m, module: 'logQuery' })),
  ...task.map(m => ({ ...m, module: 'task' })),
]

// 根据配置过滤 Mock
export default allMocks.filter(mock => {
  const moduleName = mock.module as keyof MockConfig
  return mockConfig[moduleName] === true
})
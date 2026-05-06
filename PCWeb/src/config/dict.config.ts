/**
 * 字典模块配置
 * 控制每个模块是否使用后端字典
 */

export interface DictModuleSetting {
  /** 是否使用后端字典 */
  IsUseDic: boolean
  /** 枚举名称 -> 字典编码映射 */
  enumMappings: Record<string, string>
}

export const dictModuleConfig: Record<string, DictModuleSetting> = {
  // 基础管理模块（后端字典 API 未实现，暂时禁用）
  common: {
    IsUseDic: false,
    enumMappings: {
      CommonStatus: 'common_status',
      DatasourceStatus: 'datasource_status',
      MenuVisibility: 'menu_visibility',
      ScreenPublishStatus: 'screen_publish_status'
    }
  },

  
  // ETL 模块
  etl: {
    IsUseDic: false,
    enumMappings: {
      PipelineStatus: 'pipeline_status',
      ExecutionStatus: 'execution_status',
      TriggerType: 'trigger_type',
      ScheduleStatus: 'schedule_status',
      ScheduleType: 'schedule_type',
      TaskNodeType: 'task_node_type'
    }
  },

  // 订单模块
  order: {
    IsUseDic: false,
    enumMappings: {
      OrderStatus: 'order_status',
      PayStatus: 'pay_status',
      ShipStatus: 'ship_status'
    }
  },

  // 退款模块
  refund: {
    IsUseDic: false,
    enumMappings: {
      RefundStatus: 'refund_status',
      RefundType: 'refund_type'
    }
  },

  // 商品模块
  product: {
    IsUseDic: false,
    enumMappings: {
      ProductStatus: 'product_status',
      ReviewStatus: 'review_status'
    }
  },

  // 客户模块
  customer: {
    IsUseDic: false,
    enumMappings: {
      CustomerStatus: 'customer_status',
      MemberLevelType: 'member_level_type'
    }
  },

  // 轮播图模块
  banner: {
    IsUseDic: false,
    enumMappings: {
      BannerStatus: 'banner_status',
      JumpType: 'banner_jump_type'
    }
  }
}

/**
 * 获取模块配置
 */
export function getModuleConfig(module: string): DictModuleSetting | undefined {
  return dictModuleConfig[module]
}

/**
 * 获取字典编码
 */
export function getDictCode(module: string, enumName: string): string {
  const config = dictModuleConfig[module]
  return config?.enumMappings?.[enumName] || enumName.toLowerCase()
}

/**
 * 获取所有启用字典的模块编码列表
 */
export function getEnabledDictCodes(): string[] {
  const codes: string[] = []
  Object.values(dictModuleConfig).forEach(config => {
    if (config.IsUseDic) {
      codes.push(...Object.values(config.enumMappings))
    }
  })
  return [...new Set(codes)]
}
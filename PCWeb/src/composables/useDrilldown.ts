// src/composables/useDrilldown.ts

import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import type { DrilldownPath, DrilldownRule, ColumnConfig } from '@/types'

const DRILLDOWN_PATH_KEY = 'drilldown_path'

/**
 * 钻取逻辑组合式函数
 * 管理报表钻取路径、参数传递和导航
 */
export function useDrilldown() {
  const router = useRouter()
  const route = useRoute()

  // 钻取路径
  const drilldownPath = ref<DrilldownPath[]>([])

  /**
   * 初始化钻取路径
   * @param reportId 报表ID
   * @param reportName 报表名称
   */
  const initDrilldownPath = (reportId: number, reportName: string) => {
    const stored = sessionStorage.getItem(DRILLDOWN_PATH_KEY)
    if (stored) {
      try {
        const parsed = JSON.parse(stored)
        // 检查是否是当前报表的路径
        if (parsed.length > 0 && parsed[parsed.length - 1].reportId === reportId) {
          drilldownPath.value = parsed
          return
        }
      } catch {
        // 解析失败，重新初始化
      }
    }
    // 重置路径为当前报表
    drilldownPath.value = [{ reportId, reportName, params: {} }]
    savePath()
  }

  /**
   * 保存路径到 sessionStorage
   */
  const savePath = () => {
    sessionStorage.setItem(DRILLDOWN_PATH_KEY, JSON.stringify(drilldownPath.value))
  }

  /**
   * 执行钻取
   * @param rule 钻取规则
   * @param rowData 行数据
   * @param clickedValue 点击的值
   * @param targetReportName 目标报表名称
   */
  const executeDrilldown = (
    rule: DrilldownRule,
    rowData: Record<string, any>,
    clickedValue: any,
    targetReportName: string
  ) => {
    if (!rule.enabled || !rule.targetReportId) return

    // 构建参数
    const params: Record<string, any> = {}
    rule.params.forEach((param) => {
      if (param.sourceField === '__clicked_value__') {
        params[param.targetParam] = clickedValue
      } else {
        params[param.targetParam] = rowData[param.sourceField]
      }
    })

    // 添加到钻取路径
    drilldownPath.value.push({
      reportId: rule.targetReportId,
      reportName: targetReportName,
      params,
    })
    savePath()

    // 跳转到目标报表
    const query = Object.entries(params)
      .map(([key, value]) => `${key}=${encodeURIComponent(String(value))}`)
      .join('&')

    router.push(`/report/detail/${rule.targetReportId}${query ? '?' + query : ''}`)
  }

  /**
   * 返回上级
   * @param index 指定层级索引（可选）
   */
  const goBack = (index?: number) => {
    if (index !== undefined) {
      // 跳转到指定层级
      drilldownPath.value = drilldownPath.value.slice(0, index + 1)
    } else {
      // 返回上一级
      if (drilldownPath.value.length > 1) {
        drilldownPath.value.pop()
      }
    }
    savePath()

    const current = drilldownPath.value[drilldownPath.value.length - 1]
    router.push(`/report/detail/${current.reportId}`)
  }

  /**
   * 清除钻取路径
   */
  const clearPath = () => {
    drilldownPath.value = []
    sessionStorage.removeItem(DRILLDOWN_PATH_KEY)
  }

  /**
   * 获取当前钻取参数
   */
  const currentParams = computed(() => {
    const current = drilldownPath.value[drilldownPath.value.length - 1]
    return current?.params || {}
  })

  /**
   * 检查列是否可钻取
   * @param config 列配置
   */
  const isDrilldownable = (config: ColumnConfig): boolean => {
    return !!(config.drilldown?.enabled && config.drilldown?.targetReportId)
  }

  return {
    drilldownPath,
    initDrilldownPath,
    savePath,
    executeDrilldown,
    goBack,
    clearPath,
    currentParams,
    isDrilldownable,
  }
}
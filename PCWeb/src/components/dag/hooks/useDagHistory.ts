/**
 * DAG 历史记录 Hook
 * 提供撤销/重做功能
 */
import { ref, computed, provide } from 'vue'
import type { Graph } from '@antv/x6'

export interface UseDagHistoryOptions {
  /** 获取图实例的方法 */
  getGraph: () => Graph | null
  /** 从图导出配置的方法 */
  exportConfig: (graph: Graph) => any
  /** 加载配置到图的方法 */
  loadConfig: (graph: Graph, config: any) => void
  /** 配置变化时的回调 */
  onConfigChange?: (config: any) => void
}

export function useDagHistory(options: UseDagHistoryOptions) {
  const { getGraph, exportConfig, loadConfig, onConfigChange } = options

  const history = ref<string[]>([])
  const historyIndex = ref(-1)

  const canUndo = computed(() => historyIndex.value > 0)
  const canRedo = computed(() => historyIndex.value < history.value.length - 1)

  /**
   * 推送当前状态到历史记录
   */
  const pushHistory = () => {
    const graph = getGraph()
    if (!graph) return

    const config = exportConfig(graph)
    const jsonStr = JSON.stringify(config)

    // 如果当前不在末尾，删除后面的历史
    if (historyIndex.value < history.value.length - 1) {
      history.value = history.value.slice(0, historyIndex.value + 1)
    }

    // 添加新历史记录
    history.value.push(jsonStr)
    historyIndex.value = history.value.length - 1
  }

  /**
   * 撤销操作
   */
  const undo = () => {
    if (!canUndo.value) return
    historyIndex.value--
    restoreFromHistory()
  }

  /**
   * 重做操作
   */
  const redo = () => {
    if (!canRedo.value) return
    historyIndex.value++
    restoreFromHistory()
  }

  /**
   * 从历史记录恢复状态
   */
  const restoreFromHistory = () => {
    const config = JSON.parse(history.value[historyIndex.value])
    const graph = getGraph()
    if (graph) {
      loadConfig(graph, config)
    }
    onConfigChange?.(config)
  }

  /**
   * 重置历史记录
   */
  const resetHistory = () => {
    history.value = []
    historyIndex.value = -1
  }

  /**
   * 初始化历史记录（加载初始状态）
   */
  const initHistory = () => {
    pushHistory()
  }

  // 提供给子组件
  provide('dagHistory', {
    canUndo,
    canRedo,
    pushHistory,
  })

  return {
    history,
    historyIndex,
    canUndo,
    canRedo,
    pushHistory,
    undo,
    redo,
    resetHistory,
    initHistory,
  }
}
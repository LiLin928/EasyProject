/**
 * Ant Workflow Store
 * DAG 工作流设计器状态管理
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { AntFlowNode, AntNodeType } from '@/types/antWorkflow'

// ==================== DAG 配置类型 ====================

/**
 * DAG 节点（用于设计器）
 */
export interface DagNode {
  /** 节点ID（GUID） */
  id: string
  /** 节点名称 */
  name: string
  /** 节点类型 */
  type: AntNodeType
  /** 位置坐标 */
  position: { x: number; y: number }
  /** 节点配置 */
  config: AntFlowNode
}

/**
 * DAG 边（连线）
 */
export interface DagEdge {
  /** 边ID（GUID） */
  id: string
  /** 源节点ID */
  sourceNodeId: string
  /** 目标节点ID */
  targetNodeId: string
  /** 源端口 */
  sourcePort?: string
  /** 目标端口 */
  targetPort?: string
  /** 边条件 */
  condition?: {
    branchId: string
    branchName: string
    priority: number
    isDefault?: boolean
  }
}

/**
 * DAG 全局配置
 */
export interface DagGlobalConfig {
  /** 最大并发数 */
  maxConcurrency?: number
  /** 超时时间（秒） */
  timeout?: number
  /** 重试次数 */
  retryTimes?: number
  /** 重试间隔（秒） */
  retryInterval?: number
  /** 失败策略 */
  failureStrategy?: 'stop' | 'continue'
}

/**
 * DAG 配置
 */
export interface DagConfig {
  /** 配置版本号 */
  version: string
  /** 节点列表 */
  nodes: DagNode[]
  /** 连线列表 */
  edges: DagEdge[]
  /** 全局配置 */
  globalConfig?: DagGlobalConfig
}

export const useAntWorkflowStore = defineStore('antWorkflow', () => {
  // ==================== 当前流程状态 ====================

  /** 当前编辑的流程ID */
  const currentWorkflowId = ref<string | null>(null)

  /** 流程名称 */
  const workflowName = ref<string>('')

  /** 流程状态 */
  const workflowStatus = ref<number>(0)

  /** DAG 配置 */
  const dagConfig = ref<DagConfig | null>(null)

  // ==================== 节点选中状态 ====================

  /** 选中的节点ID */
  const selectedNodeId = ref<string | null>(null)

  /** 选中的节点 */
  const selectedNode = ref<DagNode | null>(null)

  // ==================== 历史记录（撤销/重做） ====================

  /** 历史记录列表 */
  const history = ref<string[]>([])

  /** 当前历史索引 */
  const historyIndex = ref<number>(-1)

  // ==================== 校验状态 ====================

  /** 是否已触发校验 */
  const isTried = ref<boolean>(false)

  // ==================== 弹窗状态 ====================

  /** 节点配置抽屉是否打开 */
  const nodeConfigDrawer = ref<boolean>(false)

  /** 当前编辑的节点 */
  const currentEditNode = ref<DagNode | null>(null)

  // ==================== 计算属性 ====================

  /** 是否可以撤销 */
  const canUndo = computed(() => historyIndex.value > 0)

  /** 是否可以重做 */
  const canRedo = computed(() => historyIndex.value < history.value.length - 1)

  // ==================== Actions ====================

  /**
   * 设置当前流程
   */
  function setWorkflow(workflow: {
    id: string
    name: string
    status: number
    dagConfig: DagConfig
  }) {
    currentWorkflowId.value = workflow.id
    workflowName.value = workflow.name
    workflowStatus.value = workflow.status
    dagConfig.value = workflow.dagConfig
    initHistory(workflow.dagConfig)
  }

  /**
   * 更新 DAG 配置
   */
  function updateDagConfig(config: DagConfig) {
    dagConfig.value = config
    pushHistory(config)
  }

  /**
   * 选中节点
   */
  function setSelectedNode(nodeId: string | null, node: DagNode | null) {
    selectedNodeId.value = nodeId
    selectedNode.value = node
  }

  /**
   * 清除选中
   */
  function clearSelection() {
    selectedNodeId.value = null
    selectedNode.value = null
  }

  /**
   * 添加历史记录
   */
  function pushHistory(config: DagConfig) {
    const json = JSON.stringify(config)
    // 如果当前不在历史末尾，截断后续历史
    if (historyIndex.value < history.value.length - 1) {
      history.value = history.value.slice(0, historyIndex.value + 1)
    }
    history.value.push(json)
    historyIndex.value = history.value.length - 1
  }

  /**
   * 撤销操作
   */
  function undo(): DagConfig | null {
    if (!canUndo.value) return null
    historyIndex.value--
    const config = JSON.parse(history.value[historyIndex.value])
    dagConfig.value = config
    return config
  }

  /**
   * 重做操作
   */
  function redo(): DagConfig | null {
    if (!canRedo.value) return null
    historyIndex.value++
    const config = JSON.parse(history.value[historyIndex.value])
    dagConfig.value = config
    return config
  }

  /**
   * 初始化历史记录
   */
  function initHistory(config: DagConfig) {
    history.value = [JSON.stringify(config)]
    historyIndex.value = 0
  }

  /**
   * 打开节点配置抽屉
   */
  function openNodeConfigDrawer(node: DagNode) {
    currentEditNode.value = node
    nodeConfigDrawer.value = true
  }

  /**
   * 关闭节点配置抽屉
   */
  function closeNodeConfigDrawer() {
    currentEditNode.value = null
    nodeConfigDrawer.value = false
  }

  /**
   * 设置是否已触发校验
   */
  function setIsTried(value: boolean) {
    isTried.value = value
  }

  /**
   * 重置状态
   */
  function reset() {
    currentWorkflowId.value = null
    workflowName.value = ''
    workflowStatus.value = 0
    dagConfig.value = null
    selectedNodeId.value = null
    selectedNode.value = null
    history.value = []
    historyIndex.value = -1
    isTried.value = false
    nodeConfigDrawer.value = false
    currentEditNode.value = null
  }

  return {
    // State
    currentWorkflowId,
    workflowName,
    workflowStatus,
    dagConfig,
    selectedNodeId,
    selectedNode,
    history,
    historyIndex,
    isTried,
    nodeConfigDrawer,
    currentEditNode,

    // Computed
    canUndo,
    canRedo,

    // Actions
    setWorkflow,
    updateDagConfig,
    setSelectedNode,
    clearSelection,
    pushHistory,
    undo,
    redo,
    initHistory,
    openNodeConfigDrawer,
    closeNodeConfigDrawer,
    setIsTried,
    reset,
  }
})

// 导出 DAG 配置类型供外部使用
export type { DagNode, DagEdge, DagGlobalConfig, DagConfig }
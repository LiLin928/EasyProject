<!-- src/components/etl/DagDesigner/index.vue -->
<template>
  <div class="dag-designer">
    <!-- 工具栏 -->
    <EtlDagToolbar
      :can-undo="canUndo"
      :can-redo="canRedo"
      :pipeline-name="pipelineName"
      :pipeline-status="pipelineStatus"
      :readonly="readonly"
      @back="handleBack"
      @undo="handleUndo"
      @redo="handleRedo"
      @auto-layout="handleAutoLayout"
      @save="handleSave"
      @publish="handlePublish"
      @run="handleRun"
    />

    <!-- 三栏布局 -->
    <div class="designer-body">
      <!-- 左侧节点库 -->
      <TaskPalette
        v-if="!readonly"
        :collapsed="paletteCollapsed"
        @toggle="paletteCollapsed = !paletteCollapsed"
        @drag-node="handleDragNode"
      />

      <!-- 中间画布 -->
      <EtlDagCanvas
        ref="canvasRef"
        :readonly="readonly"
        @node-click="handleNodeClick"
        @node-added="handleNodeAdded"
        @selection-change="handleSelectionChange"
        @edge-click="handleEdgeClick"
        @edge-selected="handleEdgeSelected"
      />

      <!-- 右侧属性面板（仅在选中节点或边时显示） -->
      <EtlDagPropertyPanel
        v-if="selectedNode"
        :selected-node="selectedNode"
        :graph="canvasRef?.getGraph()"
        @update="handlePropertyUpdate"
        @close="selectedNode = null"
      />
      <EdgeConfigPanel
        v-else-if="selectedEdge"
        :selected-edge="selectedEdge"
        :source-node="sourceNode"
        :target-node="targetNode"
        @update="handleEdgeUpdate"
        @close="selectedEdge = null"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, provide, watch, nextTick } from 'vue'
import { ElMessage } from 'element-plus'
import type { Graph } from '@antv/x6'
import { TaskNodeType, PipelineStatus } from '@/types/etl'
import type { DagNode, DagEdge } from '@/types/etl'
import { autoLayout } from './utils/layoutUtils'
import { validateDag, exportDagFromGraph, importDagToGraph } from './utils/validationUtils'
import { useEtlStore } from '@/stores/etlStore'
import EtlDagToolbar from './EtlDagToolbar.vue'
import TaskPalette from './TaskPalette.vue'
import EtlDagCanvas from './EtlDagCanvas.vue'
import EtlDagPropertyPanel from './EtlDagPropertyPanel.vue'
import EdgeConfigPanel from './EdgeConfigPanel.vue'

// 预加载数据源列表
const etlStore = useEtlStore()

// Props
const props = withDefaults(defineProps<{
  pipelineId?: string
  pipelineName?: string
  pipelineStatus?: PipelineStatus
  initialDagConfig?: any
  readonly?: boolean
}>(), {
  pipelineName: '未命名任务流',
  pipelineStatus: PipelineStatus.DRAFT,
  readonly: false,
})

// Emits
const emit = defineEmits<{
  (e: 'back'): void
  (e: 'save', dagConfig: any): void
  (e: 'publish', dagConfig: any): void
  (e: 'run', dagConfig: any): void
  (e: 'node-select', node: DagNode): void
  (e: 'update:config', dagConfig: any): void
}>()

// Refs
const canvasRef = ref<{ getGraph: () => Graph; getNodes: () => DagNode[]; getEdges: () => DagEdge[]; updateEdge: (id: string, data: Partial<DagEdge>) => void }>()
const selectedNode = ref<DagNode | null>(null)
const selectedEdge = ref<DagEdge | null>(null)
const paletteCollapsed = ref(false)
const history = ref<string[]>([])
const historyIndex = ref(-1)

// 计算属性
const canUndo = computed(() => historyIndex.value > 0)
const canRedo = computed(() => historyIndex.value < history.value.length - 1)

// 边的源节点和目标节点
const sourceNode = computed(() => {
  if (!selectedEdge.value || !canvasRef.value) return null
  const nodes = canvasRef.value.getNodes()
  return nodes.find(n => n.id === selectedEdge.value!.sourceNodeId) || null
})

const targetNode = computed(() => {
  if (!selectedEdge.value || !canvasRef.value) return null
  const nodes = canvasRef.value.getNodes()
  return nodes.find(n => n.id === selectedEdge.value!.targetNodeId) || null
})

// ========== 历史记录 ==========

const pushHistory = () => {
  const graph = canvasRef.value?.getGraph()
  if (!graph) return

  const dagConfig = exportDagFromGraph(graph)
  const jsonStr = JSON.stringify(dagConfig)

  // 如果当前不在末尾，删除后面的历史
  if (historyIndex.value < history.value.length - 1) {
    history.value = history.value.slice(0, historyIndex.value + 1)
  }

  // 添加新历史记录
  history.value.push(jsonStr)
  historyIndex.value = history.value.length - 1
}

const handleUndo = () => {
  if (!canUndo.value) return

  historyIndex.value--
  const dagConfig = JSON.parse(history.value[historyIndex.value])
  const graph = canvasRef.value?.getGraph()
  if (graph) {
    graph.clearCells()
    dagConfig.nodes.forEach((node: DagNode) => {
      graph.addNode({
        id: node.id,
        shape: `etl-${node.type}`,
        x: node.position.x,
        y: node.position.y,
        data: { nodeType: node.type, name: node.name, config: node.config },
      })
    })
    dagConfig.edges.forEach((edge: any) => {
      graph.addEdge({
        id: edge.id,
        source: edge.sourceNodeId,
        target: edge.targetNodeId,
      })
    })
  }

  emit('update:config', dagConfig)
}

const handleRedo = () => {
  if (!canRedo.value) return

  historyIndex.value++
  const dagConfig = JSON.parse(history.value[historyIndex.value])
  const graph = canvasRef.value?.getGraph()
  if (graph) {
    graph.clearCells()
    dagConfig.nodes.forEach((node: DagNode) => {
      graph.addNode({
        id: node.id,
        shape: `etl-${node.type}`,
        x: node.position.x,
        y: node.position.y,
        data: { nodeType: node.type, name: node.name, config: node.config },
      })
    })
    dagConfig.edges.forEach((edge: any) => {
      graph.addEdge({
        id: edge.id,
        source: edge.sourceNodeId,
        target: edge.targetNodeId,
      })
    })
  }

  emit('update:config', dagConfig)
}

// ========== 工具栏操作 ==========

const handleBack = () => {
  emit('back')
}

const handleAutoLayout = () => {
  const graph = canvasRef.value?.getGraph()
  if (graph) {
    autoLayout(graph)
    pushHistory()
    ElMessage.success('自动布局完成')
  }
}

const handleSave = () => {
  const graph = canvasRef.value?.getGraph()
  if (!graph) return

  const dagConfig = exportDagFromGraph(graph)
  emit('save', dagConfig)
  ElMessage.success('保存成功')
}

const handlePublish = () => {
  const graph = canvasRef.value?.getGraph()
  if (!graph) return

  const dagConfig = exportDagFromGraph(graph)
  const errors = validateDag(dagConfig)

  if (errors.length > 0) {
    ElMessage.error(errors[0].message)
    return
  }

  emit('publish', dagConfig)
  ElMessage.success('发布成功')
}

const handleRun = () => {
  const graph = canvasRef.value?.getGraph()
  if (!graph) return

  const dagConfig = exportDagFromGraph(graph)
  const errors = validateDag(dagConfig)

  if (errors.length > 0) {
    ElMessage.error(errors[0].message)
    return
  }

  emit('run', dagConfig)
}

// ========== 拖拽和节点操作 ==========

const handleDragNode = (nodeType: TaskNodeType, event: DragEvent) => {
  // 将节点类型信息存储到 dataTransfer 中
  event.dataTransfer?.setData('nodeType', nodeType)
}

const handleNodeClick = (node: DagNode) => {
  // 选中节点时清除边选中（互斥）
  selectedEdge.value = null
  selectedNode.value = node
  emit('node-select', node)
}

const handleNodeAdded = () => {
  pushHistory()
}

const handleSelectionChange = (nodes: DagNode[]) => {
  if (nodes.length === 1) {
    // 选中节点时清除边选中（互斥）
    selectedEdge.value = null
    selectedNode.value = nodes[0]
    emit('node-select', nodes[0])
  } else if (nodes.length === 0) {
    selectedNode.value = null
  }
}

// 处理边点击事件
const handleEdgeClick = (edge: DagEdge) => {
  // 选中边时清除节点选中（互斥）
  selectedNode.value = null
  selectedEdge.value = edge
}

// 处理边选中状态变化
const handleEdgeSelected = (edge: DagEdge | null) => {
  if (edge) {
    // 选中边时清除节点选中（互斥）
    selectedNode.value = null
    selectedEdge.value = edge
  } else {
    // 边取消选中时，清除选中状态
    selectedEdge.value = null
  }
}

// 处理边更新
const handleEdgeUpdate = (edge: DagEdge) => {
  // 更新边的数据
  if (canvasRef.value && selectedEdge.value) {
    canvasRef.value.updateEdge(edge.id, {
      sourcePort: edge.sourcePort,
      targetPort: edge.targetPort,
      condition: edge.condition,
    })
    // 更新选中边的数据，保持 UI 同步
    selectedEdge.value = edge
    pushHistory()
  }
}

// 处理属性面板更新
const handlePropertyUpdate = (node: DagNode) => {
  // 更新选中节点的数据，保持 UI 同步
  selectedNode.value = node
  // PropertyPanel 内部已调用 pushHistory，无需重复调用
}

// ========== 生命周期 ==========

onMounted(async () => {
  // 预加载数据源列表（节点配置面板需要）
  await etlStore.loadDatasources()

  // 初始化历史记录
  pushHistory()

  // 监听键盘快捷键
  document.addEventListener('keydown', handleKeyDown)
})

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeyDown)
})

// 监听初始配置变化，自动加载 DAG 配置
watch(
  [() => props.initialDagConfig, canvasRef],
  ([config, canvas]) => {
    if (config && canvas && config.nodes && config.nodes.length > 0) {
      const graph = canvas.getGraph()
      if (graph) {
        importDagToGraph(graph, config)
        pushHistory()
      }
    }
  },
  { immediate: true }
)

const handleKeyDown = (e: KeyboardEvent) => {
  // Ctrl+Z: 撤销
  if (e.ctrlKey && e.key === 'z' && !e.shiftKey) {
    e.preventDefault()
    handleUndo()
    return
  }

  // Ctrl+Y 或 Ctrl+Shift+Z: 重做
  if ((e.ctrlKey && e.key === 'y') || (e.ctrlKey && e.shiftKey && e.key === 'z')) {
    e.preventDefault()
    handleRedo()
    return
  }

  // Ctrl+S: 保存
  if (e.ctrlKey && e.key === 's') {
    e.preventDefault()
    handleSave()
    return
  }
}

// 提供给子组件的状态
provide('dagHistory', {
  canUndo,
  canRedo,
  pushHistory,
})

// ========== 暴露方法 ==========

defineExpose({
  /**
   * 获取 DAG 配置
   */
  getDagConfig: () => {
    const graph = canvasRef.value?.getGraph()
    if (!graph) return null
    return exportDagFromGraph(graph)
  },

  /**
   * 设置 DAG 配置
   */
  setDagConfig: (dagConfig: any) => {
    const graph = canvasRef.value?.getGraph()
    if (!graph) return

    graph.clearCells()
    dagConfig.nodes.forEach((node: DagNode) => {
      graph.addNode({
        id: node.id,
        shape: `etl-${node.type}`,
        x: node.position.x,
        y: node.position.y,
        data: { nodeType: node.type, name: node.name, config: node.config },
      })
    })
    dagConfig.edges.forEach((edge: any) => {
      graph.addEdge({
        id: edge.id,
        source: edge.sourceNodeId,
        target: edge.targetNodeId,
      })
    })
    pushHistory()
  },

  /**
   * 校验 DAG 配置
   */
  validate: () => {
    const graph = canvasRef.value?.getGraph()
    if (!graph) return []

    const dagConfig = exportDagFromGraph(graph)
    return validateDag(dagConfig)
  },
})
</script>

<style scoped lang="scss">
$dag-bg: #f5f7fa;
$dag-border: #e4e7ed;

.dag-designer {
  position: relative;
  width: 100%;
  height: 100%;
  background: $dag-bg;
  overflow: hidden;
  display: flex;
  flex-direction: column;

  * {
    box-sizing: border-box;
  }
}

.designer-body {
  display: flex;
  flex: 1;
  overflow: hidden;
  min-height: 0;
}
</style>
<!-- src/components/etl/DagDesigner/Canvas.vue -->
<template>
  <div class="dag-canvas" ref="canvasRef">
    <!-- X6 图实例容器 -->
    <div ref="graphContainerRef" class="graph-container"></div>

    <!-- 缩放控制 -->
    <div class="zoom-control">
      <el-button-group>
        <el-button size="small" @click="handleZoomOut">
          <el-icon><ZoomOut /></el-icon>
        </el-button>
        <el-button size="small" disabled>
          {{ zoomPercent }}%
        </el-button>
        <el-button size="small" @click="handleZoomIn">
          <el-icon><ZoomIn /></el-icon>
        </el-button>
        <el-button size="small" @click="handleFitView">
          <el-icon><FullScreen /></el-icon>
        </el-button>
      </el-button-group>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, inject } from 'vue'
import { ZoomIn, ZoomOut, FullScreen } from '@element-plus/icons-vue'
import type { Graph, Node as X6Node, Cell } from '@antv/x6'
import { createGraph } from './utils/graphConfig'
import { getNodeStyle } from './utils/nodeRegistry'
import { TaskNodeType } from '@/types/etl'
import type { DagNode, DagEdge } from '@/types/etl'
import { generateGuid } from '@/utils/guid'

// Props
const props = defineProps<{
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'node-click', node: DagNode): void
  (e: 'node-added', node: DagNode): void
  (e: 'selection-change', nodes: DagNode[]): void
  (e: 'edge-click', edge: DagEdge): void
  (e: 'edge-selected', edge: DagEdge | null): void
}>()

// Refs
const canvasRef = ref<HTMLElement>()
const graphContainerRef = ref<HTMLElement>()
const graphInstance = ref<Graph | null>(null)
const zoomPercent = ref(100)

// 注入历史记录（可选）
const dagHistory = inject<any>('dagHistory')

// ========== 图实例初始化 ==========

onMounted(() => {
  initGraph()
})

onUnmounted(() => {
  if (graphInstance.value) {
    graphInstance.value.dispose()
    graphInstance.value = null
  }
})

const initGraph = () => {
  if (!graphContainerRef.value) return

  // 创建图实例
  const graph = createGraph({
    container: graphContainerRef.value,
    minimap: true,
    readonly: props.readonly,
  })

  graphInstance.value = graph

  // 监听节点点击事件
  graph.on('node:click', ({ node }) => {
    const nodeData = node.getData()
    const dagNode: DagNode = {
      id: node.id,
      name: nodeData.name || node.id,
      type: nodeData.nodeType as TaskNodeType,
      position: { x: node.position().x, y: node.position().y },
      config: nodeData.config || {},
    }
    emit('node-click', dagNode)
  })

  // 监听节点添加事件
  graph.on('node:added', ({ node }) => {
    const nodeData = node.getData()
    if (nodeData.nodeType) {
      const dagNode: DagNode = {
        id: node.id,
        name: nodeData.name || node.id,
        type: nodeData.nodeType as TaskNodeType,
        position: { x: node.position().x, y: node.position().y },
        config: nodeData.config || {},
      }
      emit('node-added', dagNode)
    }
  })

  // 监听选择变化
  graph.on('selection:changed', ({ selected }) => {
    const nodes: DagNode[] = selected
      .filter((cell: Cell) => cell.isNode())
      .map((cell: Cell) => {
        const node = cell as X6Node
        const data = node.getData()
        return {
          id: node.id,
          name: data.name || node.id,
          type: data.nodeType as TaskNodeType,
          position: { x: node.position().x, y: node.position().y },
          config: data.config || {},
        }
      })
    emit('selection-change', nodes)
  })

  // 监听边连接完成事件
  graph.on('edge:connected', () => {
    dagHistory?.pushHistory()
  })

  // 监听边点击事件
  graph.on('edge:click', ({ edge }) => {
    const dagEdge: DagEdge = {
      id: edge.id,
      sourceNodeId: edge.getSourceCellId() || '',
      targetNodeId: edge.getTargetCellId() || '',
      sourcePort: edge.getSourcePortId(),
      targetPort: edge.getTargetPortId(),
      condition: edge.getData()?.condition,
    }
    emit('edge-click', dagEdge)
    emit('edge-selected', dagEdge)
  })

  // 监听边选中状态变化
  graph.on('edge:selected', ({ edge }) => {
    const dagEdge: DagEdge = {
      id: edge.id,
      sourceNodeId: edge.getSourceCellId() || '',
      targetNodeId: edge.getTargetCellId() || '',
      sourcePort: edge.getSourcePortId(),
      targetPort: edge.getTargetPortId(),
      condition: edge.getData()?.condition,
    }
    emit('edge-selected', dagEdge)
  })

  // 监听边取消选中
  graph.on('edge:unselected', () => {
    emit('edge-selected', null)
  })

  // 监听边删除事件
  graph.on('edge:removed', () => {
    dagHistory?.pushHistory()
    // 如果当前选中的边被删除，清除选中状态
    emit('edge-selected', null)
  })

  // 监听画布空白区域点击，清除所有选中
  graph.on('blank:click', () => {
    emit('edge-selected', null)
  })

  // 监听节点移动事件
  graph.on('node:moved', () => {
    dagHistory?.pushHistory()
  })

  // 监听节点删除事件
  graph.on('node:removed', () => {
    dagHistory?.pushHistory()
  })

  // 监听缩放变化
  graph.on('scale', () => {
    zoomPercent.value = Math.round(graph.zoom() * 100)
  })

  // 设置拖放处理
  setupDropHandler()
}

// ========== 拖放处理 ==========

const setupDropHandler = () => {
  if (!canvasRef.value || !graphInstance.value) return

  const canvas = canvasRef.value

  canvas.addEventListener('dragover', (e) => {
    e.preventDefault()
    if (e.dataTransfer) {
      e.dataTransfer.dropEffect = 'copy'
    }
  })

  canvas.addEventListener('drop', (e) => {
    e.preventDefault()

    const nodeType = e.dataTransfer?.getData('nodeType') as TaskNodeType
    if (!nodeType) return

    // 获取画布坐标
    const graph = graphInstance.value!
    const point = graph.clientToLocal(e.clientX, e.clientY)

    // 创建节点
    addNode(nodeType, point.x, point.y)
  })
}

// ========== 节点操作 ==========

const addNode = (nodeType: TaskNodeType, x: number, y: number) => {
  if (!graphInstance.value) return

  const style = getNodeStyle(nodeType)
  const nodeId = generateGuid()

  // 创建节点
  const node = graphInstance.value.addNode({
    id: nodeId,
    shape: `etl-${nodeType}`,
    x: x - style.width / 2, // 居中放置
    y: y - style.height / 2,
    data: {
      nodeType,
      name: `${style.label}_${nodeId.slice(0, 8)}`,
      config: {},
    },
  })

  // 设置节点标签
  node.setAttrs({
    label: {
      text: `${style.icon} ${style.label}`,
    },
  })

  dagHistory?.pushHistory()
}

// ========== 缩放控制 ==========

const handleZoomIn = () => {
  const graph = graphInstance.value
  if (graph) {
    graph.zoom(graph.zoom() * 1.2)
    zoomPercent.value = Math.round(graph.zoom() * 100)
  }
}

const handleZoomOut = () => {
  const graph = graphInstance.value
  if (graph) {
    graph.zoom(graph.zoom() * 0.8)
    zoomPercent.value = Math.round(graph.zoom() * 100)
  }
}

const handleFitView = () => {
  const graph = graphInstance.value
  if (graph) {
    graph.zoomToFit({ padding: 50 })
    zoomPercent.value = Math.round(graph.zoom() * 100)
  }
}

// ========== 暴露方法 ==========

defineExpose({
  /**
   * 获取图实例
   */
  getGraph: () => graphInstance.value,

  /**
   * 添加节点
   */
  addNode: (nodeType: TaskNodeType, x: number, y: number) => {
    addNode(nodeType, x, y)
  },

  /**
   * 获取所有节点
   */
  getNodes: () => {
    if (!graphInstance.value) return []
    return graphInstance.value.getNodes().map((node) => {
      const data = node.getData()
      return {
        id: node.id,
        name: data.name || node.id,
        type: data.nodeType as TaskNodeType,
        position: { x: node.position().x, y: node.position().y },
        config: data.config || {},
      }
    })
  },

  /**
   * 获取所有边
   */
  getEdges: () => {
    if (!graphInstance.value) return []
    return graphInstance.value.getEdges().map((edge) => {
      const data = edge.getData() || {}
      return {
        id: edge.id,
        sourceNodeId: edge.getSourceCellId() || '',
        targetNodeId: edge.getTargetCellId() || '',
        sourcePort: edge.getSourcePortId(),
        targetPort: edge.getTargetPortId(),
        condition: data.condition,
      }
    })
  },

  /**
   * 更新边数据
   */
  updateEdge: (edgeId: string, edgeData: Partial<DagEdge>) => {
    const graph = graphInstance.value
    if (!graph) return
    // 通过 getCells 查找边
    const cells = graph.getCells()
    const cell = cells.find(c => c.id === edgeId && c.isEdge())
    if (cell && cell.isEdge()) {
      const edge = cell as any
      const currentData = edge.getData() || {}
      edge.setData({
        ...currentData,
        ...edgeData,
      })
    }
  },

  /**
   * 清空画布
   */
  clearCanvas: () => {
    if (graphInstance.value) {
      graphInstance.value.clearCells()
    }
  },
})
</script>

<style scoped lang="scss">
$canvas-bg: #f5f7fa;

.dag-canvas {
  flex: 1;
  height: 100%;
  position: relative;
  overflow: hidden;
}

.graph-container {
  width: 100%;
  height: 100%;
}

.zoom-control {
  position: absolute;
  right: 16px;
  bottom: 16px;
  z-index: 10;
  background: #fff;
  border-radius: 8px;
  padding: 4px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);

  :deep(.el-button) {
    padding: 8px 12px;
  }
}
</style>
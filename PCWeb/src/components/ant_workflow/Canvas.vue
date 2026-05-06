<!-- src/components/ant_workflow/DagDesigner/Canvas.vue -->
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
import { getAntNodeStyle, createDefaultNodeData, updateNodePorts } from './utils/nodeRegistry'
import { AntNodeType } from '@/types/antWorkflow'
import type { DagNode, DagEdge } from '@/stores/antWorkflowStore'
import { generateGuid } from '@/utils/guid'

// Props
const props = defineProps<{
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'node-click', node: DagNode): void
  (e: 'node-added', node: DagNode): void
  (e: 'node-deleted', nodeId: string): void
  (e: 'edge-connected', edge: DagEdge): void
  (e: 'edge-deleted', edgeId: string): void
  (e: 'selection-change', nodes: DagNode[]): void
}>()

// Refs
const canvasRef = ref<HTMLElement>()
const graphContainerRef = ref<HTMLElement>()
const graphInstance = ref<Graph | null>(null)
const zoomPercent = ref(100)

// 注入历史记录（可选）
const dagHistory = inject<{ pushHistory: () => void }>('dagHistory')

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

  const graph = createGraph({
    container: graphContainerRef.value,
    minimap: false,
    readonly: props.readonly,
  })

  graphInstance.value = graph

  // 监听节点点击事件
  graph.on('node:click', ({ node }) => {
    const nodeData = node.getData()
    const dagNode: DagNode = {
      id: node.id,
      name: nodeData.name || node.id,
      type: nodeData.nodeType as AntNodeType,
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
        type: nodeData.nodeType as AntNodeType,
        position: { x: node.position().x, y: node.position().y },
        config: nodeData.config || {},
      }
      emit('node-added', dagNode)

      // 为条件/并行节点创建动态输出端口
      const nodeType = nodeData.nodeType as AntNodeType
      if (nodeType === AntNodeType.CONDITION && nodeData.config?.conditionNodes) {
        updateNodePorts(graph, node.id, nodeData.config.conditionNodes)
      } else if (nodeType === AntNodeType.PARALLEL && nodeData.config?.parallelNodes) {
        updateNodePorts(graph, node.id, nodeData.config.parallelNodes)
      }
    }
  })

  // 监听节点删除事件
  graph.on('node:removed', ({ node }) => {
    emit('node-deleted', node.id)
    dagHistory?.pushHistory()
  })

  // 监听边连接完成事件
  graph.on('edge:connected', ({ edge }) => {
    // 根据源节点和目标节点位置，动态设置连接线样式
    updateEdgeConnector(graph, edge)

    const dagEdge: DagEdge = {
      id: edge.id,
      sourceNodeId: edge.getSourceCellId() || '',
      targetNodeId: edge.getTargetCellId() || '',
      sourcePort: edge.getSourcePortId(),
      targetPort: edge.getTargetPortId(),
    }
    emit('edge-connected', dagEdge)
    dagHistory?.pushHistory()
  })

  // 监听边删除事件
  graph.on('edge:removed', ({ edge }) => {
    emit('edge-deleted', edge.id)
    dagHistory?.pushHistory()
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
          type: data.nodeType as AntNodeType,
          position: { x: node.position().x, y: node.position().y },
          config: data.config || {},
        }
      })
    emit('selection-change', nodes)
  })

  // 监听节点移动事件 - 更新所有连接的边样式
  graph.on('node:moved', ({ node }) => {
    // 获取该节点连接的所有边，更新连接器样式
    const edges = graph.getConnectedEdges(node)
    edges.forEach(edge => updateEdgeConnector(graph, edge))
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
  if (!graphContainerRef.value || !graphInstance.value) return

  const graphContainer = graphContainerRef.value

  graphContainer.addEventListener('dragover', (e) => {
    e.preventDefault()
    e.stopPropagation()
    if (e.dataTransfer) {
      e.dataTransfer.dropEffect = 'copy'
    }
  })

  graphContainer.addEventListener('drop', (e) => {
    e.preventDefault()
    e.stopPropagation()

    const nodeType = e.dataTransfer?.getData('nodeType') as AntNodeType
    if (!nodeType) return

    const graph = graphInstance.value!
    const point = graph.clientToLocal(e.clientX, e.clientY)

    addNode(nodeType, point.x, point.y)
  })
}

// ========== 节点操作 ==========

const addNode = (nodeType: AntNodeType, x: number, y: number) => {
  if (!graphInstance.value) return

  const style = getAntNodeStyle(nodeType)
  const nodeId = generateGuid()

  const node = graphInstance.value.addNode({
    id: nodeId,
    shape: `ant-${nodeType}`,
    x: x - style.width / 2,
    y: y - style.height / 2,
    data: createDefaultNodeData(nodeType),
  })

  dagHistory?.pushHistory()

  return node
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

// ========== 边连接器更新 ==========

/**
 * 根据源节点和目标节点位置，动态更新边的连接器样式
 * - 同一水平线：使用直线 (normal)
 * - 不同水平线：使用平滑曲线 (smooth)
 */
const updateEdgeConnector = (graph: Graph, edge: any) => {
  const sourceCell = graph.getCellById(edge.getSourceCellId())
  const targetCell = graph.getCellById(edge.getTargetCellId())

  if (!sourceCell || !targetCell || !sourceCell.isNode() || !targetCell.isNode()) return

  const sourceNode = sourceCell as any
  const targetNode = targetCell as any

  const sourcePos = sourceNode.getPosition()
  const targetPos = targetNode.getPosition()
  const sourceSize = sourceNode.getSize()
  const targetSize = targetNode.getSize()

  // 计算节点中心 Y 坐标
  const sourceCenterY = sourcePos.y + sourceSize.height / 2
  const targetCenterY = targetPos.y + targetSize.height / 2

  // 判断是否在同一水平线上（允许 20px 误差）
  const isSameHorizontalLine = Math.abs(sourceCenterY - targetCenterY) < 20

  // 根据位置选择连接器
  const connector = isSameHorizontalLine
    ? { name: 'normal' } // 直线连接
    : { name: 'smooth' } // 平滑曲线连接

  // 更新边的连接器
  edge.setConnector(connector)
}

// ========== 暴露方法 ==========

defineExpose({
  getGraph: () => graphInstance.value,
  addNode,
  getNodes: () => {
    if (!graphInstance.value) return []
    return graphInstance.value.getNodes().map((node) => {
      const data = node.getData()
      return {
        id: node.id,
        name: data.name || node.id,
        type: data.nodeType as AntNodeType,
        position: { x: node.position().x, y: node.position().y },
        config: data.config || {},
      }
    })
  },
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
  clearCanvas: () => {
    if (graphInstance.value) {
      graphInstance.value.clearCells()
    }
  },
  updateNodeData: (nodeId: string, data: Partial<{ name: string; config: any }>) => {
    const graph = graphInstance.value
    if (!graph) return
    const node = graph.getCellById(nodeId)
    if (node && node.isNode()) {
      const currentData = node.getData() || {}
      node.setData({ ...currentData, ...data })

      // 更新节点名称显示
      if (data.name) {
        node.attr('label/text', data.name)
      }

      // 为条件/并行节点更新输出端口
      if (data.config) {
        const nodeType = currentData.nodeType as AntNodeType
        if (nodeType === AntNodeType.CONDITION && data.config.conditionNodes) {
          updateNodePorts(graph, nodeId, data.config.conditionNodes)
        } else if (nodeType === AntNodeType.PARALLEL && data.config.parallelNodes) {
          updateNodePorts(graph, nodeId, data.config.parallelNodes)
        }
      }
    }
  },
})
</script>

<style scoped lang="scss">
$canvas-bg: #f5f7fa;

.dag-canvas {
  flex: 1;
  min-width: 0;
  height: 100%;
  position: relative;
  overflow: hidden; // 隐藏溢出，不显示滚动条
}

.graph-container {
  width: 100%;
  height: 100%;
  overflow: hidden; // 隐藏滚动条

  // 隐藏所有滚动条
  &::-webkit-scrollbar {
    display: none;
    width: 0;
    height: 0;
  }

  // 端口样式
  :deep(.x6-port) {
    circle {
      cursor: crosshair;
      transition: all 0.2s;
    }
  }

  // 端口 hover 效果
  :deep(.x6-port:hover) {
    circle {
      r: 8;
      fill: #31d0c6;
      stroke-width: 3;
    }
  }

  // 连线 hover 效果
  :deep(.x6-edge:hover) {
    .x6-edge-path {
      stroke-width: 3;
      filter: drop-shadow(0 2px 4px rgba(24, 144, 255, 0.3));
    }
  }

  // 端口高亮状态（可连接）
  :deep(.x6-port-highlight) {
    circle {
      r: 10;
      fill: #1890ff;
      stroke: #1890ff;
      stroke-width: 3;
    }
  }
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
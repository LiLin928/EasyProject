/**
 * X6 图实例配置工具
 * 创建和管理 X6 图实例
 */
import { Graph, Shape } from '@antv/x6'
import { registerAntWorkflowNodes, updateNodePorts } from './nodeRegistry'
import { AntNodeType } from '@/types/antWorkflow'
import type { DagNode, DagEdge, DagConfig } from '@/stores/antWorkflowStore'

// 在模块加载时注册节点
registerAntWorkflowNodes()

// ==================== 连线样式配置 ====================

/**
 * 默认连线样式（蓝色实线）
 */
const defaultEdgeAttrs = {
  line: {
    stroke: '#1890ff',
    strokeWidth: 2,
    strokeLinecap: 'round',
    strokeLinejoin: 'round',
    targetMarker: {
      name: 'block',
      args: {
        size: 6,
        width: 10,
        height: 8,
        fill: '#1890ff',
        stroke: '#1890ff',
      },
    },
  },
}

/**
 * 条件分支连线样式（不同颜色）
 */
const conditionEdgeStyles: Record<string, any> = {
  // 高优先级/高金额 - 橙色
  high: {
    line: {
      stroke: '#fa8c16',
      strokeWidth: 2,
      targetMarker: {
        name: 'block',
        args: { size: 6, width: 10, height: 8, fill: '#fa8c16' },
      },
    },
  },
  // 默认分支 - 绿色
  default: {
    line: {
      stroke: '#52c41a',
      strokeWidth: 2,
      targetMarker: {
        name: 'block',
        args: { size: 6, width: 10, height: 8, fill: '#52c41a' },
      },
    },
  },
  // 其他条件 - 紫色
  condition: {
    line: {
      stroke: '#722ed1',
      strokeWidth: 2,
      targetMarker: {
        name: 'block',
        args: { size: 6, width: 10, height: 8, fill: '#722ed1' },
      },
    },
  },
}

/**
 * 创建连线样式（根据条件返回不同样式）
 */
function createEdgeStyle(condition?: any): any {
  if (!condition) {
    return defaultEdgeAttrs
  }

  // 根据分支类型选择样式
  if (condition.isDefault) {
    return conditionEdgeStyles.default
  }
  if (condition.priority === 1) {
    return conditionEdgeStyles.high
  }
  return conditionEdgeStyles.condition
}

/**
 * 创建 X6 图实例配置选项
 */
export interface CreateGraphOptions {
  /** 容器元素 */
  container: HTMLElement
  /** 是否启用 minimap */
  minimap?: boolean
  /** 是否只读模式 */
  readonly?: boolean
}

/**
 * 创建 X6 图实例
 */
export function createGraph(options: CreateGraphOptions): Graph {
  const { container, minimap = false, readonly = false } = options

  const graph = new Graph({
    container,
    grid: {
      size: 20,
      visible: true,
      type: 'dot',
      args: {
        color: '#e0e0e0',
        thickness: 1,
      },
    },
    // 启用画布平移（可横向拖动）
    panning: {
      enabled: true,
      modifiers: [],
      eventTypes: ['leftMouseDown', 'mouseWheel'],
    },
    mousewheel: {
      enabled: true,
      modifiers: ['ctrl'],
      minScale: 0.2,
      maxScale: 4,
    },
    connecting: {
      allowBlank: false,
      allowLoop: false,
      allowNode: false,  // 禁止连接到节点本身，只能连接到端口
      allowEdge: false,
      allowPort: true,
      highlight: true,
      // 端点吸附配置
      snap: {
        radius: 20,  // 吸附半径
      },
      // 端口验证：输出端口只能作为源，输入端口只能作为目标
      validatePort({ type, port }) {
        const portGroup = port?.group
        // source 只能连接到 out 端口
        if (type === 'source') {
          return portGroup === 'out'
        }
        // target 只能连接到 in 端口
        if (type === 'target') {
          return portGroup === 'in'
        }
        return true
      },
      createEdge() {
        // 只读模式下不添加删除工具
        const edgeTools = readonly ? [] : [
          {
            name: 'button-remove',
            args: {
              distance: -40,
              offset: { x: 0, y: 0 },
            },
          },
        ]
        return new Shape.Edge({
          attrs: defaultEdgeAttrs,
          connector: { name: 'rounded', args: { radius: 8 } },
          zIndex: 0,
          tools: edgeTools,
        })
      },
    },
    highlighting: {
      magnetAvailable: {
        name: 'stroke',
        args: {
          attrs: {
            fill: '#fff',
            stroke: '#1890ff',
            strokeWidth: 4,
          },
        },
      },
      magnetAdsorbed: {
        name: 'stroke',
        args: {
          attrs: {
            fill: '#fff',
            stroke: '#52c41a',
            strokeWidth: 4,
          },
        },
      },
    },
    interacting: {
      nodeMovable: !readonly,
      edgeMovable: !readonly,
      edgeLabelMovable: !readonly,
    },
  })

  // minimap 插件 - 可选启用
  // if (minimap) {
  //   graph.use(new Minimap())
  // }

  return graph
}

/**
 * 导出 DAG 配置从图实例
 */
export function exportDagFromGraph(graph: Graph): DagConfig {
  const nodes = graph.getNodes().map((node) => {
    const data = node.getData()
    return {
      id: node.id,
      name: data.name || node.id,
      type: data.nodeType,
      position: {
        x: node.position().x,
        y: node.position().y,
      },
      config: data.config || {},
    }
  })

  const edges = graph.getEdges().map((edge) => {
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

  return {
    version: '1.0',
    nodes,
    edges,
  }
}

/**
 * 加载 DAG 配置到图实例
 */
export function loadDagToGraph(graph: Graph, dagConfig: DagConfig, readonly = false) {
  // 清空画布
  graph.clearCells()

  // 添加节点
  dagConfig.nodes.forEach((nodeData) => {
    // 安全检查：position 可能为 undefined，使用默认值
    const x = nodeData.position?.x ?? 100
    const y = nodeData.position?.y ?? 100

    const node = graph.addNode({
      id: nodeData.id,
      shape: `ant-${nodeData.type}`,
      x,
      y,
      data: {
        nodeType: nodeData.type,
        name: nodeData.name,
        config: nodeData.config,
      },
    })

    // 更新节点名称显示
    if (nodeData.name) {
      node.attr('label/text', nodeData.name)
    }

    // 只读模式下移除节点的删除工具
    if (readonly) {
      node.removeTools()
    }

    // 为条件/并行节点创建动态输出端口
    const nodeType = nodeData.type as AntNodeType
    if (nodeType === AntNodeType.CONDITION && nodeData.config?.conditionNodes) {
      updateNodePorts(graph, nodeData.id, nodeData.config.conditionNodes)
    } else if (nodeType === AntNodeType.PARALLEL && nodeData.config?.parallelNodes) {
      updateNodePorts(graph, nodeData.id, nodeData.config.parallelNodes)
    }
  })

  // 添加边（根据节点位置自动选择直线或曲线）
  dagConfig.edges.forEach((edgeData) => {
    const edgeStyle = createEdgeStyle(edgeData.condition)

    // 获取源节点和目标节点
    const sourceNode = graph.getCellById(edgeData.sourceNodeId)
    const targetNode = graph.getCellById(edgeData.targetNodeId)

    if (sourceNode && targetNode && sourceNode.isNode() && targetNode.isNode()) {
      // 获取源节点端口列表，处理 sourcePort 缺失或不匹配的情况
      const sourcePorts = sourceNode.getPorts()
      const sourceOutPorts = sourcePorts.filter(p => p.group === 'out')

      // 如果 sourcePort 指定的端口不存在，尝试使用第一个输出端口
      let actualSourcePort = edgeData.sourcePort
      if (!actualSourcePort || !sourcePorts.find(p => p.id === actualSourcePort)) {
        actualSourcePort = sourceOutPorts.length > 0 ? sourceOutPorts[0].id : 'out'
      }

      // 获取目标节点端口列表，处理 targetPort 缺失或不匹配的情况
      const targetPorts = targetNode.getPorts()
      const targetInPorts = targetPorts.filter(p => p.group === 'in')

      // 如果 targetPort 指定的端口不存在，尝试使用第一个输入端口
      let actualTargetPort = edgeData.targetPort
      if (!actualTargetPort || !targetPorts.find(p => p.id === actualTargetPort)) {
        actualTargetPort = targetInPorts.length > 0 ? targetInPorts[0].id : 'in'
      }

      // 获取节点位置用于选择连接器类型
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

      // 只读模式下不添加删除工具
      const edgeTools = readonly ? [] : [
        {
          name: 'button-remove',
          args: {
            distance: -40,
            offset: { x: 0, y: 0 },
          },
        },
      ]

      graph.addEdge({
        id: edgeData.id,
        source: {
          cell: edgeData.sourceNodeId,
          port: actualSourcePort,
        },
        target: {
          cell: edgeData.targetNodeId,
          port: actualTargetPort,
        },
        attrs: edgeStyle,
        connector,
        data: edgeData.condition,
        tools: edgeTools,
      })
    }
  })
}
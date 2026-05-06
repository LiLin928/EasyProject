/**
 * X6 节点注册工具
 * 注册 ant_workflow 自定义节点到 X6 图引擎
 */
import { Graph } from '@antv/x6'
import { AntNodeType, nodeStyleMap, type NodeStyleConfig } from '@/types/antWorkflow'

/**
 * 节点尺寸配置
 */
const NODE_SIZE = {
  DEFAULT: { width: 160, height: 50 },
  CONDITION: { width: 160, height: 70 },
  PARALLEL: { width: 160, height: 70 },
}

/**
 * 注册 ant_workflow 自定义节点到 X6
 */
export function registerAntWorkflowNodes() {
  Object.entries(nodeStyleMap).forEach(([type, config]) => {
    const isBranchNode = type === AntNodeType.CONDITION || type === AntNodeType.PARALLEL
    const size = isBranchNode ? NODE_SIZE.CONDITION : NODE_SIZE.DEFAULT

    Graph.registerNode(`ant-${type}`, {
      inherit: 'rect',
      width: size.width,
      height: size.height,
      attrs: {
        body: {
          fill: config.bgColor,
          stroke: config.borderColor,
          strokeWidth: 2,
          rx: 8,
          ry: 8,
        },
        label: {
          text: config.typeName,
          fill: config.color,
          fontSize: 14,
          fontWeight: 500,
          refX: '50%',
          refY: '50%',
          textAnchor: 'middle',
          textVerticalAnchor: 'middle',
        },
      },
      // 节点工具：删除按钮（右上角）
      tools: [
        {
          name: 'button-remove',
          args: {
            x: '100%',  // 右侧边缘
            y: 0,       // 顶部边缘
            offset: { x: -8, y: 8 },  // 微调：向左8px，向下8px，避免完全贴边
          },
        },
      ],
      ports: {
        groups: {
          // 输入端口：左侧（只接受入边）
          in: {
            position: 'left',
            attrs: {
              circle: {
                r: 6,
                magnet: true,  // 可以作为连接目标
                stroke: '#31d0c6',
                strokeWidth: 2,
                fill: '#fff',
                style: {
                  cursor: 'crosshair',  // 显示可连接的鼠标样式
                },
              },
            },
          },
          // 输出端口：右侧（只发出出边）
          out: {
            position: 'right',
            attrs: {
              circle: {
                r: 6,
                magnet: true,  // 可以作为连接源
                stroke: '#31d0c6',
                strokeWidth: 2,
                fill: '#fff',
                style: {
                  cursor: 'crosshair',  // 显示可连接的鼠标样式
                },
              },
              text: {
                fontSize: 11,
                fill: '#666',
                refX: 12,
                textAnchor: 'start',
              },
            },
          },
        },
        items: isBranchNode || type === AntNodeType.END
          ? [{ id: 'in', group: 'in', restrict: 'target' }]  // 输入端口只能作为目标
          : [
              { id: 'in', group: 'in', restrict: 'target' },   // 输入端口只能作为目标
              { id: 'out', group: 'out', restrict: 'source' },  // 输出端口只能作为源
            ],
      },
      data: { nodeType: type },
    })
  })
}

/**
 * 更新节点的输出端口（用于条件/并行节点）
 * 分支端口在节点右侧纵向排列
 * @param graph X6 图实例
 * @param nodeId 节点ID
 * @param branches 分支列表
 */
export function updateNodePorts(
  graph: Graph,
  nodeId: string,
  branches: { id: string; name: string; isDefault?: boolean }[]
) {
  const node = graph.getCellById(nodeId)
  if (!node || !node.isNode()) return

  // 移除旧的输出端口
  const existingPorts = node.getPorts()
  existingPorts.forEach((port) => {
    if (port.group === 'out') {
      node.removePort(port.id)
    }
  })

  // 添加新的输出端口（每个分支一个，在右侧纵向分布）
  const nodeHeight = node.getSize().height
  const portCount = branches.length
  const spacing = nodeHeight / (portCount + 1) // 等间距分布

  branches.forEach((branch, index) => {
    node.addPort({
      id: branch.id,
      group: 'out',
      restrict: 'source',  // 输出端口只能作为源
      attrs: {
        text: {
          text: branch.name,
        },
      },
      args: {
        y: spacing * (index + 1) - nodeHeight / 2,
      },
    })
  })
}

/**
 * 获取节点样式配置
 */
export function getAntNodeStyle(type: AntNodeType): NodeStyleConfig & { width: number; height: number } {
  const baseStyle = nodeStyleMap[type] || nodeStyleMap[AntNodeType.APPROVER]
  const size = type === AntNodeType.CONDITION || type === AntNodeType.PARALLEL
    ? NODE_SIZE.CONDITION
    : NODE_SIZE.DEFAULT

  return {
    ...baseStyle,
    width: size.width,
    height: size.height,
  }
}

/**
 * 创建默认节点数据
 */
export function createDefaultNodeData(type: AntNodeType) {
  const style = nodeStyleMap[type]

  // 条件分支节点的默认配置
  if (type === AntNodeType.CONDITION) {
    return {
      nodeType: type,
      name: style.typeName,
      config: {
        conditionNodes: [
          { id: 'branch-1', name: '分支1', priority: 1, conditionRules: [], isDefault: false },
          { id: 'branch-default', name: '默认', priority: 2, conditionRules: [], isDefault: true },
        ],
      },
    }
  }

  // 并行分支节点的默认配置
  if (type === AntNodeType.PARALLEL) {
    return {
      nodeType: type,
      name: style.typeName,
      config: {
        parallelNodes: [
          { id: 'parallel-1', name: '分支1' },
          { id: 'parallel-2', name: '分支2' },
        ],
        completeCondition: 'all',
        completeCount: 1,
      },
    }
  }

  // 结束节点的默认配置
  if (type === AntNodeType.END) {
    return {
      nodeType: type,
      name: style.typeName,
      config: {
        endType: 'success',
        notification: {
          enabled: false,
          type: 'message',
          title: '',
          content: '',
          recipients: [],
        },
        callbackUrl: '',
      },
    }
  }

  return {
    nodeType: type,
    name: style.typeName,
    config: {},
  }
}

/**
 * 获取节点显示名称
 */
export function getNodeDisplayName(type: AntNodeType): string {
  return nodeStyleMap[type]?.typeName || type
}
/**
 * X6 节点类型注册表
 */
import { Graph } from '@antv/x6'
import { TaskNodeType } from '@/types/etl'

// 标记是否已注册，避免重复注册
let nodesRegistered = false

export interface NodeStyleConfig {
  bgColor: string
  borderColor: string
  textColor: string
  icon: string
  label: string
  width: number
  height: number
}

export const nodeStyleMap: Record<TaskNodeType, NodeStyleConfig> = {
  [TaskNodeType.DATASOURCE]: { bgColor: '#E8F5E9', borderColor: '#4CAF50', textColor: '#2E7D32', icon: '📊', label: '数据源', width: 180, height: 60 },
  [TaskNodeType.SQL]: { bgColor: '#E3F2FD', borderColor: '#2196F3', textColor: '#1565C0', icon: '📝', label: 'SQL', width: 180, height: 60 },
  [TaskNodeType.TRANSFORM]: { bgColor: '#FFF3E0', borderColor: '#FF9800', textColor: '#E65100', icon: '🔄', label: '数据转换', width: 180, height: 60 },
  [TaskNodeType.OUTPUT]: { bgColor: '#FCE4EC', borderColor: '#E91E63', textColor: '#C2185B', icon: '📤', label: '数据输出', width: 180, height: 60 },
  [TaskNodeType.API]: { bgColor: '#F3E5F5', borderColor: '#9C27B0', textColor: '#6A1B9A', icon: '🌐', label: 'API调用', width: 180, height: 60 },
  [TaskNodeType.FILE]: { bgColor: '#ECEFF1', borderColor: '#607D8B', textColor: '#37474F', icon: '📁', label: '文件处理', width: 180, height: 60 },
  [TaskNodeType.SCRIPT]: { bgColor: '#263238', borderColor: '#455A64', textColor: '#FFFFFF', icon: '🖥️', label: '脚本', width: 180, height: 60 },
  [TaskNodeType.CONDITION]: { bgColor: '#FFFDE7', borderColor: '#FFC107', textColor: '#F57F17', icon: '⚖️', label: '条件判断', width: 180, height: 80 },
  [TaskNodeType.PARALLEL]: { bgColor: '#E0F7FA', borderColor: '#00BCD4', textColor: '#006064', icon: '⚡', label: '并行执行', width: 180, height: 80 },
  [TaskNodeType.NOTIFICATION]: { bgColor: '#FFEBEE', borderColor: '#F44336', textColor: '#C62828', icon: '📧', label: '通知', width: 180, height: 60 },
  [TaskNodeType.SUBFLOW]: { bgColor: '#E8EAF6', borderColor: '#3F51B5', textColor: '#1A237E', icon: '📦', label: '子流程', width: 180, height: 60 },
}

export function registerCustomNodes() {
  // 避免重复注册
  if (nodesRegistered) return
  nodesRegistered = true

  Object.entries(nodeStyleMap).forEach(([type, config]) => {
    Graph.registerNode(`etl-${type}`, {
      inherit: 'rect',
      width: config.width,
      height: config.height,
      attrs: {
        body: { fill: config.bgColor, stroke: config.borderColor, strokeWidth: 2, rx: 8, ry: 8 },
        label: { text: '', fill: config.textColor, fontSize: 14, refX: '50%', refY: '50%' },
      },
      ports: {
        groups: {
          top: {
            position: 'top',
            attrs: {
              circle: {
                r: 6,
                magnet: true,
                stroke: '#31d0c6',
                strokeWidth: 2,
                fill: '#fff',
              },
            },
          },
          bottom: {
            position: 'bottom',
            attrs: {
              circle: {
                r: 6,
                magnet: true,
                stroke: '#31d0c6',
                strokeWidth: 2,
                fill: '#fff',
              },
            },
          },
        },
        items: [
          { id: 'in', group: 'top' },
          { id: 'out', group: 'bottom' },
        ],
      },
      data: { nodeType: type },
    })
  })
}

export function getNodeStyle(type: TaskNodeType): NodeStyleConfig {
  return nodeStyleMap[type] || nodeStyleMap[TaskNodeType.SQL]
}
/**
 * DAG 自动布局工具
 * 使用简单的自定义布局算法，避免 @antv/layout 版本兼容性问题
 */
import type { Graph, Node } from '@antv/x6'

export interface LayoutOptions {
  direction: 'TB' | 'LR'
  nodeSep: number
  rankSep: number
}

interface LayoutNode {
  id: string
  x: number
  y: number
  width: number
  height: number
}

/**
 * 简单的层次布局算法
 */
export function autoLayout(graph: Graph, options: Partial<LayoutOptions> = {}) {
  const opts = { direction: 'TB', nodeSep: 50, rankSep: 80, ...options }

  const nodes = graph.getNodes()
  const edges = graph.getEdges()

  if (nodes.length === 0) return

  // 构建邻接表
  const adjacencyList = new Map<string, string[]>()
  const inDegree = new Map<string, number>()

  nodes.forEach((node) => {
    adjacencyList.set(node.id, [])
    inDegree.set(node.id, 0)
  })

  edges.forEach((edge) => {
    const source = edge.getSourceCellId()
    const target = edge.getTargetCellId()
    if (source && target) {
      adjacencyList.get(source)?.push(target)
      inDegree.set(target, (inDegree.get(target) || 0) + 1)
    }
  })

  // 拓扑排序分层
  const layers: string[][] = []
  const visited = new Set<string>()
  const queue: string[] = []

  // 找到所有入度为0的节点作为第一层
  inDegree.forEach((degree, nodeId) => {
    if (degree === 0) {
      queue.push(nodeId)
    }
  })

  while (queue.length > 0) {
    const currentLayer = [...queue]
    layers.push(currentLayer)
    queue.length = 0

    currentLayer.forEach((nodeId) => {
      visited.add(nodeId)
      const neighbors = adjacencyList.get(nodeId) || []
      neighbors.forEach((neighbor) => {
        if (!visited.has(neighbor)) {
          const newDegree = (inDegree.get(neighbor) || 1) - 1
          inDegree.set(neighbor, newDegree)
          if (newDegree === 0) {
            queue.push(neighbor)
          }
        }
      })
    })
  }

  // 处理未访问的节点（可能存在环）
  nodes.forEach((node) => {
    if (!visited.has(node.id)) {
      if (layers.length === 0) {
        layers.push([node.id])
      } else {
        layers[layers.length - 1].push(node.id)
      }
    }
  })

  // 计算节点位置
  const layoutNodes: LayoutNode[] = []

  if (opts.direction === 'TB') {
    // 从上到下
    layers.forEach((layer, layerIndex) => {
      const layerWidth = layer.length * opts.nodeSep
      const startX = -layerWidth / 2

      layer.forEach((nodeId, nodeIndex) => {
        const node = graph.getCellById(nodeId) as Node
        const size = node?.getSize() || { width: 100, height: 40 }
        layoutNodes.push({
          id: nodeId,
          x: startX + nodeIndex * opts.nodeSep + size.width / 2,
          y: layerIndex * opts.rankSep,
          width: size.width,
          height: size.height,
        })
      })
    })
  } else {
    // 从左到右
    layers.forEach((layer, layerIndex) => {
      const layerHeight = layer.length * opts.nodeSep
      const startY = -layerHeight / 2

      layer.forEach((nodeId, nodeIndex) => {
        const node = graph.getCellById(nodeId) as Node
        const size = node?.getSize() || { width: 100, height: 40 }
        layoutNodes.push({
          id: nodeId,
          x: layerIndex * opts.rankSep,
          y: startY + nodeIndex * opts.nodeSep + size.height / 2,
          width: size.width,
          height: size.height,
        })
      })
    })
  }

  // 应用布局
  layoutNodes.forEach((layoutNode) => {
    const graphNode = graph.getCellById(layoutNode.id) as Node
    if (graphNode) {
      graphNode.position(
        layoutNode.x - layoutNode.width / 2,
        layoutNode.y - layoutNode.height / 2
      )
    }
  })
}
/**
 * DAG 校验工具
 */
import type { DagConfig, DagNode } from '@/types/etl'
import { getNodeStyle } from './nodeRegistry'

export interface ValidationError {
  type: 'empty' | 'cycle' | 'config' | 'connection' | 'start' | 'end'
  message: string
  nodeId?: string
  nodeName?: string
}

export function validateDag(dagConfig: DagConfig): ValidationError[] {
  const errors: ValidationError[] = []
  if (!dagConfig.nodes || dagConfig.nodes.length === 0) {
    errors.push({ type: 'empty', message: '任务流至少需要一个任务节点' })
    return errors
  }

  dagConfig.nodes.forEach((node) => {
    if (!node.id || !node.name || !node.type) {
      errors.push({ type: 'config', message: `节点配置不完整`, nodeId: node.id, nodeName: node.name })
    }
  })

  const targetNodeIds = new Set(dagConfig.edges.map((e) => e.targetNodeId))
  const startNodes = dagConfig.nodes.filter((node) => !targetNodeIds.has(node.id))
  if (startNodes.length === 0) {
    errors.push({ type: 'start', message: '必须存在没有上游依赖的起始节点' })
  }

  const sourceNodeIds = new Set(dagConfig.edges.map((e) => e.sourceNodeId))
  const endNodes = dagConfig.nodes.filter((node) => !sourceNodeIds.has(node.id))
  if (endNodes.length === 0) {
    errors.push({ type: 'end', message: '必须存在没有下游依赖的终止节点' })
  }

  if (hasCycle(dagConfig)) {
    errors.push({ type: 'cycle', message: 'DAG 中存在循环依赖' })
  }

  return errors
}

function hasCycle(dagConfig: DagConfig): boolean {
  const adjacencyList = new Map<string, string[]>()
  dagConfig.nodes.forEach((node) => adjacencyList.set(node.id, []))
  dagConfig.edges.forEach((edge) => {
    const neighbors = adjacencyList.get(edge.sourceNodeId) || []
    neighbors.push(edge.targetNodeId)
    adjacencyList.set(edge.sourceNodeId, neighbors)
  })

  const visited = new Set<string>()
  const recursionStack = new Set<string>()

  function dfs(nodeId: string): boolean {
    visited.add(nodeId)
    recursionStack.add(nodeId)
    const neighbors = adjacencyList.get(nodeId) || []
    for (const neighbor of neighbors) {
      if (!visited.has(neighbor)) { if (dfs(neighbor)) return true }
      else if (recursionStack.has(neighbor)) return true
    }
    recursionStack.delete(nodeId)
    return false
  }

  for (const node of dagConfig.nodes) {
    if (!visited.has(node.id)) { if (dfs(node.id)) return true }
  }
  return false
}

export function exportDagFromGraph(graph: any): DagConfig {
  const nodes: DagNode[] = []
  const edges: any[] = []

  graph.getNodes().forEach((node: any) => {
    const data = node.getData()
    const position = node.position()
    nodes.push({
      id: node.id,
      name: data.name || node.id,
      type: data.nodeType,
      position: { x: position.x, y: position.y },
      config: data.config || {},
    })
  })

  graph.getEdges().forEach((edge: any) => {
    edges.push({
      id: edge.id,
      sourceNodeId: edge.getSourceCellId() || '',
      targetNodeId: edge.getTargetCellId() || '',
    })
  })

  return { version: '1.0', nodes, edges }
}

export function importDagToGraph(graph: any, dagConfig: DagConfig) {
  graph.clearCells()
  dagConfig.nodes.forEach((node) => {
    // 获取节点样式配置
    const style = getNodeStyle(node.type)
    // 创建节点并设置名称标签
    graph.addNode({
      id: node.id,
      shape: `etl-${node.type}`,
      x: node.position.x,
      y: node.position.y,
      attrs: {
        label: {
          text: `${style.icon} ${node.name || style.label}`,
        },
      },
      data: { nodeType: node.type, name: node.name, config: node.config },
    })
  })
  dagConfig.edges.forEach((edge) => {
    graph.addEdge({
      id: edge.id,
      shape: 'dag-edge',
      source: edge.sourceNodeId,
      target: edge.targetNodeId,
    })
  })
}
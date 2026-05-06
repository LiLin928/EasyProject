/**
 * X6 图配置工具
 */
import { Graph } from '@antv/x6'
import { Selection } from '@antv/x6-plugin-selection'
import { Snapline } from '@antv/x6-plugin-snapline'
import { Keyboard } from '@antv/x6-plugin-keyboard'
import { History } from '@antv/x6-plugin-history'
import { Clipboard } from '@antv/x6-plugin-clipboard'
import { MiniMap } from '@antv/x6-plugin-minimap'
import { registerCustomNodes } from './nodeRegistry'

export interface GraphOptions {
  container: HTMLElement
  minimap?: boolean
  readonly?: boolean
}

/**
 * 注册自定义边样式
 */
function registerCustomEdge() {
  Graph.registerEdge('dag-edge', {
    inherit: 'edge',
    attrs: {
      line: {
        stroke: '#A2B1C3',
        strokeWidth: 2,
        targetMarker: {
          name: 'block',
          width: 12,
          height: 8,
        },
      },
    },
    connector: {
      name: 'rounded',
      args: {
        radius: 8,
      },
    },
    router: {
      name: 'manhattan',
      args: {
        padding: 10,
      },
    },
  })
}

export function createGraph(options: GraphOptions): Graph {
  const { container, minimap = true, readonly = false } = options

  // 注册自定义节点和边
  registerCustomNodes()
  registerCustomEdge()

  const graph = new Graph({
    container,
    width: container.offsetWidth,
    height: container.offsetHeight,
    background: { color: '#f5f7fa' },
    grid: { visible: true, type: 'dot', size: 20, args: { color: '#d0d5dd' } },
    panning: { enabled: true },
    mousewheel: { enabled: true, minScale: 0.3, maxScale: 2 },
    connecting: {
      anchor: 'center',
      connectionPoint: 'anchor',
      allowBlank: false,
      allowLoop: false,
      highlight: true,
      snap: true,
      allowNode: true,
      allowEdge: false,
      allowPort: true,
      createEdge() {
        return this.createEdge({
          shape: 'dag-edge',
        })
      },
    },
    interacting: { nodeMovable: !readonly },
  })

  graph.use(new Selection({ multiple: true, modifiers: 'shift' }))
  graph.use(new Snapline({ enabled: true }))
  graph.use(new Keyboard({ enabled: true }))
  graph.use(new History({ enabled: true }))
  graph.use(new Clipboard({ enabled: true }))
  if (minimap) graph.use(new MiniMap({ width: 150, height: 100 }))

  if (!readonly) {
    graph.bindKey(['backspace', 'delete'], () => { const cells = graph.getSelectedCells(); if (cells.length) graph.removeCells(cells) })
    graph.bindKey('ctrl+c', () => { const cells = graph.getSelectedCells(); if (cells.length) graph.copy(cells) })
    graph.bindKey('ctrl+v', () => { if (!graph.isClipboardEmpty()) { const cells = graph.paste({ offset: 32 }); graph.select(cells) } })
    graph.bindKey('ctrl+z', () => graph.undo())
    graph.bindKey('ctrl+shift+z', () => graph.redo())
  }

  return graph
}

export function zoomGraph(graph: Graph, factor: number) {
  graph.zoom(graph.zoom() * factor)
}

export function fitGraph(graph: Graph) {
  graph.zoomToFit({ padding: 50 })
}
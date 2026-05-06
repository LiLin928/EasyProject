<!-- src/components/ant_workflow/DagDesigner/index.vue -->
<template>
  <div class="dag-designer">
    <!-- 工具栏 -->
    <Toolbar
      :can-undo="canUndo"
      :can-redo="canRedo"
      :workflow-name="workflowName"
      :workflow-status="workflowStatus"
      :readonly="readonly"
      @back="handleBack"
      @undo="handleUndo"
      @redo="handleRedo"
      @auto-layout="handleAutoLayout"
      @save="handleSave"
      @publish="handlePublish"
      @preview="handlePreview"
    />

    <!-- 三栏布局 -->
    <div class="designer-body">
      <!-- 左侧：节点库 -->
      <NodePalette
        v-if="!readonly"
        :collapsed="paletteCollapsed"
        @toggle="paletteCollapsed = !paletteCollapsed"
        @drag-node="handleDragNode"
      />

      <!-- 中间：X6 画布 -->
      <Canvas
        ref="canvasRef"
        :readonly="readonly"
        @node-click="handleNodeClick"
        @node-added="handleNodeAdded"
        @node-deleted="handleNodeDeleted"
        @edge-connected="handleEdgeConnected"
        @edge-deleted="handleEdgeDeleted"
        @selection-change="handleSelectionChange"
      />

      <!-- 右侧：属性面板（仅在选中节点时显示） -->
      <PropertyPanel
        v-if="selectedNode"
        :node="selectedNode"
        :graph="canvasRef?.getGraph() ?? null"
        @update="handlePropertyUpdate"
        @close="selectedNode = null"
      />
    </div>

    <!-- 预览对话框 -->
    <el-dialog
      v-model="previewDialogVisible"
      title="流程配置预览"
      width="800px"
      top="5vh"
      destroy-on-close
    >
      <div class="preview-content">
        <pre class="json-preview">{{ previewJsonData }}</pre>
      </div>
      <template #footer>
        <el-button @click="previewDialogVisible = false">关闭</el-button>
        <el-button type="primary" @click="handleCopyJson">复制 JSON</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick, provide } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import Toolbar from './Toolbar.vue'
import Canvas from './Canvas.vue'
import NodePalette from './NodePalette.vue'
import PropertyPanel from './PropertyPanel.vue'
import { exportDagFromGraph, loadDagToGraph } from './utils/graphConfig'
import { useLocale } from '@/composables/useLocale'
import { WorkflowStatus } from '@/types/antWorkflow'
import type { DagNode, DagEdge, DagConfig } from '@/stores/antWorkflowStore'
import type { Graph } from '@antv/x6'

const { t } = useLocale()

// Props
const props = withDefaults(defineProps<{
  workflowId?: string
  workflowName?: string
  workflowStatus?: WorkflowStatus
  initialDagConfig?: DagConfig | null
  readonly?: boolean
}>(), {
  workflowName: '',
  workflowStatus: WorkflowStatus.DRAFT,
  readonly: false,
})

// Emits
const emit = defineEmits<{
  (e: 'back'): void
  (e: 'save', config: DagConfig): void
  (e: 'publish', config: DagConfig): void
  (e: 'update:config', config: DagConfig): void
}>()

// Refs
const canvasRef = ref<{ getGraph: () => Graph; getNodes: () => DagNode[]; getEdges: () => DagEdge[]; updateNodeData: (id: string, data: any) => void }>()
const selectedNode = ref<DagNode | null>(null)
const paletteCollapsed = ref(false)

// 预览对话框
const previewDialogVisible = ref(false)
const previewJsonData = ref('')

// 历史记录
const history = ref<string[]>([])
const historyIndex = ref(-1)

const canUndo = computed(() => historyIndex.value > 0)
const canRedo = computed(() => historyIndex.value < history.value.length - 1)

// ========== 历史记录 ==========

const pushHistory = () => {
  const graph = canvasRef.value?.getGraph()
  if (!graph) return

  const dagConfig = exportDagFromGraph(graph)
  const jsonStr = JSON.stringify(dagConfig)

  if (historyIndex.value < history.value.length - 1) {
    history.value = history.value.slice(0, historyIndex.value + 1)
  }

  history.value.push(jsonStr)
  historyIndex.value = history.value.length - 1
}

const handleUndo = () => {
  if (!canUndo.value) return
  historyIndex.value--
  restoreFromHistory()
}

const handleRedo = () => {
  if (!canRedo.value) return
  historyIndex.value++
  restoreFromHistory()
}

const restoreFromHistory = () => {
  const dagConfig = JSON.parse(history.value[historyIndex.value])
  const graph = canvasRef.value?.getGraph()
  if (graph) {
    loadDagToGraph(graph, dagConfig, props.readonly)
  }
  emit('update:config', dagConfig)
}

// 提供历史记录给子组件
provide('dagHistory', {
  canUndo,
  canRedo,
  pushHistory,
})

// ========== 工具栏操作 ==========

const handleBack = () => {
  emit('back')
}

const handleAutoLayout = () => {
  // TODO: 实现自动布局（使用 dagre）
  ElMessage.success('自动布局完成')
}

const handleSave = () => {
  const graph = canvasRef.value?.getGraph()
  if (!graph) return

  const dagConfig = exportDagFromGraph(graph)
  emit('save', dagConfig)
  ElMessage.success(t('antWorkflow.messages.updateSuccess'))
}

const handlePublish = async () => {
  const graph = canvasRef.value?.getGraph()
  if (!graph) return

  try {
    await ElMessageBox.confirm(
      t('antWorkflow.messages.publishConfirm'),
      t('antWorkflow.publish'),
      { type: 'warning' }
    )

    const dagConfig = exportDagFromGraph(graph)
    emit('publish', dagConfig)
    ElMessage.success(t('antWorkflow.messages.publishSuccess'))
  } catch {
    // 用户取消
  }
}

const handlePreview = () => {
  const graph = canvasRef.value?.getGraph()
  if (!graph) return

  const dagConfig = exportDagFromGraph(graph)
  // 格式化 JSON 显示（2空格缩进）
  previewJsonData.value = JSON.stringify(dagConfig, null, 2)
  previewDialogVisible.value = true
}

const handleCopyJson = async () => {
  try {
    await navigator.clipboard.writeText(previewJsonData.value)
    ElMessage.success('已复制到剪贴板')
  } catch {
    ElMessage.error('复制失败')
  }
}

// ========== 节点操作 ==========

const handleNodeClick = (node: DagNode) => {
  selectedNode.value = node
}

const handleNodeAdded = () => {
  pushHistory()
}

const handleNodeDeleted = () => {
  selectedNode.value = null
}

const handleEdgeConnected = () => {
  pushHistory()
}

const handleEdgeDeleted = () => {
  pushHistory()
}

const handleSelectionChange = (nodes: DagNode[]) => {
  if (nodes.length === 1) {
    selectedNode.value = nodes[0]
  } else {
    selectedNode.value = null
  }
}

const handlePropertyUpdate = (node: DagNode) => {
  if (canvasRef.value) {
    canvasRef.value.updateNodeData(node.id, {
      name: node.name,
      config: node.config,
    })
    selectedNode.value = node
    pushHistory()
  }
}

const handleDragNode = () => {
  // 拖拽开始，无需处理
}

// ========== 生命周期 ==========

// 监听 initialDagConfig 变化，等待 graph 初始化后加载
watch(
  () => props.initialDagConfig,
  (config) => {
    if (config) {
      // 使用 nextTick 确保 DOM 更新完成，然后再尝试获取 graph
      nextTick(() => {
        const graph = canvasRef.value?.getGraph()
        if (graph) {
          loadDagToGraph(graph, config, props.readonly)
          pushHistory()
        } else {
          // graph 还没初始化，延迟重试
          const retryInterval = setInterval(() => {
            const g = canvasRef.value?.getGraph()
            if (g) {
              clearInterval(retryInterval)
              loadDagToGraph(g, config, props.readonly)
              pushHistory()
            }
          }, 50)
          // 最多重试 1 秒
          setTimeout(() => clearInterval(retryInterval), 1000)
        }
      })
    }
  },
  { immediate: true }
)

// ========== 暴露方法 ==========

defineExpose({
  getDagConfig: () => {
    const graph = canvasRef.value?.getGraph()
    if (!graph) return null
    return exportDagFromGraph(graph)
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
  flex: 1;
  display: flex;
  overflow: hidden;
  min-height: 0;
}

// 预览对话框样式
.preview-content {
  max-height: 60vh;
  overflow: auto;
}

.json-preview {
  background: #1e1e1e;
  color: #d4d4d4;
  padding: 16px;
  border-radius: 8px;
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
  font-size: 13px;
  line-height: 1.5;
  white-space: pre-wrap;
  word-wrap: break-word;
  margin: 0;
  overflow-x: auto;
}
</style>
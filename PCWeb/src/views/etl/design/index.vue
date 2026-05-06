<!-- src/views/etl/design/index.vue -->
<template>
  <div class="etl-design-page">
    <DagDesigner
      ref="designerRef"
      :pipeline-id="pipelineId"
      :pipeline-name="pipeline?.name"
      :pipeline-status="pipeline?.status"
      :initial-dag-config="initialDagConfig"
      :readonly="readonly"
      @back="handleBack"
      @save="handleSave"
      @publish="handlePublish"
      @run="handleRun"
      @node-select="handleNodeSelect"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import DagDesigner from '@/components/etl/EtlDagDesigner.vue'
import { getPipelineDetail, updatePipeline, publishPipeline, executePipeline } from '@/api/etl/pipelineApi'
import { PipelineStatus } from '@/types/etl'
import type { Pipeline, DagConfig, DagNode } from '@/types/etl'

const router = useRouter()
const route = useRoute()

// Props from route query
const pipelineId = computed(() => route.query.id as string)
const readonly = computed(() => route.query.readonly === 'true')

// State
const designerRef = ref<{ getDagConfig: () => DagConfig | null; setDagConfig: (config: DagConfig) => void; validate: () => any[] }>()
const pipeline = ref<Pipeline | null>(null)
const initialDagConfig = ref<DagConfig | null>(null)
const loading = ref(false)

// Load pipeline data
onMounted(async () => {
  if (!pipelineId.value) {
    ElMessage.warning('缺少任务流ID')
    handleBack()
    return
  }

  loading.value = true
  try {
    const data = await getPipelineDetail(pipelineId.value)

    // 解析 dagConfig JSON 字符串
    let dagConfig = data.dagConfig
    if (typeof dagConfig === 'string' && dagConfig) {
      try {
        dagConfig = JSON.parse(dagConfig)
      } catch (e) {
        console.error('解析 DAG 配置失败', e)
        dagConfig = { nodes: [], edges: [] }
      }
    }

    pipeline.value = {
      ...data,
      dagConfig: dagConfig || { nodes: [], edges: [] }
    } as Pipeline

    // 设置初始配置，让子组件通过 props 接收
    initialDagConfig.value = pipeline.value.dagConfig as DagConfig
  } catch (error) {
    ElMessage.error('加载任务流失败')
    handleBack()
  } finally {
    loading.value = false
  }
})

// Handlers
const handleBack = () => {
  router.push('/etl/pipeline')
}

const handleSave = async (dagConfig: DagConfig) => {
  if (!pipelineId.value) return

  try {
    await updatePipeline({
      id: pipelineId.value,
      dagConfig,
    })
    ElMessage.success('保存成功')
  } catch (error) {
    ElMessage.error('保存失败')
  }
}

const handlePublish = async (dagConfig: DagConfig) => {
  if (!pipelineId.value) return

  // 先保存
  await handleSave(dagConfig)

  try {
    await ElMessageBox.confirm(
      '发布后任务流将变为已发布状态，可以添加调度。确定要发布吗？',
      '发布确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'info',
      }
    )

    await publishPipeline(pipelineId.value)
    ElMessage.success('发布成功')

    // 重新加载任务流数据
    const data = await getPipelineDetail(pipelineId.value)
    pipeline.value = data
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleRun = async (dagConfig: DagConfig) => {
  if (!pipelineId.value) return

  // 先保存
  await handleSave(dagConfig)

  try {
    const executionId = await executePipeline(pipelineId.value)
    ElMessage.success(`执行已触发，执行ID: ${executionId}`)
    router.push(`/etl/monitor?executionId=${executionId}`)
  } catch (error) {
    ElMessage.error('执行失败')
  }
}

const handleNodeSelect = (node: DagNode) => {
  console.log('Selected node:', node)
}
</script>

<style scoped lang="scss">
.etl-design-page {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 1000;
  background: #f5f7fa;
}
</style>
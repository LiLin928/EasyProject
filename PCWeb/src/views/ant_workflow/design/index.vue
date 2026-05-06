<!-- src/views/ant_workflow/design/index.vue -->
<template>
  <div class="workflow-design-page">
    <DagDesigner
      :workflow-id="workflowId"
      :workflow-name="workflowName"
      :workflow-status="workflowStatus"
      :initial-dag-config="dagConfig"
      :readonly="readonly"
      @back="handleBack"
      @save="handleSave"
      @publish="handlePublish"
      @update:config="handleConfigUpdate"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import DagDesigner from '@/components/ant_workflow/AntDagDesigner.vue'
import { useLocale } from '@/composables/useLocale'
import { WorkflowStatus } from '@/types/antWorkflow'
import { getWorkflowDetail, createWorkflow, updateWorkflow, publishWorkflow } from '@/api/ant_workflow/workflowApi'
import type { DagConfig } from '@/stores/antWorkflowStore'

const { t } = useLocale()
const router = useRouter()
const route = useRoute()

const workflowId = ref<string | undefined>(route.params.id as string)
const workflowName = ref('')
const workflowStatus = ref<WorkflowStatus>(WorkflowStatus.DRAFT)
const dagConfig = ref<DagConfig | null>(null)
const readonly = ref(false)

onMounted(async () => {
  if (workflowId.value) {
    try {
      const res = await getWorkflowDetail(workflowId.value)
      workflowName.value = res.name
      workflowStatus.value = res.status
      dagConfig.value = res.flowConfig ? JSON.parse(res.flowConfig) : null
    } catch (e) { ElMessage.error(t('antWorkflow.errors.loadDetailFailed')) }
  }
})

const handleBack = () => router.push('/ant_workflow/list')
const handleSave = async (config: DagConfig) => {
  if (workflowId.value) {
    // 更新流程 - 必须包含 name（后端必填字段）
    await updateWorkflow({
      id: workflowId.value,
      name: workflowName.value || '未命名流程',
      flowConfig: JSON.stringify(config)
    })
    ElMessage.success(t('antWorkflow.messages.updateSuccess'))
  } else {
    // 创建新流程（createWorkflow 返回的是 GUID 字符串）
    const newWorkflowId = await createWorkflow({ name: workflowName.value || '新流程', code: 'wf-' + Date.now() })
    workflowId.value = newWorkflowId
    await updateWorkflow({
      id: newWorkflowId,
      name: workflowName.value || '新流程',
      flowConfig: JSON.stringify(config)
    })
    ElMessage.success(t('antWorkflow.messages.createSuccess'))
  }
}
const handlePublish = async (config: DagConfig) => {
  if (workflowId.value) { await publishWorkflow(workflowId.value); workflowStatus.value = WorkflowStatus.PUBLISHED; ElMessage.success(t('antWorkflow.messages.publishSuccess')) }
}
const handleConfigUpdate = (config: DagConfig) => { dagConfig.value = config }
</script>

<style scoped lang="scss">
.workflow-design-page {
  width: 100%;
  height: 100%;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  overflow: hidden;
  z-index: 999; // 最高层级，覆盖所有内容包括导航栏
}
</style>
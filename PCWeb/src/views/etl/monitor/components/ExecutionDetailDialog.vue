<!-- src/views/etl/monitor/components/ExecutionDetailDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="800px"
    :close-on-click-modal="false"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('etl.monitor.detail.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="visible = false">
            {{ t('etl.monitor.detail.close') }}
          </el-button>
        </div>
      </div>
    </template>
    <el-descriptions v-if="execution" :column="2" border>
      <el-descriptions-item :label="t('etl.monitor.detail.executionNo')">
        {{ execution.executionNo }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.pipelineName')">
        {{ execution.pipelineName }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.status')">
        <el-tag :type="getStatusTagType(execution.status)">
          {{ getStatusText(execution.status) }}
        </el-tag>
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.triggerType')">
        <el-tag type="info" effect="plain">
          {{ getTriggerTypeText(execution.triggerType) }}
        </el-tag>
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.startTime')">
        {{ execution.startTime }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.endTime')">
        {{ execution.endTime || '-' }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.duration')">
        {{ formatDuration(execution.duration) }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.progress')">
        <el-progress
          :percentage="execution.progress || 0"
          :status="execution.status === 'success' ? 'success' : execution.status === 'failure' ? 'exception' : undefined"
        />
      </el-descriptions-item>
    </el-descriptions>

    <!-- Node Execution List -->
    <div class="node-executions" v-if="execution?.nodes?.length">
      <h4>{{ t('etl.monitor.detail.nodeExecutions') }}</h4>
      <el-table :data="execution.nodes" border>
        <el-table-column prop="nodeName" :label="t('etl.monitor.detail.nodeName')" min-width="150" />
        <el-table-column prop="nodeType" :label="t('etl.monitor.detail.nodeType')" width="120" />
        <el-table-column prop="status" :label="t('etl.monitor.detail.nodeStatus')" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getNodeStatusTagType(row.status)" size="small">
              {{ row.status }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="startTime" :label="t('etl.monitor.detail.nodeStartTime')" width="160" />
        <el-table-column prop="duration" :label="t('etl.monitor.detail.nodeDuration')" width="100">
          <template #default="{ row }">
            {{ formatDuration(row.duration) }}
          </template>
        </el-table-column>
        <el-table-column :label="t('etl.monitor.detail.nodeOperation')" width="80">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleNodeDetail(row)">
              {{ t('etl.monitor.detail.nodeView') }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

      </el-dialog>

  <!-- Node Detail Dialog -->
  <el-dialog
    v-model="nodeDetailVisible"
    :title="t('etl.monitor.detail.nodeDetailTitle')"
    width="600px"
  >
    <el-descriptions v-if="nodeDetail" :column="1" border>
      <el-descriptions-item :label="t('etl.monitor.detail.nodeName')">
        {{ nodeDetail.nodeName }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.nodeStatus')">
        <el-tag :type="getNodeStatusTagType(nodeDetail.status)" size="small">
          {{ nodeDetail.status }}
        </el-tag>
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.nodeStartTime')">
        {{ nodeDetail.startTime }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.nodeEndTime')">
        {{ nodeDetail.endTime || '-' }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.nodeDuration')">
        {{ formatDuration(nodeDetail.duration) }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.nodeInput')">
        <pre class="code-block">{{ JSON.stringify(nodeDetail.input, null, 2) }}</pre>
      </el-descriptions-item>
      <el-descriptions-item :label="t('etl.monitor.detail.nodeOutput')">
        <pre class="code-block">{{ JSON.stringify(nodeDetail.output, null, 2) }}</pre>
      </el-descriptions-item>
      <el-descriptions-item v-if="nodeDetail.error" :label="t('etl.monitor.detail.nodeError')">
        <el-alert type="error" :closable="false">
          {{ nodeDetail.error }}
        </el-alert>
      </el-descriptions-item>
    </el-descriptions>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { getExecutionDetail, getNodeExecutionDetail, getNodeExecutions } from '@/api/etl/monitorApi'
import { useLocale } from '@/composables/useLocale'
import type { Execution, ExecutionStatus, TriggerType } from '@/types/etl'
import { ExecutionStatus as ES, TriggerType as TT } from '@/types/etl'

const props = defineProps<{
  modelValue: boolean
  executionId?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const execution = ref<Execution | null>(null)
const nodeDetailVisible = ref(false)
const nodeDetail = ref<any>(null)

// Watch dialog open
watch(visible, (val) => {
  if (val && props.executionId) {
    loadExecutionDetail()
  }
})

const loadExecutionDetail = async () => {
  if (!props.executionId) return
  try {
    const data = await getExecutionDetail(props.executionId)
    execution.value = data
    // 加载节点执行列表
    const nodes = await getNodeExecutions(props.executionId)
    execution.value = { ...data, nodes }
  } catch (error) {
    // Error handled by interceptor
  }
}

const handleNodeDetail = async (node: any) => {
  if (!props.executionId) return
  try {
    const data = await getNodeExecutionDetail(props.executionId, node.nodeId)
    nodeDetail.value = data
    nodeDetailVisible.value = true
  } catch (error) {
    // Error handled by interceptor
  }
}

const getStatusTagType = (status: ExecutionStatus) => {
  switch (status) {
    case ES.RUNNING:
      return 'primary'
    case ES.SUCCESS:
      return 'success'
    case ES.FAILURE:
      return 'danger'
    case ES.CANCELLED:
      return 'warning'
    case ES.PENDING:
      return 'info'
    default:
      return 'info'
  }
}

const getStatusText = (status: ExecutionStatus) => {
  switch (status) {
    case ES.RUNNING:
      return t('etl.monitor.list.statusRunning')
    case ES.SUCCESS:
      return t('etl.monitor.list.statusSuccess')
    case ES.FAILURE:
      return t('etl.monitor.list.statusFailure')
    case ES.CANCELLED:
      return t('etl.monitor.list.statusCancelled')
    case ES.PENDING:
      return t('etl.monitor.list.statusPending')
    default:
      return '-'
  }
}

const getTriggerTypeText = (triggerType: TriggerType) => {
  switch (triggerType) {
    case TT.MANUAL:
      return t('etl.monitor.list.triggerManual')
    case TT.SCHEDULE:
      return t('etl.monitor.list.triggerSchedule')
    case TT.API:
      return t('etl.monitor.list.triggerApi')
    default:
      return '-'
  }
}

const getNodeStatusTagType = (status: string) => {
  switch (status) {
    case 'running':
      return 'primary'
    case 'success':
      return 'success'
    case 'failure':
      return 'danger'
    case 'skipped':
      return 'info'
    default:
      return 'info'
  }
}

const formatDuration = (duration?: number) => {
  if (!duration) return '-'
  if (duration < 1000) return `${duration}ms`
  if (duration < 60000) return `${(duration / 1000).toFixed(1)}s`
  if (duration < 3600000) return `${(duration / 60000).toFixed(1)}m`
  return `${(duration / 3600000).toFixed(1)}h`
}
</script>

<style scoped lang="scss">
.dialog-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;

  .dialog-title {
    font-size: 18px;
    font-weight: 500;
    color: #303133;
  }

  .dialog-actions {
    display: flex;
    gap: 8px;
    margin-right: 30px;
  }
}

.node-executions {
  margin-top: 20px;

  h4 {
    margin-bottom: 12px;
    font-weight: 600;
  }
}

.code-block {
  background: #f5f5f5;
  padding: 8px;
  border-radius: 4px;
  font-size: 12px;
  max-height: 200px;
  overflow: auto;
}
</style>
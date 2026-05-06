<!-- PCWeb/src/views/ops/components/TaskLogDetailDialog.vue -->
<template>
  <el-dialog
    v-model="dialogVisible"
    :title="t('ops.taskLog.detail.title')"
    width="700px"
    destroy-on-close
  >
    <div v-if="loading" class="loading-container">
      <el-icon class="is-loading" :size="32"><Loading /></el-icon>
    </div>

    <template v-else-if="logDetail">
      <el-descriptions :column="2" border>
        <el-descriptions-item :label="t('ops.taskLog.table.startTime')">
          {{ formatTime(logDetail.startTime) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.taskLog.table.status')">
          <el-tag :type="logDetail.status === 1 ? 'success' : 'danger'">{{ logDetail.statusText }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.taskLog.table.jobName')">{{ logDetail.jobName }}</el-descriptions-item>
        <el-descriptions-item :label="t('ops.taskLog.table.duration')">
          {{ logDetail.duration ? `${logDetail.duration}ms` : '-' }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.taskLog.table.triggerType')">
          {{ logDetail.triggerType === 0 ? t('ops.taskLog.triggerType.cron') : t('ops.taskLog.triggerType.manual') }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.taskLog.detail.endTime')">
          {{ formatTime(logDetail.endTime) }}
        </el-descriptions-item>
      </el-descriptions>

      <div v-if="logDetail.resultMessage" class="detail-section">
        <h4>{{ t('ops.taskLog.detail.resultMessage') }}</h4>
        <pre class="result-content">{{ logDetail.resultMessage }}</pre>
      </div>

      <div v-if="logDetail.exceptionMessage" class="detail-section">
        <h4>{{ t('ops.taskLog.detail.exceptionMessage') }}</h4>
        <pre class="error-content">{{ logDetail.exceptionMessage }}</pre>
      </div>

      <div v-if="logDetail.exceptionStackTrace" class="detail-section">
        <h4>{{ t('ops.taskLog.detail.exceptionStackTrace') }}</h4>
        <pre class="stack-content">{{ logDetail.exceptionStackTrace }}</pre>
      </div>
    </template>

    <template #footer>
      <el-button @click="handleClose">{{ t('common.button.close') }}</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Loading } from '@element-plus/icons-vue'
import { getTaskLogDetail } from '@/api/ops/taskApi'
import { useLocale } from '@/composables/useLocale'
import type { TaskExecutionLog } from '@/types/task'

const props = defineProps<{
  modelValue: boolean
  logId: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>()

const { t } = useLocale()

const dialogVisible = ref(false)
const loading = ref(false)
const logDetail = ref<TaskExecutionLog | null>(null)

watch(() => props.modelValue, (val) => {
  dialogVisible.value = val
  if (val && props.logId) loadDetail()
})

watch(dialogVisible, (val) => emit('update:modelValue', val))

const loadDetail = async () => {
  loading.value = true
  try {
    logDetail.value = await getTaskLogDetail(props.logId)
  } catch (error) {
    logDetail.value = null
  } finally {
    loading.value = false
  }
}

const formatTime = (time?: string) => time ? time.replace('T', ' ').substring(0, 19) : '-'

const handleClose = () => dialogVisible.value = false
</script>

<style scoped lang="scss">
.loading-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 200px;
}

.detail-section {
  margin-top: 16px;
  h4 { margin-bottom: 8px; font-weight: 600; }
  pre {
    padding: 12px;
    background: #f5f7fa;
    border-radius: 4px;
    overflow-x: auto;
    white-space: pre-wrap;
    word-break: break-word;
    font-family: 'Consolas', monospace;
    font-size: 13px;
  }
  .result-content { color: #606266; }
  .error-content, .stack-content { color: #f56c6c; }
}
</style>
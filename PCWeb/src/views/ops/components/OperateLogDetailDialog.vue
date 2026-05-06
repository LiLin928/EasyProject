<!-- src/views/ops/components/OperateLogDetailDialog.vue -->
<template>
  <el-dialog
    v-model="dialogVisible"
    :title="t('ops.operateLog.detailTitle')"
    width="800px"
    :close-on-click-modal="false"
    destroy-on-close
  >
    <div v-if="loading" class="loading-container">
      <el-icon class="is-loading" :size="32"><Loading /></el-icon>
      <span>{{ t('common.loading') }}</span>
    </div>

    <div v-else-if="!logDetail" class="error-container">
      <el-icon :size="32"><WarningFilled /></el-icon>
      <span>{{ t('ops.operateLog.notFound') }}</span>
    </div>

    <template v-else-if="logDetail">
      <el-descriptions :column="2" border>
        <el-descriptions-item :label="t('ops.operateLog.detail.createTime')">
          {{ formatTime(logDetail.createTime) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.operateLog.detail.status')">
          <el-tag :type="logDetail.status === 1 ? 'success' : 'danger'" size="small">
            {{ logDetail.status === 1 ? t('ops.operateLog.status.success') : t('ops.operateLog.status.failure') }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.operateLog.detail.userName')">
          {{ logDetail.userName || '-' }}
          <span v-if="logDetail.userId" class="user-id">({{ logDetail.userId }})</span>
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.operateLog.detail.moduleName')">
          {{ logDetail.module || '-' }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.operateLog.detail.actionName')">
          {{ logDetail.action || '-' }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.operateLog.detail.method')">
          <el-tag v-if="logDetail.method" :type="getMethodType(logDetail.method)" size="small">
            {{ logDetail.method }}
          </el-tag>
          <span v-else>-</span>
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.operateLog.detail.url')" :span="2">
          {{ logDetail.url || '-' }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.operateLog.detail.ip')">
          {{ logDetail.ip || '-' }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.operateLog.detail.location')">
          {{ logDetail.location || '-' }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('ops.operateLog.detail.duration')">
          <span :class="getDurationClass(logDetail.duration)">
            {{ logDetail.duration ? `${logDetail.duration}ms` : '-' }}
          </span>
        </el-descriptions-item>
      </el-descriptions>

      <!-- Request Params Section -->
      <div v-if="logDetail.params" class="detail-section">
        <h4>{{ t('ops.operateLog.detail.params') }}</h4>
        <pre class="params-content">{{ formatJson(logDetail.params) }}</pre>
      </div>

      <!-- Result Section -->
      <div v-if="logDetail.result" class="detail-section">
        <h4>{{ t('ops.operateLog.detail.result') }}</h4>
        <pre class="result-content">{{ formatJson(logDetail.result) }}</pre>
      </div>

      <!-- Error Message Section -->
      <div v-if="logDetail.errorMsg" class="detail-section">
        <h4>{{ t('ops.operateLog.detail.errorMsg') }}</h4>
        <pre class="error-content">{{ logDetail.errorMsg }}</pre>
      </div>
    </template>

    <template #footer>
      <el-button @click="handleClose">{{ t('common.button.close') }}</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Loading, WarningFilled } from '@element-plus/icons-vue'
import { getOperateLogDetail } from '@/api/ops/operateLogApi'
import { useLocale } from '@/composables/useLocale'
import type { OperateLog } from '@/types/operateLog'

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
const logDetail = ref<OperateLog | null>(null)

// Watch visible prop to sync dialogVisible and trigger loadLogDetail
watch(
  () => props.modelValue,
  (val) => {
    dialogVisible.value = val
    if (val && props.logId) {
      loadLogDetail()
    }
  }
)

// Watch dialogVisible to emit update
watch(dialogVisible, (val) => {
  emit('update:modelValue', val)
})

const loadLogDetail = async () => {
  if (!props.logId) return

  loading.value = true
  try {
    const data = await getOperateLogDetail(props.logId)
    logDetail.value = data
  } catch (error) {
    // Error handled by interceptor
    logDetail.value = null
  } finally {
    loading.value = false
  }
}

const formatTime = (time?: string) => {
  if (!time) return '-'
  return time.replace('T', ' ').substring(0, 19)
}

const formatJson = (json?: string) => {
  if (!json) return '-'
  try {
    const obj = JSON.parse(json)
    return JSON.stringify(obj, null, 2)
  } catch {
    return json
  }
}

const getMethodType = (method: string): 'success' | 'warning' | 'info' | 'danger' => {
  const types: Record<string, 'success' | 'warning' | 'info' | 'danger'> = {
    GET: 'success',
    POST: 'warning',
    PUT: 'info',
    DELETE: 'danger',
    PATCH: 'info',
  }
  return types[method.toUpperCase()] || 'info'
}

const getDurationClass = (duration?: number) => {
  if (!duration) return ''
  if (duration > 3000) return 'duration-slow'
  if (duration > 1000) return 'duration-medium'
  return 'duration-fast'
}

const handleClose = () => {
  dialogVisible.value = false
}
</script>

<style scoped lang="scss">
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 200px;
  color: #909399;

  .el-icon {
    margin-bottom: 12px;
  }
}

.error-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 200px;
  color: #f56c6c;

  .el-icon {
    margin-bottom: 12px;
  }
}

.user-id {
  color: #909399;
  font-size: 12px;
}

.detail-section {
  margin-top: 16px;

  h4 {
    margin-bottom: 8px;
    font-weight: 600;
    color: #303133;
  }

  pre {
    padding: 12px;
    background: #f5f7fa;
    border-radius: 4px;
    overflow-x: auto;
    white-space: pre-wrap;
    word-break: break-word;
    margin: 0;
    font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
    font-size: 13px;
    line-height: 1.5;
  }

  .params-content {
    color: #606266;
  }

  .result-content {
    color: #303133;
  }

  .error-content {
    color: #f56c6c;
  }
}

.duration-fast {
  color: #67c23a;
}

.duration-medium {
  color: #e6a23c;
}

.duration-slow {
  color: #f56c6c;
  font-weight: 500;
}
</style>
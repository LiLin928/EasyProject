<!-- src/views/ops/components/LogDetailDialog.vue -->
<template>
  <el-dialog
    v-model="dialogVisible"
    title="日志详情"
    width="800px"
    :close-on-click-modal="false"
    destroy-on-close
  >
    <div v-if="loading" class="loading-container">
      <el-icon class="is-loading" :size="32"><Loading /></el-icon>
      <span>加载中...</span>
    </div>

    <div v-else-if="!logDetail" class="error-container">
      <el-icon :size="32"><WarningFilled /></el-icon>
      <span>日志不存在或已被清理</span>
    </div>

    <template v-else-if="logDetail">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="时间戳">
          {{ logDetail.timestamp }}
        </el-descriptions-item>
        <el-descriptions-item label="级别">
          <el-tag :type="getLevelType(logDetail.level)" size="small">
            {{ logDetail.level }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="请求路径" :span="2">
          {{ logDetail.requestPath || '-' }}
        </el-descriptions-item>
        <el-descriptions-item label="请求方法">
          {{ logDetail.method || '-' }}
        </el-descriptions-item>
        <el-descriptions-item label="用户">
          {{ logDetail.userName ? `${logDetail.userName} (${logDetail.userId})` : '-' }}
        </el-descriptions-item>
        <el-descriptions-item label="IP 地址">
          {{ logDetail.ipAddress || '-' }}
        </el-descriptions-item>
        <el-descriptions-item label="执行时长">
          <span :class="getDurationClass(logDetail.duration)">
            {{ logDetail.duration ? `${logDetail.duration}ms` : '-' }}
          </span>
        </el-descriptions-item>
        <el-descriptions-item label="机器名">
          {{ logDetail.machineName || '-' }}
        </el-descriptions-item>
        <el-descriptions-item label="环境">
          {{ logDetail.environment || '-' }}
        </el-descriptions-item>
      </el-descriptions>

      <!-- Message Section -->
      <div class="detail-section">
        <h4>消息内容</h4>
        <pre class="message-content">{{ logDetail.message }}</pre>
      </div>

      <!-- Exception Section -->
      <div v-if="logDetail.exception" class="detail-section">
        <h4>异常信息</h4>
        <pre class="exception-content">{{ logDetail.exception }}</pre>
      </div>

      <!-- Stack Trace Section -->
      <div v-if="logDetail.stackTrace" class="detail-section">
        <h4>堆栈跟踪</h4>
        <pre class="stack-content">{{ logDetail.stackTrace }}</pre>
      </div>

      <!-- Properties Section -->
      <div v-if="logDetail.properties && Object.keys(logDetail.properties).length > 0" class="detail-section">
        <h4>附加属性</h4>
        <pre class="properties-content">{{ formatProperties(logDetail.properties) }}</pre>
      </div>
    </template>

    <template #footer>
      <el-button @click="handleClose">关闭</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Loading, WarningFilled } from '@element-plus/icons-vue'
import { getLogDetail } from '@/api/ops/logQueryApi'
import type { LogDetail, LogLevel } from '@/types/logQuery'

const props = defineProps<{
  visible: boolean
  logId: string
  environment: string
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

const dialogVisible = ref(false)
const loading = ref(false)
const logDetail = ref<LogDetail | null>(null)

// Watch visible prop to sync dialogVisible and trigger loadLogDetail
watch(
  () => props.visible,
  (val) => {
    dialogVisible.value = val
    if (val && props.logId && props.environment) {
      loadLogDetail()
    }
  }
)

// Watch dialogVisible to emit update
watch(dialogVisible, (val) => {
  emit('update:visible', val)
})

const loadLogDetail = async () => {
  if (!props.logId || !props.environment) return

  loading.value = true
  try {
    const data = await getLogDetail(props.environment, props.logId)
    logDetail.value = data
  } catch (error) {
    // Error handled by interceptor
    logDetail.value = null
  } finally {
    loading.value = false
  }
}

const getLevelType = (level: LogLevel) => {
  switch (level) {
    case 'Debug':
      return 'info'
    case 'Information':
      return 'primary'
    case 'Warning':
      return 'warning'
    case 'Error':
      return 'danger'
    case 'Fatal':
      return 'danger'
    default:
      return 'info'
  }
}

const getDurationClass = (duration?: number) => {
  if (!duration) return ''
  if (duration > 3000) return 'duration-slow'
  if (duration > 1000) return 'duration-medium'
  return 'duration-fast'
}

const formatProperties = (properties: Record<string, unknown>) => {
  return JSON.stringify(properties, null, 2)
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

  .message-content {
    color: #303133;
  }

  .exception-content {
    color: #f56c6c;
  }

  .stack-content {
    color: #909399;
    font-size: 12px;
  }

  .properties-content {
    color: #606266;
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
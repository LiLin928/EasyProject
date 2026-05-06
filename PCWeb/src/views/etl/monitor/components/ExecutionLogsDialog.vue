<!-- src/views/etl/monitor/components/ExecutionLogsDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="800px"
    :close-on-click-modal="false"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('etl.monitor.logs.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="handleDownload">
            <el-icon><Download /></el-icon>
            {{ t('etl.monitor.logs.download') }}
          </el-button>
          <el-button @click="visible = false">
            {{ t('etl.monitor.logs.close') }}
          </el-button>
        </div>
      </div>
    </template>
    <!-- Log Filters -->
    <div class="log-filters">
      <el-select
        v-model="logLevel"
        :placeholder="t('etl.monitor.logs.levelPlaceholder')"
        style="width: 120px"
        clearable
      >
        <el-option label="INFO" value="info" />
        <el-option label="WARN" value="warn" />
        <el-option label="ERROR" value="error" />
        <el-option label="DEBUG" value="debug" />
      </el-select>
      <el-select
        v-model="nodeId"
        :placeholder="t('etl.monitor.logs.nodePlaceholder')"
        style="width: 200px"
        clearable
      >
        <el-option
          v-for="node in nodes"
          :key="node.id"
          :label="node.name"
          :value="node.id"
        />
      </el-select>
      <el-button @click="handleRefresh">
        <el-icon><Refresh /></el-icon>
        {{ t('etl.monitor.logs.refresh') }}
      </el-button>
    </div>

    <!-- Log List -->
    <div class="log-list" v-loading="loading">
      <div
        v-for="log in logs"
        :key="log.time + log.message"
        class="log-item"
        :class="log.level"
      >
        <span class="log-time">{{ log.time }}</span>
        <span class="log-level">{{ log.level.toUpperCase() }}</span>
        <span v-if="log.nodeName" class="log-node">[{{ log.nodeName }}]</span>
        <span class="log-message">{{ log.message }}</span>
      </div>
      <el-empty v-if="!logs.length" :description="t('etl.monitor.logs.empty')" />
    </div>

    </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Refresh, Download } from '@element-plus/icons-vue'
import { getExecutionLogs } from '@/api/etl/monitorApi'
import { useLocale } from '@/composables/useLocale'

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

const loading = ref(false)
const logs = ref<Array<{ time: string; level: string; message: string; nodeId?: string; nodeName?: string }>>([])
const logLevel = ref<string>('')
const nodeId = ref<string>('')
const nodes = ref<Array<{ id: string; name: string }>>([])

// Watch dialog open
watch(visible, (val) => {
  if (val && props.executionId) {
    loadLogs()
  }
})

// Watch filter changes
watch([logLevel, nodeId], () => {
  if (visible.value && props.executionId) {
    loadLogs()
  }
})

const loadLogs = async () => {
  if (!props.executionId) return
  loading.value = true
  try {
    const result = await getExecutionLogs(props.executionId, {
      level: logLevel.value,
      nodeId: nodeId.value,
    })
    logs.value = result.logs
    // Extract nodes from logs
    const nodeSet = new Set<string>()
    result.logs.forEach(log => {
      if (log.nodeId && log.nodeName) {
        nodeSet.add(`${log.nodeId}:${log.nodeName}`)
      }
    })
    nodes.value = Array.from(nodeSet).map(item => {
      const [id, name] = item.split(':')
      return { id, name }
    })
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleRefresh = () => {
  loadLogs()
}

const handleDownload = () => {
  if (!logs.value.length) return
  const content = logs.value.map(log => {
    const parts = [log.time, log.level.toUpperCase()]
    if (log.nodeName) parts.push(`[${log.nodeName}]`)
    parts.push(log.message)
    return parts.join(' ')
  }).join('\n')
  const blob = new Blob([content], { type: 'text/plain' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = `execution-${props.executionId}-logs.txt`
  link.click()
  URL.revokeObjectURL(url)
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

.log-filters {
  display: flex;
  gap: 12px;
  margin-bottom: 16px;
}

.log-list {
  background: #1e1e1e;
  border-radius: 4px;
  padding: 12px;
  max-height: 400px;
  overflow: auto;

  .log-item {
    font-size: 12px;
    color: #d4d4d4;
    line-height: 1.6;
    margin-bottom: 4px;

    .log-time {
      color: #6a9955;
    }

    .log-level {
      margin-left: 8px;
      padding: 2px 4px;
      border-radius: 2px;
      font-weight: 600;
    }

    .log-node {
      margin-left: 8px;
      color: #569cd6;
    }

    .log-message {
      margin-left: 8px;
    }

    &.info .log-level {
      background: #007acc;
      color: #fff;
    }

    &.warn .log-level {
      background: #ff9800;
      color: #fff;
    }

    &.error .log-level {
      background: #f44336;
      color: #fff;
    }

    &.debug .log-level {
      background: #607d8b;
      color: #fff;
    }
  }
}
</style>
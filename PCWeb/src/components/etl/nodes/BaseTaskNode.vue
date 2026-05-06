<!-- src/components/etl/nodes/BaseTaskNode.vue -->
<template>
  <div
    class="base-task-node"
    :class="{ 'is-selected': selected }"
    :style="nodeStyle"
  >
    <!-- 节点头部 -->
    <div class="node-header">
      <span class="node-icon">{{ icon }}</span>
      <span class="node-title">{{ name }}</span>
    </div>

    <!-- 节点描述 -->
    <div v-if="description" class="node-description">
      {{ description }}
    </div>

    <!-- 状态指示器 -->
    <div v-if="status" class="node-status">
      <span
        class="status-indicator"
        :class="statusClass"
      >
        {{ statusText }}
      </span>
    </div>

    <!-- 连接点（用于连线） -->
    <div class="node-ports">
      <div class="port port-in" data-port="in"></div>
      <div class="port port-out" data-port="out"></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { NodeStyleConfig } from '../utils/nodeRegistry'

// Props
const props = withDefaults(defineProps<{
  /** 节点名称 */
  name: string
  /** 节点图标 */
  icon: string
  /** 节点描述 */
  description?: string
  /** 节点样式配置 */
  styleConfig: NodeStyleConfig
  /** 是否选中 */
  selected?: boolean
  /** 运行状态 */
  status?: 'idle' | 'running' | 'success' | 'error' | 'skipped'
}>(), {
  selected: false,
})

// 计算节点样式
const nodeStyle = computed(() => ({
  backgroundColor: props.styleConfig.bgColor,
  borderColor: props.selected ? '#409EFF' : props.styleConfig.borderColor,
  color: props.styleConfig.textColor,
  width: `${props.styleConfig.width}px`,
  minHeight: `${props.styleConfig.height}px`,
}))

// 状态样式
const statusClass = computed(() => {
  if (!props.status) return ''
  const statusMap: Record<string, string> = {
    idle: 'status-idle',
    running: 'status-running',
    success: 'status-success',
    error: 'status-error',
    skipped: 'status-skipped',
  }
  return statusMap[props.status] || ''
})

// 状态文本
const statusText = computed(() => {
  if (!props.status) return ''
  const statusMap: Record<string, string> = {
    idle: '待执行',
    running: '执行中',
    success: '成功',
    error: '失败',
    skipped: '已跳过',
  }
  return statusMap[props.status] || ''
})
</script>

<style scoped lang="scss">
.base-task-node {
  position: relative;
  padding: 12px 16px;
  border-radius: 8px;
  border: 2px solid;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);

  &:hover {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.12);
    transform: translateY(-1px);
  }

  &.is-selected {
    border-color: #409EFF;
    box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.3);
  }
}

.node-header {
  display: flex;
  align-items: center;
  gap: 8px;

  .node-icon {
    font-size: 18px;
    line-height: 1;
  }

  .node-title {
    font-size: 14px;
    font-weight: 500;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
}

.node-description {
  margin-top: 4px;
  font-size: 12px;
  color: #909399;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.node-status {
  margin-top: 6px;

  .status-indicator {
    display: inline-block;
    padding: 2px 8px;
    font-size: 11px;
    border-radius: 4px;

    &.status-idle {
      background: #f5f7fa;
      color: #909399;
    }

    &.status-running {
      background: #e6f7ff;
      color: #1890ff;
    }

    &.status-success {
      background: #f6ffed;
      color: #52c41a;
    }

    &.status-error {
      background: #fff2f0;
      color: #ff4d4f;
    }

    &.status-skipped {
      background: #fffbe6;
      color: #faad14;
    }
  }
}

.node-ports {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  pointer-events: none;

  .port {
    position: absolute;
    width: 12px;
    height: 12px;
    border-radius: 50%;
    background: #fff;
    border: 2px solid #31d0c6;
    pointer-events: all;
    cursor: crosshair;

    &:hover {
      border-color: #409EFF;
      transform: scale(1.2);
    }
  }

  .port-in {
    top: 50%;
    left: -6px;
    transform: translateY(-50%);
  }

  .port-out {
    top: 50%;
    right: -6px;
    transform: translateY(-50%);
  }
}
</style>
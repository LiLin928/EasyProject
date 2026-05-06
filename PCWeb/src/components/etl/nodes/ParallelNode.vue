<!-- src/components/etl/nodes/ParallelNode.vue -->
<template>
  <div
    class="parallel-node"
    :class="{ 'is-selected': selected }"
    :style="nodeStyle"
  >
    <!-- 节点头部 -->
    <div class="node-header">
      <span class="node-icon">{{ styleConfig.icon }}</span>
      <span class="node-title">{{ name }}</span>
    </div>

    <!-- 分支列表 -->
    <div class="branch-list">
      <div
        v-for="branch in branches"
        :key="branch.id"
        class="branch-item"
      >
        <span class="branch-name">{{ branch.name }}</span>
      </div>
      <div v-if="branches.length === 0" class="branch-empty">
        点击配置并行分支
      </div>
    </div>

    <!-- 等待模式指示 -->
    <div class="wait-mode">
      {{ waitModeText }}
    </div>

    <!-- 多连接点 -->
    <div class="node-ports">
      <div class="port port-in" data-port="in"></div>
      <div
        v-for="(branch, index) in branches"
        :key="branch.id"
        class="port port-out"
        :data-port="branch.id"
        :style="{ top: `${60 + index * 24}px` }"
      ></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { TaskNodeType, type ParallelNodeConfig, type ParallelBranch } from '@/types/etl'
import { nodeStyleMap } from '../utils/nodeRegistry'

// Props
const props = withDefaults(defineProps<{
  /** 节点名称 */
  name: string
  /** 节点配置 */
  config?: ParallelNodeConfig
  /** 是否选中 */
  selected?: boolean
  /** 运行状态 */
  status?: 'idle' | 'running' | 'success' | 'error' | 'skipped'
}>(), {
  selected: false,
})

// 样式配置
const styleConfig = nodeStyleMap[TaskNodeType.PARALLEL]

// 计算节点样式
const nodeStyle = computed(() => ({
  backgroundColor: styleConfig.bgColor,
  borderColor: props.selected ? '#409EFF' : styleConfig.borderColor,
  color: styleConfig.textColor,
  width: `${styleConfig.width}px`,
  minHeight: `${styleConfig.height}px`,
}))

// 分支列表
const branches = computed<ParallelBranch[]>(() => {
  if (!props.config?.branches) return []
  return props.config.branches
})

// 等待模式文本
const waitModeText = computed(() => {
  const mode = props.config?.waitMode || 'all'
  return mode === 'all' ? '等待全部完成' : '任一完成即结束'
})
</script>

<style scoped lang="scss">
.parallel-node {
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

.branch-list {
  margin-top: 8px;
  display: flex;
  flex-direction: column;
  gap: 4px;

  .branch-item {
    display: flex;
    align-items: center;
    gap: 4px;
    font-size: 12px;
    padding: 4px 8px;
    background: rgba(255, 255, 255, 0.5);
    border-radius: 4px;
  }

  .branch-empty {
    font-size: 12px;
    color: #909399;
    text-align: center;
    padding: 8px;
  }
}

.wait-mode {
  margin-top: 8px;
  font-size: 11px;
  color: #909399;
  text-align: center;
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
    right: -6px;
    transform: translateY(-50%);
  }
}
</style>
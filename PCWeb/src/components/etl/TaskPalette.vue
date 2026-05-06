<!-- src/components/etl/DagDesigner/TaskPalette.vue -->
<template>
  <div class="task-palette" :class="{ collapsed: collapsed }">
    <el-card shadow="never" v-show="!collapsed">
      <template #header>
        <div class="palette-header">
          <span>任务节点库</span>
          <el-button link size="small" @click="handleToggle">
            <el-icon :size="16">
              <DArrowLeft />
            </el-icon>
          </el-button>
        </div>
      </template>

      <!-- 搜索框 -->
      <el-input
        v-model="searchKeyword"
        placeholder="搜索节点"
        clearable
        size="small"
        class="search-input"
      >
        <template #prefix>
          <el-icon><Search /></el-icon>
        </template>
      </el-input>

      <!-- 节点分组列表 -->
      <div class="node-groups">
        <div
          v-for="group in filteredGroups"
          :key="group.name"
          class="node-group"
        >
          <div class="group-title" @click="toggleGroup(group.name)">
            <el-icon :size="12" class="group-arrow">
              <ArrowDown v-if="!collapsedGroups.includes(group.name)" />
              <ArrowRight v-else />
            </el-icon>
            <span>{{ group.name }}</span>
          </div>
          <div class="group-items" v-show="!collapsedGroups.includes(group.name)">
            <div
              v-for="item in group.items"
              :key="item.type"
              class="node-item"
              draggable="true"
              @dragstart="handleDragStart(item.type, $event)"
            >
              <div
                class="node-icon"
                :style="{
                  backgroundColor: item.style.bgColor,
                  borderColor: item.style.borderColor,
                }"
              >
                <span class="icon-text">{{ item.style.icon }}</span>
              </div>
              <div class="node-info">
                <span class="node-label">{{ item.style.label }}</span>
                <span class="node-type">{{ item.type }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </el-card>

    <!-- 折叠状态下显示展开按钮 -->
    <div v-if="collapsed" class="collapsed-bar" @click="handleToggle">
      <el-icon :size="16">
        <DArrowRight />
      </el-icon>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Search, DArrowLeft, DArrowRight, ArrowDown, ArrowRight } from '@element-plus/icons-vue'
import { TaskNodeType } from '@/types/etl'
import { nodeStyleMap } from './utils/nodeRegistry'

// Props
const props = defineProps<{
  collapsed?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'toggle'): void
  (e: 'drag-node', nodeType: TaskNodeType, event: DragEvent): void
}>()

// 搜索关键词
const searchKeyword = ref('')

// 折叠的分组列表
const collapsedGroups = ref<string[]>([])

// 节点分组配置
const nodeGroups = [
  {
    name: '数据节点',
    items: [
      { type: TaskNodeType.DATASOURCE, style: nodeStyleMap[TaskNodeType.DATASOURCE] },
      { type: TaskNodeType.SQL, style: nodeStyleMap[TaskNodeType.SQL] },
      { type: TaskNodeType.FILE, style: nodeStyleMap[TaskNodeType.FILE] },
    ],
  },
  {
    name: '处理节点',
    items: [
      { type: TaskNodeType.TRANSFORM, style: nodeStyleMap[TaskNodeType.TRANSFORM] },
      { type: TaskNodeType.OUTPUT, style: nodeStyleMap[TaskNodeType.OUTPUT] },
      { type: TaskNodeType.SCRIPT, style: nodeStyleMap[TaskNodeType.SCRIPT] },
    ],
  },
  {
    name: '外部接口',
    items: [
      { type: TaskNodeType.API, style: nodeStyleMap[TaskNodeType.API] },
      { type: TaskNodeType.NOTIFICATION, style: nodeStyleMap[TaskNodeType.NOTIFICATION] },
    ],
  },
  {
    name: '控制节点',
    items: [
      { type: TaskNodeType.CONDITION, style: nodeStyleMap[TaskNodeType.CONDITION] },
      { type: TaskNodeType.PARALLEL, style: nodeStyleMap[TaskNodeType.PARALLEL] },
      { type: TaskNodeType.SUBFLOW, style: nodeStyleMap[TaskNodeType.SUBFLOW] },
    ],
  },
]

// 过滤后的分组
const filteredGroups = computed(() => {
  if (!searchKeyword.value) {
    return nodeGroups
  }

  const keyword = searchKeyword.value.toLowerCase()
  return nodeGroups.map(group => ({
    name: group.name,
    items: group.items.filter(item =>
      item.style.label.toLowerCase().includes(keyword) ||
      item.type.toLowerCase().includes(keyword)
    ),
  })).filter(group => group.items.length > 0)
})

// ========== 折叠处理 ==========

const handleToggle = () => {
  emit('toggle')
}

const toggleGroup = (groupName: string) => {
  const index = collapsedGroups.value.indexOf(groupName)
  if (index > -1) {
    collapsedGroups.value.splice(index, 1)
  } else {
    collapsedGroups.value.push(groupName)
  }
}

// ========== 拖拽处理 ==========

const handleDragStart = (nodeType: TaskNodeType, event: DragEvent) => {
  // 设置拖拽数据
  if (event.dataTransfer) {
    event.dataTransfer.setData('nodeType', nodeType)
    event.dataTransfer.setData('text/plain', nodeStyleMap[nodeType].label)
    event.dataTransfer.effectAllowed = 'copy'
  }

  // 发送事件
  emit('drag-node', nodeType, event)
}
</script>

<style scoped lang="scss">
$palette-width: 280px;
$palette-collapsed-width: 32px;
$palette-bg: #fff;
$palette-border: #e4e7ed;

.task-palette {
  width: $palette-width;
  flex-shrink: 0;
  height: 100%;
  overflow-y: auto;
  overflow-x: hidden;
  background: $palette-bg;
  border-right: 1px solid $palette-border;
  transition: width 0.3s ease;
  position: relative;

  &.collapsed {
    width: $palette-collapsed-width;
  }

  .collapsed-bar {
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    background: $palette-bg;

    &:hover {
      background: #f5f7fa;
    }
  }

  :deep(.el-card) {
    height: 100%;
    border: none;
    border-radius: 0;

    .el-card__header {
      padding: 16px;
      border-bottom: 1px solid $palette-border;
    }

    .el-card__body {
      padding: 16px;
    }
  }

  .palette-header {
    font-size: 14px;
    font-weight: 500;
    color: #303133;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .search-input {
    margin-bottom: 16px;
  }

  .node-groups {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  .node-group {
    .group-title {
      font-size: 12px;
      color: #909399;
      margin-bottom: 8px;
      padding-left: 4px;
      display: flex;
      align-items: center;
      gap: 4px;
      cursor: pointer;
      user-select: none;

      &:hover {
        color: #606266;
      }

      .group-arrow {
        transition: transform 0.2s ease;
      }
    }

    .group-items {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
  }

  .node-item {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 8px 12px;
    background: #f5f7fa;
    border-radius: 8px;
    cursor: grab;
    transition: all 0.2s ease;

    &:hover {
      background: #eef1f6;
      transform: translateY(-1px);
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
    }

    &:active {
      cursor: grabbing;
      transform: translateY(0);
    }

    .node-icon {
      width: 40px;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 6px;
      border: 2px solid;

      .icon-text {
        font-size: 18px;
      }
    }

    .node-info {
      display: flex;
      flex-direction: column;
      gap: 2px;

      .node-label {
        font-size: 13px;
        font-weight: 500;
        color: #303133;
      }

      .node-type {
        font-size: 11px;
        color: #909399;
      }
    }
  }
}

// 滚动条样式
.task-palette::-webkit-scrollbar {
  width: 6px;
}

.task-palette::-webkit-scrollbar-thumb {
  background: rgba(0, 0, 0, 0.2);
  border-radius: 3px;

  &:hover {
    background: rgba(0, 0, 0, 0.3);
  }
}

.task-palette::-webkit-scrollbar-track {
  background: transparent;
}
</style>
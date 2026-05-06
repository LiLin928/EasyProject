<!-- src/components/ant_workflow/DagDesigner/NodePalette.vue -->
<template>
  <div class="node-palette" :class="{ collapsed: collapsed }">
    <el-card shadow="never" v-show="!collapsed">
      <template #header>
        <div class="palette-header">
          <span>{{ t('antWorkflow.designer.nodePalette') }}</span>
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
        :placeholder="t('antWorkflow.designer.searchNode')"
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
                <el-icon :size="18" :color="item.style.color">
                  <component :is="getIconComponent(item.style.icon)" />
                </el-icon>
              </div>
              <div class="node-info">
                <span class="node-label">{{ item.style.typeName }}</span>
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
import { Search, User, UserFilled, CopyDocument, Share, Grid, Setting, Bell, Link, Document, Stamp, CircleClose, DArrowLeft, DArrowRight, ArrowDown, ArrowRight } from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import { AntNodeType, nodeStyleMap } from '@/types/antWorkflow'

const { t } = useLocale()

// Props
const props = defineProps<{
  collapsed?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'toggle'): void
  (e: 'drag-node', nodeType: AntNodeType, event: DragEvent): void
}>()

// 搜索关键词
const searchKeyword = ref('')

// 折叠的分组列表
const collapsedGroups = ref<string[]>([])

// 图标映射
const iconComponentMap: Record<string, any> = {
  'User': User,
  'UserFilled': UserFilled,
  'CopyDocument': CopyDocument,
  'Share': Share,
  'Grid': Grid,
  'Setting': Setting,
  'Bell': Bell,
  'Link': Link,
  'Document': Document,
  'Stamp': Stamp,
  'CircleClose': CircleClose,
}

const getIconComponent = (iconName: string) => {
  return iconComponentMap[iconName] || User
}

// 节点分组配置
const nodeGroups = [
  {
    name: '基础节点',
    items: [
      { type: AntNodeType.START, style: nodeStyleMap[AntNodeType.START] },
      { type: AntNodeType.APPROVER, style: nodeStyleMap[AntNodeType.APPROVER] },
      { type: AntNodeType.COPYER, style: nodeStyleMap[AntNodeType.COPYER] },
      { type: AntNodeType.END, style: nodeStyleMap[AntNodeType.END] },
    ],
  },
  {
    name: '控制节点',
    items: [
      { type: AntNodeType.CONDITION, style: nodeStyleMap[AntNodeType.CONDITION] },
      { type: AntNodeType.PARALLEL, style: nodeStyleMap[AntNodeType.PARALLEL] },
      { type: AntNodeType.COUNTER_SIGN, style: nodeStyleMap[AntNodeType.COUNTER_SIGN] },
    ],
  },
  {
    name: '任务节点',
    items: [
      { type: AntNodeType.SERVICE, style: nodeStyleMap[AntNodeType.SERVICE] },
      { type: AntNodeType.NOTIFICATION, style: nodeStyleMap[AntNodeType.NOTIFICATION] },
      { type: AntNodeType.WEBHOOK, style: nodeStyleMap[AntNodeType.WEBHOOK] },
    ],
  },
  {
    name: '流程节点',
    items: [
      { type: AntNodeType.SUBFLOW, style: nodeStyleMap[AntNodeType.SUBFLOW] },
    ],
  },
]

// 过滤后的分组
const filteredGroups = computed(() => {
  if (!searchKeyword.value) {
    return nodeGroups
  }

  const keyword = searchKeyword.value.toLowerCase()
  return nodeGroups
    .map((group) => ({
      name: group.name,
      items: group.items.filter(
        (item) =>
          item.style.typeName.toLowerCase().includes(keyword) ||
          item.type.toLowerCase().includes(keyword)
      ),
    }))
    .filter((group) => group.items.length > 0)
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

const handleDragStart = (nodeType: AntNodeType, event: DragEvent) => {
  if (event.dataTransfer) {
    event.dataTransfer.setData('nodeType', nodeType)
    event.dataTransfer.setData('text/plain', nodeStyleMap[nodeType].typeName)
    event.dataTransfer.effectAllowed = 'copy'
  }

  emit('drag-node', nodeType, event)
}
</script>

<style scoped lang="scss">
$palette-width: 280px;
$palette-collapsed-width: 32px;
$palette-bg: #fff;
$palette-border: #e4e7ed;

.node-palette {
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
.node-palette::-webkit-scrollbar {
  width: 6px;
}

.node-palette::-webkit-scrollbar-thumb {
  background: rgba(0, 0, 0, 0.2);
  border-radius: 3px;

  &:hover {
    background: rgba(0, 0, 0, 0.3);
  }
}

.node-palette::-webkit-scrollbar-track {
  background: transparent;
}
</style>
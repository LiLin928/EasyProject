<!-- components/MenuNode.vue -->
<template>
  <view class="menu-node">
    <!-- 当前节点 -->
    <view
      class="node-item"
      :style="{
        paddingLeft: `${24 + level * 24}rpx`,
      }"
      @click="handleEdit"
    >
      <!-- 展开/折叠按钮 -->
      <view v-if="hasChildren" class="expand-btn" @click.stop="handleToggle">
        <text>{{ isExpanded ? '−' : '+' }}</text>
      </view>
      <view v-else class="expand-placeholder" />

      <!-- 菜单内容 -->
      <view class="node-content">
        <view class="node-header">
          <text class="node-name" :style="nodeNameStyle">{{ menu.name }}</text>
          <!-- 菜单类型标签 -->
          <view class="type-tag" :class="typeTagClass">
            <text>{{ typeText }}</text>
          </view>
        </view>
        <view class="node-info">
          <text v-if="menu.icon" class="node-icon">图标: {{ menu.icon }}</text>
          <text class="node-sort">排序: {{ menu.sort }}</text>
        </view>
      </view>

      <!-- 状态标签 -->
      <view
        class="status-tag"
        :class="menu.status === MenuStatus.Visible ? 'visible' : 'hidden'"
      >
        <text>{{ menu.status === MenuStatus.Visible ? '显示' : '隐藏' }}</text>
      </view>
    </view>

    <!-- 子节点(递归) -->
    <view v-if="hasChildren && isExpanded" class="node-children">
      <MenuNode
        v-for="child in menu.children"
        :key="child.id"
        :menu="child"
        :level="level + 1"
        :expanded-keys="expandedKeys"
        @edit="(menu: Menu) => $emit('edit', menu)"
        @toggle="(id: string) => $emit('toggle', id)"
      />
    </view>
  </view>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Menu } from '@/types/basic'
import { MenuType, MenuStatus } from '@/types/basic'

// Props
const props = defineProps<{
  menu: Menu
  level: number
  expandedKeys: Set<string>
}>()

// Emits
const emit = defineEmits<{
  edit: [menu: Menu]
  toggle: [id: string]
}>()

// 计算属性
const hasChildren = computed(() => props.menu.children && props.menu.children.length > 0)
const isExpanded = computed(() => props.expandedKeys.has(props.menu.id))

// 动态层级样式
const nodeNameStyle = computed(() => {
  const fontSize = props.level === 0 ? 32 : props.level === 1 ? 30 : 28
  const fontWeight = props.level === 0 ? 600 : props.level === 1 ? 500 : 400
  return {
    fontSize: `${fontSize}rpx`,
    fontWeight: fontWeight,
  }
})

// 菜单类型文本
const typeText = computed(() => {
  switch (props.menu.type) {
    case MenuType.Directory:
      return '目录'
    case MenuType.Menu:
      return '菜单'
    case MenuType.Button:
      return '按钮'
    default:
      return '未知'
  }
})

// 菜单类型标签样式类
const typeTagClass = computed(() => {
  switch (props.menu.type) {
    case MenuType.Directory:
      return 'directory'
    case MenuType.Menu:
      return 'menu'
    case MenuType.Button:
      return 'button'
    default:
      return ''
  }
})

// 方法
const handleEdit = () => {
  emit('edit', props.menu)
}

const handleToggle = () => {
  emit('toggle', props.menu.id)
}
</script>

<style lang="scss" scoped>
.menu-node {
  // 嵌套样式继承
}

.node-item {
  display: flex;
  align-items: center;
  padding: 30rpx 24rpx;
  border-bottom: 1rpx solid $u-border-color;
}

.expand-btn {
  width: 40rpx;
  height: 40rpx;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 16rpx;

  text {
    font-size: 28rpx;
    color: $u-primary;
    font-weight: 600;
  }
}

.expand-placeholder {
  width: 40rpx;
  height: 40rpx;
  margin-right: 16rpx;
}

.node-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 8rpx;
}

.node-header {
  display: flex;
  align-items: center;
  gap: 12rpx;
}

.node-name {
  font-size: 28rpx;
  color: $u-main-color;
}

.type-tag {
  padding: 4rpx 12rpx;
  border-radius: $u-radius-sm;

  text {
    font-size: 22rpx;
  }

  // 目录 - 蓝色
  &.directory {
    background: rgba(#3b82f6, 0.1);

    text {
      color: #3b82f6;
    }
  }

  // 菜单 - 绿色
  &.menu {
    background: rgba(#10b981, 0.1);

    text {
      color: #10b981;
    }
  }

  // 按钮 - 橙色
  &.button {
    background: rgba(#f59e0b, 0.1);

    text {
      color: #f59e0b;
    }
  }
}

.node-info {
  display: flex;
  gap: 16rpx;
}

.node-icon,
.node-sort {
  font-size: 24rpx;
  color: $u-tips-color;
}

.status-tag {
  padding: 8rpx 16rpx;
  border-radius: $u-radius-sm;

  text {
    font-size: 24rpx;
  }

  &.visible {
    background: rgba($u-primary, 0.1);

    text {
      color: $u-primary;
    }
  }

  &.hidden {
    background: rgba($u-tips-color, 0.1);

    text {
      color: $u-tips-color;
    }
  }
}

.node-children {
  background: $u-light-color;
}
</style>
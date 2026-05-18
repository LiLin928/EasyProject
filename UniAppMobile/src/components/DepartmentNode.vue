<!-- components/DepartmentNode.vue -->
<template>
  <view class="department-node">
    <!-- 当前节点 -->
    <view class="node-item" :class="`level-${level}`" @click="handleEdit">
      <!-- 展开/折叠按钮 -->
      <view v-if="hasChildren" class="expand-btn" @click.stop="handleToggle">
        <text>{{ isExpanded ? '−' : '+' }}</text>
      </view>
      <view v-else class="expand-placeholder" />

      <!-- 部门内容 -->
      <view class="node-content">
        <text class="node-name">{{ department.name }}</text>
        <view class="node-info">
          <text v-if="department.code" class="node-code">编码: {{ department.code }}</text>
          <text class="node-sort">排序: {{ department.sort }}</text>
        </view>
      </view>

      <!-- 状态标签 -->
      <view
        class="status-tag"
        :class="department.status === DepartmentStatus.Enabled ? 'enabled' : 'disabled'"
      >
        <text>{{ department.status === DepartmentStatus.Enabled ? '启用' : '禁用' }}</text>
      </view>
    </view>

    <!-- 子节点（递归） -->
    <view v-if="hasChildren && isExpanded" class="node-children">
      <DepartmentNode
        v-for="child in department.children"
        :key="child.id"
        :department="child"
        :level="level + 1"
        :expanded-keys="expandedKeys"
        @edit="(dept: Department) => $emit('edit', dept)"
        @toggle="(id: string) => $emit('toggle', id)"
      />
    </view>
  </view>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Department } from '@/types/basic'
import { DepartmentStatus } from '@/types/basic'

// Props
const props = defineProps<{
  department: Department
  level: number
  expandedKeys: Set<string>
}>()

// Emits
const emit = defineEmits<{
  edit: [department: Department]
  toggle: [id: string]
}>()

// 计算属性
const hasChildren = computed(() => props.department.children && props.department.children.length > 0)
const isExpanded = computed(() => props.expandedKeys.has(props.department.id))

// 方法
const handleEdit = () => {
  emit('edit', props.department)
}

const handleToggle = () => {
  emit('toggle', props.department.id)
}
</script>

<style lang="scss" scoped>
.department-node {
  // 嵌套样式继承
}

.node-item {
  display: flex;
  align-items: center;
  padding: 30rpx 24rpx;
  border-bottom: 1rpx solid $u-border-color;

  &.level-0 {
    .node-name {
      font-size: 32rpx;
      font-weight: 600;
    }
  }

  &.level-1 {
    padding-left: 48rpx;

    .node-name {
      font-size: 30rpx;
      font-weight: 500;
    }
  }

  &.level-2 {
    padding-left: 72rpx;

    .node-name {
      font-size: 28rpx;
    }
  }

  &.level-3,
  &.level-4,
  &.level-5 {
    padding-left: 96rpx;

    .node-name {
      font-size: 28rpx;
    }
  }
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

.node-name {
  font-size: 28rpx;
  color: $u-main-color;
}

.node-info {
  display: flex;
  gap: 16rpx;
}

.node-code,
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

  &.enabled {
    background: rgba($u-primary, 0.1);

    text {
      color: $u-primary;
    }
  }

  &.disabled {
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
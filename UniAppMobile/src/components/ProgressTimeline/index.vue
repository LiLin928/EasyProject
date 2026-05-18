<!-- components/ProgressTimeline/index.vue -->
<template>
  <view class="timeline">
    <view v-for="(node, index) in nodes" :key="node.id" class="timeline-item">
      <!-- 时间线左侧 -->
      <view class="timeline-left">
        <view :class="['timeline-dot', getStatusClass(node.status)]" />
        <view v-if="index < nodes.length - 1" class="timeline-line" />
      </view>

      <!-- 时间线内容 -->
      <view class="timeline-content">
        <view class="node-header">
          <text class="node-name">{{ node.name }}</text>
          <text :class="['node-status', getStatusClass(node.status)]">
            {{ getStatusText(node.status) }}
          </text>
        </view>

        <view v-if="node.operatorName" class="node-info">
          <text class="operator">{{ node.operatorName }}</text>
          <text v-if="node.operateTime" class="time">{{ node.operateTime }}</text>
        </view>

        <view v-if="node.comment" class="node-comment">
          <text>{{ node.comment }}</text>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { TaskStatus } from '@/types'
import type { ApprovalNode } from '@/types'

defineProps<{
  nodes: ApprovalNode[]
}>()

const getStatusClass = (status: TaskStatus) => {
  const map = {
    [TaskStatus.Pending]: 'pending',
    [TaskStatus.Approved]: 'approved',
    [TaskStatus.Rejected]: 'rejected',
    [TaskStatus.Transferred]: 'transferred',
    [TaskStatus.Withdrawn]: 'withdrawn',
  }
  return map[status] || 'pending'
}

const getStatusText = (status: TaskStatus) => {
  const map = {
    [TaskStatus.Pending]: '待处理',
    [TaskStatus.Approved]: '已通过',
    [TaskStatus.Rejected]: '已拒绝',
    [TaskStatus.Transferred]: '已转办',
    [TaskStatus.Withdrawn]: '已撤回',
  }
  return map[status] || '未知'
}
</script>

<style lang="scss" scoped>
.timeline {
  padding: 16rpx;

  .timeline-item {
    display: flex;

    .timeline-left {
      width: 48rpx;
      display: flex;
      flex-direction: column;
      align-items: center;

      .timeline-dot {
        width: 24rpx;
        height: 24rpx;
        border-radius: 50%;
        background: $u-border-color;

        &.pending {
          background: $u-warning;
        }
        &.approved {
          background: $u-success;
        }
        &.rejected {
          background: $u-error;
        }
        &.transferred {
          background: $u-primary;
        }
        &.withdrawn {
          background: $u-content-color;
        }
      }

      .timeline-line {
        width: 4rpx;
        height: 100rpx;
        background: $u-border-color;
        margin: 8rpx 0;
      }
    }

    .timeline-content {
      flex: 1;
      padding-left: 24rpx;
      margin-bottom: 24rpx;

      .node-header {
        display: flex;
        align-items: center;
        margin-bottom: 12rpx;

        .node-name {
          font-size: 28rpx;
          font-weight: 600;
          color: $u-main-color;
        }

        .node-status {
          font-size: 22rpx;
          padding: 4rpx 12rpx;
          border-radius: 4rpx;
          margin-left: 12rpx;
          color: #fff;

          &.pending {
            background: $u-warning;
          }
          &.approved {
            background: $u-success;
          }
          &.rejected {
            background: $u-error;
          }
          &.transferred {
            background: $u-primary;
          }
          &.withdrawn {
            background: $u-content-color;
          }
        }
      }

      .node-info {
        display: flex;
        align-items: center;
        font-size: 24rpx;
        color: $u-content-color;
        margin-bottom: 8rpx;

        .operator {
          margin-right: 16rpx;
        }
      }

      .node-comment {
        padding: 12rpx;
        background: $u-light-color;
        border-radius: $u-radius;
        font-size: 24rpx;
        color: $u-main-color;
      }
    }
  }
}
</style>
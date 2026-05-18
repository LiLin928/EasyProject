<!-- components/CardItem/index.vue -->
<template>
  <view class="card-item" @click="$emit('click')">
    <view class="card-header">
      <text class="card-title">{{ title }}</text>
      <text v-if="urgent" class="tag urgent">紧急</text>
      <text :class="['tag', statusClass]">{{ statusText }}</text>
    </view>

    <view class="card-body">
      <view class="card-row">
        <text class="label">流程：</text>
        <text class="value">{{ processName }}</text>
      </view>
      <view class="card-row">
        <text class="label">申请人：</text>
        <text class="value">{{ applicantName }}</text>
      </view>
      <view class="card-row">
        <text class="label">时间：</text>
        <text class="value">{{ createTime }}</text>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { TaskStatus } from '@/types'

const props = defineProps<{
  title: string
  processName: string
  applicantName: string
  createTime: string
  status: TaskStatus
  urgent?: boolean
}>()

defineEmits(['click'])

const statusText = computed(() => {
  const map = {
    [TaskStatus.Pending]: '待处理',
    [TaskStatus.Approved]: '已通过',
    [TaskStatus.Rejected]: '已拒绝',
    [TaskStatus.Transferred]: '已转办',
    [TaskStatus.Withdrawn]: '已撤回',
  }
  return map[props.status] || '未知'
})

const statusClass = computed(() => {
  const map = {
    [TaskStatus.Pending]: 'pending',
    [TaskStatus.Approved]: 'approved',
    [TaskStatus.Rejected]: 'rejected',
    [TaskStatus.Transferred]: 'transferred',
    [TaskStatus.Withdrawn]: 'withdrawn',
  }
  return map[props.status] || 'unknown'
})
</script>

<style lang="scss" scoped>
.card-item {
  background: #fff;
  border-radius: $u-radius;
  padding: 24rpx;
  margin-bottom: 16rpx;

  .card-header {
    display: flex;
    align-items: center;
    margin-bottom: 16rpx;

    .card-title {
      font-size: 30rpx;
      font-weight: 600;
      flex: 1;
    }

    .tag {
      font-size: 22rpx;
      padding: 4rpx 12rpx;
      border-radius: 4rpx;
      margin-left: 8rpx;

      &.urgent {
        background: $u-error;
        color: #fff;
      }

      &.pending {
        background: $u-warning;
        color: #fff;
      }

      &.approved {
        background: $u-success;
        color: #fff;
      }

      &.rejected {
        background: $u-error;
        color: #fff;
      }

      &.transferred {
        background: $u-primary;
        color: #fff;
      }

      &.withdrawn {
        background: $u-content-color;
        color: #fff;
      }
    }
  }

  .card-body {
    .card-row {
      display: flex;
      margin-bottom: 8rpx;

      .label {
        font-size: 24rpx;
        color: $u-content-color;
        width: 140rpx;
      }

      .value {
        font-size: 24rpx;
        color: $u-main-color;
      }
    }
  }
}
</style>
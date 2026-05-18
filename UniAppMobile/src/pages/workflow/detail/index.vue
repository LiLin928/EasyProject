<!-- pages/workflow/detail/index.vue -->
<template>
  <view class="detail-page">
    <Loading :show="loading" />

    <view v-if="taskDetail" class="detail-content">
      <!-- 基本信息 -->
      <view class="info-card">
        <view class="card-header">
          <text class="title">{{ taskDetail.title }}</text>
          <text v-if="taskDetail.urgent" class="tag urgent">紧急</text>
        </view>
        <view class="card-body">
          <view class="info-row">
            <text class="label">流程：</text>
            <text class="value">{{ taskDetail.processName }}</text>
          </view>
          <view class="info-row">
            <text class="label">申请人：</text>
            <text class="value">{{ taskDetail.applicantName }}</text>
          </view>
          <view class="info-row">
            <text class="label">申请时间：</text>
            <text class="value">{{ taskDetail.createTime }}</text>
          </view>
        </view>
      </view>

      <!-- 表单数据 -->
      <view class="form-card">
        <text class="section-title">表单信息</text>
        <view class="form-content">
          <view v-for="(value, key) in taskDetail.formData" :key="key" class="form-row">
            <text class="label">{{ key }}：</text>
            <text class="value">{{ value }}</text>
          </view>
        </view>
      </view>

      <!-- 审批进度 -->
      <view class="progress-card">
        <text class="section-title">审批进度</text>
        <ProgressTimeline :nodes="taskDetail.approvalProgress" />
      </view>
    </view>

    <!-- 审批操作栏 -->
    <view v-if="taskDetail?.canApprove" class="fixed-bottom">
      <ApprovalBar
        :task-id="taskDetail.id"
        :can-transfer="taskDetail.canTransfer"
        @success="handleSuccess"
      />
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getTaskDetail } from '@/api/workflow/taskApi'
import type { TaskDetail } from '@/types'
import Loading from '@/components/Loading/index.vue'
import ProgressTimeline from '@/components/ProgressTimeline/index.vue'
import ApprovalBar from '@/components/ApprovalBar/index.vue'

const loading = ref(false)
const taskDetail = ref<TaskDetail | null>(null)
const taskId = ref('')

onMounted(() => {
  // 从路由参数获取 taskId
  const pages = getCurrentPages()
  const currentPage = pages[pages.length - 1] as any
  taskId.value = currentPage.options?.id || ''

  if (taskId.value) {
    loadDetail()
  }
})

const loadDetail = async () => {
  loading.value = true
  try {
    taskDetail.value = await getTaskDetail(taskId.value)
  } finally {
    loading.value = false
  }
}

const handleSuccess = () => {
  uni.showToast({ title: '审批完成', icon: 'success' })
  // 返回上一页
  uni.navigateBack()
}
</script>

<style lang="scss" scoped>
.detail-page {
  min-height: 100vh;
  background: $u-bg-color;
  padding-bottom: 120rpx;
}

.info-card, .form-card, .progress-card {
  margin: 20rpx;
  padding: 24rpx;
  background: #fff;
  border-radius: $u-radius;
}

.info-card {
  .card-header {
    display: flex;
    align-items: center;
    margin-bottom: 16rpx;

    .title {
      font-size: 32rpx;
      font-weight: 600;
      flex: 1;
    }

    .tag {
      font-size: 22rpx;
      padding: 4rpx 12rpx;
      border-radius: 4rpx;

      &.urgent {
        background: $u-error;
        color: #fff;
      }
    }
  }

  .card-body {
    .info-row {
      display: flex;
      margin-bottom: 12rpx;

      .label {
        font-size: 26rpx;
        color: $u-content-color;
        width: 160rpx;
      }

      .value {
        font-size: 26rpx;
        color: $u-main-color;
      }
    }
  }
}

.form-card, .progress-card {
  .section-title {
    font-size: 28rpx;
    font-weight: 600;
    color: $u-main-color;
    margin-bottom: 16rpx;
    display: block;
  }
}

.form-content {
  .form-row {
    display: flex;
    margin-bottom: 12rpx;

    .label {
      font-size: 26rpx;
      color: $u-content-color;
      width: 180rpx;
    }

    .value {
      font-size: 26rpx;
      color: $u-main-color;
    }
  }
}

.fixed-bottom {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  background: #fff;
  z-index: 100;
}
</style>
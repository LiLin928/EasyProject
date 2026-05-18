<!-- components/ApprovalBar/index.vue -->
<template>
  <view class="approval-bar">
    <view class="action-btn approve" @click="handleApprove(true)">
      <text>通过</text>
    </view>
    <view class="action-btn reject" @click="handleApprove(false)">
      <text>拒绝</text>
    </view>
    <view v-if="canTransfer" class="action-btn transfer" @click="showTransferPopup = true">
      <text>转办</text>
    </view>
  </view>

  <!-- 审批意见弹窗 -->
  <view v-if="showPopup" class="popup-mask" @click="showPopup = false">
    <view class="popup-content" @click.stop>
      <view class="popup-title">{{ approveAction === 'approve' ? '审批通过' : '审批拒绝' }}</view>

      <input
        v-model="comment"
        class="comment-input"
        placeholder="请输入审批意见（可选）"
        :maxlength="200"
      />

      <view class="popup-actions">
        <view class="btn cancel" @click="showPopup = false">取消</view>
        <view class="btn confirm" @click="submitApproval">确认</view>
      </view>
    </view>
  </view>

  <!-- 转办弹窗 -->
  <view v-if="showTransferPopup" class="popup-mask" @click="showTransferPopup = false">
    <view class="popup-content" @click.stop>
      <view class="popup-title">转办任务</view>

      <input
        v-model="transferComment"
        class="comment-input"
        placeholder="请输入转办原因"
      />

      <!-- TODO: 选择转办目标用户 -->

      <view class="popup-actions">
        <view class="btn cancel" @click="showTransferPopup = false">取消</view>
        <view class="btn confirm" @click="submitTransfer">确认转办</view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { approveTask, transferTask } from '@/api/workflow/runtimeApi'

const props = defineProps<{
  taskId: string
  canTransfer?: boolean
}>()

const emit = defineEmits<{
  (e: 'success'): void
}>()

const showPopup = ref(false)
const showTransferPopup = ref(false)
const approveAction = ref<'approve' | 'reject'>('approve')
const comment = ref('')
const transferComment = ref('')
const transferTo = ref('')

const handleApprove = (approved: boolean) => {
  approveAction.value = approved ? 'approve' : 'reject'
  showPopup.value = true
}

const submitApproval = async () => {
  try {
    await approveTask({
      taskId: props.taskId,
      approved: approveAction.value === 'approve',
      comment: comment.value,
    })
    uni.showToast({ title: '审批成功', icon: 'success' })
    showPopup.value = false
    emit('success')
  } catch (error) {
    uni.showToast({ title: '审批失败', icon: 'none' })
  }
}

const submitTransfer = async () => {
  if (!transferTo.value) {
    uni.showToast({ title: '请选择转办目标', icon: 'none' })
    return
  }
  try {
    await transferTask(props.taskId, transferTo.value, transferComment.value)
    uni.showToast({ title: '转办成功', icon: 'success' })
    showTransferPopup.value = false
    emit('success')
  } catch (error) {
    uni.showToast({ title: '转办失败', icon: 'none' })
  }
}
</script>

<style lang="scss" scoped>
.approval-bar {
  display: flex;
  gap: 16rpx;
  padding: 20rpx;
  background: #fff;
  border-top: 1rpx solid $u-border-color;

  .action-btn {
    flex: 1;
    height: 80rpx;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: $u-radius;
    font-size: 28rpx;
    color: #fff;

    &.approve {
      background: $u-success;
    }
    &.reject {
      background: $u-error;
    }
    &.transfer {
      background: $u-primary;
      flex: 0.5;
    }
  }
}

.popup-mask {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;

  .popup-content {
    width: 80%;
    padding: 40rpx;
    background: #fff;
    border-radius: $u-radius-lg;

    .popup-title {
      font-size: 32rpx;
      font-weight: 600;
      margin-bottom: 24rpx;
      text-align: center;
    }

    .comment-input {
      width: 100%;
      height: 80rpx;
      padding: 0 20rpx;
      border: 1rpx solid $u-border-color;
      border-radius: $u-radius;
      font-size: 28rpx;
    }

    .popup-actions {
      display: flex;
      gap: 20rpx;
      margin-top: 24rpx;

      .btn {
        flex: 1;
        height: 80rpx;
        display: flex;
        align-items: center;
        justify-content: center;
        border-radius: $u-radius;
        font-size: 28rpx;

        &.cancel {
          background: $u-light-color;
          color: $u-content-color;
        }
        &.confirm {
          background: $u-primary;
          color: #fff;
        }
      }
    }
  }
}
</style>
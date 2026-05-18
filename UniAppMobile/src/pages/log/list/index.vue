<!-- pages/log/list/index.vue -->
<template>
  <view class="log-list-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <input
        v-model="searchKeyword"
        class="search-input"
        type="text"
        placeholder="搜索操作人/模块名称"
        @confirm="handleSearch"
      />
      <view class="search-btn" @click="handleSearch">
        <text>搜索</text>
      </view>
    </view>

    <!-- 筛选栏 -->
    <view class="filter-bar">
      <!-- 日志类型筛选 -->
      <view class="filter-item" @click="showTypePicker = true">
        <text class="filter-label">{{ logTypeText }}</text>
        <text class="filter-arrow">▼</text>
      </view>

      <!-- 时间筛选 -->
      <view class="filter-item" @click="showTimePicker = true">
        <text class="filter-label">{{ timeRangeText }}</text>
        <text class="filter-arrow">▼</text>
      </view>

      <!-- 清理日志按钮 -->
      <view class="filter-item clear-btn" @click="handleClearLogs">
        <text class="filter-label">清理日志</text>
      </view>
    </view>

    <!-- 日志列表 -->
    <scroll-view
      class="log-scroll"
      scroll-y
      @scrolltolower="loadMore"
    >
      <view class="log-list">
        <view
          v-for="item in logList"
          :key="item.id"
          class="log-card"
          @click="handleDetail(item)"
        >
          <view class="log-header">
            <text class="log-module">{{ item.module }}</text>
            <view :class="['log-type-tag', getLogTypeClass(item.logType)]">
              <text>{{ getLogTypeText(item.logType) }}</text>
            </view>
          </view>

          <view class="log-body">
            <view class="log-row">
              <text class="log-label">操作类型：</text>
              <text class="log-value">{{ item.action }}</text>
            </view>
            <view class="log-row">
              <text class="log-label">操作人：</text>
              <text class="log-value">{{ item.operator }}</text>
            </view>
            <view class="log-row">
              <text class="log-label">操作时间：</text>
              <text class="log-value">{{ formatTime(item.createTime) }}</text>
            </view>
          </view>

          <view class="log-footer">
            <text class="log-arrow">→</text>
          </view>
        </view>

        <!-- 加载状态 -->
        <view v-if="loading" class="loading-tip">
          <text>加载中...</text>
        </view>

        <!-- 加载完成 -->
        <view v-if="!hasMore && logList.length > 0" class="no-more-tip">
          <text>没有更多了</text>
        </view>

        <!-- 空状态 -->
        <view v-if="!loading && logList.length === 0" class="empty-state">
          <text class="empty-icon">📋</text>
          <text class="empty-text">暂无日志数据</text>
        </view>
      </view>
    </scroll-view>

    <!-- 日志类型选择器 -->
    <view v-if="showTypePicker" class="picker-mask" @click="showTypePicker = false">
      <view class="picker-content" @click.stop>
        <view class="picker-header">
          <text class="picker-title">选择日志类型</text>
          <view class="picker-close" @click="showTypePicker = false">
            <text>×</text>
          </view>
        </view>
        <view class="picker-list">
          <view
            class="picker-item"
            :class="{ active: queryParams.logType === undefined }"
            @click="selectLogType(undefined)"
          >
            <text>全部日志</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.logType === LogType.Login }"
            @click="selectLogType(LogType.Login)"
          >
            <text>登录日志</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.logType === LogType.Operation }"
            @click="selectLogType(LogType.Operation)"
          >
            <text>操作日志</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.logType === LogType.Exception }"
            @click="selectLogType(LogType.Exception)"
          >
            <text>异常日志</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.logType === LogType.System }"
            @click="selectLogType(LogType.System)"
          >
            <text>系统日志</text>
          </view>
        </view>
      </view>
    </view>

    <!-- 时间范围选择器 -->
    <view v-if="showTimePicker" class="picker-mask" @click="showTimePicker = false">
      <view class="picker-content" @click.stop>
        <view class="picker-header">
          <text class="picker-title">选择时间范围</text>
          <view class="picker-close" @click="showTimePicker = false">
            <text>×</text>
          </view>
        </view>
        <view class="picker-list">
          <view
            class="picker-item"
            :class="{ active: timeRangeType === 'all' }"
            @click="selectTimeRange('all')"
          >
            <text>全部时间</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: timeRangeType === 'today' }"
            @click="selectTimeRange('today')"
          >
            <text>今日</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: timeRangeType === 'yesterday' }"
            @click="selectTimeRange('yesterday')"
          >
            <text>昨日</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: timeRangeType === 'week' }"
            @click="selectTimeRange('week')"
          >
            <text>本周</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: timeRangeType === 'month' }"
            @click="selectTimeRange('month')"
          >
            <text>本月</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: timeRangeType === 'custom' }"
            @click="selectTimeRange('custom')"
          >
            <text>自定义范围</text>
          </view>
        </view>

        <!-- 自定义时间范围 -->
        <view v-if="timeRangeType === 'custom'" class="custom-time-picker">
          <view class="time-row">
            <text class="time-label">开始时间：</text>
            <picker mode="date" :value="customStartTime" @change="onStartTimeChange">
              <view class="time-picker-btn">
                <text>{{ customStartTime || '请选择' }}</text>
              </view>
            </picker>
          </view>
          <view class="time-row">
            <text class="time-label">结束时间：</text>
            <picker mode="date" :value="customEndTime" @change="onEndTimeChange">
              <view class="time-picker-btn">
                <text>{{ customEndTime || '请选择' }}</text>
              </view>
            </picker>
          </view>
          <view class="time-confirm-btn" @click="confirmCustomTime">
            <text>确认</text>
          </view>
        </view>
      </view>
    </view>

    <!-- 清理日志确认对话框 -->
    <view v-if="showClearDialog" class="picker-mask" @click="showClearDialog = false">
      <view class="dialog-content" @click.stop>
        <view class="dialog-header">
          <text class="dialog-title">清理日志</text>
        </view>
        <view class="dialog-body">
          <text class="dialog-text">请选择清理时间范围，将删除该时间之前的所有日志</text>
          <view class="clear-time-select">
            <picker mode="date" :value="clearBeforeTime" @change="onClearTimeChange">
              <view class="time-picker-btn">
                <text>{{ clearBeforeTime || '请选择时间' }}</text>
              </view>
            </picker>
          </view>
        </view>
        <view class="dialog-footer">
          <view class="dialog-btn cancel" @click="showClearDialog = false">
            <text>取消</text>
          </view>
          <view class="dialog-btn confirm" @click="confirmClearLogs">
            <text>确认清理</text>
          </view>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { getLogList, clearLogs } from '@/api/log/logApi'
import type { Log, LogQueryParams } from '@/types'
import { LogType } from '@/types'

// 搜索关键字
const searchKeyword = ref('')

// 搜索防抖定时器
let searchTimer: number | null = null

// 查询参数
const queryParams = ref<LogQueryParams>({
  pageIndex: 1,
  pageSize: 10,
  keyword: undefined,
  logType: undefined,
  startTime: undefined,
  endTime: undefined,
})

// 日志列表
const logList = ref<Log[]>([])

// 加载状态
const loading = ref(false)
const hasMore = ref(true)

// 选择器显示状态
const showTypePicker = ref(false)
const showTimePicker = ref(false)
const showClearDialog = ref(false)

// 时间范围类型
const timeRangeType = ref<'all' | 'today' | 'yesterday' | 'week' | 'month' | 'custom'>('all')

// 自定义时间范围
const customStartTime = ref('')
const customEndTime = ref('')

// 清理时间
const clearBeforeTime = ref('')

// 日志类型文本
const logTypeText = computed(() => {
  if (queryParams.value.logType === undefined) return '全部日志'
  switch (queryParams.value.logType) {
    case LogType.Login:
      return '登录日志'
    case LogType.Operation:
      return '操作日志'
    case LogType.Exception:
      return '异常日志'
    case LogType.System:
      return '系统日志'
    default:
      return '全部日志'
  }
})

// 时间范围文本
const timeRangeText = computed(() => {
  switch (timeRangeType.value) {
    case 'all':
      return '全部时间'
    case 'today':
      return '今日'
    case 'yesterday':
      return '昨日'
    case 'week':
      return '本周'
    case 'month':
      return '本月'
    case 'custom':
      return customStartTime.value && customEndTime.value
        ? `${customStartTime.value} ~ ${customEndTime.value}`
        : '自定义范围'
    default:
      return '全部时间'
  }
})

// 获取日志类型样式类
const getLogTypeClass = (type: LogType) => {
  switch (type) {
    case LogType.Login:
      return 'type-login'
    case LogType.Operation:
      return 'type-operation'
    case LogType.Exception:
      return 'type-exception'
    case LogType.System:
      return 'type-system'
    default:
      return 'type-default'
  }
}

// 获取日志类型文本
const getLogTypeText = (type: LogType) => {
  switch (type) {
    case LogType.Login:
      return '登录'
    case LogType.Operation:
      return '操作'
    case LogType.Exception:
      return '异常'
    case LogType.System:
      return '系统'
    default:
      return '未知'
  }
}

// 格式化时间
const formatTime = (time: string) => {
  if (!time) return '-'
  const date = new Date(time)
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hour = String(date.getHours()).padStart(2, '0')
  const minute = String(date.getMinutes()).padStart(2, '0')
  return `${year}-${month}-${day} ${hour}:${minute}`
}

// 获取时间范围
const getTimeRange = (type: string) => {
  const now = new Date()
  const today = new Date(now.getFullYear(), now.getMonth(), now.getDate())

  switch (type) {
    case 'today':
      return {
        startTime: today.toISOString(),
        endTime: new Date(today.getTime() + 24 * 60 * 60 * 1000).toISOString(),
      }
    case 'yesterday':
      return {
        startTime: new Date(today.getTime() - 24 * 60 * 60 * 1000).toISOString(),
        endTime: today.toISOString(),
      }
    case 'week':
      const weekStart = new Date(today.getTime() - today.getDay() * 24 * 60 * 60 * 1000)
      return {
        startTime: weekStart.toISOString(),
        endTime: new Date(weekStart.getTime() + 7 * 24 * 60 * 60 * 1000).toISOString(),
      }
    case 'month':
      const monthStart = new Date(now.getFullYear(), now.getMonth(), 1)
      const monthEnd = new Date(now.getFullYear(), now.getMonth() + 1, 0, 23, 59, 59)
      return {
        startTime: monthStart.toISOString(),
        endTime: monthEnd.toISOString(),
      }
    case 'custom':
      if (customStartTime.value && customEndTime.value) {
        return {
          startTime: new Date(customStartTime.value).toISOString(),
          endTime: new Date(customEndTime.value + 'T23:59:59').toISOString(),
        }
      }
      return { startTime: undefined, endTime: undefined }
    default:
      return { startTime: undefined, endTime: undefined }
  }
}

// 加载日志列表
const loadLogs = async (reset = false) => {
  if (loading.value) return
  if (!reset && !hasMore.value) return

  loading.value = true
  try {
    if (reset) {
      queryParams.value.pageIndex = 1
      logList.value = []
      hasMore.value = true
    }

    const response = await getLogList(queryParams.value)
    if (reset) {
      logList.value = response.list
    } else {
      logList.value = [...logList.value, ...response.list]
    }

    hasMore.value = logList.value.length < response.total
    queryParams.value.pageIndex++
  } catch (error: unknown) {
    console.error('加载日志列表失败:', error)
    const message = (error as { errMsg?: string; message?: string })?.errMsg
      || (error as { message?: string })?.message
      || '加载失败'
    uni.showToast({ title: message, icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 搜索（防抖处理）
const handleSearch = () => {
  if (searchTimer) clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    queryParams.value.keyword = searchKeyword.value || undefined
    loadLogs(true)
  }, 300) as unknown as number
}

// 加载更多
const loadMore = () => {
  if (!loading.value && hasMore.value) {
    loadLogs()
  }
}

// 选择日志类型
const selectLogType = (type: LogType | undefined) => {
  queryParams.value.logType = type
  showTypePicker.value = false
  loadLogs(true)
}

// 选择时间范围
const selectTimeRange = (type: 'all' | 'today' | 'yesterday' | 'week' | 'month' | 'custom') => {
  timeRangeType.value = type

  if (type !== 'custom') {
    const range = getTimeRange(type)
    queryParams.value.startTime = range.startTime
    queryParams.value.endTime = range.endTime

    if (type !== 'custom') {
      showTimePicker.value = false
      loadLogs(true)
    }
  }
}

// 开始时间变更
const onStartTimeChange = (e: { detail: { value: string } }) => {
  customStartTime.value = e.detail.value
}

// 结束时间变更
const onEndTimeChange = (e: { detail: { value: string } }) => {
  customEndTime.value = e.detail.value
}

// 确认自定义时间
const confirmCustomTime = () => {
  if (!customStartTime.value || !customEndTime.value) {
    uni.showToast({ title: '请选择完整的时间范围', icon: 'none' })
    return
  }

  if (new Date(customStartTime.value) > new Date(customEndTime.value)) {
    uni.showToast({ title: '开始时间不能大于结束时间', icon: 'none' })
    return
  }

  const range = getTimeRange('custom')
  queryParams.value.startTime = range.startTime
  queryParams.value.endTime = range.endTime

  showTimePicker.value = false
  loadLogs(true)
}

// 清理日志
const handleClearLogs = () => {
  // 默认清理一个月之前的日志
  const oneMonthAgo = new Date()
  oneMonthAgo.setMonth(oneMonthAgo.getMonth() - 1)
  const year = oneMonthAgo.getFullYear()
  const month = String(oneMonthAgo.getMonth() + 1).padStart(2, '0')
  const day = String(oneMonthAgo.getDate()).padStart(2, '0')
  clearBeforeTime.value = `${year}-${month}-${day}`

  showClearDialog.value = true
}

// 清理时间变更
const onClearTimeChange = (e: { detail: { value: string } }) => {
  clearBeforeTime.value = e.detail.value
}

// 确认清理日志
const confirmClearLogs = async () => {
  if (!clearBeforeTime.value) {
    uni.showToast({ title: '请选择清理时间', icon: 'none' })
    return
  }

  // 二次确认
  const res = await uni.showModal({
    title: '危险操作',
    content: `确定要删除 ${clearBeforeTime.value} 之前的所有日志吗？此操作不可恢复！`,
  })

  if (!res.confirm) {
    showClearDialog.value = false
    return
  }

  try {
    uni.showLoading({ title: '清理中...' })
    const count = await clearLogs(clearBeforeTime.value + 'T23:59:59')
    uni.hideLoading()
    uni.showToast({ title: `成功清理 ${count} 条日志`, icon: 'success' })

    showClearDialog.value = false
    loadLogs(true)
  } catch (error: unknown) {
    uni.hideLoading()
    console.error('清理日志失败:', error)
    const message = (error as { errMsg?: string; message?: string })?.errMsg
      || (error as { message?: string })?.message
      || '清理失败'
    uni.showToast({ title: message, icon: 'none' })
  }
}

// 查看详情
const handleDetail = (item: Log) => {
  uni.navigateTo({
    url: `/pages/log/detail/index?id=${item.id}`,
  })
}

onMounted(() => {
  loadLogs(true)
})

onUnmounted(() => {
  if (searchTimer) clearTimeout(searchTimer)
})
</script>

<style lang="scss" scoped>
.log-list-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: $u-bg-color;
}

.search-bar {
  display: flex;
  padding: 20rpx;
  background: #fff;
  gap: 16rpx;

  .search-input {
    flex: 1;
    height: 72rpx;
    padding: 0 24rpx;
    background: $u-light-color;
    border-radius: $u-radius;
    font-size: 28rpx;
  }

  .search-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 120rpx;
    height: 72rpx;
    background: $u-primary;
    border-radius: $u-radius;

    text {
      color: #fff;
      font-size: 28rpx;
    }
  }
}

.filter-bar {
  display: flex;
  padding: 16rpx 20rpx;
  background: #fff;
  border-top: 1rpx solid $u-border-color;
  gap: 16rpx;
  flex-wrap: wrap;

  .filter-item {
    display: flex;
    align-items: center;
    padding: 12rpx 20rpx;
    background: $u-light-color;
    border-radius: $u-radius;

    .filter-label {
      font-size: 26rpx;
      color: $u-main-color;
    }

    .filter-arrow {
      font-size: 20rpx;
      color: $u-content-color;
      margin-left: 8rpx;
    }
  }

  .clear-btn {
    background: rgba($u-warning, 0.1);

    .filter-label {
      color: $u-warning;
    }
  }
}

.log-scroll {
  flex: 1;
  overflow: hidden;
}

.log-list {
  padding: 20rpx;
}

.log-card {
  display: flex;
  flex-direction: column;
  background: #fff;
  border-radius: $u-radius-lg;
  padding: 24rpx;
  margin-bottom: 20rpx;

  .log-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 16rpx;

    .log-module {
      font-size: 30rpx;
      font-weight: 600;
      color: $u-main-color;
      flex: 1;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .log-type-tag {
      font-size: 22rpx;
      padding: 4rpx 12rpx;
      border-radius: 4rpx;
      flex-shrink: 0;

      text {
        color: #fff;
      }
    }

    .type-login {
      background: $u-primary;
    }

    .type-operation {
      background: $u-success;
    }

    .type-exception {
      background: $u-error;
    }

    .type-system {
      background: $u-content-color;
    }

    .type-default {
      background: $u-tips-color;
    }
  }

  .log-body {
    display: flex;
    flex-direction: column;
    gap: 12rpx;

    .log-row {
      display: flex;
      align-items: center;

      .log-label {
        font-size: 24rpx;
        color: $u-tips-color;
        width: 140rpx;
      }

      .log-value {
        font-size: 24rpx;
        color: $u-main-color;
        flex: 1;
      }
    }
  }

  .log-footer {
    display: flex;
    justify-content: flex-end;
    padding-top: 16rpx;
    border-top: 1rpx solid $u-border-color;
    margin-top: 16rpx;

    .log-arrow {
      font-size: 28rpx;
      color: $u-content-color;
    }
  }
}

.loading-tip,
.no-more-tip {
  text-align: center;
  padding: 30rpx 0;

  text {
    font-size: 26rpx;
    color: $u-tips-color;
  }
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 100rpx 0;

  .empty-icon {
    font-size: 80rpx;
  }

  .empty-text {
    font-size: 28rpx;
    color: $u-tips-color;
    margin-top: 20rpx;
  }
}

.picker-mask {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 1000;
  display: flex;
  align-items: flex-end;
}

.picker-content {
  width: 100%;
  background: #fff;
  border-radius: $u-radius-lg $u-radius-lg 0 0;
  max-height: 70vh;
}

.picker-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 30rpx;
  border-bottom: 1rpx solid $u-border-color;

  .picker-title {
    font-size: 32rpx;
    font-weight: 600;
    color: $u-main-color;
  }

  .picker-close {
    font-size: 40rpx;
    color: $u-content-color;
    line-height: 1;
  }
}

.picker-list {
  max-height: 50vh;
}

.picker-item {
  padding: 30rpx;
  border-bottom: 1rpx solid $u-border-color;

  text {
    font-size: 28rpx;
    color: $u-main-color;
  }

  &.active {
    background: rgba($u-primary, 0.1);

    text {
      color: $u-primary;
      font-weight: 600;
    }
  }
}

.custom-time-picker {
  padding: 30rpx;
  border-top: 1rpx solid $u-border-color;

  .time-row {
    display: flex;
    align-items: center;
    margin-bottom: 20rpx;

    .time-label {
      font-size: 28rpx;
      color: $u-main-color;
      width: 160rpx;
    }

    .time-picker-btn {
      flex: 1;
      height: 72rpx;
      display: flex;
      align-items: center;
      padding: 0 24rpx;
      background: $u-light-color;
      border-radius: $u-radius;

      text {
        font-size: 28rpx;
        color: $u-main-color;
      }
    }
  }

  .time-confirm-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 80rpx;
    background: $u-primary;
    border-radius: $u-radius;
    margin-top: 20rpx;

    text {
      color: #fff;
      font-size: 28rpx;
    }
  }
}

.dialog-content {
  width: 80%;
  max-width: 600rpx;
  background: #fff;
  border-radius: $u-radius-lg;
  margin: auto;
}

.dialog-header {
  padding: 30rpx;
  border-bottom: 1rpx solid $u-border-color;
  text-align: center;

  .dialog-title {
    font-size: 32rpx;
    font-weight: 600;
    color: $u-main-color;
  }
}

.dialog-body {
  padding: 30rpx;

  .dialog-text {
    font-size: 28rpx;
    color: $u-content-color;
    margin-bottom: 20rpx;
  }

  .clear-time-select {
    .time-picker-btn {
      width: 100%;
      height: 72rpx;
      display: flex;
      align-items: center;
      justify-content: center;
      background: $u-light-color;
      border-radius: $u-radius;

      text {
        font-size: 28rpx;
        color: $u-main-color;
      }
    }
  }
}

.dialog-footer {
  display: flex;
  padding: 20rpx 30rpx 30rpx;
  gap: 20rpx;

  .dialog-btn {
    flex: 1;
    height: 80rpx;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: $u-radius;

    text {
      font-size: 28rpx;
    }
  }

  .cancel {
    background: $u-light-color;

    text {
      color: $u-content-color;
    }
  }

  .confirm {
    background: $u-error;

    text {
      color: #fff;
    }
  }
}
</style>
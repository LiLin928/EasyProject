<!-- 文件: PCWeb/src/views/desktop/components/renderers/ListWidget.vue -->

<template>
  <div class="list-widget">
    <div v-if="listData && listData.length > 0" class="list-container">
      <div v-for="item in listData" :key="item.id" class="list-item">
        <div class="item-left">
          <el-tag
            :type="getStatusType(item.status)"
            size="small"
            class="item-status"
          >
            {{ item.statusLabel || '未知' }}
          </el-tag>
          <span class="item-name">{{ item.name }}</span>
        </div>
        <div class="item-right">
          <span class="item-time">{{ item.time || '' }}</span>
        </div>
      </div>
    </div>
    <el-empty v-else description="暂无数据" :image-size="60" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { UserWidgetConfigDto, WidgetDataResponse, WidgetListItem } from '@/types/desktopWidget'

// Props
const props = defineProps<{
  widget: UserWidgetConfigDto
  data?: WidgetDataResponse
}>()

// 列表数据
const listData = computed<WidgetListItem[]>(() => {
  return props.data?.list || []
})

// 根据状态获取标签类型
function getStatusType(status?: number): string {
  if (status === undefined) return 'info'

  const typeMap: Record<number, string> = {
    0: 'info',     // 待审核/待处理
    1: 'warning',  // 进行中
    2: 'success',  // 已完成
    3: 'danger',   // 异常/失败
  }

  return typeMap[status] || 'info'
}
</script>

<style scoped lang="scss">
.list-widget {
  min-height: 150px;

  .list-container {
    .list-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 10px 0;
      border-bottom: 1px solid #ebeef5;

      &:last-child {
        border-bottom: none;
      }

      .item-left {
        display: flex;
        align-items: center;
        gap: 8px;

        .item-status {
          flex-shrink: 0;
        }

        .item-name {
          font-size: 13px;
          color: #303133;
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
        }
      }

      .item-right {
        .item-time {
          font-size: 12px;
          color: #909399;
        }
      }
    }
  }
}
</style>
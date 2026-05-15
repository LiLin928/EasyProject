<!-- 文件: PCWeb/src/views/desktop/components/renderers/ImageWidget.vue -->

<template>
  <div class="image-widget">
    <div v-if="images && images.length > 0" class="image-grid">
      <div v-for="(img, index) in images" :key="index" class="image-item">
        <el-image
          :src="img"
          :preview-src-list="images"
          :initial-index="index"
          fit="cover"
          class="image-content"
          lazy
        >
          <template #error>
            <div class="image-error">
              <el-icon :size="24"><Picture /></el-icon>
              <span>加载失败</span>
            </div>
          </template>
          <template #placeholder>
            <div class="image-loading">
              <el-icon class="is-loading" :size="24"><Loading /></el-icon>
            </div>
          </template>
        </el-image>
      </div>
    </div>
    <el-empty v-else description="暂无图片" :image-size="60" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Picture, Loading } from '@element-plus/icons-vue'
import type { UserWidgetConfigDto, WidgetDataResponse } from '@/types/desktopWidget'

// Props
const props = defineProps<{
  widget: UserWidgetConfigDto
  data?: WidgetDataResponse
}>()

// 图片列表
const images = computed<string[]>(() => {
  return props.data?.images || []
})
</script>

<style scoped lang="scss">
.image-widget {
  min-height: 150px;

  .image-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
    gap: 12px;

    .image-item {
      .image-content {
        width: 100%;
        height: 120px;
        border-radius: 4px;
        cursor: pointer;
      }

      .image-error,
      .image-loading {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        height: 120px;
        background-color: #f5f7fa;
        color: #909399;

        span {
          font-size: 12px;
          margin-top: 4px;
        }
      }
    }
  }
}
</style>
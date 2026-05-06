<!-- src/views/report/components/BreadcrumbNav.vue -->
<template>
  <div class="breadcrumb-nav" v-if="path.length > 0">
    <el-breadcrumb separator="/">
      <el-breadcrumb-item
        v-for="(item, index) in path"
        :key="item.reportId"
        @click="handleClick(index)"
        :class="{ clickable: index < path.length - 1 }"
      >
        <span class="breadcrumb-item">
          {{ item.reportName }}
          <span v-if="Object.keys(item.params).length > 0" class="params">
            ({{ formatParams(item.params) }})
          </span>
        </span>
      </el-breadcrumb-item>
    </el-breadcrumb>
  </div>
</template>

<script setup lang="ts">
import type { DrilldownPath } from '@/types'

const props = defineProps<{
  path: DrilldownPath[]
}>()

const emit = defineEmits<{
  (e: 'navigate', index: number): void
}>()

const handleClick = (index: number) => {
  if (index < props.path.length - 1) {
    emit('navigate', index)
  }
}

const formatParams = (params: Record<string, any>): string => {
  return Object.entries(params)
    .map(([key, value]) => `${key}: ${value}`)
    .join(', ')
}
</script>

<style scoped lang="scss">
.breadcrumb-nav {
  padding: 12px 0;
  margin-bottom: 16px;
  border-bottom: 1px solid #ebeef5;

  .clickable {
    cursor: pointer;

    &:hover {
      color: #409eff;
    }
  }

  .breadcrumb-item {
    .params {
      color: #909399;
      font-size: 12px;
      margin-left: 4px;
    }
  }
}
</style>
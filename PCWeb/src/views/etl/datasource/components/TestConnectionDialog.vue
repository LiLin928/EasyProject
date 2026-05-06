<!-- src/views/etl/datasource/components/TestConnectionDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('etl.datasource.test.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="visible = false">
            {{ t('etl.datasource.test.close') }}
          </el-button>
        </div>
      </div>
    </template>
    <el-result
      :icon="result?.success ? 'success' : 'error'"
      :title="result?.success ? t('etl.datasource.test.success') : t('etl.datasource.test.failed')"
    >
      <template #sub-title>
        <span>{{ result?.message }}</span>
      </template>

      <template #extra>
        <div v-if="result?.details" class="details-section">
          <el-descriptions :column="1" border>
            <el-descriptions-item
              v-if="result.details.responseTime"
              :label="t('etl.datasource.test.responseTime')"
            >
              {{ result.details.responseTime }}ms
            </el-descriptions-item>
            <el-descriptions-item
              v-if="result.details.version"
              :label="t('etl.datasource.test.version')"
            >
              {{ result.details.version }}
            </el-descriptions-item>
            <el-descriptions-item
              v-for="(value, key) in result.details"
              :key="key"
              v-if="key !== 'responseTime' && key !== 'version'"
              :label="key"
            >
              {{ value }}
            </el-descriptions-item>
          </el-descriptions>
        </div>
      </template>
    </el-result>

      </el-dialog>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useLocale } from '@/composables/useLocale'
import type { TestConnectionResult } from '@/types/etl'

const props = defineProps<{
  modelValue: boolean
  result: TestConnectionResult | null
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})
</script>

<style scoped lang="scss">
.dialog-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;

  .dialog-title {
    font-size: 18px;
    font-weight: 500;
    color: #303133;
  }

  .dialog-actions {
    display: flex;
    gap: 8px;
    margin-right: 30px;
  }
}

.details-section {
  margin-top: 16px;
  text-align: left;

  :deep(.el-descriptions) {
    width: 100%;
  }
}
</style>
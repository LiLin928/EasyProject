<!-- src/components/SearchForm/index.vue -->
<template>
  <div class="search-form-container">
    <el-form
      ref="formRef"
      :model="localModel"
      :inline="true"
      :label-width="labelWidth"
      class="search-form"
    >
      <template v-for="item in visibleItems" :key="item.field">
        <el-form-item :label="item.label">
          <!-- Input -->
          <el-input
            v-if="item.type === 'input'"
            v-model="localModel[item.field]"
            :placeholder="item.placeholder || t('common.placeholder.input', { label: item.label })"
            clearable
            @keyup.enter="handleSearch"
            v-bind="item.props"
          />

          <!-- Select -->
          <el-select
            v-else-if="item.type === 'select'"
            v-model="localModel[item.field]"
            :placeholder="item.placeholder || t('common.placeholder.select', { label: item.label })"
            clearable
            style="min-width: 120px"
            v-bind="item.props"
          >
            <el-option
              v-for="option in item.options"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </el-select>

          <!-- Date -->
          <el-date-picker
            v-else-if="item.type === 'date'"
            v-model="localModel[item.field]"
            :placeholder="item.placeholder || t('common.placeholder.select', { label: item.label })"
            clearable
            v-bind="item.props"
          />

          <!-- DateRange -->
          <el-date-picker
            v-else-if="item.type === 'dateRange'"
            v-model="localModel[item.field]"
            type="daterange"
            :range-separator="t('common.date.rangeSeparator')"
            :start-placeholder="t('common.date.startPlaceholder')"
            :end-placeholder="t('common.date.endPlaceholder')"
            clearable
            v-bind="item.props"
          />

          <!-- Cascader -->
          <el-cascader
            v-else-if="item.type === 'cascader'"
            v-model="localModel[item.field]"
            :placeholder="item.placeholder || t('common.placeholder.select', { label: item.label })"
            clearable
            v-bind="item.props"
          />

          <!-- Number -->
          <el-input-number
            v-else-if="item.type === 'number'"
            v-model="localModel[item.field]"
            :placeholder="item.placeholder"
            v-bind="item.props"
          />
        </el-form-item>
      </template>

      <!-- 操作按钮 -->
      <el-form-item class="search-buttons">
        <el-button type="primary" :loading="loading" @click="handleSearch">
          <el-icon><Search /></el-icon>
          {{ t('common.button.search') }}
        </el-button>
        <el-button @click="handleReset">
          <el-icon><Refresh /></el-icon>
          {{ t('common.button.reset') }}
        </el-button>
        <el-button
          v-if="showCollapse && hasCollapseItems"
          link
          type="primary"
          @click="toggleCollapse"
        >
          {{ collapsed ? t('common.button.expand') : t('common.button.collapse') }}
          <el-icon>
            <ArrowDown v-if="collapsed" />
            <ArrowUp v-else />
          </el-icon>
        </el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Search, Refresh, ArrowDown, ArrowUp } from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import type { SearchFormItem, SearchFormProps, SearchFormEmits } from './types'

const props = withDefaults(defineProps<SearchFormProps>(), {
  collapsed: true,
  showCollapse: true,
  collapseCount: 3,
  labelWidth: 'auto',
  loading: false,
})

const emit = defineEmits<SearchFormEmits>()

const { t } = useLocale()

// 本地数据
const formRef = ref()
const localModel = ref<Record<string, any>>({ ...props.modelValue })
const collapsed = ref(props.collapsed)

// 监听外部变化，更新本地数据
watch(() => props.modelValue, (val) => {
  localModel.value = { ...val }
}, { deep: true })

// 监听内部变化，同步回外部（保持 reactive 对象引用）
watch(localModel, (val) => {
  // 仅同步值，不替换整个对象，保持外部 reactive 的引用
  Object.keys(val).forEach(key => {
    if (props.modelValue[key] !== val[key]) {
      props.modelValue[key] = val[key]
    }
  })
}, { deep: true })

// 是否有折叠项
const hasCollapseItems = computed(() => {
  return props.items.some(item => item.collapse)
})

// 可见表单项
const visibleItems = computed(() => {
  if (!collapsed.value) {
    return props.items
  }
  // 折叠时只显示前 collapseCount 个非折叠项
  const nonCollapseItems = props.items.filter(item => !item.collapse)
  return nonCollapseItems.slice(0, props.collapseCount)
})

// 切换折叠状态
const toggleCollapse = () => {
  collapsed.value = !collapsed.value
}

// 搜索
const handleSearch = () => {
  emit('search')
}

// 重置
const handleReset = () => {
  // 重置所有字段为初始值
  const resetData: Record<string, any> = {}
  props.items.forEach(item => {
    resetData[item.field] = item.defaultValue ?? undefined
  })
  localModel.value = resetData
  emit('update:modelValue', resetData)
  emit('reset')
}

// 暴露方法
defineExpose({
  formRef,
  reset: handleReset,
})
</script>

<style scoped lang="scss">
.search-form-container {
  margin-bottom: 16px;

  .search-buttons {
    margin-right: 0;
  }
}
</style>
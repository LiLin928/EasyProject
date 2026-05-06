<!-- src/views/basic/menu/components/IconSelectDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('menu.menuManagement.selectIcon') }}</span>
        <div class="dialog-actions">
          <el-button @click="visible = false">{{ t('common.button.cancel') }}</el-button>
          <el-button type="primary" @click="handleConfirm">{{ t('common.button.ok') }}</el-button>
        </div>
      </div>
    </template>
    <!-- Search input -->
    <el-input
      v-model="searchKeyword"
      :placeholder="t('menu.menuManagement.searchIcon')"
      clearable
      style="margin-bottom: 16px"
    >
      <template #prefix>
        <el-icon><Search /></el-icon>
      </template>
    </el-input>

    <!-- Icons grid -->
    <div class="icon-grid">
      <div
        v-for="icon in filteredIcons"
        :key="icon.name"
        class="icon-item"
        :class="{ selected: selectedIcon === icon.name }"
        @click="selectedIcon = icon.name"
      >
        <el-icon :size="24">
          <component :is="icon.component" />
        </el-icon>
        <span class="icon-name">{{ icon.name }}</span>
      </div>
    </div>

    <!-- Current selection display -->
    <div class="current-selection">
      {{ t('menu.menuManagement.currentSelect') }}:
      <el-icon v-if="selectedIcon" :size="20">
        <component :is="iconComponents[selectedIcon]" />
      </el-icon>
      <span>{{ selectedIcon || '-' }}</span>
    </div>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Search } from '@element-plus/icons-vue'
import * as Icons from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'

const props = defineProps<{
  modelValue: boolean
  currentIcon?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'select', icon: string): void
}>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const searchKeyword = ref('')
const selectedIcon = ref('')

// Build icon list
const iconList = Object.keys(Icons)
  .filter(key => key !== 'Search') // Exclude Search icon used in template
  .map(name => ({
    name,
    component: Icons[name as keyof typeof Icons],
  }))

// Icon components map for rendering
const iconComponents = Icons

// Filter icons by search keyword
const filteredIcons = computed(() => {
  if (!searchKeyword.value) return iconList
  return iconList.filter(icon =>
    icon.name.toLowerCase().includes(searchKeyword.value.toLowerCase())
  )
})

watch(visible, (val) => {
  if (val) {
    selectedIcon.value = props.currentIcon || ''
    searchKeyword.value = ''
  }
})

const handleConfirm = () => {
  emit('select', selectedIcon.value)
  visible.value = false
}

const handleClose = () => {
  searchKeyword.value = ''
}
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

.icon-grid {
  display: grid;
  grid-template-columns: repeat(6, 1fr);
  gap: 8px;
  max-height: 300px;
  overflow-y: auto;
  padding: 8px;
  border: 1px solid var(--el-border-color-lighter);
  border-radius: 4px;
}

.icon-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 8px;
  cursor: pointer;
  border-radius: 4px;
  transition: all 0.2s;

  &:hover {
    background-color: var(--el-fill-color-light);
  }

  &.selected {
    background-color: var(--el-color-primary-light-9);
    border: 1px solid var(--el-color-primary);
  }

  .icon-name {
    font-size: 12px;
    color: var(--el-text-color-secondary);
    margin-top: 4px;
    text-align: center;
    word-break: break-all;
  }
}

.current-selection {
  margin-top: 16px;
  padding: 8px;
  background-color: var(--el-fill-color-light);
  border-radius: 4px;
  display: flex;
  align-items: center;
  gap: 8px;
}
</style>
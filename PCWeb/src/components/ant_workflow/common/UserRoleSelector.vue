<!-- src/components/ant_workflow/common/UserRoleSelector.vue -->
<template>
  <div class="user-role-selector">
    <!-- 已选展示 -->
    <div v-if="selectedItems.length > 0" class="selected-tags">
      <el-tag v-for="item in selectedItems" :key="item.id" closable :type="getTagType(item.type)" @close="handleRemove(item)">
        {{ item.name }}
      </el-tag>
    </div>
    <!-- Tab 分组 -->
    <el-tabs v-model="activeTab" type="card" class="selector-tabs">
      <el-tab-pane v-for="tab in visibleTabs" :key="tab.type" :label="tab.label" :name="tab.type">
        <el-input v-model="searchKeyword" placeholder="搜索" clearable class="search-input">
          <template #prefix><el-icon><Search /></el-icon></template>
        </el-input>
        <div class="options-list" v-loading="loading">
          <el-checkbox-group v-if="mode === 'multiple'" v-model="selectedIds">
            <el-checkbox v-for="item in filteredOptions" :key="item.id" :value="item.id" :label="item.id" class="option-item" @change="handleCheckboxChange(item)">
              <span>{{ item.name }}</span>
              <span v-if="item.deptName" class="dept-name">{{ item.deptName }}</span>
            </el-checkbox>
          </el-checkbox-group>
          <el-radio-group v-else v-model="selectedIds[0]">
            <el-radio v-for="item in filteredOptions" :key="item.id" :value="item.id" class="option-item" @change="handleRadioChange(item)">
              <span>{{ item.name }}</span>
            </el-radio>
          </el-radio-group>
        </div>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Search } from '@element-plus/icons-vue'
import { SelectorTargetType, type SelectorTarget } from '@/types/antWorkflow/selector'
import { getSelectorDataByType } from '@/api/ant_workflow/selectorApi'

const props = withDefaults(defineProps<{
  selected?: SelectorTarget[]
  mode?: 'single' | 'multiple'
  allowedTypes?: SelectorTargetType[]
}>(), { selected: () => [], mode: 'multiple', allowedTypes: () => Object.values(SelectorTargetType) })

const emit = defineEmits<{ (e: 'update', selected: SelectorTarget[]): void }>()

const tabConfig = [
  { type: SelectorTargetType.USER, label: '用户' },
  { type: SelectorTargetType.ROLE, label: '角色' },
  { type: SelectorTargetType.DEPT, label: '部门' },
  { type: SelectorTargetType.POST, label: '岗位' },
  { type: SelectorTargetType.FORM_FIELD, label: '表单字段' },
]

const visibleTabs = computed(() => tabConfig.filter(tab => props.allowedTypes.includes(tab.type)))
const activeTab = ref(visibleTabs.value[0]?.type || SelectorTargetType.USER)
const searchKeyword = ref('')
const selectedItems = ref<SelectorTarget[]>([])
const selectedIds = ref<string[]>([])
const loading = ref(false)
const currentOptions = ref<SelectorTarget[]>([])

// 加载当前 Tab 的数据
const loadCurrentOptions = async () => {
  loading.value = true
  try {
    currentOptions.value = await getSelectorDataByType(activeTab.value)
  } catch (error) {
    currentOptions.value = []
  } finally {
    loading.value = false
  }
}

// 监听 activeTab 变化，重新加载数据
watch(activeTab, () => {
  loadCurrentOptions()
}, { immediate: true })

// 监听 props.selected 变化
watch(() => props.selected, (val) => {
  selectedItems.value = val || []
  selectedIds.value = selectedItems.value.map(item => item.id)
}, { immediate: true })

const filteredOptions = computed(() =>
  searchKeyword.value
    ? currentOptions.value.filter(item => item.name.includes(searchKeyword.value))
    : currentOptions.value
)

const getTagType = (type: SelectorTargetType) =>
  type === SelectorTargetType.USER ? 'success'
  : type === SelectorTargetType.ROLE ? 'primary'
  : type === SelectorTargetType.DEPT ? 'warning'
  : 'info'

const handleRemove = (item: SelectorTarget) => {
  selectedItems.value = selectedItems.value.filter(i => i.id !== item.id)
  selectedIds.value = selectedIds.value.filter(id => id !== item.id)
  emit('update', selectedItems.value)
}

const handleCheckboxChange = (item: SelectorTarget) => {
  const isSelected = selectedIds.value.includes(item.id)
  if (isSelected) {
    // 添加到已选列表（避免重复）
    if (!selectedItems.value.find(i => i.id === item.id)) {
      selectedItems.value.push(item)
    }
  } else {
    // 从已选列表移除
    selectedItems.value = selectedItems.value.filter(i => i.id !== item.id)
  }
  emit('update', selectedItems.value)
}

const handleRadioChange = (item: SelectorTarget) => {
  selectedItems.value = [item]
  selectedIds.value = [item.id]
  emit('update', [item])
}
</script>

<style scoped lang="scss">
.user-role-selector {
  .selected-tags { margin-bottom: 12px; display: flex; flex-wrap: wrap; gap: 8px; }
  .selector-tabs :deep(.el-tabs__content) { padding: 12px 0; }
  .search-input { margin-bottom: 12px; }
  .options-list { max-height: 200px; overflow-y: auto;
    .option-item { margin-bottom: 8px;
      .dept-name { color: #909399; font-size: 12px; margin-left: 8px; }
    }
  }
}
</style>
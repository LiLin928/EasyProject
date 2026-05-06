<!-- src/views/buz/product/components/CategoryTree.vue -->
<template>
  <div class="category-tree">
    <div class="tree-header">
      <span>{{ t('product.list.category') }}</span>
      <el-button type="primary" link size="small" @click="handleManageCategory">
        {{ t('product.list.manageCategory') }}
      </el-button>
    </div>
    <el-tree
      ref="treeRef"
      :data="treeData"
      :props="treeProps"
      :highlight-current="true"
      node-key="id"
      default-expand-all
      @node-click="handleNodeClick"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, type TreeInstance } from 'element-plus'
import { getCategoryList } from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { Category } from '@/types/product'

const { t } = useLocale()
const router = useRouter()

const treeRef = ref<TreeInstance>()
const loading = ref(false)
const categories = ref<Category[]>([])

// Tree configuration
const treeProps = {
  label: 'name',
  children: 'children',
}

// All Categories node as first item, followed by actual categories
const treeData = ref<TreeNode[]>([])

interface TreeNode {
  id: string | null
  name: string
  children?: TreeNode[]
}

// Emits
const emit = defineEmits<{
  (e: 'select', categoryId: string | null): void
  (e: 'loaded', categories: Category[]): void
}>()

// Load categories from API
const loadCategories = async () => {
  loading.value = true
  try {
    const data = await getCategoryList()
    categories.value = data

    // Build tree data with "All Categories" as first node
    const allCategoryNode: TreeNode = {
      id: null,
      name: t('product.list.allCategory'),
    }
    const categoryNodes: TreeNode[] = data.map(cat => ({
      id: cat.id,
      name: cat.name,
    }))
    treeData.value = [allCategoryNode, ...categoryNodes]

    // Set "All Categories" as default selected
    setTimeout(() => {
      treeRef.value?.setCurrentKey(null)
    }, 0)

    // Emit loaded categories for parent component
    emit('loaded', data)
  } catch (error) {
    ElMessage.error(t('common.message.failed'))
  } finally {
    loading.value = false
  }
}

// Handle node click
const handleNodeClick = (data: TreeNode) => {
  emit('select', data.id)
}

// Navigate to category management page
const handleManageCategory = () => {
  router.push('/buz/product/category')
}

// Expose refresh method
const refresh = () => {
  loadCategories()
}

defineExpose({
  refresh,
})

onMounted(() => {
  loadCategories()
})
</script>

<style scoped lang="scss">
.category-tree {
  height: 100%;
  display: flex;
  flex-direction: column;
  background-color: var(--el-bg-color);
  border-radius: 4px;
  overflow: hidden;

  .tree-header {
    padding: 16px;
    border-bottom: 1px solid var(--el-border-color-lighter);
    font-weight: 500;
    font-size: 14px;
    color: var(--el-text-color-primary);
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .el-tree {
    flex: 1;
    overflow-y: auto;
    padding: 8px 0;
  }
}
</style>
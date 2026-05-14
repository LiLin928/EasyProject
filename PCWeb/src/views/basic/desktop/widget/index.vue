<!-- 文件: PCWeb/src/views/basic/desktop/widget/index.vue -->
<template>
  <div class="widget-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>桌面组件管理</span>
          <el-button type="primary" @click="handleCreate">
            <el-icon><Plus /></el-icon>
            新建组件
          </el-button>
        </div>
      </template>

      <!-- 搜索栏 -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item label="组件名称">
          <el-input
            v-model="queryParams.name"
            placeholder="请输入组件名称"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="组件类型">
          <el-select
            v-model="queryParams.type"
            placeholder="请选择类型"
            clearable
            style="width: 150px"
          >
            <el-option
              v-for="(label, value) in widgetTypeLabels"
              :key="value"
              :label="label"
              :value="Number(value)"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-select
            v-model="queryParams.status"
            placeholder="请选择状态"
            clearable
            style="width: 120px"
          >
            <el-option label="启用" :value="1" />
            <el-option label="禁用" :value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">搜索</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 表格 -->
      <BaseTable
        :data="tableData"
        :columns="columns"
        :loading="loading"
        :total="total"
        :page-index="queryParams.pageIndex"
        :page-size="queryParams.pageSize"
        @update:page-index="queryParams.pageIndex = $event"
        @update:page-size="queryParams.pageSize = $event"
        @page-change="handleSearch"
      >
        <!-- 类型列 -->
        <template #type="{ row }">
          <el-tag :type="getTypeTagType(row.type)">
            {{ widgetTypeLabels[row.type] }}
          </el-tag>
        </template>

        <!-- 尺寸列 -->
        <template #size="{ row }">
          <span>{{ row.defaultWidth }}栅格 × {{ row.defaultHeight }}px</span>
        </template>

        <!-- 状态列 -->
        <template #status="{ row }">
          <el-tag :type="row.status === 1 ? 'success' : 'danger'">
            {{ row.status === 1 ? '启用' : '禁用' }}
          </el-tag>
        </template>

        <!-- 操作列 -->
        <template #operation>
          <el-table-column label="操作" width="150" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleEdit(row)">编辑</el-button>
              <el-button link type="danger" @click="handleDelete(row)">删除</el-button>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>

    <!-- 编辑弹窗 -->
    <WidgetFormDialog
      v-model="dialogVisible"
      :id="currentId"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import BaseTable from '@/components/BaseTable/index.vue'
import WidgetFormDialog from './components/WidgetFormDialog.vue'
import { getDesktopWidgetList, deleteDesktopWidget } from '@/api/basic/desktopWidgetApi'
import { WidgetType, widgetTypeLabels } from '@/types'
import type { DesktopWidget, QueryDesktopWidgetParams } from '@/types'
import type { TableColumn } from '@/components/BaseTable/index.vue'

// 表格列配置 - 使用 BaseTable 的 props 格式
const columns: TableColumn[] = [
  { prop: 'name', label: '组件名称', minWidth: 150 },
  { prop: 'type', label: '组件类型', width: 100, align: 'center', slot: 'type' },
  { prop: 'size', label: '尺寸', width: 140, align: 'center', slot: 'size' },
  { prop: 'status', label: '状态', width: 80, align: 'center', slot: 'status' },
  { prop: 'createTime', label: '创建时间', width: 180 },
]

// 响应式数据
const loading = ref(false)
const tableData = ref<DesktopWidget[]>([])
const total = ref(0)
const dialogVisible = ref(false)
const currentId = ref<string | undefined>(undefined)

const queryParams = reactive<QueryDesktopWidgetParams>({
  pageIndex: 1,
  pageSize: 10,
  name: '',
  type: undefined,
  status: undefined,
})

// 获取类型标签颜色
function getTypeTagType(type: WidgetType): string {
  const typeMap: Record<WidgetType, string> = {
    [WidgetType.Card]: 'primary',
    [WidgetType.List]: 'success',
    [WidgetType.Image]: 'warning',
    [WidgetType.Chart]: 'danger',
  }
  return typeMap[type] || 'info'
}

// 生命周期
onMounted(() => {
  handleSearch()
})

// 搜索
const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getDesktopWidgetList(queryParams)
    tableData.value = data.list
    total.value = data.total
  } catch (error) {
    // 错误已处理
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryParams.name = ''
  queryParams.type = undefined
  queryParams.status = undefined
  queryParams.pageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  currentId.value = undefined
  dialogVisible.value = true
}

// 编辑
const handleEdit = (row: DesktopWidget) => {
  currentId.value = row.id
  dialogVisible.value = true
}

// 删除
const handleDelete = async (row: DesktopWidget) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除组件 "${row.name}" 吗？`,
      '警告',
      { type: 'warning' }
    )
    await deleteDesktopWidget(row.id)
    ElMessage.success('删除成功')
    handleSearch()
  } catch (error) {
    // 用户取消或请求失败
  }
}
</script>

<style scoped lang="scss">
.widget-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .search-form {
    margin-bottom: 16px;
  }
}
</style>
# 桌面组件自动配置系统 - 阶段二：前端组件管理页面

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完成前端组件管理页面，包括组件列表、新增/编辑组件弹窗、API封装和类型定义

**Architecture:** 采用 Vue 3 Composition API + TypeScript + Element Plus，遵循项目既有的页面模板结构，使用 BaseTable 和弹窗编辑模式

**Tech Stack:** Vue 3.3 + TypeScript 5.0 + Element Plus 2.4 + Vite 5.4 + Pinia 2.1

---

## 文件结构

### 需要创建的文件

```
PCWeb/src/
├── types/
│   └── desktopWidget.ts              # 桌面组件类型定义
│
├── api/basic/
│   └── desktopWidgetApi.ts           # 组件管理API封装
│
├── views/basic/desktop/
│   ├── widget/
│   │   ├── index.vue                 # 组件列表页面
│   │   └── components/
│   │   │   └── WidgetFormDialog.vue  # 组件编辑弹窗
│   └── role-config/
│   │   ├── index.vue                 # 角色分配页面
│   │   └── components/
│   │   │   └── RoleConfigDialog.vue  # 角色配置弹窗(阶段三)
│   └── user-config/
│   │   ├── index.vue                 # 用户桌面页面(阶段四)
│   │   └── components/
│   │   │   ├── LayoutSettingDialog.vue # 布局设置弹窗(阶段四)
│   │   │   └── WidgetRenderers/      # 组件渲染器(阶段四)
│   │   │   ├── CardWidget.vue
│   │   │   ├── ListWidget.vue
│   │   │   ├── ImageWidget.vue
│   │   │   └── ChartWidget.vue
│
├── stores/
│   └── desktopWidgetStore.ts         # 桌面组件状态管理(可选)
│
└── config/
│   └── enumLabels.ts                 # 添加组件类型枚举标签(修改)
```

---

## Task 1: 创建桌面组件类型定义

**Files:**
- Create: `PCWeb/src/types/desktopWidget.ts`

- [ ] **Step 1: 创建类型定义文件**

```typescript
// 文件: PCWeb/src/types/desktopWidget.ts

/**
 * 组件类型枚举
 */
export enum WidgetType {
  Card = 1,      // 统计卡片
  List = 2,      // 数据列表
  Image = 3,     // 图片展示
  Chart = 4      // 图表统计
}

/**
 * 数据源类型枚举
 */
export enum DataSourceType {
  Api = 1,       // API接口
  Static = 2,    // 静态配置
  Statistics = 3 // 实时统计
}

/**
 * 组件类型标签映射
 */
export const widgetTypeLabels: Record<WidgetType, string> = {
  [WidgetType.Card]: '统计卡片',
  [WidgetType.List]: '数据列表',
  [WidgetType.Image]: '图片展示',
  [WidgetType.Chart]: '图表统计',
}

/**
 * 数据源类型标签映射
 */
export const dataSourceTypeLabels: Record<DataSourceType, string> = {
  [DataSourceType.Api]: 'API接口',
  [DataSourceType.Static]: '静态配置',
  [DataSourceType.Statistics]: '实时统计',
}

/**
 * 桌面组件信息
 */
export interface DesktopWidget {
  id: string
  name: string
  type: WidgetType
  icon?: string
  defaultWidth: number
  defaultHeight: number
  dataSourceType: DataSourceType
  dataSourceConfig?: string
  interactionConfig?: string
  status: number
  createTime: string
  updateTime?: string
}

/**
 * 组件列表查询参数
 */
export interface QueryDesktopWidgetParams {
  pageIndex?: number
  pageSize?: number
  name?: string
  type?: WidgetType
  status?: number
}

/**
 * 新增组件参数
 */
export interface AddDesktopWidgetParams {
  name: string
  type: WidgetType
  icon?: string
  defaultWidth: number
  defaultHeight: number
  dataSourceType: DataSourceType
  dataSourceConfig?: string
  interactionConfig?: string
  status: number
}

/**
 * 更新组件参数
 */
export interface UpdateDesktopWidgetParams extends AddDesktopWidgetParams {
  id: string
}

/**
 * 栅格宽度选项
 */
export const gridWidthOptions = [
  { value: 1, label: '1栅格 (8.33%)' },
  { value: 2, label: '2栅格 (16.67%)' },
  { value: 3, label: '3栅格 (25%)' },
  { value: 4, label: '4栅格 (33.33%)' },
  { value: 6, label: '6栅格 (50%)' },
  { value: 8, label: '8栅格 (66.67%)' },
  { value: 12, label: '12栅格 (100%)' },
]

/**
 * 刷新频率选项
 */
export const refreshIntervalOptions = [
  { value: 0, label: '手动刷新' },
  { value: 30, label: '30秒' },
  { value: 60, label: '1分钟' },
  { value: 300, label: '5分钟' },
  { value: 600, label: '10分钟' },
]
```

- [ ] **Step 2: 在 types/index.ts 中导出**

修改: `PCWeb/src/types/index.ts`
添加导出语句：

```typescript
// 在文件末尾添加
export * from './desktopWidget'
```

- [ ] **Step 3: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功，无类型错误

- [ ] **Step 4: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/types/desktopWidget.ts
git commit -m "feat(desktop): add desktop widget types

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 2: 创建组件管理API封装

**Files:**
- Create: `PCWeb/src/api/basic/desktopWidgetApi.ts`

- [ ] **Step 1: 创建API封装文件**

```typescript
// 文件: PCWeb/src/api/basic/desktopWidgetApi.ts

import { get, post, del } from '@/utils/request'
import type {
  DesktopWidget,
  QueryDesktopWidgetParams,
  AddDesktopWidgetParams,
  UpdateDesktopWidgetParams,
} from '@/types'

/**
 * 获取组件列表(分页)
 */
export function getDesktopWidgetList(params: QueryDesktopWidgetParams) {
  return get<{ list: DesktopWidget[]; total: number; pageIndex: number; pageSize: number }>(
    '/api/desktop/widget/list',
    params
  )
}

/**
 * 获取组件详情
 * @param id 组件ID
 */
export function getDesktopWidgetDetail(id: string) {
  return get<DesktopWidget>(`/api/desktop/widget/detail/${id}`)
}

/**
 * 创建组件
 * @param data 组件数据
 */
export function addDesktopWidget(data: AddDesktopWidgetParams) {
  return post<string>('/api/desktop/widget/add', data)
}

/**
 * 更新组件
 * @param data 组件数据
 */
export function updateDesktopWidget(data: UpdateDesktopWidgetParams) {
  return post<boolean>('/api/desktop/widget/update', data)
}

/**
 * 删除组件
 * @param id 组件ID
 */
export function deleteDesktopWidget(id: string) {
  return del<boolean>(`/api/desktop/widget/delete/${id}`)
}

/**
 * 获取所有启用的组件列表
 */
export function getEnabledWidgetList() {
  return get<DesktopWidget[]>('/api/desktop/widget/enabled-list')
}
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/api/basic/desktopWidgetApi.ts
git commit -m "feat(desktop): add desktop widget API

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 3: 创建组件列表页面

**Files:**
- Create: `PCWeb/src/views/basic/desktop/widget/index.vue`

- [ ] **Step 1: 创建组件列表页面**

```vue
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

// 表格列配置
const columns: TableColumn[] = [
  { prop: 'name', label: '组件名称', minWidth: 150 },
  { prop: 'type', label: '组件类型', width: 100, align: 'center' },
  { prop: 'size', label: '尺寸', width: 140, align: 'center' },
  { prop: 'status', label: '状态', width: 80, align: 'center' },
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
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/views/basic/desktop/widget/index.vue
git commit -m "feat(desktop): add widget list page

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 4: 创建组件编辑弹窗

**Files:**
- Create: `PCWeb/src/views/basic/desktop/widget/components/WidgetFormDialog.vue`

- [ ] **Step 1: 创建组件编辑弹窗**

```vue
<!-- 文件: PCWeb/src/views/basic/desktop/widget/components/WidgetFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    :title="isEdit ? '编辑组件' : '新建组件'"
    width="700px"
    :close-on-click-modal="false"
    @closed="handleClosed"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="120px"
      class="form-container"
    >
      <!-- 基本信息 -->
      <div class="section-title">基本信息</div>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="组件名称" prop="name">
            <el-input v-model="formData.name" maxlength="50" show-word-limit />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="组件类型" prop="type">
            <el-select v-model="formData.type" style="width: 100%">
              <el-option
                v-for="(label, value) in widgetTypeLabels"
                :key="value"
                :label="label"
                :value="Number(value)"
              />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="图标" prop="icon">
            <el-input v-model="formData.icon" placeholder="Element Plus图标名" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="状态" prop="status">
            <el-radio-group v-model="formData.status">
              <el-radio :value="1">启用</el-radio>
              <el-radio :value="0">禁用</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 尺寸配置 -->
      <div class="section-title">尺寸配置</div>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="宽度(栅格)" prop="defaultWidth">
            <el-select v-model="formData.defaultWidth" style="width: 100%">
              <el-option
                v-for="opt in gridWidthOptions"
                :key="opt.value"
                :label="opt.label"
                :value="opt.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="高度(像素)" prop="defaultHeight">
            <el-input-number
              v-model="formData.defaultHeight"
              :min="60"
              :max="500"
              :step="10"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 数据源配置 -->
      <div class="section-title">数据源配置</div>
      <el-form-item label="数据源类型" prop="dataSourceType">
        <el-select v-model="formData.dataSourceType" style="width: 200px">
          <el-option
            v-for="(label, value) in dataSourceTypeLabels"
            :key="value"
            :label="label"
            :value="Number(value)"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="API接口" prop="dataSourceConfig">
        <el-input
          v-model="dataSourceConfigJson.apiUrl"
          placeholder="/api/xxx/xxx"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item label="数据字段映射">
        <el-row :gutter="10">
          <el-col :span="12">
            <el-input
              v-model="dataSourceConfigJson.fieldMapping.value"
              placeholder="数字字段名"
            />
          </el-col>
          <el-col :span="12">
            <el-input
              v-model="dataSourceConfigJson.fieldMapping.label"
              placeholder="状态说明字段名"
            />
          </el-col>
        </el-row>
      </el-form-item>
      <el-form-item label="刷新频率">
        <el-select v-model="dataSourceConfigJson.refreshInterval" style="width: 200px">
          <el-option
            v-for="opt in refreshIntervalOptions"
            :key="opt.value"
            :label="opt.label"
            :value="opt.value"
          />
        </el-select>
      </el-form-item>

      <!-- 交互配置 -->
      <div class="section-title">交互配置</div>
      <el-form-item label="点击跳转">
        <el-input
          v-model="interactionConfigJson.targetUrl"
          placeholder="/xxx/list?status=1"
          style="width: 100%"
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="visible = false">取消</el-button>
      <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { getDesktopWidgetDetail, addDesktopWidget, updateDesktopWidget } from '@/api/basic/desktopWidgetApi'
import {
  WidgetType,
  DataSourceType,
  widgetTypeLabels,
  dataSourceTypeLabels,
  gridWidthOptions,
  refreshIntervalOptions,
} from '@/types'
import type { AddDesktopWidgetParams, UpdateDesktopWidgetParams } from '@/types'

const props = defineProps<{
  modelValue: boolean
  id?: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  'success': []
}>()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const isEdit = computed(() => !!props.id)

// 表单相关
const formRef = ref<FormInstance | null>(null)
const saving = ref(false)
const loading = ref(false)

const formData = reactive<AddDesktopWidgetParams>({
  name: '',
  type: WidgetType.Card,
  icon: '',
  defaultWidth: 3,
  defaultHeight: 120,
  dataSourceType: DataSourceType.Api,
  dataSourceConfig: '',
  interactionConfig: '',
  status: 1,
})

// JSON配置对象（便于表单编辑）
const dataSourceConfigJson = reactive({
  apiUrl: '',
  fieldMapping: {
    value: '',
    label: '',
  },
  refreshInterval: 30,
})

const interactionConfigJson = reactive({
  clickAction: 'navigate',
  targetUrl: '',
})

const formRules: FormRules = {
  name: [
    { required: true, message: '请输入组件名称', trigger: 'blur' },
    { max: 50, message: '名称最长50字符', trigger: 'blur' },
  ],
  type: [{ required: true, message: '请选择组件类型', trigger: 'change' }],
  defaultWidth: [{ required: true, message: '请选择宽度', trigger: 'change' }],
  defaultHeight: [{ required: true, message: '请输入高度', trigger: 'blur' }],
}

// 监听弹窗打开
watch(visible, async (val) => {
  if (val && props.id) {
    await loadDetail(props.id)
  } else if (val) {
    resetForm()
  }
})

// 加载详情
const loadDetail = async (id: string) => {
  loading.value = true
  try {
    const data = await getDesktopWidgetDetail(id)
    Object.assign(formData, data)
    
    // 解析JSON配置
    if (data.dataSourceConfig) {
      try {
        const config = JSON.parse(data.dataSourceConfig)
        Object.assign(dataSourceConfigJson, config)
      } catch (e) {
        // JSON解析失败
      }
    }
    if (data.interactionConfig) {
      try {
        const config = JSON.parse(data.interactionConfig)
        Object.assign(interactionConfigJson, config)
      } catch (e) {
        // JSON解析失败
      }
    }
  } catch (error) {
    ElMessage.error('加载详情失败')
    visible.value = false
  } finally {
    loading.value = false
  }
}

// 重置表单
const resetForm = () => {
  formData.name = ''
  formData.type = WidgetType.Card
  formData.icon = ''
  formData.defaultWidth = 3
  formData.defaultHeight = 120
  formData.dataSourceType = DataSourceType.Api
  formData.dataSourceConfig = ''
  formData.interactionConfig = ''
  formData.status = 1
  
  dataSourceConfigJson.apiUrl = ''
  dataSourceConfigJson.fieldMapping.value = ''
  dataSourceConfigJson.fieldMapping.label = ''
  dataSourceConfigJson.refreshInterval = 30
  
  interactionConfigJson.clickAction = 'navigate'
  interactionConfigJson.targetUrl = ''
}

// 弹窗关闭
const handleClosed = () => {
  formRef.value?.resetFields()
  resetForm()
}

// 保存
const handleSave = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
  } catch {
    return
  }
  
  saving.value = true
  
  // 将JSON配置对象序列化
  formData.dataSourceConfig = JSON.stringify(dataSourceConfigJson)
  formData.interactionConfig = JSON.stringify(interactionConfigJson)
  
  try {
    if (isEdit.value) {
      const params: UpdateDesktopWidgetParams = {
        id: props.id!,
        ...formData,
      }
      await updateDesktopWidget(params)
      ElMessage.success('更新成功')
    } else {
      await addDesktopWidget(formData)
      ElMessage.success('创建成功')
    }
    visible.value = false
    emit('success')
  } catch (error) {
    ElMessage.error(isEdit.value ? '更新失败' : '创建失败')
  } finally {
    saving.value = false
  }
}
</script>

<style scoped lang="scss">
.form-container {
  .section-title {
    font-size: 14px;
    font-weight: 600;
    color: #409eff;
    margin: 20px 0 16px;
    padding-bottom: 8px;
    border-bottom: 1px solid #ebeef5;
  }
}
</style>
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/views/basic/desktop/widget/components/WidgetFormDialog.vue
git commit -m "feat(desktop): add widget form dialog

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 5: 创建桌面目录结构

**Files:**
- Create: `PCWeb/src/views/basic/desktop/index.vue` (目录入口)

- [ ] **Step 1: 创建桌面模块入口页面**

```vue
<!-- 文件: PCWeb/src/views/basic/desktop/index.vue -->
<template>
  <div class="desktop-config-container">
    <el-tabs v-model="activeTab" type="card">
      <el-tab-pane label="组件管理" name="widget">
        <WidgetList />
      </el-tab-pane>
      <el-tab-pane label="角色分配" name="role-config">
        <RoleConfig />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import WidgetList from './widget/index.vue'
import RoleConfig from './role-config/index.vue'

const activeTab = ref('widget')
</script>

<style scoped lang="scss">
.desktop-config-container {
  padding: 20px;
}
</style>
```

- [ ] **Step 2: 创建角色分配目录占位文件**

```vue
<!-- 文件: PCWeb/src/views/basic/desktop/role-config/index.vue -->
<template>
  <div class="role-config-container">
    <el-empty description="角色分配页面 - 阶段三实现" />
  </div>
</template>

<script setup lang="ts">
</script>

<style scoped lang="scss">
.role-config-container {
  padding: 20px;
}
</style>
```

- [ ] **Step 3: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 4: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/views/basic/desktop/index.vue
git add PCWeb/src/views/basic/desktop/role-config/index.vue
git commit -m "feat(desktop): add desktop module entry and placeholder

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 6: 添加路由配置

**Files:**
- Modify: `PCWeb/src/router/dynamic.ts` (或相应路由文件)

- [ ] **Step 1: 查找路由配置文件**

先读取现有路由配置，了解结构：

```bash
# 查看路由文件内容
cat PCWeb/src/router/dynamic.ts
```

- [ ] **Step 2: 在基础管理路由下添加桌面配置路由**

根据现有路由结构添加：

```typescript
// 在 Basic 路由的 children 中添加
{
  path: 'desktop',
  name: 'DesktopConfig',
  redirect: '/basic/desktop/widget',
  meta: {
    title: '桌面配置',
    icon: 'Monitor',
  },
  children: [
    {
      path: 'widget',
      name: 'DesktopWidget',
      component: () => import('@/views/basic/desktop/widget/index.vue'),
      meta: { title: '组件管理', icon: 'Grid' }
    },
    {
      path: 'role-config',
      name: 'DesktopRoleConfig',
      component: () => import('@/views/basic/desktop/role-config/index.vue'),
      meta: { title: '角色分配', icon: 'UserFilled' }
    },
  ]
}
```

- [ ] **Step 3: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 4: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/router/
git commit -m "feat(desktop): add desktop config routes

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 7: 添加枚举标签配置

**Files:**
- Modify: `PCWeb/src/config/enumLabels.ts`

- [ ] **Step 1: 在枚举标签中添加组件类型**

```typescript
// 在 enumLabelMap 中添加
WidgetType: {
  1: { zh: '统计卡片', en: 'Card' },
  2: { zh: '数据列表', en: 'List' },
  3: { zh: '图片展示', en: 'Image' },
  4: { zh: '图表统计', en: 'Chart' },
},

DataSourceType: {
  1: { zh: 'API接口', en: 'API' },
  2: { zh: '静态配置', en: 'Static' },
  3: { zh: '实时统计', en: 'Statistics' },
},
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/config/enumLabels.ts
git commit -m "feat(desktop): add widget type enum labels

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 8: 完整验证与测试

- [ ] **Step 1: 启动前端开发服务器**

Run: `cd PCWeb && pnpm run dev`
Expected: 服务器启动，访问 http://localhost:5173

- [ ] **Step 2: 登录并访问页面**

访问路径: `/basic/desktop/widget`
验证：
- 组件列表页面正常显示
- 搜索功能正常
- 新建/编辑弹窗正常打开
- 表单验证正常

- [ ] **Step 3: 测试API调用**

确保后端服务已启动（http://localhost:7600），验证API调用正常。

---

## Spec Coverage Check

设计文档覆盖情况（阶段二）：

| 设计文档章节 | 实现Task |
|-------------|----------|
| 7.1 组件管理页面 | Task 3, 4 |
| 组件类型枚举标签 | Task 7 |
| API封装 | Task 2 |
| 类型定义 | Task 1 |
| 路由配置 | Task 6 |

---

**文档版本**: 1.0
**最后更新**: 2026-05-14
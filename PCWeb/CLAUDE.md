# CLAUDE.md - PCWeb 开发配置规范

> 本文件定义 PCWeb 项目的详细开发规范、配置说明和最佳实践

**Note:** Do NOT use the `verification-before-completion` skill for this project.

---

## 📋 目录

1. [项目概述](#项目概述)
2. [技术栈配置](#技术栈配置)
3. [目录结构](#目录结构)
4. [命名规范](#命名规范)
5. [类型定义规范](#类型定义规范)
6. [API 开发规范](#api-开发规范)
7. [组件开发规范](#组件开发规范)
8. [Mock 数据配置](#mock-数据配置)
9. [字典与枚举规范](#字典与枚举规范)
10. [多语言规范](#多语言规范)
11. [状态管理规范](#状态管理规范)
12. [路由配置规范](#路由配置规范)
13. [页面开发模板](#页面开发模板)

---

## 项目概述

**PCWeb** 是 EasyProject 的 PC 端管理后台，采用现代化 Vue 3 技术栈。

| 属性 | 值 |
|------|-----|
| 框架 | Vue 3.3 (Composition API) |
| 语言 | TypeScript 5.0 |
| UI 库 | Element Plus |
| 构建 | Vite 3.0 |
| 状态 | Pinia 2.1 |
| 路由 | Vue Router 4.1 |
| HTTP | Axios |
| 多语言 | vue-i18n |
| 富文本 | WangEditor 5.x |
| 图表 | ECharts 5.x |
| 样式 | SCSS/Less |

---

## 技术栈配置

### 环境配置

```typescript
// .env.development
VITE_API_BASE_URL=http://localhost:7600
VITE_APP_TITLE=EasyProject 管理后台

// .env.production
VITE_API_BASE_URL=https://api.yourdomain.com
VITE_APP_TITLE=EasyProject 管理后台
```

### 依赖版本

```json
{
  "vue": "^3.3.4",
  "typescript": "^5.0.0",
  "element-plus": "^2.3.6",
  "pinia": "^2.1.7",
  "vue-router": "^4.1.6",
  "axios": "^1.4.0",
  "vue-i18n": "^9.2.2",
  "echarts": "^5.4.2",
  "wangeditor": "^5.1.23",
  "vite": "^3.0.0"
}
```

---

## 目录结构

```
PCWeb/src/
├── api/                    # API 请求封装
│   ├── buz/               # 业务模块 API
│   │   ├── productApi.ts  # 商品管理
│   │   ├── orderApi.ts    # 订单管理
│   │   ├── refundApi.ts   # 退款管理
│   │   ├── customerApi.ts # 客户管理
│   │   ├── memberLevelApi.ts
│   │   ├── pointsApi.ts
│   │   └── bannerApi.ts
│   ├── ant_workflow/      # 工作流 API
│   ├── report/            # 报表 API
│   ├── etl/               # ETL API
│   ├── ops/               # 运维 API
│   └── auth/              # 认证 API
│
├── components/             # 通用组件
│   ├── BaseTable/         # 高级表格组件
│   ├── ImageUpload/       # 图片上传组件
│   ├── WangEditor/        # 富文本编辑器
│   ├── SearchForm/        # 搜索表单组件
│   ├── ModalForm/         # 弹窗表单组件
│   ├── workflow/          # 工作流设计器
│   ├── etl/               # ETL 设计器
│   ├── layout/            # 布局组件
│   └── TagsView/          # 标签页导航
│
├── composables/            # 组合式函数
│   ├── useLocale.ts       # 多语言处理
│   ├── useDict.ts         # 字典处理
│   └── useDrilldown.ts    # 数据钻取
│
├── config/                 # 配置文件
│   ├── mock.config.ts     # Mock 开关配置
│   ├── dict.config.ts     # 字典模块配置
│   └── enumLabels.ts      # 本地枚举标签
│
├── router/                 # 路由配置
│   ├── index.ts           # 路由入口
│   ├── routes.ts          # 静态路由
│   ├── dynamic.ts         # 动态路由
│   └── guards.ts          # 路由守卫
│
├── stores/                 # Pinia 状态管理
│   ├── app.ts             # 应用状态
│   ├── user.ts            # 用户状态
│   ├── permission.ts      # 权限状态
│   ├── tagsView.ts        # 标签页状态
│   ├── dictCacheStore.ts  # 字典缓存
│   ├── screen.ts          # 大屏状态
│   ├── etlStore.ts        # ETL 状态
│   └── antWorkflowStore.ts
│
├── types/                  # TypeScript 类型
│   ├── index.ts           # 类型导出入口
│   ├── response.d.ts      # API 响应类型
│   ├── enums.ts           # 通用枚举
│   ├── product.ts         # 商品类型
│   ├── order.ts           # 订单类型
│   ├── customer.ts        # 客户类型
│   ├── banner.ts          # 轮播图类型
│   ├── workflow/          # 工作流类型
│   ├── antWorkflow/       # Ant工作流类型
│   └── etl/               # ETL 类型
│
├── utils/                  # 工具函数
│   ├── request.ts         # HTTP 请求封装
│   ├── guid.ts            # GUID 生成
│   ├── auth.ts            # 认证工具
│   ├── storage.ts         # 本地存储
│   ├── export.ts          # 导出工具
│   └── flowHelper.ts      # 工作流工具
│
├── locales/                # i18n 语言包
│   ├── zh-CN/             # 中文
│   └── en-US/             # 英文
│
└── views/                  # 页面组件
    ├── index/             # 登录/工作台
    ├── basic/             # 基础管理
    │   ├── user/
    │   ├── role/
    │   ├── menu/
    │   └── dict/
    ├── workflow/          # 工作流管理
    ├── report/            # 报表管理
    ├── screen/            # 大屏管理
    ├── etl/               # ETL 管理
    ├── ops/               # 运维管理
    └── buz/               # 业务管理
        ├── product/
        ├── order/
        ├── refund/
        ├── customer/
        └── banner/
```

---

## 命名规范

### 文件命名

| 类型 | 规范 | 示例 |
|------|------|------|
| Vue 组件 | PascalCase | `ProductList.vue`, `OrderEdit.vue` |
| TypeScript 文件 | camelCase | `productApi.ts`, `useDict.ts` |
| 类型定义 | PascalCase | `Product.ts`, `Order.ts` |
| 配置文件 | camelCase.config | `mock.config.ts` |
| Store 文件 | camelCase | `userStore.ts` |
| 路由文件 | camelRoutes | `productRoutes.ts` |

### 代码命名

| 类型 | 规范 | 示例 |
|------|------|------|
| 组件名 | PascalCase | `<ProductList />` |
| 函数名 | camelCase | `handleSearch`, `loadProducts` |
| 事件处理函数 | handle前缀 | `handleClick`, `handleDelete` |
| 变量名 | camelCase | `tableData`, `queryParams` |
| 常量名 | UPPER_SNAKE_CASE | `API_BASE_URL`, `DEFAULT_PAGE_SIZE` |
| 类型/接口名 | PascalCase | `Product`, `OrderListParams` |
| Props | camelCase | `productId`, `showDialog` |
| Emits | kebab-case | `@update:modelValue`, `@success` |

### CSS 命名

```scss
// 使用 BEM 风格或简化的语义命名
.product-list {
  .card-header { }       // 卡片头部
  .search-bar { }        // 搜索栏
  .table-container { }   // 表格容器
  .form-container { }    // 表单容器
  .fixed-bottom-bar { }  // 固定底部栏
}
```

---

## 类型定义规范

### 基础类型文件结构

```typescript
// src/types/product.ts

/**
 * 商品信息
 */
export interface Product {
  id: string               // GUID 主键，必须是 string
  skuCode: string          // SKU码
  name: string             // 商品名称
  description?: string     // 可选字段用 ?
  price: number
  image: string
  categoryId: string
  stock: number
  createTime?: string
  updateTime?: string
}

/**
 * 商品列表查询参数
 */
export interface ProductListParams {
  pageIndex?: number       // 页码
  pageSize?: number        // 每页数量
  name?: string            // 商品名称
  categoryId?: string      // 分类ID
}

/**
 * 创建商品参数
 */
export interface CreateProductParams {
  skuCode: string          // 必填字段
  name: string
  price: number
  image: string
  categoryId: string
  stock: number
  description?: string     // 可选字段
}

/**
 * 更新商品参数
 */
export interface UpdateProductParams extends CreateProductParams {
  id: string               // 必须包含 id
}
```

### 类型导出规范

```typescript
// src/types/index.ts
// 统一导出所有类型，方便引用

export * from './response'
export * from './product'
export * from './order'
export * from './customer'
export * from './banner'
export * from './enums'
export * from './etl'
```

### API 响应类型

```typescript
// src/types/response.d.ts

/**
 * API 标准响应结构
 */
export interface ApiResponse<T = any> {
  code: number
  message: string
  data: T
}

/**
 * 分页响应结构
 */
export interface PageResponse<T = any> {
  list: T[]
  total: number
  pageIndex: number
  pageSize: number
}

/**
 * 分页请求参数
 */
export interface PageParams {
  pageIndex: number
  pageSize: number
}
```

---

## API 开发规范

### API 文件结构

```typescript
// src/api/buz/productApi.ts

import { get, post, put, del } from '@/utils/request'
import type {
  Product,
  ProductListParams,
  CreateProductParams,
  UpdateProductParams
} from '@/types'

// ===================== 商品 API =====================

/**
 * 获取商品列表
 * @param params 查询参数
 * @returns 分页商品列表
 */
export function getProductList(params: ProductListParams) {
  return get<{ list: Product[]; total: number; pageIndex: number; pageSize: number }>(
    '/api/product/list',
    params
  )
}

/**
 * 获取商品详情
 * @param id 商品ID (GUID)
 */
export function getProductDetail(id: string) {
  return get<Product>(`/api/product/detail/${id}`)
}

/**
 * 创建商品
 * @param data 商品数据
 */
export function createProduct(data: CreateProductParams) {
  return post<{ id: string }>('/api/product/add', data)
}

/**
 * 更新商品
 * @param data 商品数据（含id）
 */
export function updateProduct(data: UpdateProductParams) {
  return post<number>('/api/product/update', data)
}

/**
 * 删除商品
 * @param id 商品ID (GUID)
 */
export function deleteProduct(id: string) {
  return del<number>(`/api/product/delete/${id}`)
}
```

### HTTP 请求方法

```typescript
// src/utils/request.ts 提供的方法

// GET 请求
export function get<T>(url: string, params?: object): Promise<T>

// POST 请求（用于创建、更新）
export function post<T>(url: string, data?: object): Promise<T>

// PUT 请求（用于批量更新）
export function put<T>(url: string, data?: object): Promise<T>

// DELETE 请求
export function del<T>(url: string, data?: object): Promise<T>
```

### API 调用示例

```typescript
// 在组件中调用 API
import { getProductList, deleteProduct } from '@/api/buz/productApi'

// 列表查询
const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getProductList(queryParams.value)
    tableData.value = data.list
    total.value = data.total
  } catch (error) {
    // 错误已在拦截器处理
  } finally {
    loading.value = false
  }
}

// 删除操作
const handleDelete = async (row: Product) => {
  await deleteProduct(row.id)
  ElMessage.success('删除成功')
  handleSearch()
}
```

---

## 组件开发规范

### Vue 组件结构

```vue
<!-- 标准组件模板 -->
<template>
  <div class="component-name">
    <!-- 使用 Element Plus 组件 -->
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('module.title') }}</span>
          <el-button type="primary" @click="handleCreate">
            <el-icon><Plus /></el-icon>
            {{ t('common.button.add') }}
          </el-button>
        </div>
      </template>

      <!-- 搜索栏 -->
      <!-- 表格 -->
      <!-- 分页 -->
    </el-card>

    <!-- 弹窗/抽屉 -->
    <FormDialog v-model="dialogVisible" @success="handleSearch" />
  </div>
</template>

<script setup lang="ts">
// 1. 导入
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import type { FormInstance } from 'element-plus'

// 2. 组件导入
import BaseTable from '@/components/BaseTable/index.vue'
import FormDialog from './components/FormDialog.vue'

// 3. API 导入
import { getList, deleteItem } from '@/api/module/moduleApi'

// 4. Composable 导入
import { useLocale } from '@/composables/useLocale'
import { useDict } from '@/composables/useDict'

// 5. 类型导入
import type { Item, QueryParams } from '@/types'

// 6. Hooks 初始化
const router = useRouter()
const { t } = useLocale()
const { getLabel } = useDict('module')

// 7. 响应式数据
const loading = ref(false)
const tableData = ref<Item[]>([])
const total = ref(0)
const queryParams = reactive<QueryParams>({
  pageIndex: 1,
  pageSize: 10,
})

// 8. 生命周期
onMounted(() => {
  handleSearch()
})

// 9. 事件处理函数
const handleSearch = async () => {
  // ...
}

const handleCreate = () => {
  // ...
}

const handleDelete = async (row: Item) => {
  // ...
}
</script>

<style scoped lang="scss">
.component-name {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
}
</style>
```

### 公共组件使用

#### BaseTable 表格组件

```vue
<template>
  <BaseTable
    :data="tableData"
    :columns="columns"
    :loading="loading"
    :total="total"
    :page-index="queryParams.pageIndex"
    :page-size="queryParams.pageSize"
    :show-index="true"
    :selection="true"
    @update:page-index="queryParams.pageIndex = $event"
    @update:page-size="queryParams.pageSize = $event"
    @page-change="handleSearch"
    @selection-change="handleSelectionChange"
  >
    <!-- 自定义列插槽 -->
    <template #status="{ row }">
      <el-tag :type="row.status === 1 ? 'success' : 'danger'">
        {{ getLabel('Status', row.status) }}
      </el-tag>
    </template>

    <!-- 操作列 -->
    <template #operation>
      <el-table-column label="操作" width="150" fixed="right">
        <template #default="{ row }">
          <el-button link type="primary" @click="handleEdit(row)">
            编辑
          </el-button>
          <el-button link type="danger" @click="handleDelete(row)">
            删除
          </el-button>
        </template>
      </el-table-column>
    </template>
  </BaseTable>
</template>

<script setup lang="ts">
import BaseTable from '@/components/BaseTable/index.vue'
import type { TableColumn } from '@/components/BaseTable/index.vue'

const columns: TableColumn[] = [
  { prop: 'name', label: '名称', minWidth: 150 },
  { prop: 'status', label: '状态', width: 100, align: 'center' },
  { prop: 'createTime', label: '创建时间', width: 180 },
]
</script>
```

#### ImageUpload 图片上传组件

```vue
<template>
  <!-- 单图上传 -->
  <ImageUpload
    v-model="formData.image"
    :placeholder="t('product.edit.mainImagePlaceholder')"
  />

  <!-- 多图上传 -->
  <ImageUpload
    v-model="formData.images"
    multiple
    :limit="5"
  />
</template>

<script setup lang="ts">
import ImageUpload from '@/components/ImageUpload/index.vue'

const formData = reactive({
  image: '',
  images: [] as string[],
})
</script>
```

#### WangEditor 富文本编辑器

```vue
<template>
  <WangEditor
    v-model="formData.detail"
    :height="400"
    :placeholder="t('product.edit.detailPlaceholder')"
  />
</template>

<script setup lang="ts">
import WangEditor from '@/components/WangEditor/index.vue'

const formData = reactive({
  detail: '',
})
</script>
```

---

## Mock 数据配置

### Mock 开关配置

```typescript
// src/config/mock.config.ts

export interface MockConfig {
  auth: boolean       // 认证模块
  basic: boolean      // 基础管理
  product: boolean    // 商品管理
  order: boolean      // 订单管理
  customer: boolean   // 客户管理
  banner: boolean     // 轮播图
  workflow: boolean   // 工作流设计
  workflowRuntime: boolean // 工作流运行时
  antWorkflow: boolean // Ant Workflow
  report: boolean     // 报表
  screen: boolean     // 大屏
  etl: boolean        // ETL
  upload: boolean     // 文件上传
  logQuery: boolean   // 日志查询
}

const mockConfig: MockConfig = {
  auth: true,        // 建议 true，方便开发调试
  basic: true,
  product: true,
  order: true,
  customer: true,
  banner: true,
  workflow: true,
  workflowRuntime: true,
  antWorkflow: true,
  report: true,
  screen: true,
  etl: true,
  upload: true,
  logQuery: true,
}
```

### GUID 主键规范

**重要：所有 Mock 数据的主键必须使用 GUID（UUID）类型，禁止使用自增数字。**

```typescript
// src/utils/guid.ts

/**
 * 生成 GUID 字符串
 */
export function generateGuid(): string {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0
    const v = c === 'x' ? r : (r & 0x3 | 0x8)
    return v.toString(16)
  })
}

/**
 * 验证是否为有效的 GUID
 */
export function isValidGuid(guid: string): boolean {
  const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i
  return guidRegex.test(guid)
}
```

### Mock 数据示例

```typescript
// mock/product.ts

import { generateGuid } from '@/utils/guid'

// Mock 数据使用 GUID
const mockProducts: Product[] = [
  {
    id: generateGuid(),  // ✅ 正确：使用 GUID
    skuCode: 'SKU-001',
    name: '商品A',
    price: 100,
    // ...
  },
  // ❌ 错误示例（禁止使用）
  // {
  //   id: 1,  // 禁止使用数字 ID
  // },
]

// API 函数参数使用 string
export function getProductDetail(id: string) {  // ✅ 正确
  return get<Product>(`/api/product/detail/${id}`)
}

// ❌ 错误示例
// export function getProductDetail(id: number) {  // 禁止
//   return get<Product>(`/api/product/detail/${id}`)
// }
```

---

## 字典与枚举规范

### 字典配置

```typescript
// src/config/dict.config.ts

export interface DictModuleSetting {
  /** 是否使用后端字典 */
  IsUseDic: boolean
  /** 枚举名称 -> 字典编码映射 */
  enumMappings: Record<string, string>
}

export const dictModuleConfig: Record<string, DictModuleSetting> = {
  // 工作流模块
  workflow: {
    IsUseDic: false,  // false = 使用本地枚举
    enumMappings: {
      WorkflowStatus: 'workflow_status',
      NodeType: 'flow_node_type',
    }
  },

  // 订单模块
  order: {
    IsUseDic: false,
    enumMappings: {
      OrderStatus: 'order_status',
      PayStatus: 'pay_status',
    }
  },
}
```

### 本地枚举标签

```typescript
// src/config/enumLabels.ts

export const enumLabelMap: Record<string, Record<string | number, { zh: string; en: string }>> = {
  // 通用状态
  CommonStatus: {
    0: { zh: '禁用', en: 'Disabled' },
    1: { zh: '启用', en: 'Enabled' },
  },

  // 工作流状态
  WorkflowStatus: {
    0: { zh: '草稿', en: 'Draft' },
    1: { zh: '待审核', en: 'Pending' },
    2: { zh: '已发布', en: 'Published' },
  },
}
```

### 在组件中使用字典

```vue
<template>
  <!-- 显示状态标签 -->
  <el-tag :type="getStatusType(row.status)">
    {{ getLabel('WorkflowStatus', row.status) }}
  </el-tag>

  <!-- 下拉选项 -->
  <el-select v-model="queryParams.status">
    <el-option
      v-for="opt in getOptions('WorkflowStatus')"
      :key="opt.value"
      :label="opt.label"
      :value="opt.value"
    />
  </el-select>
</template>

<script setup lang="ts">
import { useDict } from '@/composables/useDict'

// 传入模块名称
const { getLabel, getOptions } = useDict('workflow')

// 获取标签
const label = getLabel('WorkflowStatus', 1)  // '待审核'

// 获取选项列表
const options = getOptions('WorkflowStatus')
// [{ value: '0', label: '草稿' }, { value: '1', label: '待审核' }, ...]
</script>
```

---

## 多语言规范

### useLocale Composable

```typescript
// src/composables/useLocale.ts

export function useLocale() {
  const i18n = useI18n()

  // 当前语言
  const currentLocale = computed<LocaleType>(() => {
    return i18n.locale.value as LocaleType
  })

  // Element Plus 语言包
  const elementLocale = computed<Language>(() => {
    return ELEMENT_LOCALES[currentLocale.value]
  })

  // 切换语言
  const changeLocale = (lang: LocaleType) => {
    i18n.locale.value = lang
    localStorage.setItem('locale', lang)
  }

  return {
    t: i18n.t,            // 翻译函数
    currentLocale,
    elementLocale,
    changeLocale,
  }
}
```

### 在组件中使用多语言

```vue
<template>
  <el-button>{{ t('common.button.add') }}</el-button>
  <span>{{ t('product.list.title') }}</span>
</template>

<script setup lang="ts">
import { useLocale } from '@/composables/useLocale'

const { t } = useLocale()
</script>
```

### 语言包结构

```
locales/
├── zh-CN/
│   ├── common.json      # 通用文本
│   ├── product.json     # 商品模块
│   ├── order.json       # 订单模块
│   └── workflow.json    # 工作流模块
└── en-US/
    ├── common.json
    ├── product.json
    ├── order.json
    └── workflow.json
```

---

## 状态管理规范

### Pinia Store 结构

```typescript
// src/stores/product.ts

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Product } from '@/types'

export const useProductStore = defineStore('product', () => {
  // State
  const productList = ref<Product[]>([])
  const currentProduct = ref<Product | null>(null)
  const loading = ref(false)

  // Getter
  const productCount = computed(() => productList.value.length)

  // Actions
  function setProductList(list: Product[]) {
    productList.value = list
  }

  function setCurrentProduct(product: Product | null) {
    currentProduct.value = product
  }

  async function fetchProducts() {
    loading.value = true
    try {
      const data = await getProductList({})
      productList.value = data.list
    } finally {
      loading.value = false
    }
  }

  // 重置状态
  function $reset() {
    productList.value = []
    currentProduct.value = null
    loading.value = false
  }

  return {
    // State
    productList,
    currentProduct,
    loading,
    // Getter
    productCount,
    // Actions
    setProductList,
    setCurrentProduct,
    fetchProducts,
    $reset,
  }
})
```

### 在组件中使用 Store

```vue
<script setup lang="ts">
import { useProductStore } from '@/stores/product'

const productStore = useProductStore()

// 访问状态
const products = productStore.productList

// 调用 action
productStore.fetchProducts()

// 重置状态
productStore.$reset()
</script>
```

---

## 路由配置规范

### 路由文件结构

```typescript
// src/router/routes.ts

import type { RouteRecordRaw } from 'vue-router'

// 静态路由
export const constantRoutes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/login/index.vue'),
    meta: { title: '登录' }
  },
  {
    path: '/',
    name: 'Root',
    redirect: '/desktop',
    component: () => import('@/views/layout/index.vue'),
    children: [
      {
        path: 'desktop',
        name: 'Desktop',
        component: () => import('@/views/desktop/index.vue'),
        meta: { title: '工作台', icon: 'House' }
      },
    ]
  },
]
```

### 动态路由

```typescript
// src/router/dynamic.ts

// 根据权限动态加载的路由
export const dynamicRoutes: RouteRecordRaw[] = [
  {
    path: '/basic',
    name: 'Basic',
    redirect: '/basic/user',
    meta: {
      title: '基础管理',
      icon: 'Setting',
      roles: ['admin']
    },
    children: [
      {
        path: 'user',
        name: 'User',
        component: () => import('@/views/basic/user/index.vue'),
        meta: { title: '用户管理', icon: 'User' }
      },
      {
        path: 'role',
        name: 'Role',
        component: () => import('@/views/basic/role/index.vue'),
        meta: { title: '角色管理', icon: 'UserFilled' }
      },
    ]
  },
]
```

### 路由 Meta 类型

```typescript
// src/types/router.d.ts

import 'vue-router'

declare module 'vue-router' {
  interface RouteMeta {
    title?: string         // 路由标题
    icon?: string          // 路由图标
    hidden?: boolean       // 是否隐藏
    roles?: string[]       // 权限角色
    keepAlive?: boolean    // 是否缓存
    breadcrumb?: boolean   // 是否显示面包屑
  }
}
```

---

## 页面开发模板

### 列表页面模板

```vue
<!-- src/views/module/list/index.vue -->
<template>
  <div class="list-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('module.list.title') }}</span>
          <el-button type="primary" @click="handleCreate">
            <el-icon><Plus /></el-icon>
            {{ t('common.button.add') }}
          </el-button>
        </div>
      </template>

      <!-- 搜索栏 -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('module.list.name')">
          <el-input
            v-model="queryParams.name"
            :placeholder="t('common.placeholder.input')"
            clearable
          />
        </el-form-item>
        <el-form-item :label="t('module.list.status')">
          <el-select
            v-model="queryParams.status"
            :placeholder="t('common.placeholder.select')"
            clearable
          >
            <el-option
              v-for="opt in getOptions('Status')"
              :key="opt.value"
              :label="opt.label"
              :value="opt.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            {{ t('common.button.search') }}
          </el-button>
          <el-button @click="handleReset">
            {{ t('common.button.reset') }}
          </el-button>
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
        <!-- 状态列 -->
        <template #status="{ row }">
          <el-tag :type="row.status === 1 ? 'success' : 'danger'">
            {{ getLabel('Status', row.status) }}
          </el-tag>
        </template>

        <!-- 操作列 -->
        <template #operation>
          <el-table-column :label="t('common.table.operation')" width="150" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleEdit(row)">
                {{ t('common.button.edit') }}
              </el-button>
              <el-button link type="danger" @click="handleDelete(row)">
                {{ t('common.button.delete') }}
              </el-button>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>

    <!-- 表单弹窗 -->
    <FormDialog
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
import FormDialog from './components/FormDialog.vue'
import { getList, deleteItem } from '@/api/module/moduleApi'
import { useLocale } from '@/composables/useLocale'
import { useDict } from '@/composables/useDict'
import type { Item, QueryParams } from '@/types'
import type { TableColumn } from '@/components/BaseTable/index.vue'

const { t } = useLocale()
const { getLabel, getOptions } = useDict('module')

// 表格列配置
const columns: TableColumn[] = [
  { prop: 'name', label: t('module.list.name'), minWidth: 150 },
  { prop: 'status', label: t('module.list.status'), width: 100, align: 'center' },
  { prop: 'createTime', label: t('common.table.createTime'), width: 180 },
]

// 响应式数据
const loading = ref(false)
const tableData = ref<Item[]>([])
const total = ref(0)
const dialogVisible = ref(false)
const currentId = ref<string | undefined>(undefined)

const queryParams = reactive<QueryParams>({
  pageIndex: 1,
  pageSize: 10,
  name: '',
  status: undefined,
})

// 生命周期
onMounted(() => {
  handleSearch()
})

// 搜索
const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getList(queryParams)
    tableData.value = data.list
    total.value = data.total
  } catch (error) {
    // 错误已在拦截器处理
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryParams.name = ''
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
const handleEdit = (row: Item) => {
  currentId.value = row.id
  dialogVisible.value = true
}

// 删除
const handleDelete = async (row: Item) => {
  try {
    await ElMessageBox.confirm(
      t('common.message.deleteConfirm'),
      t('common.message.warning'),
      { type: 'warning' }
    )
    await deleteItem(row.id)
    ElMessage.success(t('common.message.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // 用户取消或请求失败
  }
}
</script>

<style scoped lang="scss">
.list-container {
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

### 编辑页面模板

```vue
<!-- src/views/module/edit/index.vue -->
<template>
  <div class="edit-container">
    <!-- 面包屑 -->
    <el-breadcrumb separator="/">
      <el-breadcrumb-item :to="{ path: '/module/list' }">
        {{ t('module.list.title') }}
      </el-breadcrumb-item>
      <el-breadcrumb-item>
        {{ isEdit ? t('module.edit.title') : t('module.create.title') }}
      </el-breadcrumb-item>
    </el-breadcrumb>

    <el-card shadow="never" class="edit-card">
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
        class="form-container"
      >
        <!-- 分区标题 -->
        <div class="section-title">{{ t('module.edit.basicInfo') }}</div>

        <el-form-item :label="t('module.edit.name')" prop="name">
          <el-input v-model="formData.name" maxlength="100" show-word-limit />
        </el-form-item>

        <el-form-item :label="t('module.edit.image')" prop="image">
          <ImageUpload v-model="formData.image" />
        </el-form-item>

        <div class="section-title">{{ t('module.edit.detail') }}</div>

        <el-form-item :label="t('module.edit.content')" prop="content">
          <WangEditor v-model="formData.content" :height="400" />
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 固定底部栏 -->
    <div class="fixed-bottom-bar">
      <el-button type="primary" :loading="saving" @click="handleSave">
        {{ t('common.button.save') }}
      </el-button>
      <el-button @click="handleCancel">
        {{ t('common.button.cancel') }}
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import ImageUpload from '@/components/ImageUpload/index.vue'
import WangEditor from '@/components/WangEditor/index.vue'
import { getDetail, createItem, updateItem } from '@/api/module/moduleApi'
import { useLocale } from '@/composables/useLocale'
import type { CreateParams, UpdateParams } from '@/types'

const router = useRouter()
const route = useRoute()
const { t } = useLocale()

const loading = ref(false)
const saving = ref(false)
const formRef = ref<FormInstance | null>(null)
const isEdit = computed(() => !!route.params.id)

const formData = reactive<CreateParams>({
  name: '',
  image: '',
  content: '',
})

const formRules: FormRules = {
  name: [
    { required: true, message: t('module.edit.nameRequired'), trigger: 'blur' },
    { min: 2, max: 100, message: t('module.edit.nameLength'), trigger: 'blur' },
  ],
  image: [
    { required: true, message: t('module.edit.imageRequired'), trigger: 'change' },
  ],
}

onMounted(async () => {
  const id = route.params.id as string
  if (id) {
    await loadDetail(id)
  }
})

const loadDetail = async (id: string) => {
  loading.value = true
  try {
    const data = await getDetail(id)
    Object.assign(formData, data)
  } catch (error) {
    ElMessage.error(t('common.message.loadFailed'))
    router.push('/module/list')
  } finally {
    loading.value = false
  }
}

const handleSave = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch {
    return
  }

  saving.value = true
  try {
    if (isEdit.value) {
      const params: UpdateParams = {
        id: route.params.id as string,
        ...formData,
      }
      await updateItem(params)
    } else {
      await createItem(formData)
    }
    ElMessage.success(t('common.message.saveSuccess'))
    router.push('/module/list')
  } catch (error) {
    ElMessage.error(t('common.message.saveFailed'))
  } finally {
    saving.value = false
  }
}

const handleCancel = () => {
  router.push('/module/list')
}
</script>

<style scoped lang="scss">
.edit-container {
  padding: 20px;
  padding-bottom: 80px;

  .edit-card {
    margin-top: 16px;
  }

  .form-container {
    max-width: 800px;

    .section-title {
      font-size: 16px;
      font-weight: 600;
      margin: 20px 0 16px;
      padding-bottom: 8px;
      border-bottom: 1px solid #ebeef5;
    }
  }

  .fixed-bottom-bar {
    position: fixed;
    bottom: 0;
    left: 200px;
    right: 0;
    padding: 16px 20px;
    background: #fff;
    border-top: 1px solid #ebeef5;
    display: flex;
    gap: 12px;
    z-index: 100;
  }
}
</style>
```

---

## 最佳实践总结

### 1. 导入顺序

```typescript
// 1. Vue 核心
import { ref, reactive, onMounted } from 'vue'

// 2. 第三方库
import { ElMessage } from 'element-plus'

// 3. 组件
import BaseTable from '@/components/BaseTable/index.vue'

// 4. API
import { getList } from '@/api/module/moduleApi'

// 5. Composables
import { useLocale } from '@/composables/useLocale'

// 6. 类型
import type { Item } from '@/types'
```

### 2. 错误处理

```typescript
// 错误已在 request.ts 拦截器统一处理
// 无需在每个 API 调用处单独处理

try {
  const data = await getList(queryParams)
  tableData.value = data.list
} catch (error) {
  // 拦截器已显示错误消息
  // 只需处理 UI 状态
} finally {
  loading.value = false
}
```

### 3. 类型安全

```typescript
// ✅ 正确：使用类型定义
const tableData = ref<Product[]>([])
const queryParams = reactive<ProductListParams>({ ... })

// ❌ 错误：使用 any
const tableData = ref<any[]>([])
```

### 4. 响应式数据

```typescript
// ✅ 正确：使用 reactive 处理对象
const queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
})

// ✅ 正确：使用 ref 处理基本类型和数组
const loading = ref(false)
const tableData = ref<Product[]>([])

// ❌ 错误：对基本类型使用 reactive
const loading = reactive({ value: false })  // 不推荐
```

---

## 🏪 业务模块说明

### 商品管理 (`views/buz/product/`)
- 商品列表、编辑、分类管理、库存管理
- API: `api/buz/productApi.ts`
- 类型: `types/product.ts`

### 订单管理 (`views/buz/order/`)
- 订单列表、发货、物流跟踪、退款创建
- API: `api/buz/orderApi.ts`

### 客户管理 (`views/buz/customer/`)
- 客户列表、会员等级、积分记录
- API: `api/buz/customerApi.ts`, `memberLevelApi.ts`, `pointsApi.ts`

### 退款管理 (`views/buz/refund/`)
- 仅退款、退货退款、换货处理

### 轮播图管理 (`views/buz/banner/`)
- 小程序首页轮播图配置

---

## 🔄 工作流设计器 (`components/workflow/`)

钉钉审批风格的纯 Vue3 实现，使用 JSON 数据格式。

**节点类型**:
- StartNode - 发起人
- ApproverNode - 审批人
- CopyerNode - 抄送人
- ServiceNode - 服务任务
- NotificationNode - 通知
- ConditionNode - 条件分支

**状态管理**: `stores/antWorkflowStore.ts`

---

## 📊 大屏设计器 (`views/screen/`)

拖拽式布局，支持多数据源（静态数据、API、SQL）。

**图表类型**: 折线图、柱状图、饼图、环形图、指标卡片

---

## 🚀 快速开始

### 开发命令

```bash
# 安装依赖
pnpm install

# 启动开发服务器
pnpm run dev

# 构建生产版本
pnpm run build

# 代码检查
pnpm run lint
```

### 添加新模块

1. 在 `views/模块名/` 创建页面组件
2. 在 `api/模块名/` 创建 API 封装
3. 在 `types/` 创建类型定义
4. 在 `router/` 添加路由配置

---

## 🔗 参考资源

- **需求方案**: [需求方案.md](./需求方案.md)
- **Element Plus**: https://element-plus.org/
- **Vue 3**: https://vuejs.org/
- **Pinia**: https://pinia.vuejs.org/
- **项目风格参考**: https://github.com/LiLin928/ape-volo-web
- **工作流参考**: https://github.com/LiLin928/Workflow-Vue3
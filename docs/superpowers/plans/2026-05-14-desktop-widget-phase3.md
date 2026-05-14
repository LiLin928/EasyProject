# 桌面组件自动配置系统 - 阶段三：前端角色分配页面

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完成前端角色分配页面，管理员可以为角色分配桌面组件，设置默认启用状态和排序

**Architecture:** 采用 Vue 3 Composition API + TypeScript + Element Plus，角色选择器 + 组件分配列表 + 预览布局效果

**Tech Stack:** Vue 3.3 + TypeScript 5.0 + Element Plus 2.4 + Vite 5.4

---

## 文件结构

### 需要创建/修改的文件

```
PCWeb/src/
├── types/
│   └── desktopWidget.ts              # 添加角色配置相关类型(修改)
│
├── api/basic/
│   └── roleWidgetConfigApi.ts        # 角色配置API封装(新建)
│
├── views/basic/desktop/
│   └── role-config/
│   │   ├── index.vue                 # 角色分配页面(重写)
│   │   └── components/
│   │   │   └── WidgetAssignDialog.vue # 组件分配弹窗
│   │   │   └── LayoutPreview.vue     # 布局预览组件
│
└── stores/
│   └── roleStore.ts                  # 获取角色列表(读取现有)
```

---

## Task 1: 扩展类型定义

**Files:**
- Modify: `PCWeb/src/types/desktopWidget.ts`

- [ ] **Step 1: 在类型文件中添加角色配置类型**

在文件末尾添加：

```typescript
// 文件: PCWeb/src/types/desktopWidget.ts
// ... 保留原有内容，添加以下内容 ...

/**
 * 角色组件配置项
 */
export interface RoleWidgetConfigItem {
  widgetId: string
  sortOrder: number
  isEnabled: boolean
}

/**
 * 角色组件配置
 */
export interface RoleWidgetConfig {
  id: string
  roleId: string
  widgetId: string
  widgetName: string
  widgetType: WidgetType
  defaultWidth: number
  defaultHeight: number
  sortOrder: number
  isEnabled: boolean
}

/**
 * 保存角色配置参数
 */
export interface SaveRoleWidgetConfigParams {
  roleId: string
  widgets: RoleWidgetConfigItem[]
}

/**
 * 可用组件DTO
 */
export interface AvailableWidget {
  id: string
  name: string
  type: number
  icon?: string
  defaultWidth: number
  defaultHeight: number
  isUserEnabled: boolean
}
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/types/desktopWidget.ts
git commit -m "feat(desktop): add role widget config types

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 2: 创建角色配置API封装

**Files:**
- Create: `PCWeb/src/api/basic/roleWidgetConfigApi.ts`

- [ ] **Step 1: 创建API封装文件**

```typescript
// 文件: PCWeb/src/api/basic/roleWidgetConfigApi.ts

import { get, post } from '@/utils/request'
import type {
  RoleWidgetConfig,
  SaveRoleWidgetConfigParams,
  AvailableWidget,
} from '@/types'

/**
 * 获取角色的组件配置列表
 * @param roleId 角色ID
 */
export function getRoleWidgetConfigList(roleId: string) {
  return get<RoleWidgetConfig[]>(`/api/desktop/role-config/list/${roleId}`)
}

/**
 * 保存角色组件配置
 * @param data 配置数据
 */
export function saveRoleWidgetConfig(data: SaveRoleWidgetConfigParams) {
  return post<boolean>('/api/desktop/role-config/save', data)
}

/**
 * 获取角色可用的组件列表
 * @param roleId 角色ID
 */
export function getAvailableWidgets(roleId: string) {
  return get<AvailableWidget[]>(`/api/desktop/role-config/available-widgets/${roleId}`)
}
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/api/basic/roleWidgetConfigApi.ts
git commit -m "feat(desktop): add role widget config API

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 3: 创建角色分配主页面

**Files:**
- Modify: `PCWeb/src/views/basic/desktop/role-config/index.vue` (重写)

- [ ] **Step 1: 重写角色分配页面**

```vue
<!-- 文件: PCWeb/src/views/basic/desktop/role-config/index.vue -->
<template>
  <div class="role-config-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>角色桌面分配</span>
        </div>
      </template>

      <!-- 角色选择 -->
      <div class="role-selector">
        <el-form :inline="true">
          <el-form-item label="选择角色">
            <el-select
              v-model="selectedRoleId"
              placeholder="请选择角色"
              style="width: 200px"
              @change="handleRoleChange"
            >
              <el-option
                v-for="role in roleList"
                :key="role.id"
                :label="role.name"
                :value="role.id"
              />
            </el-select>
          </el-form-item>
        </el-form>
      </div>

      <!-- 组件分配列表 -->
      <div v-if="selectedRoleId" class="widget-assign-section">
        <div class="section-header">
          <span class="section-title">已分配组件</span>
          <el-button type="primary" size="small" @click="handleAddWidget">
            <el-icon><Plus /></el-icon>
            添加组件
          </el-button>
        </div>

        <el-table
          :data="assignedWidgets"
          :loading="loading"
          border
          style="width: 100%"
        >
          <el-table-column prop="widgetName" label="组件名称" min-width="150" />
          <el-table-column prop="widgetType" label="类型" width="100" align="center">
            <template #default="{ row }">
              <el-tag :type="getTypeTagType(row.widgetType)">
                {{ widgetTypeLabels[row.widgetType] }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="defaultWidth" label="宽度" width="100" align="center">
            <template #default="{ row }">
              {{ row.defaultWidth }}栅格
            </template>
          </el-table-column>
          <el-table-column prop="sortOrder" label="排序" width="80" align="center">
            <template #default="{ row }">
              <el-input-number
                v-model="row.sortOrder"
                :min="0"
                :max="99"
                size="small"
                controls-position="right"
                @change="handleSortChange"
              />
            </template>
          </el-table-column>
          <el-table-column prop="isEnabled" label="启用" width="80" align="center">
            <template #default="{ row }">
              <el-switch
                v-model="row.isEnabled"
                @change="handleEnableChange"
              />
            </template>
          </el-table-column>
          <el-table-column label="操作" width="80" align="center">
            <template #default="{ row }">
              <el-button link type="danger" @click="handleRemoveWidget(row)">
                移除
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <!-- 布局预览 -->
        <div class="preview-section">
          <div class="section-title">布局预览</div>
          <LayoutPreview :widgets="previewWidgets" />
        </div>

        <!-- 保存按钮 -->
        <div class="action-bar">
          <el-button type="primary" :loading="saving" @click="handleSave">
            保存配置
          </el-button>
          <el-button @click="handleReset">重置</el-button>
        </div>
      </div>

      <el-empty v-else description="请先选择角色" />
    </el-card>

    <!-- 添加组件弹窗 -->
    <WidgetAssignDialog
      v-model="assignDialogVisible"
      :role-id="selectedRoleId"
      :assigned-ids="assignedWidgetIds"
      @success="loadRoleConfig"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import WidgetAssignDialog from './components/WidgetAssignDialog.vue'
import LayoutPreview from './components/LayoutPreview.vue'
import { getRoleWidgetConfigList, saveRoleWidgetConfig } from '@/api/basic/roleWidgetConfigApi'
import { getRoleList } from '@/api/basic/roleApi'
import { WidgetType, widgetTypeLabels } from '@/types'
import type { RoleWidgetConfig, RoleWidgetConfigItem } from '@/types'

// 角色列表
const roleList = ref<{ id: string; name: string }[]>([])
const selectedRoleId = ref<string>('')
const loading = ref(false)
const saving = ref(false)
const assignDialogVisible = ref(false)

// 已分配组件
const assignedWidgets = ref<RoleWidgetConfig[]>([])

// 计算已分配组件ID列表
const assignedWidgetIds = computed(() => 
  assignedWidgets.value.map(w => w.widgetId)
)

// 预览组件列表
const previewWidgets = computed(() =>
  assignedWidgets.value
    .filter(w => w.isEnabled)
    .sort((a, b) => a.sortOrder - b.sortOrder)
)

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
onMounted(async () => {
  await loadRoles()
})

// 加载角色列表
const loadRoles = async () => {
  try {
    const data = await getRoleList({ pageIndex: 1, pageSize: 100 })
    roleList.value = data.list.map(r => ({ id: r.id, name: r.name }))
  } catch (error) {
    ElMessage.error('加载角色列表失败')
  }
}

// 角色切换
const handleRoleChange = async () => {
  if (selectedRoleId.value) {
    await loadRoleConfig()
  } else {
    assignedWidgets.value = []
  }
}

// 加载角色配置
const loadRoleConfig = async () => {
  if (!selectedRoleId.value) return
  loading.value = true
  try {
    assignedWidgets.value = await getRoleWidgetConfigList(selectedRoleId.value)
  } catch (error) {
    ElMessage.error('加载角色配置失败')
  } finally {
    loading.value = false
  }
}

// 添加组件
const handleAddWidget = () => {
  assignDialogVisible.value = true
}

// 移除组件
const handleRemoveWidget = async (row: RoleWidgetConfig) => {
  try {
    await ElMessageBox.confirm(
      `确定要移除组件 "${row.widgetName}" 吗？`,
      '提示',
      { type: 'warning' }
    )
    const index = assignedWidgets.value.findIndex(w => w.widgetId === row.widgetId)
    if (index > -1) {
      assignedWidgets.value.splice(index, 1)
    }
  } catch (error) {
    // 用户取消
  }
}

// 排序变化
const handleSortChange = () => {
  // 自动触发保存预览更新
}

// 启用状态变化
const handleEnableChange = () => {
  // 自动触发保存预览更新
}

// 保存配置
const handleSave = async () => {
  saving.value = true
  try {
    const params = {
      roleId: selectedRoleId.value,
      widgets: assignedWidgets.value.map(w => ({
        widgetId: w.widgetId,
        sortOrder: w.sortOrder,
        isEnabled: w.isEnabled,
      })),
    }
    await saveRoleWidgetConfig(params)
    ElMessage.success('保存成功')
  } catch (error) {
    ElMessage.error('保存失败')
  } finally {
    saving.value = false
  }
}

// 重置
const handleReset = async () => {
  await loadRoleConfig()
}
</script>

<style scoped lang="scss">
.role-config-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .role-selector {
    margin-bottom: 20px;
  }

  .widget-assign-section {
    .section-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }

    .section-title {
      font-size: 14px;
      font-weight: 600;
      color: #303133;
    }

    .preview-section {
      margin-top: 20px;
    }

    .action-bar {
      margin-top: 20px;
      display: flex;
      gap: 12px;
    }
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
git add PCWeb/src/views/basic/desktop/role-config/index.vue
git commit -m "feat(desktop): implement role widget config page

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 4: 创建组件分配弹窗

**Files:**
- Create: `PCWeb/src/views/basic/desktop/role-config/components/WidgetAssignDialog.vue`

- [ ] **Step 1: 创建组件分配弹窗**

```vue
<!-- 文件: PCWeb/src/views/basic/desktop/role-config/components/WidgetAssignDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    title="添加组件"
    width="600px"
    :close-on-click-modal="false"
  >
    <div class="widget-list">
      <el-checkbox-group v-model="selectedWidgetIds">
        <div
          v-for="widget in availableWidgets"
          :key="widget.id"
          class="widget-item"
          :class="{ disabled: assignedIds.includes(widget.id) }"
        >
          <el-checkbox
            :value="widget.id"
            :disabled="assignedIds.includes(widget.id)"
          >
            <div class="widget-info">
              <span class="widget-name">{{ widget.name }}</span>
              <el-tag size="small" :type="getTypeTagType(widget.type)">
                {{ getTypeLabel(widget.type) }}
              </el-tag>
              <span class="widget-size">{{ widget.defaultWidth }}栅格</span>
            </div>
          </el-checkbox>
          <span v-if="assignedIds.includes(widget.id)" class="assigned-tag">
            已分配
          </span>
        </div>
      </el-checkbox-group>
    </div>

    <template #footer>
      <el-button @click="visible = false">取消</el-button>
      <el-button type="primary" :loading="saving" @click="handleConfirm">
        确认添加
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { getEnabledWidgetList } from '@/api/basic/desktopWidgetApi'
import { DesktopWidget, WidgetType, widgetTypeLabels } from '@/types'

const props = defineProps<{
  modelValue: boolean
  roleId: string
  assignedIds: string[]
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  'success': []
}>()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const availableWidgets = ref<DesktopWidget[]>([])
const selectedWidgetIds = ref<string[]>([])
const saving = ref(false)

// 监听弹窗打开
watch(visible, async (val) => {
  if (val) {
    selectedWidgetIds.value = []
    await loadWidgets()
  }
})

// 加载可用组件
const loadWidgets = async () => {
  try {
    availableWidgets.value = await getEnabledWidgetList()
  } catch (error) {
    ElMessage.error('加载组件列表失败')
  }
}

// 获取类型标签颜色
function getTypeTagType(type: number): string {
  const typeMap: Record<number, string> = {
    [WidgetType.Card]: 'primary',
    [WidgetType.List]: 'success',
    [WidgetType.Image]: 'warning',
    [WidgetType.Chart]: 'danger',
  }
  return typeMap[type] || 'info'
}

// 获取类型标签
function getTypeLabel(type: number): string {
  return widgetTypeLabels[type as WidgetType] || '未知'
}

// 确认添加
const handleConfirm = () => {
  // 筛选出新增的组件ID
  const newIds = selectedWidgetIds.value.filter(id => !props.assignedIds.includes(id))
  if (newIds.length === 0) {
    ElMessage.warning('请选择要添加的组件')
    return
  }
  visible.value = false
  emit('success')
}
</script>

<style scoped lang="scss">
.widget-list {
  .widget-item {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 12px;
    border: 1px solid #ebeef5;
    border-radius: 4px;
    margin-bottom: 8px;
    transition: background-color 0.2s;

    &:hover {
      background-color: #f5f7fa;
    }

    &.disabled {
      background-color: #fafafa;
      opacity: 0.6;
    }

    .widget-info {
      display: flex;
      align-items: center;
      gap: 12px;

      .widget-name {
        font-weight: 500;
      }

      .widget-size {
        color: #909399;
        font-size: 12px;
      }
    }

    .assigned-tag {
      color: #67c23a;
      font-size: 12px;
    }
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
git add PCWeb/src/views/basic/desktop/role-config/components/WidgetAssignDialog.vue
git commit -m "feat(desktop): add widget assign dialog

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 5: 创建布局预览组件

**Files:**
- Create: `PCWeb/src/views/basic/desktop/role-config/components/LayoutPreview.vue`

- [ ] **Step 1: 创建布局预览组件**

```vue
<!-- 文件: PCWeb/src/views/basic/desktop/role-config/components/LayoutPreview.vue -->
<template>
  <div class="layout-preview">
    <div class="preview-grid">
      <div
        v-for="widget in widgets"
        :key="widget.widgetId"
        class="preview-widget"
        :style="{
          width: `${(widget.defaultWidth / 12) * 100}%`,
          height: `${widget.defaultHeight}px`,
        }"
      >
        <div class="widget-header">
          <el-icon v-if="widget.icon">
            <component :is="widget.icon" />
          </el-icon>
          <span>{{ widget.widgetName }}</span>
        </div>
        <div class="widget-content">
          <div class="widget-type-icon">
            <span v-if="widget.widgetType === WidgetType.Card">📊</span>
            <span v-else-if="widget.widgetType === WidgetType.List">📋</span>
            <span v-else-if="widget.widgetType === WidgetType.Image">🖼️</span>
            <span v-else-if="widget.widgetType === WidgetType.Chart">📈</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { WidgetType } from '@/types'
import type { RoleWidgetConfig } from '@/types'

const props = defineProps<{
  widgets: RoleWidgetConfig[]
}>()
</script>

<style scoped lang="scss">
.layout-preview {
  background: #f5f7fa;
  padding: 16px;
  border-radius: 8px;
  border: 1px solid #ebeef5;

  .preview-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 12px;
  }

  .preview-widget {
    background: #fff;
    border-radius: 8px;
    border: 1px solid #e4e7ed;
    display: flex;
    flex-direction: column;
    min-width: 120px;
    box-sizing: border-box;

    .widget-header {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 12px;
      font-size: 13px;
      font-weight: 500;
      color: #606266;
      border-bottom: 1px solid #ebeef5;
    }

    .widget-content {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 16px;

      .widget-type-icon {
        font-size: 24px;
      }
    }
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
git add PCWeb/src/views/basic/desktop/role-config/components/LayoutPreview.vue
git commit -m "feat(desktop): add layout preview component

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 6: 完整验证与测试

- [ ] **Step 1: 启动前端开发服务器**

Run: `cd PCWeb && pnpm run dev`
Expected: 服务器启动

- [ ] **Step 2: 访问角色分配页面**

访问路径: `/basic/desktop/role-config`
验证：
- 角色选择器正常显示
- 角色切换后加载已分配组件
- 添加组件弹窗正常工作
- 排序和启用状态切换正常
- 布局预览正确显示
- 保存配置功能正常

- [ ] **Step 3: 测试与组件管理页面联动**

验证：
- 在组件管理页面新增组件后，角色分配页面可以看到新组件
- 删除组件后，角色分配列表自动移除

---

## Spec Coverage Check

设计文档覆盖情况（阶段三）：

| 设计文档章节 | 实现Task |
|-------------|----------|
| 7.2 角色桌面分配页面 | Task 3 |
| 角色组件配置API | Task 2 |
| 角色配置类型 | Task 1 |
| 布局预览 | Task 5 |
| 组件分配弹窗 | Task 4 |

---

**文档版本**: 1.0
**最后更新**: 2026-05-14
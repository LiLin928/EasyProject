<!-- 文件: PCWeb/src/views/basic/desktop/role-config/index.vue -->
<template>
  <div class="role-config-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>角色组件分配</span>
        </div>
      </template>

      <!-- 角色选择器 -->
      <div class="role-selector">
        <el-form :inline="true">
          <el-form-item label="选择角色">
            <el-select
              v-model="selectedRoleId"
              placeholder="请选择角色"
              clearable
              filterable
              @change="handleRoleChange"
            >
              <el-option
                v-for="role in roleList"
                :key="role.id"
                :label="role.roleName"
                :value="role.id"
              />
            </el-select>
          </el-form-item>
        </el-form>
      </div>

      <!-- 未选择角色提示 -->
      <el-empty v-if="!selectedRoleId" description="请先选择一个角色" />

      <!-- 已分配组件列表 -->
      <div v-if="selectedRoleId" class="config-content">
        <div class="section-header">
          <span>已分配组件</span>
          <el-button type="primary" size="small" @click="handleAddWidget">
            <el-icon><Plus /></el-icon>
            添加组件
          </el-button>
        </div>

        <el-table
          v-loading="loading"
          :data="widgetConfigList"
          border
        >
          <el-table-column prop="widgetName" label="组件名称" min-width="120" />
          <el-table-column prop="widgetType" label="组件类型" width="100" align="center">
            <template #default="{ row }">
              <el-tag>{{ getWidgetTypeLabel(row.widgetType) }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="defaultWidth" label="默认宽度(栅格)" width="120" align="center">
            <template #default="{ row }">
              {{ row.defaultWidth }}栅格
            </template>
          </el-table-column>
          <el-table-column prop="sortOrder" label="排序" width="100" align="center">
            <template #default="{ row }">
              <el-input-number
                v-model="row.sortOrder"
                :min="1"
                :max="100"
                size="small"
                controls-position="right"
              />
            </template>
          </el-table-column>
          <el-table-column prop="isEnabled" label="启用状态" width="100" align="center">
            <template #default="{ row }">
              <el-switch v-model="row.isEnabled" />
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
        <div class="layout-preview-section">
          <div class="section-header">
            <span>布局预览</span>
          </div>
          <LayoutPreview :widgets="widgetConfigList" />
        </div>

        <!-- 底部操作按钮 -->
        <div class="action-bar">
          <el-button type="primary" :loading="saving" @click="handleSave">
            保存配置
          </el-button>
          <el-button @click="handleReset">
            重置
          </el-button>
        </div>
      </div>
    </el-card>

    <!-- 添加组件弹窗 -->
    <WidgetAssignDialog
      v-model="dialogVisible"
      :role-id="selectedRoleId"
      :assigned-widget-ids="assignedWidgetIds"
      @success="handleDialogSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { getRoleList } from '@/api/role'
import { getRoleWidgetConfigList, saveRoleWidgetConfig } from '@/api/basic/roleWidgetConfigApi'
import WidgetAssignDialog from './components/WidgetAssignDialog.vue'
import LayoutPreview from './components/LayoutPreview.vue'
import type { RoleInfo } from '@/types/role'
import type { RoleWidgetConfig, AvailableWidget } from '@/types'
import { WidgetType, widgetTypeLabels } from '@/types/desktopWidget'

const loading = ref(false)
const saving = ref(false)
const dialogVisible = ref(false)

// 角色列表
const roleList = ref<RoleInfo[]>([])
const selectedRoleId = ref<string>('')

// 已分配组件列表
const widgetConfigList = ref<RoleWidgetConfig[]>([])

// 已分配组件ID列表
const assignedWidgetIds = computed(() => {
  return widgetConfigList.value.map(item => item.widgetId)
})

// 获取组件类型标签
const getWidgetTypeLabel = (type: WidgetType) => {
  return widgetTypeLabels[type] || '未知'
}

// 加载角色列表
const loadRoleList = async () => {
  try {
    const data = await getRoleList({
      pageIndex: 1,
      pageSize: 100,
      status: 1, // 只加载启用的角色
    })
    roleList.value = data.list
  } catch (error) {
    // Error handled by interceptor
  }
}

// 角色改变时加载配置
const handleRoleChange = async (roleId: string) => {
  if (!roleId) {
    widgetConfigList.value = []
    return
  }

  loading.value = true
  try {
    const data = await getRoleWidgetConfigList(roleId)
    widgetConfigList.value = data
  } catch (error) {
    widgetConfigList.value = []
  } finally {
    loading.value = false
  }
}

// 添加组件
const handleAddWidget = () => {
  dialogVisible.value = true
}

// 弹窗成功回调 - 添加选中的组件到列表
const handleDialogSuccess = (selectedWidgets: AvailableWidget[]) => {
  // 将选中的组件转换为配置项并添加到列表
  selectedWidgets.forEach(widget => {
    // 检查是否已经存在
    if (widgetConfigList.value.some(item => item.widgetId === widget.id)) {
      return
    }

    // 创建新的配置项
    const newConfig: RoleWidgetConfig = {
      id: '',
      roleId: selectedRoleId.value,
      widgetId: widget.id,
      widgetName: widget.name,
      widgetType: widget.type as WidgetType,
      defaultWidth: widget.defaultWidth,
      defaultHeight: widget.defaultHeight,
      sortOrder: widgetConfigList.value.length + 1,
      isEnabled: true,
    }
    widgetConfigList.value.push(newConfig)
  })

  ElMessage.success(`已添加 ${selectedWidgets.length} 个组件`)
}

// 移除组件
const handleRemoveWidget = async (row: RoleWidgetConfig) => {
  try {
    await ElMessageBox.confirm(
      `确定要移除组件"${row.widgetName}"吗？`,
      '提示',
      { type: 'warning' }
    )
    widgetConfigList.value = widgetConfigList.value.filter(
      item => item.widgetId !== row.widgetId
    )
    ElMessage.success('已移除')
  } catch (error) {
    // User cancelled
  }
}

// 保存配置
const handleSave = async () => {
  if (!selectedRoleId.value) {
    ElMessage.warning('请先选择角色')
    return
  }

  saving.value = true
  try {
    const params: SaveRoleWidgetConfigParams = {
      roleId: selectedRoleId.value,
      widgets: widgetConfigList.value.map(item => ({
        widgetId: item.widgetId,
        sortOrder: item.sortOrder,
        isEnabled: item.isEnabled,
      })),
    }
    await saveRoleWidgetConfig(params)
    ElMessage.success('保存成功')
  } catch (error) {
    // Error handled by interceptor
  } finally {
    saving.value = false
  }
}

// 重置
const handleReset = () => {
  handleRoleChange(selectedRoleId.value)
}

onMounted(() => {
  loadRoleList()
})
</script>

<style scoped lang="scss">
.role-config-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 16px;
    font-weight: 600;
  }

  .role-selector {
    margin-bottom: 20px;
  }

  .config-content {
    .section-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin: 20px 0 16px;
      font-size: 14px;
      font-weight: 500;
    }
  }

  .layout-preview-section {
    margin-top: 20px;
  }

  .action-bar {
    margin-top: 20px;
    display: flex;
    gap: 12px;
  }
}
</style>
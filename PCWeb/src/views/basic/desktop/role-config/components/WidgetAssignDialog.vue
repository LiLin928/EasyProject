<!-- 文件: PCWeb/src/views/basic/desktop/role-config/components/WidgetAssignDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    title="添加组件"
    width="600px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <div v-loading="loading" class="widget-list">
      <el-checkbox-group v-model="selectedWidgetIds">
        <div
          v-for="widget in availableWidgets"
          :key="widget.id"
          class="widget-item"
        >
          <el-checkbox
            :label="widget.id"
            :disabled="assignedWidgetIds.includes(widget.id)"
          >
            <div class="widget-info">
              <span class="widget-name">{{ widget.name }}</span>
              <el-tag size="small" class="widget-type">
                {{ getWidgetTypeLabel(widget.type) }}
              </el-tag>
              <span class="widget-size">
                {{ widget.defaultWidth }}栅格
              </span>
            </div>
          </el-checkbox>
          <el-tag
            v-if="assignedWidgetIds.includes(widget.id)"
            type="info"
            size="small"
            class="assigned-tag"
          >
            已分配
          </el-tag>
        </div>
      </el-checkbox-group>

      <el-empty v-if="availableWidgets.length === 0" description="暂无可用组件" />
    </div>

    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button
        type="primary"
        :disabled="selectedWidgetIds.length === 0"
        @click="handleConfirm"
      >
        确定 ({{ selectedWidgetIds.length }})
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { getAvailableWidgets, getRoleWidgetConfigList } from '@/api/basic/roleWidgetConfigApi'
import type { AvailableWidget, RoleWidgetConfig } from '@/types'
import { widgetTypeLabels, WidgetType } from '@/types/desktopWidget'

const props = defineProps<{
  modelValue: boolean
  roleId: string
  assignedWidgetIds: string[]
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success', selectedWidgets: AvailableWidget[]): void
}>()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const loading = ref(false)
const availableWidgets = ref<AvailableWidget[]>([])
const selectedWidgetIds = ref<string[]>([])

// 获取组件类型标签
const getWidgetTypeLabel = (type: number) => {
  return widgetTypeLabels[type as WidgetType] || '未知'
}

// 加载可用组件
const loadAvailableWidgets = async () => {
  if (!props.roleId) return

  loading.value = true
  try {
    const data = await getAvailableWidgets(props.roleId)
    availableWidgets.value = data
  } catch (error) {
    availableWidgets.value = []
  } finally {
    loading.value = false
  }
}

// 监听弹窗打开
watch(visible, (val) => {
  if (val && props.roleId) {
    loadAvailableWidgets()
    selectedWidgetIds.value = []
  }
})

// 关闭弹窗
const handleClose = () => {
  visible.value = false
}

// 确认添加
const handleConfirm = async () => {
  if (selectedWidgetIds.value.length === 0) {
    ElMessage.warning('请选择要添加的组件')
    return
  }

  // 从 availableWidgets 中筛选选中的组件
  const selectedWidgets = availableWidgets.value.filter(
    widget => selectedWidgetIds.value.includes(widget.id)
  )

  emit('success', selectedWidgets)
  handleClose()
}
</script>

<style scoped lang="scss">
.widget-list {
  max-height: 400px;
  overflow-y: auto;

  .widget-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 12px;
    border-bottom: 1px solid #ebeef5;

    &:last-child {
      border-bottom: none;
    }

    &:hover {
      background-color: #f5f7fa;
    }

    .widget-info {
      display: flex;
      align-items: center;
      gap: 8px;

      .widget-name {
        font-weight: 500;
      }

      .widget-type {
        margin-left: 8px;
      }

      .widget-size {
        color: #909399;
        font-size: 12px;
        margin-left: 8px;
      }
    }

    .assigned-tag {
      margin-left: 8px;
    }
  }
}
</style>
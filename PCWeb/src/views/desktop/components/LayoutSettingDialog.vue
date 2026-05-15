<!-- 文件: PCWeb/src/views/desktop/components/LayoutSettingDialog.vue -->

<template>
  <el-dialog
    v-model="visible"
    title="桌面布局设置"
    width="600px"
    :close-on-click-modal="false"
    @open="handleOpen"
  >
    <div class="layout-setting-container">
      <!-- 可用组件区域 -->
      <div class="available-widgets-section">
        <div class="section-title">可用组件</div>
        <div class="widget-checkbox-grid">
          <el-checkbox-group v-model="enabledWidgetIds">
            <el-checkbox
              v-for="widget in availableWidgets"
              :key="widget.id"
              :label="widget.id"
              class="widget-checkbox-item"
            >
              <div class="checkbox-content">
                <el-icon v-if="widget.icon" class="widget-icon">
                  <component :is="widget.icon" />
                </el-icon>
                <span class="widget-name">{{ widget.name }}</span>
              </div>
            </el-checkbox>
          </el-checkbox-group>
        </div>
      </div>

      <!-- 已启用组件区域 -->
      <div class="enabled-widgets-section">
        <div class="section-title">已启用组件（调整宽度）</div>
        <div v-if="enabledWidgetsList.length > 0" class="enabled-widgets-list">
          <div
            v-for="(widget, index) in enabledWidgetsList"
            :key="widget.widgetId"
            class="enabled-widget-item"
          >
            <div class="widget-info">
              <span class="widget-name">{{ widget.widgetName }}</span>
              <el-tag size="small" type="info">{{ widget.widgetType }}</el-tag>
            </div>
            <div class="widget-width-setting">
              <el-radio-group v-model="widget.width" size="small">
                <el-radio-button :label="3">25%</el-radio-button>
                <el-radio-button :label="4">33%</el-radio-button>
                <el-radio-button :label="6">50%</el-radio-button>
                <el-radio-button :label="8">67%</el-radio-button>
                <el-radio-button :label="12">100%</el-radio-button>
              </el-radio-group>
            </div>
            <div class="widget-order-setting">
              <el-input-number
                v-model="widget.sortOrder"
                :min="1"
                :max="99"
                size="small"
                controls-position="right"
              />
            </div>
          </div>
        </div>
        <el-empty v-else description="请先选择要显示的组件" :image-size="60" />
      </div>
    </div>

    <!-- 底部按钮 -->
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleReset">重置为默认</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">
          保存配置
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useDesktopStore } from '@/stores/desktopStore'
import type { UserWidgetConfigDto, UserWidgetConfigItem, AvailableWidget } from '@/types/desktopWidget'
import { widgetTypeLabels, WidgetType } from '@/types/desktopWidget'

// Props
const props = defineProps<{
  modelValue: boolean
}>()

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  'success': []
}>()

// Store
const desktopStore = useDesktopStore()

// Dialog visible
const visible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value),
})

// 可用组件列表
const availableWidgets = computed<AvailableWidget[]>(() => {
  return desktopStore.availableWidgets
})

// 启用的组件ID列表
const enabledWidgetIds = ref<string[]>([])

// 启用的组件配置列表（用于编辑）
const enabledWidgetsList = ref<
  Array<{
    widgetId: string
    widgetName: string
    widgetType: string
    width: number
    sortOrder: number
    isEnabled: boolean
  }>
>([])

// 保存状态
const saving = computed(() => desktopStore.saving)

// 对话框打开时初始化数据
function handleOpen() {
  // 初始化启用的组件ID列表
  enabledWidgetIds.value = desktopStore.widgets
    .filter(w => w.isEnabled)
    .map(w => w.widgetId)

  // 初始化启用组件的详细配置
  enabledWidgetsList.value = desktopStore.widgets
    .filter(w => w.isEnabled)
    .map(w => ({
      widgetId: w.widgetId,
      widgetName: w.widgetName,
      widgetType: widgetTypeLabels[w.widgetType] || '未知',
      width: w.width,
      sortOrder: w.sortOrder,
      isEnabled: true,
    }))
}

// 监听启用组件ID变化，更新启用组件列表
watch(enabledWidgetIds, (newIds) => {
  // 找出新增的组件
  const addedIds = newIds.filter(id => !enabledWidgetsList.value.some(w => w.widgetId === id))

  // 找出移除的组件
  const removedIds = enabledWidgetsList.value
    .filter(w => !newIds.includes(w.widgetId))
    .map(w => w.widgetId)

  // 添加新组件
  addedIds.forEach(id => {
    const availableWidget = availableWidgets.value.find(w => w.id === id)
    if (availableWidget) {
      enabledWidgetsList.value.push({
        widgetId: id,
        widgetName: availableWidget.name,
        widgetType: widgetTypeLabels[availableWidget.type] || '未知',
        width: availableWidget.defaultWidth,
        sortOrder: enabledWidgetsList.value.length + 1,
        isEnabled: true,
      })
    }
  })

  // 移除组件
  enabledWidgetsList.value = enabledWidgetsList.value.filter(w => !removedIds.includes(w.widgetId))

  // 重新排序
  enabledWidgetsList.value.forEach((w, index) => {
    w.sortOrder = index + 1
  })
}, { deep: true })

// 保存配置
async function handleSave() {
  // 构建保存参数
  const items: UserWidgetConfigItem[] = enabledWidgetsList.value.map(w => ({
    widgetId: w.widgetId,
    width: w.width,
    isEnabled: true,
    sortOrder: w.sortOrder,
  }))

  // 未选中的组件也需要保存（设置为禁用）
  availableWidgets.value
    .filter(w => !enabledWidgetIds.value.includes(w.id))
    .forEach((w, index) => {
      items.push({
        widgetId: w.id,
        width: w.defaultWidth,
        isEnabled: false,
        sortOrder: enabledWidgetsList.value.length + index + 1,
      })
    })

  await desktopStore.saveConfig(items)
  emit('success')
  visible.value = false
}

// 重置为默认配置
async function handleReset() {
  await desktopStore.resetToDefault()
  emit('success')
  visible.value = false
}
</script>

<style scoped lang="scss">
.layout-setting-container {
  .section-title {
    font-size: 14px;
    font-weight: 600;
    color: #303133;
    margin-bottom: 12px;
    padding-left: 10px;
    border-left: 3px solid #409eff;
  }

  .available-widgets-section {
    margin-bottom: 24px;

    .widget-checkbox-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
      gap: 12px;

      .widget-checkbox-item {
        margin: 0;

        .checkbox-content {
          display: flex;
          align-items: center;
          gap: 8px;

          .widget-icon {
            font-size: 16px;
            color: #409eff;
          }

          .widget-name {
            font-size: 13px;
          }
        }
      }
    }
  }

  .enabled-widgets-section {
    .enabled-widgets-list {
      .enabled-widget-item {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 12px;
        margin-bottom: 12px;
        border: 1px solid #ebeef5;
        border-radius: 4px;
        background-color: #fafafa;

        .widget-info {
          display: flex;
          align-items: center;
          gap: 8px;
          min-width: 120px;

          .widget-name {
            font-size: 13px;
            color: #303133;
          }
        }

        .widget-width-setting {
          display: flex;
          align-items: center;

          .el-radio-button {
            margin-right: 4px;
          }
        }

        .widget-order-setting {
          display: flex;
          align-items: center;
          min-width: 100px;
        }
      }
    }
  }
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>
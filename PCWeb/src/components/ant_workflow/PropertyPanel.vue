<!-- src/components/ant_workflow/DagDesigner/PropertyPanel.vue -->
<template>
  <div class="property-panel">
    <el-card shadow="never">
      <template #header>
        <div class="panel-header">
          <span>{{ node?.name || t('antWorkflow.nodeConfig.nodeName') }}</span>
          <el-button link type="primary" @click="handleClose">
            <el-icon><Close /></el-icon>
          </el-button>
        </div>
      </template>

      <!-- 基础信息 -->
      <el-form :model="formData" label-width="80px" class="base-form">
        <el-form-item :label="t('antWorkflow.nodeConfig.nodeName')">
          <el-input
            v-model="formData.name"
            :placeholder="t('antWorkflow.nodeConfig.nodeNamePlaceholder')"
            @change="handleNameChange"
          />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeType')">
          <el-tag :type="getNodeTagType(node?.type)">
            {{ node?.type ? getNodeTypeName(node.type) : '-' }}
          </el-tag>
        </el-form-item>
      </el-form>

      <!-- 节点配置表单（根据类型动态加载） -->
      <el-divider />
      <div class="config-section">
        <component
          :is="configComponent"
          v-if="configComponent"
          :config="formData.config"
          @update="handleConfigUpdate"
        />
        <el-empty v-else :description="t('antWorkflow.designer.selectNodeTip')" :image-size="60" />
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, shallowRef } from 'vue'
import { Close } from '@element-plus/icons-vue'
import type { Graph } from '@antv/x6'
import { AntNodeType, getNodeTypeName } from '@/types/antWorkflow'
import { useLocale } from '@/composables/useLocale'
import type { DagNode } from '@/stores/antWorkflowStore'

// 导入节点配置组件
import StartNodeConfig from './nodes/StartNodeConfig.vue'
import ApproverNodeConfig from './nodes/ApproverNodeConfig.vue'
import CopyerNodeConfig from './nodes/CopyerNodeConfig.vue'
import ConditionNodeConfig from './nodes/ConditionNodeConfig.vue'
import ParallelNodeConfig from './nodes/ParallelNodeConfig.vue'
import ServiceNodeConfig from './nodes/ServiceNodeConfig.vue'
import NotificationNodeConfig from './nodes/NotificationNodeConfig.vue'
import WebhookNodeConfig from './nodes/WebhookNodeConfig.vue'
import SubflowNodeConfig from './nodes/SubflowNodeConfig.vue'
import CounterSignNodeConfig from './nodes/CounterSignNodeConfig.vue'
import EndNodeConfig from './nodes/EndNodeConfig.vue'

const { t } = useLocale()

// Props
const props = defineProps<{
  node: DagNode | null
  graph: Graph | null
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', node: DagNode): void
  (e: 'close'): void
}>()

// 表单数据
const formData = ref({
  name: '',
  config: {} as any,
})

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

// 配置组件映射
const configComponentMap: Record<AntNodeType, any> = {
  [AntNodeType.START]: StartNodeConfig,
  [AntNodeType.APPROVER]: ApproverNodeConfig,
  [AntNodeType.COPYER]: CopyerNodeConfig,
  [AntNodeType.CONDITION]: ConditionNodeConfig,
  [AntNodeType.PARALLEL]: ParallelNodeConfig,
  [AntNodeType.SERVICE]: ServiceNodeConfig,
  [AntNodeType.NOTIFICATION]: NotificationNodeConfig,
  [AntNodeType.WEBHOOK]: WebhookNodeConfig,
  [AntNodeType.SUBFLOW]: SubflowNodeConfig,
  [AntNodeType.COUNTER_SIGN]: CounterSignNodeConfig,
  [AntNodeType.END]: EndNodeConfig,
}

// 计算当前配置组件
const configComponent = shallowRef<any>(null)

// 监听节点变化，更新表单数据
watch(
  () => props.node,
  (newNode) => {
    if (newNode) {
      // 设置标记，防止循环更新
      isUpdatingFromProps.value = true

      formData.value = {
        name: newNode.name,
        config: newNode.config ? JSON.parse(JSON.stringify(newNode.config)) : {},
      }
      configComponent.value = configComponentMap[newNode.type]

      // 下一个 tick 后重置标记
      setTimeout(() => {
        isUpdatingFromProps.value = false
      }, 0)
    }
  },
  { immediate: true }
)

// ========== 工具函数 ==========

const getNodeTagType = (type?: AntNodeType): 'success' | 'primary' | 'warning' | 'info' | 'danger' => {
  if (!type) return 'info'
  switch (type) {
    case AntNodeType.START:
      return 'success'
    case AntNodeType.APPROVER:
      return 'primary'
    case AntNodeType.CONDITION:
      return 'warning'
    case AntNodeType.COUNTER_SIGN:
      return 'danger'
    case AntNodeType.END:
      return 'info'
    default:
      return 'info'
  }
}

// ========== 事件处理 ==========

const handleNameChange = () => {
  if (!isUpdatingFromProps.value) {
    emitUpdate()
  }
}

const handleConfigUpdate = (config: any) => {
  if (!isUpdatingFromProps.value) {
    formData.value.config = config
    emitUpdate()
  }
}

const emitUpdate = () => {
  if (props.node && !isUpdatingFromProps.value) {
    const updatedNode: DagNode = {
      ...props.node,
      name: formData.value.name,
      config: formData.value.config,
    }
    emit('update', updatedNode)
  }
}

const handleClose = () => {
  emit('close')
}
</script>

<style scoped lang="scss">
$panel-width: 400px;
$panel-bg: #fff;
$panel-border: #e4e7ed;

.property-panel {
  width: $panel-width;
  flex-shrink: 0; // 不允许收缩
  height: 100%;
  overflow-y: auto;
  overflow-x: hidden;
  background: $panel-bg;
  border-left: 1px solid $panel-border;

  :deep(.el-card) {
    height: 100%;
    border: none;
    border-radius: 0;

    .el-card__header {
      padding: 16px;
      border-bottom: 1px solid $panel-border;
    }

    .el-card__body {
      padding: 16px;
    }
  }

  .panel-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 14px;
    font-weight: 500;
    color: #303133;
  }

  .base-form {
    margin-bottom: 0;
  }

  .config-section {
    min-height: 200px;
  }
}

// 滚动条样式
.property-panel::-webkit-scrollbar {
  width: 6px;
}

.property-panel::-webkit-scrollbar-thumb {
  background: rgba(0, 0, 0, 0.2);
  border-radius: 3px;

  &:hover {
    background: rgba(0, 0, 0, 0.3);
  }
}

.property-panel::-webkit-scrollbar-track {
  background: transparent;
}
</style>
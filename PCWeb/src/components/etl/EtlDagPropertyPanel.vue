<!-- src/components/etl/DagDesigner/PropertyPanel.vue -->
<template>
  <div class="property-panel">
    <!-- 面板头部 -->
    <div class="panel-header">
      <div class="header-left">
        <div class="header-title">
          <span class="node-icon">{{ nodeStyle?.icon }}</span>
          <span class="node-type">{{ nodeStyle?.label }}</span>
        </div>
        <div class="node-id">
          ID: {{ selectedNode?.id?.slice(0, 8) }}...
        </div>
      </div>
      <el-button link type="primary" @click="handleClose">
        <el-icon><Close /></el-icon>
      </el-button>
    </div>

    <!-- 基础信息 -->
    <el-card shadow="never" class="config-card">
      <template #header>
        <span>{{ t('etl.dag.node.basicConfig') }}</span>
      </template>
      <el-form
        ref="baseFormRef"
        :model="formData"
        label-width="80px"
        size="small"
      >
        <el-form-item :label="t('etl.dag.node.nodeName')" prop="name">
          <el-input
            v-model="formData.name"
            :placeholder="t('etl.dag.node.nodeNamePlaceholder')"
            @change="handleNameChange"
          />
        </el-form-item>
        <el-form-item :label="t('etl.dag.node.retryTimes')" prop="retryTimes">
          <el-input-number
            v-model="formData.retryTimes"
            :min="0"
            :max="10"
            :placeholder="t('etl.dag.node.retryTimesPlaceholder')"
            @change="handleConfigChange"
          />
        </el-form-item>
        <el-form-item :label="t('etl.dag.node.retryInterval')" prop="retryInterval">
          <el-input-number
            v-model="formData.retryInterval"
            :min="0"
            :max="60"
            :placeholder="t('etl.dag.node.retryIntervalPlaceholder')"
            @change="handleConfigChange"
          />
        </el-form-item>
        <el-form-item :label="t('etl.dag.node.timeout')" prop="timeout">
          <el-input-number
            v-model="formData.timeout"
            :min="0"
            :max="3600"
            :placeholder="t('etl.dag.node.timeoutPlaceholder')"
            @change="handleConfigChange"
          />
        </el-form-item>
        <el-form-item :label="t('etl.dag.node.skipOnFailure')" prop="skipOnFailure">
          <el-switch
            v-model="formData.skipOnFailure"
            @change="handleConfigChange"
          />
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 动态类型配置组件 -->
    <component
      v-if="currentConfigComponent"
      :is="currentConfigComponent"
      :config="formData.config"
      @update="handleTypeConfigUpdate"
    />

    <!-- 未实现的配置面板占位 -->
    <el-card
      v-if="!currentConfigComponent"
      shadow="never"
      class="config-card placeholder-card"
    >
      <el-empty :description="t('etl.dag.node.developing')" :image-size="60" />
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, inject, type Component } from 'vue'
import type { FormInstance } from 'element-plus'
import { Close } from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import { TaskNodeType } from '@/types/etl'
import type { DagNode } from '@/types/etl'
import { nodeStyleMap } from './utils/nodeRegistry'
import type { Graph, Node } from '@antv/x6'

// 引入外部配置组件
import DataSourceConfig from './configs/DataSourceConfig.vue'
import SqlConfig from './configs/SqlConfig.vue'
import TransformConfig from './configs/TransformConfig.vue'
import OutputConfig from './configs/OutputConfig.vue'
import ApiConfig from './configs/ApiConfig.vue'
import FileConfig from './configs/FileConfig.vue'
import ScriptConfig from './configs/ScriptConfig.vue'
import ConditionConfig from './configs/ConditionConfig.vue'
import ParallelConfig from './configs/ParallelConfig.vue'
import NotificationConfig from './configs/NotificationConfig.vue'
import SubflowConfig from './configs/SubflowConfig.vue'

const { t } = useLocale()

// Props
const props = defineProps<{
  /** 选中的节点 */
  selectedNode: DagNode | null
  /** 图实例（用于更新节点） */
  graph?: Graph
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', node: DagNode): void
  (e: 'close'): void
}>()

// 注入历史记录
const dagHistory = inject<any>('dagHistory')

// 表单引用
const baseFormRef = ref<FormInstance>()

// 表单数据
const formData = ref({
  name: '',
  retryTimes: 0,
  retryInterval: 0,
  timeout: 0,
  skipOnFailure: false,
  config: {} as any,
})

// 节点样式
const nodeStyle = computed(() => {
  if (!props.selectedNode) return null
  return nodeStyleMap[props.selectedNode.type]
})

// 组件映射表
const configComponentMap: Record<TaskNodeType, Component> = {
  [TaskNodeType.DATASOURCE]: DataSourceConfig,
  [TaskNodeType.SQL]: SqlConfig,
  [TaskNodeType.TRANSFORM]: TransformConfig,
  [TaskNodeType.OUTPUT]: OutputConfig,
  [TaskNodeType.API]: ApiConfig,
  [TaskNodeType.FILE]: FileConfig,
  [TaskNodeType.SCRIPT]: ScriptConfig,
  [TaskNodeType.CONDITION]: ConditionConfig,
  [TaskNodeType.PARALLEL]: ParallelConfig,
  [TaskNodeType.NOTIFICATION]: NotificationConfig,
  [TaskNodeType.SUBFLOW]: SubflowConfig,
}

// 当前配置组件
const currentConfigComponent = computed(() => {
  if (!props.selectedNode) return null
  return configComponentMap[props.selectedNode.type] || null
})

// 监听选中节点变化，初始化表单数据
watch(
  () => props.selectedNode,
  (node) => {
    if (node) {
      formData.value = {
        name: node.name,
        retryTimes: node.retryTimes || 0,
        retryInterval: node.retryInterval || 0,
        timeout: node.timeout || 0,
        skipOnFailure: node.skipOnFailure || false,
        config: node.config || {},
      }
    }
  },
  { immediate: true }
)

// 处理名称变更
const handleNameChange = () => {
  updateNodeInGraph()
  emit('update', {
    ...props.selectedNode!,
    name: formData.value.name,
  })
}

// 处理配置变更
const handleConfigChange = () => {
  updateNodeInGraph()
  emit('update', {
    ...props.selectedNode!,
    retryTimes: formData.value.retryTimes,
    retryInterval: formData.value.retryInterval,
    timeout: formData.value.timeout,
    skipOnFailure: formData.value.skipOnFailure,
  })
}

// 处理类型配置更新
const handleTypeConfigUpdate = (config: any) => {
  formData.value.config = config
  updateNodeInGraph()
  emit('update', {
    ...props.selectedNode!,
    config,
  })
}

// 更新图中的节点
const updateNodeInGraph = () => {
  if (!props.graph || !props.selectedNode) return

  const node = props.graph.getCellById(props.selectedNode.id) as Node
  if (node) {
    node.setData({
      nodeType: props.selectedNode.type,
      name: formData.value.name,
      config: formData.value.config,
    })
    dagHistory?.pushHistory()
  }
}

// 关闭面板
const handleClose = () => {
  emit('close')
}
</script>

<style scoped lang="scss">
$panel-width: 560px;
$panel-bg: #fff;
$panel-border: #e4e7ed;

.property-panel {
  width: $panel-width;
  flex-shrink: 0;
  height: 100%;
  overflow-y: auto;
  overflow-x: hidden;
  background: $panel-bg;
  border-left: 1px solid $panel-border;
}

.panel-header {
  padding: 16px;
  border-bottom: 1px solid $panel-border;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;

  .header-left {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  .header-title {
    display: flex;
    align-items: center;
    gap: 8px;

    .node-icon {
      font-size: 18px;
    }

    .node-type {
      font-size: 14px;
      font-weight: 500;
      color: #303133;
    }
  }

  .node-id {
    font-size: 11px;
    color: #909399;
  }
}

.config-card {
  margin: 16px;
  border: none;

  :deep(.el-card__header) {
    padding: 12px 16px;
    font-size: 13px;
    font-weight: 500;
    color: #303133;
    border-bottom: 1px solid $panel-border;
  }

  :deep(.el-card__body) {
    padding: 16px;
  }
}

.placeholder-card {
  margin: 16px;
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
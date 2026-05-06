<!-- src/components/etl/DagDesigner/EdgeConfigPanel.vue -->
<template>
  <div class="edge-config-panel">
    <!-- 面板头部 -->
    <div class="panel-header">
      <div class="header-left">
        <div class="header-title">
          <span class="edge-icon">→</span>
          <span class="edge-type">{{ t('etl.dag.edge.title') }}</span>
        </div>
        <div class="edge-id">
          ID: {{ selectedEdge?.id?.slice(0, 8) }}...
        </div>
      </div>
      <el-button link type="primary" @click="handleClose">
        <el-icon><Close /></el-icon>
      </el-button>
    </div>

    <!-- 基本信息 -->
    <el-card shadow="never" class="config-card">
      <template #header>
        <span>{{ t('etl.dag.edge.basicInfo') }}</span>
      </template>
      <el-form label-width="80px" size="small">
        <el-form-item :label="t('etl.dag.edge.sourceNode')">
          <div class="node-info">
            <span class="node-name">{{ sourceNode?.name || t('common.status.unknown') }}</span>
            <span class="node-id-tag">{{ sourceNode?.id.slice(0, 8) }}</span>
          </div>
        </el-form-item>
        <el-form-item :label="t('etl.dag.edge.targetNode')">
          <div class="node-info">
            <span class="node-name">{{ targetNode?.name || t('common.status.unknown') }}</span>
            <span class="node-id-tag">{{ targetNode?.id.slice(0, 8) }}</span>
          </div>
        </el-form-item>
        <el-form-item :label="t('etl.dag.edge.sourcePort')">
          <el-input
            v-model="localEdge.sourcePort"
            :placeholder="t('etl.dag.edge.optional')"
            @change="handleEdgeUpdate"
          />
        </el-form-item>
        <el-form-item :label="t('etl.dag.edge.targetPort')">
          <el-input
            v-model="localEdge.targetPort"
            :placeholder="t('etl.dag.edge.optional')"
            @change="handleEdgeUpdate"
          />
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 条件配置 -->
    <el-card shadow="never" class="config-card">
      <template #header>
        <div class="condition-header">
          <span>{{ t('etl.dag.edge.conditionConfig') }}</span>
          <el-switch
            v-model="enableCondition"
            size="small"
            @change="handleConditionToggle"
          />
        </div>
      </template>

      <template v-if="enableCondition">
        <!-- 条件类型选择 -->
        <el-form label-width="80px" size="small">
          <el-form-item :label="t('etl.dag.edge.conditionType')">
            <el-radio-group v-model="conditionType" @change="handleConditionTypeChange">
              <el-radio value="expression">{{ t('etl.dag.edge.expression') }}</el-radio>
              <el-radio value="rules">{{ t('etl.dag.edge.rules') }}</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-form>

        <!-- 表达式模式 -->
        <div v-if="conditionType === 'expression'" class="expression-section">
          <el-form label-width="80px" size="small">
            <el-form-item :label="t('etl.dag.edge.expression')">
              <el-input
                v-model="localCondition.expression"
                type="textarea"
                :rows="3"
                :placeholder="t('etl.dag.edge.expressionPlaceholder')"
                @change="handleConditionUpdate"
              />
            </el-form-item>
          </el-form>
          <div class="expression-hint">
            <el-icon><InfoFilled /></el-icon>
            <span>{{ t('etl.dag.edge.variableHint') }}</span>
          </div>
        </div>

        <!-- 规则列表模式 -->
        <div v-if="conditionType === 'rules'" class="rules-section">
          <div class="rules-header">
            <span class="rules-label">{{ t('etl.dag.edge.ruleList') }}</span>
            <el-button
              type="primary"
              size="small"
              link
              @click="addRule"
            >
              <el-icon><Plus /></el-icon>
              {{ t('etl.dag.edge.addRule') }}
            </el-button>
          </div>

          <div v-if="localCondition.rules && localCondition.rules.length > 0" class="rules-list">
            <div
              v-for="(rule, index) in localCondition.rules"
              :key="index"
              class="rule-item"
            >
              <div class="rule-index">{{ index + 1 }}</div>
              <el-form class="rule-form" size="small">
                <el-form-item :label="t('etl.dag.edge.fieldName')">
                  <el-input
                    v-model="rule.field"
                    :placeholder="t('etl.dag.edge.fieldName')"
                    @change="handleConditionUpdate"
                  />
                </el-form-item>
                <el-form-item :label="t('common.button.select')">
                  <el-select
                    v-model="rule.operator"
                    :placeholder="t('etl.dag.edge.selectOperator')"
                    @change="handleConditionUpdate"
                  >
                    <el-option :label="t('etl.dag.operator.eq')" value="eq" />
                    <el-option :label="t('etl.dag.operator.ne')" value="ne" />
                    <el-option :label="t('etl.dag.operator.gt')" value="gt" />
                    <el-option :label="t('etl.dag.operator.lt')" value="lt" />
                    <el-option :label="t('etl.dag.operator.gte')" value="gte" />
                    <el-option :label="t('etl.dag.operator.lte')" value="lte" />
                    <el-option :label="t('etl.dag.operator.contains')" value="contains" />
                    <el-option :label="t('etl.dag.operator.regex')" value="regex" />
                  </el-select>
                </el-form-item>
                <el-form-item :label="t('etl.dag.edge.compareValue')">
                  <el-input
                    v-model="rule.value"
                    :placeholder="t('etl.dag.edge.compareValue')"
                    @change="handleConditionUpdate"
                  />
                </el-form-item>
              </el-form>
              <el-button
                type="danger"
                size="small"
                link
                class="rule-delete"
                @click="removeRule(index)"
              >
                <el-icon><Delete /></el-icon>
              </el-button>
            </div>
          </div>

          <div v-else class="rules-empty">
            <el-empty :description="t('etl.dag.edge.noRules')" :image-size="60" />
          </div>
        </div>
      </template>

      <!-- 条件禁用提示 -->
      <template v-else>
        <div class="condition-disabled">
          <el-icon><Warning /></el-icon>
          <span>{{ t('etl.dag.edge.conditionDisabled') }}</span>
        </div>
      </template>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Close, InfoFilled, Plus, Delete, Warning } from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import type { DagEdge, DagNode, EdgeCondition } from '@/types/etl'

const { t } = useLocale()

// Props
const props = defineProps<{
  /** 当前选中的边 */
  selectedEdge: DagEdge | null
  /** 源节点信息 */
  sourceNode: DagNode | null
  /** 目标节点信息 */
  targetNode: DagNode | null
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', edge: DagEdge): void
  (e: 'close'): void
}>()

// 本地边数据
const localEdge = ref<DagEdge>({
  id: '',
  sourceNodeId: '',
  targetNodeId: '',
  sourcePort: '',
  targetPort: '',
  condition: undefined,
})

// 本地条件数据
const localCondition = ref<EdgeCondition>({
  expression: '',
  rules: [],
})

// 是否启用条件
const enableCondition = ref(false)

// 条件类型
const conditionType = ref<'expression' | 'rules'>('expression')

// 监听选中边变化，初始化数据
watch(
  () => props.selectedEdge,
  (edge) => {
    if (edge) {
      localEdge.value = {
        id: edge.id,
        sourceNodeId: edge.sourceNodeId,
        targetNodeId: edge.targetNodeId,
        sourcePort: edge.sourcePort || '',
        targetPort: edge.targetPort || '',
        condition: edge.condition ? { ...edge.condition } : undefined,
      }

      // 初始化条件
      if (edge.condition) {
        enableCondition.value = true
        localCondition.value = {
          expression: edge.condition.expression || '',
          rules: edge.condition.rules ? [...edge.condition.rules] : [],
        }
        // 确定条件类型
        if (edge.condition.rules && edge.condition.rules.length > 0) {
          conditionType.value = 'rules'
        } else if (edge.condition.expression) {
          conditionType.value = 'expression'
        }
      } else {
        enableCondition.value = false
        localCondition.value = { expression: '', rules: [] }
        conditionType.value = 'expression'
      }
    }
  },
  { immediate: true }
)

// 处理边基本信息更新
const handleEdgeUpdate = () => {
  emit('update', {
    ...localEdge.value,
    condition: enableCondition.value ? localCondition.value : undefined,
  })
}

// 处理条件启用/禁用切换
const handleConditionToggle = (enabled: string | number | boolean) => {
  const isEnabled = Boolean(enabled)
  if (isEnabled) {
    // 启用条件时，初始化默认配置
    localCondition.value = {
      expression: '',
      rules: [],
    }
    conditionType.value = 'expression'
  } else {
    // 禁用条件时，清空条件数据
    localCondition.value = { expression: '', rules: [] }
  }
  handleConditionUpdate()
}

// 处理条件类型切换
const handleConditionTypeChange = (type: string | number | boolean | undefined) => {
  const typeValue = type as 'expression' | 'rules'
  if (typeValue === 'expression') {
    // 切换到表达式模式，清空规则
    localCondition.value.rules = []
  } else {
    // 切换到规则模式，清空表达式
    localCondition.value.expression = ''
  }
  handleConditionUpdate()
}

// 处理条件更新
const handleConditionUpdate = () => {
  emit('update', {
    ...localEdge.value,
    condition: enableCondition.value ? localCondition.value : undefined,
  })
}

// 添加规则
const addRule = () => {
  if (!localCondition.value.rules) {
    localCondition.value.rules = []
  }
  localCondition.value.rules.push({
    field: '',
    operator: 'eq',
    value: '',
  })
  handleConditionUpdate()
}

// 删除规则
const removeRule = (index: number) => {
  if (localCondition.value.rules) {
    localCondition.value.rules.splice(index, 1)
    handleConditionUpdate()
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

.edge-config-panel {
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

    .edge-icon {
      font-size: 18px;
      color: #409eff;
    }

    .edge-type {
      font-size: 14px;
      font-weight: 500;
      color: #303133;
    }
  }

  .edge-id {
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

.node-info {
  display: flex;
  align-items: center;
  gap: 8px;

  .node-name {
    font-size: 13px;
    color: #303133;
  }

  .node-id-tag {
    font-size: 11px;
    color: #909399;
    background: #f4f4f5;
    padding: 2px 6px;
    border-radius: 4px;
  }
}

.condition-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.expression-section {
  margin-top: 12px;

  .expression-hint {
    display: flex;
    align-items: center;
    gap: 4px;
    margin-top: 8px;
    font-size: 12px;
    color: #909399;
    padding-left: 16px;
  }
}

.rules-section {
  margin-top: 12px;

  .rules-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 12px;

    .rules-label {
      font-size: 13px;
      color: #303133;
    }
  }

  .rules-list {
    display: flex;
    flex-direction: column;
    gap: 12px;
  }

  .rule-item {
    display: flex;
    align-items: flex-start;
    gap: 8px;
    padding: 12px;
    background: #f5f7fa;
    border-radius: 4px;
    border: 1px solid $panel-border;

    .rule-index {
      width: 20px;
      height: 20px;
      background: #409eff;
      color: #fff;
      border-radius: 50%;
      display: flex;
      justify-content: center;
      align-items: center;
      font-size: 12px;
      font-weight: 500;
    }

    .rule-form {
      flex: 1;

      :deep(.el-form-item) {
        margin-bottom: 8px;
      }

      :deep(.el-form-item__label) {
        width: 60px !important;
      }
    }

    .rule-delete {
      margin-top: 8px;
    }
  }

  .rules-empty {
    padding: 20px;
  }
}

.condition-disabled {
  display: flex;
  align-items: center;
  gap: 4px;
  color: #909399;
  font-size: 13px;
  padding: 16px;
}

// 滚动条样式
.edge-config-panel::-webkit-scrollbar {
  width: 6px;
}

.edge-config-panel::-webkit-scrollbar-thumb {
  background: rgba(0, 0, 0, 0.2);
  border-radius: 3px;

  &:hover {
    background: rgba(0, 0, 0, 0.3);
  }
}

.edge-config-panel::-webkit-scrollbar-track {
  background: transparent;
}
</style>
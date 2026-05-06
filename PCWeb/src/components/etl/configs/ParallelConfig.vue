<!-- src/components/etl/DagDesigner/configs/ParallelConfig.vue -->
<template>
  <el-card shadow="never" class="config-card">
    <template #header>
      <span>{{ t('etl.dag.node.parallelConfig') }}</span>
    </template>
    <el-form
      ref="formRef"
      :model="localConfig"
      :rules="formRules"
      label-width="80px"
      size="small"
      :disabled="readonly"
    >
      <!-- 并行类型选择 -->
      <el-form-item :label="t('etl.dag.node.parallelType')" prop="parallelType">
        <el-select
          v-model="localConfig.parallelType"
          :placeholder="t('etl.dag.node.selectParallelType')"
          style="width: 100%"
          @change="handleParallelTypeChange"
        >
          <el-option
            v-for="type in parallelTypeOptions"
            :key="type.value"
            :label="type.label"
            :value="type.value"
          >
            <div style="display: flex; justify-content: space-between; align-items: center">
              <span>{{ type.label }}</span>
              <el-tag size="small" type="info">{{ type.desc }}</el-tag>
            </div>
          </el-option>
        </el-select>
      </el-form-item>

      <!-- 等待模式选择 -->
      <el-form-item :label="t('etl.dag.node.waitMode')" prop="waitMode">
        <el-select
          v-model="localConfig.waitMode"
          :placeholder="t('etl.dag.node.selectWaitMode')"
          style="width: 100%"
          @change="emitUpdate"
        >
          <el-option
            v-for="mode in waitModeOptions"
            :key="mode.value"
            :label="mode.label"
            :value="mode.value"
          >
            <div style="display: flex; justify-content: space-between; align-items: center">
              <span>{{ mode.label }}</span>
              <el-tag size="small" type="info">{{ mode.desc }}</el-tag>
            </div>
          </el-option>
        </el-select>
      </el-form-item>

      <!-- 固定分支配置 -->
      <template v-if="localConfig.parallelType === 'fixed'">
        <el-divider content-position="left">{{ t('etl.dag.node.branches') }}</el-divider>
        <el-form-item>
          <BranchEditor
            v-model="localConfig.branches!"
            :allow-default="false"
            :readonly="readonly"
            @update:model-value="emitUpdate"
          />
          <div class="form-tip">
            {{ t('etl.dag.node.branchesTip') }}
          </div>
        </el-form-item>
      </template>

      <!-- 动态分支配置 -->
      <template v-if="localConfig.parallelType === 'dynamic'">
        <el-divider content-position="left">{{ t('etl.dag.node.dynamicBranch') }}</el-divider>
        <el-form-item :label="t('etl.dag.node.dynamicSource')" prop="dynamicSource">
          <VariableInput
            v-model="localConfig.dynamicSource!"
            :placeholder="t('etl.dag.node.dynamicSourcePlaceholder')"
            :disabled="readonly"
            @update:model-value="emitUpdate"
          />
          <div class="form-tip">
            {{ t('etl.dag.node.dynamicSourceTip') }}
          </div>
        </el-form-item>
      </template>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import type { ParallelNodeConfig } from '@/types/etl/taskNode'
import { BranchEditor, VariableInput } from './shared'

const { t } = useLocale()

/**
 * ParallelConfig 组件
 *
 * 用于配置并行节点的执行方式：
 * - 固定分支：预定义多个并行分支
 * - 动态分支：根据变量动态创建分支
 * - 等待模式：全部完成或任意完成
 */

// Props
const props = defineProps<{
  /** 配置数据 */
  config: ParallelNodeConfig
  /** 只读模式 */
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', config: ParallelNodeConfig): void
}>()

// 表单引用
const formRef = ref<FormInstance>()

// 并行类型选项
const parallelTypeOptions = computed(() => [
  { label: t('etl.dag.node.fixedBranch'), value: 'fixed', desc: t('etl.dag.node.fixedBranchDesc') },
  { label: t('etl.dag.node.dynamicBranch'), value: 'dynamic', desc: t('etl.dag.node.dynamicBranchDesc') },
])

// 等待模式选项
const waitModeOptions = computed(() => [
  { label: t('etl.dag.node.waitAll'), value: 'all', desc: t('etl.dag.node.waitAllDesc') },
  { label: t('etl.dag.node.waitAny'), value: 'any', desc: t('etl.dag.node.waitAnyDesc') },
])

// 本地配置
const localConfig = ref<ParallelNodeConfig>({
  parallelType: 'fixed',
  branches: [{ id: '', name: '' }],
  dynamicSource: '',
  waitMode: 'all',
})

// 表单验证规则
const formRules = computed<FormRules>(() => ({
  parallelType: [
    { required: true, message: t('etl.dag.node.parallelTypeRequired'), trigger: 'change' },
  ],
  waitMode: [
    { required: true, message: t('etl.dag.node.waitModeRequired'), trigger: 'change' },
  ],
  dynamicSource: [
    {
      required: localConfig.value.parallelType === 'dynamic',
      message: t('etl.dag.node.dynamicSourceRequired'),
      trigger: 'blur',
    },
  ],
}))

// 监听 props 变化
watch(
  () => props.config,
  (newConfig) => {
    if (newConfig) {
      localConfig.value = {
        parallelType: newConfig.parallelType || 'fixed',
        branches: newConfig.branches || [],
        dynamicSource: newConfig.dynamicSource || '',
        waitMode: newConfig.waitMode || 'all',
      }

      // 初始化默认分支
      if (localConfig.value.parallelType === 'fixed' && (!localConfig.value.branches || localConfig.value.branches.length === 0)) {
        localConfig.value.branches = [{ id: '', name: '' }]
      }
    }
  },
  { immediate: true, deep: true }
)

// 处理并行类型变更
const handleParallelTypeChange = () => {
  // 切换类型时初始化对应配置
  if (localConfig.value.parallelType === 'fixed') {
    // 固定分支模式：初始化分支列表
    if (!localConfig.value.branches || localConfig.value.branches.length === 0) {
      localConfig.value.branches = []
    }
    // 清空动态来源
    localConfig.value.dynamicSource = ''
  } else {
    // 动态分支模式：初始化动态来源
    localConfig.value.dynamicSource = localConfig.value.dynamicSource || ''
    // 清空固定分支
    localConfig.value.branches = []
  }
  emitUpdate()
}

// 发送更新
const emitUpdate = () => {
  emit('update', { ...localConfig.value })
}

// 暴露验证方法
defineExpose({
  validate: () => formRef.value?.validate(),
  resetFields: () => formRef.value?.resetFields(),
})
</script>

<style scoped lang="scss">
.config-card {
  margin: 0 0 16px 0;
  border: none;

  :deep(.el-card__header) {
    padding: 12px 16px;
    font-size: 13px;
    font-weight: 500;
    color: #303133;
    border-bottom: 1px solid #e4e7ed;
  }

  :deep(.el-card__body) {
    padding: 16px;
  }
}

.form-tip {
  margin-top: 8px;
  font-size: 11px;
  color: #909399;
  line-height: 1.4;
}

:deep(.el-divider__text) {
  font-size: 12px;
  color: #606266;
}

:deep(.el-form-item) {
  margin-bottom: 16px;
}

:deep(.el-form-item__label) {
  font-size: 13px;
}
</style>
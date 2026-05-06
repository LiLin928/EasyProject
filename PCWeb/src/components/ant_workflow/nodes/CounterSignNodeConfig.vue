<!-- src/components/ant_workflow/nodes/CounterSignNodeConfig.vue -->
<template>
  <div class="counter-sign-node-config">
    <el-form :model="localConfig" label-width="100px">
      <el-form-item :label="t('antWorkflow.nodeConfig.counterSignConfig.userList')">
        <UserRoleSelector
          :selected="selectorTargets"
          mode="multiple"
          :allowed-types="[SelectorTargetType.USER, SelectorTargetType.ROLE]"
          @update="handleSelectorUpdate"
        />
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.counterSignConfig.passConditionType')">
        <el-radio-group v-model="localConfig.passCondition.type">
          <el-radio value="percent">{{ t('antWorkflow.nodeConfig.counterSignConfig.percentCondition') }}</el-radio>
          <el-radio value="count">{{ t('antWorkflow.nodeConfig.counterSignConfig.countCondition') }}</el-radio>
          <el-radio value="all">{{ t('antWorkflow.nodeConfig.counterSignConfig.allCondition') }}</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item v-if="localConfig.passCondition.type === 'percent'" :label="t('antWorkflow.nodeConfig.counterSignConfig.passPercent')">
        <el-input-number v-model="localConfig.passCondition.percent" :min="0" :max="100" />
      </el-form-item>
      <el-form-item v-if="localConfig.passCondition.type === 'count'" :label="t('antWorkflow.nodeConfig.counterSignConfig.passCount')">
        <el-input-number v-model="localConfig.passCondition.count" :min="1" />
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.counterSignConfig.timeout')">
        <el-input-number v-model="localConfig.timeout" :min="0" :max="72" />
        <el-select v-model="localConfig.timeoutAction" style="width: 120px; margin-left: 8px">
          <el-option label="自动通过" value="autoPass" />
          <el-option label="自动拒绝" value="autoReject" />
        </el-select>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { useLocale } from '@/composables/useLocale'
import UserRoleSelector from '../common/UserRoleSelector.vue'
import { SelectorTargetType, type SelectorTarget } from '@/types/antWorkflow/selector'

const { t } = useLocale()

interface CounterSignConfig {
  nodeUserList: NodeUser[]
  settype: number
  passCondition: { type: 'percent' | 'count' | 'all'; percent?: number; count?: number }
  timeout?: number
  timeoutAction?: 'autoPass' | 'autoReject'
}

interface NodeUser {
  targetId: string
  name: string
  type: number
}

const props = defineProps<{ config: CounterSignConfig }>()
const emit = defineEmits<{ (e: 'update', config: CounterSignConfig): void }>()

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

// 默认配置
const defaultConfig: CounterSignConfig = {
  nodeUserList: [],
  settype: 1,
  passCondition: { type: 'all' },
  timeout: 24,
  timeoutAction: 'autoReject',
}

const localConfig = ref<CounterSignConfig>({ ...defaultConfig })

// 将 nodeUserList 转换为 SelectorTarget 格式
const selectorTargets = computed<SelectorTarget[]>(() => {
  return (localConfig.value.nodeUserList || []).map((user) => ({
    id: user.targetId,
    name: user.name,
    type: user.type === 1 ? SelectorTargetType.USER : SelectorTargetType.ROLE,
  }))
})

// 处理选择器更新
const handleSelectorUpdate = (selected: SelectorTarget[]) => {
  localConfig.value.nodeUserList = selected.map((item) => ({
    targetId: item.id,
    name: item.name,
    type: item.type === SelectorTargetType.USER ? 1 : 2,
  }))
}

// 监听 props.config，更新 localConfig（防止循环，并确保默认值）
watch(() => props.config, (c) => {
  isUpdatingFromProps.value = true

  // 合并传入配置与默认配置，确保所有字段都有值
  localConfig.value = {
    nodeUserList: c?.nodeUserList || [],
    settype: c?.settype || 1,
    passCondition: c?.passCondition ? { ...c.passCondition } : { type: 'all' },
    timeout: c?.timeout || 24,
    timeoutAction: c?.timeoutAction || 'autoReject',
  }

  setTimeout(() => { isUpdatingFromProps.value = false }, 0)
}, { immediate: true })

// 监听 localConfig，emit update（防止循环）
watch(localConfig, (c) => {
  if (!isUpdatingFromProps.value) {
    emit('update', c)
  }
}, { deep: true })
</script>

<style scoped lang="scss">.counter-sign-node-config { padding: 0; }</style>
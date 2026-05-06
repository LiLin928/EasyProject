<!-- src/components/ant_workflow/nodes/ApproverNodeConfig.vue -->
<template>
  <div class="approver-node-config">
    <el-form :model="localConfig" label-width="100px">
      <el-form-item :label="t('antWorkflow.nodeConfig.approverConfig.setType')">
        <el-radio-group v-model="localConfig.settype">
          <el-radio :value="1">{{ t('antWorkflow.nodeConfig.approverConfig.fixedUser') }}</el-radio>
          <el-radio :value="2">{{ t('antWorkflow.nodeConfig.approverConfig.supervisor') }}</el-radio>
          <el-radio :value="4">{{ t('antWorkflow.nodeConfig.approverConfig.initiatorSelect') }}</el-radio>
          <el-radio :value="5">{{ t('antWorkflow.nodeConfig.approverConfig.initiatorSelf') }}</el-radio>
          <el-radio :value="7">{{ t('antWorkflow.nodeConfig.approverConfig.multiSupervisor') }}</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item v-if="localConfig.settype === 1" :label="t('antWorkflow.nodeConfig.approverConfig.userList')">
        <UserRoleSelector
          :selected="selectorTargets"
          mode="multiple"
          :allowed-types="[SelectorTargetType.USER, SelectorTargetType.ROLE, SelectorTargetType.FORM_FIELD]"
          @update="handleSelectorUpdate"
        />
      </el-form-item>
      <el-form-item v-if="localConfig.settype === 2 || localConfig.settype === 7" :label="t('antWorkflow.nodeConfig.approverConfig.directorLevel')">
        <el-input-number v-model="localConfig.directorLevel" :min="1" :max="10" />
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.approverConfig.examineMode')">
        <el-radio-group v-model="localConfig.examineMode">
          <el-radio :value="1">{{ t('antWorkflow.nodeConfig.approverConfig.sequential') }}</el-radio>
          <el-radio :value="2">{{ t('antWorkflow.nodeConfig.approverConfig.countersign') }}</el-radio>
          <el-radio :value="3">{{ t('antWorkflow.nodeConfig.approverConfig.orSign') }}</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.approverConfig.timeout')">
        <el-input-number v-model="localConfig.timeout" :min="0" :max="72" />
        <el-select v-model="localConfig.timeoutAction" style="width: 120px; margin-left: 8px">
          <el-option :label="t('antWorkflow.nodeConfig.approverConfig.autoPass')" value="autoPass" />
          <el-option :label="t('antWorkflow.nodeConfig.approverConfig.autoReject')" value="autoReject" />
          <el-option :label="t('antWorkflow.nodeConfig.approverConfig.transfer')" value="transfer" />
        </el-select>
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.approverConfig.noHandlerAction')">
        <el-radio-group v-model="localConfig.noHandlerAction">
          <el-radio :value="1">{{ t('antWorkflow.nodeConfig.approverConfig.autoPass') }}</el-radio>
          <el-radio :value="2">{{ t('antWorkflow.nodeConfig.approverConfig.transferToAdmin') }}</el-radio>
        </el-radio-group>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { useLocale } from '@/composables/useLocale'
import { ApproverSetType, ExamineMode, NoHandlerAction, type NodeUser } from '@/types/antWorkflow'
import UserRoleSelector from '../common/UserRoleSelector.vue'
import { SelectorTargetType, type SelectorTarget } from '@/types/antWorkflow/selector'

const { t } = useLocale()

interface ApproverConfig {
  settype: ApproverSetType
  nodeUserList: NodeUser[]
  directorLevel?: number
  examineMode?: ExamineMode
  noHandlerAction?: NoHandlerAction
  timeout?: number
  timeoutAction?: 'autoPass' | 'autoReject' | 'transfer'
}

const props = defineProps<{ config: ApproverConfig }>()
const emit = defineEmits<{ (e: 'update', config: ApproverConfig): void }>()

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

const localConfig = ref<ApproverConfig>({
  settype: ApproverSetType.FIXED_USER,
  nodeUserList: [],
  examineMode: ExamineMode.SEQUENTIAL,
  noHandlerAction: NoHandlerAction.TRANSFER,
  timeout: 0,
  timeoutAction: 'autoPass',
})

// Convert NodeUser[] to SelectorTarget[] for UserRoleSelector
const selectorTargets = computed<SelectorTarget[]>({
  get: () => {
    return (localConfig.value.nodeUserList || []).map((user) => ({
      id: user.targetId,
      name: user.name,
      type: user.type === 1 ? SelectorTargetType.USER : user.type === 2 ? SelectorTargetType.ROLE : SelectorTargetType.FORM_FIELD,
    }))
  },
  set: (targets: SelectorTarget[]) => {
    localConfig.value.nodeUserList = targets.map((target) => ({
      targetId: target.id,
      name: target.name,
      type: target.type === SelectorTargetType.USER ? 1 : target.type === SelectorTargetType.ROLE ? 2 : 3,
    }))
  },
})

const handleSelectorUpdate = (selected: SelectorTarget[]) => {
  selectorTargets.value = selected
}

// 监听 props.config，更新 localConfig（防止循环，并确保默认值）
watch(() => props.config, (c) => {
  isUpdatingFromProps.value = true

  // 合并传入配置与默认配置，确保所有字段都有值
  localConfig.value = {
    settype: c?.settype || ApproverSetType.FIXED_USER,
    nodeUserList: c?.nodeUserList || [],
    directorLevel: c?.directorLevel || 1,
    examineMode: c?.examineMode || ExamineMode.SEQUENTIAL,
    noHandlerAction: c?.noHandlerAction || NoHandlerAction.TRANSFER,
    timeout: c?.timeout || 0,
    timeoutAction: c?.timeoutAction || 'autoPass',
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

<style scoped lang="scss">.approver-node-config { padding: 0; }</style>
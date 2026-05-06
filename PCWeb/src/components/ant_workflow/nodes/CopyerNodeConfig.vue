<!-- src/components/ant_workflow/nodes/CopyerNodeConfig.vue -->
<template>
  <div class="copyer-node-config">
    <el-form :model="localConfig" label-width="80px">
      <el-form-item :label="t('antWorkflow.nodeConfig.copyerConfig.userList')">
        <UserRoleSelector
          :selected="selectorTargets"
          mode="multiple"
          :allowed-types="[SelectorTargetType.USER, SelectorTargetType.ROLE, SelectorTargetType.DEPT]"
          @update="handleSelectorUpdate"
        />
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.copyerConfig.allowSelfSelect')">
        <el-switch v-model="localConfig.allowSelfSelect" />
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { useLocale } from '@/composables/useLocale'
import UserRoleSelector from '../common/UserRoleSelector.vue'
import { SelectorTargetType, type SelectorTarget } from '@/types/antWorkflow/selector'
import type { NodeUser } from '@/types/antWorkflow'

const { t } = useLocale()

interface CopyerConfig { nodeUserList: NodeUser[]; allowSelfSelect?: boolean }

const props = defineProps<{ config: CopyerConfig }>()
const emit = defineEmits<{ (e: 'update', config: CopyerConfig): void }>()

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

const localConfig = ref<CopyerConfig>({ nodeUserList: [], allowSelfSelect: false })

// Convert NodeUser to SelectorTarget
const nodeUserToSelectorTarget = (user: NodeUser): SelectorTarget => ({
  id: user.targetId,
  name: user.name,
  type: user.type === 1 ? SelectorTargetType.USER : user.type === 2 ? SelectorTargetType.ROLE : SelectorTargetType.DEPT,
})

// Convert SelectorTarget to NodeUser
const selectorTargetToNodeUser = (target: SelectorTarget): NodeUser => ({
  targetId: target.id,
  name: target.name,
  type: target.type === SelectorTargetType.USER ? 1 : target.type === SelectorTargetType.ROLE ? 2 : 3,
})

// Computed selector targets from localConfig
const selectorTargets = computed<SelectorTarget[]>(() =>
  (localConfig.value.nodeUserList || []).map(nodeUserToSelectorTarget)
)

// Handle selector update
const handleSelectorUpdate = (targets: SelectorTarget[]) => {
  localConfig.value.nodeUserList = targets.map(selectorTargetToNodeUser)
}

// 监听 props.config，更新 localConfig（防止循环）
watch(() => props.config, (c) => {
  if (c) {
    isUpdatingFromProps.value = true
    localConfig.value = { nodeUserList: c.nodeUserList || [], allowSelfSelect: c.allowSelfSelect || false }
    setTimeout(() => { isUpdatingFromProps.value = false }, 0)
  }
}, { immediate: true })

// 监听 localConfig，emit update（防止循环）
watch(localConfig, (c) => {
  if (!isUpdatingFromProps.value) {
    emit('update', c)
  }
}, { deep: true })
</script>

<style scoped lang="scss">.copyer-node-config { padding: 0; }</style>
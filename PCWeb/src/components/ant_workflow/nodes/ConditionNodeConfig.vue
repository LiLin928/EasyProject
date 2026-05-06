<!-- src/components/ant_workflow/nodes/ConditionNodeConfig.vue -->
<template>
  <div class="condition-node-config">
    <el-form :model="localConfig" label-width="80px">
      <el-form-item :label="t('antWorkflow.nodeConfig.conditionConfig.branches')">
        <div v-for="(branch, idx) in localConfig.conditionNodes" :key="branch.id" class="branch-item">
          <el-input v-model="branch.name" :placeholder="t('antWorkflow.nodeConfig.conditionConfig.branchName')" style="width: 120px" />
          <el-checkbox v-model="branch.isDefault">{{ t('antWorkflow.nodeConfig.conditionConfig.defaultBranch') }}</el-checkbox>
          <el-button type="danger" link @click="removeBranch(idx)" v-if="!branch.isDefault">删除</el-button>
        </div>
        <el-button type="primary" link @click="addBranch">{{ t('antWorkflow.nodeConfig.conditionConfig.addBranch') }}</el-button>
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.conditionConfig.rules')">
        <div v-for="(branch, idx) in localConfig.conditionNodes" :key="branch.id" class="branch-rules">
          <div class="branch-header">{{ branch.name }}</div>
          <ConditionRuleEditor
            v-if="!branch.isDefault"
            :rules="branch.conditionRules"
            @update="(rules) => handleRulesUpdate(idx, rules)"
          />
          <el-empty v-else :description="t('antWorkflow.nodeConfig.conditionConfig.defaultBranchDesc')" :image-size="40" />
        </div>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useLocale } from '@/composables/useLocale'
import { generateGuid } from '@/utils/guid'
import ConditionRuleEditor from '../common/ConditionRuleEditor.vue'
import type { ConditionRule } from '@/types/antWorkflow/nodeTypes'

const { t } = useLocale()

interface ConditionBranch { id: string; name: string; priority: number; conditionRules: ConditionRule[]; isDefault?: boolean }
interface ConditionConfig { conditionNodes: ConditionBranch[] }

const props = defineProps<{ config: ConditionConfig }>()
const emit = defineEmits<{ (e: 'update', config: ConditionConfig): void }>()

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

// 默认分支配置（使用 GUID）
const createDefaultBranches = () => [
  { id: generateGuid(), name: '分支1', priority: 1, conditionRules: [], isDefault: false },
  { id: generateGuid(), name: '默认', priority: 2, conditionRules: [], isDefault: true },
]

const localConfig = ref<ConditionConfig>({ conditionNodes: createDefaultBranches() })

// 监听 props.config，更新 localConfig（防止循环）
watch(() => props.config, (c) => {
  if (c) {
    isUpdatingFromProps.value = true
    localConfig.value = { conditionNodes: c.conditionNodes || createDefaultBranches() }
    setTimeout(() => { isUpdatingFromProps.value = false }, 0)
  }
}, { immediate: true })

// 监听 localConfig，emit update（防止循环）
watch(localConfig, (c) => {
  if (!isUpdatingFromProps.value) {
    emit('update', c)
  }
}, { deep: true })

// 添加新分支
const addBranch = () => {
  const newPriority = localConfig.value.conditionNodes.length + 1
  localConfig.value.conditionNodes.push({
    id: generateGuid(),
    name: `分支${newPriority}`,
    priority: newPriority,
    conditionRules: [],
    isDefault: false,
  })
}

// 删除分支（不能删除默认分支）
const removeBranch = (idx: number) => {
  const branch = localConfig.value.conditionNodes[idx]
  if (!branch.isDefault) {
    localConfig.value.conditionNodes.splice(idx, 1)
    // 更新优先级
    localConfig.value.conditionNodes.forEach((b, i) => {
      b.priority = i + 1
    })
  }
}

// 更新分支的条件规则
const handleRulesUpdate = (idx: number, rules: ConditionRule[]) => {
  localConfig.value.conditionNodes[idx].conditionRules = rules
}
</script>

<style scoped lang="scss">
.condition-node-config { padding: 0; }
.branch-item { display: flex; align-items: center; gap: 8px; margin-bottom: 8px; }
.branch-rules {
  margin-bottom: 12px;
  padding: 8px;
  border: 1px solid #e4e7ed;
  border-radius: 4px;
  background-color: #fafafa;
  .branch-header {
    font-weight: 500;
    margin-bottom: 8px;
    color: #606266;
  }
}
</style>
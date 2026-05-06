<!-- src/components/ant_workflow/nodes/ParallelNodeConfig.vue -->
<template>
  <div class="parallel-node-config">
    <el-form :model="localConfig" label-width="80px">
      <el-form-item :label="t('antWorkflow.nodeConfig.parallelConfig.branches')">
        <div v-for="(branch, idx) in localConfig.parallelNodes" :key="branch.id" class="branch-item">
          <el-input v-model="branch.name" :placeholder="t('antWorkflow.nodeConfig.parallelConfig.branchName')" style="width: 150px" />
          <el-button type="danger" link @click="removeBranch(idx)" v-if="localConfig.parallelNodes.length > 2">删除</el-button>
        </div>
        <el-button type="primary" link @click="addBranch">{{ t('antWorkflow.nodeConfig.parallelConfig.addBranch') }}</el-button>
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.parallelConfig.completeCondition')">
        <el-radio-group v-model="localConfig.completeCondition">
          <el-radio value="all">{{ t('antWorkflow.nodeConfig.parallelConfig.allComplete') }}</el-radio>
          <el-radio value="any">{{ t('antWorkflow.nodeConfig.parallelConfig.anyComplete') }}</el-radio>
          <el-radio value="count">{{ t('antWorkflow.nodeConfig.parallelConfig.countComplete') }}</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item v-if="localConfig.completeCondition === 'count'" :label="t('antWorkflow.nodeConfig.parallelConfig.completeCount')">
        <el-input-number v-model="localConfig.completeCount" :min="1" :max="localConfig.parallelNodes.length" />
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useLocale } from '@/composables/useLocale'
import { generateGuid } from '@/utils/guid'

const { t } = useLocale()

interface ParallelBranch { id: string; name: string }
interface ParallelConfig { parallelNodes: ParallelBranch[]; completeCondition?: 'all' | 'any' | 'count'; completeCount?: number }

const props = defineProps<{ config: ParallelConfig }>()
const emit = defineEmits<{ (e: 'update', config: ParallelConfig): void }>()

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

// 默认分支配置（使用 GUID）
const createDefaultBranches = () => [
  { id: generateGuid(), name: '分支1' },
  { id: generateGuid(), name: '分支2' },
]

const localConfig = ref<ParallelConfig>({
  parallelNodes: createDefaultBranches(),
  completeCondition: 'all',
  completeCount: 1,
})

// 监听 props.config，更新 localConfig（防止循环）
watch(() => props.config, (c) => {
  if (c) {
    isUpdatingFromProps.value = true
    localConfig.value = {
      parallelNodes: c.parallelNodes || createDefaultBranches(),
      completeCondition: c.completeCondition || 'all',
      completeCount: c.completeCount || 1,
    }
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
  localConfig.value.parallelNodes.push({
    id: generateGuid(),
    name: `分支${localConfig.value.parallelNodes.length + 1}`,
  })
}

// 删除分支（至少保留 2 个分支）
const removeBranch = (idx: number) => {
  if (localConfig.value.parallelNodes.length > 2) {
    localConfig.value.parallelNodes.splice(idx, 1)
  }
}
</script>

<style scoped lang="scss">
.parallel-node-config { padding: 0; }
.branch-item { display: flex; align-items: center; gap: 8px; margin-bottom: 8px; }
</style>
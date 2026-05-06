<!-- src/components/ant_workflow/nodes/SubflowNodeConfig.vue -->
<template>
  <div class="subflow-node-config">
    <el-form :model="localConfig" label-width="100px">
      <el-form-item :label="t('antWorkflow.nodeConfig.subflowConfig.subflowId')">
        <el-select v-model="localConfig.subflowId" filterable :placeholder="t('antWorkflow.selectPlaceholder')" style="width: 100%">
          <el-option v-for="wf in workflowOptions" :key="wf.id" :label="wf.name" :value="wf.id" />
        </el-select>
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.subflowConfig.waitForCompletion')">
        <el-switch v-model="localConfig.waitForCompletion" />
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.subflowConfig.inputMappings')">
        <ParamMappingEditor :mappings="localConfig.inputMappings" @update="handleInputUpdate" />
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.subflowConfig.outputMappings')">
        <ParamMappingEditor :mappings="localConfig.outputMappings" @update="handleOutputUpdate" />
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useLocale } from '@/composables/useLocale'
import ParamMappingEditor from '../common/ParamMappingEditor.vue'

const { t } = useLocale()

interface ParamMapping { sourceField: string; targetField: string }
interface SubflowConfig {
  subflowId: string
  subflowName?: string
  waitForCompletion?: boolean
  inputMappings?: ParamMapping[]
  outputMappings?: ParamMapping[]
}

const props = defineProps<{ config: SubflowConfig }>()
const emit = defineEmits<{ (e: 'update', config: SubflowConfig): void }>()

const workflowOptions = ref([{ id: 'wf-001', name: '请假审批流程' }, { id: 'wf-002', name: '报销审批流程' }])

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

const localConfig = ref<SubflowConfig>({ subflowId: '', waitForCompletion: true, inputMappings: [], outputMappings: [] })

// 监听 props.config，更新 localConfig（防止循环，并确保默认值）
watch(() => props.config, (c) => {
  isUpdatingFromProps.value = true

  // 合并传入配置与默认配置，确保所有字段都有值
  localConfig.value = {
    subflowId: c?.subflowId || '',
    subflowName: c?.subflowName || '',
    waitForCompletion: c?.waitForCompletion ?? true,
    inputMappings: c?.inputMappings || [],
    outputMappings: c?.outputMappings || [],
  }

  setTimeout(() => { isUpdatingFromProps.value = false }, 0)
}, { immediate: true })

// 监听 localConfig，emit update（防止循环）
watch(localConfig, (c) => {
  if (!isUpdatingFromProps.value) {
    emit('update', c)
  }
}, { deep: true })

const handleInputUpdate = (mappings: ParamMapping[]) => {
  localConfig.value.inputMappings = mappings
}

const handleOutputUpdate = (mappings: ParamMapping[]) => {
  localConfig.value.outputMappings = mappings
}
</script>

<style scoped lang="scss">.subflow-node-config { padding: 0; }</style>
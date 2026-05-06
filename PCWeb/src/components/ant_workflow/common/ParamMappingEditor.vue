<template>
  <div class="param-mapping-editor">
    <div v-for="(item, idx) in mappings" :key="idx" class="mapping-row">
      <el-select v-model="item.sourceField" placeholder="源字段" filterable class="source-field">
        <el-option v-for="field in sourceFields" :key="field" :label="field" :value="field" />
      </el-select>
      <el-icon class="arrow-icon"><Right /></el-icon>
      <el-input v-model="item.targetField" placeholder="目标字段" class="target-field" />
      <el-button type="danger" link @click="handleRemove(idx)">
        <el-icon><Delete /></el-icon>
      </el-button>
    </div>
    <el-button type="primary" link @click="handleAdd">
      <el-icon><Plus /></el-icon>
      添加映射
    </el-button>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { Delete, Plus, Right } from '@element-plus/icons-vue'

interface ParamMapping { sourceField: string; targetField: string }

const props = defineProps<{ mappings?: ParamMapping[]; sourceFields?: string[] }>()
const emit = defineEmits<{ (e: 'update', mappings: ParamMapping[]): void }>()

const defaultSourceFields = ['applicant', 'dept', 'amount', 'reason', 'createTime']
const sourceFields = computed(() => props.sourceFields || defaultSourceFields)
const mappings = ref<ParamMapping[]>([])

watch(() => props.mappings, (val) => { mappings.value = val || [] }, { immediate: true })

const handleAdd = () => { mappings.value.push({ sourceField: '', targetField: '' }); emit('update', mappings.value) }
const handleRemove = (idx: number) => { mappings.value.splice(idx, 1); emit('update', mappings.value) }
</script>

<style scoped lang="scss">
.param-mapping-editor {
  .mapping-row { display: flex; align-items: center; gap: 8px; margin-bottom: 8px; }
  .source-field { width: 150px; }
  .arrow-icon { color: #909399; }
  .target-field { width: 150px; }
}
</style>
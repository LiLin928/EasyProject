<!-- src/components/ant_workflow/common/ConditionRuleEditor.vue -->
<template>
  <div class="condition-rule-editor">
    <div v-for="(rule, idx) in rules" :key="idx" class="rule-row">
      <el-select v-model="rule.fieldId" placeholder="字段" class="field-select" @change="handleFieldChange(idx)">
        <el-option v-for="field in fieldOptions" :key="field.id" :label="field.name" :value="field.id" />
      </el-select>
      <el-select v-model="rule.operator" placeholder="操作符" class="operator-select">
        <el-option v-for="op in operatorOptions" :key="op.value" :label="op.label" :value="op.value" />
      </el-select>
      <el-input v-if="showValueInput(rule.operator)" v-model="rule.value" placeholder="值" class="value-input" />
      <el-button type="danger" link @click="handleRemove(idx)">
        <el-icon><Delete /></el-icon>
      </el-button>
    </div>
    <el-button type="primary" link @click="handleAdd">
      <el-icon><Plus /></el-icon>
      添加条件
    </el-button>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { Delete, Plus } from '@element-plus/icons-vue'
import { ConditionOperator, type ConditionRule } from '@/types/antWorkflow'

interface FieldOption { id: string; name: string; type: 'string' | 'number' | 'date' | 'boolean' | 'array' }

const props = defineProps<{ rules?: ConditionRule[]; fields?: FieldOption[] }>()
const emit = defineEmits<{ (e: 'update', rules: ConditionRule[]): void }>()

const defaultFieldOptions: FieldOption[] = [
  { id: 'amount', name: '金额', type: 'number' },
  { id: 'dept', name: '部门', type: 'string' },
  { id: 'level', name: '级别', type: 'number' },
  { id: 'type', name: '类型', type: 'string' },
]

const fieldOptions = computed(() => props.fields || defaultFieldOptions)
const operatorOptions = [
  { value: ConditionOperator.EQ, label: '等于' },
  { value: ConditionOperator.NE, label: '不等于' },
  { value: ConditionOperator.GT, label: '大于' },
  { value: ConditionOperator.GTE, label: '大于等于' },
  { value: ConditionOperator.LT, label: '小于' },
  { value: ConditionOperator.LTE, label: '小于等于' },
  { value: ConditionOperator.CONTAINS, label: '包含' },
  { value: ConditionOperator.NOT_CONTAINS, label: '不包含' },
  { value: ConditionOperator.EMPTY, label: '为空' },
  { value: ConditionOperator.NOT_EMPTY, label: '不为空' },
]

const rules = ref<ConditionRule[]>([])

const showValueInput = (operator: ConditionOperator) => ![ConditionOperator.EMPTY, ConditionOperator.NOT_EMPTY].includes(operator)

watch(() => props.rules, (val) => { rules.value = val || [] }, { immediate: true })

const handleFieldChange = (idx: number) => {
  const field = fieldOptions.value.find(f => f.id === rules.value[idx].fieldId)
  if (field) { rules.value[idx].fieldName = field.name; rules.value[idx].fieldType = field.type }
}

const handleAdd = () => {
  rules.value.push({ fieldId: '', fieldName: '', fieldType: 'string', operator: ConditionOperator.EQ, value: '' })
  emit('update', rules.value)
}

const handleRemove = (idx: number) => { rules.value.splice(idx, 1); emit('update', rules.value) }
</script>

<style scoped lang="scss">
.condition-rule-editor {
  .rule-row { display: flex; align-items: center; gap: 8px; margin-bottom: 8px; }
  .field-select { width: 120px; }
  .operator-select { width: 100px; }
  .value-input { width: 150px; }
}
</style>
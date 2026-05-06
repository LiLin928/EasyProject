<!-- src/components/ant_workflow/common/KeyValueEditor.vue -->
<template>
  <div class="key-value-editor">
    <div v-for="(item, idx) in items" :key="idx" class="kv-row">
      <el-input v-model="item.key" placeholder="Key" class="kv-key" @change="handleChange" />
      <el-input v-model="item.value" placeholder="Value" class="kv-value" @change="handleChange" />
      <el-button type="danger" link @click="handleRemove(idx)">
        <el-icon><Delete /></el-icon>
      </el-button>
    </div>
    <el-button type="primary" link @click="handleAdd">
      <el-icon><Plus /></el-icon>
      添加
    </el-button>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Delete, Plus } from '@element-plus/icons-vue'

interface KeyValueItem { key: string; value: string }

const props = defineProps<{ data?: KeyValueItem[] }>()
const emit = defineEmits<{ (e: 'update', data: KeyValueItem[]): void; (e: 'change', data: KeyValueItem[]): void }>()

const items = ref<KeyValueItem[]>([])

watch(() => props.data, (val) => { items.value = val || [] }, { immediate: true })

const handleAdd = () => { items.value.push({ key: '', value: '' }); emit('update', items.value) }
const handleRemove = (idx: number) => { items.value.splice(idx, 1); handleChange() }
const handleChange = () => { emit('update', items.value); emit('change', items.value) }
</script>

<style scoped lang="scss">
.key-value-editor {
  .kv-row { display: flex; align-items: center; gap: 8px; margin-bottom: 8px; }
  .kv-key { width: 120px; }
  .kv-value { flex: 1; }
}
</style>
<!-- SQL 编辑器组件 -->
<template>
  <div class="sql-editor">
    <!-- 工具栏 -->
    <div class="sql-toolbar">
      <el-button size="small" @click="handleFormat">
        <el-icon><Document /></el-icon>
        格式化
      </el-button>
      <el-button size="small" @click="handleValidate" :loading="validating">
        <el-icon><Check /></el-icon>
        验证
      </el-button>
    </div>

    <!-- 编辑区域 -->
    <div class="sql-content">
      <div class="line-numbers" ref="lineNumbersRef">
        <span v-for="n in lineCount" :key="n">{{ n }}</span>
      </div>
      <textarea
        ref="textareaRef"
        v-model="sqlValue"
        class="sql-textarea"
        :placeholder="placeholder"
        @input="handleInput"
        @scroll="syncScroll"
        @keydown="handleKeydown"
      ></textarea>
    </div>

    <!-- 验证结果 -->
    <div v-if="validateResult" class="validate-result" :class="{ 'is-error': !validateResult.valid }">
      <el-icon v-if="validateResult.valid"><CircleCheck /></el-icon>
      <el-icon v-else><CircleClose /></el-icon>
      <span>{{ validateResult.valid ? 'SQL语法正确' : validateResult.message }}</span>
    </div>

    <!-- 查询结果预览 -->
    <div v-if="previewData" class="preview-section">
      <div class="preview-header">
        <span>数据预览 ({{ previewData.length }} 条)</span>
        <el-button size="small" text @click="previewData = null">
          <el-icon><Close /></el-icon>
        </el-button>
      </div>
      <el-table :data="previewData.slice(0, 10)" size="small" max-height="200">
        <el-table-column
          v-for="col in previewColumns"
          :key="col.name"
          :prop="col.name"
          :label="col.name"
          min-width="100"
        />
      </el-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue'
import { Document, Check, CircleCheck, CircleClose, Close } from '@element-plus/icons-vue'
import { validateSql } from '@/api/screen'

const props = defineProps<{
  modelValue: string
  datasourceId?: number
  placeholder?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'validate', result: { valid: boolean; columns?: { name: string; type: string }[] }): void
}>()

// 状态
const sqlValue = ref(props.modelValue || '')
const validating = ref(false)
const validateResult = ref<{ valid: boolean; message?: string } | null>(null)
const previewData = ref<any[] | null>(null)
const previewColumns = ref<{ name: string; type: string }[]>([])

// 引用
const textareaRef = ref<HTMLTextAreaElement>()
const lineNumbersRef = ref<HTMLDivElement>()

// 计算行数
const lineCount = computed(() => {
  return (sqlValue.value || '').split('\n').length
})

// 监听外部值变化
watch(() => props.modelValue, (val) => {
  if (val !== sqlValue.value) {
    sqlValue.value = val || ''
  }
})

// 输入处理
const handleInput = () => {
  emit('update:modelValue', sqlValue.value)
  validateResult.value = null
}

// 同步滚动
const syncScroll = () => {
  if (lineNumbersRef.value && textareaRef.value) {
    lineNumbersRef.value.scrollTop = textareaRef.value.scrollTop
  }
}

// Tab键处理
const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Tab') {
    event.preventDefault()
    const textarea = textareaRef.value
    if (textarea) {
      const start = textarea.selectionStart
      const end = textarea.selectionEnd
      const value = sqlValue.value
      sqlValue.value = value.substring(0, start) + '  ' + value.substring(end)
      nextTick(() => {
        textarea.selectionStart = textarea.selectionEnd = start + 2
      })
      emit('update:modelValue', sqlValue.value)
    }
  }
}

// 格式化SQL（简单实现）
const handleFormat = () => {
  let sql = sqlValue.value.trim()

  // 简单格式化：关键字换行
  const keywords = ['SELECT', 'FROM', 'WHERE', 'GROUP BY', 'ORDER BY', 'HAVING', 'LIMIT', 'JOIN', 'LEFT JOIN', 'RIGHT JOIN', 'INNER JOIN', 'ON', 'AND', 'OR']

  for (const keyword of keywords) {
    const regex = new RegExp(`\\b${keyword}\\b`, 'gi')
    sql = sql.replace(regex, '\n' + keyword)
  }

  sql = sql.replace(/^\n/, '').replace(/\n\s*\n/g, '\n')

  sqlValue.value = sql
  emit('update:modelValue', sqlValue.value)
}

// 验证SQL
const handleValidate = async () => {
  if (!props.datasourceId || !sqlValue.value.trim()) {
    return
  }

  validating.value = true
  try {
    const result = await validateSql({
      datasourceId: props.datasourceId,
      sql: sqlValue.value,
    })

    validateResult.value = {
      valid: result.valid,
      message: result.message,
    }

    emit('validate', { valid: result.valid, columns: result.columns })
  } catch (error) {
    validateResult.value = { valid: false, message: '验证失败' }
  } finally {
    validating.value = false
  }
}

// 设置预览数据
const setPreviewData = (data: any[], columns: { name: string; type: string }[]) => {
  previewData.value = data
  previewColumns.value = columns
}

// 暴露方法
defineExpose({
  setPreviewData,
  getSql: () => sqlValue.value,
})
</script>

<style scoped lang="scss">
.sql-editor {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.sql-toolbar {
  display: flex;
  gap: 8px;
}

.sql-content {
  display: flex;
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 4px;
  overflow: hidden;
  background: rgba(0, 0, 0, 0.2);
}

.line-numbers {
  display: flex;
  flex-direction: column;
  padding: 8px 0;
  width: 36px;
  background: rgba(255, 255, 255, 0.05);
  border-right: 1px solid rgba(255, 255, 255, 0.1);
  text-align: right;
  user-select: none;
  overflow-y: hidden;

  span {
    padding: 0 8px;
    font-size: 12px;
    line-height: 20px;
    color: rgba(255, 255, 255, 0.4);
    font-family: 'Consolas', 'Monaco', monospace;
  }
}

.sql-textarea {
  flex: 1;
  min-height: 150px;
  max-height: 300px;
  padding: 8px;
  border: none;
  background: transparent;
  color: #fff;
  font-size: 13px;
  font-family: 'Consolas', 'Monaco', monospace;
  line-height: 20px;
  resize: vertical;

  &::placeholder {
    color: rgba(255, 255, 255, 0.3);
  }

  &:focus {
    outline: none;
  }
}

.validate-result {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 12px;
  border-radius: 4px;
  font-size: 13px;
  background: rgba(103, 194, 58, 0.2);
  color: #67c23a;

  &.is-error {
    background: rgba(245, 108, 108, 0.2);
    color: #f56c6c;
  }
}

.preview-section {
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 4px;
  overflow: hidden;
}

.preview-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 12px;
  background: rgba(255, 255, 255, 0.05);
  font-size: 13px;
  color: rgba(255, 255, 255, 0.7);
}

:deep(.el-table) {
  --el-table-bg-color: transparent;
  --el-table-tr-bg-color: transparent;
  --el-table-header-bg-color: rgba(255, 255, 255, 0.05);
  --el-table-text-color: rgba(255, 255, 255, 0.8);
  --el-table-border-color: rgba(255, 255, 255, 0.1);
}
</style>
<!-- src/components/etl/DagDesigner/configs/shared/MonacoEditor.vue -->
<template>
  <div class="monaco-editor-wrapper" :class="{ 'is-fullscreen': isFullscreen }">
    <!-- 工具栏 -->
    <div v-if="showFormat || showFullscreen" class="monaco-toolbar">
      <el-button
        v-if="showFormat"
        size="small"
        @click="handleFormat"
      >
        <el-icon><Document /></el-icon>
        格式化
      </el-button>
      <el-button
        v-if="showFullscreen"
        size="small"
        @click="toggleFullscreen"
      >
        <el-icon>
          <FullScreen v-if="!isFullscreen" />
          <Close v-else />
        </el-icon>
        {{ isFullscreen ? '退出全屏' : '全屏' }}
      </el-button>
    </div>

    <!-- 编辑器容器 - 使用 VueMonacoEditor -->
    <div class="monaco-editor-container" :class="{ 'is-fullscreen': isFullscreen }">
      <VueMonacoEditor
        v-model:value="content"
        :language="language"
        :theme="theme"
        :height="editorHeight"
        :options="editorOptions"
        @change="handleChange"
        @mount="handleMount"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, shallowRef, nextTick } from 'vue'
import { VueMonacoEditor } from '@guolao/vue-monaco-editor'
import { format as formatSQL } from 'sql-formatter'
import { Document, FullScreen, Close } from '@element-plus/icons-vue'

/**
 * MonacoEditor - Monaco 编辑器封装组件
 *
 * 使用 @guolao/vue-monaco-editor 包装器，支持异步加载 Monaco
 * 避免 Monaco 大体积同步加载导致页面冻结
 *
 * 支持功能：
 * - v-model 双向绑定
 * - 多语言支持（sql、json、javascript、python、shell）
 * - 代码格式化（SQL、JSON）
 * - 只读模式
 * - 主题切换
 * - 全屏模式
 */

// Props 定义
interface Props {
  /** 编辑内容 (v-model) */
  modelValue: string
  /** 语言类型 */
  language?: 'sql' | 'json' | 'javascript' | 'python' | 'shell' | 'markdown' | 'text'
  /** 编辑器高度 */
  height?: string
  /** 只读模式 */
  readonly?: boolean
  /** 显示格式化按钮 */
  showFormat?: boolean
  /** 显示全屏按钮 */
  showFullscreen?: boolean
  /** 编辑器主题 */
  theme?: 'vs' | 'vs-dark' | 'hc-black'
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  language: 'sql',
  height: '200px',
  readonly: false,
  showFormat: false,
  showFullscreen: false,
  theme: 'vs',
})

// Emits 定义
const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'change', value: string): void
}>()

// 响应式状态
const content = ref(props.modelValue)
const isFullscreen = ref(false)
const editorInstance = shallowRef<any>(null)

// 计算编辑器高度
const editorHeight = computed(() => {
  if (isFullscreen.value) {
    return 'calc(100vh - 50px)'
  }
  return props.height
})

// 编辑器配置选项 - 使用 automaticLayout: true 以便正确响应尺寸变化
const editorOptions = computed(() => ({
  readOnly: props.readonly,
  minimap: { enabled: false },
  fontSize: 13,
  lineNumbers: 'on',
  scrollBeyondLastLine: false,
  wordWrap: 'on',
  folding: true,
  tabSize: 2,
  renderLineHighlight: 'line',
  cursorBlinking: 'smooth',
  smoothScrolling: true,
  padding: { top: 8, bottom: 8 },
  automaticLayout: true,
  contextmenu: true,
  scrollbar: {
    vertical: 'auto',
    horizontal: 'auto',
    verticalScrollbarSize: 12,
    horizontalScrollbarSize: 12,
  },
}))

// 监听 props.modelValue 变化，更新内部 content
watch(() => props.modelValue, (newValue) => {
  if (newValue !== content.value) {
    content.value = newValue
  }
})

// 编辑器挂载完成
const handleMount = (editor: any) => {
  editorInstance.value = editor
}

// 内容变化事件
const handleChange = (value: string | undefined) => {
  if (value !== undefined) {
    emit('update:modelValue', value)
    emit('change', value)
  }
}

/**
 * 格式化代码
 */
const handleFormat = () => {
  if (!editorInstance.value) return

  const value = content.value
  let formattedValue = value

  try {
    // 根据语言类型进行格式化
    if (props.language === 'sql') {
      formattedValue = formatSQL(value, {
        language: 'sql',
        tabWidth: 2,
        keywordCase: 'upper',
        linesBetweenQueries: 2,
      })
    } else if (props.language === 'json') {
      const jsonObj = JSON.parse(value)
      formattedValue = JSON.stringify(jsonObj, null, 2)
    }

    // 更新内容
    content.value = formattedValue
    emit('update:modelValue', formattedValue)
    emit('change', formattedValue)
  } catch (error) {
    console.error('Format error:', error)
    // 格式化失败时不做任何操作，保持原内容
  }
}

/**
 * 切换全屏模式
 */
const toggleFullscreen = () => {
  isFullscreen.value = !isFullscreen.value

  // 等待 DOM 更新后重新布局编辑器
  nextTick(() => {
    if (editorInstance.value) {
      editorInstance.value.layout()
    }
  })
}

/**
 * 获取编辑器内容
 */
const getContent = (): string => {
  return content.value
}

/**
 * 设置编辑器内容
 */
const setContent = (value: string) => {
  content.value = value
}

/**
 * 获取选中的文本
 */
const getSelection = (): string => {
  if (!editorInstance.value) return ''
  const selection = editorInstance.value.getSelection()
  if (!selection) return ''
  return editorInstance.value.getModel()?.getValueInRange(selection) || ''
}

/**
 * 插入文本到光标位置
 */
const insertText = (text: string) => {
  if (!editorInstance.value) return
  const position = editorInstance.value.getPosition()
  if (position) {
    editorInstance.value.executeEdits('', [
      {
        range: {
          startLineNumber: position.lineNumber,
          startColumn: position.column,
          endLineNumber: position.lineNumber,
          endColumn: position.column,
        },
        text,
      },
    ])
  }
}

/**
 * 聚焦编辑器
 */
const focus = () => {
  editorInstance.value?.focus()
}

// 暴露方法
defineExpose({
  getContent,
  setContent,
  getSelection,
  insertText,
  focus,
  format: handleFormat,
})
</script>

<style scoped lang="less">
.monaco-editor-wrapper {
  display: flex;
  flex-direction: column;
  width: 100%;
  position: relative;

  &.is-fullscreen {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    z-index: 9999;
    background: #fff;
    padding: 0;

    .monaco-toolbar {
      padding: 12px 16px;
      border-bottom: 1px solid #e4e7ed;
      margin-bottom: 0;
      background: #f5f7fa;
    }
  }
}

.monaco-toolbar {
  display: flex;
  gap: 8px;
  padding: 8px 0;
  border-bottom: 1px solid var(--el-border-color-light);
  margin-bottom: 8px;
  flex-shrink: 0;
}

.monaco-editor-container {
  width: 100%;
  border: 1px solid var(--el-border-color);
  border-radius: 4px;
  overflow: hidden;
  min-height: 200px;
}

.monaco-editor-container.is-fullscreen {
  position: fixed;
  top: 50px;
  left: 0;
  right: 0;
  bottom: 0;
  border: none;
  border-radius: 0;
}

// 深度选择器，调整 Monaco 编辑器样式
:deep(.monaco-editor) {
  .margin {
    background-color: #f5f7fa;
  }

  .monaco-editor-background {
    background-color: #ffffff;
  }
}

// 暗色主题适配
@media (prefers-color-scheme: dark) {
  :deep(.monaco-editor) {
    .margin {
      background-color: #1e1e1e;
    }

    .monaco-editor-background {
      background-color: #1e1e1e;
    }
  }
}
</style>
<template>
  <div class="wang-editor-container">
    <Toolbar
      :editor="editorRef"
      :defaultConfig="toolbarConfig"
      :mode="mode"
    />
    <Editor
      v-model="valueHtml"
      :defaultConfig="editorConfig"
      :mode="mode"
      @onCreated="handleCreated"
      @onChange="handleChange"
    />
  </div>
</template>

<script setup lang="ts">
import '@wangeditor/editor/dist/css/style.css'
import { ref, shallowRef, watch, onBeforeUnmount, computed } from 'vue'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import { getToken } from '@/utils/auth'

interface Props {
  modelValue: string
  height?: number
  disabled?: boolean
  placeholder?: string
  businessId?: string
}

const props = withDefaults(defineProps<Props>(), {
  height: 300,
  disabled: false,
  placeholder: '',
  businessId: '',
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'uploadSuccess', fileId: string, url: string): void
}>()

// 编辑器实例，必须用 shallowRef
const editorRef = shallowRef()

// HTML 内容
const valueHtml = ref(props.modelValue)

// 模式：default 或 simple
const mode = 'default'

// 工具栏配置
const toolbarConfig = {
  excludeKeys: [],
}

// 构建上传URL
const buildUploadUrl = () => {
  let uploadUrl = '/api/file/upload'
  const params = new URLSearchParams()
  if (props.businessId) {
    params.append('businessId', props.businessId)
  }
  if (params.toString()) {
    uploadUrl += '?' + params.toString()
  }
  return uploadUrl
}

// 编辑器配置
const editorConfig = computed(() => ({
  placeholder: props.placeholder,
  MENU_CONF: {
    // 上传图片配置
    uploadImage: {
      fieldName: 'file',
      server: buildUploadUrl(),
      maxFileSize: 5 * 1024 * 1024, // 5M
      allowedFileTypes: ['image/*'],
      headers: {
        Authorization: `Bearer ${getToken()}`
      },
      customInsert(res: any, insertFn: any) {
        // res 是服务器返回的数据，格式：{ code: 200, data: { url: ... } }
        if (res && res.code === 200 && res.data && res.data.url) {
          insertFn(res.data.url)
          // 通知父组件上传成功
          if (res.data.id) {
            emit('uploadSuccess', res.data.id, res.data.url)
          }
        }
      },
    },
  },
}))

// 监听 businessId 变化，需要重新配置上传URL
// WangEditor v5 不支持动态修改配置，需要销毁重建或使用其他方式
watch(() => props.businessId, (newId, oldId) => {
  console.log('WangEditor businessId changed:', oldId, '->', newId)
  // 如果有旧值且编辑器已创建，说明是在编辑过程中 businessId 变化了
  // 这种情况下需要手动处理（已在 AnnouncementEdit 中通过立即关联解决）
})

// 监听外部传入的值变化
watch(() => props.modelValue, (val) => {
  if (val !== valueHtml.value) {
    valueHtml.value = val
  }
})

// 内容变化时通知外部
const handleChange = (editor: any) => {
  emit('update:modelValue', editor.getHtml())
}

// 编辑器创建完成
const handleCreated = (editor: any) => {
  editorRef.value = editor
  // 设置高度
  if (props.height) {
    const editorElem = document.querySelector('.wang-editor-container .w-e-text-container')
    if (editorElem) {
      (editorElem as HTMLElement).style.height = `${props.height}px`
    }
  }
  // 禁用状态
  if (props.disabled) {
    editor.disable()
  }
  console.log('WangEditor initialized, businessId:', props.businessId, 'upload URL:', buildUploadUrl())
}

// 监听 disabled 变化
watch(() => props.disabled, (val) => {
  if (editorRef.value) {
    if (val) {
      editorRef.value.disable()
    } else {
      editorRef.value.enable()
    }
  }
})

// 组件销毁时，也销毁编辑器
onBeforeUnmount(() => {
  const editor = editorRef.value
  if (editor == null) return
  editor.destroy()
})
</script>

<style scoped lang="scss">
.wang-editor-container {
  width: 100%;
  border: 1px solid var(--el-border-color);
  border-radius: 4px;

  &:hover {
    border-color: var(--el-border-color-hover);
  }

  :deep(.w-e-toolbar) {
    border-bottom: 1px solid var(--el-border-color-lighter);
    background-color: #fff;
  }

  :deep(.w-e-text-container) {
    background-color: #fff;
    min-height: 200px;
  }

  :deep(.w-e-scroll) {
    min-height: 200px;
  }
}
</style>
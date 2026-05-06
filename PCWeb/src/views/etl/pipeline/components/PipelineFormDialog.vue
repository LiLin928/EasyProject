<!-- src/views/etl/pipeline/components/PipelineFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ isEdit ? t('etl.pipeline.form.editTitle') : t('etl.pipeline.form.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="visible = false">
            {{ t('etl.pipeline.form.cancel') }}
          </el-button>
          <el-button type="primary" :loading="loading" @click="handleSubmit">
            {{ t('etl.pipeline.form.save') }}
          </el-button>
        </div>
      </div>
    </template>
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="120px"
    >
      <el-form-item :label="t('etl.pipeline.form.name')" prop="name">
        <el-input
          v-model="formData.name"
          :placeholder="t('etl.pipeline.form.namePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('etl.pipeline.form.description')">
        <el-input
          v-model="formData.description"
          type="textarea"
          :rows="3"
          :placeholder="t('etl.pipeline.form.descriptionPlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('etl.pipeline.form.category')">
        <el-select
          v-model="formData.category"
          :placeholder="t('etl.pipeline.form.categoryPlaceholder')"
          style="width: 100%"
          clearable
        >
          <el-option label="数据同步" value="sync" />
          <el-option label="数据清洗" value="clean" />
          <el-option label="数据转换" value="transform" />
          <el-option label="数据分析" value="analysis" />
          <el-option label="报表生成" value="report" />
          <el-option label="其他" value="other" />
        </el-select>
      </el-form-item>

      <el-form-item :label="t('etl.pipeline.form.tags')">
        <el-select
          v-model="formData.tags"
          :placeholder="t('etl.pipeline.form.tagsPlaceholder')"
          style="width: 100%"
          multiple
          filterable
          allow-create
          default-first-option
        >
          <el-option label="重要" value="important" />
          <el-option label="定时" value="scheduled" />
          <el-option label="实时" value="realtime" />
          <el-option label="批量" value="batch" />
        </el-select>
      </el-form-item>

      <el-form-item :label="t('etl.pipeline.form.timeout')">
        <el-input-number
          v-model="formData.timeout"
          :placeholder="t('etl.pipeline.form.timeoutPlaceholder')"
          style="width: 100%"
          :min="0"
          :max="86400"
        />
        <div class="form-tip">{{ t('etl.pipeline.form.timeoutTip') }}</div>
      </el-form-item>

      <el-form-item :label="t('etl.pipeline.form.retryCount')">
        <el-input-number
          v-model="formData.retryCount"
          :placeholder="t('etl.pipeline.form.retryCountPlaceholder')"
          style="width: 100%"
          :min="0"
          :max="10"
        />
      </el-form-item>

      <el-form-item :label="t('etl.pipeline.form.concurrency')">
        <el-input-number
          v-model="formData.concurrency"
          :placeholder="t('etl.pipeline.form.concurrencyPlaceholder')"
          style="width: 100%"
          :min="1"
          :max="10"
        />
      </el-form-item>

      <el-form-item :label="t('etl.pipeline.form.failureStrategy')">
        <el-select
          v-model="formData.failureStrategy"
          :placeholder="t('etl.pipeline.form.failureStrategyPlaceholder')"
          style="width: 100%"
        >
          <el-option label="停止" value="stop" />
          <el-option label="继续" value="continue" />
          <el-option label="跳过并继续" value="skip" />
        </el-select>
      </el-form-item>
    </el-form>

    </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import {
  getPipelineDetail,
  createPipeline,
  updatePipeline,
} from '@/api/etl/pipelineApi'
import { useLocale } from '@/composables/useLocale'

const props = defineProps<{
  modelValue: boolean
  pipelineId?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const isEdit = computed(() => !!props.pipelineId)

const formRef = ref<FormInstance>()
const loading = ref(false)

const formData = reactive({
  name: '',
  description: '',
  category: '',
  tags: [] as string[],
  timeout: 3600,
  retryCount: 0,
  concurrency: 1,
  failureStrategy: 'stop',
})

const formRules = computed<FormRules>(() => ({
  name: [
    { required: true, message: t('etl.pipeline.form.validation.nameRequired'), trigger: 'blur' },
  ],
}))

// Load pipeline detail for edit
const loadPipelineDetail = async () => {
  if (!props.pipelineId) return
  try {
    const data = await getPipelineDetail(props.pipelineId)
    formData.name = data.name
    formData.description = data.description || ''
    formData.category = data.category || ''
    formData.tags = data.tags || []
    formData.timeout = data.timeout || 3600
    formData.retryCount = data.retryCount || 0
    formData.concurrency = data.concurrency || 1
    formData.failureStrategy = data.failureStrategy || 'stop'
  } catch (error) {
    ElMessage.error('Failed to load pipeline details')
  }
}

// Watch dialog open
watch(visible, (val) => {
  if (val) {
    if (props.pipelineId) {
      loadPipelineDetail()
    } else {
      resetForm()
    }
  }
})

const resetForm = () => {
  formData.name = ''
  formData.description = ''
  formData.category = ''
  formData.tags = []
  formData.timeout = 3600
  formData.retryCount = 0
  formData.concurrency = 1
  formData.failureStrategy = 'stop'
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    if (isEdit.value) {
      await updatePipeline({
        id: props.pipelineId!,
        name: formData.name,
        description: formData.description,
        category: formData.category,
        tags: formData.tags,
        timeout: formData.timeout,
        retryCount: formData.retryCount,
        concurrency: formData.concurrency,
        failureStrategy: formData.failureStrategy,
      })
      ElMessage.success(t('etl.pipeline.form.updateSuccess'))
    } else {
      await createPipeline({
        name: formData.name,
        description: formData.description,
        category: formData.category,
        tags: formData.tags,
        timeout: formData.timeout,
        retryCount: formData.retryCount,
        concurrency: formData.concurrency,
        failureStrategy: formData.failureStrategy,
      })
      ElMessage.success(t('etl.pipeline.form.createSuccess'))
    }
    visible.value = false
    emit('success')
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleClose = () => {
  formRef.value?.clearValidate()
  resetForm()
}
</script>

<style scoped lang="scss">
.dialog-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;

  .dialog-title {
    font-size: 18px;
    font-weight: 500;
    color: #303133;
  }

  .dialog-actions {
    display: flex;
    gap: 8px;
    margin-right: 30px;
  }
}

.form-tip {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}
</style>
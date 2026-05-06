<!-- src/views/etl/schedule/components/ScheduleFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="700px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ isEdit ? t('etl.schedule.form.editTitle') : t('etl.schedule.form.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="visible = false">
            {{ t('etl.schedule.form.cancel') }}
          </el-button>
          <el-button type="primary" :loading="loading" @click="handleSubmit">
            {{ t('etl.schedule.form.save') }}
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
      <el-form-item :label="t('etl.schedule.form.name')" prop="name">
        <el-input
          v-model="formData.name"
          :placeholder="t('etl.schedule.form.namePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('etl.schedule.form.pipeline')" prop="pipelineId">
        <el-select
          v-model="formData.pipelineId"
          :placeholder="t('etl.schedule.form.pipelinePlaceholder')"
          style="width: 100%"
          :disabled="isEdit"
        >
          <el-option
            v-for="option in pipelineOptions"
            :key="option.value"
            :label="option.label"
            :value="option.value"
          />
        </el-select>
      </el-form-item>

      <el-form-item :label="t('etl.schedule.form.description')">
        <el-input
          v-model="formData.description"
          type="textarea"
          :rows="2"
          :placeholder="t('etl.schedule.form.descriptionPlaceholder')"
        />
      </el-form-item>

      <el-divider content-position="left">
        {{ t('etl.schedule.form.scheduleConfig') }}
      </el-divider>

      <el-form-item :label="t('etl.schedule.form.scheduleType')" prop="scheduleType">
        <el-select
          v-model="formData.scheduleType"
          :placeholder="t('etl.schedule.form.scheduleTypePlaceholder')"
          style="width: 100%"
        >
          <el-option label="Cron 表达式" value="cron" />
          <el-option label="固定间隔" value="interval" />
          <el-option label="一次性执行" value="once" />
        </el-select>
      </el-form-item>

      <!-- Cron Expression - Visual Picker -->
      <template v-if="formData.scheduleType === 'cron'">
        <CronVisualPicker v-model="formData.cronExpression" />
      </template>

      <!-- Interval -->
      <template v-if="formData.scheduleType === 'interval'">
        <el-form-item :label="t('etl.schedule.form.interval')" prop="interval">
          <el-input-number
            v-model="formData.interval"
            :min="1"
            :max="86400"
            style="width: 200px"
          />
          <el-select v-model="formData.intervalUnit" style="width: 100px; margin-left: 8px">
            <el-option label="秒" value="seconds" />
            <el-option label="分钟" value="minutes" />
            <el-option label="小时" value="hours" />
          </el-select>
        </el-form-item>
      </template>

      <!-- One-time execution -->
      <template v-if="formData.scheduleType === 'once'">
        <el-form-item :label="t('etl.schedule.form.executeTime')" prop="executeTime">
          <el-date-picker
            v-model="formData.executeTime"
            type="datetime"
            :placeholder="t('etl.schedule.form.executeTimePlaceholder')"
            style="width: 100%"
          />
        </el-form-item>
      </template>

      <el-divider content-position="left">
        {{ t('etl.schedule.form.executionConfig') }}
      </el-divider>

      <el-form-item :label="t('etl.schedule.form.maxRetries')">
        <el-input-number
          v-model="formData.maxRetries"
          :min="0"
          :max="10"
          style="width: 100%"
        />
      </el-form-item>

      <el-form-item :label="t('etl.schedule.form.retryInterval')">
        <el-input-number
          v-model="formData.retryInterval"
          :min="0"
          :max="3600"
          style="width: 100%"
        />
        <div class="form-tip">{{ t('etl.schedule.form.retryIntervalTip') }}</div>
      </el-form-item>

      <el-form-item :label="t('etl.schedule.form.timeout')">
        <el-input-number
          v-model="formData.timeout"
          :min="0"
          :max="86400"
          style="width: 100%"
        />
        <div class="form-tip">{{ t('etl.schedule.form.timeoutTip') }}</div>
      </el-form-item>

      <el-form-item :label="t('etl.schedule.form.notifyOnSuccess')">
        <el-switch v-model="formData.notifyOnSuccess" />
      </el-form-item>

      <el-form-item :label="t('etl.schedule.form.notifyOnFailure')">
        <el-switch v-model="formData.notifyOnFailure" />
      </el-form-item>

      <el-form-item :label="t('etl.schedule.form.notifyEmails')">
        <div class="email-input-container">
          <el-input
            v-model="emailInput"
            :placeholder="t('etl.schedule.form.notifyEmailsPlaceholder')"
            @keyup.enter="handleAddEmail"
          >
            <template #append>
              <el-button @click="handleAddEmail">
                {{ t('common.button.add') }}
              </el-button>
            </template>
          </el-input>
          <div class="email-tags" v-if="formData.notifyEmails.length > 0">
            <el-tag
              v-for="(email, index) in formData.notifyEmails"
              :key="index"
              closable
              @close="handleRemoveEmail(index)"
              class="email-tag"
            >
              {{ email }}
            </el-tag>
          </div>
        </div>
      </el-form-item>
    </el-form>

    </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import {
  getScheduleDetail,
  createSchedule,
  updateSchedule,
} from '@/api/etl/scheduleApi'
import { getPipelineList } from '@/api/etl/pipelineApi'
import { useLocale } from '@/composables/useLocale'
import CronVisualPicker from './CronVisualPicker.vue'

const props = defineProps<{
  modelValue: boolean
  scheduleId?: string
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

const isEdit = computed(() => !!props.scheduleId)

const formRef = ref<FormInstance>()
const loading = ref(false)
const pipelineOptions = ref<Array<{ label: string; value: string }>>([])
const emailInput = ref('')

const formData = reactive({
  name: '',
  pipelineId: '',
  description: '',
  scheduleType: 'cron',
  cronExpression: '0 0 * * * ?',  // Quartz 6-field format
  interval: 1,
  intervalUnit: 'hours',
  executeTime: null as Date | null,
  maxRetries: 3,
  retryInterval: 60,
  timeout: 3600,
  notifyOnSuccess: false,
  notifyOnFailure: true,
  notifyEmails: [] as string[],
})

const formRules = computed<FormRules>(() => ({
  name: [
    { required: true, message: t('etl.schedule.form.validation.nameRequired'), trigger: 'blur' },
  ],
  pipelineId: [
    { required: true, message: t('etl.schedule.form.validation.pipelineRequired'), trigger: 'change' },
  ],
  scheduleType: [
    { required: true, message: t('etl.schedule.form.validation.scheduleTypeRequired'), trigger: 'change' },
  ],
  cronExpression: [
    { required: formData.scheduleType === 'cron', message: t('etl.schedule.form.validation.cronRequired'), trigger: 'blur' },
  ],
  interval: [
    { required: formData.scheduleType === 'interval', message: t('etl.schedule.form.validation.intervalRequired'), trigger: 'blur' },
  ],
  executeTime: [
    { required: formData.scheduleType === 'once', message: t('etl.schedule.form.validation.executeTimeRequired'), trigger: 'change' },
  ],
}))

// Load pipeline options
const loadPipelineOptions = async () => {
  try {
    const { list } = await getPipelineList({ pageIndex: 1, pageSize: 100 })
    pipelineOptions.value = list.map(p => ({
      label: p.name,
      value: p.id,
    }))
  } catch (error) {
    // Error handled by interceptor
  }
}

// Load schedule detail for edit
const loadScheduleDetail = async () => {
  if (!props.scheduleId) return
  try {
    const data = await getScheduleDetail(props.scheduleId)
    formData.name = data.name
    formData.pipelineId = data.pipelineId
    formData.description = data.description || ''
    formData.scheduleType = data.scheduleType || 'cron'
    formData.cronExpression = data.cronExpression || '0 0 * * * ?'  // Quartz 6-field format
    formData.interval = data.interval || 1
    formData.intervalUnit = data.intervalUnit || 'hours'
    formData.executeTime = data.executeTime ? new Date(data.executeTime) : null
    formData.maxRetries = data.maxRetries || 3
    formData.retryInterval = data.retryInterval || 60
    formData.timeout = data.timeout || 3600
    formData.notifyOnSuccess = data.notifyOnSuccess || false
    formData.notifyOnFailure = data.notifyOnFailure ?? true
    formData.notifyEmails = data.notifyEmails || []
  } catch (error) {
    ElMessage.error('Failed to load schedule details')
  }
}

// Watch dialog open
watch(visible, (val) => {
  if (val) {
    if (props.scheduleId) {
      loadScheduleDetail()
    } else {
      resetForm()
    }
  }
})

onMounted(() => {
  loadPipelineOptions()
})

const resetForm = () => {
  formData.name = ''
  formData.pipelineId = ''
  formData.description = ''
  formData.scheduleType = 'cron'
  formData.cronExpression = '0 0 * * * ?'  // Quartz 6-field format
  formData.interval = 1
  formData.intervalUnit = 'hours'
  formData.executeTime = null
  formData.maxRetries = 3
  formData.retryInterval = 60
  formData.timeout = 3600
  formData.notifyOnSuccess = false
  formData.notifyOnFailure = true
  formData.notifyEmails = []
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    // 计算间隔秒数
    let intervalSeconds: number | undefined = undefined
    if (formData.scheduleType === 'interval') {
      const unitMultiplier = {
        seconds: 1,
        minutes: 60,
        hours: 3600,
      }
      intervalSeconds = formData.interval * (unitMultiplier[formData.intervalUnit] || 1)
    }

    const submitData = {
      name: formData.name,
      pipelineId: formData.pipelineId,
      scheduleType: formData.scheduleType,
      cronExpression: formData.scheduleType === 'cron' ? formData.cronExpression : undefined,
      intervalSeconds: intervalSeconds,
      executeTime: formData.scheduleType === 'once' ? formData.executeTime?.toISOString() : undefined,
      executeParams: JSON.stringify({
        description: formData.description,
        maxRetries: formData.maxRetries,
        retryInterval: formData.retryInterval,
        timeout: formData.timeout,
        notifyOnSuccess: formData.notifyOnSuccess,
        notifyOnFailure: formData.notifyOnFailure,
        notifyEmails: formData.notifyEmails,
      }),
    }

    if (isEdit.value) {
      await updateSchedule({
        id: props.scheduleId!,
        ...submitData,
      })
      ElMessage.success(t('etl.schedule.form.updateSuccess'))
    } else {
      await createSchedule(submitData)
      ElMessage.success(t('etl.schedule.form.createSuccess'))
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
  emailInput.value = ''
}

// Email handling
const handleAddEmail = () => {
  const email = emailInput.value.trim()
  if (!email) return

  // Basic email validation
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailRegex.test(email)) {
    ElMessage.warning(t('etl.schedule.form.invalidEmail'))
    return
  }

  // Check if already added
  if (formData.notifyEmails.includes(email)) {
    ElMessage.warning(t('etl.schedule.form.emailAlreadyAdded'))
    return
  }

  formData.notifyEmails.push(email)
  emailInput.value = ''
}

const handleRemoveEmail = (index: number) => {
  formData.notifyEmails.splice(index, 1)
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

:deep(.el-divider__text) {
  font-weight: 600;
  color: #303133;
}

.email-input-container {
  width: 100%;

  .email-tags {
    display: flex;
    flex-wrap: wrap;
    gap: 8px;
    margin-top: 12px;
  }

  .email-tag {
    max-width: 200px;
    overflow: hidden;
    text-overflow: ellipsis;
  }
}
</style>
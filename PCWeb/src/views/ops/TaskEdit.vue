<!-- PCWeb/src/views/ops/TaskEdit.vue -->
<template>
  <div class="task-edit-container">
    <el-breadcrumb separator="/">
      <el-breadcrumb-item :to="{ path: '/ops/task' }">{{ t('ops.task.title') }}</el-breadcrumb-item>
      <el-breadcrumb-item>{{ isEdit ? t('ops.task.edit') : t('ops.task.create') }}</el-breadcrumb-item>
    </el-breadcrumb>

    <el-card shadow="never" class="edit-card">
      <template #header>
        <div class="card-header">
          <span>{{ isEdit ? t('ops.task.edit') : t('ops.task.create') }}</span>
          <div class="header-buttons">
            <el-button type="primary" :loading="saving" @click="handleSave">{{ t('common.button.save') }}</el-button>
            <el-button @click="handleCancel">{{ t('common.button.cancel') }}</el-button>
          </div>
        </div>
      </template>

      <el-form v-loading="loading" ref="formRef" :model="formData" :rules="formRules" label-width="120px">
        <!-- 基础信息 -->
        <div class="section-title">{{ t('ops.task.basicInfo') }}</div>
        <el-form-item :label="t('ops.task.form.taskName')" prop="taskName">
          <el-input v-model="formData.taskName" maxlength="100" show-word-limit />
        </el-form-item>
        <el-form-item :label="t('ops.task.form.taskGroup')">
          <el-select v-model="formData.taskGroup">
            <el-option label="Default" value="Default" />
            <el-option label="System" value="System" />
            <el-option label="Test" value="Test" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('ops.task.form.description')">
          <el-input v-model="formData.description" type="textarea" :rows="2" maxlength="500" />
        </el-form-item>

        <!-- 执行时间 - 使用 CronVisualPicker -->
        <div class="section-title">{{ t('ops.task.executeTime') }}</div>
        <CronVisualPicker v-model="formData.cronExpression" />

        <!-- 执行器配置 -->
        <div class="section-title">{{ t('ops.task.executor') }}</div>
        <el-form-item :label="t('ops.task.form.executorType')" prop="executorType">
          <el-radio-group v-model="formData.executorType">
            <el-radio :value="0">{{ t('ops.task.executorType.reflection') }}</el-radio>
            <el-radio :value="1">{{ t('ops.task.executorType.api') }}</el-radio>
          </el-radio-group>
        </el-form-item>

        <!-- 反射执行 -->
        <template v-if="formData.executorType === 0">
          <el-form-item :label="t('ops.task.form.handlerType')" prop="handlerType">
            <el-input v-model="formData.handlerType" placeholder="ClearLogJob" />
          </el-form-item>
          <el-form-item :label="t('ops.task.form.handlerMethod')">
            <el-input v-model="formData.handlerMethod" placeholder="ExecuteAsync" />
          </el-form-item>
        </template>

        <!-- API 调用 -->
        <template v-if="formData.executorType === 1">
          <el-form-item :label="t('ops.task.form.apiEndpoint')" prop="apiEndpoint">
            <el-input v-model="formData.apiEndpoint" placeholder="http://localhost/api/report" />
          </el-form-item>
          <el-form-item :label="t('ops.task.form.apiMethod')">
            <el-select v-model="formData.apiMethod">
              <el-option label="GET" value="GET" />
              <el-option label="POST" value="POST" />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('ops.task.form.apiPayload')">
            <el-input v-model="formData.apiPayload" type="textarea" :rows="3" placeholder="JSON" />
          </el-form-item>
        </template>

        <!-- 其他设置 -->
        <div class="section-title">{{ t('ops.task.otherSettings') }}</div>
        <el-form-item :label="t('ops.task.form.maxRetries')">
          <el-input-number v-model="formData.maxRetries" :min="0" :max="10" />
        </el-form-item>
        <el-form-item :label="t('ops.task.form.timeoutSeconds')">
          <el-input-number v-model="formData.timeoutSeconds" :min="30" :max="3600" :step="30" />
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { getTaskDetail, createTask, updateTask } from '@/api/ops/taskApi'
import { useLocale } from '@/composables/useLocale'
import type { UpdateTaskParams } from '@/types/task'
import CronVisualPicker from '@/views/etl/schedule/components/CronVisualPicker.vue'

const router = useRouter()
const route = useRoute()
const { t } = useLocale()

const formRef = ref<FormInstance | null>(null)
const loading = ref(false)
const saving = ref(false)
const isEdit = computed(() => !!route.params.id)

const formData = reactive<UpdateTaskParams>({
  taskName: '',
  taskGroup: 'Default',
  taskType: 0, // Cron 定时
  cronExpression: '0 0 8 * * ?', // Quartz 6-field: 默认每天 8:00
  executorType: 0,
  handlerType: '',
  handlerMethod: '',
  apiEndpoint: '',
  apiMethod: 'POST',
  apiPayload: '',
  maxRetries: 3,
  timeoutSeconds: 300,
  description: '',
})

const formRules = computed<FormRules>(() => ({
  taskName: [
    { required: true, message: t('ops.task.validation.taskNameRequired'), trigger: 'blur' },
    { min: 2, max: 100, message: t('ops.task.validation.taskNameLength'), trigger: 'blur' },
  ],
  executorType: [{ required: true, message: t('ops.task.validation.executorRequired'), trigger: 'change' }],
  handlerType: [
    { required: formData.executorType === 0, message: t('ops.task.validation.handlerRequired'), trigger: 'blur' },
  ],
  apiEndpoint: [
    { required: formData.executorType === 1, message: t('ops.task.validation.apiRequired'), trigger: 'blur' },
  ],
}))

onMounted(async () => {
  const id = route.params.id as string
  if (id) {
    await loadDetail(id)
  }
})

const loadDetail = async (id: string) => {
  loading.value = true
  try {
    const data = await getTaskDetail(id)
    Object.assign(formData, data)
    formData.id = id
    // 根据执行器类型设置 executorType
    formData.executorType = data.executorType ?? (data.handlerType ? 0 : 1)
  } catch (error) {
    ElMessage.error(t('common.message.loadFailed'))
    router.push('/ops/task')
  } finally {
    loading.value = false
  }
}

const handleSave = async () => {
  if (!formRef.value) return
  try {
    await formRef.value.validate()
  } catch {
    return
  }

  saving.value = true
  try {
    // 根据 CronVisualPicker 生成的表达式，自动设置 taskType 为 Cron
    formData.taskType = 0

    if (isEdit.value) {
      await updateTask(formData)
    } else {
      await createTask(formData)
    }
    ElMessage.success(t('common.message.saveSuccess'))
    router.push('/ops/task')
  } catch (error) {
    ElMessage.error(t('common.message.saveFailed'))
  } finally {
    saving.value = false
  }
}

const handleCancel = () => {
  router.push('/ops/task')
}
</script>

<style scoped lang="scss">
.task-edit-container {
  padding: 20px;

  .edit-card {
    margin-top: 16px;
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .header-buttons {
      display: flex;
      gap: 12px;
    }
  }

  .section-title {
    font-size: 16px;
    font-weight: 600;
    margin: 20px 0 16px;
    padding-bottom: 8px;
    border-bottom: 1px solid #ebeef5;
  }
}
</style>
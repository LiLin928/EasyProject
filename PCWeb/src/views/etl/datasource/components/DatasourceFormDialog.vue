<!-- src/views/etl/datasource/components/DatasourceFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ isEdit ? t('etl.datasource.form.editTitle') : t('etl.datasource.form.title') }}</span>
        <div class="dialog-actions">
          <el-button
            type="success"
            :loading="testLoading"
            :disabled="!formData.host || !formData.port || !formData.database || !formData.username"
            @click="handleTestConnection"
          >
            {{ t('etl.datasource.form.testConnection') }}
          </el-button>
          <el-button @click="visible = false">
            {{ t('etl.datasource.form.cancel') }}
          </el-button>
          <el-button type="primary" :loading="loading" @click="handleSubmit">
            {{ t('etl.datasource.form.save') }}
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
      <el-form-item :label="t('etl.datasource.form.name')" prop="name">
        <el-input
          v-model="formData.name"
          :placeholder="t('etl.datasource.form.namePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('etl.datasource.form.type')" prop="type">
        <el-select
          v-model="formData.type"
          :placeholder="t('etl.datasource.form.typePlaceholder')"
          style="width: 100%"
          :disabled="isEdit"
          clearable
        >
          <el-option :label="t('etl.datasource.type.mysql')" value="mysql" />
          <el-option :label="t('etl.datasource.type.postgresql')" value="postgresql" />
          <el-option :label="t('etl.datasource.type.oracle')" value="oracle" />
          <el-option :label="t('etl.datasource.type.sqlserver')" value="sqlserver" />
          <el-option :label="t('etl.datasource.type.clickhouse')" value="clickhouse" />
        </el-select>
      </el-form-item>

      <el-form-item :label="t('etl.datasource.form.host')" prop="host">
        <el-input
          v-model="formData.host"
          :placeholder="t('etl.datasource.form.hostPlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('etl.datasource.form.port')" prop="port">
        <el-input-number
          v-model="formData.port"
          :placeholder="t('etl.datasource.form.portPlaceholder')"
          style="width: 100%"
          :min="1"
          :max="65535"
        />
      </el-form-item>

      <el-form-item :label="t('etl.datasource.form.database')" prop="database">
        <el-input
          v-model="formData.database"
          :placeholder="t('etl.datasource.form.databasePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('etl.datasource.form.username')" prop="username">
        <el-input
          v-model="formData.username"
          :placeholder="t('etl.datasource.form.usernamePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('etl.datasource.form.password')" prop="password">
        <el-input
          v-model="formData.password"
          type="password"
          show-password
          :placeholder="t('etl.datasource.form.passwordPlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('etl.datasource.form.description')">
        <el-input
          v-model="formData.description"
          type="textarea"
          :rows="2"
          :placeholder="t('etl.datasource.form.descriptionPlaceholder')"
        />
      </el-form-item>
    </el-form>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import {
  getDatasourceDetail,
  createDatasource,
  updateDatasource,
  testDatasourceConnection,
} from '@/api/etl/datasourceApi'
import { useLocale } from '@/composables/useLocale'

const props = defineProps<{
  modelValue: boolean
  datasourceId?: string
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

const isEdit = computed(() => !!props.datasourceId)

const formRef = ref<FormInstance>()
const loading = ref(false)
const testLoading = ref(false)

// Default port for each database type
const defaultPorts: Record<string, number> = {
  mysql: 3306,
  postgresql: 5432,
  oracle: 1521,
  sqlserver: 1433,
  clickhouse: 8123,
}

const formData = reactive({
  name: '',
  type: 'mysql',
  host: '',
  port: 3306,
  database: '',
  username: '',
  password: '',
  description: '',
})

// Dynamic form rules
const formRules = computed<FormRules>(() => ({
  name: [
    { required: true, message: t('etl.datasource.form.validation.nameRequired'), trigger: 'blur' },
  ],
  type: [
    { required: true, message: t('etl.datasource.form.validation.typeRequired'), trigger: 'change' },
  ],
  host: [
    { required: true, message: t('etl.datasource.form.validation.hostRequired'), trigger: 'blur' },
  ],
  port: [
    { required: true, message: t('etl.datasource.form.validation.portRequired'), trigger: 'blur' },
  ],
  database: [
    { required: true, message: t('etl.datasource.form.validation.databaseRequired'), trigger: 'blur' },
  ],
  username: [
    { required: true, message: t('etl.datasource.form.validation.usernameRequired'), trigger: 'blur' },
  ],
  password: [
    { required: !isEdit.value, message: t('etl.datasource.form.validation.passwordRequired'), trigger: 'blur' },
  ],
}))

// Set default port when type changes
watch(() => formData.type, (newType) => {
  formData.port = defaultPorts[newType] || 3306
})

// Load datasource detail for edit
const loadDatasourceDetail = async () => {
  if (!props.datasourceId) return
  try {
    const data = await getDatasourceDetail(props.datasourceId)
    formData.name = data.name
    formData.type = data.type
    formData.host = data.host
    formData.port = data.port
    formData.database = data.database
    formData.username = data.username
    formData.password = ''
    formData.description = data.description || ''
  } catch (error) {
    ElMessage.error('Failed to load datasource details')
  }
}

// Watch dialog open
watch(visible, (val) => {
  if (val) {
    if (props.datasourceId) {
      loadDatasourceDetail()
    } else {
      resetForm()
    }
  }
})

const resetForm = () => {
  formData.name = ''
  formData.type = 'mysql'
  formData.host = ''
  formData.port = 3306
  formData.database = ''
  formData.username = ''
  formData.password = ''
  formData.description = ''
}

const handleTestConnection = async () => {
  // First save the datasource if it's new
  if (!isEdit.value) {
    ElMessage.warning('请先保存数据源后再测试连接')
    return
  }

  testLoading.value = true
  try {
    const result = await testDatasourceConnection({ id: props.datasourceId })
    if (result.success) {
      ElMessage.success(t('etl.datasource.list.testSuccess'))
    } else {
      ElMessage.error(result.message || t('etl.datasource.list.testFailed'))
    }
  } catch (error) {
    ElMessage.error(t('etl.datasource.list.testFailed'))
  } finally {
    testLoading.value = false
  }
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    if (isEdit.value) {
      await updateDatasource({
        id: props.datasourceId!,
        name: formData.name,
        host: formData.host,
        port: formData.port,
        database: formData.database,
        username: formData.username,
        password: formData.password || undefined,
        description: formData.description,
      })
      ElMessage.success(t('etl.datasource.form.updateSuccess'))
    } else {
      await createDatasource({
        name: formData.name,
        type: formData.type,
        host: formData.host,
        port: formData.port,
        database: formData.database,
        username: formData.username,
        password: formData.password,
        description: formData.description,
      })
      ElMessage.success(t('etl.datasource.form.createSuccess'))
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
</style>
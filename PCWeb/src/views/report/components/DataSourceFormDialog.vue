<!-- src/views/report/components/DataSourceFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="550px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ isEdit ? t('datasource.form.editTitle') : t('datasource.form.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="handleTestConnection" :loading="testing">
            {{ t('datasource.form.testConnection') }}
          </el-button>
          <el-button @click="handleClose">
            {{ t('datasource.form.cancel') }}
          </el-button>
          <el-button type="primary" @click="handleSave" :loading="saving">
            {{ t('datasource.form.save') }}
          </el-button>
        </div>
      </div>
    </template>
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="100px"
    >
      <el-form-item :label="t('datasource.form.name')" prop="name">
        <el-input
          v-model="formData.name"
          :placeholder="t('datasource.form.namePlaceholder')"
        />
      </el-form-item>
      <el-form-item :label="t('datasource.form.dbType')" prop="type">
        <el-select
          v-model="formData.type"
          :placeholder="t('datasource.form.dbTypePlaceholder')"
          style="width: 100%"
          @change="handleDbTypeChange"
        >
          <el-option
            v-for="db in dbTypes"
            :key="db.value"
            :label="db.label"
            :value="db.value"
          />
        </el-select>
      </el-form-item>
      <el-form-item :label="t('datasource.form.host')" prop="host">
        <el-input
          v-model="formData.host"
          :placeholder="t('datasource.form.hostPlaceholder')"
        />
      </el-form-item>
      <el-form-item :label="t('datasource.form.port')" prop="port">
        <el-input-number
          v-model="formData.port"
          :placeholder="t('datasource.form.portPlaceholder')"
          :min="1"
          :max="65535"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item :label="t('datasource.form.database')" prop="database">
        <el-input
          v-model="formData.database"
          :placeholder="t('datasource.form.databasePlaceholder')"
        />
      </el-form-item>
      <el-form-item :label="t('datasource.form.username')" prop="username">
        <el-input
          v-model="formData.username"
          :placeholder="t('datasource.form.usernamePlaceholder')"
        />
      </el-form-item>
      <el-form-item :label="t('datasource.form.password')" prop="password">
        <el-input
          v-model="formData.password"
          type="password"
          show-password
          :placeholder="t('datasource.form.passwordPlaceholder')"
        />
      </el-form-item>
    </el-form>

    </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, computed } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import {
  getDataSourceDetail,
  createDataSource,
  updateDataSource,
  getDbTypes,
  testConnection,
} from '@/api/report/datasourceApi'
import { useLocale } from '@/composables/useLocale'
import type { DataSource, CreateDataSourceParams, DbTypeInfo } from '@/types'

const props = defineProps<{
  modelValue: boolean
  dataSourceId?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value),
})

const isEdit = computed(() => !!props.dataSourceId)

const formRef = ref<FormInstance | null>(null)
const dbTypes = ref<DbTypeInfo[]>([])
const saving = ref(false)
const testing = ref(false)

const formData = reactive<CreateDataSourceParams>({
  name: '',
  type: '',
  host: '',
  port: 3306,
  database: '',
  username: '',
  password: '',
})

const formRules: FormRules = {
  name: [{ required: true, message: t('datasource.form.validation.nameRequired'), trigger: 'blur' }],
  type: [{ required: true, message: t('datasource.form.validation.dbTypeRequired'), trigger: 'change' }],
  host: [{ required: true, message: t('datasource.form.validation.hostRequired'), trigger: 'blur' }],
  port: [{ required: true, message: t('datasource.form.validation.portRequired'), trigger: 'blur' }],
  database: [{ required: true, message: t('datasource.form.validation.databaseRequired'), trigger: 'blur' }],
  username: [{ required: true, message: t('datasource.form.validation.usernameRequired'), trigger: 'blur' }],
  password: [{ required: true, message: t('datasource.form.validation.passwordRequired'), trigger: 'blur' }],
}

// 监听弹窗打开
watch(visible, async (val) => {
  if (val) {
    await loadDbTypes()
    if (props.dataSourceId) {
      await loadDataSource(props.dataSourceId)
    } else {
      resetForm()
    }
  }
})

const loadDbTypes = async () => {
  try {
    const data = await getDbTypes()
    dbTypes.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

const loadDataSource = async (id: string) => {
  try {
    const data = await getDataSourceDetail(id)
    formData.name = data.name
    formData.type = data.type
    formData.host = data.host
    formData.port = data.port
    formData.database = data.database
    formData.username = data.username
    formData.password = '' // 编辑时密码置空，需要重新输入
  } catch (error) {
    // Error handled by interceptor
  }
}

const handleDbTypeChange = (value: string) => {
  const dbType = dbTypes.value.find((db) => db.value === value)
  if (dbType && dbType.defaultPort > 0) {
    formData.port = dbType.defaultPort
  }
}

const handleTestConnection = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch {
    return
  }

  testing.value = true
  try {
    const result = await testConnection(formData)
    if (result.success) {
      ElMessage.success(t('datasource.list.testSuccess'))
    } else {
      ElMessage.error(result.message || t('datasource.list.testFailed'))
    }
  } catch (error) {
    ElMessage.error(t('datasource.list.testFailed'))
  } finally {
    testing.value = false
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
    if (isEdit.value && props.dataSourceId) {
      await updateDataSource({ id: props.dataSourceId, ...formData })
    } else {
      await createDataSource(formData)
    }
    ElMessage.success(isEdit.value ? t('datasource.form.updateSuccess') : t('datasource.form.createSuccess'))
    emit('success')
    handleClose()
  } catch (error) {
    // Error handled by interceptor
  } finally {
    saving.value = false
  }
}

const resetForm = () => {
  formData.name = ''
  formData.type = ''
  formData.host = ''
  formData.port = 3306
  formData.database = ''
  formData.username = ''
  formData.password = ''
}

const handleClose = () => {
  formRef.value?.resetFields()
  visible.value = false
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
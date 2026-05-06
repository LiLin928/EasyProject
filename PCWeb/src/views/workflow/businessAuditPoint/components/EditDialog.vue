<!-- src/views/workflow/businessAuditPoint/components/EditDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    :title="isEdit ? t('antWorkflow.businessAuditPoint.editTitle') : t('antWorkflow.businessAuditPoint.createTitle')"
    width="700px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="140px"
      class="form-container"
    >
      <!-- 基础信息 -->
      <div class="section-title">{{ t('antWorkflow.businessAuditPoint.basicInfo') }}</div>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.code')" prop="code">
        <el-input
          v-model="formData.code"
          :placeholder="t('antWorkflow.businessAuditPoint.codePlaceholder')"
          maxlength="50"
          show-word-limit
        />
      </el-form-item>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.name')" prop="name">
        <el-input
          v-model="formData.name"
          :placeholder="t('antWorkflow.businessAuditPoint.namePlaceholder')"
          maxlength="100"
          show-word-limit
        />
      </el-form-item>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.category')" prop="category">
        <el-input
          v-model="formData.category"
          :placeholder="t('antWorkflow.businessAuditPoint.categoryPlaceholder')"
          maxlength="50"
          show-word-limit
        />
      </el-form-item>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.workflowName')" prop="workflowId">
        <el-select
          v-model="formData.workflowId"
          :placeholder="t('antWorkflow.businessAuditPoint.workflowPlaceholder')"
          clearable
          filterable
          style="width: 100%"
        >
          <el-option
            v-for="workflow in workflowList"
            :key="workflow.id"
            :label="workflow.name"
            :value="workflow.id"
          />
        </el-select>
      </el-form-item>

      <!-- 数据表配置 -->
      <div class="section-title">{{ t('antWorkflow.businessAuditPoint.tableConfig') }}</div>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.tableName')" prop="tableName">
        <el-input
          v-model="formData.tableName"
          :placeholder="t('antWorkflow.businessAuditPoint.tableNamePlaceholder')"
          maxlength="100"
          show-word-limit
        />
      </el-form-item>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.primaryKeyField')" prop="primaryKeyField">
        <el-input
          v-model="formData.primaryKeyField"
          :placeholder="t('antWorkflow.businessAuditPoint.primaryKeyFieldPlaceholder')"
          maxlength="50"
          show-word-limit
        />
        <div class="form-tip">{{ t('antWorkflow.businessAuditPoint.primaryKeyFieldTip') }}</div>
      </el-form-item>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.statusField')" prop="statusField">
        <el-input
          v-model="formData.statusField"
          :placeholder="t('antWorkflow.businessAuditPoint.statusFieldPlaceholder')"
          maxlength="50"
          show-word-limit
        />
      </el-form-item>

      <!-- 状态值配置 -->
      <div class="section-title">{{ t('antWorkflow.businessAuditPoint.statusConfig') }}</div>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item :label="t('antWorkflow.businessAuditPoint.auditStatusValue')" prop="auditStatusValue">
            <el-input-number
              v-model="formData.auditStatusValue"
              :min="0"
              :max="999"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item :label="t('antWorkflow.businessAuditPoint.passStatusValue')" prop="passStatusValue">
            <el-input-number
              v-model="formData.passStatusValue"
              :min="0"
              :max="999"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item :label="t('antWorkflow.businessAuditPoint.rejectStatusValue')" prop="rejectStatusValue">
            <el-input-number
              v-model="formData.rejectStatusValue"
              :min="0"
              :max="999"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item :label="t('antWorkflow.businessAuditPoint.withdrawStatusValue')" prop="withdrawStatusValue">
            <el-input-number
              v-model="formData.withdrawStatusValue"
              :min="0"
              :max="999"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 扩展配置 -->
      <div class="section-title">{{ t('antWorkflow.businessAuditPoint.extendConfig') }}</div>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.titleTemplate')">
        <el-input
          v-model="formData.titleTemplate"
          :placeholder="t('antWorkflow.businessAuditPoint.titleTemplatePlaceholder')"
          maxlength="200"
          show-word-limit
        />
        <div class="form-tip">{{ t('antWorkflow.businessAuditPoint.titleTemplateTip') }}</div>
      </el-form-item>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.codeField')">
        <el-input
          v-model="formData.codeField"
          :placeholder="t('antWorkflow.businessAuditPoint.codeFieldPlaceholder')"
          maxlength="50"
          show-word-limit
        />
      </el-form-item>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.auditPageUrl')">
        <el-input
          v-model="formData.auditPageUrl"
          :placeholder="t('antWorkflow.businessAuditPoint.auditPageUrlPlaceholder')"
          maxlength="200"
          show-word-limit
        />
      </el-form-item>

      <!-- 回调配置 -->
      <div class="section-title">{{ t('antWorkflow.businessAuditPoint.callbackConfig') }}</div>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.passCallbackApi')">
        <el-input
          v-model="formData.passCallbackApi"
          :placeholder="t('antWorkflow.businessAuditPoint.callbackApiPlaceholder')"
          maxlength="200"
          show-word-limit
        />
      </el-form-item>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.rejectCallbackApi')">
        <el-input
          v-model="formData.rejectCallbackApi"
          :placeholder="t('antWorkflow.businessAuditPoint.callbackApiPlaceholder')"
          maxlength="200"
          show-word-limit
        />
      </el-form-item>

      <!-- 其他配置 -->
      <div class="section-title">{{ t('antWorkflow.businessAuditPoint.otherConfig') }}</div>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item :label="t('antWorkflow.businessAuditPoint.status')">
            <el-radio-group v-model="formData.status">
              <el-radio :value="1">{{ t('antWorkflow.businessAuditPoint.enabled') }}</el-radio>
              <el-radio :value="0">{{ t('antWorkflow.businessAuditPoint.disabled') }}</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item :label="t('antWorkflow.businessAuditPoint.sort')">
            <el-input-number
              v-model="formData.sort"
              :min="0"
              :max="999"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item :label="t('antWorkflow.businessAuditPoint.remark')">
        <el-input
          v-model="formData.remark"
          type="textarea"
          :rows="3"
          :placeholder="t('antWorkflow.businessAuditPoint.remarkPlaceholder')"
          maxlength="500"
          show-word-limit
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose">
        {{ t('antWorkflow.businessAuditPoint.cancel') }}
      </el-button>
      <el-button type="primary" :loading="saving" @click="handleSubmit">
        {{ t('antWorkflow.businessAuditPoint.save') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
// 1. Vue 核心
import { ref, reactive, computed, watch } from 'vue'

// 2. 第三方库
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'

// 3. API
import {
  getBusinessAuditPointDetail,
  createBusinessAuditPoint,
  updateBusinessAuditPoint,
} from '@/api/ant_workflow/businessAuditPointApi'
import { getWorkflowList } from '@/api/ant_workflow/workflowApi'

// 4. Composables
import { useLocale } from '@/composables/useLocale'

// 5. 类型
import type {
  BusinessAuditPoint,
  CreateBusinessAuditPointParams,
  UpdateBusinessAuditPointParams,
} from '@/types/businessAuditPoint'
import type { AntWorkflowDefinition } from '@/types/antWorkflow'

// Props
const props = defineProps<{
  id?: string
}>()

// Emits
const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

// 对话框可见性
const visible = defineModel<boolean>({ default: false })

// 是否编辑模式
const isEdit = computed(() => !!props.id)

// 表单引用
const formRef = ref<FormInstance | null>(null)

// 保存状态
const saving = ref(false)

// 工作流列表
const workflowList = ref<AntWorkflowDefinition[]>([])

// 表单数据
const formData = reactive<CreateBusinessAuditPointParams>({
  code: '',
  name: '',
  category: '',
  workflowId: '',
  tableName: '',
  primaryKeyField: 'Id',
  statusField: 'AuditStatus',
  auditStatusValue: 1,
  passStatusValue: 3,
  rejectStatusValue: 2,
  withdrawStatusValue: 0,
  auditPageUrl: '',
  titleTemplate: '',
  codeField: '',
  passCallbackApi: '',
  rejectCallbackApi: '',
  status: 1,
  sort: 0,
  remark: '',
})

// 表单校验规则
const formRules = computed<FormRules>(() => ({
  code: [
    { required: true, message: t('antWorkflow.businessAuditPoint.validation.codeRequired'), trigger: 'blur' },
    { max: 50, message: t('antWorkflow.businessAuditPoint.validation.codeLength'), trigger: 'blur' },
  ],
  name: [
    { required: true, message: t('antWorkflow.businessAuditPoint.validation.nameRequired'), trigger: 'blur' },
    { max: 100, message: t('antWorkflow.businessAuditPoint.validation.nameLength'), trigger: 'blur' },
  ],
  workflowId: [
    { required: true, message: t('antWorkflow.businessAuditPoint.validation.workflowRequired'), trigger: 'change' },
  ],
  tableName: [
    { required: true, message: t('antWorkflow.businessAuditPoint.validation.tableNameRequired'), trigger: 'blur' },
  ],
  statusField: [
    { required: true, message: t('antWorkflow.businessAuditPoint.validation.statusFieldRequired'), trigger: 'blur' },
  ],
  auditStatusValue: [
    { required: true, message: t('antWorkflow.businessAuditPoint.validation.auditStatusValueRequired'), trigger: 'blur' },
  ],
  passStatusValue: [
    { required: true, message: t('antWorkflow.businessAuditPoint.validation.passStatusValueRequired'), trigger: 'blur' },
  ],
  rejectStatusValue: [
    { required: true, message: t('antWorkflow.businessAuditPoint.validation.rejectStatusValueRequired'), trigger: 'blur' },
  ],
  withdrawStatusValue: [
    { required: true, message: t('antWorkflow.businessAuditPoint.validation.withdrawStatusValueRequired'), trigger: 'blur' },
  ],
}))

// 监听对话框打开
watch(visible, async (val) => {
  if (val) {
    await loadWorkflowList()
    if (props.id) {
      await loadDetail(props.id)
    } else {
      resetForm()
    }
  }
})

/**
 * 加载工作流列表
 */
const loadWorkflowList = async () => {
  try {
    const data = await getWorkflowList({ pageIndex: 1, pageSize: 999, status: 2 })
    workflowList.value = data.list || []
  } catch (error) {
    // 错误已在拦截器处理
  }
}

/**
 * 加载详情
 */
const loadDetail = async (id: string) => {
  try {
    const data = await getBusinessAuditPointDetail(id)
    Object.assign(formData, {
      code: data.code,
      name: data.name,
      category: data.category || '',
      workflowId: data.workflowId,
      tableName: data.tableName,
      primaryKeyField: data.primaryKeyField || 'Id',
      statusField: data.statusField || 'AuditStatus',
      auditStatusValue: data.auditStatusValue,
      passStatusValue: data.passStatusValue,
      rejectStatusValue: data.rejectStatusValue,
      withdrawStatusValue: data.withdrawStatusValue,
      auditPageUrl: data.auditPageUrl || '',
      titleTemplate: data.titleTemplate || '',
      codeField: data.codeField || '',
      passCallbackApi: data.passCallbackApi || '',
      rejectCallbackApi: data.rejectCallbackApi || '',
      status: data.status,
      sort: data.sort,
      remark: data.remark || '',
    })
  } catch (error) {
    ElMessage.error(t('antWorkflow.businessAuditPoint.errors.loadDetailFailed'))
    visible.value = false
  }
}

/**
 * 重置表单
 */
const resetForm = () => {
  Object.assign(formData, {
    code: '',
    name: '',
    category: '',
    workflowId: '',
    tableName: '',
    primaryKeyField: 'Id',
    statusField: 'AuditStatus',
    auditStatusValue: 1,
    passStatusValue: 3,
    rejectStatusValue: 2,
    withdrawStatusValue: 0,
    auditPageUrl: '',
    titleTemplate: '',
    codeField: '',
    passCallbackApi: '',
    rejectCallbackApi: '',
    status: 1,
    sort: 0,
    remark: '',
  })
  formRef.value?.clearValidate()
}

/**
 * 关闭对话框
 */
const handleClose = () => {
  visible.value = false
  resetForm()
}

/**
 * 提交表单
 */
const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch {
    return
  }

  saving.value = true
  try {
    if (isEdit.value) {
      const params: UpdateBusinessAuditPointParams = {
        id: props.id!,
        ...formData,
      }
      await updateBusinessAuditPoint(params)
      ElMessage.success(t('antWorkflow.businessAuditPoint.messages.updateSuccess'))
    } else {
      await createBusinessAuditPoint(formData)
      ElMessage.success(t('antWorkflow.businessAuditPoint.messages.createSuccess'))
    }
    emit('success')
    handleClose()
  } catch (error) {
    ElMessage.error(t('antWorkflow.businessAuditPoint.errors.saveFailed'))
  } finally {
    saving.value = false
  }
}
</script>

<style scoped lang="scss">
.form-container {
  max-height: 500px;
  overflow-y: auto;

  .section-title {
    font-size: 14px;
    font-weight: 600;
    color: #303133;
    margin: 16px 0 12px;
    padding-bottom: 8px;
    border-bottom: 1px solid #ebeef5;

    &:first-child {
      margin-top: 0;
    }
  }

  .form-tip {
    font-size: 12px;
    color: #909399;
    margin-top: 4px;
  }
}
</style>
<!-- 审批弹框组件 -->
<template>
  <el-dialog
    v-model="visible"
    :title="dialogTitle"
    width="800px"
    :close-on-click-modal="false"
    :close-on-press-escape="false"
    @close="handleClose"
  >
    <div v-loading="loading" class="approve-dialog-content">
      <!-- 审批进度 -->
      <el-card shadow="never" class="progress-card">
        <template #header>
          <span>{{ t('antWorkflow.approveProgress') }}</span>
        </template>
        <ApproveProgress :nodes="instanceDetail?.nodeStatusList || []" />
      </el-card>

      <!-- 审批操作 -->
      <el-card shadow="never" class="action-card">
        <template #header>
          <span>{{ t('antWorkflow.approve') }}</span>
        </template>
        <el-form
          ref="formRef"
          :model="approveForm"
          :rules="formRules"
          label-width="100px"
        >
          <el-form-item :label="t('antWorkflow.approveComment')" prop="comment">
            <el-input
              v-model="approveForm.comment"
              type="textarea"
              :rows="3"
              :placeholder="t('antWorkflow.approveCommentPlaceholder')"
              maxlength="500"
              show-word-limit
            />
          </el-form-item>

          <el-form-item v-if="showTransferSelect" :label="t('antWorkflow.transferTo')" prop="toUserId">
            <el-select
              v-model="approveForm.toUserId"
              :placeholder="t('antWorkflow.transferToPlaceholder')"
              clearable
              filterable
              style="width: 100%"
            >
              <el-option
                v-for="user in transferUsers"
                :key="user.id"
                :label="user.name"
                :value="user.id"
              />
            </el-select>
          </el-form-item>
        </el-form>
      </el-card>
    </div>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">{{ t('antWorkflow.cancel') }}</el-button>
        <el-button type="danger" @click="handleReject">{{ t('antWorkflow.approveReject') }}</el-button>
        <el-button type="warning" @click="handleTransfer">{{ t('antWorkflow.transfer') }}</el-button>
        <el-button type="primary" @click="handleApprove">{{ t('antWorkflow.approvePass') }}</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import ApproveProgress from './ApproveProgress.vue'
import { getInstanceDetail } from '@/api/ant_workflow/runtimeApi'
import { approveTask, rejectTask, transferTask } from '@/api/ant_workflow/taskApi'
import { getUserList } from '@/api/user'
import type {
  AntWorkflowTaskDto,
  AntWorkflowInstanceDetailDto,
} from '@/types/antWorkflow/runtime'
import type { UserInfo } from '@/types'

const { t } = useLocale()

const props = defineProps<{
  task: AntWorkflowTaskDto | null
  modelValue: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

// 弹窗可见性
const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

// 弹窗标题
const dialogTitle = computed(() => {
  return `${t('antWorkflow.approve')} - ${props.task?.instanceTitle || ''}`
})

// 加载状态
const loading = ref(false)

// 流程实例详情
const instanceDetail = ref<AntWorkflowInstanceDetailDto | null>(null)

// 表单引用
const formRef = ref<FormInstance>()

// 审批表单
const approveForm = ref({
  comment: '',
  toUserId: '',
})

// 表单验证规则
const formRules: FormRules = {
  comment: [
    { required: true, message: t('antWorkflow.approveCommentPlaceholder'), trigger: 'blur' },
  ],
}

// 是否显示转交选择
const showTransferSelect = ref(false)

// 可转交用户列表（从后端 API 获取）
const transferUsers = ref<{ id: string; name: string }[]>([])

/**
 * 加载可转交用户列表
 */
const loadTransferUsers = async () => {
  try {
    const res = await getUserList({ pageIndex: 1, pageSize: 100, status: 1 })
    transferUsers.value = res.list.map(user => ({
      id: user.id,
      name: user.realName || user.userName,
    }))
  } catch (error) {
    console.error('加载用户列表失败', error)
    transferUsers.value = []
  }
}

/**
 * 加载流程实例详情
 */
const loadInstanceDetail = async () => {
  if (!props.task?.instanceId) return

  loading.value = true
  try {
    const res = await getInstanceDetail(props.task.instanceId)
    instanceDetail.value = res
  } catch (error) {
    ElMessage.error(t('antWorkflow.errors.loadDetailFailed'))
    console.error(error)
  } finally {
    loading.value = false
  }
}

/**
 * 监听弹窗打开，加载详情和用户列表
 */
watch(visible, (val) => {
  if (val && props.task) {
    loadInstanceDetail()
    loadTransferUsers()
    approveForm.value = { comment: '', toUserId: '' }
    showTransferSelect.value = false
  }
})

/**
 * 处理审批通过
 */
const handleApprove = async () => {
  try {
    await formRef.value?.validate()

    await ElMessageBox.confirm(
      t('antWorkflow.messages.approveConfirm'),
      t('antWorkflow.warning'),
      { type: 'success' }
    )

    loading.value = true
    await approveTask(props.task!.id, { comment: approveForm.value.comment })

    ElMessage.success(t('antWorkflow.messages.approveSuccess'))
    emit('success')
    handleClose()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(t('antWorkflow.errors.approveFailed'))
      console.error(error)
    }
  } finally {
    loading.value = false
  }
}

/**
 * 处理审批驳回
 */
const handleReject = async () => {
  try {
    await formRef.value?.validate()

    await ElMessageBox.confirm(
      t('antWorkflow.messages.rejectConfirm'),
      t('antWorkflow.warning'),
      { type: 'warning' }
    )

    loading.value = true
    await rejectTask(props.task!.id, { comment: approveForm.value.comment })

    ElMessage.success(t('antWorkflow.messages.rejectSuccess'))
    emit('success')
    handleClose()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(t('antWorkflow.errors.approveFailed'))
      console.error(error)
    }
  } finally {
    loading.value = false
  }
}

/**
 * 处理转交
 */
const handleTransfer = async () => {
  showTransferSelect.value = true

  try {
    await formRef.value?.validate()

    if (!approveForm.value.toUserId) {
      ElMessage.warning(t('antWorkflow.transferToPlaceholder'))
      return
    }

    const targetUser = transferUsers.value.find(u => u.id === approveForm.value.toUserId)
    await ElMessageBox.confirm(
      `${t('antWorkflow.messages.transferConfirm')} ${targetUser?.name}?`,
      t('antWorkflow.warning'),
      { type: 'info' }
    )

    loading.value = true
    await transferTask(props.task!.id, {
      toUserId: approveForm.value.toUserId,
      comment: approveForm.value.comment,
    })

    ElMessage.success(t('antWorkflow.messages.transferSuccess'))
    emit('success')
    handleClose()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(t('antWorkflow.errors.approveFailed'))
      console.error(error)
    }
  } finally {
    loading.value = false
  }
}

/**
 * 关闭弹窗
 */
const handleClose = () => {
  visible.value = false
  instanceDetail.value = null
  approveForm.value = { comment: '', toUserId: '' }
  showTransferSelect.value = false
}
</script>

<style scoped lang="scss">
.approve-dialog-content {
  .progress-card,
  .action-card {
    margin-bottom: 16px;

    &:last-child {
      margin-bottom: 0;
    }
  }
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}
</style>
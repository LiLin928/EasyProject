<!-- 流程实例详情弹框 -->
<template>
  <el-dialog
    v-model="visible"
    :title="t('antWorkflow.businessDetail')"
    width="900px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <div v-loading="loading" class="instance-detail-content">
      <!-- 基本信息 -->
      <el-card shadow="never" class="info-card">
        <template #header>
          <span>{{ t('antWorkflow.nodeConfig.nodeName') }}</span>
        </template>
        <el-descriptions :column="2" border>
          <el-descriptions-item :label="t('antWorkflow.taskTitle')">
            {{ instanceDetail?.instance?.title }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('antWorkflow.initiator')">
            {{ instanceDetail?.instance?.initiatorName || '-' }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('antWorkflow.entryTime')">
            {{ instanceDetail?.instance?.startTime }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('antWorkflow.status')">
            <el-tag :type="getStatusType(instanceDetail?.instance?.status)">
              {{ getStatusLabel(instanceDetail?.instance?.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item :label="t('antWorkflow.approveTime')">
            {{ instanceDetail?.instance?.finishTime || '-' }}
          </el-descriptions-item>
        </el-descriptions>
      </el-card>

      <!-- 审批进度 -->
      <el-card shadow="never" class="progress-card">
        <template #header>
          <span>{{ t('antWorkflow.approveProgress') }}</span>
        </template>
        <ApproveProgress :nodes="instanceDetail?.nodeStatusList || []" />
      </el-card>

      <!-- 审批记录 -->
      <el-card shadow="never" class="records-card">
        <template #header>
          <span>{{ t('antWorkflow.approveRecord') }}</span>
        </template>
        <el-table :data="instanceDetail?.approveRecords || []" border>
          <el-table-column prop="nodeName" :label="t('antWorkflow.nodeName')" width="120" />
          <el-table-column prop="handlerName" :label="t('antWorkflow.initiator')" width="100" />
          <el-table-column :label="t('antWorkflow.approveResult')" width="100">
            <template #default="{ row }">
              <el-tag :type="getResultType(row.approveStatus)" size="small">
                {{ getResultLabel(row.approveStatus) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="approveDesc" :label="t('antWorkflow.approveComment')" />
          <el-table-column prop="approveTime" :label="t('antWorkflow.handleTime')" width="160" />
          <el-table-column :label="t('antWorkflow.transferTo')" width="100">
            <template #default="{ row }">
              {{ row.transferToName || '-' }}
            </template>
          </el-table-column>
        </el-table>
      </el-card>
    </div>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import ApproveProgress from './ApproveProgress.vue'
import { getInstanceDetail } from '@/api/ant_workflow/runtimeApi'

const { t } = useLocale()

const props = defineProps<{
  instanceId: string | null
  modelValue: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>()

// 弹窗可见性
const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

// 加载状态
const loading = ref(false)

// 流程实例详情（后端返回的结构）
const instanceDetail = ref<any>(null)

/**
 * 加载流程实例详情
 */
const loadInstanceDetail = async () => {
  if (!props.instanceId) return

  loading.value = true
  try {
    const res = await getInstanceDetail(props.instanceId)
    instanceDetail.value = res
  } catch (error) {
    ElMessage.error(t('antWorkflow.errors.loadDetailFailed'))
    console.error(error)
  } finally {
    loading.value = false
  }
}

/**
 * 监听弹窗打开，加载详情
 */
watch(visible, (val) => {
  if (val && props.instanceId) {
    loadInstanceDetail()
  }
})

/**
 * 获取状态标签类型
 */
const getStatusType = (status?: number) => {
  switch (status) {
    case 2: // PASSED
      return 'success'
    case 1: // APPROVING
      return 'warning'
    case 3: // REJECTED
      return 'danger'
    case 4: // WITHDRAWN
      return 'info'
    default:
      return 'info'
  }
}

/**
 * 获取状态标签文字
 */
const getStatusLabel = (status?: number) => {
  switch (status) {
    case 0: // WAIT_SUBMIT
      return t('antWorkflow.instanceStatusLabels.waitSubmit')
    case 1: // APPROVING
      return t('antWorkflow.instanceStatusLabels.approving')
    case 2: // PASSED
      return t('antWorkflow.instanceStatusLabels.passed')
    case 3: // REJECTED
      return t('antWorkflow.instanceStatusLabels.rejected')
    case 4: // WITHDRAWN
      return t('antWorkflow.instanceStatusLabels.withdrawn')
    case 5: // TERMINATED
      return t('antWorkflow.instanceStatusLabels.terminated')
    default:
      return ''
  }
}

/**
 * 获取审批结果标签类型
 */
const getResultType = (result?: number) => {
  switch (result) {
    case 1: // PASS
      return 'success'
    case 2: // REJECT
      return 'danger'
    case 3: // TRANSFER
      return 'warning'
    case 4: // WITHDRAW
      return 'info'
    default:
      return 'info'
  }
}

/**
 * 获取审批结果标签文字
 */
const getResultLabel = (result?: number) => {
  switch (result) {
    case 1: // PASS
      return t('antWorkflow.handleResultLabels.pass')
    case 2: // REJECT
      return t('antWorkflow.handleResultLabels.reject')
    case 3: // TRANSFER
      return t('antWorkflow.handleResultLabels.transfer')
    case 4: // WITHDRAW
      return t('antWorkflow.handleResultLabels.withdraw')
    default:
      return ''
  }
}

/**
 * 关闭弹窗
 */
const handleClose = () => {
  visible.value = false
  instanceDetail.value = null
}
</script>

<style scoped lang="scss">
.instance-detail-content {
  .info-card,
  .progress-card,
  .records-card {
    margin-bottom: 16px;

    &:last-child {
      margin-bottom: 0;
    }
  }
}
</style>
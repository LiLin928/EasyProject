<!-- 审批进度组件 -->
<template>
  <div class="approve-progress">
    <el-steps :active="getActiveStep()" direction="horizontal" finish-status="success" :space="200">
      <el-step
        v-for="node in nodes"
        :key="node.nodeId"
        :title="node.nodeName"
        :description="getNodeDescription(node)"
        :status="getNodeStatus(node)"
      >
        <template #icon>
          <el-icon :class="['step-icon', getStepIconClass(node)]">
            <component :is="getNodeIcon(node.nodeType)" />
          </el-icon>
        </template>
      </el-step>
    </el-steps>
  </div>
</template>

<script setup lang="ts">
import {
  User,
  CircleCheck,
  DocumentCopy,
  Setting,
  Bell,
  Link,
  Share,
} from '@element-plus/icons-vue'
import type { AntWorkflowNodeStatusDto } from '@/types/antWorkflow/runtime'
import { AntNodeApproveStatus } from '@/types/antWorkflow/runtime'

const props = defineProps<{
  nodes: AntWorkflowNodeStatusDto[]
}>()

/**
 * 获取处理人姓名列表
 */
const getHandlerNames = (node: AntWorkflowNodeStatusDto): string[] => {
  if (!node.handlers) return []
  return node.handlers
    .filter(h => h.userName)
    .map(h => h.userName!)
}

/**
 * 获取当前激活步骤
 */
const getActiveStep = () => {
  const pendingIndex = props.nodes.findIndex(
    (n) => n.approveStatus === AntNodeApproveStatus.PENDING
  )
  return pendingIndex >= 0 ? pendingIndex : props.nodes.length
}

/**
 * 获取节点描述
 */
const getNodeDescription = (node: AntWorkflowNodeStatusDto) => {
  const handlerNames = getHandlerNames(node)
  if (node.approveStatus === AntNodeApproveStatus.COMPLETED) {
    return handlerNames.length > 0 ? handlerNames.join('、') : ''
  }
  if (node.approveStatus === AntNodeApproveStatus.PROCESSING) {
    return handlerNames.length > 0
      ? `${handlerNames.join('、')} 处理中`
      : '处理中'
  }
  if (node.approveStatus === AntNodeApproveStatus.PENDING) {
    return handlerNames.length > 0
      ? `待 ${handlerNames.join('、')} 处理`
      : '待处理'
  }
  return ''
}

/**
 * 获取节点状态
 */
const getNodeStatus = (node: AntWorkflowNodeStatusDto): 'wait' | 'process' | 'success' | 'error' | 'finish' => {
  switch (node.approveStatus) {
    case AntNodeApproveStatus.COMPLETED:
      return 'success'
    case AntNodeApproveStatus.PROCESSING:
      return 'process'
    case AntNodeApproveStatus.SKIPPED:
      return 'error'
    default:
      return 'wait'
  }
}

/**
 * 获取节点图标（nodeType 为数字）
 */
const getNodeIcon = (nodeType: number) => {
  switch (nodeType) {
    case 0: // Start
      return User
    case 1: // Approver
      return CircleCheck
    case 2: // Copyer
      return DocumentCopy
    case 3: // Service
      return Setting
    case 4: // Notification
      return Bell
    case 5: // Webhook
      return Link
    case 6: // Condition
    case 7: // Parallel
      return Share
    case 8: // Subflow
      return Share
    default:
      return CircleCheck
  }
}

/**
 * 获取步骤图标样式类
 */
const getStepIconClass = (node: AntWorkflowNodeStatusDto) => {
  switch (node.approveStatus) {
    case AntNodeApproveStatus.COMPLETED:
      return 'completed'
    case AntNodeApproveStatus.PROCESSING:
      return 'processing'
    case AntNodeApproveStatus.SKIPPED:
      return 'skipped'
    default:
      return 'pending'
  }
}
</script>

<style scoped lang="scss">
.approve-progress {
  padding: 10px 0;
  overflow-x: auto;
  // 显示滚动条
  scrollbar-width: thin;
  scrollbar-color: var(--el-border-color-darker) transparent;

  &::-webkit-scrollbar {
    height: 6px;
  }

  &::-webkit-scrollbar-thumb {
    background: var(--el-border-color-darker);
    border-radius: 3px;
  }

  &::-webkit-scrollbar-track {
    background: transparent;
  }

  // 横向步骤条样式调整
  :deep(.el-steps) {
    // 根据节点数量自动计算宽度
    display: flex;
    flex-wrap: nowrap;
  }

  :deep(.el-step) {
    flex-shrink: 0;
    min-width: 180px;

    // 确保描述文字不换行溢出时能正确显示
    .el-step__description {
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      max-width: 160px;
    }
  }

  .step-icon {
    font-size: 18px;

    &.completed {
      color: var(--el-color-success);
    }

    &.processing {
      color: var(--el-color-primary);
    }

    &.skipped {
      color: var(--el-color-info);
    }

    &.pending {
      color: var(--el-text-color-secondary);
    }
  }
}
</style>
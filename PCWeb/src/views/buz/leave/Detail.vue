<template>
  <div class="leave-detail">
    <el-descriptions :column="2" border>
      <el-descriptions-item label="请假单号">
        {{ data.orderNo || '-' }}
      </el-descriptions-item>
      <el-descriptions-item label="请假类型">
        {{ data.leaveType || '-' }}
      </el-descriptions-item>
      <el-descriptions-item label="开始日期">
        {{ data.startDate || '-' }}
      </el-descriptions-item>
      <el-descriptions-item label="结束日期">
        {{ data.endDate || '-' }}
      </el-descriptions-item>
      <el-descriptions-item label="请假天数">
        {{ data.days || 0 }} 天
      </el-descriptions-item>
      <el-descriptions-item label="申请人">
        {{ data.applicantName || '-' }}
      </el-descriptions-item>
      <el-descriptions-item label="申请部门">
        {{ data.department || '-' }}
      </el-descriptions-item>
      <el-descriptions-item label="申请时间">
        {{ data.applyTime || '-' }}
      </el-descriptions-item>
      <el-descriptions-item label="请假原因" :span="2">
        {{ data.reason || '-' }}
      </el-descriptions-item>
    </el-descriptions>

    <el-divider content-position="left">审批流程</el-divider>

    <el-steps direction="vertical" :active="1">
      <el-step title="发起申请" :description="data.applyTime" status="success" />
      <el-step title="部门审批" description="待审批" status="process" />
      <el-step title="HR审批" description="待处理" status="wait" />
    </el-steps>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const props = defineProps<{
  businessId: string
  formData?: string
  readonly?: boolean
}>()

// 解析表单数据
const data = ref<any>({
  orderNo: '',
  leaveType: '',
  startDate: '',
  endDate: '',
  days: 0,
  applicantName: '',
  department: '',
  applyTime: '',
  reason: '',
})

/**
 * 加载数据
 */
const loadData = () => {
  // 如果有 formData，直接解析
  if (props.formData) {
    try {
      data.value = JSON.parse(props.formData)
      return
    } catch (e) {
      console.error('解析 formData 失败:', e)
    }
  }

  // 否则根据 businessId 加载（这里使用 Mock 数据）
  data.value = {
    orderNo: 'LV-2024-001',
    leaveType: '事假',
    startDate: '2026-04-15',
    endDate: '2026-04-17',
    days: 3,
    applicantName: '王五',
    department: '研发部',
    applyTime: '2026-04-10 14:00:00',
    reason: '家中有事需要处理',
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped lang="less">
.leave-detail {
  .el-divider {
    margin: 16px 0;
  }

  .el-steps {
    padding: 10px 0;
  }
}
</style>
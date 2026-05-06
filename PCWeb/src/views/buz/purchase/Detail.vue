<template>
  <div class="purchase-detail">
    <el-descriptions :column="2" border>
      <el-descriptions-item label="采购单号">
        {{ data.orderNo || '-' }}
      </el-descriptions-item>
      <el-descriptions-item label="采购类型">
        {{ data.type || '-' }}
      </el-descriptions-item>
      <el-descriptions-item label="申请金额">
        {{ formatAmount(data.amount) }}
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
      <el-descriptions-item label="采购原因" :span="2">
        {{ data.reason || '-' }}
      </el-descriptions-item>
    </el-descriptions>

    <el-divider content-position="left">采购明细</el-divider>

    <el-table :data="data.items || []" border>
      <el-table-column prop="name" label="商品名称" min-width="150" />
      <el-table-column prop="spec" label="规格型号" width="100" />
      <el-table-column prop="quantity" label="数量" width="80" />
      <el-table-column prop="unit" label="单位" width="60" />
      <el-table-column label="单价" width="100">
        <template #default="{ row }">
          {{ formatAmount(row.price) }}
        </template>
      </el-table-column>
      <el-table-column label="金额" width="100">
        <template #default="{ row }">
          {{ formatAmount(row.price * row.quantity) }}
        </template>
      </el-table-column>
    </el-table>

    <div class="total-row">
      <span>合计金额：</span>
      <span class="amount">{{ formatAmount(totalAmount) }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

const props = defineProps<{
  businessId: string
  formData?: string
  readonly?: boolean
}>()

// 解析表单数据
const data = ref<any>({
  orderNo: '',
  type: '',
  amount: 0,
  applicantName: '',
  department: '',
  applyTime: '',
  reason: '',
  items: [],
})

// 计算合计金额
const totalAmount = computed(() => {
  return (data.value.items || []).reduce(
    (sum: number, item: any) => sum + item.price * item.quantity,
    0
  )
})

/**
 * 格式化金额
 */
const formatAmount = (amount: number): string => {
  if (!amount) return '¥0.00'
  return `¥${amount.toFixed(2)}`
}

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
    orderNo: 'PO-2024-001',
    type: '办公用品采购',
    amount: 50000,
    applicantName: '张三',
    department: '研发部',
    applyTime: '2026-04-10 09:00:00',
    reason: '项目开发需要采购办公用品',
    items: [
      { name: '笔记本电脑', spec: 'ThinkPad X1', quantity: 5, unit: '台', price: 8000 },
      { name: '显示器', spec: '27寸 4K', quantity: 5, unit: '台', price: 2000 },
    ],
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped lang="less">
.purchase-detail {
  .el-divider {
    margin: 16px 0;
  }

  .total-row {
    margin-top: 16px;
    text-align: right;
    font-size: 14px;

    .amount {
      color: var(--el-color-danger);
      font-weight: bold;
      font-size: 16px;
    }
  }
}
</style>
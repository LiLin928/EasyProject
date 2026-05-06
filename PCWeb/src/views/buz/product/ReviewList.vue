<!-- src/views/buz/product/ReviewList.vue -->
<template>
  <div class="review-list-container">
    <!-- Breadcrumb -->
    <el-breadcrumb separator="/" class="breadcrumb">
      <el-breadcrumb-item :to="{ path: '/product/list' }">
        商品列表
      </el-breadcrumb-item>
      <el-breadcrumb-item v-if="queryParams.productId">
        {{ queryParams.productName || '商品评价' }}
      </el-breadcrumb-item>
      <el-breadcrumb-item>商品评价</el-breadcrumb-item>
    </el-breadcrumb>

    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('product.review.title') }}</span>
          <div class="header-right">
            <el-button v-if="queryParams.productId" link type="primary" @click="clearProductFilter">
              查看全部评价
            </el-button>
            <el-button @click="handleBack">
              返回
            </el-button>
          </div>
        </div>
      </template>

      <!-- 商品筛选提示 -->
      <el-alert
        v-if="queryParams.productId"
        :title="`当前筛选：${queryParams.productName || '指定商品'} 的评价`"
        type="info"
        :closable="false"
        show-icon
        style="margin-bottom: 16px"
      />

      <!-- 统计卡片 -->
      <el-row :gutter="16" class="stats-row">
        <el-col :span="6">
          <div class="stats-card">
            <div class="stats-value">{{ statistics.total }}</div>
            <div class="stats-label">总评价数</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="stats-card">
            <div class="stats-value rating">{{ statistics.averageRating }}</div>
            <div class="stats-label">平均评分</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="stats-card warning">
            <div class="stats-value">{{ statistics.pendingCount }}</div>
            <div class="stats-label">待审核</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="stats-card success">
            <div class="stats-value">{{ statistics.approvedCount }}</div>
            <div class="stats-label">已通过</div>
          </div>
        </el-col>
      </el-row>

      <!-- 搜索表单 -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item label="商品名称">
          <el-input
            v-model="queryParams.productName"
            placeholder="请输入商品名称"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item label="用户名称">
          <el-input
            v-model="queryParams.userName"
            placeholder="请输入用户名称"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item label="评分">
          <el-select v-model="queryParams.rating" placeholder="全部评分" clearable style="width: 120px">
            <el-option v-for="i in 5" :key="i" :label="`${i}星`" :value="i" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryParams.status" placeholder="全部状态" clearable style="width: 120px">
            <el-option label="待审核" value="pending" />
            <el-option label="已通过" value="approved" />
            <el-option label="已拒绝" value="rejected" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('common.date.to')">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            :range-separator="t('common.date.rangeSeparator')"
            :start-placeholder="t('common.date.startPlaceholder')"
            :end-placeholder="t('common.date.endPlaceholder')"
            value-format="YYYY-MM-DD"
            style="width: 240px"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            {{ t('common.button.search') }}
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            {{ t('common.button.reset') }}
          </el-button>
        </el-form-item>
      </el-form>

      <!-- 批量操作 -->
      <div class="batch-actions" v-if="selectedRows.length > 0">
        <el-button type="success" @click="handleBatchAudit('approved')">
          批量通过
        </el-button>
        <el-button type="danger" @click="handleBatchAudit('rejected')">
          批量拒绝
        </el-button>
        <span class="selected-count">已选择 {{ selectedRows.length }} 条</span>
      </div>

      <!-- 表格 -->
      <el-table
        v-loading="loading"
        :data="tableData"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="50" />
        <el-table-column label="商品" min-width="200">
          <template #default="{ row }">
            <div class="product-info">
              <el-image
                :src="row.productImage || '/static/images/products/default.jpg'"
                fit="cover"
                class="product-image"
              >
                <template #error>
                  <div class="image-placeholder">
                    <el-icon><Picture /></el-icon>
                  </div>
                </template>
              </el-image>
              <div class="product-name">{{ row.productName }}</div>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="用户" width="150">
          <template #default="{ row }">
            <div class="user-info">
              <span v-if="row.isAnonymous">匿名用户</span>
              <span v-else>{{ row.userName }}</span>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="评分" width="150" align="center">
          <template #default="{ row }">
            <el-rate v-model="row.rating" disabled />
          </template>
        </el-table-column>
        <el-table-column label="评价内容" min-width="300">
          <template #default="{ row }">
            <div class="review-content">
              <div class="content-text">{{ row.content }}</div>
              <div v-if="row.images && row.images.length > 0" class="content-images">
                <el-image
                  v-for="(img, index) in row.images"
                  :key="index"
                  :src="img"
                  :preview-src-list="row.images"
                  :initial-index="index"
                  fit="cover"
                  class="review-image"
                >
                  <template #error>
                    <div class="image-placeholder small">
                      <el-icon><Picture /></el-icon>
                    </div>
                  </template>
                </el-image>
              </div>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="商家回复" min-width="200">
          <template #default="{ row }">
            <div v-if="row.reply" class="reply-content">
              {{ row.reply }}
              <div class="reply-time">{{ row.replyTime }}</div>
            </div>
            <el-button v-else link type="primary" @click="handleReply(row)">
              回复
            </el-button>
          </template>
        </el-table-column>
        <el-table-column label="评价时间" width="160">
          <template #default="{ row }">
            {{ row.createTime }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <div class="operation-buttons">
              <el-button link type="primary" @click="handleView(row)">查看</el-button>
              <el-button v-if="row.status === 'pending'" link type="success" @click="handleAudit(row, 'approved')">
                通过
              </el-button>
              <el-button v-if="row.status === 'pending'" link type="warning" @click="handleAudit(row, 'rejected')">
                拒绝
              </el-button>
              <el-button v-if="!row.reply" link type="primary" @click="handleReply(row)">
                回复
              </el-button>
              <el-button link type="danger" @click="handleDelete(row)">
                删除
              </el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination
        v-model:current-page="queryParams.pageIndex"
        v-model:page-size="queryParams.pageSize"
        :total="total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSearch"
        @current-change="handleSearch"
      />
    </el-card>

    <!-- 评价详情弹窗 -->
    <el-dialog v-model="detailDialogVisible" title="评价详情" width="700px">
      <el-descriptions :column="2" border v-if="currentReview">
        <el-descriptions-item label="商品名称">{{ currentReview.productName }}</el-descriptions-item>
        <el-descriptions-item label="订单编号">{{ currentReview.orderNo }}</el-descriptions-item>
        <el-descriptions-item label="用户">{{ currentReview.isAnonymous ? '匿名用户' : currentReview.userName }}</el-descriptions-item>
        <el-descriptions-item label="评分">
          <el-rate v-model="currentReview.rating" disabled />
        </el-descriptions-item>
        <el-descriptions-item label="评价时间">{{ currentReview.createTime }}</el-descriptions-item>
        <el-descriptions-item label="状态">
          <el-tag :type="getStatusType(currentReview.status)">
            {{ getStatusText(currentReview.status) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="评价内容" :span="2">
          {{ currentReview.content }}
        </el-descriptions-item>
        <el-descriptions-item label="评价图片" :span="2" v-if="currentReview.images?.length">
          <div class="detail-images">
            <el-image
              v-for="(img, index) in currentReview.images"
              :key="index"
              :src="img"
              :preview-src-list="currentReview.images"
              :initial-index="index"
              fit="cover"
              class="detail-image"
            />
          </div>
        </el-descriptions-item>
        <el-descriptions-item label="商家回复" :span="2" v-if="currentReview.reply">
          {{ currentReview.reply }}
          <div class="reply-time">{{ currentReview.replyTime }}</div>
        </el-descriptions-item>
      </el-descriptions>
    </el-dialog>

    <!-- 回复弹窗 -->
    <el-dialog v-model="replyDialogVisible" title="回复评价" width="500px">
      <el-form :model="replyForm" :rules="replyRules" ref="replyFormRef" label-width="80px">
        <el-form-item label="回复内容" prop="reply">
          <el-input
            v-model="replyForm.reply"
            type="textarea"
            :rows="4"
            placeholder="请输入回复内容"
            maxlength="500"
            show-word-limit
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="replyDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="replySaving" @click="submitReply">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Refresh, Picture } from '@element-plus/icons-vue'
import {
  getReviewList,
  getReviewStatistics,
  auditReview,
  replyReview,
  deleteReview,
  batchAuditReview,
} from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { ProductReview, ReviewStatistics } from '@/types/product'

const { t } = useLocale()
const route = useRoute()
const router = useRouter()

const loading = ref(false)
const tableData = ref<ProductReview[]>([])
const total = ref(0)
const selectedRows = ref<ProductReview[]>([])

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  productId: '' as string,
  productName: '',
  userName: '',
  rating: undefined as number | undefined,
  status: '' as '' | 'pending' | 'approved' | 'rejected',
  startDate: '',
  endDate: '',
})

const dateRange = computed({
  get: () => {
    if (queryParams.startDate && queryParams.endDate) {
      return [queryParams.startDate, queryParams.endDate]
    }
    return []
  },
  set: (val: string[]) => {
    if (val && val.length === 2) {
      queryParams.startDate = val[0]
      queryParams.endDate = val[1]
    } else {
      queryParams.startDate = ''
      queryParams.endDate = ''
    }
  },
})

const statistics = ref<ReviewStatistics>({
  total: 0,
  averageRating: 0,
  ratingDistribution: [],
  pendingCount: 0,
  approvedCount: 0,
  rejectedCount: 0,
})

// 详情弹窗
const detailDialogVisible = ref(false)
const currentReview = ref<ProductReview | null>(null)

// 回复弹窗
const replyDialogVisible = ref(false)
const replyFormRef = ref()
const replyForm = reactive({
  id: '',
  reply: '',
})
const replyRules = {
  reply: [{ required: true, message: '请输入回复内容', trigger: 'blur' }],
}
const replySaving = ref(false)

onMounted(() => {
  // 从URL参数获取商品ID和名称
  const { productId, productName } = route.query
  if (productId) {
    queryParams.productId = productId as string
  }
  if (productName) {
    queryParams.productName = productName as string
  }
  loadStatistics()
  handleSearch()
})

const loadStatistics = async () => {
  try {
    const data = await getReviewStatistics({
      productId: queryParams.productId || undefined,
      productName: queryParams.productName || undefined,
      userName: queryParams.userName || undefined,
      rating: queryParams.rating,
      status: queryParams.status || undefined,
      startDate: queryParams.startDate || undefined,
      endDate: queryParams.endDate || undefined,
    })
    statistics.value = data
  } catch (error) {
    // Error handled
  }
}

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getReviewList(queryParams)
    tableData.value = data.list
    total.value = data.total
    // 同时更新统计数据
    loadStatistics()
  } catch (error) {
    // Error handled
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.pageIndex = 1
  queryParams.userName = ''
  queryParams.rating = undefined
  queryParams.status = ''
  queryParams.startDate = ''
  queryParams.endDate = ''
  // 如果有商品筛选，也清除
  if (queryParams.productId) {
    queryParams.productId = ''
    queryParams.productName = ''
    // 清除URL参数
    router.replace({ path: '/product/review' })
  }
  loadStatistics()
  handleSearch()
}

/**
 * 清除商品筛选，查看全部评价
 */
const clearProductFilter = () => {
  queryParams.productId = ''
  queryParams.productName = ''
  router.replace({ path: '/product/review' })
  loadStatistics()
  handleSearch()
}

const handleSelectionChange = (rows: ProductReview[]) => {
  selectedRows.value = rows
}

const getStatusType = (status: string) => {
  const types: Record<string, string> = {
    pending: 'warning',
    approved: 'success',
    rejected: 'danger',
  }
  return types[status] || 'info'
}

const getStatusText = (status: string) => {
  const texts: Record<string, string> = {
    pending: '待审核',
    approved: '已通过',
    rejected: '已拒绝',
  }
  return texts[status] || status
}

const handleView = (row: ProductReview) => {
  currentReview.value = row
  detailDialogVisible.value = true
}

const handleAudit = async (row: ProductReview, status: 'pending' | 'approved' | 'rejected') => {
  try {
    await auditReview({ id: row.id, status })
    ElMessage.success('审核成功')
    row.status = status
    loadStatistics()
  } catch (error) {
    // Error handled
  }
}

const handleReply = (row: ProductReview) => {
  replyForm.id = row.id
  replyForm.reply = ''
  replyDialogVisible.value = true
}

const submitReply = async () => {
  try {
    await replyFormRef.value?.validate()
  } catch {
    return
  }

  replySaving.value = true
  try {
    await replyReview(replyForm)
    ElMessage.success('回复成功')
    replyDialogVisible.value = false
    handleSearch()
  } catch (error) {
    // Error handled
  } finally {
    replySaving.value = false
  }
}

const handleDelete = async (row: ProductReview) => {
  try {
    await ElMessageBox.confirm('确定要删除该评价吗？', '提示', {
      type: 'warning',
    })
    await deleteReview(row.id)
    ElMessage.success('删除成功')
    handleSearch()
    loadStatistics()
  } catch (error) {
    // Error handled
  }
}

const handleBatchAudit = async (status: 'approved' | 'rejected') => {
  const pendingRows = selectedRows.value.filter(r => r.status === 'pending')
  if (pendingRows.length === 0) {
    ElMessage.warning('请选择待审核的评价')
    return
  }

  try {
    await ElMessageBox.confirm(
      `确定要批量${status === 'approved' ? '通过' : '拒绝'} ${pendingRows.length} 条评价吗？`,
      '提示',
      { type: 'warning' }
    )
    await batchAuditReview(pendingRows.map(r => r.id), status)
    ElMessage.success('批量审核成功')
    handleSearch()
    loadStatistics()
  } catch (error) {
    // Error handled
  }
}

/**
 * 返回上一页
 */
const handleBack = () => {
  router.back()
}
</script>

<style scoped lang="scss">
.review-list-container {
  padding: 20px;

  .breadcrumb {
    margin-bottom: 16px;
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .header-right {
      display: flex;
      align-items: center;
      gap: 12px;
    }
  }

  .stats-row {
    margin-bottom: 20px;

    .stats-card {
      background: #f5f7fa;
      border-radius: 8px;
      padding: 16px 20px;
      display: flex;
      align-items: center;
      gap: 16px;

      .stats-value {
        font-size: 28px;
        font-weight: 600;
        color: #303133;

        &.rating {
          color: #e6a23c;
        }
      }

      .stats-label {
        font-size: 14px;
        color: #909399;
      }

      &.warning {
        background: #fdf6ec;

        .stats-value {
          color: #e6a23c;
        }
      }

      &.success {
        background: #f0f9eb;

        .stats-value {
          color: #67c23a;
        }
      }
    }
  }

  .search-form {
    margin-bottom: 16px;
  }

  .batch-actions {
    margin-bottom: 16px;
    padding: 10px;
    background: #f5f7fa;
    border-radius: 4px;

    .selected-count {
      margin-left: 16px;
      color: #909399;
    }
  }

  .product-info {
    display: flex;
    align-items: center;
    gap: 10px;

    .product-image {
      width: 50px;
      height: 50px;
      border-radius: 4px;
    }

    .product-name {
      flex: 1;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
  }

  .review-content {
    .content-text {
      line-height: 1.6;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }

    .content-images {
      display: flex;
      gap: 8px;
      margin-top: 8px;

      .review-image {
        width: 60px;
        height: 60px;
        border-radius: 4px;
        cursor: pointer;
      }
    }
  }

  .reply-content {
    .reply-time {
      font-size: 12px;
      color: #909399;
      margin-top: 4px;
    }
  }

  .image-placeholder {
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    background: #f5f7fa;
    color: #c0c4cc;

    &.small {
      width: 60px;
      height: 60px;
    }
  }

  .detail-images {
    display: flex;
    gap: 8px;

    .detail-image {
      width: 100px;
      height: 100px;
      border-radius: 4px;
    }
  }

  .operation-buttons {
    display: flex;
    align-items: center;
    gap: 8px;
  }
}
</style>
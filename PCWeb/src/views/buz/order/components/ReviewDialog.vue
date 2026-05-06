<!-- src/views/buz/order/components/ReviewDialog.vue -->
<template>
  <el-dialog
    v-model="dialogVisible"
    width="600px"
    destroy-on-close
    :close-on-click-modal="false"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('order.review.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="dialogVisible = false">{{ t('common.button.cancel') }}</el-button>
          <el-button type="primary" :loading="submitting" @click="handleSubmit">
            {{ t('order.review.submit') }}
          </el-button>
        </div>
      </div>
    </template>
    <div v-loading="loading" class="review-form">
      <!-- 商品信息 -->
      <div class="product-info">
        <el-image :src="product?.productImage" fit="cover" class="product-image" />
        <div class="product-detail">
          <div class="product-name">{{ product?.productName }}</div>
          <div class="product-price">¥{{ product?.price?.toFixed(2) }}</div>
        </div>
      </div>

      <!-- 商品评价 -->
      <div class="section">
        <div class="section-title">{{ t('order.review.productRating') }}</div>
        <div class="rating-items">
          <div class="rating-item">
            <span class="rating-label">{{ t('order.review.productQuality') }}</span>
            <el-rate v-model="form.productQuality" :colors="rateColors" show-score />
          </div>
          <div class="rating-item">
            <span class="rating-label">{{ t('order.review.descriptionMatch') }}</span>
            <el-rate v-model="form.descriptionMatch" :colors="rateColors" show-score />
          </div>
          <div class="rating-item">
            <span class="rating-label">{{ t('order.review.costPerformance') }}</span>
            <el-rate v-model="form.costPerformance" :colors="rateColors" show-score />
          </div>
        </div>
      </div>

      <!-- 服务评价 -->
      <div class="section">
        <div class="section-title">{{ t('order.review.serviceRating') }}</div>
        <div class="rating-items">
          <div class="rating-item">
            <span class="rating-label">{{ t('order.review.shippingSpeed') }}</span>
            <el-rate v-model="form.shippingSpeed" :colors="rateColors" show-score />
          </div>
          <div class="rating-item">
            <span class="rating-label">{{ t('order.review.logisticsService') }}</span>
            <el-rate v-model="form.logisticsService" :colors="rateColors" show-score />
          </div>
          <div class="rating-item">
            <span class="rating-label">{{ t('order.review.customerService') }}</span>
            <el-rate v-model="form.customerService" :colors="rateColors" show-score />
          </div>
        </div>
      </div>

      <!-- 综合评分 -->
      <div class="overall-rating">
        <span class="overall-label">{{ t('order.review.overallRating') }}</span>
        <el-rate :model-value="overallRating" :colors="rateColors" disabled show-score />
      </div>

      <!-- 快捷标签 -->
      <div class="section">
        <div class="section-title">{{ t('order.review.tags') }}</div>
        <div class="tag-group">
          <el-check-tag
            v-for="tag in currentTags"
            :key="tag.value"
            :checked="form.tags.includes(tag.value)"
            @change="handleTagChange(tag.value)"
          >
            {{ tag.label }}
          </el-check-tag>
        </div>
      </div>

      <!-- 评价内容 -->
      <div class="section">
        <div class="section-title">{{ t('order.review.content') }}</div>
        <el-input
          v-model="form.content"
          type="textarea"
          :rows="4"
          :placeholder="t('order.review.contentPlaceholder')"
          :maxlength="500"
          show-word-limit
        />
      </div>

      <!-- 图片上传 -->
      <div class="section">
        <div class="section-title">{{ t('order.review.images') }}</div>
        <el-upload
          v-model:file-list="imageFileList"
          action="#"
          list-type="picture-card"
          :auto-upload="false"
          :limit="9"
          :on-exceed="handleImageExceed"
          :accept="imageAccept"
        >
          <el-icon><Plus /></el-icon>
          <template #tip>
            <div class="upload-tip">
              {{ t('order.review.imageTip') }}
            </div>
          </template>
        </el-upload>
      </div>

      <!-- 视频上传 -->
      <div class="section">
        <div class="section-title">{{ t('order.review.video') }}</div>
        <el-upload
          v-model:file-list="videoFileList"
          action="#"
          :auto-upload="false"
          :limit="1"
          :on-exceed="handleVideoExceed"
          :accept="videoAccept"
        >
          <el-button type="primary">
            <el-icon><VideoCamera /></el-icon>
            {{ t('order.review.addVideo') }}
          </el-button>
          <template #tip>
            <div class="upload-tip">
              {{ t('order.review.videoTip') }}
            </div>
          </template>
        </el-upload>
      </div>

      <!-- 匿名评价 -->
      <div class="anonymous-option">
        <el-checkbox v-model="form.isAnonymous">
          {{ t('order.review.anonymous') }}
        </el-checkbox>
      </div>

      <!-- 积分提示 -->
      <div class="points-tip">
        <el-icon><Coin /></el-icon>
        <span>{{ t('order.review.pointsTip', { points: estimatedPoints }) }}</span>
      </div>
    </div>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, type UploadProps, type UploadUserFile } from 'element-plus'
import { Plus, VideoCamera, Coin } from '@element-plus/icons-vue'
import { getOrderDetail, createReview } from '@/api/buz/orderApi'
import { GOOD_TAGS, NEUTRAL_TAGS, BAD_TAGS } from '@/types/order'
import type { OrderItem, Order } from '@/types/order'

const { t } = useI18n()

const props = defineProps<{
  visible: boolean
  orderId: string
  productId?: string
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success', pointsEarned: number): void
}>()

const rateColors = ['#99A9BF', '#F7BA2A', '#FF9900']

const loading = ref(false)
const submitting = ref(false)
const order = ref<Order | null>(null)
const product = ref<OrderItem | null>(null)

const imageAccept = 'image/jpeg,image/png,image/webp'
const videoAccept = 'video/mp4,video/webm'

const imageFileList = ref<UploadUserFile[]>([])
const videoFileList = ref<UploadUserFile[]>([])

const form = ref({
  productQuality: 5,
  descriptionMatch: 5,
  costPerformance: 5,
  shippingSpeed: 5,
  logisticsService: 5,
  customerService: 5,
  content: '',
  tags: [] as string[],
  images: [] as string[],
  videos: [] as string[],
  isAnonymous: false,
})

const dialogVisible = computed({
  get: () => props.visible,
  set: (value) => emit('update:visible', value),
})

// 综合评分（自动计算）
const overallRating = computed(() => {
  const total = form.value.productQuality + form.value.descriptionMatch + form.value.costPerformance +
    form.value.shippingSpeed + form.value.logisticsService + form.value.customerService
  return Math.round(total / 6 * 10) / 10
})

// 根据评分动态显示标签
const currentTags = computed(() => {
  const rating = overallRating.value
  if (rating >= 4) return GOOD_TAGS
  if (rating >= 3) return [...GOOD_TAGS, ...NEUTRAL_TAGS]
  return BAD_TAGS
})

// 预估积分
const estimatedPoints = computed(() => {
  let points = 10 // 纯文字基础积分
  if (imageFileList.value.length > 0) {
    points = 20
    if (imageFileList.value.length >= 3) {
      points += 5 // 晒图奖励
    }
  }
  if (videoFileList.value.length > 0) {
    points = 30 // 含视频
  }
  return Math.min(points, 50) // 上限50
})

const handleTagChange = (tagValue: string) => {
  const index = form.value.tags.indexOf(tagValue)
  if (index > -1) {
    form.value.tags.splice(index, 1)
  } else {
    form.value.tags.push(tagValue)
  }
}

const handleImageExceed: UploadProps['onExceed'] = () => {
  ElMessage.warning(t('order.review.imageLimit'))
}

const handleVideoExceed: UploadProps['onExceed'] = () => {
  ElMessage.warning(t('order.review.videoLimit'))
}

const fetchOrderDetail = async () => {
  if (!props.orderId) return

  loading.value = true
  try {
    const res = await getOrderDetail(props.orderId)
    order.value = res

    // 确定要评价的商品
    if (props.productId) {
      product.value = res.items.find(item => item.productId === props.productId) || null
    } else {
      // 默认第一个商品
      product.value = res.items[0] || null
    }
  } catch (error) {
    console.error('Failed to fetch order detail:', error)
  } finally {
    loading.value = false
  }
}

const handleSubmit = async () => {
  if (!order.value || !product.value) return

  // 校验内容长度
  if (form.value.content.length < 10) {
    ElMessage.warning(t('order.review.contentMin'))
    return
  }

  submitting.value = true
  try {
    // Mock 图片和视频 URL（实际项目中需要先上传到 OSS）
    const images = imageFileList.value.map(file => file.url || file.name)
    const videos = videoFileList.value.map(file => file.url || file.name)

    const res = await createReview({
      orderId: order.value.id,
      productId: product.value.productId,
      productQuality: form.value.productQuality,
      descriptionMatch: form.value.descriptionMatch,
      costPerformance: form.value.costPerformance,
      shippingSpeed: form.value.shippingSpeed,
      logisticsService: form.value.logisticsService,
      customerService: form.value.customerService,
      content: form.value.content,
      images,
      videos,
      tags: form.value.tags,
      isAnonymous: form.value.isAnonymous,
    })

    ElMessage.success(t('order.review.success', { points: res.pointsEarned }))
    emit('success', res.pointsEarned)
    dialogVisible.value = false
  } catch (error) {
    console.error('Failed to submit review:', error)
  } finally {
    submitting.value = false
  }
}

watch(
  () => props.visible,
  (visible) => {
    if (visible && props.orderId) {
      // 重置表单
      form.value = {
        productQuality: 5,
        descriptionMatch: 5,
        costPerformance: 5,
        shippingSpeed: 5,
        logisticsService: 5,
        customerService: 5,
        content: '',
        tags: [],
        images: [],
        videos: [],
        isAnonymous: false,
      }
      imageFileList.value = []
      videoFileList.value = []
      fetchOrderDetail()
    }
  },
  { immediate: true }
)
</script>

<style scoped lang="less">
.review-form {
  .product-info {
    display: flex;
    align-items: center;
    padding: 12px;
    background: #f5f7fa;
    border-radius: 8px;
    margin-bottom: 20px;

    .product-image {
      width: 60px;
      height: 60px;
      border-radius: 4px;
    }

    .product-detail {
      margin-left: 12px;

      .product-name {
        font-size: 14px;
        color: #303133;
        margin-bottom: 4px;
      }

      .product-price {
        font-size: 14px;
        color: #f56c6c;
        font-weight: 500;
      }
    }
  }

  .section {
    margin-bottom: 20px;

    .section-title {
      font-size: 14px;
      font-weight: 600;
      color: #303133;
      margin-bottom: 12px;
    }

    .rating-items {
      .rating-item {
        display: flex;
        align-items: center;
        margin-bottom: 8px;

        .rating-label {
          width: 80px;
          font-size: 13px;
          color: #606266;
        }
      }
    }

    .tag-group {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
    }
  }

  .overall-rating {
    display: flex;
    align-items: center;
    padding: 12px;
    background: #ecf5ff;
    border-radius: 8px;
    margin-bottom: 20px;

    .overall-label {
      width: 80px;
      font-size: 14px;
      font-weight: 600;
      color: #409eff;
    }
  }

  .upload-tip {
    font-size: 12px;
    color: #909399;
    margin-top: 8px;
  }

  .anonymous-option {
    margin-bottom: 16px;
  }

  .points-tip {
    display: flex;
    align-items: center;
    padding: 12px;
    background: #fdf6ec;
    border-radius: 8px;
    color: #e6a23c;
    font-size: 14px;

    .el-icon {
      margin-right: 8px;
    }
  }
}

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
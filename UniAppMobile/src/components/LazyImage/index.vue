<!-- components/LazyImage/index.vue -->
<template>
  <view class="lazy-image-container">
    <!-- 占位图 -->
    <image
      v-if="!isLoaded"
      :src="placeholderSrc"
      :mode="mode"
      class="placeholder-image"
    />

    <!-- 实际图片 -->
    <image
      :src="actualSrc"
      :mode="mode"
      :lazy-load="true"
      class="actual-image"
      :class="{ loaded: isLoaded }"
      @load="onLoad"
      @error="onError"
    />

    <!-- 加载失败占位 -->
    <view v-if="hasError" class="error-placeholder">
      <text class="error-icon">!</text>
      <text class="error-text">加载失败</text>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'

const props = defineProps({
  /** 图片地址 */
  src: {
    type: String,
    default: '',
  },
  /** 图片裁剪、缩放模式 */
  mode: {
    type: String,
    default: 'aspectFill',
  },
  /** 占位图地址 */
  placeholder: {
    type: String,
    default: '',
  },
  /** 图片宽度 */
  width: {
    type: [String, Number],
    default: '100%',
  },
  /** 图片高度 */
  height: {
    type: [String, Number],
    default: 'auto',
  },
})

// 是否已加载完成
const isLoaded = ref(false)

// 是否加载失败
const hasError = ref(false)

// 默认占位图（灰色背景）
const defaultPlaceholder = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9IjEwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnL3N2ZyI+PHJlY3Qgd2lkdGg9IjEwMCIgaGVpZ2h0PSIxMDAiIGZpbGw9IiNlZWVlZWUiLz48dGV4dCB4PSI1MCIgeT0iNTAiIGZpbGw9IiNjY2NjY2MiIGZvbnQtZmFtaWx5PSJBcmlhbCIgZm9udC1zaXplPSIxMiIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPuWbvuS8iueDn+iKPC90ZXh0Pjwvc3ZnPg=='

// 占位图地址
const placeholderSrc = computed(() => {
  return props.placeholder || defaultPlaceholder
})

// 实际图片地址
const actualSrc = ref('')

// 监听 src 变化
watch(() => props.src, (newSrc) => {
  if (newSrc) {
    isLoaded.value = false
    hasError.value = false
    // 使用 IntersectionObserver 实现懒加载
    observeImage(newSrc)
  }
}, { immediate: true })

// 图片加载完成
const onLoad = () => {
  isLoaded.value = true
  hasError.value = false
}

// 图片加载失败
const onError = () => {
  hasError.value = true
  isLoaded.value = false
}

// 观察图片（懒加载逻辑）
const observeImage = (src: string) => {
  // 在 uni-app 中，image 标签的 lazy-load 属性会自动处理
  // 这里我们手动控制显示时机
  actualSrc.value = src
}

onMounted(() => {
  // 初始化时如果已有 src，开始加载
  if (props.src) {
    observeImage(props.src)
  }
})
</script>

<style lang="scss" scoped>
.lazy-image-container {
  position: relative;
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.placeholder-image {
  width: 100%;
  height: 100%;
  opacity: 1;
  transition: opacity 0.3s;
}

.actual-image {
  width: 100%;
  height: 100%;
  opacity: 0;
  transition: opacity 0.3s;

  &.loaded {
    opacity: 1;
  }
}

.error-placeholder {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: #f5f5f5;

  .error-icon {
    font-size: 32rpx;
    font-weight: bold;
    color: #ff4d4f;
  }

  .error-text {
    font-size: 24rpx;
    color: #999;
    margin-top: 8rpx;
  }
}
</style>
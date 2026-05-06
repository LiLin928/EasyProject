<!-- src/views/buz/customer/components/FavoriteViewer.vue -->
<template>
  <div class="favorite-viewer">
    <div class="favorite-header">
      <el-select
        v-model="selectedGroupId"
        :placeholder="t('customer.favoriteGroup')"
        size="small"
        style="width: 150px"
        @change="loadFavoriteList"
      >
        <el-option :label="t('customer.allGroup')" value="" />
        <el-option
          v-for="group in groupList"
          :key="group.id"
          :label="group.name"
          :value="group.id"
        />
      </el-select>
    </div>

    <div v-loading="loading" class="favorite-content">
      <template v-if="favoriteList.length > 0">
        <div class="favorite-list">
          <div
            v-for="item in favoriteList"
            :key="item.id"
            class="favorite-item"
          >
            <div class="product-image">
              <el-image
                :src="item.productImage"
                fit="cover"
                style="width: 80px; height: 80px"
              >
                <template #error>
                  <div class="image-placeholder">
                    <el-icon><Picture /></el-icon>
                  </div>
                </template>
              </el-image>
            </div>
            <div class="product-info">
              <div class="product-name">{{ item.productName }}</div>
              <div class="group-name">
                <el-tag type="info" size="small">{{ item.groupName }}</el-tag>
              </div>
              <div class="favorite-time">{{ t('customer.favoriteTime') }}: {{ item.createTime }}</div>
            </div>
          </div>
        </div>
      </template>
      <el-empty v-else :description="t('customer.favoriteEmpty')" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Picture } from '@element-plus/icons-vue'
import { getCustomerFavoriteList, getFavoriteGroupList } from '@/api/buz/customerApi'
import { useLocale } from '@/composables/useLocale'
import type { CustomerFavorite, FavoriteGroup } from '@/types'

const props = defineProps<{
  customerId: string
}>()

const { t } = useLocale()

const loading = ref(false)
const groupList = ref<FavoriteGroup[]>([])
const favoriteList = ref<CustomerFavorite[]>([])
const selectedGroupId = ref('')

// Load group list
const loadGroupList = async () => {
  if (!props.customerId) return
  try {
    const data = await getFavoriteGroupList(props.customerId)
    groupList.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

// Load favorite list
const loadFavoriteList = async () => {
  if (!props.customerId) return
  loading.value = true
  try {
    const data = await getCustomerFavoriteList(
      props.customerId,
      selectedGroupId.value || undefined
    )
    favoriteList.value = data
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

// Watch customer id change
watch(() => props.customerId, (val) => {
  if (val) {
    selectedGroupId.value = ''
    loadGroupList()
    loadFavoriteList()
  }
}, { immediate: true })
</script>

<style scoped lang="scss">
.favorite-viewer {
  .favorite-header {
    margin-bottom: 16px;
  }

  .favorite-content {
    min-height: 200px;
  }

  .favorite-list {
    .favorite-item {
      display: flex;
      align-items: center;
      padding: 16px 0;
      border-bottom: 1px solid #f0f0f0;

      .product-image {
        flex-shrink: 0;
        margin-right: 12px;

        .image-placeholder {
          display: flex;
          align-items: center;
          justify-content: center;
          width: 80px;
          height: 80px;
          background: #f5f7fa;
          color: #909399;
        }
      }

      .product-info {
        flex: 1;
        min-width: 0;

        .product-name {
          font-size: 14px;
          color: #303133;
          margin-bottom: 8px;
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
        }

        .group-name {
          margin-bottom: 8px;
        }

        .favorite-time {
          font-size: 12px;
          color: #c0c4cc;
        }
      }
    }
  }
}
</style>
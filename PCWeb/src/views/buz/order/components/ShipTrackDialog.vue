<!-- src/views/buz/order/components/ShipTrackDialog.vue -->
<template>
  <el-dialog
    v-model="dialogVisible"
    width="500px"
    destroy-on-close
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('order.shipTrack.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="dialogVisible = false">{{ t('common.button.close') }}</el-button>
        </div>
      </div>
    </template>
    <div v-loading="loading" class="ship-track">
      <!-- 快递信息 -->
      <div class="ship-info">
        <div class="ship-item">
          <span class="label">{{ t('order.shipTrack.company') }}</span>
          <span class="value">{{ getShipCompanyLabel(trackData?.company) }}</span>
        </div>
        <div class="ship-item">
          <span class="label">{{ t('order.shipTrack.shipNo') }}</span>
          <span class="value">{{ trackData?.shipNo }}</span>
        </div>
        <div class="ship-item">
          <span class="label">{{ t('order.shipTrack.status') }}</span>
          <el-tag :type="trackData?.isSigned ? 'success' : 'primary'" size="small">
            {{ trackData?.isSigned ? t('order.shipTrack.signed') : t('order.shipTrack.inTransit') }}
          </el-tag>
        </div>
      </div>

      <!-- 物流轨迹 -->
      <div class="track-timeline">
        <el-timeline>
          <el-timeline-item
            v-for="(item, index) in trackData?.tracks"
            :key="index"
            :timestamp="item.time"
            :color="index === 0 ? '#409eff' : '#909399'"
            placement="top"
          >
            <div class="track-item">
              <div class="track-status">{{ item.status }}</div>
              <div v-if="item.location" class="track-location">{{ item.location }}</div>
              <div class="track-desc">{{ item.description }}</div>
            </div>
          </el-timeline-item>
        </el-timeline>
      </div>
    </div>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { getShipTrack } from '@/api/buz/orderApi'
import { SHIP_COMPANIES } from '@/types/order'
import type { ShipTrackResponse } from '@/types/order'

const { t } = useI18n()

const props = defineProps<{
  visible: boolean
  orderId: string
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

const loading = ref(false)
const trackData = ref<ShipTrackResponse | null>(null)

const dialogVisible = computed({
  get: () => props.visible,
  set: (value) => emit('update:visible', value),
})

const getShipCompanyLabel = (company?: string) => {
  if (!company) return '-'
  const found = SHIP_COMPANIES.find(c => c.value === company)
  return found ? found.label : company
}

const fetchShipTrack = async () => {
  if (!props.orderId) return

  loading.value = true
  try {
    const res = await getShipTrack(props.orderId)
    trackData.value = res
  } catch (error) {
    console.error('Failed to fetch ship track:', error)
  } finally {
    loading.value = false
  }
}

watch(
  () => props.visible,
  (visible) => {
    if (visible && props.orderId) {
      trackData.value = null
      fetchShipTrack()
    }
  },
  { immediate: true }
)
</script>

<style scoped lang="less">
.ship-track {
  .ship-info {
    display: flex;
    flex-wrap: wrap;
    padding: 16px;
    background: #f5f7fa;
    border-radius: 8px;
    margin-bottom: 20px;

    .ship-item {
      width: 50%;
      display: flex;
      align-items: center;
      margin-bottom: 8px;

      &:last-child {
        width: 100%;
        margin-bottom: 0;
      }

      .label {
        font-size: 13px;
        color: #909399;
        width: 70px;
      }

      .value {
        font-size: 14px;
        color: #303133;
      }
    }
  }

  .track-timeline {
    padding: 0 20px;

    .track-item {
      .track-status {
        font-size: 14px;
        font-weight: 500;
        color: #303133;
        margin-bottom: 4px;
      }

      .track-location {
        font-size: 12px;
        color: #909399;
        margin-bottom: 4px;
      }

      .track-desc {
        font-size: 13px;
        color: #606266;
      }
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
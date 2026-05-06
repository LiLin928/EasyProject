<!-- src/views/report/components/DrilldownConfigDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="480px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('columnTemplate.edit.drilldown') }}</span>
        <div class="dialog-actions">
          <el-button @click="handleClose">{{ t('columnTemplate.edit.cancel') }}</el-button>
          <el-button type="primary" @click="handleSave">{{ t('columnTemplate.edit.save') }}</el-button>
        </div>
      </div>
    </template>
    <el-form :model="formData" label-width="100px">
      <el-form-item :label="t('columnTemplate.edit.drilldownEnabled')">
        <el-switch v-model="formData.enabled" />
      </el-form-item>

      <template v-if="formData.enabled">
        <el-form-item :label="t('columnTemplate.edit.targetReport')">
          <el-select
            v-model="formData.targetReportId"
            :placeholder="t('columnTemplate.edit.targetReportPlaceholder')"
            style="width: 100%"
          >
            <el-option
              v-for="report in reports"
              :key="report.id"
              :label="report.name"
              :value="report.id"
            />
          </el-select>
        </el-form-item>

        <el-form-item :label="t('columnTemplate.edit.paramMapping')">
          <div class="param-list">
            <div
              v-for="(param, index) in formData.params"
              :key="index"
              class="param-item"
            >
              <el-select
                v-model="param.sourceField"
                :placeholder="t('columnTemplate.edit.sourceField')"
                style="width: 120px"
              >
                <el-option
                  :label="t('columnTemplate.edit.clickedValue')"
                  value="__clicked_value__"
                />
                <el-option
                  v-for="field in availableFields"
                  :key="field"
                  :label="field"
                  :value="field"
                />
              </el-select>
              <span class="arrow">-></span>
              <el-input
                v-model="param.targetParam"
                :placeholder="t('columnTemplate.edit.targetParam')"
                style="width: 120px"
              />
              <el-button
                link
                type="danger"
                @click="removeParam(index)"
              >
                {{ t('columnTemplate.edit.removeParam') }}
              </el-button>
            </div>
            <el-button @click="addParam" type="primary" link>
              + {{ t('columnTemplate.edit.addParam') }}
            </el-button>
          </div>
        </el-form-item>
      </template>
    </el-form>

    </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, computed } from 'vue'
import { useLocale } from '@/composables/useLocale'
import type { DrilldownRule, DrilldownParam } from '@/types'
import { getReportList } from '@/api/report/reportApi'

const props = defineProps<{
  modelValue: boolean
  config?: DrilldownRule
  availableFields: string[]
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'save', config: DrilldownRule): void
}>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value),
})

const reports = ref<{ id: string; name: string }[]>([])

const formData = reactive<DrilldownRule>({
  enabled: false,
  targetReportId: 0,
  params: [],
})

watch(visible, async (val) => {
  if (val) {
    await loadReports()
    if (props.config) {
      formData.enabled = props.config.enabled
      formData.targetReportId = props.config.targetReportId
      formData.params = [...(props.config.params || [])]
    } else {
      resetForm()
    }
  }
})

const loadReports = async () => {
  try {
    const data = await getReportList({ pageIndex: 1, pageSize: 100 })
    reports.value = data.list.map((r) => ({ id: r.id, name: r.name }))
  } catch (error) {
    // Error handled
  }
}

const addParam = () => {
  formData.params.push({ sourceField: '__clicked_value__', targetParam: '' })
}

const removeParam = (index: number) => {
  formData.params.splice(index, 1)
}

const resetForm = () => {
  formData.enabled = false
  formData.targetReportId = 0
  formData.params = []
}

const handleClose = () => {
  visible.value = false
}

const handleSave = () => {
  emit('save', { ...formData })
  handleClose()
}
</script>

<style scoped lang="scss">
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

.param-list {
  .param-item {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 8px;

    .arrow {
      color: #909399;
    }
  }
}
</style>
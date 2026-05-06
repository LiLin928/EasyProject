<!-- src/views/report/ColumnTemplateEdit.vue -->
<template>
  <div class="column-template-edit">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ isEdit ? t('columnTemplate.edit.editTitle') : t('columnTemplate.edit.createTitle') }}</span>
          <el-button @click="goBack">
            {{ t('columnTemplate.edit.back') }}
          </el-button>
        </div>
      </template>

      <div v-loading="loading" class="edit-content">
        <el-form
          ref="formRef"
          :model="formData"
          :rules="formRules"
          label-width="120px"
        >
          <!-- Basic Information -->
          <div class="section-title">{{ t('columnTemplate.edit.basicInfo') }}</div>
          <el-form-item :label="t('columnTemplate.edit.name')" prop="name">
            <el-input
              v-model="formData.name"
              :placeholder="t('columnTemplate.edit.namePlaceholder')"
            />
          </el-form-item>
          <el-form-item :label="t('columnTemplate.edit.type')" prop="type">
            <el-radio-group v-model="formData.type" @change="handleTypeChange">
              <el-radio-button value="single">{{ t('columnTemplate.edit.typeSingle') }}</el-radio-button>
              <el-radio-button value="table">{{ t('columnTemplate.edit.typeTable') }}</el-radio-button>
            </el-radio-group>
          </el-form-item>
          <el-form-item :label="t('columnTemplate.edit.description')">
            <el-input
              v-model="formData.description"
              type="textarea"
              :rows="2"
              :placeholder="t('columnTemplate.edit.descriptionPlaceholder')"
            />
          </el-form-item>

          <!-- Data Source Configuration -->
          <div class="section-title">{{ t('columnTemplate.edit.dataSourceConfig') }}</div>
          <el-form-item :label="t('columnTemplate.edit.dataSource')">
            <el-select
              v-model="formData.dataSourceId"
              :placeholder="t('columnTemplate.edit.dataSourcePlaceholder')"
              clearable
              style="width: 300px"
            >
              <el-option
                v-for="ds in dataSources"
                :key="ds.id"
                :label="ds.name"
                :value="ds.id"
              />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('columnTemplate.edit.sqlQuery')">
            <div class="sql-input-wrapper">
              <el-input
                v-model="formData.sqlQuery"
                type="textarea"
                :rows="4"
                :placeholder="t('columnTemplate.edit.sqlPlaceholder')"
              />
              <el-button
                type="primary"
                :loading="fetchingColumns"
                :disabled="!formData.dataSourceId || !formData.sqlQuery"
                @click="handleFetchColumns"
                style="margin-top: 8px"
              >
                <el-icon><Search /></el-icon>
                {{ t('columnTemplate.edit.fetchColumns') }}
              </el-button>
            </div>
          </el-form-item>

          <!-- Column Configuration -->
          <div class="section-title">{{ t('columnTemplate.edit.columnConfig') }}</div>
          <el-form-item>
            <div class="column-config-table">
              <el-table :data="columnConfigs" border size="small" style="width: 100%">
                <el-table-column prop="field" :label="t('columnTemplate.edit.fieldName')" width="180">
                  <template #default="{ row }">
                    <el-input
                      v-model="row.field"
                      size="small"
                      :placeholder="t('columnTemplate.edit.fieldNamePlaceholder')"
                    />
                  </template>
                </el-table-column>
                <el-table-column prop="label" :label="t('columnTemplate.edit.label')" width="180">
                  <template #default="{ row }">
                    <el-input
                      v-model="row.label"
                      size="small"
                      :placeholder="t('columnTemplate.edit.labelPlaceholder')"
                    />
                  </template>
                </el-table-column>
                <el-table-column prop="width" :label="t('columnTemplate.edit.width')" width="120">
                  <template #default="{ row }">
                    <el-input-number
                      v-model="row.width"
                      size="small"
                      :min="50"
                      :max="500"
                      style="width: 100px"
                    />
                  </template>
                </el-table-column>
                <el-table-column prop="align" :label="t('columnTemplate.edit.align')" width="120">
                  <template #default="{ row }">
                    <el-select v-model="row.align" size="small">
                      <el-option :label="t('columnTemplate.edit.alignLeft')" value="left" />
                      <el-option :label="t('columnTemplate.edit.alignCenter')" value="center" />
                      <el-option :label="t('columnTemplate.edit.alignRight')" value="right" />
                    </el-select>
                  </template>
                </el-table-column>
                <el-table-column prop="format" :label="t('columnTemplate.edit.format')" width="130">
                  <template #default="{ row }">
                    <el-select v-model="row.format" size="small" clearable>
                      <el-option :label="t('columnTemplate.edit.formatNone')" value="" />
                      <el-option :label="t('columnTemplate.edit.formatNumber')" value="number" />
                      <el-option :label="t('columnTemplate.edit.formatMoney')" value="money" />
                      <el-option :label="t('columnTemplate.edit.formatDate')" value="date" />
                      <el-option :label="t('columnTemplate.edit.formatPercent')" value="percent" />
                    </el-select>
                  </template>
                </el-table-column>
                <el-table-column :label="t('columnTemplate.edit.drilldown')" width="150">
                  <template #default="{ row, $index }">
                    <el-button
                      v-if="row.drilldown?.enabled"
                      type="primary"
                      link
                      size="small"
                      @click="openDrilldownDialog($index)"
                    >
                      {{ t('columnTemplate.edit.drilldownEnabled') }}
                    </el-button>
                    <el-button
                      v-else
                      link
                      size="small"
                      @click="openDrilldownDialog($index)"
                    >
                      {{ t('columnTemplate.edit.drilldown') }}
                    </el-button>
                  </template>
                </el-table-column>
                <el-table-column v-if="formData.type === 'table'" :label="t('columnTemplate.list.operation')" width="100">
                  <template #default="{ $index }">
                    <el-button
                      link
                      type="danger"
                      size="small"
                      @click="removeColumn($index)"
                    >
                      {{ t('columnTemplate.edit.removeColumn') }}
                    </el-button>
                  </template>
                </el-table-column>
              </el-table>
              <div v-if="formData.type === 'table'" class="column-actions">
                <el-button size="small" @click="addColumn">
                  {{ t('columnTemplate.edit.addColumn') }}
                </el-button>
              </div>
            </div>
          </el-form-item>

          <!-- Actions -->
          <el-form-item>
            <el-button type="primary" @click="handleSave">
              <el-icon><Check /></el-icon>
              {{ t('columnTemplate.edit.save') }}
            </el-button>
            <el-button @click="goBack">
              {{ t('columnTemplate.edit.cancel') }}
            </el-button>
          </el-form-item>
        </el-form>
      </div>
    </el-card>

    <!-- Drilldown Config Dialog -->
    <DrilldownConfigDialog
      v-model="drilldownDialogVisible"
      :config="currentColumnIndex !== undefined ? columnConfigs[currentColumnIndex]?.drilldown : undefined"
      :available-fields="availableFields"
      @save="saveDrilldownConfig"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Check, Search } from '@element-plus/icons-vue'
import type { FormInstance, FormRules } from 'element-plus'
import {
  getColumnTemplateDetail,
  createColumnTemplate,
  updateColumnTemplate,
  fetchColumnsFromSql,
} from '@/api/report/columnTemplateApi'
import { getDataSourceList } from '@/api/report/datasourceApi'
import DrilldownConfigDialog from './components/DrilldownConfigDialog.vue'
import { useLocale } from '@/composables/useLocale'
import type {
  ColumnTemplateType,
  ColumnConfig,
  DrilldownRule,
  DataSource,
} from '@/types'

const router = useRouter()
const route = useRoute()
const { t } = useLocale()

const loading = ref(false)
const fetchingColumns = ref(false)
const isEdit = ref(false)
const formRef = ref<FormInstance | null>(null)
const dataSources = ref<DataSource[]>([])

const formData = reactive<{
  name: string
  type: ColumnTemplateType
  description: string
  dataSourceId: string
  sqlQuery: string
}>({
  name: '',
  type: 'table',
  description: '',
  dataSourceId: '',
  sqlQuery: '',
})

const columnConfigs = ref<ColumnConfig[]>([])

// Drilldown config dialog
const drilldownDialogVisible = ref(false)
const currentColumnIndex = ref<number | undefined>()

// Available fields for drilldown config
const availableFields = computed(() => {
  return columnConfigs.value.map((c) => c.field).filter(Boolean)
})

const formRules: FormRules = {
  name: [
    { required: true, message: t('columnTemplate.edit.validation.nameRequired'), trigger: 'blur' },
  ],
  type: [
    { required: true, message: t('columnTemplate.edit.validation.typeRequired'), trigger: 'change' },
  ],
}

onMounted(async () => {
  await loadDataSources()
  const id = route.params.id
  if (id) {
    isEdit.value = true
    await loadTemplate(id as string)
  } else {
    // Initialize with one empty column for table type
    addColumn()
  }
})

const loadDataSources = async () => {
  try {
    const data = await getDataSourceList({ pageIndex: 1, pageSize: 100 })
    dataSources.value = data.list
  } catch (error) {
    // Error handled by interceptor
  }
}

const loadTemplate = async (id: string) => {
  loading.value = true
  try {
    const data = await getColumnTemplateDetail(id)
    formData.name = data.name
    formData.type = data.type
    formData.description = data.description || ''
    formData.dataSourceId = data.dataSourceId || ''
    formData.sqlQuery = data.sqlQuery || ''
    columnConfigs.value = data.columns ? JSON.parse(JSON.stringify(data.columns)) : []
  } catch (error) {
    ElMessage.error('Failed to load template')
    router.push('/report/column-template')
  } finally {
    loading.value = false
  }
}

const handleTypeChange = (type: ColumnTemplateType) => {
  if (type === 'single') {
    // Single type: ensure exactly one column
    if (columnConfigs.value.length === 0) {
      addColumn()
    } else {
      columnConfigs.value = [columnConfigs.value[0]]
    }
  } else {
    // Table type: ensure at least one column
    if (columnConfigs.value.length === 0) {
      addColumn()
    }
  }
}

const handleFetchColumns = async () => {
  if (!formData.dataSourceId || !formData.sqlQuery) {
    ElMessage.warning(t('columnTemplate.edit.selectDataSourceAndSql'))
    return
  }

  fetchingColumns.value = true
  try {
    const result = await fetchColumnsFromSql({
      dataSourceId: formData.dataSourceId,
      sqlQuery: formData.sqlQuery,
    })

    // 将 DetectedColumn 转换为 ColumnConfig 格式
    columnConfigs.value = result.map(col => ({
      field: col.field,
      label: col.field, // 默认使用字段名作为标签
      width: 100,
      align: 'left',
    }))

    // 如果是单列模式，只保留第一列
    if (formData.type === 'single' && columnConfigs.value.length > 1) {
      columnConfigs.value = [columnConfigs.value[0]]
    }

    ElMessage.success(t('columnTemplate.edit.fetchColumnsSuccess'))
  } catch (error) {
    ElMessage.error(t('columnTemplate.edit.fetchColumnsFailed'))
  } finally {
    fetchingColumns.value = false
  }
}

const addColumn = () => {
  columnConfigs.value.push({
    field: '',
    label: '',
    width: 100,
    align: 'left',
  })
}

const removeColumn = (index: number) => {
  columnConfigs.value.splice(index, 1)
}

const openDrilldownDialog = (index: number) => {
  currentColumnIndex.value = index
  drilldownDialogVisible.value = true
}

const saveDrilldownConfig = (config: DrilldownRule) => {
  if (currentColumnIndex.value !== undefined) {
    columnConfigs.value[currentColumnIndex.value].drilldown = config
  }
}

const handleSave = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch {
    return
  }

  // Validate columns
  if (columnConfigs.value.length === 0) {
    ElMessage.warning('Please add at least one column')
    return
  }

  for (const col of columnConfigs.value) {
    if (!col.field) {
      ElMessage.warning(t('columnTemplate.edit.validation.fieldNameRequired'))
      return
    }
    if (!col.label) {
      ElMessage.warning(t('columnTemplate.edit.validation.labelRequired'))
      return
    }
  }

  loading.value = true
  try {
    const params = {
      name: formData.name,
      type: formData.type,
      description: formData.description,
      dataSourceId: formData.dataSourceId || undefined,
      sqlQuery: formData.sqlQuery || undefined,
      columns: columnConfigs.value,
    }

    if (isEdit.value) {
      const id = route.params.id as string
      await updateColumnTemplate({ id, ...params })
    } else {
      await createColumnTemplate(params)
    }
    ElMessage.success('Template saved successfully')
    router.push('/report/column-template')
  } catch (error) {
    ElMessage.error('Failed to save template')
  } finally {
    loading.value = false
  }
}

const goBack = () => {
  router.push('/report/column-template')
}
</script>

<style scoped lang="scss">
.column-template-edit {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .edit-content {
    max-width: 1000px;

    .section-title {
      font-weight: bold;
      margin: 16px 0 12px;
      padding-bottom: 8px;
      border-bottom: 1px solid #ebeef5;
    }

    .sql-input-wrapper {
      width: 100%;
    }

    .column-config-table {
      width: 100%;

      .column-actions {
        margin-top: 8px;
      }
    }
  }
}
</style>
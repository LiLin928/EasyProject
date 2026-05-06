<!-- src/views/basic/dict/index.vue -->
<template>
  <div class="dict-container">
    <el-card shadow="never">
      <template #header>
        <span>{{ t('dict.dict.title') }}</span>
      </template>

      <div class="dict-layout">
        <!-- 左侧：字典类型 -->
        <div class="dict-type-panel">
          <div class="panel-header">
            <span>{{ t('dict.dict.dictType') }}</span>
            <el-button type="primary" size="small" @click="handleAddType">
              {{ t('dict.dict.addType') }}
            </el-button>
          </div>

          <!-- 搜索 -->
          <el-input
            v-model="typeSearchKey"
            :placeholder="t('dict.dict.searchTypePlaceholder')"
            clearable
            style="margin-bottom: 12px"
          />

          <!-- 类型列表 -->
          <el-table
            v-loading="typeLoading"
            :data="filteredTypeList"
            highlight-current-row
            @current-change="handleTypeSelect"
          >
            <el-table-column prop="name" :label="t('dict.dict.typeName')">
              <template #default="{ row }">
                <span class="type-name" :class="{ disabled: row.status === 0 }">
                  {{ row.name }}
                </span>
              </template>
            </el-table-column>
            <el-table-column :label="t('dict.dict.status')" width="80" align="center">
              <template #default="{ row }">
                <el-switch
                  v-model="row.status"
                  :active-value="1"
                  :inactive-value="0"
                  size="small"
                  @change="handleTypeStatusChange(row)"
                />
              </template>
            </el-table-column>
            <el-table-column :label="t('dict.dict.operation')" width="100" align="center">
              <template #default="{ row }">
                <el-button link type="primary" size="small" @click="handleEditType(row)">
                  {{ t('dict.dict.editType') }}
                </el-button>
                <el-button link type="danger" size="small" @click="handleDeleteType(row)">
                  {{ t('dict.dict.deleteType') }}
                </el-button>
              </template>
            </el-table-column>
          </el-table>
        </div>

        <!-- 右侧：字典数据 -->
        <div class="dict-data-panel">
          <div class="panel-header">
            <span>
              {{ t('dict.dict.dictData') }}
              <template v-if="selectedType">
                - {{ selectedType.name }}
              </template>
            </span>
            <div>
              <el-button
                type="danger"
                size="small"
                :disabled="selectedDataRows.length === 0"
                @click="handleBatchDeleteData"
              >
                {{ t('dict.dict.batchDelete') }}
              </el-button>
              <el-button
                type="primary"
                size="small"
                :disabled="!selectedType"
                @click="handleAddData"
              >
                {{ t('dict.dict.addData') }}
              </el-button>
            </div>
          </div>

          <!-- 提示或数据表格 -->
          <template v-if="!selectedType">
            <el-empty :description="t('dict.dict.selectDictType')" />
          </template>
          <template v-else>
            <el-table
              v-loading="dataLoading"
              :data="dataTableList"
              @selection-change="handleDataSelectionChange"
            >
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column prop="label" :label="t('dict.dict.dataLabel')" min-width="120" />
              <el-table-column prop="value" :label="t('dict.dict.dataValue')" min-width="120" />
              <el-table-column prop="sort" :label="t('dict.dict.dataSort')" width="80" align="center" />
              <el-table-column :label="t('dict.dict.status')" width="80" align="center">
                <template #default="{ row }">
                  <el-switch
                    v-model="row.status"
                    :active-value="1"
                    :inactive-value="0"
                    size="small"
                    @change="handleDataStatusChange(row)"
                  />
                </template>
              </el-table-column>
              <el-table-column :label="t('dict.dict.operation')" width="100" align="center">
                <template #default="{ row }">
                  <el-button link type="primary" size="small" @click="handleEditData(row)">
                    {{ t('dict.dict.editData') }}
                  </el-button>
                  <el-button link type="danger" size="small" @click="handleDeleteData(row)">
                    {{ t('dict.dict.deleteData') }}
                  </el-button>
                </template>
              </el-table-column>
            </el-table>

            <el-pagination
              v-model:current-page="dataQueryParams.pageIndex"
              v-model:page-size="dataQueryParams.pageSize"
              :total="dataTotal"
              :page-sizes="[10, 20, 50]"
              layout="total, sizes, prev, pager, next"
              style="margin-top: 12px; justify-content: flex-end"
              @size-change="loadDataList"
              @current-change="loadDataList"
            />
          </template>
        </div>
      </div>
    </el-card>

    <!-- 字典类型弹窗 -->
    <DictTypeFormDialog
      v-model="typeDialogVisible"
      :type-id="currentTypeId"
      @success="handleTypeSuccess"
    />

    <!-- 字典数据弹窗 -->
    <DictDataFormDialog
      v-model="dataDialogVisible"
      :data-id="currentDataId"
      :type-code="selectedType?.code || ''"
      :current-data="currentData"
      @success="loadDataList"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  getDictTypeList,
  getDictDataList,
  updateDictType,
  updateDictData,
  deleteDictType,
  deleteDictData,
  deleteDictDataBatch,
} from '@/api/dict'
import { useLocale } from '@/composables/useLocale'
import type { DictType, DictData } from '@/types/dict'
import { CommonStatus } from '@/types/enums'
import DictTypeFormDialog from './components/DictTypeFormDialog.vue'
import DictDataFormDialog from './components/DictDataFormDialog.vue'

const { t } = useLocale()

// 字典类型状态
const typeLoading = ref(false)
const typeList = ref<DictType[]>([])
const typeSearchKey = ref('')
const selectedType = ref<DictType | null>(null)

// 字典数据状态
const dataLoading = ref(false)
const dataTableList = ref<DictData[]>([])
const dataTotal = ref(0)
const selectedDataRows = ref<DictData[]>([])

const dataQueryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  typeCode: '',
})

// 弹窗状态
const typeDialogVisible = ref(false)
const currentTypeId = ref<string | undefined>(undefined)
const dataDialogVisible = ref(false)
const currentDataId = ref<string | undefined>(undefined)
const currentData = ref<DictData | undefined>(undefined)

// 过滤后的类型列表
const filteredTypeList = computed(() => {
  if (!typeSearchKey.value) return typeList.value
  return typeList.value.filter((item) =>
    item.name.toLowerCase().includes(typeSearchKey.value.toLowerCase())
  )
})

onMounted(() => {
  loadTypeList()
})

// 加载字典类型列表
const loadTypeList = async () => {
  typeLoading.value = true
  try {
    const data = await getDictTypeList({
      pageIndex: 1,
      pageSize: 100,
    })
    typeList.value = data.list
    // 默认选中第一个
    if (typeList.value.length > 0 && !selectedType.value) {
      handleTypeSelect(typeList.value[0])
    }
  } catch (error) {
    // Error handled by interceptor
  } finally {
    typeLoading.value = false
  }
}

// 加载字典数据列表
const loadDataList = async () => {
  if (!selectedType.value) return

  dataLoading.value = true
  try {
    const data = await getDictDataList({
      ...dataQueryParams,
      typeCode: selectedType.value.code,
    })
    dataTableList.value = data.list
    dataTotal.value = data.total
  } catch (error) {
    // Error handled by interceptor
  } finally {
    dataLoading.value = false
  }
}

// 选择字典类型
const handleTypeSelect = (row: DictType | null) => {
  selectedType.value = row
  if (row) {
    dataQueryParams.typeCode = row.code
    dataQueryParams.pageIndex = 1
    loadDataList()
  }
}

// 字典类型操作
const handleAddType = () => {
  currentTypeId.value = undefined
  typeDialogVisible.value = true
}

const handleEditType = (row: DictType) => {
  currentTypeId.value = row.id
  typeDialogVisible.value = true
}

const handleTypeSuccess = () => {
  loadTypeList()
}

const handleTypeStatusChange = async (row: DictType) => {
  try {
    await updateDictType({
      id: row.id,
      status: row.status,
    })
    ElMessage.success(row.status === CommonStatus.ENABLED ? t('dict.dict.enabled') : t('dict.dict.disabled'))
  } catch (error) {
    row.status = row.status === CommonStatus.ENABLED ? CommonStatus.DISABLED : CommonStatus.ENABLED
  }
}

const handleDeleteType = async (row: DictType) => {
  try {
    await ElMessageBox.confirm(
      t('dict.dict.deleteTypeConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteDictType(row.id)
    ElMessage.success(t('dict.dict.deleteTypeSuccess'))
    // 清空选中状态
    if (selectedType.value?.id === row.id) {
      selectedType.value = null
      dataTableList.value = []
      dataTotal.value = 0
    }
    loadTypeList()
  } catch (error) {
    // User cancelled or request failed
  }
}

// 字典数据操作
const handleAddData = () => {
  currentDataId.value = undefined
  currentData.value = undefined
  dataDialogVisible.value = true
}

const handleEditData = (row: DictData) => {
  currentDataId.value = row.id
  currentData.value = row
  dataDialogVisible.value = true
}

const handleDeleteData = async (row: DictData) => {
  try {
    await ElMessageBox.confirm(
      t('dict.dict.deleteDataConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteDictData(row.id)
    ElMessage.success(t('dict.dict.deleteDataSuccess'))
    loadDataList()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleDataStatusChange = async (row: DictData) => {
  try {
    await updateDictData({
      id: row.id,
      status: row.status,
    })
    ElMessage.success(row.status === CommonStatus.ENABLED ? t('dict.dict.enabled') : t('dict.dict.disabled'))
  } catch (error) {
    row.status = row.status === CommonStatus.ENABLED ? CommonStatus.DISABLED : CommonStatus.ENABLED
  }
}

const handleDataSelectionChange = (rows: DictData[]) => {
  selectedDataRows.value = rows
}

const handleBatchDeleteData = async () => {
  if (selectedDataRows.value.length === 0) return

  try {
    await ElMessageBox.confirm(
      t('dict.dict.deleteDatasConfirm', { count: selectedDataRows.value.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    const ids = selectedDataRows.value.map((row) => row.id)
    await deleteDictDataBatch(ids)
    ElMessage.success(t('dict.dict.deleteDataSuccess'))
    loadDataList()
  } catch (error) {
    // User cancelled or request failed
  }
}
</script>

<style scoped lang="scss">
.dict-container {
  padding: 20px;

  .dict-layout {
    display: flex;
    gap: 20px;
    min-height: 500px;
  }

  .dict-type-panel {
    width: 320px;
    flex-shrink: 0;
    border-right: 1px solid var(--el-border-color-lighter);
    padding-right: 20px;
  }

  .dict-data-panel {
    flex: 1;
    min-width: 0;
  }

  .panel-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 12px;
    font-weight: 500;
  }

  .type-name {
    cursor: pointer;

    &.disabled {
      color: var(--el-text-color-placeholder);
    }
  }
}
</style>
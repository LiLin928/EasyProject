<!-- 大屏列表页面 -->
<template>
  <div class="screen-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('screen.screen.title') }}</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>
            {{ t('screen.screen.add') }}
          </el-button>
        </div>
      </template>

      <!-- 搜索栏 -->
      <el-form :inline="true" class="search-form">
        <el-form-item :label="t('screen.screen.name')">
          <el-input
            v-model="queryParams.name"
            :placeholder="t('screen.screen.namePlaceholder')"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('screen.screen.status')">
          <el-select
            v-model="queryParams.isPublic"
            :placeholder="t('screen.screen.statusPlaceholder')"
            clearable
            style="width: 120px"
          >
            <el-option :label="t('screen.screen.public')" :value="1" />
            <el-option :label="t('screen.screen.private')" :value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            {{ t('screen.screen.search') }}
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            {{ t('screen.screen.reset') }}
          </el-button>
        </el-form-item>
      </el-form>

      <!-- 大屏卡片网格 -->
      <div class="screen-grid" v-loading="loading">
        <!-- 大屏卡片列表 -->
        <ScreenCard
          v-for="screen in screenList"
          :key="screen.id"
          :screen="screen"
          @refresh="handleSearch"
        />

        <!-- 无数据提示 -->
        <div v-if="!loading && screenList.length === 0" class="screen-empty">
          <el-empty :description="t('screen.screen.noData')" />
        </div>
      </div>

      <!-- 分页 -->
      <div class="pagination-wrapper" v-if="total > queryParams.pageSize">
        <el-pagination
          v-model:current-page="queryParams.pageIndex"
          v-model:page-size="queryParams.pageSize"
          :total="total"
          :page-sizes="[12, 24, 48]"
          layout="total, sizes, prev, pager, next"
          @current-change="handleSearch"
          @size-change="handleSearch"
        />
      </div>
    </el-card>

    <!-- 新增大屏弹窗 -->
    <el-dialog
      v-model="dialogVisible"
      :title="t('screen.screen.add')"
      width="500px"
      class="screen-dialog"
    >
      <el-form :model="formData" label-width="80px">
        <el-form-item :label="t('screen.screen.name')" required>
          <el-input v-model="formData.name" :placeholder="t('screen.screen.namePlaceholder')" />
        </el-form-item>
        <el-form-item :label="t('screen.screen.description')">
          <el-input v-model="formData.description" type="textarea" :rows="3" placeholder="请输入大屏描述" />
        </el-form-item>
        <el-form-item :label="t('screen.screen.status')">
          <el-radio-group v-model="formData.isPublic">
            <el-radio :value="false">{{ t('screen.screen.private') }}</el-radio>
            <el-radio :value="true">{{ t('screen.screen.public') }}</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">{{ t('common.button.cancel') }}</el-button>
        <el-button type="primary" @click="handleCreate">{{ t('common.button.confirm') }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus, Search, Refresh } from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import { getScreenList, createScreen } from '@/api/screen'
import type { ScreenConfig } from '@/types'
import ScreenCard from './components/ScreenCard.vue'

const { t } = useLocale()

const loading = ref(false)
const screenList = ref<ScreenConfig[]>([])
const total = ref(0)

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 12,
  name: '',
  isPublic: undefined as number | undefined,
})

const dialogVisible = ref(false)
const formData = reactive({
  name: '',
  description: '',
  isPublic: false,
})

onMounted(() => {
  handleSearch()
})

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getScreenList({
      pageIndex: queryParams.pageIndex,
      pageSize: queryParams.pageSize,
      name: queryParams.name || undefined,
      isPublic: queryParams.isPublic,
    })
    screenList.value = data.list
    total.value = data.total
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.name = ''
  queryParams.isPublic = undefined
  queryParams.pageIndex = 1
  handleSearch()
}

const handleAdd = () => {
  formData.name = ''
  formData.description = ''
  formData.isPublic = false
  dialogVisible.value = true
}

const handleCreate = async () => {
  if (!formData.name) {
    ElMessage.warning(t('screen.screen.nameRequired'))
    return
  }

  try {
    await createScreen({
      name: formData.name,
      description: formData.description,
      isPublic: formData.isPublic ? 1 : 0,
      style: JSON.stringify({
        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
        width: 1920,
        height: 1080,
      }),
      permissions: JSON.stringify({ sharedUsers: [], sharedRoles: [] }),
    })
    ElMessage.success(t('screen.screen.createSuccess'))
    dialogVisible.value = false
    handleSearch()
  } catch (error) {
    // Error handled by interceptor
  }
}
</script>

<style scoped lang="scss">
.screen-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .search-form {
    margin-bottom: 16px;
  }

  .screen-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
    gap: 16px;
    margin-top: 0;
  }

  .screen-empty {
    grid-column: 1 / -1;
    display: flex;
    justify-content: center;
    padding: 40px 0;
  }

  .pagination-wrapper {
    display: flex;
    justify-content: flex-end;
    margin-top: 16px;
  }
}

.screen-dialog {
  :deep(.el-dialog) {
    border-radius: 12px;
  }
}
</style>
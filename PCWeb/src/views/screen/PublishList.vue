<!-- 大屏发布列表页面 -->
<template>
  <div class="publish-container">
    <el-card shadow="never">
      <!-- 搜索栏 -->
      <el-form :inline="true" class="search-form">
        <el-form-item label="大屏名称">
          <el-input
            v-model="queryParams.name"
            placeholder="请输入大屏名称"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryParams.status" placeholder="全部" clearable style="width: 120px">
            <el-option label="已发布" :value="1" />
            <el-option label="已下架" :value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
        </el-form-item>
      </el-form>

      <!-- 表格 -->
      <el-table :data="publishList" v-loading="loading" border stripe>
        <el-table-column prop="screenName" label="大屏名称" min-width="150" />
        <el-table-column prop="screenDescription" label="描述" min-width="180" show-overflow-tooltip>
          <template #default="{ row }">
            {{ row.screenDescription || '暂无描述' }}
          </template>
        </el-table-column>
        <el-table-column label="发布链接" min-width="200" show-overflow-tooltip>
          <template #default="{ row }">
            <el-link v-if="row.status === 1" :href="row.publishUrl" target="_blank" type="primary">
              {{ row.publishUrl }}
            </el-link>
            <span v-else class="text-gray">{{ row.publishUrl }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="publishedAt" label="发布时间" width="160" />
        <el-table-column prop="viewCount" label="访问次数" width="100" align="center" />
        <el-table-column label="状态" width="90" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status === 1 ? 'success' : 'info'" size="small">
              {{ row.status === 1 ? '已发布' : '已下架' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="220" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleView(row)">
              <el-icon><View /></el-icon>
              查看
            </el-button>
            <el-button link type="primary" @click="handleCopyLink(row)">
              <el-icon><Link /></el-icon>
              复制链接
            </el-button>
            <el-button
              v-if="row.status === 1"
              link
              type="danger"
              @click="handleUnpublish(row)"
            >
              <el-icon><Close /></el-icon>
              下架
            </el-button>
            <el-button
              v-if="row.status === 0"
              link
              type="success"
              @click="handleRepublish(row)"
            >
              <el-icon><Top /></el-icon>
              上架
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination-wrapper">
        <el-pagination
          v-model:current-page="queryParams.pageIndex"
          v-model:page-size="queryParams.pageSize"
          :total="total"
          :page-sizes="[10, 20, 50]"
          layout="total, sizes, prev, pager, next, jumper"
          @current-change="handleSearch"
          @size-change="handleSearch"
        />
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Refresh, View, Link, Close, Top } from '@element-plus/icons-vue'
import { getScreenPublishList, unpublishScreenByPublishId, republishScreen } from '@/api/screen'
import type { ScreenPublishRecord } from '@/types'

const loading = ref(false)
const publishList = ref<ScreenPublishRecord[]>([])
const total = ref(0)

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  name: '',
  status: undefined as number | undefined,
})

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getScreenPublishList(queryParams)
    publishList.value = data.list
    total.value = data.total
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.name = ''
  queryParams.status = undefined
  queryParams.pageIndex = 1
  handleSearch()
}

const handleView = (record: ScreenPublishRecord) => {
  window.open(`/#/screen/publish/${record.publishId}`, '_blank')
}

const handleCopyLink = (record: ScreenPublishRecord) => {
  navigator.clipboard.writeText(record.publishUrl)
  ElMessage.success('链接已复制到剪贴板')
}

const handleUnpublish = async (record: ScreenPublishRecord) => {
  try {
    await ElMessageBox.confirm('确定要下架该发布吗？下架后可以重新上架。', '提示', {
      type: 'warning',
    })
    await unpublishScreenByPublishId(record.publishId)
    ElMessage.success('下架成功')
    handleSearch()
  } catch {
    // 用户取消
  }
}

const handleRepublish = async (record: ScreenPublishRecord) => {
  try {
    await ElMessageBox.confirm('确定要重新上架该发布吗？', '提示', {
      type: 'info',
    })
    await republishScreen(record.publishId)
    ElMessage.success('上架成功')
    handleSearch()
  } catch {
    // 用户取消
  }
}

onMounted(() => {
  handleSearch()
})
</script>

<style scoped lang="scss">
.publish-container {
  padding: 20px;
}

.search-form {
  margin-bottom: 16px;
}

.pagination-wrapper {
  display: flex;
  justify-content: flex-end;
  margin-top: 16px;
}

.text-gray {
  color: #999;
}
</style>
<!-- pages/basic/dict/index.vue -->
<template>
  <view class="dict-page">
    <!-- 字典类型区域 -->
    <view class="dict-type-section">
      <!-- 搜索栏 -->
      <view class="search-bar">
        <input
          v-model="searchKeyword"
          class="search-input"
          type="text"
          placeholder="搜索字典名称/编码"
          @confirm="handleSearch"
        />
        <view class="search-btn" @click="handleSearch">
          <text>搜索</text>
        </view>
      </view>

      <!-- 字典类型列表 -->
      <scroll-view class="type-scroll" scroll-y>
        <view class="type-list">
          <view
            v-for="item in dictTypeList"
            :key="item.id"
            :class="['type-card', { active: selectedTypeId === item.id }]"
            @click="handleSelectType(item)"
          >
            <view class="type-info">
              <view class="type-header">
                <text class="type-name">{{ item.name }}</text>
                <view class="type-tags">
                  <text :class="['tag', item.status === DictStatus.Enabled ? 'tag-enabled' : 'tag-disabled']">
                    {{ item.status === DictStatus.Enabled ? '启用' : '禁用' }}
                  </text>
                </view>
              </view>
              <view class="type-detail">
                <text class="detail-row">
                  <text class="label">编码：</text>
                  <text class="value">{{ item.code }}</text>
                </text>
                <text v-if="item.description" class="detail-row">
                  <text class="label">说明：</text>
                  <text class="value">{{ item.description }}</text>
                </text>
              </view>
            </view>
            <view class="type-actions">
              <view class="action-btn action-edit" @click.stop="handleEditType(item)">
                <text>编辑</text>
              </view>
              <view class="action-btn action-delete" @click.stop="handleDeleteType(item)">
                <text>删除</text>
              </view>
            </view>
          </view>

          <!-- 加载状态 -->
          <view v-if="loadingType" class="loading-tip">
            <text>加载中...</text>
          </view>

          <!-- 空状态 -->
          <view v-if="!loadingType && dictTypeList.length === 0" class="empty-state">
            <text class="empty-icon">📚</text>
            <text class="empty-text">暂无字典类型</text>
          </view>
        </view>
      </scroll-view>

      <!-- 新增字典类型按钮 -->
      <view class="add-type-btn" @click="handleAddType">
        <text>+ 新增类型</text>
      </view>
    </view>

    <!-- 字典数据区域 -->
    <view class="dict-data-section">
      <!-- 数据标题栏 -->
      <view class="data-header">
        <text class="data-title">
          {{ selectedType ? selectedType.name + ' - 字典数据' : '字典数据' }}
        </text>
        <view v-if="selectedTypeId" class="add-data-btn" @click="handleAddData">
          <text>+ 新增数据</text>
        </view>
      </view>

      <!-- 字典数据列表 -->
      <scroll-view class="data-scroll" scroll-y>
        <view class="data-list">
          <view
            v-for="item in dictDataList"
            :key="item.id"
            class="data-card"
          >
            <view class="data-info">
              <view class="data-header">
                <text class="data-label">{{ item.label }}</text>
                <view class="data-tags">
                  <text :class="['tag', item.status === DictStatus.Enabled ? 'tag-enabled' : 'tag-disabled']">
                    {{ item.status === DictStatus.Enabled ? '启用' : '禁用' }}
                  </text>
                </view>
              </view>
              <view class="data-detail">
                <text class="detail-row">
                  <text class="label">值：</text>
                  <text class="value">{{ item.value }}</text>
                </text>
                <text class="detail-row">
                  <text class="label">排序：</text>
                  <text class="value">{{ item.sort }}</text>
                </text>
              </view>
            </view>
            <view class="data-actions">
              <view class="action-btn action-edit" @click="handleEditData(item)">
                <text>编辑</text>
              </view>
              <view class="action-btn action-delete" @click.stop="handleDeleteData(item)">
                <text>删除</text>
              </view>
            </view>
          </view>

          <!-- 加载状态 -->
          <view v-if="loadingData" class="loading-tip">
            <text>加载中...</text>
          </view>

          <!-- 空状态 -->
          <view v-if="!loadingData && selectedTypeId && dictDataList.length === 0" class="empty-state">
            <text class="empty-icon">📝</text>
            <text class="empty-text">暂无字典数据</text>
          </view>

          <!-- 未选择提示 -->
          <view v-if="!selectedTypeId" class="empty-state">
            <text class="empty-icon">👆</text>
            <text class="empty-text">请选择字典类型查看数据</text>
          </view>
        </view>
      </scroll-view>
    </view>

    <!-- 字典类型编辑弹窗 -->
    <view v-if="showTypeModal" class="modal-mask" @click="showTypeModal = false">
      <view class="modal-content" @click.stop>
        <view class="modal-header">
          <text class="modal-title">{{ editingType ? '编辑字典类型' : '新增字典类型' }}</text>
          <view class="modal-close" @click="showTypeModal = false">
            <text>×</text>
          </view>
        </view>
        <view class="modal-body">
          <view class="form-item">
            <text class="form-label">名称</text>
            <input
              v-model="typeForm.name"
              class="form-input"
              type="text"
              placeholder="请输入名称"
            />
          </view>
          <view class="form-item">
            <text class="form-label">编码</text>
            <input
              v-model="typeForm.code"
              class="form-input"
              type="text"
              placeholder="请输入编码"
            />
          </view>
          <view class="form-item">
            <text class="form-label">说明</text>
            <input
              v-model="typeForm.description"
              class="form-input"
              type="text"
              placeholder="请输入说明"
            />
          </view>
          <view class="form-item">
            <text class="form-label">状态</text>
            <view class="status-switch">
              <view
                :class="['switch-item', { active: typeForm.status === DictStatus.Enabled }]"
                @click="typeForm.status = DictStatus.Enabled"
              >
                <text>启用</text>
              </view>
              <view
                :class="['switch-item', { active: typeForm.status === DictStatus.Disabled }]"
                @click="typeForm.status = DictStatus.Disabled"
              >
                <text>禁用</text>
              </view>
            </view>
          </view>
        </view>
        <view class="modal-footer">
          <view class="btn-cancel" @click="showTypeModal = false">
            <text>取消</text>
          </view>
          <view class="btn-confirm" @click="handleSaveType">
            <text>确定</text>
          </view>
        </view>
      </view>
    </view>

    <!-- 字典数据编辑弹窗 -->
    <view v-if="showDataModal" class="modal-mask" @click="showDataModal = false">
      <view class="modal-content" @click.stop>
        <view class="modal-header">
          <text class="modal-title">{{ editingData ? '编辑字典数据' : '新增字典数据' }}</text>
          <view class="modal-close" @click="showDataModal = false">
            <text>×</text>
          </view>
        </view>
        <view class="modal-body">
          <view class="form-item">
            <text class="form-label">标签</text>
            <input
              v-model="dataForm.label"
              class="form-input"
              type="text"
              placeholder="请输入标签"
            />
          </view>
          <view class="form-item">
            <text class="form-label">值</text>
            <input
              v-model="dataForm.value"
              class="form-input"
              type="text"
              placeholder="请输入值"
            />
          </view>
          <view class="form-item">
            <text class="form-label">排序</text>
            <input
              v-model.number="dataForm.sort"
              class="form-input"
              type="number"
              placeholder="请输入排序值"
            />
          </view>
          <view class="form-item">
            <text class="form-label">状态</text>
            <view class="status-switch">
              <view
                :class="['switch-item', { active: dataForm.status === DictStatus.Enabled }]"
                @click="dataForm.status = DictStatus.Enabled"
              >
                <text>启用</text>
              </view>
              <view
                :class="['switch-item', { active: dataForm.status === DictStatus.Disabled }]"
                @click="dataForm.status = DictStatus.Disabled"
              >
                <text>禁用</text>
              </view>
            </view>
          </view>
        </view>
        <view class="modal-footer">
          <view class="btn-cancel" @click="showDataModal = false">
            <text>取消</text>
          </view>
          <view class="btn-confirm" @click="handleSaveData">
            <text>确定</text>
          </view>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import {
  getDictTypeList,
  getDictDataList,
  createDictType,
  updateDictType,
  deleteDictType,
  createDictData,
  updateDictData,
  deleteDictData,
} from '@/api/basic/dictApi'
import type { DictType, DictData } from '@/types/basic'
import { DictStatus } from '@/types/basic'

// 搜索关键字
const searchKeyword = ref('')

// 搜索防抖定时器
let searchTimer: number | null = null

// 字典类型列表
const dictTypeList = ref<DictType[]>([])
const loadingType = ref(false)

// 选中的字典类型
const selectedTypeId = ref<string>('')
const selectedType = ref<DictType | null>(null)

// 字典数据列表
const dictDataList = ref<DictData[]>([])
const loadingData = ref(false)

// 字典类型编辑弹窗
const showTypeModal = ref(false)
const editingType = ref<DictType | null>(null)
const typeForm = ref({
  name: '',
  code: '',
  description: '',
  status: DictStatus.Enabled,
})

// 字典数据编辑弹窗
const showDataModal = ref(false)
const editingData = ref<DictData | null>(null)
const dataForm = ref({
  label: '',
  value: '',
  sort: 0,
  status: DictStatus.Enabled,
})

// 加载字典类型列表
const loadDictTypes = async () => {
  loadingType.value = true
  try {
    const list = await getDictTypeList()
    // 搜索过滤
    const keyword = searchKeyword.value.trim().toLowerCase()
    if (keyword) {
      dictTypeList.value = list.filter(
        item =>
          item.name.toLowerCase().includes(keyword) ||
          item.code.toLowerCase().includes(keyword)
      )
    } else {
      dictTypeList.value = list
    }

    // 如果选中的类型不在列表中，清除选中状态
    if (selectedTypeId.value && !dictTypeList.value.find(t => t.id === selectedTypeId.value)) {
      selectedTypeId.value = ''
      selectedType.value = null
      dictDataList.value = []
    }
  } catch (error: unknown) {
    console.error('加载字典类型失败:', error)
    const message = (error as { errMsg?: string; message?: string })?.errMsg
      || (error as { message?: string })?.message
      || '加载失败'
    uni.showToast({ title: message, icon: 'none' })
  } finally {
    loadingType.value = false
  }
}

// 加载字典数据列表
const loadDictData = async () => {
  if (!selectedTypeId.value) return

  loadingData.value = true
  try {
    const list = await getDictDataList(selectedTypeId.value)
    dictDataList.value = list.sort((a, b) => a.sort - b.sort)
  } catch (error: unknown) {
    console.error('加载字典数据失败:', error)
    const message = (error as { errMsg?: string; message?: string })?.errMsg
      || (error as { message?: string })?.message
      || '加载失败'
    uni.showToast({ title: message, icon: 'none' })
  } finally {
    loadingData.value = false
  }
}

// 搜索（防抖处理）
const handleSearch = () => {
  if (searchTimer) clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    loadDictTypes()
  }, 300) as unknown as number
}

// 选择字典类型
const handleSelectType = (item: DictType) => {
  selectedTypeId.value = item.id
  selectedType.value = item
  loadDictData()
}

// 新增字典类型
const handleAddType = () => {
  editingType.value = null
  typeForm.value = {
    name: '',
    code: '',
    description: '',
    status: DictStatus.Enabled,
  }
  showTypeModal.value = true
}

// 编辑字典类型
const handleEditType = (item: DictType) => {
  editingType.value = item
  typeForm.value = {
    name: item.name,
    code: item.code,
    description: item.description || '',
    status: item.status,
  }
  showTypeModal.value = true
}

// 保存字典类型
const handleSaveType = async () => {
  // 表单验证
  if (!typeForm.value.name.trim()) {
    uni.showToast({ title: '请输入名称', icon: 'none' })
    return
  }
  if (!typeForm.value.code.trim()) {
    uni.showToast({ title: '请输入编码', icon: 'none' })
    return
  }

  try {
    uni.showLoading({ title: '保存中...' })

    if (editingType.value) {
      // 更新
      await updateDictType({
        id: editingType.value.id,
        name: typeForm.value.name.trim(),
        code: typeForm.value.code.trim(),
        description: typeForm.value.description.trim(),
        status: typeForm.value.status,
      })
    } else {
      // 新增
      await createDictType({
        name: typeForm.value.name.trim(),
        code: typeForm.value.code.trim(),
        description: typeForm.value.description.trim(),
        status: typeForm.value.status,
      })
    }

    uni.hideLoading()
    uni.showToast({ title: '保存成功', icon: 'success' })
    showTypeModal.value = false
    loadDictTypes()
  } catch (error: unknown) {
    uni.hideLoading()
    console.error('保存字典类型失败:', error)
    const message = (error as { errMsg?: string; message?: string })?.errMsg
      || (error as { message?: string })?.message
      || '保存失败'
    uni.showToast({ title: message, icon: 'none' })
  }
}

// 删除字典类型
const handleDeleteType = async (item: DictType) => {
  const res = await uni.showModal({
    title: '确认删除',
    content: `确定要删除字典类型 "${item.name}" 吗？删除后其下所有字典数据也将被删除。`,
  })

  if (!res.confirm) return

  try {
    uni.showLoading({ title: '删除中...' })
    await deleteDictType(item.id)
    uni.hideLoading()
    uni.showToast({ title: '删除成功', icon: 'success' })

    // 清除选中状态
    if (selectedTypeId.value === item.id) {
      selectedTypeId.value = ''
      selectedType.value = null
      dictDataList.value = []
    }

    loadDictTypes()
  } catch (error: unknown) {
    uni.hideLoading()
    console.error('删除字典类型失败:', error)
    const message = (error as { errMsg?: string; message?: string })?.errMsg
      || (error as { message?: string })?.message
      || '删除失败'
    uni.showToast({ title: message, icon: 'none' })
  }
}

// 新增字典数据
const handleAddData = () => {
  if (!selectedTypeId.value) {
    uni.showToast({ title: '请先选择字典类型', icon: 'none' })
    return
  }

  editingData.value = null
  dataForm.value = {
    label: '',
    value: '',
    sort: dictDataList.value.length,
    status: DictStatus.Enabled,
  }
  showDataModal.value = true
}

// 编辑字典数据
const handleEditData = (item: DictData) => {
  editingData.value = item
  dataForm.value = {
    label: item.label,
    value: item.value,
    sort: item.sort,
    status: item.status,
  }
  showDataModal.value = true
}

// 保存字典数据
const handleSaveData = async () => {
  // 表单验证
  if (!dataForm.value.label.trim()) {
    uni.showToast({ title: '请输入标签', icon: 'none' })
    return
  }
  if (!dataForm.value.value.trim()) {
    uni.showToast({ title: '请输入值', icon: 'none' })
    return
  }

  try {
    uni.showLoading({ title: '保存中...' })

    if (editingData.value) {
      // 更新
      await updateDictData({
        id: editingData.value.id,
        dictTypeId: selectedTypeId.value,
        label: dataForm.value.label.trim(),
        value: dataForm.value.value.trim(),
        sort: dataForm.value.sort,
        status: dataForm.value.status,
      })
    } else {
      // 新增
      await createDictData({
        dictTypeId: selectedTypeId.value,
        label: dataForm.value.label.trim(),
        value: dataForm.value.value.trim(),
        sort: dataForm.value.sort,
        status: dataForm.value.status,
      })
    }

    uni.hideLoading()
    uni.showToast({ title: '保存成功', icon: 'success' })
    showDataModal.value = false
    loadDictData()
  } catch (error: unknown) {
    uni.hideLoading()
    console.error('保存字典数据失败:', error)
    const message = (error as { errMsg?: string; message?: string })?.errMsg
      || (error as { message?: string })?.message
      || '保存失败'
    uni.showToast({ title: message, icon: 'none' })
  }
}

// 删除字典数据
const handleDeleteData = async (item: DictData) => {
  const res = await uni.showModal({
    title: '确认删除',
    content: `确定要删除字典数据 "${item.label}" 吗？`,
  })

  if (!res.confirm) return

  try {
    uni.showLoading({ title: '删除中...' })
    await deleteDictData(item.id)
    uni.hideLoading()
    uni.showToast({ title: '删除成功', icon: 'success' })
    loadDictData()
  } catch (error: unknown) {
    uni.hideLoading()
    console.error('删除字典数据失败:', error)
    const message = (error as { errMsg?: string; message?: string })?.errMsg
      || (error as { message?: string })?.message
      || '删除失败'
    uni.showToast({ title: message, icon: 'none' })
  }
}

onMounted(() => {
  loadDictTypes()
})

onUnmounted(() => {
  if (searchTimer) clearTimeout(searchTimer)
})
</script>

<style lang="scss" scoped>
.dict-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: $u-bg-color;
}

// 字典类型区域
.dict-type-section {
  flex: 0 0 45%;
  display: flex;
  flex-direction: column;
  background: #fff;
  border-bottom: 2rpx solid $u-border-color;
}

.search-bar {
  display: flex;
  padding: 20rpx;
  background: #fff;
  gap: 16rpx;

  .search-input {
    flex: 1;
    height: 72rpx;
    padding: 0 24rpx;
    background: $u-light-color;
    border-radius: $u-radius;
    font-size: 28rpx;
  }

  .search-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 120rpx;
    height: 72rpx;
    background: $u-primary;
    border-radius: $u-radius;

    text {
      color: #fff;
      font-size: 28rpx;
    }
  }
}

.type-scroll {
  flex: 1;
  overflow: hidden;
}

.type-list {
  padding: 20rpx;
}

.type-card {
  display: flex;
  justify-content: space-between;
  background: #fff;
  border-radius: $u-radius-lg;
  padding: 20rpx;
  margin-bottom: 16rpx;
  border: 2rpx solid $u-border-color;
  transition: all 0.2s;

  &.active {
    border-color: $u-primary;
    background: rgba($u-primary, 0.05);
  }

  .type-info {
    flex: 1;
    display: flex;
    flex-direction: column;
  }

  .type-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 12rpx;
  }

  .type-name {
    font-size: 28rpx;
    font-weight: 600;
    color: $u-main-color;
    flex: 1;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .type-tags {
    display: flex;
    gap: 8rpx;
    flex-shrink: 0;

    .tag {
      font-size: 20rpx;
      padding: 4rpx 12rpx;
      border-radius: 4rpx;
    }

    .tag-enabled {
      background: rgba($u-primary, 0.1);
      color: $u-primary;
    }

    .tag-disabled {
      background: rgba($u-content-color, 0.1);
      color: $u-content-color;
    }
  }

  .type-detail {
    display: flex;
    flex-direction: column;
    gap: 6rpx;

    .detail-row {
      display: flex;
      align-items: center;

      .label {
        font-size: 22rpx;
        color: $u-tips-color;
        width: 80rpx;
      }

      .value {
        font-size: 22rpx;
        color: $u-main-color;
      }
    }
  }

  .type-actions {
    display: flex;
    align-items: center;
    padding-left: 16rpx;
    gap: 12rpx;

    .action-btn {
      padding: 8rpx 16rpx;
      border-radius: $u-radius;
      font-size: 22rpx;
    }

    .action-edit {
      background: rgba($u-primary, 0.1);
      color: $u-primary;
    }

    .action-delete {
      background: rgba($u-error, 0.1);
      color: $u-error;
    }
  }
}

.add-type-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20rpx;
  background: #fff;
  border-top: 1rpx solid $u-border-color;

  text {
    font-size: 28rpx;
    color: $u-primary;
  }
}

// 字典数据区域
.dict-data-section {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: $u-bg-color;
}

.data-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20rpx;
  background: #fff;
  border-bottom: 1rpx solid $u-border-color;

  .data-title {
    font-size: 28rpx;
    font-weight: 600;
    color: $u-main-color;
  }

  .add-data-btn {
    padding: 8rpx 20rpx;
    background: rgba($u-primary, 0.1);
    border-radius: $u-radius;

    text {
      font-size: 24rpx;
      color: $u-primary;
    }
  }
}

.data-scroll {
  flex: 1;
  overflow: hidden;
}

.data-list {
  padding: 20rpx;
}

.data-card {
  display: flex;
  justify-content: space-between;
  background: #fff;
  border-radius: $u-radius-lg;
  padding: 20rpx;
  margin-bottom: 16rpx;

  .data-info {
    flex: 1;
    display: flex;
    flex-direction: column;
  }

  .data-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 12rpx;
  }

  .data-label {
    font-size: 28rpx;
    font-weight: 600;
    color: $u-main-color;
    flex: 1;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .data-tags {
    display: flex;
    gap: 8rpx;
    flex-shrink: 0;

    .tag {
      font-size: 20rpx;
      padding: 4rpx 12rpx;
      border-radius: 4rpx;
    }

    .tag-enabled {
      background: rgba($u-primary, 0.1);
      color: $u-primary;
    }

    .tag-disabled {
      background: rgba($u-content-color, 0.1);
      color: $u-content-color;
    }
  }

  .data-detail {
    display: flex;
    gap: 20rpx;

    .detail-row {
      display: flex;
      align-items: center;

      .label {
        font-size: 22rpx;
        color: $u-tips-color;
      }

      .value {
        font-size: 22rpx;
        color: $u-main-color;
      }
    }
  }

  .data-actions {
    display: flex;
    align-items: center;
    padding-left: 16rpx;
    gap: 12rpx;

    .action-btn {
      padding: 8rpx 16rpx;
      border-radius: $u-radius;
      font-size: 22rpx;
    }

    .action-edit {
      background: rgba($u-primary, 0.1);
      color: $u-primary;
    }

    .action-delete {
      background: rgba($u-error, 0.1);
      color: $u-error;
    }
  }
}

.loading-tip {
  text-align: center;
  padding: 30rpx 0;

  text {
    font-size: 26rpx;
    color: $u-tips-color;
  }
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 60rpx 0;

  .empty-icon {
    font-size: 60rpx;
  }

  .empty-text {
    font-size: 26rpx;
    color: $u-tips-color;
    margin-top: 16rpx;
  }
}

// 弹窗样式
.modal-mask {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-content {
  width: 90%;
  max-width: 600rpx;
  background: #fff;
  border-radius: $u-radius-lg;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 30rpx;
  border-bottom: 1rpx solid $u-border-color;

  .modal-title {
    font-size: 32rpx;
    font-weight: 600;
    color: $u-main-color;
  }

  .modal-close {
    font-size: 40rpx;
    color: $u-content-color;
    line-height: 1;
  }
}

.modal-body {
  padding: 30rpx;
}

.form-item {
  margin-bottom: 24rpx;

  &:last-child {
    margin-bottom: 0;
  }

  .form-label {
    font-size: 28rpx;
    color: $u-main-color;
    margin-bottom: 12rpx;
    display: block;
  }

  .form-input {
    width: 100%;
    height: 80rpx;
    padding: 0 20rpx;
    background: $u-light-color;
    border-radius: $u-radius;
    font-size: 28rpx;
    color: $u-main-color;
  }
}

.status-switch {
  display: flex;
  gap: 16rpx;

  .switch-item {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 80rpx;
    background: $u-light-color;
    border-radius: $u-radius;
    border: 2rpx solid transparent;

    text {
      font-size: 28rpx;
      color: $u-content-color;
    }

    &.active {
      background: rgba($u-primary, 0.1);
      border-color: $u-primary;

      text {
        color: $u-primary;
        font-weight: 600;
      }
    }
  }
}

.modal-footer {
  display: flex;
  padding: 30rpx;
  gap: 20rpx;
  border-top: 1rpx solid $u-border-color;

  .btn-cancel,
  .btn-confirm {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 88rpx;
    border-radius: $u-radius;
  }

  .btn-cancel {
    background: $u-light-color;

    text {
      font-size: 28rpx;
      color: $u-content-color;
    }
  }

  .btn-confirm {
    background: $u-primary;

    text {
      font-size: 28rpx;
      color: #fff;
    }
  }
}
</style>
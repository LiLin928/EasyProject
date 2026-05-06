<!-- src/views/basic/department/index.vue -->
<template>
  <div class="department-container">
    <el-row :gutter="20">
      <!-- 左侧：部门树 -->
      <el-col :span="5">
        <el-card shadow="never" class="tree-card">
          <template #header>
            <div class="tree-header">
              <span>{{ t('department.tree.title') }}</span>
              <el-button type="primary" size="small" @click="handleAddRoot">
                {{ t('department.tree.addRoot') }}
              </el-button>
            </div>
          </template>

          <el-input
            v-model="filterText"
            :placeholder="t('department.tree.searchPlaceholder')"
            clearable
            class="tree-search"
          />

          <el-tree
            ref="treeRef"
            :data="treeData"
            :props="{ children: 'children', label: 'name' }"
            node-key="id"
            highlight-current
            :expand-on-click-node="false"
            :filter-node-method="filterNode"
            default-expand-all
            @node-click="handleNodeClick"
          >
            <template #default="{ data }">
              <span class="tree-node">
                <span class="node-label">{{ data.name }}</span>
                <span class="node-count" v-if="data.memberCount > 0">
                  ({{ data.memberCount }})
                </span>
                <span class="tree-actions">
                  <el-button link size="small" type="primary" @click.stop="handleAdd(data)">
                    {{ t('department.tree.addChild') }}
                  </el-button>
                  <el-button link size="small" type="primary" @click.stop="handleEdit(data)">
                    {{ t('department.tree.edit') }}
                  </el-button>
                  <el-button
                    link
                    size="small"
                    type="danger"
                    :disabled="data.children && data.children.length > 0"
                    @click.stop="handleDelete(data)"
                  >
                    {{ t('department.tree.delete') }}
                  </el-button>
                </span>
              </span>
            </template>
          </el-tree>
        </el-card>
      </el-col>

      <!-- 右侧：部门详情/成员列表 -->
      <el-col :span="19">
        <el-card shadow="never" class="detail-card">
          <template #header>
            <div class="detail-header">
              <span>{{ currentDept?.name || t('department.detail.selectDept') }}</span>
              <el-button
                v-if="currentDept"
                type="primary"
                size="small"
                @click="handleEditDetail"
              >
                {{ t('department.tree.edit') }}
              </el-button>
            </div>
          </template>

          <!-- 部门详情 -->
          <div v-if="currentDept" class="dept-info">
            <el-descriptions :column="2" border>
              <el-descriptions-item :label="t('department.detail.name')">
                {{ currentDept.name }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('department.detail.code')">
                {{ currentDept.code || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('department.detail.path')">
                {{ currentDept.fullPath || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('department.detail.level')">
                {{ currentDept.level }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('department.detail.leader')">
                {{ currentDept.leaderName || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('department.detail.phone')">
                {{ currentDept.phone || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('department.detail.email')">
                {{ currentDept.email || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('department.detail.memberCount')">
                {{ currentDept.memberCount }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('department.detail.description')" :span="2">
                {{ currentDept.description || '-' }}
              </el-descriptions-item>
            </el-descriptions>
          </div>

          <!-- 成员列表 -->
          <div v-if="currentDept" class="member-section">
            <div class="section-header">
              <span>{{ t('department.member.title') }}</span>
            </div>

            <el-table :data="memberList" v-loading="memberLoading" border stripe>
              <el-table-column prop="userName" :label="t('department.member.userName')" width="120" />
              <el-table-column prop="realName" :label="t('department.member.realName')" width="120" />
              <el-table-column prop="phone" :label="t('department.member.phone')" width="150" />
              <el-table-column prop="email" :label="t('department.member.email')" width="200" />
              <el-table-column :label="t('department.member.status')" width="100" align="center">
                <template #default="{ row }">
                  <el-tag :type="row.status === 1 ? 'success' : 'danger'" size="small">
                    {{ row.status === 1 ? t('common.status.enabled') : t('common.status.disabled') }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column :label="t('department.member.roles')" min-width="150">
                <template #default="{ row }">
                  <el-tag
                    v-for="role in row.roles"
                    :key="role"
                    type="primary"
                    effect="plain"
                    size="small"
                    style="margin-right: 4px"
                  >
                    {{ role }}
                  </el-tag>
                  <span v-if="!row.roles?.length">-</span>
                </template>
              </el-table-column>
            </el-table>
          </div>

          <!-- 无数据提示 -->
          <el-empty v-if="!currentDept" :description="t('department.detail.selectDeptTip')" />
        </el-card>
      </el-col>
    </el-row>

    <!-- 部门表单弹窗 -->
    <DepartmentFormDialog
      v-model="dialogVisible"
      :department-id="currentDeptId"
      :parent-id="currentParentId"
      :tree-data="treeData"
      @success="handleDialogSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { ElTree } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { getDepartmentTree, getDepartmentUsers, deleteDepartment } from '@/api/basic/departmentApi'
import type { Department } from '@/types/department'
import type { UserInfo } from '@/types'
import DepartmentFormDialog from './components/DepartmentFormDialog.vue'

const { t } = useLocale()

// 树相关
const treeRef = ref<InstanceType<typeof ElTree>>()
const treeData = ref<Department[]>([])
const filterText = ref('')
const loading = ref(false)

// 当前选中的部门
const currentDept = ref<Department | null>(null)
const memberList = ref<UserInfo[]>([])
const memberLoading = ref(false)

// 弹窗相关
const dialogVisible = ref(false)
const currentDeptId = ref<string | undefined>(undefined)
const currentParentId = ref<string | null>(null)

// 保存后需要重新选中的部门ID
const refreshSelectDeptId = ref<string | null>(null)

// 过滤树节点
const filterNode = (value: string, data: any) => {
  if (!value) return true
  return data.name?.includes(value)
}

watch(filterText, (val) => {
  treeRef.value?.filter(val)
})

onMounted(() => {
  loadTree()
})

// 加载部门树
const loadTree = async () => {
  loading.value = true
  try {
    const data = await getDepartmentTree()
    treeData.value = data

    // 如果有需要重新选中的部门ID，刷新后重新选中
    if (refreshSelectDeptId.value) {
      // 使用 nextTick 确保 DOM 更新后再选中
      setTimeout(() => {
        treeRef.value?.setCurrentKey(refreshSelectDeptId.value)
        // 查找并设置当前部门详情
        const node = findDeptById(treeData.value, refreshSelectDeptId.value!)
        if (node) {
          currentDept.value = node
          // 重新加载成员列表
          loadMembers(node.id)
        }
        refreshSelectDeptId.value = null
      }, 100)
    }
  } catch (error) {
    // 错误已在拦截器处理
  } finally {
    loading.value = false
  }
}

// 根据ID在树中查找部门
const findDeptById = (list: Department[], id: string): Department | null => {
  for (const item of list) {
    if (item.id === id) return item
    if (item.children) {
      const found = findDeptById(item.children, id)
      if (found) return found
    }
  }
  return null
}

// 加载部门成员
const loadMembers = async (deptId: string) => {
  memberLoading.value = true
  try {
    const users = await getDepartmentUsers(deptId)
    memberList.value = users
  } catch (error) {
    memberList.value = []
  } finally {
    memberLoading.value = false
  }
}

// 点击树节点
const handleNodeClick = async (data: Department) => {
  currentDept.value = data
  loadMembers(data.id)
}

// 新增根部门
const handleAddRoot = () => {
  currentDeptId.value = undefined
  currentParentId.value = null
  dialogVisible.value = true
}

// 新增子部门
const handleAdd = (data: Department) => {
  currentDeptId.value = undefined
  currentParentId.value = data.id
  dialogVisible.value = true
}

// 编辑部门
const handleEdit = (data: Department) => {
  currentDeptId.value = data.id
  currentParentId.value = null
  refreshSelectDeptId.value = data.id
  dialogVisible.value = true
}

// 从右侧详情区编辑当前部门
const handleEditDetail = () => {
  if (currentDept.value) {
    currentDeptId.value = currentDept.value.id
    currentParentId.value = null
    refreshSelectDeptId.value = currentDept.value.id
    dialogVisible.value = true
  }
}

// 弹窗保存成功后的处理
const handleDialogSuccess = () => {
  loadTree()
}

// 删除部门
const handleDelete = async (data: Department) => {
  if (data.children && data.children.length > 0) {
    ElMessage.warning(t('department.message.hasChildren'))
    return
  }

  try {
    await ElMessageBox.confirm(
      t('department.message.deleteConfirm', { name: data.name }),
      t('common.message.warning'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteDepartment(data.id)
    ElMessage.success(t('department.message.deleteSuccess'))
    // 如果删除的是当前选中的部门，清空右侧
    if (currentDept.value?.id === data.id) {
      currentDept.value = null
      memberList.value = []
    }
    loadTree()
  } catch (error) {
    // 用户取消或请求失败
  }
}
</script>

<style scoped lang="scss">
.department-container {
  padding: 20px;
  height: calc(100vh - 100px);

  .el-row {
    height: 100%;
  }

  .tree-card {
    height: 100%;

    .tree-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .tree-search {
      margin-bottom: 12px;
    }

    .tree-node {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: space-between;
      font-size: 14px;
      padding-right: 8px;

      .node-label {
        flex: 1;
      }

      .node-count {
        color: #909399;
        font-size: 12px;
        margin-left: 4px;
      }

      .tree-actions {
        display: none;
      }
    }

    .el-tree-node__content:hover .tree-actions {
      display: inline-flex;
    }
  }

  .detail-card {
    height: 100%;

    .detail-header {
      font-size: 16px;
      font-weight: 500;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .dept-info {
      margin-bottom: 20px;
    }

    .member-section {
      .section-header {
        font-size: 14px;
        font-weight: 500;
        margin-bottom: 12px;
        padding-bottom: 8px;
        border-bottom: 1px solid #ebeef5;
      }
    }
  }
}
</style>
<!-- src/views/basic/role/components/RoleFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="650px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ isEdit ? t('role.role.editRole') : t('role.role.addRole') }}</span>
        <div class="dialog-actions">
          <el-button @click="visible = false">{{ t('common.button.cancel') }}</el-button>
          <el-button type="primary" :loading="loading" @click="handleSubmit">
            {{ t('common.button.ok') }}
          </el-button>
        </div>
      </div>
    </template>
    <el-tabs v-model="activeTab">
      <!-- Basic Info Tab -->
      <el-tab-pane :label="t('role.role.basicInfo')" name="basic">
        <el-form
          ref="formRef"
          :model="formData"
          :rules="formRules"
          label-width="100px"
        >
          <el-form-item :label="t('role.role.roleName')" prop="roleName">
            <el-input
              v-model="formData.roleName"
              :placeholder="t('role.role.roleNamePlaceholder')"
            />
          </el-form-item>
          <el-form-item :label="t('role.role.roleCode')" prop="roleCode">
            <el-input
              v-model="formData.roleCode"
              :placeholder="t('role.role.roleCodePlaceholder')"
              :disabled="isEdit"
            />
          </el-form-item>
          <el-form-item :label="t('role.role.description')" prop="description">
            <el-input
              v-model="formData.description"
              type="textarea"
              :rows="3"
              :placeholder="t('role.role.descriptionPlaceholder')"
              maxlength="100"
              show-word-limit
            />
          </el-form-item>
          <el-form-item :label="t('role.role.status')">
            <el-switch
              v-model="formData.status"
              :active-value="1"
              :inactive-value="0"
              :active-text="t('role.role.enabled')"
              :inactive-text="t('role.role.disabled')"
            />
          </el-form-item>
        </el-form>
      </el-tab-pane>

      <!-- Menu Permission Tab -->
      <el-tab-pane :label="t('role.role.menuPermission')" name="permission">
        <div class="permission-toolbar">
          <el-checkbox
            v-model="checkAll"
            :indeterminate="isIndeterminate"
            @change="handleCheckAll"
          >
            {{ t('role.role.all') }}
          </el-checkbox>
        </div>
        <el-tree
          ref="treeRef"
          :data="menuTree"
          :props="{ label: 'menuName', children: 'children' }"
          show-checkbox
          node-key="id"
          :default-checked-keys="formData.menuIds"
          :default-expand-all="true"
          @check="handleTreeCheck"
        />
      </el-tab-pane>
    </el-tabs>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules, type TreeInstance } from 'element-plus'
import { getRoleDetail, createRole, updateRole, getRoleMenuIds, updateRoleMenu } from '@/api/role'
import { getMenuList } from '@/api'
import { useLocale } from '@/composables/useLocale'
import type { RoleInfo } from '@/types/role'
import type { MockMenu } from '@/types/menu'
import { CommonStatus } from '@/types/enums'

const props = defineProps<{
  modelValue: boolean
  roleId?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const isEdit = computed(() => !!props.roleId)

const formRef = ref<FormInstance>()
const treeRef = ref<TreeInstance>()
const loading = ref(false)
const activeTab = ref('basic')
const menuTree = ref<MockMenu[]>([])
const checkAll = ref(false)
const isIndeterminate = ref(false)

const formData = reactive({
  roleName: '',
  roleCode: '',
  description: '',
  status: CommonStatus.ENABLED,
  menuIds: [] as string[],
})

const formRules: FormRules = {
  roleName: [
    { required: true, message: t('role.role.roleNameRequired'), trigger: 'blur' },
    { min: 2, max: 20, message: t('role.role.roleNameFormat'), trigger: 'blur' },
  ],
  roleCode: [
    { required: true, message: t('role.role.roleCodeRequired'), trigger: 'blur' },
    { pattern: /^[a-zA-Z0-9_]{2,20}$/, message: t('role.role.roleCodeFormat'), trigger: 'blur' },
  ],
}

const loadMenuTree = async () => {
  try {
    const data = await getMenuList()
    menuTree.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

const loadRoleDetail = async () => {
  if (!props.roleId) return
  try {
    const data = await getRoleDetail(props.roleId)
    formData.roleName = data.roleName
    formData.roleCode = data.roleCode || ''
    formData.description = data.description || ''
    formData.status = data.status ?? CommonStatus.ENABLED

    const menuIds = await getRoleMenuIds(props.roleId)
    formData.menuIds = menuIds
    treeRef.value?.setCheckedKeys(menuIds)
    updateCheckAllStatus()
  } catch (error) {
    // Error handled by interceptor
  }
}

watch(visible, (val) => {
  if (val) {
    activeTab.value = 'basic'
    loadMenuTree()
    if (props.roleId) {
      loadRoleDetail()
    } else {
      resetForm()
    }
  }
})

const resetForm = () => {
  formData.roleName = ''
  formData.roleCode = ''
  formData.description = ''
  formData.status = CommonStatus.ENABLED
  formData.menuIds = []
  checkAll.value = false
  isIndeterminate.value = false
  treeRef.value?.setCheckedKeys([])
}

const handleTreeCheck = () => {
  updateCheckAllStatus()
}

const updateCheckAllStatus = () => {
  const checkedKeys = treeRef.value?.getCheckedKeys(false) || []
  const allNodeCount = getAllNodeCount(menuTree.value)
  checkAll.value = checkedKeys.length === allNodeCount
  isIndeterminate.value = checkedKeys.length > 0 && checkedKeys.length < allNodeCount
}

const getAllNodeCount = (nodes: MockMenu[]): number => {
  let count = 0
  nodes.forEach(node => {
    count++
    if (node.children && node.children.length > 0) {
      count += getAllNodeCount(node.children)
    }
  })
  return count
}

const handleCheckAll = (val: boolean) => {
  if (val) {
    const allIds = getAllNodeIds(menuTree.value)
    treeRef.value?.setCheckedKeys(allIds)
  } else {
    treeRef.value?.setCheckedKeys([])
  }
  isIndeterminate.value = false
}

const getAllNodeIds = (nodes: MockMenu[]): string[] => {
  const ids: string[] = []
  nodes.forEach(node => {
    ids.push(node.id)
    if (node.children && node.children.length > 0) {
      ids.push(...getAllNodeIds(node.children))
    }
  })
  return ids
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  const checkedKeys = treeRef.value?.getCheckedKeys(false) || []
  const menuIds = checkedKeys as string[]

  loading.value = true
  try {
    if (isEdit.value) {
      // 更新角色信息
      await updateRole({
        id: props.roleId!,
        roleName: formData.roleName,
        description: formData.description || undefined,
        status: formData.status,
      })
      // 更新菜单权限
      await updateRoleMenu(props.roleId!, menuIds)
      ElMessage.success(t('role.role.updateRoleSuccess'))
    } else {
      // 创建角色
      const result = await createRole({
        roleName: formData.roleName,
        roleCode: formData.roleCode,
        description: formData.description || undefined,
      })
      // 更新菜单权限（新增后设置权限）
      if (result && menuIds.length > 0) {
        await updateRoleMenu(result, menuIds)
      }
      ElMessage.success(t('role.role.createRoleSuccess'))
    }
    visible.value = false
    emit('success')
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleClose = () => {
  formRef.value?.resetFields()
  resetForm()
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

.permission-toolbar {
  margin-bottom: 12px;
  padding-bottom: 12px;
  border-bottom: 1px solid var(--el-border-color-lighter);
}
</style>
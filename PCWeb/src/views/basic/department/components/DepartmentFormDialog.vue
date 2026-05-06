<!-- src/views/basic/department/components/DepartmentFormDialog.vue -->
<template>
  <el-dialog
    :model-value="modelValue"
    :title="isEdit ? t('department.form.editTitle') : t('department.form.addTitle')"
    width="500px"
    @update:model-value="emit('update:modelValue', $event)"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="100px"
    >
      <el-form-item :label="t('department.form.parent')" prop="parentId">
        <el-tree-select
          v-model="formData.parentId"
          :data="treeDataForSelect"
          :props="treeSelectProps"
          check-strictly
          clearable
          :placeholder="t('department.form.parentPlaceholder')"
          style="width: 100%"
        />
      </el-form-item>

      <el-form-item :label="t('department.form.name')" prop="name">
        <el-input
          v-model="formData.name"
          maxlength="100"
          show-word-limit
          :placeholder="t('department.form.namePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('department.form.code')" prop="code">
        <el-input
          v-model="formData.code"
          maxlength="50"
          show-word-limit
          :placeholder="t('department.form.codePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('department.form.manager')" prop="managerId">
        <el-select
          v-model="formData.managerId"
          clearable
          filterable
          :placeholder="t('department.form.managerPlaceholder')"
          style="width: 100%"
        >
          <el-option
            v-for="user in userList"
            :key="user.id"
            :label="user.realName || user.userName"
            :value="user.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item :label="t('department.form.phone')" prop="phone">
        <el-input
          v-model="formData.phone"
          maxlength="20"
          :placeholder="t('department.form.phonePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('department.form.email')" prop="email">
        <el-input
          v-model="formData.email"
          maxlength="100"
          :placeholder="t('department.form.emailPlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('department.form.description')" prop="description">
        <el-input
          v-model="formData.description"
          type="textarea"
          :rows="3"
          maxlength="500"
          show-word-limit
          :placeholder="t('department.form.descriptionPlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('department.form.sort')" prop="sort">
        <el-input-number v-model="formData.sort" :min="0" :max="999" />
      </el-form-item>

      <el-form-item :label="t('department.form.status')" prop="status">
        <el-radio-group v-model="formData.status">
          <el-radio :value="1">{{ t('common.status.enabled') }}</el-radio>
          <el-radio :value="0">{{ t('common.status.disabled') }}</el-radio>
        </el-radio-group>
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="emit('update:modelValue', false)">
        {{ t('common.button.cancel') }}
      </el-button>
      <el-button type="primary" :loading="saving" @click="handleSave">
        {{ t('common.button.save') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { getDepartmentDetail, createDepartment, updateDepartment } from '@/api/basic/departmentApi'
import { getUserList } from '@/api/user'
import type { Department, AddDepartmentParams, UpdateDepartmentParams } from '@/types/department'
import type { UserInfo } from '@/types'

const props = defineProps<{
  modelValue: boolean
  departmentId?: string
  parentId?: string | null
  treeData: Department[]
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

const formRef = ref<FormInstance | null>(null)
const saving = ref(false)
const userList = ref<UserInfo[]>([])

// 是否是编辑模式
const isEdit = computed(() => !!props.departmentId)

// 表单数据
const formData = reactive<AddDepartmentParams>({
  parentId: null,
  name: '',
  code: '',
  managerId: null,
  phone: '',
  email: '',
  description: '',
  sort: 0,
  status: 1,
})

// 表单验证规则
const formRules: FormRules = {
  name: [
    { required: true, message: t('department.form.nameRequired'), trigger: 'blur' },
    { min: 2, max: 100, message: t('department.form.nameLength'), trigger: 'blur' },
  ],
  email: [
    { type: 'email', message: t('department.form.emailFormat'), trigger: 'blur' },
  ],
}

// 为 TreeSelect 准备的数据（添加一个"无"选项）
const treeDataForSelect = computed(() => {
  return [
    { id: '', name: t('department.form.noParent'), children: [] },
    ...props.treeData,
  ]
})

// TreeSelect props 配置
const treeSelectProps = {
  children: 'children',
  label: 'name',
  value: 'id',
}

// 监听弹窗打开
watch(() => props.modelValue, async (val) => {
  if (val) {
    // 加载用户列表（用于选择主管）
    loadUserList()

    if (props.departmentId) {
      // 编辑模式：加载部门详情
      await loadDetail(props.departmentId)
    } else {
      // 新增模式：设置默认上级
      resetForm()
      formData.parentId = props.parentId || null
    }
  }
})

onMounted(() => {
  loadUserList()
})

// 加载用户列表
const loadUserList = async () => {
  try {
    const data = await getUserList({ pageIndex: 1, pageSize: 9999, status: 1 })
    userList.value = data.list
  } catch (error) {
    userList.value = []
  }
}

// 加载部门详情
const loadDetail = async (id: string) => {
  try {
    const dept = await getDepartmentDetail(id)
    if (dept) {
      formData.parentId = dept.parentId
      formData.name = dept.name
      formData.code = dept.code || ''
      formData.managerId = dept.managerId
      formData.phone = dept.phone || ''
      formData.email = dept.email || ''
      formData.description = dept.description || ''
      formData.sort = dept.sort
      formData.status = dept.status
    }
  } catch (error) {
    ElMessage.error(t('common.message.loadFailed'))
  }
}

// 重置表单
const resetForm = () => {
  formData.parentId = null
  formData.name = ''
  formData.code = ''
  formData.managerId = null
  formData.phone = ''
  formData.email = ''
  formData.description = ''
  formData.sort = 0
  formData.status = 1
}

// 关闭弹窗
const handleClose = () => {
  resetForm()
  formRef.value?.resetFields()
}

// 保存
const handleSave = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch {
    return
  }

  saving.value = true
  try {
    if (isEdit.value) {
      const params: UpdateDepartmentParams = {
        id: props.departmentId!,
        parentId: formData.parentId,
        name: formData.name,
        code: formData.code,
        managerId: formData.managerId,
        phone: formData.phone,
        email: formData.email,
        description: formData.description,
        sort: formData.sort ?? 0,
        status: formData.status ?? 1,
      }
      await updateDepartment(params)
      ElMessage.success(t('department.message.updateSuccess'))
    } else {
      await createDepartment(formData)
      ElMessage.success(t('department.message.addSuccess'))
    }
    emit('update:modelValue', false)
    emit('success')
  } catch (error) {
    ElMessage.error(t('common.message.saveFailed'))
  } finally {
    saving.value = false
  }
}
</script>
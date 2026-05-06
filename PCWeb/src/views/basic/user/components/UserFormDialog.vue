<!-- src/views/basic/user/components/UserFormDialog.vue -->
<template>
  <ModalForm
    ref="modalFormRef"
    v-model="visible"
    :title="isEdit ? t('user.user.editUser') : t('user.user.addUser')"
    :items="formItems"
    :form-data="formData"
    :mode="isEdit ? 'edit' : 'create'"
    :loading="loading"
    @submit="handleSubmit"
  >
    <!-- 角色选择自定义渲染 -->
    <template #roleIds="{ formData }">
      <el-select
        v-model="formData.roleIds"
        multiple
        :placeholder="t('user.user.selectRole')"
        style="width: 100%"
      >
        <el-option
          v-for="role in roleList"
          :key="role.id"
          :label="role.roleName"
          :value="role.id"
        />
      </el-select>
    </template>
  </ModalForm>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { getUserDetail, createUser, updateUser, getRoleList } from '@/api/user'
import { useLocale } from '@/composables/useLocale'
import type { RoleInfo } from '@/types/user'
import { CommonStatus } from '@/types/enums'
import ModalForm from '@/components/ModalForm/index.vue'
import type { ModalFormItem } from '@/components/ModalForm/types'

const props = defineProps<{
  modelValue: boolean
  userId?: string
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

const isEdit = computed(() => !!props.userId)

const loading = ref(false)
const roleList = ref<RoleInfo[]>([])

// ModalForm 组件引用
const modalFormRef = ref<InstanceType<typeof ModalForm> | null>(null)

const formData = reactive({
  userName: '',
  realName: '',
  password: '',
  confirmPassword: '',
  email: '',
  phone: '',
  roleIds: [] as string[],
  status: CommonStatus.ENABLED,
})

// 密码确认校验
const validateConfirmPassword = (rule: any, value: string, callback: any) => {
  if (value !== formData.password) {
    callback(new Error(t('user.user.passwordNotMatch')))
  } else {
    callback()
  }
}

// 监听密码变化，重新验证确认密码字段
watch(() => formData.password, () => {
  if (visible.value && formData.confirmPassword) {
    // 先清除验证状态，再重新验证
    modalFormRef.value?.clearValidate('confirmPassword')
    modalFormRef.value?.validateField('confirmPassword')
  }
})

// 表单配置
const formItems = computed<ModalFormItem[]>(() => [
  {
    field: 'userName',
    label: t('user.user.username'),
    type: 'input',
    rules: [
      { required: true, message: t('user.user.usernameRequired'), trigger: 'blur' },
      { pattern: /^[a-zA-Z0-9_]{3,20}$/, message: t('user.user.usernameFormat'), trigger: 'blur' },
    ],
    props: { disabledInEdit: true },
  },
  {
    field: 'realName',
    label: t('user.user.nickname'),
    type: 'input',
    rules: [
      { required: true, message: t('user.user.nicknameRequired'), trigger: 'blur' },
      { min: 2, max: 20, message: t('user.user.nicknameFormat'), trigger: 'blur' },
    ],
  },
  ...(!isEdit.value ? [
    {
      field: 'password',
      label: t('user.user.password'),
      type: 'input',
      rules: [
        { required: true, message: t('user.user.passwordRequired'), trigger: 'blur' },
        { min: 6, max: 20, message: t('user.user.passwordFormat'), trigger: 'blur' },
      ],
      props: { type: 'password', showPassword: true },
      hideInEdit: true,
    },
    {
      field: 'confirmPassword',
      label: t('user.user.confirmPassword'),
      type: 'input',
      rules: [
        { required: true, message: t('user.user.confirmPasswordRequired'), trigger: 'blur' },
        { validator: validateConfirmPassword, trigger: 'blur' },
      ],
      props: { type: 'password', showPassword: true },
      hideInEdit: true,
    },
  ] as ModalFormItem[] : []),
  {
    field: 'email',
    label: t('user.user.email'),
    type: 'input',
    rules: [{ type: 'email', message: t('common.validation.email'), trigger: 'blur' }],
  },
  {
    field: 'phone',
    label: t('user.user.phone'),
    type: 'input',
    rules: [{ pattern: /^1[3-9]\d{9}$/, message: t('common.validation.phone'), trigger: 'blur' }],
  },
  {
    field: 'roleIds',
    label: t('user.user.role'),
    type: 'slot',
  },
  {
    field: 'status',
    label: t('user.user.status'),
    type: 'switch',
    props: { activeValue: CommonStatus.ENABLED, inactiveValue: CommonStatus.DISABLED, activeText: t('user.user.enabled'), inactiveText: t('user.user.disabled') },
  },
])

// 加载角色列表
const loadRoleList = async () => {
  try {
    const data = await getRoleList()
    roleList.value = data.list
  } catch (error) {
    // 错误已由拦截器处理
  }
}

// 加载用户详情
const loadUserDetail = async () => {
  if (!props.userId) return
  try {
    const data = await getUserDetail(props.userId)
    formData.userName = data.userName
    formData.realName = data.realName || ''
    formData.email = data.email || ''
    formData.phone = data.phone || ''
    formData.roleIds = data.roleIds || []
    formData.status = data.status ?? CommonStatus.ENABLED
  } catch (error) {
    // 错误已由拦截器处理
  }
}

// 监听弹窗打开
watch(visible, (val) => {
  if (val) {
    loadRoleList()
    if (props.userId) {
      loadUserDetail()
    } else {
      resetForm()
    }
  }
})

const resetForm = () => {
  formData.userName = ''
  formData.realName = ''
  formData.password = ''
  formData.confirmPassword = ''
  formData.email = ''
  formData.phone = ''
  formData.roleIds = []
  formData.status = CommonStatus.ENABLED
}

const handleSubmit = async (data: Record<string, any>) => {
  loading.value = true
  try {
    if (isEdit.value) {
      await updateUser({
        id: props.userId!,
        realName: data.realName,
        email: data.email || undefined,
        phone: data.phone || undefined,
        roleIds: data.roleIds,
        status: data.status,
      })
      ElMessage.success(t('user.user.updateUserSuccess'))
    } else {
      await createUser({
        userName: data.userName,
        realName: data.realName,
        password: data.password,
        email: data.email || undefined,
        phone: data.phone || undefined,
        roleIds: data.roleIds,
        status: data.status,
      })
      ElMessage.success(t('user.user.createUserSuccess'))
    }
    visible.value = false
    emit('success')
  } catch (error) {
    // 错误已由拦截器处理
  } finally {
    loading.value = false
  }
}
</script>
<!-- src/views/basic/menu/components/MenuFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="550px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ isEdit ? t('menu.menuManagement.editMenu') : t('menu.menuManagement.addMenu') }}</span>
        <div class="dialog-actions">
          <el-button @click="visible = false">{{ t('common.button.cancel') }}</el-button>
          <el-button type="primary" :loading="loading" @click="handleSubmit">
            {{ t('common.button.ok') }}
          </el-button>
        </div>
      </div>
    </template>

    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="100px"
    >
      <!-- Parent Menu -->
      <el-form-item :label="t('menu.menuManagement.parentMenu')">
        <el-cascader
          v-model="formData.parentId"
          :options="parentMenuOptions"
          :props="{
            value: 'id',
            label: 'menuName',
            children: 'children',
            checkStrictly: true,
            emitPath: false,
          }"
          clearable
          :placeholder="t('menu.menuManagement.noParent')"
          style="width: 100%"
        />
      </el-form-item>

      <!-- Menu Name (显示标题) -->
      <el-form-item :label="t('menu.menuManagement.menuName')" prop="menuName">
        <el-input
          v-model="formData.menuName"
          :placeholder="t('menu.menuManagement.menuNamePlaceholder')"
        />
      </el-form-item>

      <!-- Menu Code (路由名称) -->
      <el-form-item label="路由名称" prop="menuCode">
        <el-input
          v-model="formData.menuCode"
          placeholder="路由 name 属性，如 'User'"
        />
      </el-form-item>

      <!-- Menu Path -->
      <el-form-item :label="t('menu.menuManagement.menuPath')" prop="path">
        <el-input
          v-model="formData.path"
          :placeholder="t('menu.menuManagement.menuPathPlaceholder')"
        />
      </el-form-item>

      <!-- Component Path -->
      <el-form-item :label="t('menu.menuManagement.componentPath')">
        <el-input
          v-model="formData.component"
          :placeholder="t('menu.menuManagement.componentPathPlaceholder')"
        />
      </el-form-item>

      <!-- Icon -->
      <el-form-item :label="t('menu.menuManagement.icon')">
        <el-input
          v-model="formData.icon"
          :placeholder="t('menu.menuManagement.selectIcon')"
        >
          <template #append>
            <el-button @click="iconDialogVisible = true">
              <el-icon><EditPen /></el-icon>
            </el-button>
          </template>
        </el-input>
      </el-form-item>

      <!-- Sort -->
      <el-form-item :label="t('menu.menuManagement.sort')">
        <el-input-number
          v-model="formData.sort"
          :min="0"
          :max="999"
          :placeholder="t('menu.menuManagement.sortPlaceholder')"
          style="width: 100%"
        />
      </el-form-item>

      <!-- Status -->
      <el-form-item :label="t('menu.menuManagement.status')">
        <el-switch
          v-model="formData.status"
          :active-value="1"
          :inactive-value="0"
          :active-text="t('menu.menuManagement.enabled')"
          :inactive-text="t('menu.menuManagement.disabled')"
        />
      </el-form-item>

      <!-- Visible -->
      <el-form-item :label="t('menu.menuManagement.visible')">
        <el-switch
          v-model="formData.hidden"
          :active-value="0"
          :inactive-value="1"
          :active-text="t('menu.menuManagement.shown')"
          :inactive-text="t('menu.menuManagement.hidden')"
        />
      </el-form-item>

      <!-- Affix (固定标签页) -->
      <el-form-item label="固定标签页">
        <el-switch
          v-model="formData.affix"
          :active-value="1"
          :inactive-value="0"
          active-text="固定"
          inactive-text="可关闭"
        />
      </el-form-item>
    </el-form>

    <!-- Icon Selector Dialog -->
    <IconSelectDialog
      v-model="iconDialogVisible"
      :current-icon="formData.icon"
      @select="handleIconSelect"
    />
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { EditPen } from '@element-plus/icons-vue'
import { getMenuTree, getMenuDetail, createMenu, updateMenu } from '@/api/menu'
import { useLocale } from '@/composables/useLocale'
import type { MockMenu, CreateMenuParams, UpdateMenuParams } from '@/types/menu'
import { CommonStatus, MenuVisibility } from '@/types/enums'
import IconSelectDialog from './IconSelectDialog.vue'

const props = defineProps<{
  modelValue: boolean
  menuId?: string
  parentId?: string | null
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

const isEdit = computed(() => !!props.menuId)

const formRef = ref<FormInstance>()
const loading = ref(false)
const iconDialogVisible = ref(false)
const menuTree = ref<MockMenu[]>([])

const formData = reactive({
  parentId: null as string | null,
  menuName: '',
  menuCode: '' as string | null,
  path: '',
  component: '' as string | null,
  icon: '' as string | null,
  sort: 0,
  status: CommonStatus.ENABLED,
  hidden: MenuVisibility.VISIBLE,
  affix: 0,
})

const formRules: FormRules = {
  menuName: [
    { required: true, message: t('menu.menuManagement.menuNameRequired'), trigger: 'blur' },
    { min: 2, max: 50, message: t('menu.menuManagement.menuNameFormat'), trigger: 'blur' },
  ],
  path: [
    { required: true, message: t('menu.menuManagement.menuPathRequired'), trigger: 'blur' },
    { pattern: /^\//, message: t('menu.menuManagement.menuPathFormat'), trigger: 'blur' },
  ],
}

// Build parent menu options (exclude current menu when editing)
const parentMenuOptions = computed(() => {
  if (!isEdit.value) return menuTree.value
  // Filter out current menu to prevent circular reference
  return filterMenuTree(menuTree.value, props.menuId!)
})

const filterMenuTree = (menus: MockMenu[], excludeId: string): MockMenu[] => {
  return menus
    .filter(menu => menu.id !== excludeId)
    .map(menu => ({
      ...menu,
      children: menu.children ? filterMenuTree(menu.children, excludeId) : undefined,
    }))
}

const loadMenuTree = async () => {
  try {
    const data = await getMenuTree()
    menuTree.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

const loadMenuDetail = async () => {
  if (!props.menuId) return
  try {
    const data = await getMenuDetail(props.menuId)
    formData.parentId = data.parentId
    formData.menuName = data.menuName
    formData.menuCode = data.menuCode
    formData.path = data.path
    formData.component = data.component
    formData.icon = data.icon
    formData.sort = data.sort
    formData.status = data.status
    formData.hidden = data.hidden
    formData.affix = data.affix
  } catch (error) {
    // Error handled by interceptor
  }
}

watch(visible, (val) => {
  if (val) {
    loadMenuTree()
    if (props.menuId) {
      loadMenuDetail()
    } else {
      resetForm()
      if (props.parentId) {
        formData.parentId = props.parentId
      }
    }
  }
})

const resetForm = () => {
  formData.parentId = null
  formData.menuName = ''
  formData.menuCode = null
  formData.path = ''
  formData.component = null
  formData.icon = null
  formData.sort = 0
  formData.status = CommonStatus.ENABLED
  formData.hidden = MenuVisibility.VISIBLE
  formData.affix = 0
}

const handleIconSelect = (icon: string) => {
  formData.icon = icon
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    const params: CreateMenuParams | UpdateMenuParams = {
      ...(isEdit.value ? { id: props.menuId! } : {}),
      parentId: formData.parentId,
      menuName: formData.menuName,
      menuCode: formData.menuCode,
      path: formData.path,
      component: formData.component || null,
      icon: formData.icon,
      sort: formData.sort,
      status: formData.status,
      hidden: formData.hidden,
      affix: formData.affix,
    }

    if (isEdit.value) {
      await updateMenu(params as UpdateMenuParams)
      ElMessage.success(t('menu.menuManagement.updateMenuSuccess'))
    } else {
      await createMenu(params as CreateMenuParams)
      ElMessage.success(t('menu.menuManagement.createMenuSuccess'))
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
</style>
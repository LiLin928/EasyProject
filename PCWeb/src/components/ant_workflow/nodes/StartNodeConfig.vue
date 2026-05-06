<!-- src/components/ant_workflow/nodes/StartNodeConfig.vue -->
<template>
  <div class="start-node-config">
    <el-form :model="localConfig" label-width="80px">
      <el-form-item label="发起权限">
        <el-select
          v-model="localConfig.permissions"
          multiple
          filterable
          :placeholder="t('antWorkflow.nodeConfig.startConfig.flowPermissionPlaceholder')"
          style="width: 100%"
        >
          <el-option-group label="用户">
            <el-option
              v-for="user in userOptions"
              :key="user.id"
              :label="user.name"
              :value="{ targetId: user.id, name: user.name, type: 1 }"
            />
          </el-option-group>
          <el-option-group label="角色">
            <el-option
              v-for="role in roleOptions"
              :key="role.id"
              :label="role.name"
              :value="{ targetId: role.id, name: role.name, type: 2 }"
            />
          </el-option-group>
        </el-select>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useLocale } from '@/composables/useLocale'
import type { FlowPermission } from '@/types/antWorkflow'

const { t } = useLocale()

interface StartConfig {
  permissions?: FlowPermission[]
}

const props = defineProps<{ config: StartConfig }>()
const emit = defineEmits<{ (e: 'update', config: StartConfig): void }>()

const userOptions = ref([
  { id: 'user-001', name: '管理员' },
  { id: 'user-002', name: '张三' },
  { id: 'user-003', name: '李四' },
])

const roleOptions = ref([
  { id: 'role-001', name: '系统管理员' },
  { id: 'role-002', name: '部门经理' },
  { id: 'role-003', name: '普通员工' },
])

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

const localConfig = ref<StartConfig>({ permissions: [] })

// 监听 props.config，更新 localConfig（防止循环）
watch(() => props.config, (newConfig) => {
  if (newConfig) {
    isUpdatingFromProps.value = true
    localConfig.value = { permissions: newConfig.permissions || [] }
    setTimeout(() => { isUpdatingFromProps.value = false }, 0)
  }
}, { immediate: true })

// 监听 localConfig，emit update（防止循环）
watch(localConfig, (newConfig) => {
  if (!isUpdatingFromProps.value) {
    emit('update', newConfig)
  }
}, { deep: true })
</script>

<style scoped lang="scss">.start-node-config { padding: 0; }</style>
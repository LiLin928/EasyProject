<!-- 文件: PCWeb/src/views/basic/desktop/index.vue -->
<template>
  <div class="desktop-config-container">
    <el-tabs v-model="activeTab" type="card">
      <el-tab-pane label="组件管理" name="widget">
        <WidgetList />
      </el-tab-pane>
      <el-tab-pane label="角色分配" name="role-config">
        <RoleConfig />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import WidgetList from './widget/index.vue'
import RoleConfig from './role-config/index.vue'

const activeTab = ref('widget')
</script>

<style scoped lang="scss">
.desktop-config-container {
  padding: 20px;
}
</style>
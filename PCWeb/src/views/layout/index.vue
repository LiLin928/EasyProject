<!-- 主布局页面 -->
<template>
  <el-container class="layout-container" direction="vertical">
    <!-- 顶部导航栏（全宽） -->
    <el-header class="layout-header">
      <Navbar v-model:collapsed="sidebarCollapsed" />
    </el-header>

    <!-- 下方区域：侧边栏 + 内容 -->
    <el-container class="layout-body">
      <!-- 侧边栏 -->
      <el-aside :width="sidebarWidth" class="layout-aside">
        <Sidebar v-model:collapsed="sidebarCollapsed" />
      </el-aside>

      <!-- 右侧内容 -->
      <el-container class="layout-main">
        <!-- TagsView 标签页导航 -->
        <TagsView />

        <!-- 主内容区 -->
        <el-main class="layout-content">
          <AppMain />
        </el-main>
      </el-container>
    </el-container>
  </el-container>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAppStore } from '@/stores/app'
import Navbar from '@/components/layout/Navbar.vue'
import Sidebar from '@/components/layout/Sidebar.vue'
import TagsView from '@/components/TagsView/index.vue'
import AppMain from '@/components/layout/AppMain.vue'

const appStore = useAppStore()

// 侧边栏折叠状态
const sidebarCollapsed = computed({
  get: () => appStore.sidebarCollapsed,
  set: (val: boolean) => appStore.setSidebarCollapsed(val),
})

// 侧边栏宽度
const sidebarWidth = computed(() => (sidebarCollapsed.value ? '64px' : '200px'))
</script>

<style scoped lang="scss">
.layout-container {
  width: 100%;
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.layout-header {
  height: 60px;
  padding: 0;
  z-index: 10;
  width: 100%;
}

.layout-body {
  flex: 1;
  height: calc(100vh - 60px);
  overflow: hidden;
}

.layout-aside {
  background-color: #fff;
  box-shadow: 2px 0 8px rgba(0, 0, 0, 0.05);
  transition: width 0.3s;
  overflow: hidden;
  height: 100%;
}

.layout-main {
  display: flex;
  flex-direction: column;
  height: 100%;
  overflow: hidden;
}

.layout-content {
  flex: 1;
  padding: 0;
  overflow: hidden;
  background-color: #f5f7fa;
}
</style>
<!-- 侧边菜单栏 -->
<template>
  <div class="sidebar" :class="{ 'is-collapsed': collapsed }">
    <el-scrollbar>
      <el-menu
        :default-active="activeMenu"
        :collapse="collapsed"
        :unique-opened="true"
        :collapse-transition="false"
        router
        class="sidebar-menu"
      >
        <SidebarMenuItem
          v-for="item in menuList"
          :key="item.path"
          :item="item"
          :collapsed="collapsed"
        />
      </el-menu>
    </el-scrollbar>

    <!-- 折叠按钮 -->
    <div class="collapse-btn" @click="toggleCollapse">
      <el-icon>
        <Fold v-if="!collapsed" />
        <Expand v-else />
      </el-icon>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { Fold, Expand } from '@element-plus/icons-vue'
import { usePermissionStore } from '@/stores/permission'
import type { MockMenu } from '@/types/menu'
import SidebarMenuItem from './SidebarMenuItem.vue'

const collapsed = defineModel<boolean>('collapsed', { default: false })

const route = useRoute()
const permissionStore = usePermissionStore()

// 当前激活菜单
const activeMenu = computed(() => route.path)

// 递归过滤菜单（过滤 hidden 为 1 和 status 不为 1 的菜单）
const filterMenus = (menus: MockMenu[]): MockMenu[] => {
  return menus
    .filter((menu) => menu.hidden !== 1 && menu.status === 1)
    .map((menu) => ({
      ...menu,
      children: menu.children ? filterMenus(menu.children) : undefined,
    }))
}

// 菜单列表（从 permissionStore 获取）
const menuList = computed(() => filterMenus(permissionStore.menus))

// 切换折叠
const toggleCollapse = () => {
  collapsed.value = !collapsed.value
}
</script>

<style scoped lang="scss">
.sidebar {
  display: flex;
  flex-direction: column;
  height: 100%;
  background-color: #fff;
  transition: width 0.3s;

  &.is-collapsed {
    :deep(.el-menu) {
      width: 64px;
    }
  }
}

.sidebar-menu {
  border-right: none;
  flex: 1;
}

:deep(.el-menu) {
  width: 200px;
  border-right: none;
}

:deep(.el-menu--collapse) {
  width: 64px;
}

:deep(.el-menu-item),
:deep(.el-sub-menu__title) {
  height: 50px;
  line-height: 50px;
}

:deep(.el-menu-item.is-active) {
  background-color: var(--el-color-primary-light-9);
}

.collapse-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 50px;
  cursor: pointer;
  border-top: 1px solid #e4e7ed;
  transition: background-color 0.3s;

  &:hover {
    background-color: #f5f7fa;
  }

  .el-icon {
    font-size: 18px;
    color: #606266;
  }
}
</style>
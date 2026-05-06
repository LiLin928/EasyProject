<!-- 递归菜单项组件 -->
<template>
  <!-- 有子菜单 -->
  <el-sub-menu v-if="item.children && item.children.length > 0" :index="item.path || ''">
    <template #title>
      <el-icon v-if="item.icon">
        <component :is="item.icon" />
      </el-icon>
      <span>{{ item.menuName }}</span>
    </template>
    <!-- 递归渲染子菜单 -->
    <SidebarMenuItem
      v-for="child in item.children"
      :key="child.path"
      :item="child"
      :parent-path="item.path"
      :collapsed="collapsed"
    />
  </el-sub-menu>
  <!-- 无子菜单 -->
  <el-menu-item v-else :index="resolvedPath">
    <el-icon v-if="item.icon">
      <component :is="item.icon" />
    </el-icon>
    <span>{{ item.menuName }}</span>
  </el-menu-item>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { MockMenu } from '@/types/menu'

const props = defineProps<{
  item: MockMenu
  parentPath?: string | null
  collapsed?: boolean
}>()

// 解析完整路径
const resolvedPath = computed(() => {
  const path = props.item.path || ''
  const parentPath = props.parentPath

  if (!parentPath) {
    return path
  }

  // 如果已经是完整路径，直接返回
  if (path.startsWith('/')) {
    return path
  }

  // 拼接父路径
  return parentPath.endsWith('/') ? `${parentPath}${path}` : `${parentPath}/${path}`
})
</script>
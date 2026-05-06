<!-- 面包屑导航 -->
<template>
  <el-breadcrumb class="breadcrumb" separator="/">
    <el-breadcrumb-item v-for="item in breadcrumbs" :key="item.path">
      <span v-if="item.redirect === 'noRedirect' || item === breadcrumbs[breadcrumbs.length - 1]">
        {{ item.meta?.title }}
      </span>
      <a v-else @click.prevent="handleLink(item)">
        {{ item.meta?.title }}
      </a>
    </el-breadcrumb-item>
  </el-breadcrumb>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute, useRouter, type RouteLocationMatched } from 'vue-router'

const route = useRoute()
const router = useRouter()

const breadcrumbs = ref<RouteLocationMatched[]>([])

// 获取面包屑数据
const getBreadcrumbs = () => {
  const matched = route.matched.filter(
    (item) => item.meta && item.meta.title && !item.meta.hidden
  )
  breadcrumbs.value = matched
}

// 点击链接跳转
const handleLink = (item: RouteLocationMatched) => {
  const { redirect, path } = item
  if (redirect) {
    router.push(redirect as string)
    return
  }
  router.push(path)
}

// 监听路由变化
watch(
  () => route.path,
  () => {
    getBreadcrumbs()
  },
  { immediate: true }
)
</script>

<style scoped lang="scss">
.breadcrumb {
  display: flex;
  align-items: center;
  height: 30px;
  padding: 0 10px;
  font-size: 14px;

  a {
    color: #606266;
    text-decoration: none;
    cursor: pointer;

    &:hover {
      color: var(--el-color-primary);
    }
  }
}

:deep(.el-breadcrumb__item:last-child .el-breadcrumb__inner) {
  color: #303133;
  font-weight: normal;
}
</style>
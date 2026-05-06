<!-- 主内容区域 -->
<template>
  <section class="app-main">
    <router-view v-slot="{ Component, route }">
      <transition name="fade-transform" mode="out-in">
        <keep-alive :include="cachedNames">
          <component :is="Component" :key="route.path" />
        </keep-alive>
      </transition>
    </router-view>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useTagsViewStore } from '@/stores/tagsView'

const tagsViewStore = useTagsViewStore()

// 使用 TagsView store 的缓存列表
const cachedNames = computed(() => tagsViewStore.cachedNames)
</script>

<style scoped lang="scss">
.app-main {
  position: relative;
  height: calc(100vh - 60px - 34px); // 减去导航栏和 TagsView 高度
  overflow: auto;
  background-color: #f5f7fa;
}

// 过渡动画
.fade-transform-enter-active,
.fade-transform-leave-active {
  transition: all 0.3s;
}

.fade-transform-enter-from {
  opacity: 0;
  transform: translateX(-30px);
}

.fade-transform-leave-to {
  opacity: 0;
  transform: translateX(30px);
}
</style>
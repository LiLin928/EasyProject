<!-- src/components/TagsView/index.vue -->
<template>
  <div class="tags-view-container" ref="containerRef">
    <div class="tags-view-wrapper" ref="scrollContainerRef" @wheel.prevent="handleScroll">
      <router-link
        v-for="tag in visitedTags"
        :key="tag.path"
        :to="{ path: tag.path, query: tag.query }"
        class="tags-view-item"
        :class="{ active: isActive(tag), affix: tag.affix }"
        @contextmenu.prevent="openContextMenu(tag, $event)"
      >
        <el-icon v-if="tag.icon" class="tag-icon">
          <component :is="tag.icon" />
        </el-icon>
        <span class="tag-title">{{ tag.title }}</span>
        <el-icon
          v-if="!tag.affix"
          class="close-icon"
          @click.prevent.stop="closeTag(tag)"
        >
          <Close />
        </el-icon>
      </router-link>
    </div>

    <!-- 右键菜单 -->
    <ContextMenu
      v-model:visible="contextMenuVisible"
      :x="contextMenuX"
      :y="contextMenuY"
      :tag="selectedTag"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, nextTick } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Close } from '@element-plus/icons-vue'
import { useTagsViewStore } from '@/stores/tagsView'
import type { TagView } from '@/types/tagsView'
import ContextMenu from './ContextMenu.vue'

const route = useRoute()
const router = useRouter()
const tagsViewStore = useTagsViewStore()

const visitedTags = computed(() => tagsViewStore.visitedTags)

const containerRef = ref<HTMLElement>()
const scrollContainerRef = ref<HTMLElement>()

// 右键菜单状态
const contextMenuVisible = ref(false)
const contextMenuX = ref(0)
const contextMenuY = ref(0)
const selectedTag = ref<TagView>()

// 判断是否激活
function isActive(tag: TagView) {
  return route.path === tag.path
}

// 关闭标签
function closeTag(tag: TagView) {
  const tags = tagsViewStore.closeTag(tag.path)
  // 如果关闭的是当前标签，跳转到最后一个标签
  if (isActive(tag)) {
    const lastTag = tags[tags.length - 1]
    if (lastTag) {
      router.push(lastTag.path)
    } else {
      router.push('/')
    }
  }
}

// 打开右键菜单
function openContextMenu(tag: TagView, e: MouseEvent) {
  selectedTag.value = tag
  contextMenuX.value = e.clientX
  contextMenuY.value = e.clientY
  contextMenuVisible.value = true
}

// 滚动处理
function handleScroll(e: WheelEvent) {
  const scrollContainer = scrollContainerRef.value
  if (!scrollContainer) return

  const scrollAmount = e.deltaY > 0 ? 100 : -100
  scrollContainer.scrollLeft += scrollAmount
}

// 滚动到当前标签
function scrollToCurrentTag() {
  nextTick(() => {
    const scrollContainer = scrollContainerRef.value
    if (!scrollContainer) return

    const activeTag = scrollContainer.querySelector('.tags-view-item.active') as HTMLElement
    if (!activeTag) return

    const containerWidth = scrollContainer.offsetWidth
    const tagLeft = activeTag.offsetLeft
    const tagWidth = activeTag.offsetWidth

    // 标签在可视区域左侧外
    if (tagLeft < scrollContainer.scrollLeft) {
      scrollContainer.scrollLeft = tagLeft
    }
    // 标签在可视区域右侧外
    else if (tagLeft + tagWidth > scrollContainer.scrollLeft + containerWidth) {
      scrollContainer.scrollLeft = tagLeft + tagWidth - containerWidth
    }
  })
}

// 监听路由变化
watch(
  () => route.path,
  () => {
    scrollToCurrentTag()
  }
)

onMounted(() => {
  scrollToCurrentTag()
})
</script>

<style scoped lang="scss">
.tags-view-container {
  height: 34px;
  background: #fff;
  border-bottom: 1px solid #d8dce5;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.12), 0 0 3px 0 rgba(0, 0, 0, 0.04);

  .tags-view-wrapper {
    display: flex;
    align-items: center;
    height: 100%;
    padding: 0 10px;
    overflow-x: auto;
    white-space: nowrap;

    &::-webkit-scrollbar {
      height: 0;
    }
  }

  .tags-view-item {
    display: inline-flex;
    align-items: center;
    height: 26px;
    padding: 0 10px;
    margin-right: 5px;
    font-size: 12px;
    color: #495060;
    background: #fff;
    border: 1px solid #d8dce5;
    border-radius: 3px;
    text-decoration: none;
    cursor: pointer;
    transition: all 0.3s;

    &:hover {
      color: #409eff;
    }

    &.active {
      color: #fff;
      background-color: #409eff;
      border-color: #409eff;

      .close-icon:hover {
        background-color: rgba(255, 255, 255, 0.3);
      }
    }

    &.affix {
      .tag-title {
        padding-left: 8px;
        border-left: 3px solid #409eff;
      }
    }

    .tag-icon {
      margin-right: 4px;
    }

    .tag-title {
      max-width: 120px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .close-icon {
      margin-left: 5px;
      border-radius: 50%;
      font-size: 12px;

      &:hover {
        background-color: rgba(0, 0, 0, 0.1);
      }
    }
  }
}
</style>
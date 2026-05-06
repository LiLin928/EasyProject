<!-- src/components/TagsView/ContextMenu.vue -->
<template>
  <teleport to="body">
    <div
      v-show="visible"
      class="context-menu"
      :style="{ left: x + 'px', top: y + 'px' }"
    >
      <ul class="context-menu-list">
        <li class="context-menu-item" @click="handleRefresh">
          <el-icon><Refresh /></el-icon>
          <span>{{ t('tagsView.refresh') }}</span>
        </li>
        <li
          class="context-menu-item"
          @click="handleToggleAffix"
        >
          <el-icon><StarFilled /></el-icon>
          <span>{{ tag?.affix ? t('tagsView.unpin') : t('tagsView.pin') }}</span>
        </li>
        <li
          v-if="!tag?.affix"
          class="context-menu-item"
          @click="handleClose"
        >
          <el-icon><Close /></el-icon>
          <span>{{ t('tagsView.close') }}</span>
        </li>
        <el-divider />
        <li class="context-menu-item" @click="handleCloseOther">
          <el-icon><FolderRemove /></el-icon>
          <span>{{ t('tagsView.closeOther') }}</span>
        </li>
        <li class="context-menu-item" @click="handleCloseRight">
          <el-icon><Right /></el-icon>
          <span>{{ t('tagsView.closeRight') }}</span>
        </li>
        <li class="context-menu-item" @click="handleCloseAll">
          <el-icon><CircleClose /></el-icon>
          <span>{{ t('tagsView.closeAll') }}</span>
        </li>
      </ul>
    </div>
  </teleport>
</template>

<script setup lang="ts">
import { watch, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { Refresh, Close, CircleClose, StarFilled, FolderRemove, Right } from '@element-plus/icons-vue'
import { useTagsViewStore } from '@/stores/tagsView'
import type { TagView } from '@/types/tagsView'
import { useI18n } from 'vue-i18n'

const props = defineProps<{
  visible: boolean
  x: number
  y: number
  tag?: TagView
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

const router = useRouter()
const tagsViewStore = useTagsViewStore()
const { t } = useI18n()

// Click elsewhere to close menu
watch(() => props.visible, (val) => {
  if (val) {
    document.addEventListener('click', closeMenu)
  } else {
    document.removeEventListener('click', closeMenu)
  }
})

// Cleanup on unmount
onUnmounted(() => {
  document.removeEventListener('click', closeMenu)
})

function closeMenu() {
  emit('update:visible', false)
}

function handleRefresh() {
  closeMenu()
  router.replace({ path: '/redirect' + props.tag?.path })
}

function handleToggleAffix() {
  closeMenu()
  if (props.tag) {
    tagsViewStore.toggleAffix(props.tag.path)
  }
}

function handleClose() {
  closeMenu()
  if (props.tag) {
    const tags = tagsViewStore.closeTag(props.tag.path)
    // If closing current tag, navigate to the last tag
    if (router.currentRoute.value.path === props.tag.path) {
      const lastTag = tags[tags.length - 1]
      if (lastTag) {
        router.push(lastTag.path)
      } else {
        router.push('/')
      }
    }
  }
}

function handleCloseOther() {
  closeMenu()
  if (props.tag) {
    tagsViewStore.closeOtherTags(props.tag.path)
  }
}

function handleCloseRight() {
  closeMenu()
  if (props.tag) {
    tagsViewStore.closeRightTags(props.tag.path)
  }
}

function handleCloseAll() {
  closeMenu()
  tagsViewStore.closeAllTags()
  // Navigate to home
  router.push('/')
}
</script>

<style scoped lang="scss">
.context-menu {
  position: fixed;
  z-index: 3000;
  padding: 5px 0;
  background: #fff;
  border-radius: 4px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.context-menu-list {
  margin: 0;
  padding: 0;
  list-style: none;
}

.context-menu-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  font-size: 14px;
  color: #606266;
  cursor: pointer;

  &:hover {
    background-color: #ecf5ff;
    color: #409eff;
  }
}

.el-divider {
  margin: 5px 0;
}
</style>
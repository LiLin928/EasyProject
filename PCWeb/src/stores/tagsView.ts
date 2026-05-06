// src/stores/tagsView.ts

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { RouteRecordRaw } from 'vue-router'
import type { TagView } from '@/types/tagsView'

const STORAGE_KEY = 'TAGS_VIEW'

/**
 * 从 localStorage 恢复标签
 */
function loadFromStorage(): TagView[] {
  try {
    const data = localStorage.getItem(STORAGE_KEY)
    return data ? JSON.parse(data) : []
  } catch {
    return []
  }
}

export const useTagsViewStore = defineStore('tagsView', () => {
  // 标签列表
  const visitedTags = ref<TagView[]>(loadFromStorage())

  // 缓存的组件名称（用于 keep-alive）
  const cachedNames = computed(() => {
    return visitedTags.value
      .filter(tag => tag.name && !tag.affix)
      .map(tag => tag.name)
  })

  /**
   * 保存标签到 localStorage
   * 只保存非固定的标签
   */
  function saveToStorage() {
    try {
      const tagsToSave = visitedTags.value.filter(t => !t.affix)
      localStorage.setItem(STORAGE_KEY, JSON.stringify(tagsToSave))
    } catch (e) {
      console.error('Failed to save tagsView to localStorage:', e)
    }
  }

  /**
   * 初始化固定标签
   * @param routes - 路由配置
   */
  function initAffixTags(routes: RouteRecordRaw[]) {
    routes.forEach(route => {
      if (route.meta?.affix && route.name) {
        const tag: TagView = {
          path: route.path,
          name: route.name as string,
          title: route.meta.title as string || 'no-name',
          icon: route.meta.icon as string,
          affix: true,
        }
        // 避免重复添加
        if (!visitedTags.value.some(t => t.path === tag.path)) {
          visitedTags.value.push(tag)
        }
      }
      if (route.children) {
        initAffixTags(route.children)
      }
    })
  }

  /**
   * 添加标签
   * @param tag - 标签信息
   */
  function addTag(tag: TagView) {
    // 已存在则不重复添加
    if (visitedTags.value.some(t => t.path === tag.path)) return
    visitedTags.value.push(tag)
    saveToStorage()
  }

  /**
   * 关闭标签
   * @param path - 标签路径
   * @returns 更新后的标签列表
   */
  function closeTag(path: string) {
    const index = visitedTags.value.findIndex(t => t.path === path)
    if (index > -1 && !visitedTags.value[index].affix) {
      visitedTags.value.splice(index, 1)
      saveToStorage()
    }
    return visitedTags.value
  }

  /**
   * 关闭其他标签
   * @param path - 保留的标签路径
   */
  function closeOtherTags(path: string) {
    visitedTags.value = visitedTags.value.filter(t => t.path === path || t.affix)
    saveToStorage()
  }

  /**
   * 关闭右侧标签
   * @param path - 当前标签路径
   */
  function closeRightTags(path: string) {
    const index = visitedTags.value.findIndex(t => t.path === path)
    if (index > -1) {
      // 保留当前标签及左侧标签，同时保留所有固定标签
      visitedTags.value = visitedTags.value.filter((t, i) => i <= index || t.affix)
      saveToStorage()
    }
  }

  /**
   * 关闭所有标签
   */
  function closeAllTags() {
    visitedTags.value = visitedTags.value.filter(t => t.affix)
    saveToStorage()
  }

  /**
   * 更新标签标题
   * @param path - 标签路径
   * @param title - 新标题
   */
  function updateTagTitle(path: string, title: string) {
    const tag = visitedTags.value.find(t => t.path === path)
    if (tag) tag.title = title
  }

  /**
   * 固定/取消固定
   * @param path - 标签路径
   */
  function toggleAffix(path: string) {
    const tag = visitedTags.value.find(t => t.path === path)
    if (tag) {
      tag.affix = !tag.affix
      saveToStorage()
    }
  }

  return {
    visitedTags,
    cachedNames,
    initAffixTags,
    addTag,
    closeTag,
    closeOtherTags,
    closeRightTags,
    closeAllTags,
    updateTagTitle,
    toggleAffix,
  }
})
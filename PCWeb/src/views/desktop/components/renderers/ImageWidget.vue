<!-- 文件: PCWeb/src/views/desktop/components/renderers/ImageWidget.vue -->
<!-- 快捷入口组件 -->

<template>
  <div class="quick-entry-widget">
    <div v-if="menuItems && menuItems.length > 0" class="entry-grid">
      <div
        v-for="(item, index) in menuItems"
        :key="index"
        class="entry-item"
        @click="handleClick(item)"
      >
        <div class="entry-icon">
          <el-icon :size="32">
            <component :is="item.icon" />
          </el-icon>
        </div>
        <div class="entry-name">{{ item.name }}</div>
      </div>
    </div>
    <el-empty v-else description="暂无快捷入口" :image-size="60" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import {
  DataAnalysis,
  DataBoard,
  Document,
  Setting,
  User,
  Goods,
  ShoppingCart,
  List,
  Monitor,
  House,
} from '@element-plus/icons-vue'
import type { UserWidgetConfigDto, WidgetDataResponse } from '@/types/desktopWidget'

// 图标映射表
const iconMap: Record<string, any> = {
  DataAnalysis,
  DataBoard,
  Document,
  Setting,
  User,
  Goods,
  ShoppingCart,
  List,
  Monitor,
  House,
}

// 快捷入口项
interface QuickEntryItem {
  menuId?: string
  icon: string
  name: string
  path?: string
}

// Props
const props = defineProps<{
  widget: UserWidgetConfigDto
  data?: WidgetDataResponse
}>()

const router = useRouter()

// 解析菜单配置
const menuItems = computed<QuickEntryItem[]>(() => {
  // 从 DataSourceConfig 解析
  if (props.widget.dataSourceConfig) {
    try {
      const config = JSON.parse(props.widget.dataSourceConfig)
      if (config.menus && Array.isArray(config.menus)) {
        return config.menus.map((m: any) => ({
          menuId: m.menuId,
          icon: m.icon || 'House',
          name: m.name,
          path: m.path,
        }))
      }
      // 旧格式：items 数组
      if (config.items && Array.isArray(config.items)) {
        return config.items.map((m: any) => ({
          icon: m.icon || 'House',
          name: m.name,
          path: m.path,
        }))
      }
    } catch (e) {
      console.error('解析快捷入口配置失败', e)
    }
  }

  // 从 data 解析（静态数据）
  if (props.data?.menus) {
    return props.data.menus.map((m: any) => ({
      menuId: m.menuId,
      icon: m.icon || 'House',
      name: m.name,
      path: m.path,
    }))
  }

  return []
})

// 点击跳转
function handleClick(item: QuickEntryItem) {
  if (item.path) {
    router.push(item.path)
  } else if (item.menuId) {
    // 根据 menuId 查找路由并跳转
    // 这里需要从菜单数据中查找对应的 path
    // 简化处理：直接跳转菜单管理页面
    router.push('/basic/menu')
  }
}
</script>

<style scoped lang="scss">
.quick-entry-widget {
  min-height: 120px;
  padding: 16px;

  .entry-grid {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 16px;

    .entry-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 12px;
      border-radius: 8px;
      cursor: pointer;
      transition: all 0.3s ease;

      &:hover {
        background-color: #f5f7fa;
        transform: translateY(-2px);

        .entry-icon {
          color: #409eff;
        }
      }

      .entry-icon {
        width: 48px;
        height: 48px;
        display: flex;
        align-items: center;
        justify-content: center;
        border-radius: 8px;
        background-color: #e6f7ff;
        color: #409eff;
        margin-bottom: 8px;
      }

      .entry-name {
        font-size: 14px;
        color: #303133;
        text-align: center;
      }
    }
  }

  @media (max-width: 768px) {
    .entry-grid {
      grid-template-columns: repeat(2, 1fr);
    }
  }
}
</style>
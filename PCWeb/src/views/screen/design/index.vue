<!-- 大屏设计器主页面 -->
<template>
  <div class="screen-designer">
    <!-- 左侧：组件库面板 -->
    <div class="designer-left" :class="{ 'designer-left--collapsed': leftCollapsed }">
      <ComponentLib @drag-start="handleDragStart" />
      <div class="panel-toggle" @click="leftCollapsed = !leftCollapsed">
        <el-icon>
          <ArrowLeft v-if="!leftCollapsed" />
          <ArrowRight v-else />
        </el-icon>
      </div>
    </div>

    <!-- 中间：画布区域 -->
    <div class="designer-center">
      <Canvas
        ref="canvasRef"
        :show-grid="showGrid"
        @component-dblclick="handleComponentDblClick"
      >
        <template #toolbar>
          <Toolbar
            ref="toolbarRef"
            :show-grid="showGrid"
            @toggle-grid="showGrid = !showGrid"
            @preview="handlePreview"
          />
        </template>
      </Canvas>
    </div>

    <!-- 右侧：属性面板 -->
    <div class="designer-right" :class="{ 'designer-right--collapsed': rightCollapsed }">
      <div class="property-panel-container">
        <PropertyPanel
          :component="selectedComponent"
          :screen="currentScreen"
          @update="handleUpdateComponent"
          @update-screen="handleUpdateScreen"
        />
      </div>
      <div class="panel-toggle" @click="rightCollapsed = !rightCollapsed">
        <el-icon>
          <ArrowRight v-if="!rightCollapsed" />
          <ArrowLeft v-else />
        </el-icon>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter, onBeforeRouteLeave } from 'vue-router'
import { ArrowLeft, ArrowRight } from '@element-plus/icons-vue'
import { ElMessageBox } from 'element-plus'
import { useScreenStore } from '@/stores/screen'
import { storeToRefs } from 'pinia'
import ComponentLib from './components/ComponentLib.vue'
import Canvas from './components/Canvas.vue'
import Toolbar from './components/Toolbar.vue'
import PropertyPanel from './components/PropertyPanel.vue'
import type { ScreenComponent, ScreenConfig } from '@/types'

const route = useRoute()
const router = useRouter()
const store = useScreenStore()

const { currentScreen, selectedComponent } = storeToRefs(store)

// 面板折叠状态
const leftCollapsed = ref(false)
const rightCollapsed = ref(false)
const showGrid = ref(true)

// 引用
const canvasRef = ref()
const toolbarRef = ref()

// 拖拽开始
const handleDragStart = (event: DragEvent, componentType: string) => {
  // 拖拽开始时的处理
}

// 组件双击
const handleComponentDblClick = (component: ScreenComponent) => {
  // 双击进入编辑模式
}

// 更新组件
const handleUpdateComponent = (updates: Partial<ScreenComponent>) => {
  if (selectedComponent.value) {
    store.updateComponent(selectedComponent.value.id, updates)
  }
}

// 更新大屏
const handleUpdateScreen = (updates: Partial<ScreenConfig>) => {
  if (updates.name !== undefined || updates.description !== undefined) {
    store.updateScreenInfo({ name: updates.name, description: updates.description })
  }
  if (updates.style) {
    store.updateScreenStyle(updates.style)
  }
}

// 预览
const handlePreview = () => {
  if (currentScreen.value) {
    // 先保存，再跳转预览
    const id = currentScreen.value.id
    if (id) {
      window.open(`/#/screen/view/${id}`, '_blank')
    }
  }
}

// 加载大屏数据
onMounted(async () => {
  const id = route.params.id as string | undefined
  await store.loadScreen(id || null)
})

// 离开前确认
onBeforeRouteLeave(async (to, from, next) => {
  if (store.isDirty) {
    try {
      await ElMessageBox.confirm(
        '有未保存的更改，确定离开？',
        '提示',
        {
          confirmButtonText: '保存并离开',
          cancelButtonText: '取消',
          distinguishCancelAndClose: true,
        }
      )
      // 用户选择保存
      await store.saveScreen()
      next()
    } catch (action) {
      if (action === 'cancel') {
        next(false)
      } else {
        // 关闭弹窗，直接离开
        next()
      }
    }
  } else {
    next()
  }
})
</script>

<style scoped lang="scss">
.screen-designer {
  display: flex;
  height: 100vh;
  background: #1a1a2e;
  overflow: hidden;

  // 全局滚动条样式
  * {
    &::-webkit-scrollbar {
      width: 6px;
      height: 6px;
    }

    &::-webkit-scrollbar-track {
      background: rgba(255, 255, 255, 0.05);
      border-radius: 3px;
    }

    &::-webkit-scrollbar-thumb {
      background: rgba(255, 255, 255, 0.2);
      border-radius: 3px;

      &:hover {
        background: rgba(255, 255, 255, 0.3);
      }
    }

    &::-webkit-scrollbar-corner {
      background: transparent;
    }
  }
}

.designer-left {
  position: relative;
  width: 280px;
  flex-shrink: 0;
  transition: width 0.3s;

  &--collapsed {
    width: 0;
  }
}

.designer-center {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.designer-right {
  position: relative;
  width: 320px;
  height: 100vh;
  flex-shrink: 0;
  transition: width 0.3s;
  display: flex;
  flex-direction: column;

  &--collapsed {
    width: 0;
  }

  // 确保属性面板可以正确滚动
  .property-panel-container {
    flex: 1;
    min-height: 0;
    display: flex;
    flex-direction: column;
  }

  // 折叠时隐藏内容
  &.designer-right--collapsed {
    .property-panel-container,
    .panel-toggle {
      visibility: hidden;
      opacity: 0;
    }
  }
}

.panel-toggle {
  position: absolute;
  top: 50%;
  width: 20px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.1);
  cursor: pointer;
  z-index: 10;

  .designer-left & {
    right: -20px;
    transform: translateY(-50%);
  }

  .designer-right & {
    left: -20px;
    transform: translateY(-50%);
  }

  &:hover {
    background: rgba(255, 255, 255, 0.2);
  }

  .el-icon {
    color: #fff;
    font-size: 12px;
  }
}
</style>
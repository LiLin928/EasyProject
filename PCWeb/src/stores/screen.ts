// 大屏设计器状态管理 Store

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { ScreenConfig, ScreenComponent, ScreenDesignState, ComponentType } from '@/types'
import { getScreenDetail, updateScreen } from '@/api/screen'
import { generateGuid } from '@/utils/guid'
import { convertScreenToFrontend } from '@/utils/screenConverter'

// 网格对齐单位
const GRID_SIZE = 20

// 最大历史记录数
const MAX_HISTORY = 50

// 组件默认配置
const COMPONENT_DEFAULTS: Record<ComponentType, { size: { width: number; height: number }; data: any[]; config: Record<string, any> }> = {
  'line-chart': {
    size: { width: 400, height: 300 },
    data: [{ x: '1月', y: 100 }, { x: '2月', y: 150 }, { x: '3月', y: 200 }],
    config: { title: '折线图', xField: 'x', yField: 'y' },
  },
  'bar-chart': {
    size: { width: 400, height: 300 },
    data: [{ x: 'A', y: 100 }, { x: 'B', y: 150 }, { x: 'C', y: 200 }],
    config: { title: '柱状图', xField: 'x', yField: 'y' },
  },
  'pie-chart': {
    size: { width: 400, height: 300 },
    data: [{ name: '类别A', value: 30 }, { name: '类别B', value: 40 }, { name: '类别C', value: 30 }],
    config: { title: '饼图', nameField: 'name', valueField: 'value' },
  },
  'radar-chart': {
    size: { width: 400, height: 300 },
    data: [{ dimension: '销售', value: 80 }, { dimension: '管理', value: 90 }, { dimension: '技术', value: 70 }],
    config: { title: '雷达图', dimensionField: 'dimension', valueField: 'value' },
  },
  'funnel-chart': {
    size: { width: 400, height: 300 },
    data: [{ name: '浏览', value: 100 }, { name: '点击', value: 80 }, { name: '转化', value: 50 }],
    config: { title: '漏斗图', nameField: 'name', valueField: 'value' },
  },
  'heatmap-chart': {
    size: { width: 500, height: 400 },
    data: [{ x: '周一', y: '上午', value: 10 }, { x: '周一', y: '下午', value: 20 }],
    config: { title: '热力图', xField: 'x', yField: 'y', valueField: 'value' },
  },
  'number-card': {
    size: { width: 200, height: 150 },
    data: [{ value: 0, label: '数值' }],
    config: { title: '数字卡片', unit: '', color: '#409eff' },
  },
  'data-table': {
    size: { width: 500, height: 300 },
    data: [],
    config: { title: '数据表格', columns: [] },
  },
  'title': {
    size: { width: 1920, height: 80 },
    data: [],
    config: { text: '标题文字', fontSize: 36, color: '#ffffff', align: 'center' },
  },
  'border': {
    size: { width: 400, height: 300 },
    data: [],
    config: { borderWidth: 2, borderColor: 'rgba(255,255,255,0.3)', borderRadius: 8 },
  },
  'image': {
    size: { width: 400, height: 300 },
    data: [],
    config: { url: '', fit: 'cover', opacity: 100 },
  },
  'video': {
    size: { width: 400, height: 300 },
    data: [],
    config: { url: '', autoplay: false, loop: false, muted: true, controls: true },
  },
}

export const useScreenStore = defineStore('screen', () => {
  // 状态
  const currentScreen = ref<ScreenConfig | null>(null)
  const selectedComponentId = ref<string | null>(null)
  const canvasZoom = ref(1)
  const canvasOffset = ref({ x: 0, y: 0 })
  const history = ref<ScreenConfig[]>([])
  const historyIndex = ref(-1)
  const isDirty = ref(false)
  const loading = ref(false)

  // 计算属性
  const selectedComponent = computed(() => {
    if (!currentScreen.value || !selectedComponentId.value) return null
    return currentScreen.value.components.find(c => c.id === selectedComponentId.value)
  })

  const canUndo = computed(() => historyIndex.value > 0)
  const canRedo = computed(() => historyIndex.value < history.value.length - 1)

  // 网格对齐函数
  const alignToGrid = (value: number): number => {
    return Math.round(value / GRID_SIZE) * GRID_SIZE
  }

  // 记录历史
  const pushHistory = () => {
    if (!currentScreen.value) return

    // 删除当前位置之后的历史
    history.value = history.value.slice(0, historyIndex.value + 1)

    // 添加当前状态到历史
    history.value.push(JSON.parse(JSON.stringify(currentScreen.value)))

    // 限制历史记录数量
    if (history.value.length > MAX_HISTORY) {
      history.value.shift()
    } else {
      historyIndex.value++
    }

    isDirty.value = true
  }

  // 加载大屏配置
  const loadScreen = async (id: string | null) => {
    loading.value = true
    try {
      if (id) {
        // 编辑现有大屏 - 使用转换器转换后端格式
        const data = await getScreenDetail(id)
        currentScreen.value = convertScreenToFrontend(data as any)
      } else {
        // 新建大屏，创建空白配置
        currentScreen.value = {
          id: '',
          name: '新建大屏',
          description: '',
          thumbnail: '',
          style: {
            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
            width: 1920,
            height: 1080,
          },
          components: [
            {
              id: generateGuid(),
              type: 'title',
              position: { x: 0, y: 20 },
              size: { width: 1920, height: 80 },
              dataSource: { type: 'static', data: [] },
              config: { text: '新建大屏', fontSize: 36, color: '#ffffff', align: 'center' },
            },
          ],
          permissions: { sharedUsers: [], sharedRoles: [] },
          isPublic: 0,
          createdBy: '',
          createTime: new Date().toISOString(),
          updateTime: new Date().toISOString(),
        }
      }
      // 清空历史
      history.value = []
      historyIndex.value = -1
      isDirty.value = false
      selectedComponentId.value = null
    } catch (error) {
      console.error('加载大屏失败:', error)
    } finally {
      loading.value = false
    }
  }

  // 添加组件
  const addComponent = (type: ComponentType, position: { x: number; y: number }): string => {
    console.log('addComponent called:', { type, position, currentScreen: !!currentScreen.value })

    if (!currentScreen.value) {
      console.error('addComponent: currentScreen is null')
      return ''
    }

    const defaults = COMPONENT_DEFAULTS[type]
    if (!defaults) {
      console.error('addComponent: unknown component type:', type)
      return ''
    }

    const id = generateGuid()

    const newComponent: ScreenComponent = {
      id,
      type,
      position: { x: alignToGrid(position.x), y: alignToGrid(position.y) },
      size: { width: defaults.size.width, height: defaults.size.height },
      dataSource: { type: 'static', data: defaults.data },
      config: defaults.config,
    }

    console.log('Adding new component:', newComponent)
    currentScreen.value.components.push(newComponent)
    pushHistory()
    selectedComponentId.value = id

    return id
  }

  // 更新组件配置
  const updateComponent = (id: string, updates: Partial<ScreenComponent>) => {
    if (!currentScreen.value) return

    const index = currentScreen.value.components.findIndex(c => c.id === id)
    if (index === -1) return

    const component = currentScreen.value.components[index]

    // 如果更新位置或尺寸，进行网格对齐
    if (updates.position) {
      updates.position = {
        x: alignToGrid(updates.position.x),
        y: alignToGrid(updates.position.y),
      }
    }
    if (updates.size) {
      updates.size = {
        width: alignToGrid(updates.size.width),
        height: alignToGrid(updates.size.height),
      }
    }

    currentScreen.value.components[index] = { ...component, ...updates }
    pushHistory()
  }

  // 删除组件
  const deleteComponent = (id: string) => {
    if (!currentScreen.value) return

    const index = currentScreen.value.components.findIndex(c => c.id === id)
    if (index === -1) return

    currentScreen.value.components.splice(index, 1)
    pushHistory()

    if (selectedComponentId.value === id) {
      selectedComponentId.value = null
    }
  }

  // 选中/取消选中组件
  const selectComponent = (id: string | null) => {
    selectedComponentId.value = id
  }

  // 移动组件
  const moveComponent = (id: string, position: { x: number; y: number }) => {
    updateComponent(id, { position })
  }

  // 缩放组件
  const resizeComponent = (id: string, size: { width: number; height: number }) => {
    updateComponent(id, { size })
  }

  // 撤销
  const undo = () => {
    if (!canUndo.value) return

    historyIndex.value--
    currentScreen.value = JSON.parse(JSON.stringify(history.value[historyIndex.value]))
  }

  // 重做
  const redo = () => {
    if (!canRedo.value) return

    historyIndex.value++
    currentScreen.value = JSON.parse(JSON.stringify(history.value[historyIndex.value]))
  }

  // 设置画布缩放
  const setCanvasZoom = (zoom: number) => {
    canvasZoom.value = Math.max(0.5, Math.min(2, zoom))
  }

  // 设置画布偏移
  const setCanvasOffset = (offset: { x: number; y: number }) => {
    canvasOffset.value = offset
  }

  // 更新大屏样式
  const updateScreenStyle = (style: Partial<ScreenConfig['style']>) => {
    if (!currentScreen.value) return

    currentScreen.value.style = { ...currentScreen.value.style, ...style }
    pushHistory()
  }

  // 更新大屏基本信息
  const updateScreenInfo = (info: { name?: string; description?: string }) => {
    if (!currentScreen.value) return

    if (info.name) currentScreen.value.name = info.name
    if (info.description) currentScreen.value.description = info.description
    pushHistory()
  }

  // 保存大屏
  const saveScreen = async (): Promise<boolean> => {
    if (!currentScreen.value || !isDirty.value) return true

    loading.value = true
    try {
      // 转换组件格式：前端嵌套对象 -> 后端扁平字段 + JSON字符串
      const convertedComponents = currentScreen.value.components.map((comp, index) => ({
        id: comp.id,
        screenId: currentScreen.value!.id,
        componentType: comp.type,
        positionX: comp.position.x,
        positionY: comp.position.y,
        width: comp.size.width,
        height: comp.size.height,
        rotation: comp.rotation || 0,
        locked: comp.locked ? 1 : 0,
        visible: comp.visible !== false ? 1 : 0,
        dataSource: JSON.stringify(comp.dataSource),
        config: JSON.stringify(comp.config),
        styleConfig: JSON.stringify(comp.style || {}),
        dataBinding: JSON.stringify(comp.dataBinding || {}),
        sortOrder: index,
      }))

      await updateScreen({
        id: currentScreen.value.id,
        name: currentScreen.value.name,
        description: currentScreen.value.description,
        style: JSON.stringify(currentScreen.value.style),
        components: convertedComponents,
        permissions: JSON.stringify(currentScreen.value.permissions),
      })

      // 保存成功后清空历史
      history.value = []
      historyIndex.value = -1
      isDirty.value = false

      return true
    } catch (error) {
      console.error('保存大屏失败:', error)
      return false
    } finally {
      loading.value = false
    }
  }

  // 重置状态
  const resetState = () => {
    currentScreen.value = null
    selectedComponentId.value = null
    canvasZoom.value = 1
    canvasOffset.value = { x: 0, y: 0 }
    history.value = []
    historyIndex.value = -1
    isDirty.value = false
  }

  return {
    // 状态
    currentScreen,
    selectedComponentId,
    canvasZoom,
    canvasOffset,
    history,
    historyIndex,
    isDirty,
    loading,

    // 计算属性
    selectedComponent,
    canUndo,
    canRedo,

    // 方法
    loadScreen,
    addComponent,
    updateComponent,
    deleteComponent,
    selectComponent,
    moveComponent,
    resizeComponent,
    undo,
    redo,
    setCanvasZoom,
    setCanvasOffset,
    updateScreenStyle,
    updateScreenInfo,
    saveScreen,
    resetState,
    alignToGrid,
  }
})
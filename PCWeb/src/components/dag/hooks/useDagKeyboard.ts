/**
 * DAG 键盘快捷键 Hook
 * 处理 Ctrl+Z (撤销)、Ctrl+Y/Ctrl+Shift+Z (重做)、Ctrl+S (保存)
 */
import { onMounted, onUnmounted } from 'vue'

export interface UseDagKeyboardOptions {
  /** 撤销回调 */
  onUndo: () => void
  /** 重做回调 */
  onRedo: () => void
  /** 保存回调 */
  onSave: () => void
}

export function useDagKeyboard(options: UseDagKeyboardOptions) {
  const handleKeyDown = (e: KeyboardEvent) => {
    // Ctrl+Z: 撤销
    if (e.ctrlKey && e.key === 'z' && !e.shiftKey) {
      e.preventDefault()
      options.onUndo()
      return
    }

    // Ctrl+Y 或 Ctrl+Shift+Z: 重做
    if ((e.ctrlKey && e.key === 'y') || (e.ctrlKey && e.shiftKey && e.key === 'z')) {
      e.preventDefault()
      options.onRedo()
      return
    }

    // Ctrl+S: 保存
    if (e.ctrlKey && e.key === 's') {
      e.preventDefault()
      options.onSave()
      return
    }
  }

  onMounted(() => {
    document.addEventListener('keydown', handleKeyDown)
  })

  onUnmounted(() => {
    document.removeEventListener('keydown', handleKeyDown)
  })
}
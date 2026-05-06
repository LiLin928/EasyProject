import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import '@/assets/styles/index.scss'
import '@/assets/styles/screen.scss'
import i18n from './locales'
import zhCnElement from 'element-plus/es/locale/lang/zh-cn'
import enElement from 'element-plus/es/locale/lang/en'
import { useTagsViewStore } from '@/stores/tagsView'
import { useDictCacheStore } from '@/stores/dictCacheStore'
import { getEnabledDictCodes } from '@/config/dict.config'
import { constantRoutes } from '@/router/routes'
import { loader } from '@guolao/vue-monaco-editor'

// 配置 Monaco Editor loader (使用本地 monaco-editor)
loader.config({
  paths: {
    vs: 'https://cdn.jsdelivr.net/npm/monaco-editor@0.52.0/min/vs'
  },
})

const app = createApp(App)

// 注册 Element Plus 图标
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

app.use(createPinia())

// 初始化字典缓存
const dictCacheStore = useDictCacheStore()
dictCacheStore.init()

// 检查并更新字典缓存（异步执行，不阻塞应用启动）
const enabledCodes = getEnabledDictCodes()
if (enabledCodes.length > 0) {
  dictCacheStore.checkAndUpdate(enabledCodes).catch(e => {
    console.warn('Failed to update dict cache:', e)
  })
}

app.use(router)
app.use(i18n)

// 初始化语言设置（直接处理，不使用 composable）
const savedLocale = localStorage.getItem('locale')
const defaultLocale: 'zh-CN' | 'en-US' = (savedLocale && ['zh-CN', 'en-US'].includes(savedLocale)) ? savedLocale as 'zh-CN' | 'en-US' : 'zh-CN'

// 设置 i18n locale
i18n.global.locale.value = defaultLocale
document.documentElement.setAttribute('lang', defaultLocale)

// 获取 Element Plus 语言包
const elementLocale = defaultLocale === 'en-US' ? enElement : zhCnElement
app.use(ElementPlus, { locale: elementLocale })

// 初始化固定标签
const tagsViewStore = useTagsViewStore()
tagsViewStore.initAffixTags(constantRoutes)

app.mount('#app')
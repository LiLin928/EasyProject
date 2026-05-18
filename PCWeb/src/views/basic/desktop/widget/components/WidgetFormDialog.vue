<!-- 文件: PCWeb/src/views/basic/desktop/widget/components/WidgetFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    :title="isEdit ? '编辑组件' : '新建组件'"
    width="800px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="120px"
      v-loading="loading"
    >
      <!-- 基本信息 -->
      <el-divider content-position="left">基本信息</el-divider>

      <el-form-item label="组件名称" prop="name">
        <el-input v-model="formData.name" maxlength="50" show-word-limit />
      </el-form-item>

      <el-form-item label="组件类型" prop="type">
        <el-select v-model="formData.type" @change="handleTypeChange">
          <el-option
            v-for="(label, value) in widgetTypeLabels"
            :key="value"
            :label="label"
            :value="Number(value)"
          />
        </el-select>
      </el-form-item>

      <el-row :gutter="20">
        <el-col :span="8">
          <el-form-item label="默认宽度" prop="defaultWidth">
            <el-select v-model="formData.defaultWidth">
              <el-option
                v-for="opt in gridWidthOptions"
                :key="opt.value"
                :label="opt.label"
                :value="opt.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="默认高度" prop="defaultHeight">
            <el-input-number v-model="formData.defaultHeight" :min="100" :max="800" :step="50" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="状态" prop="status">
            <el-switch
              v-model="formData.status"
              :active-value="1"
              :inactive-value="0"
              active-text="启用"
              inactive-text="禁用"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 数据源配置 -->
      <el-divider content-position="left">数据源配置</el-divider>

      <el-form-item label="数据源类型" prop="dataSourceType">
        <el-select v-model="formData.dataSourceType" @change="handleDataSourceTypeChange">
          <el-option
            v-for="(label, value) in dataSourceTypeLabels"
            :key="value"
            :label="label"
            :value="Number(value)"
          />
        </el-select>
      </el-form-item>

      <!-- SQL类型配置 -->
      <template v-if="formData.dataSourceType === DataSourceType.Sql">
        <el-form-item label="SQL语句" prop="sqlStatement">
          <el-input
            v-model="sqlConfig.sql"
            type="textarea"
            :rows="3"
            placeholder="SELECT COUNT(*) FROM User WHERE Status = 1"
          />
          <div class="form-tip">只允许SELECT语句，支持COUNT统计或多列查询</div>
        </el-form-item>
        <el-form-item label="显示标签">
          <el-input v-model="sqlConfig.label" placeholder="如：用户总数" />
        </el-form-item>
      </template>

      <!-- 报表类型配置 -->
      <template v-if="formData.dataSourceType === DataSourceType.Report">
        <el-form-item label="选择报表" prop="reportId">
          <el-select
            v-model="reportConfig.reportId"
            placeholder="请选择报表"
            filterable
            @change="handleReportSelect"
          >
            <el-option
              v-for="report in reportList"
              :key="report.id"
              :label="report.name"
              :value="report.id"
            >
              <span>{{ report.name }}</span>
              <span style="color: #909399; margin-left: 8px; font-size: 12px;">{{ report.category }}</span>
            </el-option>
          </el-select>
          <div class="form-tip">选择报表后，组件将显示报表的图表数据</div>
        </el-form-item>
        <el-form-item label="刷新间隔">
          <el-select v-model="reportConfig.refreshInterval">
            <el-option label="手动刷新" :value="0" />
            <el-option label="30秒" :value="30" />
            <el-option label="1分钟" :value="60" />
            <el-option label="5分钟" :value="300" />
          </el-select>
        </el-form-item>
      </template>

      <!-- 静态类型配置（快捷入口） -->
      <template v-if="formData.dataSourceType === DataSourceType.Static && formData.type === WidgetType.Image">
        <el-form-item label="快捷菜单">
          <div class="quick-menu-list">
            <div v-for="(item, index) in quickMenus" :key="index" class="quick-menu-item">
              <el-select
                v-model="item.menuId"
                placeholder="选择菜单"
                filterable
                @change="handleMenuSelect(index)"
              >
                <el-option
                  v-for="menu in flatMenuList"
                  :key="menu.id"
                  :label="menu.menuName"
                  :value="menu.id"
                >
                  <span>{{ menu.menuName }}</span>
                  <span style="color: #909399; margin-left: 8px; font-size: 12px;">{{ menu.path }}</span>
                </el-option>
              </el-select>
              <el-button
                type="danger"
                link
                @click="removeQuickMenu(index)"
                v-if="quickMenus.length > 1"
              >
                删除
              </el-button>
            </div>
            <el-button type="primary" link @click="addQuickMenu">
              <el-icon><Plus /></el-icon>
              添加菜单
            </el-button>
          </div>
        </el-form-item>
      </template>

      <!-- API类型配置 -->
      <template v-if="formData.dataSourceType === DataSourceType.Api">
        <el-form-item label="API路径">
          <el-input v-model="apiConfig.api" placeholder="/api/xxx/list" />
        </el-form-item>
        <el-form-item label="请求方法">
          <el-select v-model="apiConfig.method">
            <el-option label="GET" value="GET" />
            <el-option label="POST" value="POST" />
          </el-select>
        </el-form-item>
      </template>

      <!-- 交互配置 -->
      <el-divider content-position="left">交互配置</el-divider>

      <el-form-item label="点击跳转类型">
        <el-radio-group v-model="interactionConfig.type" @change="handleInteractionTypeChange">
          <el-radio value="menu">跳转菜单</el-radio>
          <el-radio value="report">跳转报表</el-radio>
          <el-radio value="none">无跳转</el-radio>
        </el-radio-group>
      </el-form-item>

      <!-- 菜单跳转 -->
      <el-form-item v-if="interactionConfig.type === 'menu'" label="跳转菜单">
        <el-select
          v-model="interactionConfig.menuId"
          placeholder="选择跳转菜单"
          filterable
          clearable
        >
          <el-option
            v-for="menu in flatMenuList"
            :key="menu.id"
            :label="menu.menuName"
            :value="menu.id"
          >
            <span>{{ menu.menuName }}</span>
            <span style="color: #909399; margin-left: 8px; font-size: 12px;">{{ menu.path }}</span>
          </el-option>
        </el-select>
        <div class="form-tip">点击组件卡片时跳转到选中的菜单页面</div>
      </el-form-item>

      <!-- 报表跳转 -->
      <el-form-item v-if="interactionConfig.type === 'report'" label="跳转报表">
        <el-select
          v-model="interactionConfig.reportId"
          placeholder="选择报表"
          filterable
          @change="handleInteractionReportSelect"
        >
          <el-option
            v-for="report in reportList"
            :key="report.id"
            :label="report.name"
            :value="report.id"
          >
            <span>{{ report.name }}</span>
            <span style="color: #909399; margin-left: 8px; font-size: 12px;">预览: /report/publish/{{ report.id }}</span>
          </el-option>
        </el-select>
        <div class="form-tip">点击组件卡片时跳转到报表预览页面 /report/publish/:id</div>
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" :loading="saving" @click="handleSubmit">保存</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import type { FormInstance, FormRules } from 'element-plus'
import { getDesktopWidgetDetail, addDesktopWidget, updateDesktopWidget } from '@/api/basic/desktopWidgetApi'
import { getMenuTree } from '@/api/menu'
import { getReportList } from '@/api/report/reportApi'
import {
  WidgetType,
  DataSourceType,
  widgetTypeLabels,
  dataSourceTypeLabels,
  gridWidthOptions,
} from '@/types'
import type { MockMenu } from '@/types/menu'
import type { Report } from '@/types'

const props = defineProps<{
  modelValue: boolean
  id?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const isEdit = computed(() => !!props.id)

const loading = ref(false)
const saving = ref(false)
const formRef = ref<FormInstance | null>(null)

// 菜单列表（扁平化）
const menuList = ref<MockMenu[]>([])
const flatMenuList = computed(() => {
  const result: MockMenu[] = []
  function flatten(menus: MockMenu[]) {
    menus.forEach(menu => {
      if (menu.path && menu.component) {
        result.push(menu)
      }
      if (menu.children?.length) {
        flatten(menu.children)
      }
    })
  }
  flatten(menuList.value)
  return result
})

// 报表列表
const reportList = ref<Report[]>([])

// 表单数据
const formData = reactive({
  name: '',
  type: WidgetType.Card,
  defaultWidth: 3,
  defaultHeight: 200,
  dataSourceType: DataSourceType.Sql,
  status: 1,
})

// SQL配置
const sqlConfig = reactive({
  sql: '',
  label: '',
})

// API配置
const apiConfig = reactive({
  api: '',
  method: 'GET',
})

// 报表配置
const reportConfig = reactive({
  reportId: '',
  reportName: '',
  refreshInterval: 0,
})

// 快捷菜单列表
const quickMenus = ref<{ menuId: string; name: string; icon: string; path: string }[]>([
  { menuId: '', name: '', icon: '', path: '' }
])

// 交互配置
const interactionConfig = reactive({
  type: 'none' as 'menu' | 'report' | 'none',
  menuId: '',
  reportId: '',
  reportName: '',
})

// 表单规则
const formRules: FormRules = {
  name: [
    { required: true, message: '请输入组件名称', trigger: 'blur' },
    { min: 2, max: 50, message: '名称长度为2-50个字符', trigger: 'blur' },
  ],
  type: [{ required: true, message: '请选择组件类型', trigger: 'change' }],
  defaultWidth: [{ required: true, message: '请选择默认宽度', trigger: 'change' }],
  defaultHeight: [{ required: true, message: '请输入默认高度', trigger: 'blur' }],
  dataSourceType: [{ required: true, message: '请选择数据源类型', trigger: 'change' }],
}

// 加载菜单列表
const loadMenuList = async () => {
  try {
    const data = await getMenuTree()
    menuList.value = data
  } catch (error) {
    console.error('加载菜单失败', error)
  }
}

// 加载报表列表
const loadReportList = async () => {
  try {
    const data = await getReportList({ pageIndex: 1, pageSize: 100 })
    reportList.value = data.list
  } catch (error) {
    console.error('加载报表列表失败', error)
  }
}

// 加载详情
const loadDetail = async () => {
  if (!props.id) return
  loading.value = true
  try {
    const data = await getDesktopWidgetDetail(props.id)
    formData.name = data.name
    formData.type = data.type
    formData.defaultWidth = data.defaultWidth
    formData.defaultHeight = data.defaultHeight
    formData.dataSourceType = data.dataSourceType
    formData.status = data.status

    // 解析数据源配置
    if (data.dataSourceConfig) {
      const config = JSON.parse(data.dataSourceConfig)
      if (formData.dataSourceType === DataSourceType.Sql) {
        sqlConfig.sql = config.sql || ''
        sqlConfig.label = config.label || ''
      } else if (formData.dataSourceType === DataSourceType.Api) {
        apiConfig.api = config.api || ''
        apiConfig.method = config.method || 'GET'
      } else if (formData.dataSourceType === DataSourceType.Report) {
        reportConfig.reportId = config.reportId || ''
        reportConfig.reportName = config.reportName || ''
        reportConfig.refreshInterval = config.refreshInterval || 0
      } else if (formData.dataSourceType === DataSourceType.Static && formData.type === WidgetType.Image) {
        if (config.menus && Array.isArray(config.menus)) {
          quickMenus.value = config.menus.map((m: any) => ({
            menuId: m.menuId || '',
            name: m.name || '',
            icon: m.icon || '',
            path: m.path || '',
          }))
        }
      }
    }

    // 解析交互配置
    if (data.interactionConfig) {
      const config = JSON.parse(data.interactionConfig)
      if (config.menuId) {
        interactionConfig.type = 'menu'
        interactionConfig.menuId = config.menuId
      } else if (config.reportId) {
        interactionConfig.type = 'report'
        interactionConfig.reportId = config.reportId
        interactionConfig.reportName = config.reportName || ''
      } else {
        interactionConfig.type = 'none'
      }
    }
  } catch (error) {
    console.error('加载详情失败', error)
  } finally {
    loading.value = false
  }
}

// 类型变化处理
const handleTypeChange = () => {
  // 快捷入口默认使用静态类型
  if (formData.type === WidgetType.Image) {
    formData.dataSourceType = DataSourceType.Static
  }
  // 图表组件默认使用报表类型
  if (formData.type === WidgetType.Chart) {
    formData.dataSourceType = DataSourceType.Report
  }
}

// 数据源类型变化处理
const handleDataSourceTypeChange = () => {
  // 重置配置
  sqlConfig.sql = ''
  sqlConfig.label = ''
  apiConfig.api = ''
  apiConfig.method = 'GET'
  reportConfig.reportId = ''
  reportConfig.reportName = ''
  reportConfig.refreshInterval = 0
  quickMenus.value = [{ menuId: '', name: '', icon: '', path: '' }]
}

// 菜单选择处理
const handleMenuSelect = (index: number) => {
  const menuId = quickMenus.value[index].menuId
  const menu = flatMenuList.value.find(m => m.id === menuId)
  if (menu) {
    quickMenus.value[index].name = menu.menuName
    quickMenus.value[index].icon = menu.icon || ''
    quickMenus.value[index].path = menu.path || ''
  }
}

// 报表选择处理
const handleReportSelect = () => {
  const report = reportList.value.find(r => r.id === reportConfig.reportId)
  if (report) {
    reportConfig.reportName = report.name
  }
}

// 交互报表选择处理
const handleInteractionReportSelect = () => {
  const report = reportList.value.find(r => r.id === interactionConfig.reportId)
  if (report) {
    interactionConfig.reportName = report.name
  }
}

// 交互类型变化处理
const handleInteractionTypeChange = () => {
  interactionConfig.menuId = ''
  interactionConfig.reportId = ''
  interactionConfig.reportName = ''
}

// 添加快捷菜单
const addQuickMenu = () => {
  quickMenus.value.push({ menuId: '', name: '', icon: '', path: '' })
}

// 删除快捷菜单
const removeQuickMenu = (index: number) => {
  quickMenus.value.splice(index, 1)
}

// 构建数据源配置JSON
const buildDataSourceConfig = () => {
  switch (formData.dataSourceType) {
    case DataSourceType.Sql:
      return JSON.stringify({
        sql: sqlConfig.sql,
        label: sqlConfig.label || formData.name,
      })
    case DataSourceType.Api:
      return JSON.stringify({
        api: apiConfig.api,
        method: apiConfig.method,
      })
    case DataSourceType.Report:
      return JSON.stringify({
        reportId: reportConfig.reportId,
        reportName: reportConfig.reportName,
        refreshInterval: reportConfig.refreshInterval,
      })
    case DataSourceType.Static:
      if (formData.type === WidgetType.Image) {
        return JSON.stringify({
          menus: quickMenus.value.filter(m => m.menuId),
        })
      }
      return ''
    default:
      return ''
  }
}

// 构建交互配置JSON
const buildInteractionConfig = () => {
  switch (interactionConfig.type) {
    case 'menu':
      if (interactionConfig.menuId) {
        return JSON.stringify({
          menuId: interactionConfig.menuId,
        })
      }
      return ''
    case 'report':
      if (interactionConfig.reportId) {
        return JSON.stringify({
          reportId: interactionConfig.reportId,
          reportName: interactionConfig.reportName,
          path: `/report/publish/${interactionConfig.reportId}`,
        })
      }
      return ''
    default:
      return ''
  }
}

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch {
    return
  }

  // 验证SQL配置
  if (formData.dataSourceType === DataSourceType.Sql && !sqlConfig.sql) {
    ElMessage.warning('请输入SQL语句')
    return
  }

  // 验证报表配置
  if (formData.dataSourceType === DataSourceType.Report && !reportConfig.reportId) {
    ElMessage.warning('请选择报表')
    return
  }

  // 验证快捷菜单配置
  if (formData.dataSourceType === DataSourceType.Static && formData.type === WidgetType.Image) {
    if (!quickMenus.value.some(m => m.menuId)) {
      ElMessage.warning('请至少选择一个菜单')
      return
    }
  }

  saving.value = true
  try {
    const dataSourceConfig = buildDataSourceConfig()
    const interactionConfigStr = buildInteractionConfig()

    if (isEdit.value) {
      await updateDesktopWidget({
        id: props.id!,
        name: formData.name,
        type: formData.type,
        defaultWidth: formData.defaultWidth,
        defaultHeight: formData.defaultHeight,
        dataSourceType: formData.dataSourceType,
        dataSourceConfig: dataSourceConfig,
        interactionConfig: interactionConfigStr,
        status: formData.status,
      })
      ElMessage.success('更新成功')
    } else {
      await addDesktopWidget({
        name: formData.name,
        type: formData.type,
        defaultWidth: formData.defaultWidth,
        defaultHeight: formData.defaultHeight,
        dataSourceType: formData.dataSourceType,
        dataSourceConfig: dataSourceConfig,
        interactionConfig: interactionConfigStr,
        status: formData.status,
      })
      ElMessage.success('创建成功')
    }
    visible.value = false
    emit('success')
  } catch (error) {
    console.error('保存失败', error)
  } finally {
    saving.value = false
  }
}

// 关闭
const handleClose = () => {
  visible.value = false
}

// 重置表单
const resetForm = () => {
  formData.name = ''
  formData.type = WidgetType.Card
  formData.defaultWidth = 3
  formData.defaultHeight = 200
  formData.dataSourceType = DataSourceType.Sql
  formData.status = 1
  sqlConfig.sql = ''
  sqlConfig.label = ''
  apiConfig.api = ''
  apiConfig.method = 'GET'
  reportConfig.reportId = ''
  reportConfig.reportName = ''
  reportConfig.refreshInterval = 0
  quickMenus.value = [{ menuId: '', name: '', icon: '', path: '' }]
  interactionConfig.type = 'none'
  interactionConfig.menuId = ''
  interactionConfig.reportId = ''
  interactionConfig.reportName = ''
}

// 监听弹窗打开
watch(visible, (val) => {
  if (val) {
    if (props.id) {
      loadDetail()
    } else {
      resetForm()
    }
  }
})

onMounted(() => {
  loadMenuList()
  loadReportList()
})
</script>

<style scoped lang="scss">
.form-tip {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}

.quick-menu-list {
  .quick-menu-item {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 8px;

    .el-select {
      flex: 1;
    }
  }
}

.el-divider {
  margin: 20px 0 16px;
}
</style>
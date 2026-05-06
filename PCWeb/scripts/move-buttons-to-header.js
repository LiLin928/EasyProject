// scripts/move-buttons-to-header.js
// 将 Dialog/Drawer 的底部按钮移到顶部

const fs = require('fs')
const path = require('path')

// 需要处理的文件列表
const files = [
  // views
  'src/views/workflow/list/components/WorkflowFormDialog.vue',
  'src/views/workflow/list/components/VersionHistoryDialog.vue',
  'src/views/workflow/list/components/PublishDialog.vue',
  'src/views/workflow/approval/components/ApprovalDialog.vue',
  'src/views/screen/components/ShareConfigDialog.vue',
  'src/views/report/components/DrilldownConfigDialog.vue',
  'src/views/report/components/DataSourceFormDialog.vue',
  'src/views/person/components/EditProfileDialog.vue',
  'src/views/person/components/ChangePasswordDialog.vue',
  'src/views/person/components/ChangeAvatarDialog.vue',
  'src/views/person/components/BindPhoneDialog.vue',
  'src/views/person/components/BindEmailDialog.vue',
  'src/views/etl/schedule/components/ScheduleFormDialog.vue',
  'src/views/etl/pipeline/components/PipelineFormDialog.vue',
  'src/views/etl/monitor/components/ExecutionLogsDialog.vue',
  'src/views/etl/monitor/components/ExecutionDetailDialog.vue',
  'src/views/etl/datasource/components/TestConnectionDialog.vue',
  'src/views/etl/datasource/components/DatasourceFormDialog.vue',
  'src/views/buz/supplier/components/SupplierProductDialog.vue',
  'src/views/buz/supplier/components/SupplierFormDialog.vue',
  'src/views/buz/refund/components/RefundDetailDialog.vue',
  'src/views/buz/product/components/StockInDialog.vue',
  'src/views/buz/product/components/StockAdjustDialog.vue',
  'src/views/buz/product/components/ProductSupplierDialog.vue',
  'src/views/buz/product/components/CategoryFormDialog.vue',
  'src/views/buz/order/components/ShipTrackDialog.vue',
  'src/views/buz/order/components/ShipDialog.vue',
  'src/views/buz/order/components/ReviewDialog.vue',
  'src/views/buz/order/components/RefundCreateDialog.vue',
  'src/views/buz/order/components/OrderDetailDialog.vue',
  'src/views/buz/order/components/ExchangeProductSelectDialog.vue',
  'src/views/buz/customer/level/components/LevelFormDialog.vue',
  'src/views/buz/customer/components/PointsAdjustDialog.vue',
  'src/views/buz/customer/components/LevelAdjustDialog.vue',
  'src/views/buz/customer/components/CustomerFormDialog.vue',
  'src/views/buz/customer/components/AddressManager.vue',
  'src/views/buz/banner/components/BannerFormDialog.vue',
  'src/views/basic/user/components/ResetPasswordDialog.vue',
  'src/views/basic/role/components/RoleFormDialog.vue',
  'src/views/basic/menu/components/MenuFormDialog.vue',
  'src/views/basic/menu/components/IconSelectDialog.vue',
  // workflow components
  'src/components/workflow/drawers/ServiceDrawer.vue',
  'src/components/workflow/drawers/PromoterDrawer.vue',
  'src/components/workflow/drawers/NotificationDrawer.vue',
  'src/components/workflow/drawers/CopyerDrawer.vue',
  'src/components/workflow/drawers/ConditionDrawer.vue',
  'src/components/workflow/drawers/ApproverDrawer.vue',
  'src/components/workflow/FlowDesigner/ErrorDialog.vue',
]

function processFile(filePath) {
  const fullPath = path.resolve(__dirname, '..', filePath)
  if (!fs.existsSync(fullPath)) {
    console.log(`[跳过] 文件不存在: ${filePath}`)
    return
  }

  let content = fs.readFileSync(fullPath, 'utf-8')
  const originalContent = content

  // 检查是否是 drawer 还是 dialog
  const isDrawer = content.includes('<el-drawer')
  const isDialog = content.includes('<el-dialog')

  if (!isDrawer && !isDialog) {
    console.log(`[跳过] 非 Dialog/Drawer: ${filePath}`)
    return
  }

  // 提取 title 属性值
  const titleMatch = content.match(/:title="([^"]+)"/) || content.match(/title="([^"]+)"/)
  const titleValue = titleMatch ? titleMatch[1] : ''

  // 提取 footer 内容
  const footerRegex = /<template\s+#footer[^>]*>([\s\S]*?)<\/template>/
  const footerMatch = content.match(footerRegex)

  if (!footerMatch) {
    console.log(`[跳过] 无 footer: ${filePath}`)
    return
  }

  const footerContent = footerMatch[1].trim()
    .replace(/<span class="dialog-footer">/g, '')
    .replace(/<\/span>/g, '')
    .replace(/<div class="drawer-footer">/g, '')
    .replace(/<\/div>/g, '')
    .trim()

  // 提取按钮
  const buttonMatches = [...footerContent.matchAll(/<el-button[^>]*>[\s\S]*?<\/el-button>/g)]
  if (buttonMatches.length === 0) {
    console.log(`[跳过] 无按钮: ${filePath}`)
    return
  }

  const buttons = buttonMatches.map(m => m[0]).join('\n          ')

  // 创建新的 header
  const newHeader = isDrawer
    ? `<template #header>
      <div class="drawer-header">
        <span class="drawer-title">${titleValue}</span>
        <div class="drawer-actions">
          ${buttons}
        </div>
      </div>
    </template>`
    : `<template #header>
      <div class="dialog-header">
        <span class="dialog-title">${titleValue}</span>
        <div class="dialog-actions">
          ${buttons}
        </div>
      </div>
    </template>`

  // 删除 title 属性
  content = content.replace(/:title="[^"]+"\s*/g, '')
  content = content.replace(/title="[^"]+"\s*/g, '')

  // 删除 footer
  content = content.replace(footerRegex, '')

  // 在 <el-dialog 或 <el-drawer 后插入新的 header
  if (isDialog) {
    content = content.replace(/(<el-dialog[^>]*>)/, `$1\n    ${newHeader}\n`)
  } else {
    content = content.replace(/(<el-drawer[^>]*>)/, `$1\n    ${newHeader}\n`)
  }

  // 添加样式（如果不存在）
  const styleClass = isDrawer ? 'drawer-header' : 'dialog-header'
  if (!content.includes(`.${styleClass}`)) {
    const styleContent = isDrawer
      ? `\n\n<style scoped lang="scss">
.drawer-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;

  .drawer-title {
    font-size: 18px;
    font-weight: 500;
    color: #303133;
  }

  .drawer-actions {
    display: flex;
    gap: 8px;
  }
}
</style>`
      : `\n\n<style scoped lang="scss">
.dialog-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;

  .dialog-title {
    font-size: 18px;
    font-weight: 500;
    color: #303133;
  }

  .dialog-actions {
    display: flex;
    gap: 8px;
    margin-right: 30px;
  }
}
</style>`

    // 在文件末尾添加样式（如果没有 style 标签）或合并到现有 style
    if (!content.includes('<style')) {
      content = content.trimEnd() + styleContent
    }
  }

  if (content !== originalContent) {
    fs.writeFileSync(fullPath, content, 'utf-8')
    console.log(`[完成] ${filePath}`)
  } else {
    console.log(`[无变化] ${filePath}`)
  }
}

// 处理所有文件
console.log('开始处理...\n')
files.forEach(processFile)
console.log('\n处理完成!')
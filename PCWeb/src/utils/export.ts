/**
 * 表格导出工具
 */
import * as XLSX from 'xlsx'
import ExcelJS from 'exceljs'
import type { TableColumn } from '@/components/DraggableTable/index.vue'

/**
 * 导出配置
 */
export interface ExportConfig {
  fileName?: string
  sheetName?: string
  columns: TableColumn[]
  data: any[]
  customLabels?: Record<string, string>
}

/**
 * 导出表格数据到 Excel
 */
export function exportToExcel(config: ExportConfig) {
  const {
    fileName = `导出数据_${new Date().toLocaleDateString().replace(/\//g, '-')}`,
    sheetName = 'Sheet1',
    columns,
    data,
    customLabels,
  } = config

  // 构建导出数据
  const exportList = data.map((row, index) => {
    const item: Record<string, any> = { 序号: index + 1 }

    columns.forEach((col) => {
      const label = customLabels?.[col.prop] || col.label
      let value = row[col.prop]

      // 处理数组
      if (Array.isArray(value)) {
        value = value.join(', ')
      }

      // 处理布尔值
      if (typeof value === 'boolean') {
        value = value ? '是' : '否'
      }

      // 处理状态
      if (col.prop === 'status' && typeof value === 'number') {
        value = value === 1 ? '启用' : '禁用'
      }

      item[label] = value ?? '-'
    })

    return item
  })

  // 创建工作表
  const worksheet = XLSX.utils.json_to_sheet(exportList)
  const workbook = XLSX.utils.book_new()
  XLSX.utils.book_append_sheet(workbook, worksheet, sheetName)

  // 设置列宽（基于标签长度估算，最小10，最大25）
  const colWidths = [
    { wch: 6 }, // 序号
    ...columns.map((col) => {
      // 基于标签长度估算，最小 10，最大 25
      const width = Math.min(Math.max(col.label.length + 4, 10), 25)
      return { wch: width }
    }),
  ]
  worksheet['!cols'] = colWidths

  // 导出文件
  XLSX.writeFile(workbook, `${fileName}.xlsx`)
}

/**
 * 导出选中的表格数据
 */
export function exportSelectedToExcel(
  columns: TableColumn[],
  selectedRows: any[],
  fileName?: string
) {
  if (selectedRows.length === 0) {
    return false
  }

  exportToExcel({
    columns,
    data: selectedRows,
    fileName,
  })

  return true
}

/**
 * 导出图表和数据到 Excel（带图片）
 */
export async function exportChartAndDataToExcel(config: {
  fileName?: string
  chartImage?: string | null // 图表 base64 图片（不含前缀）
  chartTitle?: string
  columns: TableColumn[]
  data: any[]
  summary?: { total: number; count: number; avg: number } | null
  sheetName?: string
}) {
  const {
    fileName = `报表导出_${new Date().toLocaleDateString().replace(/\//g, '-')}`,
    chartImage,
    chartTitle = '图表',
    columns,
    data,
    summary,
    sheetName = '报表数据',
  } = config

  const workbook = new ExcelJS.Workbook()
  const worksheet = workbook.addWorksheet(sheetName)

  // 设置标题
  worksheet.mergeCells('A1:F1')
  const titleCell = worksheet.getCell('A1')
  titleCell.value = fileName
  titleCell.font = { size: 16, bold: true }
  titleCell.alignment = { horizontal: 'center' }
  worksheet.getRow(1).height = 30

  // 添加图表图片
  if (chartImage) {
    try {
      // 添加图表标题
      worksheet.getCell('A3').value = chartTitle
      worksheet.getCell('A3').font = { size: 12, bold: true }

      // 浏览器环境：将 base64 转换为 ArrayBuffer
      const binaryString = atob(chartImage)
      const bytes = new Uint8Array(binaryString.length)
      for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i)
      }
      const imageBuffer = bytes.buffer as ArrayBuffer

      // 添加图片到工作表
      const imageId = workbook.addImage({
        buffer: imageBuffer,
        extension: 'png',
      })

      worksheet.addImage(imageId, {
        tl: { col: 0, row: 4 },
        ext: { width: 500, height: 280 },
      })

      // 设置数据表起始行（图表高度约 15 行）
      const dataStartRow = 20
      await addDataTable(worksheet, columns, data, dataStartRow, summary)
    } catch (error) {
      console.error('添加图表图片失败:', error)
      // 如果添加图片失败，只导出数据表
      await addDataTable(worksheet, columns, data, 3, summary)
    }
  } else {
    // 无图表，直接导出数据表
    await addDataTable(worksheet, columns, data, 3, summary)
  }

  // 导出文件
  const buffer = await workbook.xlsx.writeBuffer()
  const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
  const url = window.URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = `${fileName}.xlsx`
  link.click()
  window.URL.revokeObjectURL(url)
}

/**
 * 添加数据表到工作表
 */
async function addDataTable(
  worksheet: ExcelJS.Worksheet,
  columns: TableColumn[],
  data: any[],
  startRow: number,
  summary?: { total: number; count: number; avg: number } | null
) {
  // 添加数据汇总
  if (summary) {
    worksheet.getCell(`A${startRow}`).value = '数据汇总'
    worksheet.getCell(`A${startRow}`).font = { size: 12, bold: true }
    worksheet.getCell(`B${startRow}`).value = `总计: ${summary.total}`
    worksheet.getCell(`C${startRow}`).value = `数量: ${summary.count}`
    worksheet.getCell(`D${startRow}`).value = `平均: ${summary.avg}`
    startRow += 2
  }

  // 数据表标题
  worksheet.getCell(`A${startRow}`).value = '数据明细'
  worksheet.getCell(`A${startRow}`).font = { size: 12, bold: true }
  startRow += 1

  // 表头
  const headerRow = worksheet.getRow(startRow)
  headerRow.values = ['序号', ...columns.map(col => col.label)]
  headerRow.font = { bold: true }
  headerRow.alignment = { horizontal: 'center' }
  headerRow.fill = {
    type: 'pattern',
    pattern: 'solid',
    fgColor: { argb: 'FFE0E0E0' },
  }

  // 设置列宽
  worksheet.columns = [
    { width: 8 }, // 序号
    ...columns.map(col => ({ width: Math.max(col.width || 15, 10) })),
  ]

  // 数据行
  data.forEach((row, index) => {
    const dataRow = worksheet.getRow(startRow + 1 + index)
    const values: any[] = [index + 1]
    columns.forEach(col => {
      let value = row[col.prop]
      if (Array.isArray(value)) {
        value = value.join(', ')
      }
      if (typeof value === 'boolean') {
        value = value ? '是' : '否'
      }
      values.push(value ?? '-')
    })
    dataRow.values = values
    dataRow.alignment = { horizontal: 'center' }
  })

  // 设置表格样式
  const endRow = startRow + data.length
  for (let i = startRow; i <= endRow; i++) {
    worksheet.getRow(i).height = 20
  }
}
/**
 * GUID 工具函数
 */

/**
 * 生成 GUID 字符串
 * @returns GUID 字符串，格式：xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
 */
export function generateGuid(): string {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0
    const v = c === 'x' ? r : (r & 0x3 | 0x8)
    return v.toString(16)
  })
}

/**
 * 验证是否为有效的 GUID
 * @param guid 要验证的字符串
 * @returns 是否为有效的 GUID
 */
export function isValidGuid(guid: string): boolean {
  const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i
  return guidRegex.test(guid)
}
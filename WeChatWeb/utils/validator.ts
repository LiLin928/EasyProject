// utils/validator.ts

/** 验证工具类 */
export class Validator {

  /** 验证手机号 */
  static phone(phone: string): boolean {
    return /^1[3-9]\d{9}$/.test(phone);
  }

  /** 验证身份证号 */
  static idCard(idCard: string): boolean {
    return /^[1-9]\d{5}(18|19|20)\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{3}[\dXx]$/.test(idCard);
  }

  /** 验证邮箱 */
  static email(email: string): boolean {
    return /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(email);
  }

  /** 验证是否为空 */
  static isEmpty(value: any): boolean {
    if (value === null || value === undefined) return true;
    if (typeof value === 'string') return value.trim() === '';
    if (Array.isArray(value)) return value.length === 0;
    if (typeof value === 'object') return Object.keys(value).length === 0;
    return false;
  }

  /** 验证是否为数字 */
  static isNumber(value: any): boolean {
    return typeof value === 'number' && !isNaN(value);
  }

  /** 验证是否为正整数 */
  static isPositiveInteger(value: any): boolean {
    return Number.isInteger(value) && value > 0;
  }

  /** 验证价格范围 */
  static priceRange(price: number, min: number = 0, max: number = 999999): boolean {
    return this.isNumber(price) && price >= min && price <= max;
  }

  /** 验证数量范围 */
  static countRange(count: number, min: number = 1, max: number = 999): boolean {
    return this.isPositiveInteger(count) && count >= min && count <= max;
  }

  /** 验证地址信息完整性 */
  static address(address: { name: string; phone: string; province: string; city: string; district: string; detail: string }): { valid: boolean; errors: string[] } {
    const errors: string[] = [];

    if (this.isEmpty(address.name)) errors.push('请输入收货人姓名');
    if (!this.phone(address.phone)) errors.push('请输入正确的手机号');
    if (this.isEmpty(address.province)) errors.push('请选择省份');
    if (this.isEmpty(address.city)) errors.push('请选择城市');
    if (this.isEmpty(address.district)) errors.push('请选择区县');
    if (this.isEmpty(address.detail)) errors.push('请输入详细地址');

    return { valid: errors.length === 0, errors };
  }
}
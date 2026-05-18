// types/user.ts

/** 用户信息 */
export interface UserInfo {
  id: string
  userName: string
  realName?: string
  avatar?: string
  email?: string
  phone?: string
  status: number
  createTime?: string
  roles?: string[]
  roleIds?: string[]
  permissions?: string[]
}

/** 登录参数 */
export interface LoginParams {
  username: string
  password: string
}

/** 登录响应 */
export interface LoginResponse {
  accessToken: string
  refreshToken: string
  expiresIn: number
  user: UserInfo
}

/** 修改密码参数 */
export interface ChangePasswordParams {
  oldPassword: string
  newPassword: string
  confirmPassword: string
}
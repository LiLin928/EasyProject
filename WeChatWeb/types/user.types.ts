// types/user.types.ts

import { IBaseEntity } from './common.types';

/** 用户信息 */
export interface IUserInfo extends IBaseEntity {
  id: string;
  openId?: string;
  nickname?: string;
  avatarUrl?: string;
  phone?: string;
  gender?: 0 | 1 | 2; // 0:未知 1:男 2:女
}

/** 登录用户（含 token） */
export interface IUser {
  id: string;
  token: string;
  userInfo: IUserInfo;
  code?: string; // 微信登录 code
}

/** 手机号+密码登录参数 */
export interface IPhoneLoginParams {
  phone: string;
  password: string;
}

/** 微信手机号快速验证参数 */
export interface IWxPhoneLoginParams {
  code: string;
  encryptedData: string;
  iv: string;
}
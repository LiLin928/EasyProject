// stores/user.store.ts

import { BaseStore } from './base.store';
import { IUser, IUserInfo } from '../types/index';
import { StorageUtil } from '../utils/storage';

interface IUserState {
  user: IUser | null;
  userInfo: IUserInfo | null;
  isLoggedIn: boolean;
  token: string | null;
}

export class UserStore extends BaseStore<IUserState> {
  private static instance: UserStore;

  static getInstance(): UserStore {
    if (!UserStore.instance) {
      UserStore.instance = new UserStore();
    }
    return UserStore.instance;
  }

  protected getInitialState(): IUserState {
    const token = StorageUtil.get<string>('token');
    const userInfo = StorageUtil.get<IUserInfo>('userInfo');
    return {
      user: null,
      userInfo,
      isLoggedIn: !!token,
      token,
    };
  }

  /** 设置用户登录信息 */
  setUser(user: IUser): void {
    StorageUtil.set('token', user.token);
    StorageUtil.set('userInfo', user.userInfo);
    this.setState({
      user,
      userInfo: user.userInfo,
      isLoggedIn: true,
      token: user.token,
    });
  }

  /** 更新用户信息 */
  updateUserInfo(info: Partial<IUserInfo>): void {
    const current = this.state.userInfo;
    if (current) {
      const updated = { ...current, ...info };
      StorageUtil.set('userInfo', updated);
      this.setState({ userInfo: updated });
    }
  }

  /** 退出登录 */
  logout(): void {
    StorageUtil.remove('token');
    StorageUtil.remove('userInfo');
    this.setState({
      user: null,
      userInfo: null,
      isLoggedIn: false,
      token: null,
    });
  }
}

/** 导出单例实例 */
export const userStore = UserStore.getInstance();
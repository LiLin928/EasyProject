// mock-data/user.mock.ts

import { IUser, IUserInfo } from '../types/index';

export const MockUser = {
  defaultUser: {
    id: '550e8400-e29b-41d4-a716-446655440201',
    token: 'mock_token_' + Date.now(),
    userInfo: {
      id: '550e8400-e29b-41d4-a716-446655440201',
      openId: 'mock_openid_001',
      nickname: '微信用户',
      avatarUrl: '/static/images/default-avatar.png',
      phone: '',
      gender: 0,
      createdAt: Date.now(),
    } as IUserInfo,
  } as IUser,

  defaultUserInfo: {
    id: '550e8400-e29b-41d4-a716-446655440201',
    nickname: '微信用户',
    avatarUrl: '/static/images/default-avatar.png',
    gender: 0,
  } as IUserInfo,
};
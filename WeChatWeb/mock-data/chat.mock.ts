// mock-data/chat.mock.ts

import { IChatMessage, IChatSession, SenderType, MessageType, ChatSessionStatus } from '../types/index';

/** 生成 GUID */
function generateGuid(): string {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
    const r = (Math.random() * 16) | 0;
    const v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

/** Mock 会话数据 */
export const mockChatSession: IChatSession = {
  sessionId: generateGuid(),
  status: ChatSessionStatus.Active,
  unreadCount: 2,
  lastMessage: '您好，请问有什么可以帮您？',
  lastMessageTime: Date.now() - 3600000,
  createTime: Date.now() - 86400000 * 7,
};

/** Mock 消息数据 */
export const mockChatMessages: IChatMessage[] = [
  {
    messageId: generateGuid(),
    senderType: SenderType.Staff,
    senderName: '客服小王',
    messageType: MessageType.Text,
    content: '您好，欢迎来到鲜花商城客服中心，请问有什么可以帮您？',
    duration: 0,
    createTime: Date.now() - 86400000 * 7,
    isRead: true,
  },
  {
    messageId: generateGuid(),
    senderType: SenderType.Customer,
    senderName: '张三',
    messageType: MessageType.Text,
    content: '我想问一下玫瑰花的配送时间',
    duration: 0,
    createTime: Date.now() - 86400000 * 7 + 60000,
    isRead: true,
  },
  {
    messageId: generateGuid(),
    senderType: SenderType.Staff,
    senderName: '客服小王',
    messageType: MessageType.Text,
    content: '玫瑰花一般下单后当天或次日配送，具体配送时间您可以在下单时选择。',
    duration: 0,
    createTime: Date.now() - 86400000 * 7 + 120000,
    isRead: true,
  },
  {
    messageId: generateGuid(),
    senderType: SenderType.Customer,
    senderName: '张三',
    messageType: MessageType.Image,
    content: '/static/images/mock-flower.png',
    duration: 0,
    createTime: Date.now() - 86400000 * 2,
    isRead: true,
  },
  {
    messageId: generateGuid(),
    senderType: SenderType.Customer,
    senderName: '张三',
    messageType: MessageType.Text,
    content: '请问这款花束多少钱？',
    duration: 0,
    createTime: Date.now() - 86400000 * 2 + 30000,
    isRead: true,
  },
  {
    messageId: generateGuid(),
    senderType: SenderType.Staff,
    senderName: '客服小王',
    messageType: MessageType.Text,
    content: '这款是「浪漫玫瑰」花束，售价299元，包含11支红玫瑰，非常受欢迎的一款。',
    duration: 0,
    createTime: Date.now() - 86400000 * 2 + 60000,
    isRead: true,
  },
  {
    messageId: generateGuid(),
    senderType: SenderType.Staff,
    senderName: '客服小王',
    messageType: MessageType.Text,
    content: '您好，请问有什么可以帮您？',
    duration: 0,
    createTime: Date.now() - 3600000,
    isRead: false,
  },
];

/** 获取 Mock 会话 */
export function getMockChatSession(): Promise<IChatSession> {
  return new Promise(resolve => {
    setTimeout(() => resolve(mockChatSession), 200);
  });
}

/** 获取 Mock 历史消息 */
export function getMockChatHistory(pageIndex: number, pageSize: number): Promise<{ list: IChatMessage[]; total: number }> {
  return new Promise(resolve => {
    const start = (pageIndex - 1) * pageSize;
    const list = mockChatMessages.slice(start, start + pageSize).reverse();
    setTimeout(() => resolve({ list, total: mockChatMessages.length }), 200);
  });
}
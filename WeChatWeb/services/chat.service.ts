// services/chat.service.ts

import { StorageUtil } from '../utils/storage';
import { Http } from '../utils/http';
import {
  IChatSession,
  IChatMessage,
  IChatMessageQuery,
  IChatMessageResponse,
  ChatSessionStatus,
  WsConnectionState,
} from '../types/index';
import { ChatAdapter } from '../adapters/chat.adapter';
import { envConfig } from '../config/env.dev';

/** 后端会话响应格式 */
interface IChatSessionResponse {
  sessionId: string;
  status: number;
  unreadCount: number;
  lastMessage: string;
  lastMessageTime: number;
  createTime: number;
}

/** 后端消息响应格式 */
interface IChatMessageBackend {
  messageId: string;
  senderType: number;
  senderName: string;
  messageType: number;
  content: string;
  duration: number;
  createTime: number;
  isRead: boolean;
}

/** 后端分页响应 */
interface IPageResponse<T> {
  items: T[];
  total: number;
  pageIndex: number;
  pageSize: number;
}

/** 聊天服务 - 管理会话和消息 */
export class ChatService {
  private static instance: ChatService;
  private chatAdapter: ChatAdapter;
  private currentSession: IChatSession | null = null;

  /** 是否使用 Mock 模式 - 与主 adapter 配置同步 */
  private useMock = false;

  static getInstance(): ChatService {
    if (!ChatService.instance) {
      ChatService.instance = new ChatService();
    }
    return ChatService.instance;
  }

  constructor() {
    this.chatAdapter = new ChatAdapter();
  }

  /** 创建或获取会话 */
  async getOrCreateSession(): Promise<IChatSession> {
    if (this.useMock) {
      return this.getMockSession();
    }

    // 真实 API 模式
    const response = await Http.post<IChatSessionResponse>('/chat/session');
    this.currentSession = {
      sessionId: response.sessionId,
      status: response.status === 1 ? ChatSessionStatus.Active : ChatSessionStatus.Closed,
      unreadCount: response.unreadCount,
      lastMessage: response.lastMessage,
      lastMessageTime: response.lastMessageTime,
      createTime: response.createTime,
    };
    return this.currentSession;
  }

  /** Mock 会话（备用） */
  private async getMockSession(): Promise<IChatSession> {
    return {
      sessionId: 'mock-session-id',
      status: ChatSessionStatus.Active,
      unreadCount: 0,
      lastMessage: '',
      lastMessageTime: Date.now(),
      createTime: Date.now(),
    };
  }

  /** 获取当前会话 */
  getCurrentSession(): IChatSession | null {
    return this.currentSession;
  }

  /** 获取历史消息 */
  async getHistoryMessages(params: IChatMessageQuery): Promise<IChatMessageResponse> {
    if (this.useMock) {
      return { list: [], total: 0, pageIndex: params.pageIndex, pageSize: params.pageSize };
    }

    // 真实 API 模式
    const response = await Http.get<IPageResponse<IChatMessageBackend>>('/chat/messages', {
      sessionId: params.sessionId,
      pageIndex: params.pageIndex,
      pageSize: params.pageSize,
    });

    const messages: IChatMessage[] = response.items.map(msg => ({
      messageId: msg.messageId,
      senderType: msg.senderType,
      senderName: msg.senderName,
      messageType: msg.messageType,
      content: msg.content,
      duration: msg.duration,
      createTime: msg.createTime,
      isRead: msg.isRead,
    }));

    return {
      list: messages,
      total: response.total,
      pageIndex: params.pageIndex,
      pageSize: params.pageSize,
    };
  }

  /** 标记消息已读 */
  async markAsRead(sessionId: string): Promise<void> {
    if (this.useMock) {
      if (this.currentSession) {
        this.currentSession.unreadCount = 0;
      }
      return;
    }

    // 真实 API 模式
    await Http.post<void>('/chat/read', { sessionId });
    if (this.currentSession) {
      this.currentSession.unreadCount = 0;
    }
  }

  /** 连接 WebSocket */
  connectWebSocket(onMessage: (msg: IChatMessage) => void, onStateChange: (state: WsConnectionState) => void): void {
    const token = StorageUtil.get(envConfig.tokenKey) as string || '';
    this.chatAdapter.connect(token, onMessage, onStateChange);
  }

  /** 发送消息 */
  sendMessage(sessionId: string, messageType: number, content: string, duration: number = 0): void {
    this.chatAdapter.send({
      type: 'message',
      sessionId,
      messageType,
      content,
      duration,
    });
  }

  /** 断开 WebSocket */
  disconnectWebSocket(): void {
    this.chatAdapter.close();
  }

  /** 获取 WebSocket 连接状态 */
  getConnectionState(): string {
    return this.chatAdapter.getState();
  }

  /** 上传图片 */
  async uploadImage(filePath: string): Promise<string> {
    if (this.useMock) {
      return filePath;
    }
    // 真实 API：调用上传接口
    return new Promise((resolve, reject) => {
      wx.uploadFile({
        url: `${envConfig.baseUrl}/chat/upload`,
        filePath,
        name: 'file',
        header: {
          Authorization: `Bearer ${StorageUtil.get(envConfig.tokenKey)}`,
        },
        success: (res) => {
          const data = JSON.parse(res.data);
          if (data.code === 200) {
            resolve(data.data.url);
          } else {
            reject(new Error(data.message));
          }
        },
        fail: (err) => {
          reject(err);
        },
      });
    });
  }

  /** 上传语音 */
  async uploadVoice(filePath: string): Promise<string> {
    return this.uploadImage(filePath);
  }
}

export const chatService = ChatService.getInstance();
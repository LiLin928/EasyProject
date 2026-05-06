// adapters/chat.adapter.ts

import {
  IWsSendMessage,
  IWsReceiveMessage,
  IChatMessage,
  WsConnectionState,
  SenderType,
  MessageType,
} from '../types/index';
import { envConfig } from '../config/env.dev';

/** WebSocket 事件回调 */
interface WsCallbacks {
  onMessage: (msg: IChatMessage) => void;
  onStateChange: (state: WsConnectionState) => void;
}

/** 聊天 WebSocket 适配器 - 支持 Mock 和真实模式 */
export class ChatAdapter {
  private socket: WechatMiniprogram.SocketTask | null = null;
  private callbacks: WsCallbacks | null = null;
  private connectionState: WsConnectionState = 'disconnected';
  private reconnectCount = 0;
  private maxReconnect = 5;
  private reconnectTimer: ReturnType<typeof setTimeout> | null = null;
  private token: string = '';

  /** 是否使用 Mock 模式 - 与主 adapter 配置同步 */
  private useMock = false;

  /** 获取连接状态 */
  getState(): WsConnectionState {
    return this.connectionState;
  }

  /** 连接 WebSocket */
  connect(token: string, onMessage: (msg: IChatMessage) => void, onStateChange: (state: WsConnectionState) => void): void {
    this.token = token;
    this.callbacks = { onMessage, onStateChange };
    this.reconnectCount = 0;

    if (this.useMock) {
      this.connectMock();
    } else {
      this.connectReal();
    }
  }

  /** Mock 模式连接 */
  private connectMock(): void {
    this.updateState('connecting');

    // 模拟连接延迟
    setTimeout(() => {
      this.updateState('connected');

      // 模拟系统消息
      const systemMsg: IChatMessage = {
        messageId: this.generateId(),
        senderType: SenderType.Staff,
        senderName: '系统',
        messageType: MessageType.Text,
        content: '客服已上线，欢迎咨询',
        duration: 0,
        createTime: Date.now(),
        isRead: true,
      };
      this.callbacks?.onMessage(systemMsg);
    }, 500);
  }

  /** 真实 WebSocket 连接 */
  private connectReal(): void {
    this.updateState('connecting');

    // WebSocket 路径是 /ws/chat，不是 /api/wechat/ws/chat
    // baseUrl 是 http://localhost:7600/api/wechat，需要去掉 /api/wechat
    const wsBaseUrl = envConfig.baseUrl.replace('http', 'ws').replace('/api/wechat', '');
    const wsUrl = `${wsBaseUrl}/ws/chat?token=${this.token}`;

    this.socket = wx.connectSocket({
      url: wsUrl,
      success: () => {
        console.log('WebSocket 连接发起成功');
      },
      fail: (err) => {
        console.error('WebSocket 连接失败:', err);
        this.updateState('failed');
        this.tryReconnect();
      },
    });

    this.socket.onOpen(() => {
      this.reconnectCount = 0;
      this.updateState('connected');
    });

    this.socket.onMessage((res) => {
      try {
        const data: IWsReceiveMessage = JSON.parse(res.data as string);
        this.handleReceiveMessage(data);
      } catch (error) {
        console.error('解析消息失败:', error);
      }
    });

    this.socket.onClose(() => {
      this.updateState('disconnected');
      this.tryReconnect();
    });

    this.socket.onError((err) => {
      console.error('WebSocket 错误:', err);
      this.updateState('failed');
    });
  }

  /** 发送消息 */
  send(message: IWsSendMessage): void {
    if (this.connectionState !== 'connected') {
      console.warn('WebSocket 未连接，无法发送消息');
      return;
    }

    if (this.useMock) {
      this.sendMock(message);
    } else {
      this.socket?.send({
        data: JSON.stringify(message),
        success: () => {
          // 发送成功，本地回显
          const localMsg: IChatMessage = {
            messageId: this.generateId(),
            senderType: SenderType.Customer,
            senderName: '我',
            messageType: message.messageType,
            content: message.content,
            duration: message.duration,
            createTime: Date.now(),
            isRead: true,
          };
          this.callbacks?.onMessage(localMsg);
        },
        fail: (err) => {
          console.error('发送失败:', err);
        },
      });
    }
  }

  /** Mock 模式发送 */
  private sendMock(message: IWsSendMessage): void {
    // 先回显自己发送的消息
    const myMsg: IChatMessage = {
      messageId: this.generateId(),
      senderType: SenderType.Customer,
      senderName: '我',
      messageType: message.messageType,
      content: message.content,
      duration: message.duration,
      createTime: Date.now(),
      isRead: true,
    };
    this.callbacks?.onMessage(myMsg);

    // 模拟客服自动回复
    setTimeout(() => {
      const replyMsg: IChatMessage = {
        messageId: this.generateId(),
        senderType: SenderType.Staff,
        senderName: '客服',
        messageType: MessageType.Text,
        content: this.getMockReply(message.content),
        duration: 0,
        createTime: Date.now(),
        isRead: false,
      };
      this.callbacks?.onMessage(replyMsg);
    }, 800 + Math.random() * 1500);
  }

  /** Mock 自动回复 */
  private getMockReply(content: string): string {
    const replies = [
      '您好，感谢您的咨询，请问有什么可以帮您？',
      '收到您的消息，我们会尽快处理。',
      '好的，请稍等，我帮您查询一下。',
      '这个问题我帮您确认后回复您。',
      '感谢您的耐心等待，请问还有其他问题吗？',
    ];
    return replies[Math.floor(Math.random() * replies.length)];
  }

  /** 处理接收消息 */
  private handleReceiveMessage(data: IWsReceiveMessage): void {
    if (data.type === 'message') {
      const msg: IChatMessage = {
        messageId: data.messageId || this.generateId(),
        senderType: data.senderType || SenderType.Staff,
        senderName: data.senderName || '客服',
        messageType: data.messageType || MessageType.Text,
        content: data.content || '',
        duration: data.duration || 0,
        createTime: data.createTime || Date.now(),
        isRead: false,
      };
      this.callbacks?.onMessage(msg);
    } else if (data.type === 'system') {
      const sysMsg: IChatMessage = {
        messageId: this.generateId(),
        senderType: SenderType.Staff,
        senderName: '系统',
        messageType: MessageType.Text,
        content: data.content || '',
        duration: 0,
        createTime: Date.now(),
        isRead: true,
      };
      this.callbacks?.onMessage(sysMsg);
    }
  }

  /** 断线重连 */
  private tryReconnect(): void {
    if (this.reconnectCount >= this.maxReconnect) {
      this.updateState('failed');
      return;
    }

    this.reconnectCount++;
    console.log(`尝试重连 (${this.reconnectCount}/${this.maxReconnect})`);

    this.reconnectTimer = setTimeout(() => {
      this.connect(this.token, this.callbacks!.onMessage, this.callbacks!.onStateChange);
    }, 3000);
  }

  /** 更新连接状态 */
  private updateState(state: WsConnectionState): void {
    this.connectionState = state;
    this.callbacks?.onStateChange(state);
  }

  /** 关闭连接 */
  close(): void {
    if (this.reconnectTimer) {
      clearTimeout(this.reconnectTimer);
      this.reconnectTimer = null;
    }
    this.socket?.close({});
    this.socket = null;
    this.connectionState = 'disconnected';
  }

  /** 生成唯一 ID */
  private generateId(): string {
    return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  }
}
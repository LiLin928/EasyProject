// stores/chat.store.ts

import { BaseStore } from './base.store';
import { IChatMessage, IChatSession, WsConnectionState, SenderType } from '../types/index';

/** 聊天状态 */
interface IChatState {
  /** 当前会话 */
  session: IChatSession | null;
  /** 消息列表 */
  messages: IChatMessage[];
  /** WebSocket 连接状态 */
  connectionState: WsConnectionState;
  /** 是否正在加载 */
  loading: boolean;
  /** 是否有更多历史消息 */
  hasMore: boolean;
}

/** 聊天状态管理 */
export class ChatStore extends BaseStore<IChatState> {
  private static instance: ChatStore;

  static getInstance(): ChatStore {
    if (!ChatStore.instance) {
      ChatStore.instance = new ChatStore();
    }
    return ChatStore.instance;
  }

  constructor() {
    super({
      session: null,
      messages: [],
      connectionState: 'disconnected',
      loading: false,
      hasMore: true,
    });
  }

  protected getInitialState(): IChatState {
    return {
      session: null,
      messages: [],
      connectionState: 'disconnected',
      loading: false,
      hasMore: true,
    };
  }

  // ========== 计算属性 ==========

  /** 获取未读消息数 */
  get unreadCount(): number {
    return this.state.messages.filter(m => !m.isRead && m.senderType === SenderType.Staff).length;
  }

  /** 获取最新消息 */
  get lastMessage(): IChatMessage | null {
    const messages = this.state.messages;
    return messages.length > 0 ? messages[messages.length - 1] : null;
  }

  // ========== 操作方法 ==========

  /** 设置会话 */
  setSession(session: IChatSession | null): void {
    this.setState({ session });
  }

  /** 设置连接状态 */
  setConnectionState(state: WsConnectionState): void {
    this.setState({ connectionState: state });
  }

  /** 添加新消息 */
  addMessage(message: IChatMessage): void {
    const messages = [...this.state.messages, message];
    this.setState({ messages });
  }

  /** 添加历史消息（追加到头部） */
  addHistoryMessages(messages: IChatMessage[]): void {
    const allMessages = [...messages, ...this.state.messages];
    this.setState({ messages: allMessages, hasMore: messages.length > 0 });
  }

  /** 标记所有消息已读 */
  markAllAsRead(): void {
    const messages = this.state.messages.map(m => ({ ...m, isRead: true }));
    this.setState({ messages });
    if (this.state.session) {
      this.setState({
        session: { ...this.state.session, unreadCount: 0 },
      });
    }
  }

  /** 设置加载状态 */
  setLoading(loading: boolean): void {
    this.setState({ loading });
  }

  /** 设置是否有更多消息 */
  setHasMore(hasMore: boolean): void {
    this.setState({ hasMore });
  }

  /** 清空消息 */
  clearMessages(): void {
    this.setState({ messages: [], hasMore: true });
  }

  /** 重置状态 */
  reset(): void {
    this.setState(this.getInitialState());
  }
}

export const chatStore = ChatStore.getInstance();
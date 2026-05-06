// types/chat.types.ts

/** WebSocket 连接状态 */
export type WsConnectionState = 'connecting' | 'connected' | 'disconnected' | 'failed';

/** 会话状态 */
export enum ChatSessionStatus {
  /** 进行中 */
  Active = 0,
  /** 已结束 */
  Closed = 1,
}

/** 消息发送者类型 */
export enum SenderType {
  /** 客户 */
  Customer = 0,
  /** 客服 */
  Staff = 1,
}

/** 消息类型 */
export enum MessageType {
  /** 文字 */
  Text = 0,
  /** 图片 */
  Image = 1,
  /** 语音 */
  Voice = 2,
}

/** 聊天会话信息 */
export interface IChatSession {
  /** 会话ID */
  sessionId: string;
  /** 会话状态 */
  status: ChatSessionStatus;
  /** 客户未读消息数 */
  unreadCount: number;
  /** 最后消息摘要 */
  lastMessage: string;
  /** 最后消息时间戳 */
  lastMessageTime: number;
  /** 创建时间戳 */
  createTime: number;
}

/** 聊天消息 */
export interface IChatMessage {
  /** 消息ID */
  messageId: string;
  /** 发送者类型 */
  senderType: SenderType;
  /** 发送者名称 */
  senderName: string;
  /** 消息类型 */
  messageType: MessageType;
  /** 消息内容（文字/图片URL/语音URL） */
  content: string;
  /** 语音时长（秒） */
  duration: number;
  /** 发送时间戳 */
  createTime: number;
  /** 是否已读 */
  isRead: boolean;
}

/** WebSocket 发送消息格式 */
export interface IWsSendMessage {
  /** 消息类型标识 */
  type: 'message';
  /** 会话ID */
  sessionId: string;
  /** 消息类型 */
  messageType: MessageType;
  /** 消息内容 */
  content: string;
  /** 语音时长 */
  duration: number;
}

/** WebSocket 接收消息格式 */
export interface IWsReceiveMessage {
  /** 消息类型标识 */
  type: 'message' | 'system' | 'error';
  /** 消息ID */
  messageId?: string;
  /** 发送者类型 */
  senderType?: SenderType;
  /** 发送者名称 */
  senderName?: string;
  /** 消息类型 */
  messageType?: MessageType;
  /** 消息内容 */
  content?: string;
  /** 语音时长 */
  duration?: number;
  /** 发送时间戳 */
  createTime?: number;
}

/** 创建/获取会话响应 */
export interface IGetSessionResponse {
  sessionId: string;
  status: number;
  unreadCount: number;
  lastMessage: string;
  lastMessageTime: number;
  createTime: number;
}

/** 历史消息查询参数 */
export interface IChatMessageQuery {
  sessionId: string;
  pageIndex: number;
  pageSize: number;
}

/** 历史消息响应 */
export interface IChatMessageResponse {
  list: IChatMessage[];
  total: number;
  pageIndex: number;
  pageSize: number;
}

/** 聊天页面数据 */
export interface IChatPageData {
  /** 会话信息 */
  session: IChatSession | null;
  /** 消息列表 */
  messages: IChatMessage[];
  /** 连接状态 */
  connectionState: WsConnectionState;
  /** 当前消息类型（0-文字，1-图片，2-语音） */
  currentMessageType: MessageType;
  /** 输入文字内容 */
  inputText: string;
  /** 是否正在录音 */
  isRecording: boolean;
  /** 录音时长 */
  recordDuration: number;
  /** 是否正在加载 */
  loading: boolean;
  /** 是否有更多历史消息 */
  hasMore: boolean;
  /** 当前页码 */
  pageIndex: number;
}
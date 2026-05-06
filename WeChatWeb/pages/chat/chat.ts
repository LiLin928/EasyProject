// pages/chat/chat.ts

import { chatService } from '../../services/chat.service';
import { chatStore } from '../../stores/chat.store';
import { userStore } from '../../stores/index';
import {
  IChatMessage,
  IChatSession,
  WsConnectionState,
  MessageType,
  SenderType,
} from '../../types/index';
import { authService } from '../../services/index';

interface IChatPageData {
  session: IChatSession | null;
  messages: IChatMessage[];
  connectionState: WsConnectionState;
  currentMessageType: MessageType;
  inputText: string;
  isRecording: boolean;
  recordDuration: number;
  loading: boolean;
  hasMore: boolean;
  pageIndex: number;
  scrollTop: number;
  userInfo: { avatarUrl: string } | null;
}

/** 聊天页面 */
Page<IChatPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    session: null,
    messages: [],
    connectionState: 'disconnected',
    currentMessageType: MessageType.Text,
    inputText: '',
    isRecording: false,
    recordDuration: 0,
    loading: false,
    hasMore: true,
    pageIndex: 1,
    scrollTop: 0,
    userInfo: null,
  },

  // 录音管理器
  recorderManager: null as WechatMiniprogram.RecorderManager | null,
  // 录音开始时间
  recordStartTime: 0,
  // 录音文件路径
  recordFilePath: '',
  // 定时器
  recordTimer: null as ReturnType<typeof setInterval> | null,
  // 音频播放器
  audioPlayer: null as WechatMiniprogram.InnerAudioContext | null,

  onLoad() {
    // 检查登录状态
    if (!authService.requireLogin()) {
      return;
    }

    // 加载用户信息
    const userState = userStore.getState();
    this.setData({
      userInfo: userState.userInfo ? { avatarUrl: userState.userInfo.avatarUrl || '' } : null,
    });

    // 初始化录音管理器
    this.recorderManager = wx.getRecorderManager();
    this.initRecorderEvents();

    // 加载会话和连接 WebSocket
    this.initChat();
  },

  onUnload() {
    // 断开 WebSocket
    chatService.disconnectWebSocket();
    chatStore.reset();
    // 清理音频播放器
    if (this.audioPlayer) {
      this.audioPlayer.destroy();
      this.audioPlayer = null;
    }
  },

  onShow() {
    // 标记消息已读
    if (this.data.session) {
      chatService.markAsRead(this.data.session.sessionId);
      chatStore.markAllAsRead();
    }
  },

  /** 初始化聊天 */
  async initChat() {
    try {
      this.setData({ loading: true });

      // 获取会话
      const session = await chatService.getOrCreateSession();
      this.setData({ session });

      // 加载历史消息
      await this.loadHistoryMessages();

      // 连接 WebSocket
      chatService.connectWebSocket(
        this.onReceiveMessage.bind(this),
        this.onConnectionStateChange.bind(this)
      );

      this.setData({ loading: false });
    } catch (error) {
      console.error('初始化聊天失败:', error);
      this.setData({ loading: false });
      wx.showToast({ title: '加载失败', icon: 'none' });
    }
  },

  /** 加载历史消息 */
  async loadHistoryMessages() {
    if (!this.data.session || !this.data.hasMore) return;

    try {
      const result = await chatService.getHistoryMessages({
        sessionId: this.data.session.sessionId,
        pageIndex: this.data.pageIndex,
        pageSize: 20,
      });

      // 添加到消息列表头部
      const messages = [...result.list.reverse(), ...this.data.messages];
      this.setData({
        messages,
        hasMore: result.list.length >= 20,
        pageIndex: this.data.pageIndex + 1,
      });

      // 首次加载滚动到底部
      if (this.data.pageIndex === 2) {
        this.scrollToBottom();
      }
    } catch (error) {
      console.error('加载历史消息失败:', error);
    }
  },

  /** 接收新消息 */
  onReceiveMessage(message: IChatMessage) {
    const messages = [...this.data.messages, message];
    this.setData({ messages });
    this.scrollToBottom();

    // 如果是客服消息且未读，更新未读数
    if (message.senderType === SenderType.Staff && !message.isRead) {
      // 播放提示音
      // wx.playBackgroundAudio() 可考虑使用
    }
  },

  /** 连接状态变化 */
  onConnectionStateChange(state: WsConnectionState) {
    this.setData({ connectionState: state });
  },

  /** 滚动到底部 */
  scrollToBottom() {
    setTimeout(() => {
      this.setData({ scrollTop: this.data.messages.length * 100 + 1000 });
    }, 100);
  },

  // ========== 消息类型切换 ==========

  /** 切换消息类型 */
  switchMessageType(e: WechatMiniprogram.TouchEvent) {
    const { type } = e.currentTarget.dataset;
    this.setData({ currentMessageType: parseInt(type) as MessageType });
  },

  // ========== 文字消息 ==========

  /** 输入文字 */
  onInput(e: WechatMiniprogram.CustomEvent<{ value: string }>) {
    this.setData({ inputText: e.detail.value });
  },

  /** 发送文字消息 */
  sendTextMessage() {
    const { inputText, session } = this.data;
    if (!inputText.trim() || !session) return;

    chatService.sendMessage(session.sessionId, MessageType.Text, inputText.trim());
    this.setData({ inputText: '' });
  },

  // ========== 图片消息 ==========

  /** 选择并发送图片 */
  async sendImageMessage() {
    try {
      const { tempFiles } = await wx.chooseMedia({
        count: 1,
        mediaType: ['image'],
        sourceType: ['album', 'camera'],
      });

      const filePath = tempFiles[0].tempFilePath;
      wx.showLoading({ title: '上传中' });

      // 上传图片
      const imageUrl = await chatService.uploadImage(filePath);

      // 发送消息
      if (this.data.session) {
        chatService.sendMessage(this.data.session.sessionId, MessageType.Image, imageUrl);
      }

      wx.hideLoading();
    } catch (error) {
      wx.hideLoading();
      console.error('发送图片失败:', error);
      wx.showToast({ title: '发送失败', icon: 'none' });
    }
  },

  // ========== 语音消息 ==========

  /** 初始化录音事件 */
  initRecorderEvents() {
    if (!this.recorderManager) return;

    this.recorderManager.onStart(() => {
      this.setData({ isRecording: true, recordDuration: 0 });
      this.recordStartTime = Date.now();
      this.recordTimer = setInterval(() => {
        const duration = Math.floor((Date.now() - this.recordStartTime) / 1000);
        this.setData({ recordDuration: duration });
      }, 1000);
    });

    this.recorderManager.onStop((res) => {
      clearInterval(this.recordTimer);
      this.setData({ isRecording: false });
      this.recordFilePath = res.tempFilePath;
      this.sendVoiceMessage();
    });

    this.recorderManager.onError((err) => {
      clearInterval(this.recordTimer);
      this.setData({ isRecording: false });
      console.error('录音失败:', err);
      wx.showToast({ title: '录音失败', icon: 'none' });
    });
  },

  /** 开始录音 */
  startRecording() {
    if (!this.recorderManager) return;
    this.recorderManager.start({
      format: 'mp3',
      duration: 60000, // 最长60秒
    });
  },

  /** 停止录音 */
  stopRecording() {
    if (!this.recorderManager || !this.data.isRecording) return;

    const duration = Math.floor((Date.now() - this.recordStartTime) / 1000);
    if (duration < 1) {
      this.recorderManager.stop();
      wx.showToast({ title: '录音时间太短', icon: 'none' });
      return;
    }

    this.recorderManager.stop();
  },

  /** 发送语音消息 */
  async sendVoiceMessage() {
    if (!this.recordFilePath || !this.data.session) return;

    try {
      wx.showLoading({ title: '上传中' });

      const duration = Math.floor((Date.now() - this.recordStartTime) / 1000);
      const voiceUrl = await chatService.uploadVoice(this.recordFilePath);

      chatService.sendMessage(this.data.session.sessionId, MessageType.Voice, voiceUrl, duration);

      wx.hideLoading();
      this.recordFilePath = '';
    } catch (error) {
      wx.hideLoading();
      console.error('发送语音失败:', error);
      wx.showToast({ title: '发送失败', icon: 'none' });
    }
  },

  // ========== 消息操作 ==========

  /** 预览图片 */
  previewImage(e: WechatMiniprogram.TouchEvent) {
    const { url } = e.currentTarget.dataset;
    const imageUrls = this.data.messages
      .filter(m => m.messageType === MessageType.Image)
      .map(m => m.content);

    wx.previewImage({
      current: url,
      urls: imageUrls,
    });
  },

  /** 播放语音 */
  playVoice(e: WechatMiniprogram.TouchEvent) {
    const { url } = e.currentTarget.dataset;

    // 创建音频播放器
    if (!this.audioPlayer) {
      this.audioPlayer = wx.createInnerAudioContext();
    }

    this.audioPlayer.src = url;
    this.audioPlayer.play();
  },

  // ========== 滚动加载 ==========

  /** 上拉加载更多历史消息 */
  onReachTop() {
    if (this.data.hasMore && !this.data.loading) {
      this.loadHistoryMessages();
    }
  },
});
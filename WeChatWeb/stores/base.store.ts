// stores/base.store.ts

/** Store 变化监听器 */
type Listener<T> = (state: T) => void;

/** 基础 Store 类 - 提供状态管理和订阅机制 */
export abstract class BaseStore<T extends object> {
  protected state: T;
  protected listeners: Listener<T>[] = [];

  constructor(initialState?: T) {
    // 如果没有传入 initialState，使用 getInitialState
    this.state = initialState ?? this.getInitialState();
  }

  /** 获取当前状态 */
  getState(): T {
    return { ...this.state } as T;
  }

  /** 更新状态 */
  setState(partial: Partial<T>): void {
    this.state = { ...this.state, ...partial };
    this.notify();
  }

  /** 订阅状态变化 */
  subscribe(listener: Listener<T>): () => void {
    this.listeners.push(listener);
    return () => {
      this.listeners = this.listeners.filter(l => l !== listener);
    };
  }

  /** 通知所有监听器 */
  protected notify(): void {
    const state = this.getState();
    this.listeners.forEach(listener => listener(state));
  }

  /** 重置状态 */
  reset(): void {
    this.state = this.getInitialState();
    this.notify();
  }

  /** 获取初始状态（子类实现） */
  protected abstract getInitialState(): T;
}
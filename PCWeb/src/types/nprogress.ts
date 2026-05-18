declare module 'nprogress' {
  interface NProgressOptions {
    minimum?: number
    template?: string
    easing?: string
    speed?: number
    trickleSpeed?: number
    showSpinner?: boolean
    parent?: string
  }

  interface NProgress {
    configure(options: NProgressOptions): NProgress
    set(n: number): NProgress
    inc(n?: number): NProgress
    start(): NProgress
    done(): NProgress
    remove(): void
  }

  const nprogress: NProgress
  export default nprogress
}
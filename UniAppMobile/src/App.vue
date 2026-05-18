<script setup lang="ts">
import { onLaunch, onShow, onHide } from '@dcloudio/uni-app'

onLaunch(() => {
  console.log('App Launch')
})

onShow(() => {
  console.log('App Show')
})

onHide(() => {
  console.log('App Hide')
})
</script>

<style lang="scss">
/* 全局样式 */
page {
  background-color: $u-bg-color;
  font-size: 28rpx;
  color: $u-main-color;
}

/* 禁止按钮点击后出现蓝色边框 */
button::after {
  border: none;
}
</style>
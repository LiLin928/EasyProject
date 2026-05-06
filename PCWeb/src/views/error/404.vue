<template>
  <div class="error-page">
    <div class="error-content">
      <div class="error-code">404</div>
      <div class="error-message">
        <h1>页面未找到</h1>
        <p>抱歉，您访问的页面不存在或已被移除</p>
      </div>
      <div class="error-actions">
        <el-button type="primary" @click="goHome">
          <el-icon><HomeFilled /></el-icon>
          返回首页
        </el-button>
        <el-button @click="goBack">
          <el-icon><Back /></el-icon>
          返回上页
        </el-button>
      </div>
      <div class="error-illustration">
        <svg viewBox="0 0 400 300" width="320" height="240">
          <!-- Background elements -->
          <circle cx="200" cy="150" r="80" fill="#f5f7fa" />
          <circle cx="120" cy="80" r="20" fill="#ecf5ff" />
          <circle cx="300" cy="220" r="30" fill="#f0f9eb" />

          <!-- Main illustration - broken robot/404 -->
          <g transform="translate(140, 70)">
            <!-- Robot head -->
            <rect x="20" y="0" width="80" height="60" rx="10" fill="#409eff" />
            <rect x="30" y="15" width="25" height="20" rx="4" fill="#fff" />
            <rect x="65" y="15" width="25" height="20" rx="4" fill="#fff" />
            <circle cx="42" cy="25" r="5" fill="#409eff" />
            <circle cx="77" cy="25" r="5" fill="#409eff" />
            <rect x="40" y="45" width="40" height="8" rx="4" fill="#fff" opacity="0.5" />

            <!-- Robot body -->
            <rect x="15" y="70" width="90" height="80" rx="8" fill="#79bbff" />
            <rect x="30" y="85" width="60" height="10" rx="4" fill="#fff" opacity="0.3" />
            <rect x="30" y="105" width="60" height="10" rx="4" fill="#fff" opacity="0.3" />
            <rect x="30" y="125" width="35" height="10" rx="4" fill="#fff" opacity="0.3" />

            <!-- Robot arms -->
            <rect x="-15" y="75" width="20" height="50" rx="6" fill="#409eff" />
            <rect x="115" y="75" width="20" height="50" rx="6" fill="#409eff" />

            <!-- Question marks -->
            <text x="45" y="-10" font-size="24" fill="#e6a23c" font-weight="bold">?</text>
            <text x="95" y="-5" font-size="18" fill="#e6a23c" font-weight="bold">?</text>
            <text x="-5" y="60" font-size="20" fill="#e6a23c" font-weight="bold">?</text>
          </g>

          <!-- Decorative dots -->
          <circle cx="80" cy="180" r="4" fill="#409eff" opacity="0.5" />
          <circle cx="320" cy="100" r="4" fill="#409eff" opacity="0.5" />
          <circle cx="350" cy="180" r="3" fill="#67c23a" opacity="0.5" />
          <circle cx="60" cy="240" r="3" fill="#67c23a" opacity="0.5" />
        </svg>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { HomeFilled, Back } from '@element-plus/icons-vue'

const router = useRouter()

// 返回首页
function goHome() {
  router.push('/desktop')
}

// 返回上一页
function goBack() {
  router.go(-1)
}
</script>

<style scoped lang="scss">
.error-page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 60px);
  background-color: #f5f7fa;
  padding: 20px;
}

.error-content {
  text-align: center;
  max-width: 500px;
}

.error-code {
  font-size: 120px;
  font-weight: 700;
  color: #409eff;
  line-height: 1;
  margin-bottom: 20px;
  text-shadow: 2px 2px 4px rgba(64, 158, 255, 0.2);
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.02);
  }
}

.error-message {
  margin-bottom: 40px;

  h1 {
    margin: 0 0 12px;
    font-size: 28px;
    font-weight: 600;
    color: #303133;
  }

  p {
    margin: 0;
    font-size: 16px;
    color: #909399;
  }
}

.error-actions {
  display: flex;
  justify-content: center;
  gap: 16px;
  margin-bottom: 40px;
}

.error-illustration {
  display: flex;
  justify-content: center;
  opacity: 0.9;
}

@media (max-width: 768px) {
  .error-code {
    font-size: 80px;
  }

  .error-message {
    h1 {
      font-size: 22px;
    }

    p {
      font-size: 14px;
    }
  }

  .error-actions {
    flex-direction: column;
    align-items: center;
  }

  .error-illustration {
    svg {
      width: 240px;
      height: 180px;
    }
  }
}
</style>
import { defineConfig, loadEnv } from "vite";
import uni from "@dcloudio/vite-plugin-uni";
import path from "path";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  // 加载 src 目录下的环境变量文件
  const envDir = path.resolve(__dirname, "src");
  const env = loadEnv(mode, envDir, "VITE_");

  return {
    plugins: [uni()],
    envDir: envDir,
    define: {
      // 将环境变量注入到全局
      "import.meta.env.VITE_API_BASE_URL": JSON.stringify(env.VITE_API_BASE_URL || ""),
      "import.meta.env.VITE_APP_TITLE": JSON.stringify(env.VITE_APP_TITLE || ""),
      "import.meta.env.VITE_MOCK_ENABLED": JSON.stringify(env.VITE_MOCK_ENABLED || "false"),
    },
  };
});
# EasyWeChatWeb Dockerfile
# 多阶段构建：构建阶段 + 运行阶段

# ==================== 构建阶段 ====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制所有项目文件
COPY ["EasyWeChatWeb/EasyWeChatWeb.csproj", "EasyWeChatWeb/"]
COPY ["BusinessManager/BusinessManager.csproj", "BusinessManager/"]
COPY ["EasyWeChatModels/EasyWeChatModels.csproj", "EasyWeChatModels/"]
COPY ["CommonManager/CommonManager.csproj", "CommonManager/"]
COPY ["InfrastructureManager/InfrastructureManager.csproj", "InfrastructureManager/"]

# 还原 NuGet 包
RUN dotnet restore "EasyWeChatWeb/EasyWeChatWeb.csproj"

# 复制所有源代码
COPY . .

# 构建项目
WORKDIR "/src/EasyWeChatWeb"
RUN dotnet build "EasyWeChatWeb.csproj" -c Release -o /app/build

# ==================== 发布阶段 ====================
FROM build AS publish
RUN dotnet publish "EasyWeChatWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==================== 运行阶段 ====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 设置时区为中国标准时间
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# 创建日志和上传文件目录
RUN mkdir -p /app/logs /app/uploads

# 复制发布文件
COPY --from=publish /app/publish .

# 暴露端口
EXPOSE 7600

# 设置环境变量
ENV ASPNETCORE_URLS=http://+:7600
ENV ASPNETCORE_ENVIRONMENT=Production

# 启动应用
ENTRYPOINT ["dotnet", "EasyWeChatWeb.dll"]
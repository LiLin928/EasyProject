-- ============================================
-- 一键初始化所有模块测试数据
-- 执行方式: source docs/sql/init_all.sql;
-- 注意：执行前请先执行 docs/database/database.sql 创建表结构
-- 生成时间: 2026-04-15
-- ============================================

USE EasyProject;

-- 基础模块（用户、角色、菜单、用户角色、角色菜单、操作日志）
source docs/sql/basic.sql;

-- 微信核心模块（微信用户、地址、购物车、订单、支付）
source docs/sql/wechat.sql;

-- 商品模块（分类、供应商、商品、规格、SKU、库存、评价）
source docs/sql/product.sql;

-- 会员模块（会员等级、积分记录、收藏分组、用户收藏）
source docs/sql/member.sql;

-- 优惠券模块（优惠券、用户优惠券）
source docs/sql/coupon.sql;

-- 字典模块（字典类型、字典数据）
source docs/sql/dict.sql;

-- 轮播图模块
source docs/sql/banner.sql;

-- 文件模块（文件记录）
source docs/sql/file.sql;

-- 消息模块（消息、消息用户）
source docs/sql/message.sql;

-- 搜索模块（热门关键词）
source docs/sql/search.sql;

-- AntWorkflow模块（流程定义、版本、实例、节点、任务、记录等）
source docs/sql/ant_workflow.sql;

-- 桌面组件模块（桌面组件、用户配置、角色配置）
source sql/desktop_widget.sql;

-- ============================================
-- 完成
-- ============================================
SELECT '========================================' AS '';
SELECT '所有模块测试数据初始化完成！' AS Message;
SELECT '========================================' AS '';

-- 显示各模块数据统计
SELECT 'Basic' AS 模块, (SELECT COUNT(*) FROM User) AS 用户, (SELECT COUNT(*) FROM Role) AS 角色, (SELECT COUNT(*) FROM Menu) AS 菜单, (SELECT COUNT(*) FROM OperateLog) AS 操作日志;
SELECT 'WeChat' AS 模块, (SELECT COUNT(*) FROM WeChatUser) AS 微信用户, (SELECT COUNT(*) FROM Address) AS 地址, (SELECT COUNT(*) FROM Cart) AS 购物车, (SELECT COUNT(*) FROM `Order`) AS 订单, (SELECT COUNT(*) FROM Payment) AS 支付;
SELECT 'Product' AS 模块, (SELECT COUNT(*) FROM Category) AS 分类, (SELECT COUNT(*) FROM Supplier) AS 供应商, (SELECT COUNT(*) FROM Product) AS 商品, (SELECT COUNT(*) FROM ProductSku) AS SKU;
SELECT 'Member' AS 模块, (SELECT COUNT(*) FROM MemberLevel) AS 会员等级, (SELECT COUNT(*) FROM PointsRecord) AS 积分记录, (SELECT COUNT(*) FROM UserFavorite) AS 收藏;
SELECT 'Coupon' AS 模块, (SELECT COUNT(*) FROM Coupon) AS 优惠券, (SELECT COUNT(*) FROM UserCoupon) AS 用户券;
SELECT 'Dict' AS 模块, (SELECT COUNT(*) FROM DictType) AS 字典类型, (SELECT COUNT(*) FROM DictData) AS 字典数据;
SELECT 'Banner' AS 模块, (SELECT COUNT(*) FROM Banner) AS 轮播图;
SELECT 'File' AS 模块, (SELECT COUNT(*) FROM FileRecord) AS 文件;
SELECT 'Message' AS 模块, (SELECT COUNT(*) FROM Message) AS 消息, (SELECT COUNT(*) FROM MessageUser) AS 消息用户;
SELECT 'Search' AS 模块, (SELECT COUNT(*) FROM HotKeyword) AS 关键词;
SELECT 'AntWorkflow' AS 模块, (SELECT COUNT(*) FROM AntWorkflow) AS 流程定义, (SELECT COUNT(*) FROM AntWorkflowInstance) AS 流程实例, (SELECT COUNT(*) FROM AntWorkflowCurrentTask) AS 待办任务;
SELECT 'DesktopWidget' AS 模块, (SELECT COUNT(*) FROM DesktopWidget) AS 组件, (SELECT COUNT(*) FROM UserWidgetConfig) AS 用户配置, (SELECT COUNT(*) FROM RoleWidgetConfig) AS 角色配置;
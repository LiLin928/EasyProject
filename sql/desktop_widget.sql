-- ============================================
-- DesktopWidget模块 - 桌面布局配置表结构
-- 包含 DesktopWidget, UserWidgetConfig, RoleWidgetConfig 表
-- 执行方式: source sql/desktop_widget.sql;
-- 生成时间: 2026-05-18
-- ============================================

USE EasyProject;

-- ============================================
-- 一、桌面组件表
-- ============================================
DROP TABLE IF EXISTS `DesktopWidget`;
CREATE TABLE `DesktopWidget` (
    `Id` CHAR(36) NOT NULL COMMENT '组件ID',
    `Name` VARCHAR(50) NOT NULL COMMENT '组件名称',
    `Type` INT NOT NULL DEFAULT 1 COMMENT '组件类型: 1-统计卡片 2-数据列表 3-图片展示 4-图表统计',
    `Icon` VARCHAR(50) NULL COMMENT '图标名称',
    `DefaultWidth` INT NOT NULL DEFAULT 3 COMMENT '默认宽度(栅格数1-12)',
    `DefaultHeight` INT NOT NULL DEFAULT 120 COMMENT '默认高度(像素)',
    `DataSourceType` INT NOT NULL DEFAULT 1 COMMENT '数据源类型: 1-API接口 2-静态配置 3-实时统计',
    `DataSourceConfig` TEXT NULL COMMENT '数据源配置(JSON)',
    `InteractionConfig` TEXT NULL COMMENT '交互配置(JSON)',
    `Status` INT NOT NULL DEFAULT 1 COMMENT '状态: 0-禁用 1-启用',
    `CreateTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdateTime` DATETIME NULL COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_type` (`Type`),
    INDEX `idx_status` (`Status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='桌面组件表';

-- ============================================
-- 二、用户组件配置表
-- ============================================
DROP TABLE IF EXISTS `UserWidgetConfig`;
CREATE TABLE `UserWidgetConfig` (
    `Id` CHAR(36) NOT NULL COMMENT '配置ID',
    `UserId` CHAR(36) NOT NULL COMMENT '用户ID',
    `WidgetId` CHAR(36) NOT NULL COMMENT '组件ID',
    `Width` INT NOT NULL DEFAULT 3 COMMENT '用户自定义宽度(栅格数)',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用: 0-禁用 1-启用',
    `SortOrder` INT NOT NULL DEFAULT 0 COMMENT '排序序号',
    `CreateTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdateTime` DATETIME NULL COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_user_id` (`UserId`),
    INDEX `idx_widget_id` (`WidgetId`),
    UNIQUE INDEX `idx_user_widget` (`UserId`, `WidgetId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户组件配置表';

-- ============================================
-- 三、角色组件配置表
-- ============================================
DROP TABLE IF EXISTS `RoleWidgetConfig`;
CREATE TABLE `RoleWidgetConfig` (
    `Id` CHAR(36) NOT NULL COMMENT '配置ID',
    `RoleId` CHAR(36) NOT NULL COMMENT '角色ID',
    `WidgetId` CHAR(36) NOT NULL COMMENT '组件ID',
    `SortOrder` INT NOT NULL DEFAULT 0 COMMENT '排序序号',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用: 0-禁用 1-启用',
    `CreateTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_role_id` (`RoleId`),
    INDEX `idx_widget_id` (`WidgetId`),
    UNIQUE INDEX `idx_role_widget` (`RoleId`, `WidgetId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色组件配置表';

-- ============================================
-- 四、初始组件数据
-- ============================================

-- 统计卡片：待办任务
INSERT INTO `DesktopWidget` (`Id`, `Name`, `Type`, `Icon`, `DefaultWidth`, `DefaultHeight`, `DataSourceType`, `DataSourceConfig`, `Status`, `CreateTime`) VALUES
('d0000001-0000-0000-0000-000000000001', '待办任务', 1, 'List', 3, 120, 1, '{"api":"/api/ant_workflow/todo/count","method":"GET"}', 1, NOW());

-- 统计卡片：商品总数
INSERT INTO `DesktopWidget` (`Id`, `Name`, `Type`, `Icon`, `DefaultWidth`, `DefaultHeight`, `DataSourceType`, `DataSourceConfig`, `Status`, `CreateTime`) VALUES
('d0000001-0000-0000-0000-000000000002', '商品总数', 1, 'Goods', 3, 120, 1, '{"api":"/api/product/count","method":"GET"}', 1, NOW());

-- 统计卡片：用户总数
INSERT INTO `DesktopWidget` (`Id`, `Name`, `Type`, `Icon`, `DefaultWidth`, `DefaultHeight`, `DataSourceType`, `DataSourceConfig`, `Status`, `CreateTime`) VALUES
('d0000001-0000-0000-0000-000000000003', '用户总数', 1, 'User', 3, 120, 1, '{"api":"/api/user/count","method":"GET"}', 1, NOW());

-- 统计卡片：订单总数
INSERT INTO `DesktopWidget` (`Id`, `Name`, `Type`, `Icon`, `DefaultWidth`, `DefaultHeight`, `DataSourceType`, `DataSourceConfig`, `Status`, `CreateTime`) VALUES
('d0000001-0000-0000-0000-000000000004', '订单总数', 1, 'ShoppingCart', 3, 120, 1, '{"api":"/api/order/count","method":"GET"}', 1, NOW());

-- 数据列表：待办任务列表
INSERT INTO `DesktopWidget` (`Id`, `Name`, `Type`, `Icon`, `DefaultWidth`, `DefaultHeight`, `DataSourceType`, `DataSourceConfig`, `InteractionConfig`, `Status`, `CreateTime`) VALUES
('d0000002-0000-0000-0000-000000000001', '待办任务列表', 2, 'List', 6, 200, 1, '{"api":"/api/ant_workflow/todo/list","method":"GET","params":{"pageIndex":1,"pageSize":5}}', '{"click":{"action":"navigate","path":"/ant_workflow/runtime/todo"}}', 1, NOW());

-- 数据列表：最新订单
INSERT INTO `DesktopWidget` (`Id`, `Name`, `Type`, `Icon`, `DefaultWidth`, `DefaultHeight`, `DataSourceType`, `DataSourceConfig`, `InteractionConfig`, `Status`, `CreateTime`) VALUES
('d0000002-0000-0000-0000-000000000002', '最新订单', 2, 'ShoppingCart', 6, 200, 1, '{"api":"/api/order/list","method":"GET","params":{"pageIndex":1,"pageSize":5}}', '{"click":{"action":"navigate","path":"/buz/order"}}', 1, NOW());

-- 图片展示：快捷入口
INSERT INTO `DesktopWidget` (`Id`, `Name`, `Type`, `Icon`, `DefaultWidth`, `DefaultHeight`, `DataSourceType`, `DataSourceConfig`, `Status`, `CreateTime`) VALUES
('d0000003-0000-0000-0000-000000000001', '快捷入口', 3, 'Grid', 12, 150, 2, '{"items":[{"icon":"DataAnalysis","name":"报表管理","path":"/report/list"},{"icon":"DataBoard","name":"大屏管理","path":"/screen/list"},{"icon":"Document","name":"日志查询","path":"/ops/log"},{"icon":"Setting","name":"系统设置","path":"/basic/user"}]}', 1, NOW());

-- 图表统计：订单趋势
INSERT INTO `DesktopWidget` (`Id`, `Name`, `Type`, `Icon`, `DefaultWidth`, `DefaultHeight`, `DataSourceType`, `DataSourceConfig`, `Status`, `CreateTime`) VALUES
('d0000004-0000-0000-0000-000000000001', '订单趋势', 4, 'TrendCharts', 6, 300, 3, '{"chartType":"line","metrics":["orderCount","orderAmount"],"timeRange":"7d"}', 1, NOW());

-- 图表统计：商品分类统计
INSERT INTO `DesktopWidget` (`Id`, `Name`, `Type`, `Icon`, `DefaultWidth`, `DefaultHeight`, `DataSourceType`, `DataSourceConfig`, `Status`, `CreateTime`) VALUES
('d0000004-0000-0000-0000-000000000002', '商品分类统计', 4, 'PieChart', 6, 300, 3, '{"chartType":"pie","metrics":["productCount"],"groupBy":"category"}', 1, NOW());

-- ============================================
-- 五、菜单数据（桌面布局设置）
-- ============================================

-- 添加桌面布局设置菜单到基础管理下
INSERT INTO `Menu` (`Id`, `ParentId`, `MenuName`, `MenuCode`, `Path`, `Component`, `Icon`, `Sort`, `Type`, `Status`, `Hidden`, `Affix`, `CreateTime`) VALUES
('b0000000-0000-0000-0000-000000000099', 'a0000000-0000-0000-0000-000000000002', '桌面布局', 'desktop_widget', '/basic/desktop', 'basic/desktop/index', 'Grid', 5, 1, 1, 0, 0, NOW());

-- 组件管理子菜单
INSERT INTO `Menu` (`Id`, `ParentId`, `MenuName`, `MenuCode`, `Path`, `Component`, `Icon`, `Sort`, `Type`, `Status`, `Hidden`, `Affix`, `CreateTime`) VALUES
('c0000000-0000-0000-0000-000000000100', 'b0000000-0000-0000-0000-000000000099', '组件管理', 'desktop_widget_list', '/basic/desktop/widget', 'basic/desktop/widget/index', 'Component', 1, 1, 1, 0, 0, NOW());

-- 角色组件配置子菜单
INSERT INTO `Menu` (`Id`, `ParentId`, `MenuName`, `MenuCode`, `Path`, `Component`, `Icon`, `Sort`, `Type`, `Status`, `Hidden`, `Affix`, `CreateTime`) VALUES
('c0000000-0000-0000-0000-000000000101', 'b0000000-0000-0000-0000-000000000099', '角色配置', 'role_widget_config', '/basic/desktop/role-config', 'basic/desktop/role-config/index', 'UserFilled', 2, 1, 1, 0, 0, NOW());

-- 为管理员角色分配菜单权限
INSERT INTO `RoleMenu` (`Id`, `RoleId`, `MenuId`, `CreateTime`)
SELECT UUID(), 'a1000000-0000-0000-0000-000000000001', `Id`, NOW()
FROM `Menu`
WHERE `Id` IN ('b0000000-0000-0000-0000-000000000099', 'c0000000-0000-0000-0000-000000000100', 'c0000000-0000-0000-0000-000000000101');

-- ============================================
-- 六、为管理员角色分配所有组件
-- ============================================

INSERT INTO `RoleWidgetConfig` (`Id`, `RoleId`, `WidgetId`, `SortOrder`, `IsEnabled`, `CreateTime`)
SELECT UUID(), 'a1000000-0000-0000-0000-000000000001', `Id`, `SortOrder`, 1, NOW()
FROM (
    SELECT `Id`, 1 AS `SortOrder` FROM `DesktopWidget` WHERE `Type` = 1
    UNION ALL
    SELECT `Id`, 10 AS `SortOrder` FROM `DesktopWidget` WHERE `Type` = 2
    UNION ALL
    SELECT `Id`, 20 AS `SortOrder` FROM `DesktopWidget` WHERE `Type` = 3
    UNION ALL
    SELECT `Id`, 30 AS `SortOrder` FROM `DesktopWidget` WHERE `Type` = 4
) AS tmp;

-- ============================================
-- 七、验证数据
-- ============================================

-- 查看组件总数
SELECT COUNT(*) AS '组件总数' FROM `DesktopWidget`;

-- 查看各类型组件数量
SELECT `Type`, CASE `Type`
    WHEN 1 THEN '统计卡片'
    WHEN 2 THEN '数据列表'
    WHEN 3 THEN '图片展示'
    WHEN 4 THEN '图表统计'
END AS '类型名称', COUNT(*) AS '数量'
FROM `DesktopWidget` GROUP BY `Type`;

-- 查看管理员角色组件配置
SELECT rw.`Id`, r.`RoleName`, w.`Name`, w.`Type`, rw.`SortOrder`
FROM `RoleWidgetConfig` rw
JOIN `Role` r ON rw.`RoleId` = r.`Id`
JOIN `DesktopWidget` w ON rw.`WidgetId` = w.`Id`
WHERE rw.`RoleId` = 'a1000000-0000-0000-0000-000000000001'
ORDER BY rw.`SortOrder`;
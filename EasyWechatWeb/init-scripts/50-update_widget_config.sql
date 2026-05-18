-- ============================================
-- 桌面组件配置更新脚本
-- 简化数据源配置：统计卡片用 SQL，快捷入口改菜单选择
-- 执行方式: source sql/update_widget_config.sql;
-- ============================================

USE EasyProject;

-- ============================================
-- 一、更新统计卡片组件（改用 SQL 执行方式）
-- ============================================

-- 待办任务统计
UPDATE `DesktopWidget` SET
    `DataSourceType` = 3,  -- Sql 类型
    `DataSourceConfig` = '{"sql":"SELECT COUNT(*) FROM AntWorkflowTask WHERE Status = 0","label":"待办任务"}',
    `InteractionConfig` = '{"menuId":"c0000000-0000-0000-0000-000000000009"}'  -- 待办任务菜单
WHERE `Id` = 'd0000001-0000-0000-0000-000000000001';

-- 商品总数
UPDATE `DesktopWidget` SET
    `DataSourceType` = 3,
    `DataSourceConfig` = '{"sql":"SELECT COUNT(*) FROM Product WHERE Status = 1","label":"商品总数"}',
    `InteractionConfig` = '{"menuId":"c0000000-0000-0000-0000-000000000001"}'  -- 商品列表菜单
WHERE `Id` = 'd0000001-0000-0000-0000-000000000002';

-- 用户总数
UPDATE `DesktopWidget` SET
    `DataSourceType` = 3,
    `DataSourceConfig` = '{"sql":"SELECT COUNT(*) FROM User WHERE Status = 1","label":"用户总数"}',
    `InteractionConfig` = '{"menuId":"b0000000-0000-0000-0000-000000000001"}'  -- 用户管理菜单
WHERE `Id` = 'd0000001-0000-0000-0000-000000000003';

-- 订单总数
UPDATE `DesktopWidget` SET
    `DataSourceType` = 3,
    `DataSourceConfig` = '{"sql":"SELECT COUNT(*) FROM `Order`","label":"订单总数"}',
    `InteractionConfig` = '{"menuId":"b0000000-0000-0000-0000-000000000006"}'  -- 订单管理菜单
WHERE `Id` = 'd0000001-0000-0000-0000-000000000004';

-- ============================================
-- 二、更新数据列表组件（暂时禁用，等接口完善）
-- ============================================

-- 待办任务列表 - 暂时禁用
UPDATE `DesktopWidget` SET
    `Status` = 0,
    `DataSourceConfig` = '{"api":"/api/ant-workflow/task/todo","method":"POST"}',
    `InteractionConfig` = '{"menuId":"c0000000-0000-0000-0000-000000000009"}'
WHERE `Id` = 'd0000002-0000-0000-0000-000000000001';

-- 最新订单 - 暂时禁用
UPDATE `DesktopWidget` SET
    `Status` = 0,
    `InteractionConfig` = '{"menuId":"b0000000-0000-0000-0000-000000000006"}'
WHERE `Id` = 'd0000002-0000-0000-0000-000000000002';

-- ============================================
-- 三、更新快捷入口组件（改用菜单配置）
-- ============================================

-- 快捷入口 - 使用实际菜单ID
UPDATE `DesktopWidget` SET
    `DataSourceType` = 2,
    `DataSourceConfig` = '{"menus":[{"menuId":"b0000000-0000-0000-0000-000000000011","icon":"DataAnalysis","name":"报表管理"},{"menuId":"b0000000-0000-0000-0000-000000000014","icon":"DataBoard","name":"大屏管理"},{"menuId":"b0000000-0000-0000-0000-000000000021","icon":"Document","name":"日志查询"},{"menuId":"b0000000-0000-0000-0000-000000000001","icon":"User","name":"用户管理"}]}'
WHERE `Id` = 'd0000003-0000-0000-0000-000000000001';

-- ============================================
-- 四、更新图表统计组件（暂时禁用）
-- ============================================

-- 订单趋势图表 - 暂时禁用
UPDATE `DesktopWidget` SET
    `Status` = 0,
    `DataSourceType` = 4,
    `DataSourceConfig` = '{"reportId":"","refreshInterval":300}'
WHERE `Id` = 'd0000004-0000-0000-0000-000000000001';

-- 商品分类统计图表 - 暂时禁用
UPDATE `DesktopWidget` SET
    `Status` = 0,
    `DataSourceType` = 4,
    `DataSourceConfig` = '{"reportId":"","refreshInterval":300}'
WHERE `Id` = 'd0000004-0000-0000-0000-000000000002';

-- ============================================
-- 五、验证更新结果
-- ============================================

SELECT Id, Name, Type, DataSourceType, Status, LEFT(DataSourceConfig, 100) AS DataSourceConfigPreview
FROM DesktopWidget
ORDER BY Type, Id;

SELECT '组件配置更新完成！' AS Message;
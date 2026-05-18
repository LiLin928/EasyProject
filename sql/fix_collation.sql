-- ============================================
-- 字符集统一修复脚本
-- 解决 Illegal mix of collations 错误
-- 执行方式: source sql/fix_collation.sql;
-- ============================================

USE EasyProject;

-- 修改数据库默认字符集
ALTER DATABASE EasyProject CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 修改 DesktopWidget 表
ALTER TABLE `DesktopWidget` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 修改 UserWidgetConfig 表
ALTER TABLE `UserWidgetConfig` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 修改 RoleWidgetConfig 表
ALTER TABLE `RoleWidgetConfig` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 修改 User 表（如果需要）
ALTER TABLE `User` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 修改 Role 表（如果需要）
ALTER TABLE `Role` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 验证修复结果
SELECT
    TABLE_NAME,
    TABLE_COLLATION
FROM
    information_schema.TABLES
WHERE
    TABLE_SCHEMA = 'EasyProject'
    AND TABLE_NAME IN ('DesktopWidget', 'UserWidgetConfig', 'RoleWidgetConfig', 'User', 'Role');

SELECT '字符集统一修复完成！' AS Message;
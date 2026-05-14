-- 文件: EasyWechatWeb/EasyWeChatWeb/Scripts/desktop_widget_tables.sql
-- 桌面组件配置系统数据库表

-- 组件表
DROP TABLE IF EXISTS `DesktopWidget`;
CREATE TABLE `DesktopWidget` (
    `Id` CHAR(36) NOT NULL,
    `Name` VARCHAR(50) NOT NULL,
    `Type` INT NOT NULL DEFAULT 1,
    `Icon` VARCHAR(50) NULL,
    `DefaultWidth` INT NOT NULL DEFAULT 3,
    `DefaultHeight` INT NOT NULL DEFAULT 120,
    `DataSourceType` INT NOT NULL DEFAULT 1,
    `DataSourceConfig` TEXT NULL,
    `InteractionConfig` TEXT NULL,
    `Status` INT NOT NULL DEFAULT 1,
    `CreateTime` DATETIME NOT NULL,
    `UpdateTime` DATETIME NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='桌面组件表';

-- 角色组件配置表
DROP TABLE IF EXISTS `RoleWidgetConfig`;
CREATE TABLE `RoleWidgetConfig` (
    `Id` CHAR(36) NOT NULL,
    `RoleId` CHAR(36) NOT NULL,
    `WidgetId` CHAR(36) NOT NULL,
    `SortOrder` INT NOT NULL DEFAULT 0,
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1,
    `CreateTime` DATETIME NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `idx_role_id` (`RoleId`),
    INDEX `idx_widget_id` (`WidgetId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='角色组件配置表';

-- 用户组件配置表
DROP TABLE IF EXISTS `UserWidgetConfig`;
CREATE TABLE `UserWidgetConfig` (
    `Id` CHAR(36) NOT NULL,
    `UserId` CHAR(36) NOT NULL,
    `WidgetId` CHAR(36) NOT NULL,
    `Width` INT NOT NULL DEFAULT 3,
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1,
    `SortOrder` INT NOT NULL DEFAULT 0,
    `CreateTime` DATETIME NOT NULL,
    `UpdateTime` DATETIME NULL,
    PRIMARY KEY (`Id`),
    INDEX `idx_user_id` (`UserId`),
    INDEX `idx_widget_id` (`WidgetId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户组件配置表';
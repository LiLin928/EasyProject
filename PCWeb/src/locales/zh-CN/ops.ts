/**
 * 运维监控模块国际化 - 中文
 */
export default {
  operateLog: {
    title: '操作日志查询',
    clear: '清理日志',
    detailBtn: '详情',
    detailTitle: '操作日志详情',
    notFound: '日志不存在',
    deleteConfirm: '确定要删除该操作日志吗？',
    clearPrompt: '请输入要清理的日志保留天数（超过此天数的日志将被删除）：',
    clearDaysError: '请输入正整数',
    clearSuccess: '清理完成，删除了 {count} 条日志',
    search: {
      timeRange: '时间范围',
      userName: '用户名',
      module: '模块',
      action: '操作',
      status: '状态',
      ip: 'IP地址',
    },
    status: {
      success: '成功',
      failure: '失败',
    },
    table: {
      createTime: '操作时间',
      userName: '用户',
      module: '模块',
      action: '操作',
      method: '方法',
      url: '请求地址',
      ip: 'IP地址',
      status: '状态',
      duration: '耗时',
      operation: '操作',
    },
    detail: {
      params: '请求参数',
      result: '操作结果',
      errorMsg: '错误信息',
      createTime: '操作时间',
      status: '状态',
      userName: '操作用户',
      moduleName: '操作模块',
      actionName: '操作动作',
      method: '请求方法',
      url: '请求地址',
      ip: 'IP地址',
      location: '操作地点',
      duration: '执行时长',
    },
  },
  task: {
    title: '定时任务管理',
    create: '新建任务',
    edit: '编辑任务',
    pause: '暂停',
    resume: '恢复',
    trigger: '立即执行',
    pauseSuccess: '任务已暂停',
    resumeSuccess: '任务已恢复',
    triggerSuccess: '任务已触发执行',
    deleteConfirm: '确定要删除该任务吗？',
    basicInfo: '基础信息',
    executeTime: '执行时间',
    executor: '执行器配置',
    otherSettings: '其他设置',
    schedule: '执行计划',
    nextExecuteTime: '下次执行时间',

    search: {
      taskName: '任务名称',
      taskType: '任务类型',
      status: '状态',
    },

    type: {
      cron: 'Cron定时',
      immediate: '即时执行',
      periodic: '周期任务',
    },

    status: {
      pending: '待调度',
      scheduled: '已调度',
      paused: '已暂停',
      completed: '已完成',
      failed: '失败',
    },

    form: {
      taskName: '任务名称',
      taskGroup: '任务分组',
      taskType: '任务类型',
      cronExpression: 'Cron表达式',
      scheduleType: '周期类型',
      executeHour: '执行时间(时)',
      executeMinute: '执行时间(分)',
      dayOfMonth: '每月几号',
      handlerType: '处理器类',
      handlerMethod: '处理器方法',
      apiEndpoint: 'API地址',
      apiMethod: '请求方法',
      apiPayload: '请求参数',
      maxRetries: '最大重试',
      timeoutSeconds: '超时时间',
      description: '任务描述',
      executorType: '执行方式',
    },

    scheduleType: {
      daily: '每天',
      monthly: '每月',
      specific: '指定时间',
    },

    executorType: {
      reflection: '反射执行',
      api: 'API调用',
    },

    quick: {
      daily: '每天',
      hourly: '每小时',
      weekly: '每周一',
    },

    statistics: {
      totalCount: '总任务数',
      enabledCount: '已启用',
      pausedCount: '已暂停',
      todayExecuted: '今日执行',
    },

    validation: {
      taskNameRequired: '请输入任务名称',
      taskNameLength: '任务名称长度为2-100个字符',
      taskTypeRequired: '请选择任务类型',
      cronRequired: '请输入Cron表达式',
      executorRequired: '请选择执行方式',
      handlerRequired: '请输入处理器类',
      apiRequired: '请输入API地址',
    },
  },
  taskLog: {
    title: '任务执行日志',
    clear: '清理日志',
    clearPrompt: '请输入要清理的日志保留天数：',
    clearError: '请输入正整数',
    clearSuccess: '清理完成，删除了 {count} 条日志',
    detailBtn: '详情',

    search: {
      timeRange: '时间范围',
      jobName: '任务名称',
      status: '状态',
      triggerType: '触发类型',
    },

    status: {
      running: '执行中',
      success: '成功',
      failure: '失败',
      cancelled: '取消',
    },

    triggerType: {
      cron: 'Cron触发',
      manual: '手动触发',
    },

    table: {
      startTime: '执行时间',
      jobName: '任务名称',
      status: '状态',
      duration: '耗时',
      triggerType: '触发类型',
      operation: '操作',
    },

    detail: {
      title: '执行日志详情',
      resultMessage: '执行结果',
      exceptionMessage: '异常信息',
      exceptionStackTrace: '异常堆栈',
      endTime: '结束时间',
    },

    statistics: {
      todayTotal: '今日执行',
      successRate: '成功率',
      avgDuration: '平均耗时',
      failureCount: '失败数',
    },

    chart: {
      executeCount: '执行次数',
      successRate: '成功率',
      count: '次数',
    },
  },
}
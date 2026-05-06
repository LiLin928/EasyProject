/**
 * Ops Module i18n - English
 */
export default {
  operateLog: {
    title: 'Operation Log Query',
    clear: 'Clear Logs',
    detailBtn: 'Detail',
    detailTitle: 'Operation Log Detail',
    notFound: 'Log not found',
    deleteConfirm: 'Are you sure to delete this operation log?',
    clearPrompt: 'Enter retention days (logs older than this will be deleted):',
    clearDaysError: 'Please enter a positive integer',
    clearSuccess: 'Clear completed, deleted {count} logs',
    search: {
      timeRange: 'Time Range',
      userName: 'User Name',
      module: 'Module',
      action: 'Action',
      status: 'Status',
      ip: 'IP Address',
    },
    status: {
      success: 'Success',
      failure: 'Failure',
    },
    table: {
      createTime: 'Time',
      userName: 'User',
      module: 'Module',
      action: 'Action',
      method: 'Method',
      url: 'URL',
      ip: 'IP',
      status: 'Status',
      duration: 'Duration',
      operation: 'Operation',
    },
    detail: {
      params: 'Request Params',
      result: 'Result',
      errorMsg: 'Error Message',
      createTime: 'Time',
      status: 'Status',
      userName: 'User',
      moduleName: 'Module',
      actionName: 'Action',
      method: 'Method',
      url: 'URL',
      ip: 'IP',
      location: 'Location',
      duration: 'Duration',
    },
  },
  task: {
    title: 'Scheduled Task Management',
    create: 'Create Task',
    edit: 'Edit Task',
    pause: 'Pause',
    resume: 'Resume',
    trigger: 'Execute Now',
    pauseSuccess: 'Task paused',
    resumeSuccess: 'Task resumed',
    triggerSuccess: 'Task triggered',
    deleteConfirm: 'Are you sure to delete this task?',
    basicInfo: 'Basic Information',
    executeTime: 'Execute Time',
    executor: 'Executor Configuration',
    otherSettings: 'Other Settings',
    schedule: 'Schedule',
    nextExecuteTime: 'Next Execute Time',

    search: {
      taskName: 'Task Name',
      taskType: 'Task Type',
      status: 'Status',
    },

    type: {
      cron: 'Cron Schedule',
      immediate: 'Immediate',
      periodic: 'Periodic',
    },

    status: {
      pending: 'Pending',
      scheduled: 'Scheduled',
      paused: 'Paused',
      completed: 'Completed',
      failed: 'Failed',
    },

    form: {
      taskName: 'Task Name',
      taskGroup: 'Task Group',
      taskType: 'Task Type',
      cronExpression: 'Cron Expression',
      scheduleType: 'Schedule Type',
      executeHour: 'Hour',
      executeMinute: 'Minute',
      dayOfMonth: 'Day of Month',
      handlerType: 'Handler Class',
      handlerMethod: 'Handler Method',
      apiEndpoint: 'API Endpoint',
      apiMethod: 'HTTP Method',
      apiPayload: 'Request Payload',
      maxRetries: 'Max Retries',
      timeoutSeconds: 'Timeout (seconds)',
      description: 'Description',
      executorType: 'Executor Type',
    },

    scheduleType: {
      daily: 'Daily',
      monthly: 'Monthly',
      specific: 'Specific Time',
    },

    executorType: {
      reflection: 'Reflection',
      api: 'API Call',
    },

    quick: {
      daily: 'Daily',
      hourly: 'Hourly',
      weekly: 'Weekly',
    },

    statistics: {
      totalCount: 'Total Tasks',
      enabledCount: 'Enabled',
      pausedCount: 'Paused',
      todayExecuted: 'Today Executed',
    },

    validation: {
      taskNameRequired: 'Please enter task name',
      taskNameLength: 'Task name length: 2-100 characters',
      taskTypeRequired: 'Please select task type',
      cronRequired: 'Please enter cron expression',
      executorRequired: 'Please select executor type',
      handlerRequired: 'Please enter handler class',
      apiRequired: 'Please enter API endpoint',
    },
  },
  taskLog: {
    title: 'Task Execution Log',
    clear: 'Clear Logs',
    clearPrompt: 'Enter retention days:',
    clearError: 'Please enter a positive integer',
    clearSuccess: 'Clear completed, deleted {count} logs',
    detailBtn: 'Detail',

    search: {
      timeRange: 'Time Range',
      jobName: 'Task Name',
      status: 'Status',
      triggerType: 'Trigger Type',
    },

    status: {
      running: 'Running',
      success: 'Success',
      failure: 'Failure',
      cancelled: 'Cancelled',
    },

    triggerType: {
      cron: 'Cron Trigger',
      manual: 'Manual Trigger',
    },

    table: {
      startTime: 'Start Time',
      jobName: 'Task Name',
      status: 'Status',
      duration: 'Duration',
      triggerType: 'Trigger Type',
      operation: 'Operation',
    },

    detail: {
      title: 'Execution Log Detail',
      resultMessage: 'Result Message',
      exceptionMessage: 'Exception Message',
      exceptionStackTrace: 'Exception Stack Trace',
      endTime: 'End Time',
    },

    statistics: {
      todayTotal: 'Today Executed',
      successRate: 'Success Rate',
      avgDuration: 'Avg Duration',
      failureCount: 'Failure Count',
    },

    chart: {
      executeCount: 'Execute Count',
      successRate: 'Success Rate',
      count: 'Count',
    },
  },
}
/**
 * 中文 ant_workflow 模块语言包
 * DAG 工作流模块国际化配置
 */
export default {
  // 模块标题
  moduleTitle: 'DAG工作流',

  // 流程列表
  list: '流程列表',
  design: '流程设计',
  designTitle: '流程设计 - {name}',
  runtime: '运行时管理',

  // 运行时页面标题
  runtimeTodo: '待办任务',
  runtimeDone: '已办任务',
  runtimeMy: '我发起的',
  runtimeCc: '抄送我的',

  // 流程状态
  draft: '草稿',
  pending: '待审核',
  published: '已发布',
  rejected: '已拒绝',
  disabled: '已停用',

  // 节点类型标签
  nodeType: '节点类型',
  nodeName: '节点名称',

  // 操作按钮
  view: '查看',

  // 节点类型
  nodeTypes: {
    start: '发起人',
    approver: '审批人',
    copyer: '抄送人',
    condition: '条件分支',
    parallel: '并行分支',
    service: '服务任务',
    notification: '通知',
    webhook: 'Webhook',
    subflow: '子流程',
    counter_sign: '会签',
    end: '结束',
  },

  // 操作
  create: '新建流程',
  edit: '编辑',
  delete: '删除',
  publish: '发布',
  copy: '复制',
  designAction: '设计',
  import: '导入',
  export: '导出',
  enable: '启用',
  disable: '停用',
  versionHistory: '版本历史',
  preview: '预览',
  back: '返回',
  save: '保存',
  createAndDesign: '创建并设计',

  // 弹窗标题
  createTitle: '新建流程',
  editTitle: '编辑流程',
  copyTitle: '复制流程',
  publishTitle: '发布流程',
  versionHistoryTitle: '版本历史',
  deleteTitle: '删除流程',

  // 表单字段
  workflowName: '流程名称',
  name: '流程名称',
  namePlaceholder: '请输入流程名称',
  code: '流程编码',
  codePlaceholder: '请输入流程编码',
  description: '描述',
  descriptionPlaceholder: '请输入描述',
  category: '分类',
  categoryPlaceholder: '请选择分类',
  status: '状态',
  creator: '创建人',
  createTime: '创建时间',
  updateTime: '更新时间',
  version: '版本',

  // 运行时菜单
  todo: '待办任务',
  done: '已办任务',
  myInstances: '我发起的',
  cc: '抄送我的',

  // 运行时字段
  taskTitle: '任务标题',
  currentNode: '当前节点',
  initiator: '发起人',
  entryTime: '进入时间',
  approveTime: '审批时间',
  approveResult: '审批结果',
  ccTime: '抄送时间',
  isRead: '是否已读',
  businessType: '业务类型',
  businessDetail: '业务单据详情',
  instanceStatus: '实例状态',

  // 审批操作
  approve: '审批',
  approvePass: '通过',
  approveReject: '驳回',
  transfer: '转交',
  addSigner: '加签',
  withdraw: '撤回',
  approveComment: '审批意见',
  approveCommentPlaceholder: '请输入审批意见',
  transferTo: '转交给',
  transferToPlaceholder: '请选择转交人员',
  markRead: '标记已读',
  batchMarkRead: '全部标记已读',

  // 审批进度
  approveProgress: '审批进度',
  approveRecord: '审批记录',
  handleTime: '处理时间',
  duration: '耗时',

  // 流程实例状态
  instanceStatusLabels: {
    waitSubmit: '待提交',
    approving: '审批中',
    passed: '已通过',
    rejected: '已驳回',
    withdrawn: '已撤回',
    terminated: '已终止',
  },

  // 处理结果
  handleResultLabels: {
    pass: '通过',
    reject: '驳回',
    transfer: '转交',
    withdraw: '撤回',
  },

  // 设计器
  designer: {
    nodePalette: '节点库',
    propertyPanel: '属性配置',
    searchNode: '搜索节点',
    selectNodeTip: '请选择节点进行配置',
    undo: '撤销',
    redo: '重做',
    autoLayout: '自动布局',
    zoomIn: '放大',
    zoomOut: '缩小',
    fitView: '适应视图',
    clearCanvas: '清空画布',
    save: '保存',
    preview: '预览',
    back: '返回',
    deleteNode: '删除节点',
    editNode: '编辑节点',
    copyNode: '复制节点',
    pasteNode: '粘贴节点',
  },

  // 节点配置面板
  nodeConfig: {
    nodeName: '节点名称',
    nodeNamePlaceholder: '请输入节点名称',
    nodeDescription: '节点描述',
    nodeDescriptionPlaceholder: '请输入节点描述',

    // 发起人节点
    startConfig: {
      flowPermission: '发起权限',
      flowPermissionPlaceholder: '选择可发起流程的人员/角色',
      formConfig: '表单配置',
      addField: '添加字段',
    },

    // 审批人节点
    approverConfig: {
      setType: '审批人类型',
      fixedUser: '固定用户',
      supervisor: '直接主管',
      initiatorSelect: '发起人自选',
      initiatorSelf: '发起人自己',
      multiSupervisor: '多级主管',
      role: '指定角色',
      formField: '表单字段',
      userList: '审批人列表',
      directorLevel: '主管层级',
      examineMode: '审批模式',
      sequential: '依次审批',
      countersign: '会签',
      orSign: '或签',
      noHandlerAction: '无处理人时',
      autoPass: '自动通过',
      transferToAdmin: '转交管理员',
      autoReject: '自动拒绝',
      transfer: '转交',
      timeout: '超时时间(小时)',
      timeoutAction: '超时动作',
    },

    // 抄送人节点
    copyerConfig: {
      userList: '抄送人列表',
      allowSelfSelect: '允许自选抄送人',
    },

    // 服务任务节点
    serviceConfig: {
      taskType: '任务类型',
      apiCall: 'API调用',
      script: '脚本',
      expression: '表达式',
      apiUrl: '接口地址',
      apiMethod: '请求方法',
      apiHeaders: '请求头',
      apiBody: '请求体',
      resultVariable: '结果变量名',
      scriptContent: '脚本内容',
      scriptFormat: '脚本格式',
      expressionContent: '表达式内容',
      errorHandling: '错误处理',
      errorStrategy: '错误处理策略',
      continueOnError: '继续执行',
      stopOnError: '停止流程',
      retryOnError: '重试',
      retryCount: '重试次数',
      retryInterval: '重试间隔(秒)',
    },

    // 通知节点
    notificationConfig: {
      notificationType: '通知类型',
      message: '站内消息',
      email: '邮件',
      sms: '短信',
      wechat: '微信',
      title: '消息标题',
      content: '消息内容',
      template: '消息模板',
      recipients: '接收人',
      addRecipient: '添加接收人',
      recipientType: '接收人类型',
      recipientUser: '指定用户',
      recipientRole: '指定角色',
      recipientInitiator: '发起人',
      recipientSupervisor: '上级领导',
      recipientFormField: '表单字段',
      sendToInitiator: '发送给发起人',
      sendToSupervisor: '发送给上级领导',
    },

    // 条件分支节点
    conditionConfig: {
      branches: '分支列表',
      addBranch: '添加分支',
      branchName: '分支名称',
      conditionRules: '条件规则',
      rules: '条件规则',
      addCondition: '添加条件',
      defaultBranch: '默认分支',
      defaultBranchDesc: '默认分支无需设置条件，当其他分支都不满足时进入',
      fieldName: '字段名',
      operator: '操作符',
      conditionValue: '值',
      priority: '优先级',
    },

    // 并行分支节点
    parallelConfig: {
      branches: '并行分支列表',
      addBranch: '添加分支',
      branchName: '分支名称',
      completeCondition: '完成条件',
      allComplete: '全部完成',
      anyComplete: '任一完成',
      countComplete: '指定数量完成',
      completeCount: '完成数量',
    },

    // Webhook 节点
    webhookConfig: {
      url: 'Webhook URL',
      method: '请求方法',
      headers: '请求头',
      body: '请求体',
      trigger: '触发事件',
      triggerBefore: '节点执行前',
      triggerAfter: '节点执行后',
      triggerManual: '手动触发',
      authConfig: '认证配置',
      authType: '认证类型',
      authNone: '无认证',
      authBasic: 'Basic认证',
      authBearer: 'Bearer Token',
      authApiKey: 'API Key',
      timeout: '超时时间(ms)',
      retryConfig: '重试配置',
      retryCount: '重试次数',
      retryInterval: '重试间隔(ms)',
    },

    // 子流程节点
    subflowConfig: {
      subflowId: '子流程',
      subflowName: '子流程名称',
      inputMappings: '输入参数映射',
      outputMappings: '输出参数映射',
      waitForCompletion: '等待子流程完成',
    },

    // 会签节点
    counterSignConfig: {
      userList: '审批人列表',
      setType: '审批人设置类型',
      passConditionType: '通过条件类型',
      passCondition: '通过条件',
      percentCondition: '按比例',
      countCondition: '按数量',
      allCondition: '全部通过',
      passPercent: '通过比例(%)',
      passCount: '通过数量',
      timeout: '超时时间(小时)',
      timeoutAction: '超时动作',
    },

    // 结束节点
    endConfig: {
      endType: '结束类型',
      endTypeSuccess: '成功完成',
      endTypeReject: '审批拒绝',
      endTypeCancel: '流程取消',
      notification: '通知配置',
      notificationEnabled: '启用结束通知',
      notificationType: '通知类型',
      notificationTitle: '消息标题',
      notificationContent: '消息内容',
      notificationRecipients: '接收人',
      callbackUrl: '回调 URL',
      callbackUrlPlaceholder: '流程结束时调用的 Webhook 地址',
    },
  },

  // 校验提示
  validation: {
    noStartNode: '流程必须包含发起人节点',
    disconnectedNode: '存在未连接的节点',
    emptyConfig: '节点配置不完整',
    nameRequired: '请输入流程名称',
    codeRequired: '请输入流程编码',
    categoryRequired: '请选择分类',
    noApprover: '审批人节点必须设置审批人',
    noCopyer: '抄送人节点必须设置抄送人',
    noConditionBranches: '条件分支至少需要2个分支',
    noParallelBranches: '并行分支至少需要2个分支',
  },

  // 提示信息
  messages: {
    createSuccess: '创建成功',
    createFailed: '创建失败',
    updateSuccess: '更新成功',
    deleteSuccess: '删除成功',
    publishSuccess: '发布成功',
    copySuccess: '复制成功',
    approveSuccess: '审批通过',
    rejectSuccess: '已驳回',
    transferSuccess: '转办成功',
    withdrawSuccess: '撤回成功',
    runtimeWithdrawSuccess: '撤回成功',
    saveSuccess: '保存成功',
    markReadSuccess: '已标记已读',
    selectReadRecords: '请选择要标记已读的记录',
    markFailed: '标记失败',
    deleteConfirm: '确定要删除此流程吗？',
    deleteSelectedConfirm: '确定要删除选中的 {count} 个流程吗？',
    publishConfirm: '确定要发布此流程吗？发布后将立即生效。',
    disableConfirm: '确定要停用此流程吗？',
    enableConfirm: '确定要启用此流程吗？',
    copyConfirm: '确定要复制此流程吗？',
    approveConfirm: '确定要通过此审批吗？',
    rejectConfirm: '确定要驳回此审批吗？',
    transferConfirm: '确定要转交吗？',
    withdrawConfirm: '确定要撤回此流程吗？撤回后可重新发起。',
    batchReadConfirm: '确定要将 {count} 条抄送记录标记为已读吗？',
  },

  // 确认对话框
  confirmWithdraw: '确定要撤回此流程吗？',
  warning: '提示',
  confirmBatchRead: '确定要将 {count} 条抄送记录标记为已读吗？',

  // 已读状态
  readStatus: {
    all: '全部',
    read: '已读',
    unread: '未读',
  },
  selectReadStatus: '请选择',

  // 错误信息
  errors: {
    nameExists: '流程名称已存在',
    codeExists: '流程编码已存在',
    cannotDeletePublished: '已发布的流程不能删除',
    cannotEditPublished: '已发布的流程不能编辑',
    importFailed: '导入失败',
    exportFailed: '导出失败',
    selectWorkflow: '请选择要操作的流程',
    loadDetailFailed: '加载详情失败',
    approveFailed: '审批失败',
    loadDesignerFailed: '加载设计器数据失败',
    saveFailed: '保存失败',
    validateFailed: '校验失败，请检查流程配置',
  },

  // 审批人设置类型
  approverSetTypes: {
    fixedUser: '固定用户',
    supervisor: '直接主管',
    initiatorSelect: '发起人自选',
    initiatorSelf: '发起人自己',
    multiSupervisor: '多级主管',
    role: '指定角色',
    formField: '表单字段',
  },

  // 审批模式
  examineModes: {
    sequential: '依次审批',
    countersign: '会签',
    orSign: '或签',
  },

  // 操作符
  operators: {
    eq: '等于',
    ne: '不等于',
    gt: '大于',
    gte: '大于等于',
    lt: '小于',
    lte: '小于等于',
    contains: '包含',
    notContains: '不包含',
    empty: '为空',
    notEmpty: '不为空',
    in: '在列表中',
    notIn: '不在列表中',
  },

  // 通用
  search: '搜索',
  reset: '重置',
  all: '全部',
  statusPlaceholder: '请选择状态',
  batchDelete: '批量删除',
  operation: '操作',
  selectPlaceholder: '请选择',
  confirm: '确定',
  cancel: '取消',

  // ===================== 业务审核点 =====================
  businessAuditPoint: {
    // 模块标题
    list: '审核点列表',
    create: '新增审核点',
    edit: '编辑审核点',
    delete: '删除审核点',

    // 弹窗标题
    createTitle: '新增审核点',
    editTitle: '编辑审核点',
    warning: '提示',

    // 表单字段
    code: '审核点编码',
    codePlaceholder: '请输入审核点编码',
    name: '审核点名称',
    namePlaceholder: '请输入审核点名称',
    category: '审核点分类',
    categoryPlaceholder: '请输入审核点分类',
    workflowName: '关联流程',
    workflowPlaceholder: '请选择关联流程',
    tableName: '数据表名',
    tableNamePlaceholder: '请输入数据表名',
    primaryKeyField: '主键字段名',
    primaryKeyFieldPlaceholder: '请输入主键字段名',
    primaryKeyFieldTip: '用于条件分支查询：WHERE {主键字段} = {BusinessId}，默认为 Id',
    statusField: '状态字段名',
    statusFieldPlaceholder: '请输入状态字段名',

    // 状态值配置
    statusConfig: '状态值配置',
    auditStatusValue: '待审核状态值',
    passStatusValue: '审核通过值',
    rejectStatusValue: '审核拒绝值',
    withdrawStatusValue: '撤回状态值',

    // 扩展配置
    extendConfig: '扩展配置',
    titleTemplate: '标题模板',
    titleTemplatePlaceholder: '请输入标题模板，如：{name}',
    titleTemplateTip: '可用变量: {name}, {code}, {id} 等',
    codeField: '编码字段名',
    codeFieldPlaceholder: '请输入编码字段名',
    auditPageUrl: '审核页面URL',
    auditPageUrlPlaceholder: '请输入审核页面URL',

    // 回调配置
    callbackConfig: '回调配置',
    passCallbackApi: '通过回调API',
    rejectCallbackApi: '拒绝回调API',
    callbackApiPlaceholder: '请输入回调API地址',

    // 其他配置
    otherConfig: '其他配置',
    status: '状态',
    enabled: '启用',
    disabled: '禁用',
    sort: '排序',
    remark: '备注',
    remarkPlaceholder: '请输入备注',
    createTime: '创建时间',

    // 分区标题
    basicInfo: '基础信息',
    tableConfig: '数据表配置',

    // 操作
    operation: '操作',
    search: '搜索',
    reset: '重置',
    save: '保存',
    confirm: '确定',
    cancel: '取消',

    // 状态筛选
    statusPlaceholder: '请选择状态',

    // 校验提示
    validation: {
      codeRequired: '请输入审核点编码',
      codeLength: '编码长度不能超过50个字符',
      nameRequired: '请输入审核点名称',
      nameLength: '名称长度不能超过100个字符',
      categoryRequired: '请输入审核点分类',
      workflowRequired: '请选择关联流程',
      tableNameRequired: '请输入数据表名',
      statusFieldRequired: '请输入状态字段名',
      auditStatusValueRequired: '请输入待审核状态值',
      passStatusValueRequired: '请输入审核通过状态值',
      rejectStatusValueRequired: '请输入审核拒绝状态值',
      withdrawStatusValueRequired: '请输入撤回状态值',
    },

    // 提示信息
    messages: {
      createSuccess: '创建成功',
      updateSuccess: '更新成功',
      deleteSuccess: '删除成功',
      deleteConfirm: '确定要删除此审核点吗？',
    },

    // 错误信息
    errors: {
      loadDetailFailed: '加载详情失败',
      saveFailed: '保存失败',
    },
  },

  // ===================== 产品审核 =====================
  productAudit: {
    // 操作按钮
    submit: '提交审核',
    withdraw: '撤回审核',
    selectAuditPoint: '选择审核点',

    // 成功消息
    submitSuccess: '提交审核成功',
    withdrawSuccess: '撤回审核成功',

    // 审批操作
    auditPassed: '审核通过',
    auditRejected: '审核拒绝',
    rejectReason: '拒绝原因',

    // 审核状态
    waitSubmit: '待提交',
    pending: '待审核',
    rejected: '已拒绝',
    passed: '已通过',
    withdrawn: '已撤回',
    auditStatus: '审核状态',
  },
}
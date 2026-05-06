/**
 * English ant_workflow module language pack
 * DAG Workflow module i18n configuration
 */
export default {
  // Module title
  moduleTitle: 'DAG Workflow',

  // Workflow list
  list: 'Workflow List',
  design: 'Workflow Design',
  designTitle: 'Workflow Design - {name}',
  runtime: 'Runtime Management',

  // Runtime page titles
  runtimeTodo: 'Todo Tasks',
  runtimeDone: 'Done Tasks',
  runtimeMy: 'My Instances',
  runtimeCc: 'CC to Me',

  // Workflow status
  draft: 'Draft',
  pending: 'Pending',
  published: 'Published',
  rejected: 'Rejected',
  disabled: 'Disabled',

  // Node type label
  nodeType: 'Node Type',
  nodeName: 'Node Name',

  // Action buttons
  view: 'View',

  // Node types
  nodeTypes: {
    start: 'Initiator',
    approver: 'Approver',
    copyer: 'Copyer',
    condition: 'Condition',
    parallel: 'Parallel',
    service: 'Service Task',
    notification: 'Notification',
    webhook: 'Webhook',
    subflow: 'Subflow',
    counter_sign: 'Counter Sign',
    end: 'End',
  },

  // Actions
  create: 'Create Workflow',
  edit: 'Edit',
  delete: 'Delete',
  publish: 'Publish',
  copy: 'Copy',
  designAction: 'Design',
  import: 'Import',
  export: 'Export',
  enable: 'Enable',
  disable: 'Disable',
  versionHistory: 'Version History',
  preview: 'Preview',
  back: 'Back',
  save: 'Save',
  createAndDesign: 'Create & Design',

  // Dialog titles
  createTitle: 'Create Workflow',
  editTitle: 'Edit Workflow',
  copyTitle: 'Copy Workflow',
  publishTitle: 'Publish Workflow',
  versionHistoryTitle: 'Version History',
  deleteTitle: 'Delete Workflow',

  // Form fields
  workflowName: 'Workflow Name',
  name: 'Name',
  namePlaceholder: 'Enter workflow name',
  code: 'Code',
  codePlaceholder: 'Enter workflow code',
  description: 'Description',
  descriptionPlaceholder: 'Enter description',
  category: 'Category',
  categoryPlaceholder: 'Select category',
  status: 'Status',
  creator: 'Creator',
  createTime: 'Create Time',
  updateTime: 'Update Time',
  version: 'Version',

  // Runtime menu
  todo: 'Todo Tasks',
  done: 'Done Tasks',
  myInstances: 'My Instances',
  cc: 'CC to Me',

  // Runtime fields
  taskTitle: 'Task Title',
  currentNode: 'Current Node',
  initiator: 'Initiator',
  entryTime: 'Entry Time',
  approveTime: 'Approve Time',
  approveResult: 'Approve Result',
  ccTime: 'CC Time',
  isRead: 'Is Read',
  businessType: 'Business Type',
  businessDetail: 'Business Detail',
  instanceStatus: 'Instance Status',

  // Approve actions
  approve: 'Approve',
  approvePass: 'Pass',
  approveReject: 'Reject',
  transfer: 'Transfer',
  addSigner: 'Add Signer',
  withdraw: 'Withdraw',
  approveComment: 'Comment',
  approveCommentPlaceholder: 'Enter comment',
  transferTo: 'Transfer to',
  transferToPlaceholder: 'Select user',
  markRead: 'Mark Read',
  batchMarkRead: 'Mark All Read',

  // Approve progress
  approveProgress: 'Approve Progress',
  approveRecord: 'Approve Record',
  handleTime: 'Handle Time',
  duration: 'Duration',

  // Instance status labels
  instanceStatusLabels: {
    waitSubmit: 'Wait Submit',
    approving: 'Approving',
    passed: 'Passed',
    rejected: 'Rejected',
    withdrawn: 'Withdrawn',
    terminated: 'Terminated',
  },

  // Handle result labels
  handleResultLabels: {
    pass: 'Pass',
    reject: 'Reject',
    transfer: 'Transfer',
    withdraw: 'Withdraw',
  },

  // Designer
  designer: {
    nodePalette: 'Node Palette',
    propertyPanel: 'Property Panel',
    searchNode: 'Search Node',
    selectNodeTip: 'Select a node to configure',
    undo: 'Undo',
    redo: 'Redo',
    autoLayout: 'Auto Layout',
    zoomIn: 'Zoom In',
    zoomOut: 'Zoom Out',
    fitView: 'Fit View',
    clearCanvas: 'Clear Canvas',
    save: 'Save',
    preview: 'Preview',
    back: 'Back',
    deleteNode: 'Delete Node',
    editNode: 'Edit Node',
    copyNode: 'Copy Node',
    pasteNode: 'Paste Node',
  },

  // Node config panel
  nodeConfig: {
    nodeName: 'Node Name',
    nodeNamePlaceholder: 'Enter node name',
    nodeDescription: 'Node Description',
    nodeDescriptionPlaceholder: 'Enter node description',

    // Start node
    startConfig: {
      flowPermission: 'Flow Permission',
      flowPermissionPlaceholder: 'Select who can initiate this workflow',
      formConfig: 'Form Config',
      addField: 'Add Field',
    },

    // Approver node
    approverConfig: {
      setType: 'Approver Type',
      fixedUser: 'Fixed User',
      supervisor: 'Direct Supervisor',
      initiatorSelect: 'Initiator Select',
      initiatorSelf: 'Initiator Self',
      multiSupervisor: 'Multi-level Supervisor',
      role: 'Role',
      formField: 'Form Field',
      userList: 'Approver List',
      directorLevel: 'Director Level',
      examineMode: 'Examine Mode',
      sequential: 'Sequential',
      countersign: 'Countersign',
      orSign: 'Or Sign',
      noHandlerAction: 'No Handler Action',
      autoPass: 'Auto Pass',
      transferToAdmin: 'Transfer to Admin',
      autoReject: 'Auto Reject',
      transfer: 'Transfer',
      timeout: 'Timeout(hours)',
      timeoutAction: 'Timeout Action',
    },

    // Copyer node
    copyerConfig: {
      userList: 'Copyer List',
      allowSelfSelect: 'Allow Self Select',
    },

    // Service node
    serviceConfig: {
      taskType: 'Task Type',
      apiCall: 'API Call',
      script: 'Script',
      expression: 'Expression',
      apiUrl: 'API URL',
      apiMethod: 'Method',
      apiHeaders: 'Headers',
      apiBody: 'Body',
      resultVariable: 'Result Variable',
      scriptContent: 'Script Content',
      scriptFormat: 'Script Format',
      expressionContent: 'Expression Content',
      errorHandling: 'Error Handling',
      errorStrategy: 'Error Strategy',
      continueOnError: 'Continue',
      stopOnError: 'Stop',
      retryOnError: 'Retry',
      retryCount: 'Retry Count',
      retryInterval: 'Retry Interval(s)',
    },

    // Notification node
    notificationConfig: {
      notificationType: 'Notification Type',
      message: 'Message',
      email: 'Email',
      sms: 'SMS',
      wechat: 'WeChat',
      title: 'Title',
      content: 'Content',
      template: 'Template',
      recipients: 'Recipients',
      addRecipient: 'Add Recipient',
      recipientType: 'Recipient Type',
      recipientUser: 'User',
      recipientRole: 'Role',
      recipientInitiator: 'Initiator',
      recipientSupervisor: 'Supervisor',
      recipientFormField: 'Form Field',
      sendToInitiator: 'Send to Initiator',
      sendToSupervisor: 'Send to Supervisor',
    },

    // Condition node
    conditionConfig: {
      branches: 'Branches',
      addBranch: 'Add Branch',
      branchName: 'Branch Name',
      conditionRules: 'Condition Rules',
      rules: 'Condition Rules',
      addCondition: 'Add Condition',
      defaultBranch: 'Default Branch',
      defaultBranchDesc: 'Default branch needs no conditions, entered when other branches are not matched',
      fieldName: 'Field Name',
      operator: 'Operator',
      conditionValue: 'Value',
      priority: 'Priority',
    },

    // Parallel node
    parallelConfig: {
      branches: 'Parallel Branches',
      addBranch: 'Add Branch',
      branchName: 'Branch Name',
      completeCondition: 'Complete Condition',
      allComplete: 'All Complete',
      anyComplete: 'Any Complete',
      countComplete: 'Count Complete',
      completeCount: 'Complete Count',
    },

    // Webhook node
    webhookConfig: {
      url: 'Webhook URL',
      method: 'Method',
      headers: 'Headers',
      body: 'Body',
      trigger: 'Trigger Event',
      triggerBefore: 'Before Execution',
      triggerAfter: 'After Execution',
      triggerManual: 'Manual Trigger',
      authConfig: 'Auth Config',
      authType: 'Auth Type',
      authNone: 'None',
      authBasic: 'Basic',
      authBearer: 'Bearer Token',
      authApiKey: 'API Key',
      timeout: 'Timeout(ms)',
      retryConfig: 'Retry Config',
      retryCount: 'Retry Count',
      retryInterval: 'Retry Interval(ms)',
    },

    // Subflow node
    subflowConfig: {
      subflowId: 'Subflow',
      subflowName: 'Subflow Name',
      inputMappings: 'Input Mappings',
      outputMappings: 'Output Mappings',
      waitForCompletion: 'Wait for Completion',
    },

    // Counter sign node
    counterSignConfig: {
      userList: 'Approver List',
      setType: 'Approver Type',
      passConditionType: 'Pass Condition Type',
      passCondition: 'Pass Condition',
      percentCondition: 'Percent',
      countCondition: 'Count',
      allCondition: 'All Pass',
      passPercent: 'Pass Percent',
      passCount: 'Pass Count',
      timeout: 'Timeout(hours)',
      timeoutAction: 'Timeout Action',
    },

    // End node
    endConfig: {
      endType: 'End Type',
      endTypeSuccess: 'Success',
      endTypeReject: 'Reject',
      endTypeCancel: 'Cancel',
      notification: 'Notification',
      notificationEnabled: 'Enable End Notification',
      notificationType: 'Notification Type',
      notificationTitle: 'Message Title',
      notificationContent: 'Message Content',
      notificationRecipients: 'Recipients',
      callbackUrl: 'Callback URL',
      callbackUrlPlaceholder: 'Webhook URL to call when process ends',
    },
  },

  // Validation messages
  validation: {
    noStartNode: 'Workflow must have a start node',
    disconnectedNode: 'Some nodes are disconnected',
    emptyConfig: 'Node config is incomplete',
    nameRequired: 'Name is required',
    nameLength: 'Name length must be between 2 and 50 characters',
    codeRequired: 'Code is required',
    categoryRequired: 'Category is required',
    noApprover: 'Approver node must have approvers',
    noCopyer: 'Copyer node must have copyers',
    noConditionBranches: 'Condition node needs at least 2 branches',
    noParallelBranches: 'Parallel node needs at least 2 branches',
  },

  // Messages
  messages: {
    createSuccess: 'Created successfully',
    createFailed: 'Creation failed',
    updateSuccess: 'Updated successfully',
    deleteSuccess: 'Deleted successfully',
    publishSuccess: 'Published successfully',
    copySuccess: 'Copied successfully',
    approveSuccess: 'Approved successfully',
    rejectSuccess: 'Rejected successfully',
    transferSuccess: 'Transferred successfully',
    withdrawSuccess: 'Withdrawn successfully',
    runtimeWithdrawSuccess: 'Withdrawn successfully',
    saveSuccess: 'Saved successfully',
    markReadSuccess: 'Marked as read',
    selectReadRecords: 'Please select records to mark as read',
    markFailed: 'Mark failed',
    deleteConfirm: 'Are you sure to delete this workflow?',
    deleteSelectedConfirm: 'Are you sure to delete {count} selected workflows?',
    publishConfirm: 'Are you sure to publish this workflow?',
    disableConfirm: 'Are you sure to disable this workflow?',
    enableConfirm: 'Are you sure to enable this workflow?',
    copyConfirm: 'Are you sure to copy this workflow?',
    approveConfirm: 'Are you sure to approve?',
    rejectConfirm: 'Are you sure to reject?',
    transferConfirm: 'Are you sure to transfer?',
    withdrawConfirm: 'Are you sure to withdraw?',
    batchReadConfirm: 'Are you sure to mark {count} CC records as read?',
  },

  // Confirm dialog
  confirmWithdraw: 'Are you sure to withdraw this instance?',
  warning: 'Warning',
  confirmBatchRead: 'Are you sure to mark {count} CC records as read?',
  selectPlaceholder: 'Please select',

  // Read status
  readStatus: {
    all: 'All',
    read: 'Read',
    unread: 'Unread',
  },
  selectReadStatus: 'Please select',

  // Error messages
  errors: {
    nameExists: 'Workflow name already exists',
    codeExists: 'Workflow code already exists',
    cannotDeletePublished: 'Published workflow cannot be deleted',
    cannotEditPublished: 'Published workflow cannot be edited',
    importFailed: 'Import failed',
    exportFailed: 'Export failed',
    selectWorkflow: 'Please select workflow to operate',
    loadDetailFailed: 'Failed to load detail',
    approveFailed: 'Approve failed',
    loadDesignerFailed: 'Failed to load designer data',
    saveFailed: 'Save failed',
    validateFailed: 'Validation failed, please check workflow config',
  },

  // Approver set types
  approverSetTypes: {
    fixedUser: 'Fixed User',
    supervisor: 'Direct Supervisor',
    initiatorSelect: 'Initiator Select',
    initiatorSelf: 'Initiator Self',
    multiSupervisor: 'Multi-level Supervisor',
    role: 'Role',
    formField: 'Form Field',
  },

  // Examine modes
  examineModes: {
    sequential: 'Sequential',
    countersign: 'Countersign',
    orSign: 'Or Sign',
  },

  // Operators
  operators: {
    eq: 'Equals',
    ne: 'Not Equals',
    gt: 'Greater Than',
    gte: 'Greater Than or Equal',
    lt: 'Less Than',
    lte: 'Less Than or Equal',
    contains: 'Contains',
    notContains: 'Not Contains',
    empty: 'Is Empty',
    notEmpty: 'Is Not Empty',
    in: 'In List',
    notIn: 'Not In List',
  },

  // Common
  search: 'Search',
  reset: 'Reset',
  all: 'All',
  statusPlaceholder: 'Select status',
  batchDelete: 'Batch Delete',
  operation: 'Operation',
  confirm: 'Confirm',
  cancel: 'Cancel',

  // ===================== Business Audit Point =====================
  businessAuditPoint: {
    // Module title
    list: 'Audit Point List',
    create: 'Create Audit Point',
    edit: 'Edit Audit Point',
    delete: 'Delete Audit Point',

    // Dialog titles
    createTitle: 'Create Audit Point',
    editTitle: 'Edit Audit Point',
    warning: 'Warning',

    // Form fields
    code: 'Code',
    codePlaceholder: 'Enter audit point code',
    name: 'Name',
    namePlaceholder: 'Enter audit point name',
    category: 'Category',
    categoryPlaceholder: 'Enter audit point category',
    workflowName: 'Workflow',
    workflowPlaceholder: 'Select workflow',
    tableName: 'Table Name',
    tableNamePlaceholder: 'Enter table name',
    primaryKeyField: 'Primary Key Field',
    primaryKeyFieldPlaceholder: 'Enter primary key field name',
    primaryKeyFieldTip: 'For condition query: WHERE {primaryKey} = {BusinessId}, default is Id',
    statusField: 'Status Field',
    statusFieldPlaceholder: 'Enter status field name',

    // Status value config
    statusConfig: 'Status Value Config',
    auditStatusValue: 'Audit Status Value',
    passStatusValue: 'Pass Status Value',
    rejectStatusValue: 'Reject Status Value',
    withdrawStatusValue: 'Withdraw Status Value',

    // Extend config
    extendConfig: 'Extend Config',
    titleTemplate: 'Title Template',
    titleTemplatePlaceholder: 'Enter title template, e.g. {name}',
    titleTemplateTip: 'Available variables: {name}, {code}, {id}',
    codeField: 'Code Field',
    codeFieldPlaceholder: 'Enter code field name',
    auditPageUrl: 'Audit Page URL',
    auditPageUrlPlaceholder: 'Enter audit page URL',

    // Callback config
    callbackConfig: 'Callback Config',
    passCallbackApi: 'Pass Callback API',
    rejectCallbackApi: 'Reject Callback API',
    callbackApiPlaceholder: 'Enter callback API URL',

    // Other config
    otherConfig: 'Other Config',
    status: 'Status',
    enabled: 'Enabled',
    disabled: 'Disabled',
    sort: 'Sort',
    remark: 'Remark',
    remarkPlaceholder: 'Enter remark',
    createTime: 'Create Time',

    // Section titles
    basicInfo: 'Basic Info',
    tableConfig: 'Table Config',

    // Actions
    operation: 'Operation',
    search: 'Search',
    reset: 'Reset',
    save: 'Save',
    confirm: 'Confirm',
    cancel: 'Cancel',

    // Status filter
    statusPlaceholder: 'Select status',

    // Validation
    validation: {
      codeRequired: 'Code is required',
      codeLength: 'Code length cannot exceed 50',
      nameRequired: 'Name is required',
      nameLength: 'Name length cannot exceed 100',
      categoryRequired: 'Category is required',
      workflowRequired: 'Workflow is required',
      tableNameRequired: 'Table name is required',
      statusFieldRequired: 'Status field is required',
      auditStatusValueRequired: 'Audit status value is required',
      passStatusValueRequired: 'Pass status value is required',
      rejectStatusValueRequired: 'Reject status value is required',
      withdrawStatusValueRequired: 'Withdraw status value is required',
    },

    // Messages
    messages: {
      createSuccess: 'Created successfully',
      updateSuccess: 'Updated successfully',
      deleteSuccess: 'Deleted successfully',
      deleteConfirm: 'Are you sure to delete this audit point?',
    },

    // Errors
    errors: {
      loadDetailFailed: 'Failed to load detail',
      saveFailed: 'Save failed',
    },
  },

  // ===================== Product Audit =====================
  productAudit: {
    // Action buttons
    submit: 'Submit Audit',
    withdraw: 'Withdraw Audit',
    selectAuditPoint: 'Select Audit Point',

    // Success messages
    submitSuccess: 'Audit submitted successfully',
    withdrawSuccess: 'Audit withdrawn successfully',

    // Audit actions
    auditPassed: 'Audit Passed',
    auditRejected: 'Audit Rejected',
    rejectReason: 'Reject Reason',

    // Audit status
    waitSubmit: 'Wait Submit',
    pending: 'Pending',
    rejected: 'Rejected',
    passed: 'Passed',
    withdrawn: 'Withdrawn',
    auditStatus: 'Audit Status',
  },
}
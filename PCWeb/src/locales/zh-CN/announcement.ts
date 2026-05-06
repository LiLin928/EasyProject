/**
 * 中文公告模块语言包
 */
export default {
  // 列表页面
  list: {
    title: '公告管理',
    create: '新建公告',
    type: '类型',
    level: '级别',
    status: '状态',
    top: '置顶',
    publishTime: '发布时间',
    creator: '创建人',
    publish: '发布',
    recall: '撤回',
    republish: '重新发布',
    unTop: '取消置顶',
    readStats: '阅读统计',
  },
  // 类型
  type: {
    all: '全员',
    targeted: '定向',
  },
  // 级别
  level: {
    normal: '普通',
    important: '重要',
    urgent: '紧急',
  },
  // 状态
  status: {
    draft: '草稿',
    published: '已发布',
    recalled: '已撤回',
  },
  // 创建页面
  create: {
    title: '新建公告',
  },
  // 编辑页面
  edit: {
    title: '编辑公告',
    basicInfo: '基本信息',
    titleLabel: '标题',
    titleRequired: '请输入标题',
    titleLength: '标题长度为2-200个字符',
    typeLabel: '类型',
    typeRequired: '请选择类型',
    levelLabel: '级别',
    levelRequired: '请选择级别',
    targetRoles: '目标角色',
    selectRoles: '请选择目标角色',
    rolesRequired: '定向公告请选择目标角色',
    contentSection: '公告内容',
    contentLabel: '内容',
    contentRequired: '请输入公告内容',
    contentPlaceholder: '请输入公告内容',
    attachmentSection: '附件',
    attachmentsLabel: '附件',
    uploadButton: '上传附件',
    uploadTip: '支持上传图片、PDF、Word、Excel等文件，单个文件不超过20MB',
    fileTooLarge: '文件大小不能超过20MB',
    uploadSuccess: '上传成功',
    uploadFailed: '上传失败',
    saveDraft: '保存草稿',
    saveDraftSuccess: '保存草稿成功',
    publish: '立即发布',
  },
  // 详情弹窗
  detail: {
    title: '公告详情',
    attachments: '附件',
    preview: '预览',
    markRead: '标记已读',
    markReadSuccess: '已标记为已读',
    unreadTip: '这是一条未读公告，请阅读后点击"标记已读"',
  },
  // 阅读详情弹窗
  readDetail: {
    title: '阅读详情',
    total: '总人数',
    read: '已读',
    unread: '未读',
    all: '全部',
    readOnly: '仅已读',
    unreadOnly: '仅未读',
    userName: '用户名',
    realName: '真实姓名',
    status: '阅读状态',
    readTime: '阅读时间',
  },
  // 消息提示
  message: {
    publishConfirm: '确认发布该公告？发布后将对目标用户可见',
    publishSuccess: '发布成功',
    publishFailed: '发布失败',
    recallConfirm: '确认撤回该公告？撤回后用户将无法查看',
    recallSuccess: '撤回成功',
    deleteConfirm: '确认删除该公告？',
    topSuccess: '置顶成功',
    unTopSuccess: '取消置顶成功',
  },
}
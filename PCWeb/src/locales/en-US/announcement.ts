/**
 * English announcement module language pack
 */
export default {
  // List page
  list: {
    title: 'Announcement Management',
    create: 'Create Announcement',
    type: 'Type',
    level: 'Level',
    status: 'Status',
    top: 'Top',
    publishTime: 'Publish Time',
    creator: 'Creator',
    publish: 'Publish',
    recall: 'Recall',
    republish: 'Republish',
    unTop: 'Unpin',
    readStats: 'Read Statistics',
  },
  // Type
  type: {
    all: 'All Staff',
    targeted: 'Targeted',
  },
  // Level
  level: {
    normal: 'Normal',
    important: 'Important',
    urgent: 'Urgent',
  },
  // Status
  status: {
    draft: 'Draft',
    published: 'Published',
    recalled: 'Recalled',
  },
  // Create page
  create: {
    title: 'Create Announcement',
  },
  // Edit page
  edit: {
    title: 'Edit Announcement',
    basicInfo: 'Basic Information',
    titleLabel: 'Title',
    titleRequired: 'Please enter title',
    titleLength: 'Title length should be 2-200 characters',
    typeLabel: 'Type',
    typeRequired: 'Please select type',
    levelLabel: 'Level',
    levelRequired: 'Please select level',
    targetRoles: 'Target Roles',
    selectRoles: 'Please select target roles',
    rolesRequired: 'Please select target roles for targeted announcement',
    contentSection: 'Content',
    contentLabel: 'Content',
    contentRequired: 'Please enter announcement content',
    contentPlaceholder: 'Please enter announcement content',
    attachmentSection: 'Attachments',
    attachmentsLabel: 'Attachments',
    uploadButton: 'Upload Attachment',
    uploadTip: 'Support images, PDF, Word, Excel files, max 20MB per file',
    fileTooLarge: 'File size cannot exceed 20MB',
    uploadSuccess: 'Upload successful',
    uploadFailed: 'Upload failed',
    saveDraft: 'Save Draft',
    saveDraftSuccess: 'Draft saved successfully',
    publish: 'Publish Now',
  },
  // Detail dialog
  detail: {
    title: 'Announcement Detail',
    attachments: 'Attachments',
    preview: 'Preview',
    markRead: 'Mark as Read',
    markReadSuccess: 'Marked as read',
    unreadTip: 'This is an unread announcement, please click "Mark as Read" after reading',
  },
  // Read detail dialog
  readDetail: {
    title: 'Read Details',
    total: 'Total',
    read: 'Read',
    unread: 'Unread',
    all: 'All',
    readOnly: 'Read Only',
    unreadOnly: 'Unread Only',
    userName: 'Username',
    realName: 'Real Name',
    status: 'Status',
    readTime: 'Read Time',
  },
  // Messages
  message: {
    publishConfirm: 'Confirm to publish this announcement? It will be visible to target users',
    publishSuccess: 'Published successfully',
    publishFailed: 'Publish failed',
    recallConfirm: 'Confirm to recall this announcement? Users will not be able to view it',
    recallSuccess: 'Recalled successfully',
    deleteConfirm: 'Confirm to delete this announcement?',
    topSuccess: 'Pinned successfully',
    unTopSuccess: 'Unpinned successfully',
  },
}
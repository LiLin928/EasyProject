/**
 * 选择器 API - 用于工作流设计器中选择用户、角色等
 */
import { post } from '@/utils/request'
import { SelectorTarget, SelectorTargetType } from '@/types/antWorkflow/selector'

/**
 * 获取用户列表（用于选择器）
 */
export function getUserSelectorList(): Promise<SelectorTarget[]> {
  return post<{ list: UserInfoForSelector[]; total: number }>('/api/user/list', { pageIndex: 1, pageSize: 1000, status: 1 })
    .then(res => res.list.map(user => ({
      id: user.id,
      name: user.realName || user.userName,
      type: SelectorTargetType.USER,
      deptName: user.departmentName || undefined,
    })))
}

/**
 * 获取角色列表（用于选择器）
 */
export function getRoleSelectorList(): Promise<SelectorTarget[]> {
  return post<{ list: RoleInfoForSelector[]; total: number }>('/api/role/list', { pageIndex: 1, pageSize: 1000, status: 1 })
    .then(res => res.list.map(role => ({
      id: role.id,
      name: role.roleName,
      type: SelectorTargetType.ROLE,
    })))
}

/**
 * 用户信息（选择器用）
 */
interface UserInfoForSelector {
  id: string
  userName: string
  realName?: string
  departmentName?: string
}

/**
 * 角色信息（选择器用）
 */
interface RoleInfoForSelector {
  id: string
  roleName: string
}

/**
 * 根据类型获取选择器数据
 */
export async function getSelectorDataByType(type: SelectorTargetType): Promise<SelectorTarget[]> {
  switch (type) {
    case SelectorTargetType.USER:
      return await getUserSelectorList()
    case SelectorTargetType.ROLE:
      return await getRoleSelectorList()
    case SelectorTargetType.DEPT:
      // 部门暂时使用 Mock 数据，后续接入真实 API
      return getMockDepts()
    case SelectorTargetType.POST:
      // 岗位暂时使用 Mock 数据，后续接入真实 API
      return getMockPosts()
    case SelectorTargetType.FORM_FIELD:
      // 表单字段暂时使用 Mock 数据
      return getMockFormFields()
    default:
      return []
  }
}

// 暂时保留的 Mock 数据（部门和岗位）
function getMockDepts(): SelectorTarget[] {
  return [
    { id: 'e1f2a3b4-c5d6-7890-abcd-ef12345678901', name: '研发部', type: SelectorTargetType.DEPT },
    { id: 'f2a3b4c5-d6e7-8901-bcde-f123456789012', name: '销售部', type: SelectorTargetType.DEPT },
    { id: 'a3b4c5d6-e7f8-9012-cdef-1234567890123', name: '财务部', type: SelectorTargetType.DEPT },
    { id: 'b4c5d6e7-f8a9-0123-def1-2345678901234', name: '人事部', type: SelectorTargetType.DEPT },
  ]
}

function getMockPosts(): SelectorTarget[] {
  return [
    { id: 'c5d6e7f8-a9b0-1234-ef12-3456789012345', name: '产品经理', type: SelectorTargetType.POST },
    { id: 'd6e7f8a9-b0c1-2345-f123-4567890123456', name: '高级开发工程师', type: SelectorTargetType.POST },
    { id: 'e7f8a9b0-c1d2-3456-1234-5678901234567', name: '项目经理', type: SelectorTargetType.POST },
    { id: 'f8a9b0c1-d2e3-4567-2345-6789012345678', name: '销售经理', type: SelectorTargetType.POST },
  ]
}

function getMockFormFields(): SelectorTarget[] {
  return [
    { id: 'a9b0c1d2-e3f4-5678-3456-7890123456789', name: '发起人', type: SelectorTargetType.FORM_FIELD },
    { id: 'b0c1d2e3-f4a5-6789-4567-8901234567890', name: '经办人', type: SelectorTargetType.FORM_FIELD },
    { id: 'c1d2e3f4-a5b6-7890-5678-9012345678901', name: '部门负责人', type: SelectorTargetType.FORM_FIELD },
  ]
}
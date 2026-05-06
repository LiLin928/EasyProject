/**
 * ETL Store
 * ETL 工作流状态管理
 */
import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Datasource } from '@/types/etl/datasource'
import type { Pipeline } from '@/types/etl/pipeline'
import { DatasourceType, DatasourceStatus } from '@/types/etl/datasource'
import { PipelineStatus } from '@/types/etl/pipeline'
import { getDatasourceList } from '@/api/etl/datasourceApi'

/**
 * 获取当前时间的 ISO 格式字符串
 */
const formatNow = (): string => {
  return new Date().toISOString()
}

export const useEtlStore = defineStore('etl', () => {
  // 数据源列表（从后端 API 加载）
  const datasources = ref<Datasource[]>([])
  const datasourcesLoading = ref(false)

  // 任务流列表（从后端 API 加载或页面管理）
  const pipelines = ref<Pipeline[]>([])

  // 当前编辑的节点
  const currentNode = ref<any>(null)

  // 当前编辑的边
  const currentEdge = ref<any>(null)

  // 操作历史
  const history = ref<any[]>([])

  /**
   * 设置当前节点
   */
  function setCurrentNode(node: any) {
    currentNode.value = node
  }

  /**
   * 设置当前边
   */
  function setCurrentEdge(edge: any) {
    currentEdge.value = edge
  }

  /**
   * 添加数据源
   */
  function addDatasource(datasource: Datasource) {
    datasources.value.push(datasource)
  }

  /**
   * 更新数据源
   */
  function updateDatasource(id: string, datasource: Partial<Datasource>) {
    const index = datasources.value.findIndex(ds => ds.id === id)
    if (index !== -1) {
      datasources.value[index] = {
        ...datasources.value[index],
        ...datasource,
        updateTime: formatNow(),
      }
    }
  }

  /**
   * 删除数据源
   */
  function deleteDatasource(id: string) {
    const index = datasources.value.findIndex(ds => ds.id === id)
    if (index !== -1) {
      datasources.value.splice(index, 1)
    }
  }

  /**
   * 获取数据源
   */
  function getDatasource(id: string) {
    return datasources.value.find(ds => ds.id === id)
  }

  /**
   * 从后端 API 加载数据源列表
   */
  async function loadDatasources() {
    if (datasourcesLoading.value) return
    datasourcesLoading.value = true
    try {
      const { list } = await getDatasourceList({ pageIndex: 1, pageSize: 100 })
      datasources.value = list.map(ds => ({
        id: ds.id,
        name: ds.name,
        type: ds.type as DatasourceType,
        description: ds.description || '',
        host: ds.host,
        port: ds.port,
        database: ds.database,
        username: ds.username,
        status: (ds.status === 'connected' ? DatasourceStatus.ACTIVE : DatasourceStatus.INACTIVE) as DatasourceStatus,
        creatorId: '',
        creatorName: '',
        createTime: ds.createTime || formatNow(),
        updateTime: formatNow(),
      }))
    } catch (error) {
      console.error('加载数据源列表失败', error)
    } finally {
      datasourcesLoading.value = false
    }
  }

  /**
   * 添加任务流
   */
  function addPipeline(pipeline: Pipeline) {
    pipelines.value.push(pipeline)
  }

  /**
   * 更新任务流
   */
  function updatePipeline(id: string, pipeline: Partial<Pipeline>) {
    const index = pipelines.value.findIndex(p => p.id === id)
    if (index !== -1) {
      pipelines.value[index] = {
        ...pipelines.value[index],
        ...pipeline,
        updateTime: formatNow(),
      }
    }
  }

  /**
   * 删除任务流
   */
  function deletePipeline(id: string) {
    const index = pipelines.value.findIndex(p => p.id === id)
    if (index !== -1) {
      pipelines.value.splice(index, 1)
    }
  }

  /**
   * 获取任务流
   */
  function getPipeline(id: string) {
    return pipelines.value.find(p => p.id === id)
  }

  return {
    datasources,
    datasourcesLoading,
    pipelines,
    currentNode,
    currentEdge,
    history,
    setCurrentNode,
    setCurrentEdge,
    addDatasource,
    updateDatasource,
    deleteDatasource,
    getDatasource,
    loadDatasources,
    addPipeline,
    updatePipeline,
    deletePipeline,
    getPipeline,
  }
})
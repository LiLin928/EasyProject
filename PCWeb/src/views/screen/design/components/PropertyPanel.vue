<!-- 属性面板 -->
<template>
  <div class="property-panel">
    <!-- 无选中组件时的提示 -->
    <div v-if="!component" class="empty-tip">
      <el-icon><InfoFilled /></el-icon>
      <span>{{ t('screen.selectComponentTip') }}</span>
    </div>

    <!-- 有选中组件时显示配置 -->
    <template v-else>
      <!-- Tab 切换 -->
      <el-tabs v-model="activeTab" class="panel-tabs">
        <!-- 基础配置 Tab -->
        <el-tab-pane :label="t('screen.basicConfig.title')" name="basic">
          <div class="config-section">
            <el-form label-width="80px" size="small">
              <el-form-item :label="t('screen.basicConfig.positionX')">
                <el-input-number
                  v-model="formData.position.x"
                  :step="20"
                  controls-position="right"
                  @change="handlePositionChange"
                />
              </el-form-item>
              <el-form-item :label="t('screen.basicConfig.positionY')">
                <el-input-number
                  v-model="formData.position.y"
                  :step="20"
                  controls-position="right"
                  @change="handlePositionChange"
                />
              </el-form-item>
              <el-form-item :label="t('screen.basicConfig.width')">
                <el-input-number
                  v-model="formData.size.width"
                  :min="50"
                  :step="20"
                  controls-position="right"
                  @change="handleSizeChange"
                />
              </el-form-item>
              <el-form-item :label="t('screen.basicConfig.height')">
                <el-input-number
                  v-model="formData.size.height"
                  :min="30"
                  :step="20"
                  controls-position="right"
                  @change="handleSizeChange"
                />
              </el-form-item>
              <el-form-item :label="t('screen.basicConfig.locked')">
                <el-switch v-model="formData.locked" @change="handleConfigChange" />
              </el-form-item>
              <el-form-item :label="t('screen.basicConfig.visible')">
                <el-switch v-model="formData.visible" @change="handleConfigChange" />
              </el-form-item>
            </el-form>
          </div>
        </el-tab-pane>

        <!-- 样式设置 Tab -->
        <el-tab-pane :label="t('screen.styleConfig.title')" name="style">
          <div class="config-section">
            <!-- 标题样式 -->
            <div class="config-group">
              <div class="group-header collapsible" @click="toggleSection('titleStyle')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.titleStyle }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.styleConfig.titleStyle.title') }}
              </div>
              <div v-show="!collapsedSections.titleStyle" class="group-content">
                <el-form label-width="80px" size="small">
                  <el-form-item :label="t('screen.styleConfig.titleStyle.showTitle')">
                    <el-switch v-model="styleConfig.title.show" @change="handleStyleChange" />
                  </el-form-item>
                  <template v-if="styleConfig.title.show">
                    <el-form-item :label="t('screen.styleConfig.titleStyle.titleText')">
                      <el-input v-model="styleConfig.title.text" @change="handleStyleChange" />
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.titleStyle.fontSize')">
                      <el-input-number v-model="styleConfig.title.fontSize" :min="12" :max="36" @change="handleStyleChange" />
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.titleStyle.fontColor')">
                      <el-color-picker v-model="styleConfig.title.color" @change="handleStyleChange" />
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.titleStyle.fontWeight')">
                      <el-select v-model="styleConfig.title.fontWeight" @change="handleStyleChange">
                        <el-option :label="t('screen.styleConfig.titleStyle.fontWeightNormal')" value="normal" />
                        <el-option :label="t('screen.styleConfig.titleStyle.fontWeightBold')" value="bold" />
                      </el-select>
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.titleStyle.titlePosition')">
                      <el-select v-model="styleConfig.title.position" @change="handleStyleChange">
                        <el-option label="顶部" value="top" />
                        <el-option label="底部" value="bottom" />
                        <el-option label="左侧" value="left" />
                        <el-option label="右侧" value="right" />
                      </el-select>
                    </el-form-item>
                  </template>
                </el-form>
              </div>
            </div>

            <!-- 背景设置 -->
            <div class="config-group">
              <div class="group-header collapsible" @click="toggleSection('background')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.background }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.styleConfig.background.title') }}
              </div>
              <div v-show="!collapsedSections.background" class="group-content">
                <el-form label-width="80px" size="small">
                  <el-form-item :label="t('screen.styleConfig.background.bgColor')">
                    <el-color-picker v-model="styleConfig.background.color" show-alpha @change="handleStyleChange" />
                  </el-form-item>
                  <el-form-item :label="t('screen.styleConfig.background.opacity')">
                    <el-slider v-model="styleConfig.background.opacity" :min="0" :max="100" @change="handleStyleChange" />
                  </el-form-item>
                </el-form>
              </div>
            </div>

            <!-- 边框设置 -->
            <div class="config-group">
              <div class="group-header collapsible" @click="toggleSection('border')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.border }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.styleConfig.border.title') }}
              </div>
              <div v-show="!collapsedSections.border" class="group-content">
                <el-form label-width="80px" size="small">
                  <el-form-item :label="t('screen.styleConfig.border.showBorder')">
                    <el-switch v-model="styleConfig.border.show" @change="handleStyleChange" />
                  </el-form-item>
                  <template v-if="styleConfig.border.show">
                    <el-form-item :label="t('screen.styleConfig.border.borderColor')">
                      <el-color-picker v-model="styleConfig.border.color" @change="handleStyleChange" />
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.border.borderWidth')">
                      <el-input-number v-model="styleConfig.border.width" :min="1" :max="10" @change="handleStyleChange" />
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.border.borderStyle')">
                      <el-select v-model="styleConfig.border.style" @change="handleStyleChange">
                        <el-option :label="t('screen.styleConfig.border.borderSolid')" value="solid" />
                        <el-option :label="t('screen.styleConfig.border.borderDashed')" value="dashed" />
                        <el-option :label="t('screen.styleConfig.border.borderDotted')" value="dotted" />
                      </el-select>
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.border.borderRadius')">
                      <el-input-number v-model="styleConfig.border.radius" :min="0" :max="50" @change="handleStyleChange" />
                    </el-form-item>
                  </template>
                </el-form>
              </div>
            </div>

            <!-- 配色方案（图表类组件） -->
            <div v-if="isChartComponent" class="config-group">
              <div class="group-header collapsible" @click="toggleSection('colors')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.colors }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.styleConfig.colors.title') }}
              </div>
              <div v-show="!collapsedSections.colors" class="group-content">
                <el-form label-width="80px" size="small">
                  <el-form-item :label="t('screen.styleConfig.colors.presetColors')">
                    <el-select v-model="selectedColorTheme" @change="handleColorThemeChange">
                      <el-option :label="t('screen.styleConfig.colors.presetThemes.default')" value="default" />
                      <el-option :label="t('screen.styleConfig.colors.presetThemes.warm')" value="warm" />
                      <el-option :label="t('screen.styleConfig.colors.presetThemes.cool')" value="cool" />
                      <el-option :label="t('screen.styleConfig.colors.presetThemes.gradient')" value="gradient" />
                      <el-option :label="t('screen.styleConfig.colors.presetThemes.business')" value="business" />
                    </el-select>
                  </el-form-item>
                  <el-form-item :label="t('screen.styleConfig.colors.customColors')">
                    <div class="color-list">
                      <div v-for="(color, index) in styleConfig.colors" :key="index" class="color-item">
                        <el-color-picker v-model="styleConfig.colors[index]" size="small" @change="handleStyleChange" />
                        <el-button size="small" text type="danger" @click="removeColor(index)">
                          <el-icon><Close /></el-icon>
                        </el-button>
                      </div>
                      <el-button size="small" @click="addColor">
                        <el-icon><Plus /></el-icon>
                        {{ t('screen.styleConfig.colors.addColor') }}
                      </el-button>
                    </div>
                  </el-form-item>
                </el-form>
              </div>
            </div>

            <!-- 图例设置（图表类组件） -->
            <div v-if="isChartComponent" class="config-group">
              <div class="group-header collapsible" @click="toggleSection('legend')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.legend }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.styleConfig.legend.title') }}
              </div>
              <div v-show="!collapsedSections.legend" class="group-content">
                <el-form label-width="80px" size="small">
                  <el-form-item :label="t('screen.styleConfig.legend.showLegend')">
                    <el-switch v-model="styleConfig.legend.show" @change="handleStyleChange" />
                  </el-form-item>
                  <template v-if="styleConfig.legend.show">
                    <el-form-item :label="t('screen.styleConfig.legend.legendPosition')">
                      <el-select v-model="styleConfig.legend.position" @change="handleStyleChange">
                        <el-option label="顶部" value="top" />
                        <el-option label="底部" value="bottom" />
                        <el-option label="左侧" value="left" />
                        <el-option label="右侧" value="right" />
                      </el-select>
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.legend.legendFontSize')">
                      <el-input-number v-model="styleConfig.legend.fontSize" :min="10" :max="24" @change="handleStyleChange" />
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.legend.legendColor')">
                      <el-color-picker v-model="styleConfig.legend.color" @change="handleStyleChange" />
                    </el-form-item>
                  </template>
                </el-form>
              </div>
            </div>

            <!-- 标签设置（图表类组件） -->
            <div v-if="isChartComponent" class="config-group">
              <div class="group-header collapsible" @click="toggleSection('label')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.label }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.styleConfig.label.title') }}
              </div>
              <div v-show="!collapsedSections.label" class="group-content">
                <el-form label-width="80px" size="small">
                  <el-form-item :label="t('screen.styleConfig.label.showLabel')">
                    <el-switch v-model="styleConfig.label.show" @change="handleStyleChange" />
                  </el-form-item>
                  <template v-if="styleConfig.label.show">
                    <el-form-item :label="t('screen.styleConfig.label.labelPosition')">
                      <el-select v-model="styleConfig.label.position" @change="handleStyleChange">
                        <el-option :label="t('screen.styleConfig.label.labelInside')" value="inside" />
                        <el-option :label="t('screen.styleConfig.label.labelOutside')" value="outside" />
                        <el-option :label="t('screen.styleConfig.label.labelTop')" value="top" />
                        <el-option :label="t('screen.styleConfig.label.labelBottom')" value="bottom" />
                      </el-select>
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.label.labelFontSize')">
                      <el-input-number v-model="styleConfig.label.fontSize" :min="10" :max="24" @change="handleStyleChange" />
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.label.labelColor')">
                      <el-color-picker v-model="styleConfig.label.color" @change="handleStyleChange" />
                    </el-form-item>
                    <el-form-item :label="t('screen.styleConfig.label.labelFormatter')">
                      <el-input v-model="styleConfig.label.formatter" placeholder="{value}%" @change="handleStyleChange" />
                    </el-form-item>
                  </template>
                </el-form>
              </div>
            </div>

            <!-- 坐标轴设置（折线图/柱状图） -->
            <div v-if="component && ['line-chart', 'bar-chart'].includes(component.type)" class="config-group">
              <div class="group-header collapsible" @click="toggleSection('axis')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.axis }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.styleConfig.axis.title') }}
              </div>
              <div v-show="!collapsedSections.axis" class="group-content">
                <!-- X轴 -->
                <div class="axis-section">
                  <div class="axis-title">{{ t('screen.styleConfig.axis.xAxis') }}</div>
                  <el-form label-width="80px" size="small">
                    <el-form-item :label="t('screen.styleConfig.axis.showAxis')">
                      <el-switch v-model="styleConfig.axis.xAxis.show" @change="handleStyleChange" />
                    </el-form-item>
                    <template v-if="styleConfig.axis.xAxis.show">
                      <el-form-item :label="t('screen.styleConfig.axis.axisName')">
                        <el-input v-model="styleConfig.axis.xAxis.name" @change="handleStyleChange" />
                      </el-form-item>
                      <el-form-item :label="t('screen.styleConfig.axis.axisNameColor')">
                        <el-color-picker v-model="styleConfig.axis.xAxis.nameColor" @change="handleStyleChange" />
                      </el-form-item>
                      <el-form-item :label="t('screen.styleConfig.axis.axisLineColor')">
                        <el-color-picker v-model="styleConfig.axis.xAxis.lineColor" @change="handleStyleChange" />
                      </el-form-item>
                    </template>
                  </el-form>
                </div>
                <!-- Y轴 -->
                <div class="axis-section">
                  <div class="axis-title">{{ t('screen.styleConfig.axis.yAxis') }}</div>
                  <el-form label-width="80px" size="small">
                    <el-form-item :label="t('screen.styleConfig.axis.showAxis')">
                      <el-switch v-model="styleConfig.axis.yAxis.show" @change="handleStyleChange" />
                    </el-form-item>
                    <template v-if="styleConfig.axis.yAxis.show">
                      <el-form-item :label="t('screen.styleConfig.axis.axisName')">
                        <el-input v-model="styleConfig.axis.yAxis.name" @change="handleStyleChange" />
                      </el-form-item>
                      <el-form-item :label="t('screen.styleConfig.axis.axisNameColor')">
                        <el-color-picker v-model="styleConfig.axis.yAxis.nameColor" @change="handleStyleChange" />
                      </el-form-item>
                      <el-form-item :label="t('screen.styleConfig.axis.axisLineColor')">
                        <el-color-picker v-model="styleConfig.axis.yAxis.lineColor" @change="handleStyleChange" />
                      </el-form-item>
                      <el-form-item :label="t('screen.styleConfig.axis.splitLineColor')">
                        <el-color-picker v-model="styleConfig.axis.yAxis.splitLineColor" @change="handleStyleChange" />
                      </el-form-item>
                      <el-form-item :label="t('screen.styleConfig.axis.axisMin')">
                        <el-input-number v-model="styleConfig.axis.yAxis.min" @change="handleStyleChange" />
                      </el-form-item>
                      <el-form-item :label="t('screen.styleConfig.axis.axisMax')">
                        <el-input-number v-model="styleConfig.axis.yAxis.max" @change="handleStyleChange" />
                      </el-form-item>
                    </template>
                  </el-form>
                </div>
              </div>
            </div>
          </div>
        </el-tab-pane>

        <!-- 数据设置 Tab -->
        <el-tab-pane :label="t('screen.dataConfig')" name="data">
          <div class="config-section">
            <!-- 数据源设置 -->
            <div class="config-group">
              <div class="group-header collapsible" @click="toggleSection('dataSource')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.dataSource }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.dataSource.title') }}
              </div>
              <div v-show="!collapsedSections.dataSource" class="group-content">
                <!-- 数据源类型选择 -->
                <el-form label-width="80px" size="small">
                  <el-form-item :label="t('screen.dataSource.type')">
                    <el-select v-model="dataSourceType" @change="handleDataSourceTypeChange">
                      <el-option :label="t('screen.dataSource.static')" value="static" />
                      <el-option :label="t('screen.dataSource.api')" value="api" />
                      <el-option :label="t('screen.dataSource.dataset')" value="dataset" />
                    </el-select>
                  </el-form-item>
                </el-form>

                <!-- 静态数据配置 -->
                <template v-if="dataSourceType === 'static'">
                  <el-form label-width="80px" size="small">
                    <el-form-item :label="t('screen.dataSource.jsonEditor')">
                      <el-input
                        v-model="jsonDataStr"
                        type="textarea"
                        :rows="8"
                        :placeholder="jsonPlaceholder"
                        @blur="handleJsonDataChange"
                      />
                    </el-form-item>
                    <el-form-item v-if="jsonError" label=" ">
                      <span class="error-tip">{{ jsonError }}</span>
                    </el-form-item>
                  </el-form>
                </template>

                <!-- API接口配置 -->
                <template v-if="dataSourceType === 'api'">
                  <el-form label-width="80px" size="small">
                    <el-form-item :label="t('screen.dataSource.apiConfig.url')">
                      <el-input v-model="apiConfig.url" placeholder="请输入API地址" @change="handleApiConfigChange" />
                    </el-form-item>
                    <el-form-item :label="t('screen.dataSource.apiConfig.method')">
                      <el-select v-model="apiConfig.method" @change="handleApiConfigChange">
                        <el-option :label="t('screen.dataSource.apiConfig.get')" value="GET" />
                        <el-option :label="t('screen.dataSource.apiConfig.post')" value="POST" />
                      </el-select>
                    </el-form-item>
                    <el-form-item :label="t('screen.dataSource.apiConfig.dataPath')">
                      <el-input
                        v-model="apiConfig.dataPath"
                        :placeholder="t('screen.dataSource.apiConfig.dataPathHint')"
                        @change="handleApiConfigChange"
                      />
                      <div class="form-tip">{{ t('screen.dataSource.apiConfig.dataPathHint') }}</div>
                    </el-form-item>
                    <el-form-item :label="t('screen.dataSource.apiConfig.refreshInterval')">
                      <el-input-number
                        v-model="apiConfig.refreshInterval"
                        :min="0"
                        :max="3600"
                        placeholder="秒"
                        @change="handleApiConfigChange"
                      />
                      <div class="form-tip">{{ t('screen.dataSource.apiConfig.refreshIntervalHint') }}</div>
                    </el-form-item>
                    <el-form-item :label="t('screen.dataSource.apiConfig.params')">
                      <el-input
                        v-model="apiParamsStr"
                        type="textarea"
                        :rows="4"
                        placeholder='{"key": "value"}'
                        @blur="handleApiParamsChange"
                      />
                    </el-form-item>
                    <el-form-item>
                      <el-button type="primary" size="small" @click="handlePreviewApi" :loading="apiPreviewing">
                        {{ t('screen.dataSource.apiConfig.preview') || '预览数据' }}
                      </el-button>
                    </el-form-item>
                  </el-form>
                </template>

                <!-- 数据源SQL配置 -->
                <template v-if="dataSourceType === 'dataset'">
                  <el-form label-width="80px" size="small">
                    <el-form-item :label="t('screen.dataSource.datasetConfig.datasource')">
                      <el-select
                        v-model="datasetConfig.datasourceId"
                        :placeholder="t('screen.dataSource.datasetConfig.selectDatasource')"
                        @change="handleDatasetChange"
                      >
                        <el-option
                          v-for="ds in datasourceList"
                          :key="ds.id"
                          :label="ds.name"
                          :value="ds.id"
                        />
                      </el-select>
                    </el-form-item>
                    <el-form-item :label="t('screen.dataSource.datasetConfig.sql')">
                      <SqlEditor
                        ref="sqlEditorRef"
                        v-model="datasetConfig.sql"
                        :datasource-id="datasetConfig.datasourceId"
                        @validate="handleSqlValidate"
                      />
                    </el-form-item>
                    <el-form-item :label="t('screen.dataSource.datasetConfig.refreshInterval')">
                      <el-input-number
                        v-model="datasetConfig.refreshInterval"
                        :min="0"
                        :max="3600"
                        placeholder="秒"
                        @change="handleDatasetChange"
                      />
                      <div class="form-tip">{{ t('screen.dataSource.datasetConfig.refreshIntervalHint') }}</div>
                    </el-form-item>
                    <el-form-item>
                      <el-button type="primary" size="small" @click="handlePreviewSql" :loading="previewing">
                        {{ t('screen.dataSource.datasetConfig.execute') }}
                      </el-button>
                    </el-form-item>
                  </el-form>
                </template>
              </div>
            </div>

            <!-- 数据预览 -->
            <div v-if="previewData.length > 0" class="data-preview">
              <div class="preview-header">
                <span>{{ t('screen.dataSource.datasetConfig.previewData') }} ({{ previewData.length }} {{ t('screen.dataSource.datasetConfig.previewCount') }})</span>
                <el-button size="small" text @click="previewData = []">
                  <el-icon><Close /></el-icon>
                </el-button>
              </div>
              <el-table :data="previewData.slice(0, 5)" size="small" max-height="200">
                <el-table-column
                  v-for="col in previewColumns"
                  :key="col.name"
                  :prop="col.name"
                  :label="col.name"
                  min-width="100"
                />
              </el-table>
            </div>

            <!-- 字段映射（有预览数据时显示） -->
            <div v-if="previewColumns.length > 0 && isChartComponent" class="config-group">
              <div class="group-header collapsible" @click="toggleSection('fieldMapping')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.fieldMapping }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.dataBinding.title') }}
              </div>
              <div v-show="!collapsedSections.fieldMapping" class="group-content">
                <el-form label-width="80px" size="small">
                  <el-form-item :label="t('screen.dataBinding.dimensionField')">
                    <el-select v-model="dataBinding.fieldMapping.dimension" @change="handleDataBindingChange">
                      <el-option v-for="col in previewColumns" :key="col.name" :label="col.name" :value="col.name" />
                    </el-select>
                  </el-form-item>
                  <el-form-item :label="t('screen.dataBinding.valueField')">
                    <el-select v-model="dataBinding.fieldMapping.value" multiple @change="handleDataBindingChange">
                      <el-option v-for="col in previewColumns" :key="col.name" :label="col.name" :value="col.name" />
                    </el-select>
                  </el-form-item>
                  <el-form-item v-if="component && ['line-chart', 'bar-chart'].includes(component.type)" :label="t('screen.dataBinding.seriesField')">
                    <el-select v-model="dataBinding.fieldMapping.series" clearable @change="handleDataBindingChange">
                      <el-option v-for="col in previewColumns" :key="col.name" :label="col.name" :value="col.name" />
                    </el-select>
                  </el-form-item>
                </el-form>
              </div>
            </div>

            <!-- 数据过滤 -->
            <div v-if="previewColumns.length > 0" class="config-group">
              <div class="group-header collapsible" @click="toggleSection('filter')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.filter }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.dataBinding.filter.title') }}
              </div>
              <div v-show="!collapsedSections.filter" class="group-content">
                <div v-for="(filter, index) in dataBinding.filter" :key="index" class="filter-item">
                  <el-form label-width="60px" size="small" inline>
                    <el-form-item label="字段">
                      <el-select v-model="filter.field" size="small">
                        <el-option v-for="col in previewColumns" :key="col.name" :label="col.name" :value="col.name" />
                      </el-select>
                    </el-form-item>
                    <el-form-item label="条件">
                      <el-select v-model="filter.operator" size="small">
                        <el-option :label="t('screen.dataBinding.filter.operatorEq')" value="eq" />
                        <el-option :label="t('screen.dataBinding.filter.operatorNe')" value="ne" />
                        <el-option :label="t('screen.dataBinding.filter.operatorGt')" value="gt" />
                        <el-option :label="t('screen.dataBinding.filter.operatorLt')" value="lt" />
                        <el-option :label="t('screen.dataBinding.filter.operatorContains')" value="contains" />
                      </el-select>
                    </el-form-item>
                    <el-form-item label="值">
                      <el-input v-model="filter.value" size="small" @change="handleDataBindingChange" />
                    </el-form-item>
                    <el-form-item>
                      <el-button size="small" text type="danger" @click="removeFilter(index)">
                        <el-icon><Close /></el-icon>
                      </el-button>
                    </el-form-item>
                  </el-form>
                </div>
                <el-button size="small" @click="addFilter">
                  <el-icon><Plus /></el-icon>
                  {{ t('screen.dataBinding.filter.addFilter') }}
                </el-button>
              </div>
            </div>

            <!-- 数据排序 -->
            <div v-if="previewColumns.length > 0" class="config-group">
              <div class="group-header collapsible" @click="toggleSection('sort')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.sort }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.dataBinding.sort.title') }}
              </div>
              <div v-show="!collapsedSections.sort" class="group-content">
                <el-form label-width="80px" size="small">
                  <el-form-item :label="t('screen.dataBinding.sort.sortField')">
                    <el-select v-model="dataBinding.sort.field" clearable @change="handleDataBindingChange">
                      <el-option v-for="col in previewColumns" :key="col.name" :label="col.name" :value="col.name" />
                    </el-select>
                  </el-form-item>
                  <el-form-item v-if="dataBinding.sort.field" :label="t('screen.dataBinding.sort.sortOrder')">
                    <el-select v-model="dataBinding.sort.order" @change="handleDataBindingChange">
                      <el-option :label="t('screen.dataBinding.sort.sortAsc')" value="asc" />
                      <el-option :label="t('screen.dataBinding.sort.sortDesc')" value="desc" />
                    </el-select>
                  </el-form-item>
                </el-form>
              </div>
            </div>

            <!-- 数据格式化 -->
            <div v-if="isChartComponent" class="config-group">
              <div class="group-header collapsible" @click="toggleSection('formatter')">
                <el-icon class="collapse-icon" :class="{ collapsed: collapsedSections.formatter }">
                  <ArrowRight />
                </el-icon>
                {{ t('screen.dataBinding.formatter.title') }}
              </div>
              <div v-show="!collapsedSections.formatter" class="group-content">
                <el-form label-width="80px" size="small">
                  <el-form-item :label="t('screen.dataBinding.formatter.decimals')">
                    <el-input-number v-model="dataBinding.formatter.number.decimals" :min="0" :max="6" @change="handleDataBindingChange" />
                  </el-form-item>
                  <el-form-item :label="t('screen.dataBinding.formatter.prefix')">
                    <el-input v-model="dataBinding.formatter.number.prefix" @change="handleDataBindingChange" />
                  </el-form-item>
                  <el-form-item :label="t('screen.dataBinding.formatter.suffix')">
                    <el-input v-model="dataBinding.formatter.number.suffix" @change="handleDataBindingChange" />
                  </el-form-item>
                  <el-form-item :label="t('screen.dataBinding.formatter.unit')">
                    <el-select v-model="dataBinding.formatter.number.unit" @change="handleDataBindingChange">
                      <el-option :label="t('screen.dataBinding.formatter.unitNone')" value="none" />
                      <el-option :label="t('screen.dataBinding.formatter.unitThousand')" value="thousand" />
                      <el-option :label="t('screen.dataBinding.formatter.unitMillion')" value="million" />
                      <el-option :label="t('screen.dataBinding.formatter.unitBillion')" value="billion" />
                      <el-option :label="t('screen.dataBinding.formatter.unitPercent')" value="percent" />
                    </el-select>
                  </el-form-item>
                </el-form>
              </div>
            </div>
          </div>
        </el-tab-pane>
      </el-tabs>

      <!-- 删除按钮 -->
      <div class="panel-footer">
        <el-button type="danger" size="small" @click="handleDelete">
          <el-icon><Delete /></el-icon>
          {{ t('screen.delete') }}
        </el-button>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, computed } from 'vue'
import { InfoFilled, Delete, Close, Plus, ArrowRight } from '@element-plus/icons-vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { useScreenStore } from '@/stores/screen'
import { getAvailableDatasources, executeSqlQuery } from '@/api/screen'
import SqlEditor from './SqlEditor.vue'
import type { ScreenComponent, ScreenConfig, DataSource, ApiDataSourceConfig, DatasetSourceConfig, DatasourceOption, ComponentStyleConfig, DataBindingConfig } from '@/types'

const { t } = useLocale()
const store = useScreenStore()

const props = defineProps<{
  component: ScreenComponent | null
  screen: ScreenConfig | null
}>()

const emit = defineEmits<{
  (e: 'update', updates: Partial<ScreenComponent>): void
  (e: 'update-screen', updates: Partial<ScreenConfig>): void
}>()

// 状态
const activeTab = ref('basic')
const formData = ref({
  position: { x: 0, y: 0 },
  size: { width: 0, height: 0 },
  config: {} as Record<string, any>,
  locked: false,
  visible: true,
})

// 判断是否为图表组件
const isChartComponent = computed(() => {
  const chartTypes = ['line-chart', 'bar-chart', 'pie-chart', 'radar-chart', 'funnel-chart', 'heatmap-chart', 'number-card']
  return props.component && chartTypes.includes(props.component.type)
})

// 样式配置
const defaultStyleConfig: ComponentStyleConfig = {
  title: {
    show: true,
    text: '',
    fontSize: 14,
    fontFamily: 'Microsoft YaHei',
    color: '#ffffff',
    fontWeight: 'normal',
    position: 'top',
    align: 'center',
  },
  background: {
    color: '',
    opacity: 100,
  },
  border: {
    show: false,
    color: '#409eff',
    width: 1,
    style: 'solid',
    radius: 0,
  },
  colors: ['#5470c6', '#91cc75', '#fac858', '#ee6666', '#73c0de', '#3ba272', '#fc8452', '#9a60b4'],
  legend: {
    show: true,
    position: 'top',
    fontSize: 12,
    color: '#ffffff',
  },
  label: {
    show: true,
    position: 'top',
    fontSize: 12,
    color: '#ffffff',
    formatter: '{value}',
  },
  axis: {
    xAxis: {
      show: true,
      name: '',
      nameColor: '#ffffff',
      nameFontSize: 12,
      labelColor: '#ffffff',
      labelFontSize: 12,
      lineColor: '#ffffff',
    },
    yAxis: {
      show: true,
      name: '',
      nameColor: '#ffffff',
      nameFontSize: 12,
      labelColor: '#ffffff',
      labelFontSize: 12,
      lineColor: '#ffffff',
      splitLineColor: 'rgba(255,255,255,0.1)',
    },
  },
}
const styleConfig = ref<ComponentStyleConfig>(JSON.parse(JSON.stringify(defaultStyleConfig)))

// 选中的配色主题
const selectedColorTheme = ref('default')

// 预设配色方案
const colorThemes: Record<string, string[]> = {
  default: ['#5470c6', '#91cc75', '#fac858', '#ee6666', '#73c0de', '#3ba272', '#fc8452', '#9a60b4'],
  warm: ['#e6a23c', '#f56c6c', '#ff9f7f', '#ffd666', '#ffb347', '#ff6b6b', '#ffa07a', '#ff7f50'],
  cool: ['#409eff', '#67c23a', '#00d4ff', '#00bcd4', '#4fc3f7', '#81d4fa', '#4db6ac', '#26a69a'],
  gradient: ['#667eea', '#764ba2', '#f093fb', '#f5576c', '#4facfe', '#00f2fe', '#43e97b', '#38f9d7'],
  business: ['#2d8cf0', '#19be6b', '#ff9900', '#e46cbb', '#5cadff', '#b4d7ac', '#d48265', '#91c7ae'],
}

// 数据绑定配置
const defaultDataBinding: DataBindingConfig = {
  fieldMapping: {
    dimension: '',
    value: [],
    series: '',
  },
  filter: [],
  sort: {
    field: '',
    order: 'asc',
  },
  formatter: {
    number: {
      decimals: 2,
      prefix: '',
      suffix: '',
      unit: 'none',
    },
    date: {
      format: 'YYYY-MM-DD',
    },
  },
}
const dataBinding = ref<DataBindingConfig>(JSON.parse(JSON.stringify(defaultDataBinding)))

// 数据源相关
const dataSourceType = ref<'static' | 'api' | 'dataset'>('static')
const jsonDataStr = ref('[]')
const jsonError = ref('')
const jsonPlaceholder = '[\n  { "name": "示例", "value": 100 }\n]'

// API配置
const apiConfig = ref<ApiDataSourceConfig>({
  url: '',
  method: 'GET',
  dataPath: '',
  refreshInterval: 0,
})
const apiParamsStr = ref('')

// 数据源SQL配置
const datasetConfig = ref<DatasetSourceConfig>({
  datasourceId: '',
  sql: '',
  refreshInterval: 0,
})

// 数据源列表
const datasourceList = ref<DatasourceOption[]>([])

// 预览数据
const previewData = ref<any[]>([])
const previewColumns = ref<{ name: string; type: string }[]>([])
const previewing = ref(false)

// API预览加载状态
const apiPreviewing = ref(false)

// 数据绑定折叠状态
const collapsedSections = ref({
  // 数据设置
  dataSource: false,
  fieldMapping: false,
  filter: false,
  sort: false,
  formatter: false,
  // 样式设置
  titleStyle: false,
  background: false,
  border: false,
  colors: false,
  legend: false,
  label: false,
  axis: false,
})

// 切换折叠状态
const toggleSection = (section: keyof typeof collapsedSections.value) => {
  collapsedSections.value[section] = !collapsedSections.value[section]
}

// SQL编辑器引用
const sqlEditorRef = ref<InstanceType<typeof SqlEditor>>()

// 加载数据源列表
const loadDatasources = async () => {
  try {
    const res = await getAvailableDatasources()
    datasourceList.value = res
  } catch (error) {
    console.error('加载数据源列表失败:', error)
  }
}

// 监听组件变化，更新表单数据
watch(
  () => props.component,
  (comp, oldComp) => {
    if (comp) {
      // 只有当组件 ID 变化时才重置预览数据（避免字段映射等更新时清空预览）
      const isComponentChanged = comp.id !== oldComp?.id

      formData.value = {
        position: { ...comp.position },
        size: { ...comp.size },
        config: { ...comp.config },
        locked: comp.locked || false,
        visible: comp.visible !== false,
      }

      // 更新样式配置
      if (comp.style) {
        styleConfig.value = {
          ...JSON.parse(JSON.stringify(defaultStyleConfig)),
          ...comp.style,
        }
      } else {
        styleConfig.value = JSON.parse(JSON.stringify(defaultStyleConfig))
      }

      // 更新数据绑定配置
      if (comp.dataBinding) {
        dataBinding.value = {
          ...JSON.parse(JSON.stringify(defaultDataBinding)),
          ...comp.dataBinding,
          // 确保 value 是数组
          fieldMapping: {
            ...comp.dataBinding.fieldMapping,
            value: Array.isArray(comp.dataBinding.fieldMapping?.value)
              ? comp.dataBinding.fieldMapping.value
              : (comp.dataBinding.fieldMapping?.value ? [comp.dataBinding.fieldMapping.value] : []),
          },
        }
      } else {
        dataBinding.value = JSON.parse(JSON.stringify(defaultDataBinding))
      }

      // 更新数据源配置 - 只有组件切换时才重置预览数据
      const ds = comp.dataSource
      dataSourceType.value = ds.type || 'static'

      if (isComponentChanged) {
        // 组件切换时，根据数据源类型初始化预览数据
        if (ds.type === 'static' || !ds.type) {
          jsonDataStr.value = JSON.stringify(ds.data || [], null, 2)
          const data = ds.data || []
          if (data.length > 0) {
            previewData.value = data
            previewColumns.value = Object.keys(data[0]).map(key => ({ name: key, type: typeof data[0][key] }))
          } else {
            previewData.value = []
            previewColumns.value = []
          }
        } else if (ds.type === 'api' && ds.apiConfig) {
          apiConfig.value = { ...ds.apiConfig }
          apiParamsStr.value = ds.apiConfig.params ? JSON.stringify(ds.apiConfig.params, null, 2) : ''
          // 如果已有预览数据，恢复它
          if (ds.data && ds.data.length > 0) {
            previewData.value = ds.data
            previewColumns.value = Object.keys(ds.data[0]).map(key => ({ name: key, type: typeof ds.data[0][key] }))
          } else {
            previewData.value = []
            previewColumns.value = []
          }
        } else if (ds.type === 'dataset' && ds.datasetConfig) {
          datasetConfig.value = { ...ds.datasetConfig }
          // 如果已有预览数据，恢复它
          if (ds.data && ds.data.length > 0) {
            previewData.value = ds.data
            previewColumns.value = Object.keys(ds.data[0]).map(key => ({ name: key, type: typeof ds.data[0][key] }))
          } else {
            previewData.value = []
            previewColumns.value = []
          }
        }
      } else {
        // 同组件属性更新时，只同步配置，不重置预览数据
        if (ds.type === 'api' && ds.apiConfig) {
          apiConfig.value = { ...ds.apiConfig }
          apiParamsStr.value = ds.apiConfig.params ? JSON.stringify(ds.apiConfig.params, null, 2) : ''
        } else if (ds.type === 'dataset' && ds.datasetConfig) {
          datasetConfig.value = { ...ds.datasetConfig }
        }
      }

      jsonError.value = ''
    }
  },
  { immediate: true, deep: true }
)

// 位置变化
const handlePositionChange = () => {
  emit('update', { position: formData.value.position })
}

// 尺寸变化
const handleSizeChange = () => {
  emit('update', { size: formData.value.size })
}

// 配置变化
const handleConfigChange = () => {
  emit('update', { config: formData.value.config })
}

// 样式变化
const handleStyleChange = () => {
  emit('update', { style: styleConfig.value })
}

// 配色主题变化
const handleColorThemeChange = (theme: string) => {
  if (colorThemes[theme]) {
    styleConfig.value.colors = [...colorThemes[theme]]
    handleStyleChange()
  }
}

// 添加颜色
const addColor = () => {
  styleConfig.value.colors?.push('#409eff')
  handleStyleChange()
}

// 移除颜色
const removeColor = (index: number) => {
  styleConfig.value.colors?.splice(index, 1)
  handleStyleChange()
}

// 数据绑定变化
const handleDataBindingChange = () => {
  emit('update', { dataBinding: dataBinding.value })
}

// 添加过滤条件
const addFilter = () => {
  dataBinding.value.filter?.push({
    field: previewColumns.value[0]?.name || '',
    operator: 'eq',
    value: '',
  })
}

// 移除过滤条件
const removeFilter = (index: number) => {
  dataBinding.value.filter?.splice(index, 1)
  handleDataBindingChange()
}

// 数据源类型变化
const handleDataSourceTypeChange = () => {
  previewData.value = []
  updateDataSource()
}

// 更新数据源
const updateDataSource = () => {
  const dataSource: DataSource = {
    type: dataSourceType.value,
  }

  if (dataSourceType.value === 'static') {
    try {
      dataSource.data = JSON.parse(jsonDataStr.value || '[]')
    } catch {
      return
    }
  } else if (dataSourceType.value === 'api') {
    dataSource.apiConfig = { ...apiConfig.value }
    if (apiParamsStr.value) {
      try {
        dataSource.apiConfig.params = JSON.parse(apiParamsStr.value)
      } catch {
        // 忽略解析错误
      }
    }
  } else if (dataSourceType.value === 'dataset') {
    dataSource.datasetConfig = { ...datasetConfig.value }
  }

  emit('update', { dataSource })
}

// JSON 数据变化
const handleJsonDataChange = () => {
  try {
    const data = JSON.parse(jsonDataStr.value)
    jsonError.value = ''
    previewData.value = data
    previewColumns.value = data.length > 0
      ? Object.keys(data[0]).map(key => ({ name: key, type: typeof data[0][key] }))
      : []
    updateDataSource()
  } catch {
    jsonError.value = t('screen.dataSource.jsonInvalid')
  }
}

// API配置变化
const handleApiConfigChange = () => {
  updateDataSource()
}

// API参数变化
const handleApiParamsChange = () => {
  updateDataSource()
}

// 数据源SQL变化
const handleDatasetChange = () => {
  updateDataSource()
}

// SQL验证
const handleSqlValidate = (result: { valid: boolean; columns?: { name: string; type: string }[] }) => {
  if (result.valid && result.columns) {
    previewColumns.value = result.columns
  }
}

// 执行SQL预览
const handlePreviewSql = async () => {
  if (!datasetConfig.value.datasourceId || !datasetConfig.value.sql.trim()) {
    ElMessage.warning('请选择数据源并输入SQL语句')
    return
  }

  previewing.value = true
  try {
    const result = await executeSqlQuery({
      datasourceId: datasetConfig.value.datasourceId,
      sql: datasetConfig.value.sql,
    })
    previewData.value = result.data
    previewColumns.value = result.columns
    ElMessage.success(`查询成功，共 ${result.data.length} 条数据`)

    // 同步到SQL编辑器
    sqlEditorRef.value?.setPreviewData(result.data, result.columns)

    // 更新组件的数据源，包含预览数据（用于图表渲染）
    emit('update', {
      dataSource: {
        type: 'dataset',
        datasetConfig: { ...datasetConfig.value },
        data: result.data
      }
    })
  } catch (error) {
    ElMessage.error('查询失败')
  } finally {
    previewing.value = false
  }
}

// API调用函数
const callApi = async (config: ApiDataSourceConfig) => {
  const { url, method, params } = config

  if (method === 'GET') {
    const queryStr = params ? new URLSearchParams(params).toString() : ''
    const fullUrl = queryStr ? `${url}?${queryStr}` : url
    const response = await fetch(fullUrl)
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    return response.json()
  } else {
    const response = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(params || {})
    })
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    return response.json()
  }
}

// 数据路径解析函数
const extractDataByPath = (response: any, path: string): any[] => {
  if (!path) return Array.isArray(response) ? response : []
  const parts = path.split('.')
  let result = response
  for (const part of parts) {
    result = result?.[part]
  }
  return Array.isArray(result) ? result : []
}

// API预览
const handlePreviewApi = async () => {
  if (!apiConfig.value.url) {
    ElMessage.warning(t('screen.dataSource.apiConfig.urlHint') || '请输入API地址')
    return
  }

  apiPreviewing.value = true
  try {
    // 调用 API
    const response = await callApi(apiConfig.value)

    // 解析数据路径
    const data = extractDataByPath(response, apiConfig.value.dataPath || '')

    if (data.length === 0) {
      ElMessage.warning(t('screen.dataSource.datasetConfig.noData') || '数据为空，请检查dataPath配置')
      previewData.value = []
      previewColumns.value = []
      // 更新组件的数据源，清空数据
      emit('update', {
        dataSource: {
          type: 'api',
          apiConfig: { ...apiConfig.value },
          data: []
        }
      })
      return
    }

    // 更新预览数据
    previewData.value = data
    previewColumns.value = Object.keys(data[0]).map(key => ({ name: key, type: typeof data[0][key] }))

    // 更新组件的数据源，包含预览数据（用于图表渲染）
    emit('update', {
      dataSource: {
        type: 'api',
        apiConfig: { ...apiConfig.value },
        data: data
      }
    })

    ElMessage.success(t('screen.dataSource.datasetConfig.previewSuccess') || `预览成功，共 ${data.length} 条数据`)
  } catch (error: any) {
    console.error('API预览失败:', error)
    ElMessage.error(t('screen.dataSource.apiConfig.previewFailed') || 'API调用失败，请检查配置或网络')
  } finally {
    apiPreviewing.value = false
  }
}

// 删除组件
const handleDelete = async () => {
  try {
    await ElMessageBox.confirm('确定删除此组件？', '提示', {
      type: 'warning',
    })
    if (props.component) {
      store.deleteComponent(props.component.id)
    }
  } catch {
    // 取消删除
  }
}

onMounted(() => {
  loadDatasources()
})
</script>

<style scoped lang="scss">
.property-panel {
  height: 100%;
  display: flex;
  flex-direction: column;
  background: rgba(255, 255, 255, 0.05);
  border-left: 1px solid rgba(255, 255, 255, 0.1);
  overflow: hidden;
}

.empty-tip {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
  color: rgba(255, 255, 255, 0.5);
  font-size: 14px;

  .el-icon {
    font-size: 32px;
    margin-bottom: 8px;
  }
}

.panel-tabs {
  flex: 1;
  min-height: 0;
  display: flex;
  flex-direction: column;
  overflow: hidden;

  // Element Plus tabs 组件整体样式
  :deep(.el-tabs) {
    display: flex;
    flex-direction: column;
    height: 100%;
    overflow: hidden;
  }

  :deep(.el-tabs__header) {
    margin: 0;
    flex-shrink: 0;
    background: rgba(255, 255, 255, 0.05);
  }

  :deep(.el-tabs__nav-wrap::after) {
    display: none;
  }

  :deep(.el-tabs__item) {
    color: rgba(255, 255, 255, 0.6);

    &.is-active {
      color: #409eff;
    }
  }

  :deep(.el-tabs__content) {
    flex: 1;
    min-height: 0;
    overflow-y: auto;
    overflow-x: hidden;

    // 自定义滚动条
    &::-webkit-scrollbar {
      width: 6px;
    }

    &::-webkit-scrollbar-track {
      background: rgba(255, 255, 255, 0.05);
      border-radius: 3px;
    }

    &::-webkit-scrollbar-thumb {
      background: rgba(255, 255, 255, 0.2);
      border-radius: 3px;

      &:hover {
        background: rgba(255, 255, 255, 0.3);
      }
    }
  }

  :deep(.el-tab-pane) {
    // 让内容自然撑开，由 el-tabs__content 提供滚动
    overflow: visible;
  }
}

.config-section {
  padding: 16px;

  :deep(.el-form-item__label) {
    color: rgba(255, 255, 255, 0.7);
  }

  :deep(.el-input__wrapper),
  :deep(.el-textarea__inner) {
    background: rgba(255, 255, 255, 0.1);
    box-shadow: none;
  }

  :deep(.el-input__inner),
  :deep(.el-textarea__inner) {
    color: #fff;
  }

  :deep(.el-input-number) {
    width: 100%;
  }

  :deep(.el-select) {
    width: 100%;
  }
}

.error-tip {
  color: #f56c6c;
  font-size: 12px;
}

.form-tip {
  font-size: 12px;
  color: rgba(255, 255, 255, 0.5);
  margin-top: 4px;
}

.data-preview {
  margin-top: 16px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 4px;
  overflow: hidden;
}

.preview-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 12px;
  background: rgba(255, 255, 255, 0.05);
  font-size: 13px;
  color: rgba(255, 255, 255, 0.7);
}

.panel-footer {
  flex-shrink: 0;
  padding: 12px 16px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  background: rgba(255, 255, 255, 0.05);
}

:deep(.el-table) {
  --el-table-bg-color: transparent;
  --el-table-tr-bg-color: transparent;
  --el-table-header-bg-color: rgba(255, 255, 255, 0.05);
  --el-table-text-color: rgba(255, 255, 255, 0.8);
  --el-table-border-color: rgba(255, 255, 255, 0.1);
}

// 配置分组
.config-group {
  margin-bottom: 16px;
  padding-bottom: 16px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);

  &:last-child {
    border-bottom: none;
  }
}

.group-header {
  font-size: 13px;
  font-weight: 500;
  color: rgba(255, 255, 255, 0.9);
  margin-bottom: 12px;
  padding-left: 8px;
  border-left: 3px solid #409eff;

  &.collapsible {
    cursor: pointer;
    display: flex;
    align-items: center;
    user-select: none;

    &:hover {
      color: #409eff;
    }

    .collapse-icon {
      margin-right: 6px;
      transition: transform 0.2s;
      font-size: 12px;

      &.collapsed {
        transform: rotate(90deg);
      }
    }
  }
}

.group-content {
  animation: fadeIn 0.2s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

// 坐标轴区域
.axis-section {
  margin-bottom: 12px;
  padding: 8px 12px;
  background: rgba(255, 255, 255, 0.03);
  border-radius: 4px;
}

.axis-title {
  font-size: 12px;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 8px;
}

// 颜色列表
.color-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  align-items: center;
}

.color-item {
  display: flex;
  align-items: center;
  gap: 4px;
}

// 过滤条件
.filter-item {
  padding: 8px;
  margin-bottom: 8px;
  background: rgba(255, 255, 255, 0.03);
  border-radius: 4px;

  :deep(.el-form-item) {
    margin-bottom: 0;
    margin-right: 8px;
  }

  :deep(.el-select) {
    width: 100px;
  }
}
</style>
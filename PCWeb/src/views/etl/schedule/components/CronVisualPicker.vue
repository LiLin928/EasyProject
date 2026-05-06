<!-- src/views/etl/schedule/components/CronVisualPicker.vue -->
<template>
  <div class="cron-visual-picker">
    <!-- 频率选择 -->
    <el-form-item :label="t('etl.schedule.form.frequency')">
      <el-select v-model="frequency" style="width: 100%" @change="handleFrequencyChange">
        <el-option :label="t('etl.schedule.form.frequencyDaily')" value="daily" />
        <el-option :label="t('etl.schedule.form.frequencyWeekly')" value="weekly" />
        <el-option :label="t('etl.schedule.form.frequencyMonthly')" value="monthly" />
        <el-option :label="t('etl.schedule.form.frequencyHourly')" value="hourly" />
        <el-option :label="t('etl.schedule.form.frequencyNMinutes')" value="nMinutes" />
        <el-option :label="t('etl.schedule.form.frequencyCustom')" value="custom" />
      </el-select>
    </el-form-item>

    <!-- 每天：选择时间 -->
    <el-form-item v-if="frequency === 'daily'" :label="t('etl.schedule.form.executeTime')">
      <el-time-select
        v-model="dailyTime"
        start="00:00"
        step="00:30"
        end="23:30"
        :placeholder="t('etl.schedule.form.selectTime')"
        style="width: 100%"
      />
    </el-form-item>

    <!-- 每周：选择星期几 + 时间 -->
    <template v-if="frequency === 'weekly'">
      <el-form-item :label="t('etl.schedule.form.dayOfWeek')">
        <el-checkbox-group v-model="weeklyDays">
          <el-checkbox :label="1">{{ t('etl.schedule.form.weekdayMon') }}</el-checkbox>
          <el-checkbox :label="2">{{ t('etl.schedule.form.weekdayTue') }}</el-checkbox>
          <el-checkbox :label="3">{{ t('etl.schedule.form.weekdayWed') }}</el-checkbox>
          <el-checkbox :label="4">{{ t('etl.schedule.form.weekdayThu') }}</el-checkbox>
          <el-checkbox :label="5">{{ t('etl.schedule.form.weekdayFri') }}</el-checkbox>
          <el-checkbox :label="6">{{ t('etl.schedule.form.weekdaySat') }}</el-checkbox>
          <el-checkbox :label="0">{{ t('etl.schedule.form.weekdaySun') }}</el-checkbox>
        </el-checkbox-group>
      </el-form-item>
      <el-form-item :label="t('etl.schedule.form.executeTime')">
        <el-time-select
          v-model="weeklyTime"
          start="00:00"
          step="00:30"
          end="23:30"
          :placeholder="t('etl.schedule.form.selectTime')"
          style="width: 100%"
        />
      </el-form-item>
    </template>

    <!-- 每月：选择几号 + 时间 -->
    <template v-if="frequency === 'monthly'">
      <el-form-item :label="t('etl.schedule.form.dayOfMonth')">
        <el-select v-model="monthlyDay" style="width: 100%">
          <el-option v-for="d in 31" :key="d" :label="t('etl.schedule.form.dayN', { n: d })" :value="d" />
          <el-option :label="t('etl.schedule.form.lastDay')" value="L" />
        </el-select>
      </el-form-item>
      <el-form-item :label="t('etl.schedule.form.executeTime')">
        <el-time-select
          v-model="monthlyTime"
          start="00:00"
          step="00:30"
          end="23:30"
          :placeholder="t('etl.schedule.form.selectTime')"
          style="width: 100%"
        />
      </el-form-item>
    </template>

    <!-- 每小时：选择分钟 -->
    <el-form-item v-if="frequency === 'hourly'" :label="t('etl.schedule.form.minute')">
      <el-select v-model="hourlyMinute" style="width: 100%">
        <el-option :label="t('etl.schedule.form.minuteStart')" value="0" />
        <el-option :label="t('etl.schedule.form.minute15')" value="15" />
        <el-option :label="t('etl.schedule.form.minute30')" value="30" />
        <el-option :label="t('etl.schedule.form.minute45')" value="45" />
      </el-select>
    </el-form-item>

    <!-- 每N分钟 -->
    <el-form-item v-if="frequency === 'nMinutes'" :label="t('etl.schedule.form.everyNMinutes')">
      <el-input-number v-model="nMinutesValue" :min="1" :max="60" style="width: 100%" />
    </el-form-item>

    <!-- 自定义：显示原始输入框 -->
    <template v-if="frequency === 'custom'">
      <el-form-item :label="t('etl.schedule.form.cronExpression')" prop="cronExpression">
        <el-input
          v-model="customCron"
          :placeholder="t('etl.schedule.form.cronExpressionPlaceholder')"
        />
      </el-form-item>
      <el-form-item :label="t('etl.schedule.form.cronDescription')">
        <el-text type="info">{{ cronDescription || '-' }}</el-text>
      </el-form-item>
    </template>

    <!-- 显示生成的表达式预览 -->
    <el-form-item v-if="frequency !== 'custom'" :label="t('etl.schedule.form.preview')">
      <div class="cron-preview">
        <el-tag type="success" size="large">
          {{ generatedCronExpression }}
        </el-tag>
        <el-text type="info" class="preview-text">
          {{ humanReadableDescription }}
        </el-text>
      </div>
    </el-form-item>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useLocale } from '@/composables/useLocale'

const props = defineProps<{
  modelValue: string // Cron expression
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const { t } = useLocale()

// Frequency type
const frequency = ref('daily')
const customCron = ref('')

// Time values for each frequency type
const dailyTime = ref('08:00')
const weeklyDays = ref<number[]>([1]) // Monday by default
const weeklyTime = ref('08:00')
const monthlyDay = ref<number | string>(1)
const monthlyTime = ref('08:00')
const hourlyMinute = ref('0')
const nMinutesValue = ref(30)

// Weekday name mapping for display
const weekdayNames: Record<number, string> = {
  0: 'Sun',
  1: 'MON',
  2: 'TUE',
  3: 'WED',
  4: 'THU',
 5: 'FRI',
  6: 'SAT'
}

// Generate cron expression based on frequency (Quartz 6-field format)
// Quartz format: Second Minute Hour Day Month Weekday
// Quartz rule: Day and Weekday cannot both be *, one must be ?
const generatedCronExpression = computed(() => {
  switch (frequency.value) {
    case 'daily': {
      const [hour, minute] = dailyTime.value.split(':').map(Number)
      // Quartz: 秒 分钟 小时 日 月 周
      // 每天：日在每月都有，所以周用 ?
      return `0 ${minute} ${hour} * * ?`
    }
    case 'weekly': {
      const [hour, minute] = weeklyTime.value.split(':').map(Number)
      const days = weeklyDays.value.sort().map(d => weekdayNames[d]).join(',')
      // Quartz: 周有指定值，日用 ?
      return `0 ${minute} ${hour} ? * ${days}`
    }
    case 'monthly': {
      const [hour, minute] = monthlyTime.value.split(':').map(Number)
      const day = monthlyDay.value === 'L' ? 'L' : monthlyDay.value
      // Quartz: 日有指定值，周用 ?
      return `0 ${minute} ${hour} ${day} * ?`
    }
    case 'hourly': {
      // Quartz: 每小时在某分钟执行，日和月都是 *，周用 ?
      return `0 ${hourlyMinute.value} * * * ?`
    }
    case 'nMinutes': {
      // Quartz: 每 N 分钟，日和月都是 *，周用 ?
      return `0 */${nMinutesValue.value} * * * ?`
    }
    case 'custom': {
      return customCron.value
    }
    default:
      return '0 0 * * * ?'  // Quartz 默认每小时
  }
})

// Human readable description
const humanReadableDescription = computed(() => {
  switch (frequency.value) {
    case 'daily': {
      return t('etl.schedule.form.descDaily', { time: dailyTime.value })
    }
    case 'weekly': {
      const dayNames = weeklyDays.value
        .sort()
        .map(d => t(`etl.schedule.form.weekday${weekdayNames[d]}`))
        .join(', ')
      return t('etl.schedule.form.descWeekly', { days: dayNames, time: weeklyTime.value })
    }
    case 'monthly': {
      const dayText = monthlyDay.value === 'L'
        ? t('etl.schedule.form.lastDay')
        : t('etl.schedule.form.dayN', { n: monthlyDay.value })
      return t('etl.schedule.form.descMonthly', { day: dayText, time: monthlyTime.value })
    }
    case 'hourly': {
      return t('etl.schedule.form.descHourly', { minute: hourlyMinute.value })
    }
    case 'nMinutes': {
      return t('etl.schedule.form.descNMinutes', { n: nMinutesValue.value })
    }
    case 'custom': {
      return cronDescription.value
    }
    default:
      return ''
  }
})

// Simple cron description for custom (Quartz 6-field format)
const cronDescription = computed(() => {
  const cron = customCron.value
  // Try to match common patterns (Quartz 6-field)
  if (cron === '0 0 * * * ?') return t('etl.schedule.form.cronEveryHour')
  if (cron === '0 0 0 * * ?') return t('etl.schedule.form.cronEveryDay')
  if (cron === '0 0 0 ? * MON') return t('etl.schedule.form.cronEveryWeek')
  if (cron === '0 0 0 1 * ?') return t('etl.schedule.form.cronEveryMonth')
  // 每 N 分钟格式：0 */N * * * ?
  if (cron === '0 */1 * * * ?') return t('etl.schedule.form.cronEvery1Min')
  if (cron === '0 */30 * * * ?') return t('etl.schedule.form.cronEvery30Min')
  if (cron === '0 */15 * * * ?') return t('etl.schedule.form.cronEvery15Min')
  if (cron === '0 */5 * * * ?') return t('etl.schedule.form.cronEvery5Min')
  return t('etl.schedule.form.cronCustom')
})

// Emit cron expression when it changes
watch(generatedCronExpression, (val) => {
  emit('update:modelValue', val)
}, { immediate: true })

// Parse existing cron expression on mount or when modelValue changes
watch(() => props.modelValue, (val) => {
  if (!val) return
  parseCronExpression(val)
}, { immediate: true })

// Parse cron expression to set UI values
function parseCronExpression(cron: string) {
  const parts = cron.trim().split(' ')

  // Quartz 6-field format: Second Minute Hour Day Month Weekday
  if (parts.length === 6) {
    const [second, minute, hour, day, month, weekday] = parts

    // Hourly: 0 minute * * * ? (每小时在某分钟执行)
    if (hour === '*' && day === '*' && month === '*' && (weekday === '*' || weekday === '?')) {
      frequency.value = 'hourly'
      hourlyMinute.value = parseInt(minute) || 0
      return
    }

    // Every N minutes: 0 */N * * * ? (每N分钟执行)
    if (minute.startsWith('*/') && hour === '*' && day === '*' && month === '*' && (weekday === '*' || weekday === '?')) {
      frequency.value = 'nMinutes'
      const n = parseInt(minute.replace('*/', '')) || 30
      nMinutesValue.value = Math.min(60, Math.max(1, n))
      return
    }

    // Daily: 0 minute hour * * ? (每天在指定时间执行)
    if (day === '*' && month === '*' && (weekday === '*' || weekday === '?')) {
      frequency.value = 'daily'
      dailyTime.value = `${hour.padStart(2, '0')}:${minute.padStart(2, '0')}`
      return
    }

    // Weekly: 0 minute hour ? * weekday (每周在指定星期执行)
    if (day === '?' && month === '*' && weekday !== '*' && weekday !== '?') {
      frequency.value = 'weekly'
      weeklyTime.value = `${hour.padStart(2, '0')}:${minute.padStart(2, '0')}`
      // Parse weekdays
      const days = weekday.split(',').map(w => {
        const dayMap: Record<string, number> = { SUN: 0, MON: 1, TUE: 2, WED: 3, THU: 4, FRI: 5, SAT: 6 }
        return dayMap[w.toUpperCase()] ?? parseInt(w) ?? 1
      })
      weeklyDays.value = days.filter(d => d >= 0 && d <= 6)
      return
    }

    // Monthly: 0 minute hour day * ? (每月在指定日执行)
    if (weekday === '?' && month === '*' && day !== '*' && day !== '?') {
      frequency.value = 'monthly'
      monthlyTime.value = `${hour.padStart(2, '0')}:${minute.padStart(2, '0')}`
      monthlyDay.value = day === 'L' ? 'L' : parseInt(day) || 1
      return
    }

    // Unknown pattern - treat as custom
    frequency.value = 'custom'
    customCron.value = cron
    return
  }

  // Legacy 5-field format: Minute Hour Day Month Weekday
  if (parts.length === 5) {
    const [minute, hour, day, month, weekday] = parts

    // Hourly: minute * * * *
    if (hour === '*' && day === '*' && month === '*' && weekday === '*') {
      frequency.value = 'hourly'
      hourlyMinute.value = parseInt(minute) || 0
      return
    }

    // Every N minutes: */N * * * *
    if (minute.startsWith('*/') && hour === '*' && day === '*' && month === '*' && weekday === '*') {
      frequency.value = 'nMinutes'
      const n = parseInt(minute.replace('*/', '')) || 30
      nMinutesValue.value = Math.min(60, Math.max(1, n))
      return
    }

    // Daily: minute hour * * *
    if (day === '*' && month === '*' && (weekday === '*' || weekday === '?')) {
      frequency.value = 'daily'
      dailyTime.value = `${hour.padStart(2, '0')}:${minute.padStart(2, '0')}`
      return
    }

    // Weekly: minute hour ? * weekday
    if (day === '?' && month === '*' && weekday !== '*' && weekday !== '?') {
      frequency.value = 'weekly'
      weeklyTime.value = `${hour.padStart(2, '0')}:${minute.padStart(2, '0')}`
      const days = weekday.split(',').map(w => {
        const dayMap: Record<string, number> = { SUN: 0, MON: 1, TUE: 2, WED: 3, THU: 4, FRI: 5, SAT: 6 }
        return dayMap[w.toUpperCase()] ?? parseInt(w) ?? 1
      })
      weeklyDays.value = days.filter(d => d >= 0 && d <= 6)
      return
    }

    // Monthly: minute hour day * ?
    if (weekday === '?' && month === '*' && day !== '*' && day !== '?') {
      frequency.value = 'monthly'
      monthlyTime.value = `${hour.padStart(2, '0')}:${minute.padStart(2, '0')}`
      monthlyDay.value = day === 'L' ? 'L' : parseInt(day) || 1
      return
    }

    // Unknown pattern - treat as custom
    frequency.value = 'custom'
    customCron.value = cron
    return
  }

  // Other formats - treat as custom
  frequency.value = 'custom'
  customCron.value = cron
}

// Reset time values when frequency changes
function handleFrequencyChange() {
  // Keep default values, no need to reset
}
</script>

<style scoped lang="scss">
.cron-visual-picker {
  .cron-preview {
    display: flex;
    flex-direction: column;
    gap: 8px;

    .preview-text {
      font-size: 14px;
    }
  }

  .el-checkbox-group {
    display: flex;
    flex-wrap: wrap;
    gap: 8px;
  }
}
</style>
import $ from 'jquery'
import Games from './games'

$(function () {
  const switchSelector = '#schedule-visibility-switch'
  const schedule = new Games(['#schedule-container table tbody tr', '#schedule-container ul li'], switchSelector)
  $(switchSelector).click(function () {
    schedule.toggle()
  })
  schedule.hideMost()
})

import $ from 'jquery'
import Games from './games'

$(function () {
  const switchSelector = '#results-visibility-switch'
  const schedule = new Games(['#results-container table tbody tr', '#results-container ul li'], switchSelector)
  $(switchSelector).click(function () {
    schedule.toggle()
  })
  schedule.hideMost()
})

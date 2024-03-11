import $ from 'jquery'

function contentLoaded (container) {
  $(container).siblings().show()
}

$(document).ready(function () {
  const $schedule = $('#schedule')
  const scheduleTeamId = $schedule.attr('data-attr-team-id')
  $schedule.load(`/games/schedule/${scheduleTeamId}?embedded=true`, function (response, status, xhr) {
    if (status !== 'error') {
      contentLoaded($(this))
    }
  })

  const $standings = $('#standings')
  const standingsTeamId = $standings.attr('data-attr-team-id')
  $standings.load(`/groups/${standingsTeamId}?embedded=true`, function (response, status, xhr) {
    if (status !== 'error') {
      contentLoaded($(this))
    }
  })

  const $results = $('#results')
  const resultsTeamId = $results.attr('data-attr-team-id')
  $results.load(`/games/results/${resultsTeamId}?embedded=true`, function (response, status, xhr) {
    if (status !== 'error') {
      contentLoaded($(this))
    }
  })
})

import $ from 'jquery'

export default class Games {
  constructor (itemSelectors, switchSelector) {
    this.itemSelectors = itemSelectors
    this.$switch = $(switchSelector)
    this.switchText = this.$switch.text()
    this.alternativeText = this.$switch.attr('data-alternative-text')
    $.each(this.itemSelectors,
      function (index, selector) {
        $(selector).click((event) => {
          event.preventDefault()
          const target = $(event.currentTarget).attr('data-link-target')
          window.open(target, '_blank')
        })
      })
  }

  show (start, end) {
    $.each(this.itemSelectors,
      function (index, selector) {
        $(selector).slice(start, end).show()
      })
  }

  hide (start, end) {
    $.each(this.itemSelectors,
      function (index, selector) {
        $(selector).slice(start, end).hide()
      })
  }

  toggle () {
    if (this.$switch.text() === this.switchText) {
      this.showAll()
    } else {
      this.hideMost()
    }
  }

  showAll = function () {
    this.show(0)
    this.$switch.text(this.alternativeText)
  }

  hideMost = function () {
    this.show(0, 3)
    this.hide(3)
    this.$switch.text(this.switchText)
  }
}

var gamesModule = (function () {
    function Games(itemSelectors, switchSelector) {
        var $switch = $(switchSelector);

        var switchText = $switch.text();
        var alternativeText = $switch.attr('data-alternative-text');

        function show(start, end) {
            $.each(itemSelectors,
                function (index, selector) {
                    $(selector).slice(start, end).show();
                });

        }

        function hide(start, end) {
            $.each(itemSelectors,
                function (index, selector) {
                    $(selector).slice(start, end).hide();
                });
        }

        this.toggle = function() {
            if ($switch.text() === switchText) {
                this.showAll();
            } else {
                this.hideMost();
            }
        };

        this.showAll = function() {
            show(0);
            $switch.text(alternativeText);
        };

        this.hideMost = function() {
            show(0, 3);
            hide(3);
            $switch.text(switchText);
        };

    }
    return {
        games: function (itemSelector, switchSelector) { return new Games(itemSelector, switchSelector); }
    }
}());
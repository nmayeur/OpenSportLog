import $ from 'jquery';
require('jquery-ui/ui/widgets/button');
require('jquery-ui/themes/base/all.css');

let OSLTester = window.OSLTester || {};
(function () {
    'use strict'

    this.init = function () {
        //---------------
        // Resizable divs
        //---------------
        var dragging = false;
        $('#dragbar').mousedown(function (e) {
            e.preventDefault();

            dragging = true;
            var main = $('#main');
            var ghostbar = $('<div>',
                {
                    id: 'ghostbar',
                    css: {
                        height: main.outerHeight(),
                        top: main.offset().top,
                        left: main.offset().left
                    }
                }).appendTo('body');

            $(document).mousemove(function (e) {
                ghostbar.css("left", e.pageX + 2);
            });
        });

        $(document).mouseup(function (e) {
            if (dragging) {
                $('#sidebar').css("width", e.pageX + 2);
                $('#main').css("left", e.pageX + 2);
                $('#ghostbar').remove();
                $(document).unbind('mousemove');
                dragging = false;
            }
        });

        //--------
        // Buttons
        //--------
        $(function () {
            $("#sidebar input[type=submit], #sidebar a, #sidebar button").button();
            $("#sidebar #btnGenerateData").click(function (event) {
                let data = ['time', 'hr'];
                let base_timestamp = Date.now() - 1000 * 60 * 60 * 24;
                data.push([base_timestamp, 150 + Math.round(Math.random() * 30)]);

                for (let i = 1; i < 20000; i++) {
                    data.push([base_timestamp + i * 1000, Math.round(((Math.random() - 0.5) * 6) + data[i - 1][1])]);
                }
                OSL.loadData(data);
            });
            $("#sidebar #btnClear").click(function (event) {
                OSL.clear();
            });
        });

    }

}).call(OSLTester);

(function () {
    'use strict'
    window.addEventListener('load', function () {
        window.OSLTester = OSLTester;
        OSLTester.init();
    }, false)
}())

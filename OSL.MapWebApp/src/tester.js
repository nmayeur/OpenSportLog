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

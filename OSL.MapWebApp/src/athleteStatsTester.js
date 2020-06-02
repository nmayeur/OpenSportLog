import $ from 'jquery';

let MapTester = window.MapTester || {};
(function () {
    'use strict'

    this.init = function () {

        //--------
        // Buttons
        //--------
        $(function () {
            $("#sidebar #btnGenerateData").click(function (event) {
                let data = [['time', 'hr', 'cadence', 'elevation']];
                let base_timestamp = Date.now() - 1000 * 60 * 60 * 24;
                data.push([base_timestamp,
                    150 + Math.round(Math.random() * 30),
                    80 + Math.round(Math.random() * 20),
                    1000 + Math.round(Math.random() * 200)
                ]);

                for (let i = 2; i < 20000; i++) {
                    data.push([base_timestamp + i * 1000,
                        Math.round(((Math.random() - 0.5) * 6) + data[i - 1][1]),
                        Math.round(((Math.random() - 0.5) * 3) + data[i - 1][2]),
                        Math.round(((Math.random() - 0.5) * 20) + data[i - 1][3])]);
                }
                OSL.loadData(data);
            });
            $("#sidebar #btnClear").click(function (event) {
                OSL.clear();
            });
        });

    }

}).call(MapTester);

(function () {
    'use strict'
    window.addEventListener('load', function () {
        window.MapTester = MapTester;
        MapTester.init();
    }, false)
}())

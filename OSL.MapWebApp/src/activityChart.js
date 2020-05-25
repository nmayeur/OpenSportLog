import * as d3 from 'd3';

let OSL = window.OSL || {};
(function () {
    'use strict'

    let _hrData = [];

    this.loadData = function (hrData) {
        _hrData = hrData;
    }
    this.drawHeartRate = function () {
        // create svg element:
        let svg = d3.select("#dataviz").append("svg").attr("width", 800).attr("height", 200)

        // prepare a helper function
        var curveFunc = d3.line()
            .curve(d3.curveBasis)              // This is where you define the type of curve. Try curveStep for instance.
            .x(function (d) { return d.x })
            .y(function (d) { return d.y })

        // Add the path using this helper function
        svg.append('path')
            .attr('d', curveFunc(_hrData))
            .attr('stroke', 'red')
            .attr('fill', 'none');

    }

}).call(OSL);

(function () {
    'use strict'
    window.addEventListener('load', function () {
        window.OSL = OSL;

        //OSL.loadData([{ x: 0, y: 20 }, { x: 150, y: 150 }, { x: 300, y: 100 }, { x: 450, y: 20 }, { x: 600, y: 130 }]);
        //OSL.drawHeartRate();


    }, false)
}())

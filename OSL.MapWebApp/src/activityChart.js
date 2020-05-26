import * as d3 from 'd3';

let OSL = window.OSL || {};
(function () {
    'use strict'

    let _hrData = [];
    let svg;

    this.loadData = function (hrData) {
        _hrData = hrData;
        this.clear();
    }

    this.clear = function () {
        svg.selectAll("*").remove();
    }

    this.drawHeartRate = function () {

        // prepare a helper function
        var curveFunc = d3.line()
            .curve(d3.curveBasis)
            .x(function (d) { return d.x })
            .y(function (d) { return d.y })

        // Add the path using this helper function
        svg.append('path')
            .attr('d', curveFunc(_hrData))
            .attr('stroke', 'red')
            .attr('fill', 'none');

    }

    this.init = function () {
        // create svg element
        svg = d3.select("#dataviz").append("svg").attr("width", 800).attr("height", 200)
    }

}).call(OSL);

(function () {
    'use strict'
    window.addEventListener('load', function () {
        window.OSL = OSL;
        OSL.init();
    }, false)
}())

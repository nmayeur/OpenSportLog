import $ from 'jquery';
let echarts = require('echarts/lib/echarts');
require('echarts/lib/chart/line');
require('echarts/lib/component/tooltip');
require('echarts/lib/component/legend');
require('echarts/lib/component/toolbox');
require('echarts/lib/component/dataZoom');

let OSL = window.OSL || {};
(function () {
    'use strict'

    let chart = null;

    function _PrepareLegend() {
        let labels = {};
        if ($("#hr").is(":checked")) labels.hr = "HR";
        if ($("#cadence").is(":checked")) labels.cadence = "Cadence";
        if ($("#elevation").is(":checked")) labels.elevation = "Elevation";
        if ($("#power").is(":checked")) labels.power = "Power";
        if ($("#temperature").is(":checked")) labels.temperature = "Temperature";
        if ($("#distance").is(":checked")) labels.distance = "Distance";
        if ($("#duration").is(":checked")) labels.duration = "Duration";
        return labels;
    }

    function _PrepareAxis() {
        let axis = {};
        axis.hr = $("#hr_axe").val();
        axis.cadence = $("#cadence_axe").val();
        axis.elevation = $("#elevation_axe").val();
        axis.power = $("#power_axe").val();
        axis.temperature = $("#temperature_axe").val();
        axis.distance = $("#distance_axe").val();
        axis.duration = $("#duration_axe").val();
        return axis;
    }

    this.loadData = function (_data) {
        alert(_data);
        this.drawChart({ data: _PrepareData(_data), legends: _PrepareLegend(), axis: _PrepareAxis() });
    }

    this.clear = function () {
        this.drawChart([]);
    }

    let _userLocale = "en-US";//Default value
    let _userTimezone = "UTC";//Default value

    this.drawChart = function (config) {
        let legends = [];
        let labels = config.labels;
        if (labels.hr) legends.push(labels.hr);
        if (labels.cadence) legends.push(labels.cadence);
        if (labels.elevation) legends.push(labels.elevation);
        if (labels.power) legends.push(labels.power);
        if (labels.temperature) legends.push(labels.temperature);
        if (labels.distance) legends.push(labels.distance);
        if (labels.duration) legends.push(labels.duration);


        let series = [];
        let yAxisLabelsLeft = "";
        let yAxisLabelsRight = "";
        let axis = config.axis;
        if (axis.hr && labels.hr) {
            if (axis.hr == "left") {
                yAxisLabelsLeft += labels.hr;
            } else {
                yAxisLabelsRight += labels.hr;
            }
            series.push({
                name: labels.hr,
                type: 'line',
                smooth: true,
                yAxisIndex: axis.hr == "right" ? 1 : 0,
                symbol: 'none',
                sampling: 'average',
                itemStyle: {
                    color: 'rgb(255, 70, 131)'
                },
                connectNulls: true,
                areaStyle: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                        offset: 0,
                        color: 'rgb(255, 158, 68)'
                    }, {
                        offset: 1,
                        color: 'rgb(255, 70, 131)'
                    }])
                },
                encode: { x: 'time', y: 'hr' }
            });
        }
        if (axis.cadence && labels.cadence) {
            if (axis.cadence == "left") {
                if (yAxisLabelsLeft) yAxisLabelsLeft += " / ";
                yAxisLabelsLeft += labels.cadence;
            } else {
                if (yAxisLabelsRight) yAxisLabelsRight += " / ";
                yAxisLabelsRight += labels.cadence;
            }
            series.push({
                name: labels.cadence,
                type: 'line',
                smooth: true,
                yAxisIndex: axis.cadence == "right" ? 1 : 0,
                symbol: 'none',
                sampling: 'average',
                itemStyle: {
                    color: 'rgb(54, 32, 255)'
                },
                connectNulls: true,
                areaStyle: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                        offset: 0,
                        color: 'rgb(190, 183, 255)'
                    }, {
                        offset: 1,
                        color: 'rgb(54, 32, 255)'
                    }])
                },
                encode: { x: 'time', y: 'cadence' }
            });
        }
        if (axis.elevation && labels.elevation) {
            if (axis.elevation == "left") {
                if (yAxisLabelsLeft) yAxisLabelsLeft += " / ";
                yAxisLabelsLeft += labels.elevation;
            } else {
                if (yAxisLabelsRight) yAxisLabelsRight += " / ";
                yAxisLabelsRight += labels.elevation;
            }
            series.push({
                name: labels.elevation,
                type: 'line',
                smooth: true,
                yAxisIndex: axis.elevation == "right" ? 1 : 0,
                symbol: 'none',
                sampling: 'average',
                itemStyle: {
                    color: 'rgb(245, 238, 95)'
                },
                connectNulls: true,
                areaStyle: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                        offset: 0,
                        color: 'rgb(251, 248, 187)'
                    }, {
                        offset: 1,
                        color: 'rgb(245, 238, 95)'
                    }])
                },
                encode: { x: 'time', y: 'elevation' }
            });
        }
        if (axis.calories && labels.calories) {
            if (axis.calories == "left") {
                if (yAxisLabelsLeft) yAxisLabelsLeft += " / ";
                yAxisLabelsLeft += labels.elevation;
            } else {
                if (yAxisLabelsRight) yAxisLabelsRight += " / ";
                yAxisLabelsRight += labels.elevation;
            }
            series.push({
                name: labels.calories,
                type: 'line',
                smooth: true,
                yAxisIndex: axis.calories == "right" ? 1 : 0,
                symbol: 'none',
                sampling: 'average',
                itemStyle: {
                    color: 'rgb(245, 238, 95)'
                },
                connectNulls: true,
                areaStyle: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                        offset: 0,
                        color: 'rgb(251, 248, 187)'
                    }, {
                        offset: 1,
                        color: 'rgb(245, 238, 95)'
                    }])
                },
                encode: { x: 'time', y: 'calories' }
            });
        }
        if (axis.power && labels.power) {
            if (axis.power == "left") {
                if (yAxisLabelsLeft) yAxisLabelsLeft += " / ";
                yAxisLabelsLeft += labels.power;
            } else {
                if (yAxisLabelsRight) yAxisLabelsRight += " / ";
                yAxisLabelsRight += labels.power;
            }
            series.push({
                name: 'Power',
                type: 'line',
                smooth: true,
                yAxisIndex: axis.power == "right" ? 1 : 0,
                symbol: 'none',
                sampling: 'average',
                itemStyle: {
                    color: 'rgb(255, 0, 0)'
                },
                connectNulls: true,
                areaStyle: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                        offset: 0,
                        color: 'rgb(190, 183, 255)'
                    }, {
                        offset: 1,
                        color: 'rgb(255, 100, 0)'
                    }])
                },
                encode: { x: 'time', y: 'power' }
            });
        }
        if (axis.temperature && labels.temperature) {
            if (axis.temperature == "left") {
                if (yAxisLabelsLeft) yAxisLabelsLeft += " / ";
                yAxisLabelsLeft += labels.temperature;
            } else {
                if (yAxisLabelsRight) yAxisLabelsRight += " / ";
                yAxisLabelsRight += labels.temperature;
            }
            series.push({
                name: labels.temperature,
                type: 'line',
                smooth: true,
                yAxisIndex: axis.temperature == "right" ? 1 : 0,
                symbol: 'none',
                sampling: 'average',
                itemStyle: {
                    color: 'rgb(225, 255, 0)'
                },
                connectNulls: true,
                areaStyle: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                        offset: 0,
                        color: 'rgb(0, 255, 253)'
                    }, {
                        offset: 1,
                        color: 'rgb(225, 255, 0)'
                    }])
                },
                encode: { x: 'time', y: 'temperature' }
            });
        }
        if (axis.distance && labels.distance) {
            if (axis.distance == "left") {
                if (yAxisLabelsLeft) yAxisLabelsLeft += " / ";
                yAxisLabelsLeft += labels.distance;
            } else {
                if (yAxisLabelsRight) yAxisLabelsRight += " / ";
                yAxisLabelsRight += labels.distance;
            }
            series.push({
                name: labels.distance,
                type: 'line',
                smooth: true,
                yAxisIndex: axis.distance == "right" ? 1 : 0,
                symbol: 'none',
                sampling: 'average',
                itemStyle: {
                    color: 'rgb(225, 255, 0)'
                },
                connectNulls: true,
                areaStyle: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                        offset: 0,
                        color: 'rgb(0, 255, 253)'
                    }, {
                        offset: 1,
                        color: 'rgb(225, 255, 0)'
                    }])
                },
                encode: { x: 'time', y: 'distance' }
            });
        }
        if (axis.duration && labels.duration) {
            if (axis.duration == "left") {
                if (yAxisLabelsLeft) yAxisLabelsLeft += " / ";
                yAxisLabelsLeft += labels.duration;
            } else {
                if (yAxisLabelsRight) yAxisLabelsRight += " / ";
                yAxisLabelsRight += labels.duration;
            }
            series.push({
                name: labels.duration,
                type: 'line',
                smooth: true,
                yAxisIndex: axis.duration == "right" ? 1 : 0,
                symbol: 'none',
                sampling: 'average',
                itemStyle: {
                    color: 'rgb(225, 255, 0)'
                },
                connectNulls: true,
                areaStyle: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                        offset: 0,
                        color: 'rgb(0, 255, 253)'
                    }, {
                        offset: 1,
                        color: 'rgb(225, 255, 0)'
                    }])
                },
                encode: { x: 'time', y: 'duration' }
            });
        }

        chart = echarts.init(document.getElementById('main'));

        let option = {
            tooltip: {
                trigger: 'axis',
                position: function (pt) {
                    return [pt[0], '10%'];
                }
            },
            toolbox: {
                feature: {
                    dataZoom: {
                        yAxisIndex: 'none',
                        title: {
                            zoom: 'Area zooming',
                            back: 'Restore area zooming'
                        },
                        show: true
                    },
                    restore: {
                        title: 'Restore'
                    },
                    saveAsImage: {
                        title: 'Save as image'
                    }
                }
            },
            dataset: {
                source: config.data
            },
            legend: {
                data: config.legends
            },
            xAxis: {
                type: 'time',
                axisLabel: {
                    formatter: function (value, index) {
                        let date = new Date(value);
                        return date.getFullYear() + "-" + ("0" + (date.getMonth() + 1).toString()).substring(0, 2);
                    }
                }
            },
            yAxis: [{
                type: 'value',
                name: yAxisLabelsLeft
            }, {
                type: 'value',
                    name: yAxisLabelsRight
            }],
            dataZoom: [{
                type: 'inside',
                start: 0,
                end: 100
            }, {
                start: 0,
                end: 10,
                handleIcon: 'M10.7,11.9v-1.3H9.3v1.3c-4.9,0.3-8.8,4.4-8.8,9.4c0,5,3.9,9.1,8.8,9.4v1.3h1.3v-1.3c4.9-0.3,8.8-4.4,8.8-9.4C19.5,16.3,15.6,12.2,10.7,11.9z M13.3,24.4H6.7V23h6.6V24.4z M13.3,19.6H6.7v-1.4h6.6V19.6z',
                handleSize: '80%',
                handleStyle: {
                    color: '#fff',
                    shadowBlur: 3,
                    shadowColor: 'rgba(0, 0, 0, 0.6)',
                    shadowOffsetX: 2,
                    shadowOffsetY: 2
                }
            }],
            series: series
        };

        chart.setOption(option, true);

        chart.on('dataZoom', function (evt) {
            let axis = chart.getModel().option.xAxis[0];
            let message = {
                type: "datazoom",
                startTime: axis.rangeStart,
                endTime: axis.rangeEnd,
                startPercent: evt.start,
                endPercent: evt.end
            }
            console.log('zoom', message);
            if (typeof CefSharp !== "undefined") CefSharp.PostMessage(message);
        })

    }

    this.init = function (options) {
        if (options) {
            if (options.userLocale) _userLocale = options.userLocale;
            if (options.userTimezone) _userTimezone = options.userTimezone;
        }
        this.drawChart([], {}, {});
        $(window).on('resize', function () {
            if (chart != null && chart != undefined) {
                chart.resize();
            }
        });
    }

}).call(OSL);

(function () {
    'use strict'
    window.addEventListener('load', function () {
        window.OSL = OSL;
        OSL.init({ userLocale: "fr-FR" });
    }, false)
}())

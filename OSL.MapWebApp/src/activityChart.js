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

    let _hrData = [['time', 'hr', 'candence', 'elevation', 'power', 'temperature']];
    let chart = null;
    this.loadData = function (hrData) {
        _hrData = hrData;
        this.drawChart();
    }

    let _userLocale = "en-US";//Default value
    let _userTimezone = "UTC";//Default value

    this.clear = function () {
        _hrData = [];
        this.drawChart();
    }

    this.drawChart = function () {
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
                source: _hrData
            },
            legend: {
                data: ['HR', 'Cadence', 'Elevation', 'Power', 'Temperature']
            },
            xAxis: {
                type: 'time',
                axisLabel: {
                    formatter: function (value, index) {
                        let date = new Date(value);
                        //return date.toLocaleTimeString(_userLocale, { timeZone: _userLocale });
                        return date.toLocaleTimeString(_userLocale, { timeZone: _userTimezone });
                    }
                }
            },
            yAxis: [{
                type: 'value',
                name: 'HT / Cad / Power / Temp'
            }, {
                type: 'value',
                name: 'Elevation'
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
            series: [
                {
                    name: 'HR',
                    type: 'line',
                    smooth: true,
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
                }, {
                    name: 'Cadence',
                    type: 'line',
                    smooth: true,
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
                }, {
                    name: 'Elevation',
                    type: 'line',
                    smooth: true,
                    yAxisIndex: 1,
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
                }, {
                    name: 'Power',
                    type: 'line',
                    smooth: true,
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
                }, {
                    name: 'Temperature',
                    type: 'line',
                    smooth: true,
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
                }
            ]
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
        this.drawChart();
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

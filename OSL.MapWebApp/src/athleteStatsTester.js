import $ from 'jquery';
require('jquery-ui');

let MapTester = window.MapTester || {};
(function () {
    'use strict'
    let _data = [[]];
    function _GenerateTestData() {
        let data = [];
        let now = new Date();
        let base_timestamp = new Date(now.getUTCFullYear() - 1, now.getUTCMonth(), 1).valueOf();
        data.push({
            time: base_timestamp,
            hr: 150 + Math.round(Math.random() * 30),
            cadence: 80 + Math.round(Math.random() * 20),
            elevation: 1000 + Math.round(Math.random() * 200),
            power: 150 + Math.round(Math.random() * 20),
            temperature: 15 + Math.round(Math.random() * 3),
            distance: 150 + Math.round(Math.random() * 20),
            duration: 3 + Math.round(Math.random() * 20)
        });

        for (let i = 1; i < 24; i++) {
            base_timestamp = new Date(now.getUTCFullYear() - 1, now.getUTCMonth() + i, 1).valueOf();
            data.push({
                time: base_timestamp,
                hr: Math.round(((Math.random() - 0.5) * 6) + data[i - 1].hr),
                cadence: Math.round(((Math.random() - 0.5) * 3) + data[i - 1].cadence),
                elevation: Math.round(((Math.random() - 0.5) * 20) + data[i - 1].elevation),
                power: Math.round(((Math.random() - 0.5) * 2) + data[i - 1].power),
                temperature: Math.round(((Math.random() - 0.5) * 2) + data[i - 1].temperature),
                distance: Math.round(((Math.random() - 0.5) * 20) + data[i - 1].distance),
                duration: Math.round(((Math.random() - 0.5) * 3) + data[i - 1].duration)
            });
        }
        return data;
    }

    function _PrepareData(data) {
        let formattedData = [];
        let headers = ["time"];
        if ($("#hr").is(":checked")) headers.push("hr");
        if ($("#cadence").is(":checked")) headers.push("cadence");
        if ($("#elevation").is(":checked")) headers.push("elevation");
        if ($("#power").is(":checked")) headers.push("power");
        if ($("#temperature").is(":checked")) headers.push("temperature");
        if ($("#distance").is(":checked")) headers.push("distance");
        if ($("#duration").is(":checked")) headers.push("duration");
        formattedData.push(headers);
        for (let i = 0; i < data.length; i++) {
            let lineData = [data[i].time];
            if ($("#hr").is(":checked")) lineData.push(data[i].hr);
            if ($("#cadence").is(":checked")) lineData.push(data[i].cadence);
            if ($("#elevation").is(":checked")) lineData.push(data[i].elevation);
            if ($("#power").is(":checked")) lineData.push(data[i].power);
            if ($("#temperature").is(":checked")) lineData.push(data[i].temperature);
            if ($("#distance").is(":checked")) lineData.push(data[i].distance);
            if ($("#duration").is(":checked")) lineData.push(data[i].duration);
            formattedData.push(lineData);
        }
        return formattedData;
    }

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

    this.init = function () {

        //--------
        // Buttons
        //--------
        $(function () {
            $("#sidebar #btnGenerateData").click(function (event) {
                _data = _GenerateTestData();
                OSL.drawChart({ data: _PrepareData(_data), legends: _PrepareLegend(), axis: _PrepareAxis() });
            });
            $("#sidebar #btnClear").click(function (event) {
                OSL.clear();
            });
            $("#sidebar select").on('change', function () {
                OSL.drawChart({ data: _PrepareData(_data), legends: _PrepareLegend(), axis: _PrepareAxis() });
            });
            $('#sidebar :checkbox').on('change', function () {
                OSL.drawChart({ data: _PrepareData(_data), legends: _PrepareLegend(), axis: _PrepareAxis() });
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

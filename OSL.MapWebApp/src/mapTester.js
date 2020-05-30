import $ from 'jquery';
import { polyline } from 'leaflet';

let MapTester = window.MapTester || {};
(function () {
    'use strict'
    const START_MARKER = 1;
    const END_MARKER = 2;

    this.init = function () {

        var _polylinePoints = {
            "type": "FeatureCollection",
            "features": [
                {
                    "type": "Feature",
                    "geometry": {
                        "type": "LineString",
                        "coordinates": [
                            [-122.48369693756104, 37.83381888486939],
                            [-122.48348236083984, 37.83317489144141],
                            [-122.48339653015138, 37.83270036637107],
                            [-122.48356819152832, 37.832056363179625],
                            [-122.48404026031496, 37.83114119107971],
                            [-122.48404026031496, 37.83049717427869],
                            [-122.48348236083984, 37.829920943955045],
                            [-122.48356819152832, 37.82954808664175],
                            [-122.48507022857666, 37.82944639795659],
                            [-122.48610019683838, 37.82880236636284],
                            [-122.48695850372314, 37.82931081282506],
                            [-122.48700141906738, 37.83080223556934],
                            [-122.48751640319824, 37.83168351665737],
                            [-122.48803138732912, 37.832158048267786],
                            [-122.48888969421387, 37.83297152392784],
                            [-122.48987674713133, 37.83263257682617],
                            [-122.49043464660643, 37.832937629287755],
                            [-122.49125003814696, 37.832429207817725],
                            [-122.49163627624512, 37.832564787218985],
                            [-122.49223709106445, 37.83337825839438],
                            [-122.49378204345702, 37.83368330777276]
                        ]
                    },
                    "properties": {
                        "popupContent": "test popup",
                        "zoomed": false
                    },
                    "id": 1
                }
            ]
        };


        //--------
        // Buttons
        //--------
        $(function () {
            $("#sidebar #btnGotoStart").click(function (event) {
                OSL.goToCoordinates(_polylinePoints.features[0].geometry.coordinates[0][1], _polylinePoints.features[0].geometry.coordinates[0][0]);
            });
            $("#sidebar #btnDrawRoute").click(function (event) {
                OSL.drawRoute(_polylinePoints);
            });
            $("#sidebar #btnDrawZoomedRoute").click(function (event) {
                let countPoints = _polylinePoints.features[0].geometry.coordinates.length;
                OSL.drawZoomedRoute(polylinePoints.slice(Math.round(countPoints * 0.2), Math.round(countPoints * 0.8)));
            });
            $("#sidebar #btnDrawMarkers").click(function (event) {
                OSL.setMarker(_polylinePoints.features[0].geometry.coordinates[0][1], _polylinePoints.features[0].geometry.coordinates[0][0], START_MARKER);
                let countLines = _polylinePoints.features.length;
                let countPoints = _polylinePoints.features[countLines - 1].geometry.coordinates.length;
                OSL.setMarker(_polylinePoints.features[countLines - 1].geometry.coordinates[countPoints - 1][1], _polylinePoints.features[countLines - 1].geometry.coordinates[countPoints - 1][0], END_MARKER);
            });
            $("#sidebar #btnClear").click(function (event) {
                OSL.cleanMap();
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

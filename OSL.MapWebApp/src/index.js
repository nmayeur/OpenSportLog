import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import './styles/site.scss';

import iconRetinaUrl from 'leaflet/dist/images/marker-icon-2x.png';
import iconUrl from 'leaflet/dist/images/marker-icon.png';
import shadowUrl from 'leaflet/dist/images/marker-shadow.png';


let OSL = window.OSL || {};
(function () {
    'use strict'

    let _map = null;
    let _startMarker = null;
    let _endMarker = null;
    const START_MARKER = 1;
    const END_MARKER = 2;
    let _polyline = null;

    this.goToCoordinates = function (latitude, longitude) {
        if (!_map) throw "OSL map is not initialized";
        _map.setView([latitude, longitude], 13);
    }
    this.setMarker = function (latitude, longitude, type) {
        if (!_map) throw "OSL map is not initialized";
        let marker = type == START_MARKER ? _startMarker : _endMarker;
        if (marker) _map.removeLayer(marker);
        marker = L.marker([latitude, longitude]);
        marker.addTo(_map);
        if (type == START_MARKER) {
            _startMarker = marker;
        } else {
            _endMarker = marker;
        }
    }
    this.drawRoute = function (latlngs) {
        if (!_map) throw "OSL map is not initialized";

        if (_polyline) _map.removeLayer(_polyline);
        _polyline = L.polyline(latlngs).addTo(_map);
        _map.fitBounds(_polyline.getBounds());
    }
    this.cleanMap = function () {
        if (_polyline) _map.removeLayer(_polyline);
        if (_startMarker) _map.removeLayer(_startMarker);
        if (_endMarker) _map.removeLayer(_endMarker);
        _polyline = null;
        _startMarker = null;
        _endMarker = null;
    }

    this.init = function (map) {
        _map = map;
    }
}).call(OSL);

(function () {
    'use strict'
    /* This code is needed to properly load the images in the Leaflet CSS */
    delete L.Icon.Default.prototype._getIconUrl;
    L.Icon.Default.mergeOptions({
        iconRetinaUrl: iconRetinaUrl,
        iconUrl: iconUrl,
        shadowUrl: shadowUrl
    });

    const map = L.map('map');
    const defaultCenter = [38.889269, -77.050176];
    const defaultZoom = 15;
    const basemap = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}', {
        attribution: 'Tiles &copy; Esri &mdash; Esri, DeLorme, NAVTEQ, TomTom, Intermap, iPC, USGS, FAO, NPS, NRCAN, GeoBase, Kadaster NL, Ordnance Survey, Esri Japan, METI, Esri China (Hong Kong), and the GIS User Community'
    });

    map.setView(defaultCenter, defaultZoom);

    basemap.addTo(map);

    window.addEventListener('load', function () {
        window.OSL = OSL;

        OSL.init(map);
        //OSL.goToCoordinates(48.875, 2.22);
        //OSL.drawRoute([[48.875, 2.22], [48.8, 2.0]]);
    }, false)
}())

﻿import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import './styles/map.scss';

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
    let _lineString = null;
    let _zoomedLineString = null;
    let _basemap;

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
    this.drawRoute = function (geoJson) {
        if (!_map) throw "OSL map is not initialized";

        if (_lineString) _map.removeLayer(_lineString);
        _lineString = L.geoJSON(geoJson).addTo(_map);
        _map.fitBounds(_lineString.getBounds());
    }
    this.drawZoomedRoute = function (geoJson) {
        if (!_map) throw "OSL map is not initialized";

        if (_zoomedLineString) _map.removeLayer(_zoomedLineString);
        _zoomedLineString = L.geoJSON(geoJson);
        _zoomedLineString.setStyle({
            color: 'black'
        });
        _zoomedLineString.addTo(_map);
        _map.fitBounds(_zoomedLineString.getBounds());
    }
    this.cleanMap = function () {
        if (_lineString) _map.removeLayer(_lineString);
        if (_zoomedLineString) _map.removeLayer(_zoomedLineString);
        if (_startMarker) _map.removeLayer(_startMarker);
        if (_endMarker) _map.removeLayer(_endMarker);
        _lineString = null;
        _zoomedLineString = null;
        _startMarker = null;
        _endMarker = null;
    }
    this.changeBaseMap = function (url, attribution) {
        _map.removeLayer(_basemap);
        _basemap = L.tileLayer(url, { attribution: attribution });
        _basemap.addTo(_map);
    }

    this.init = function (map) {
        _map = map;
        const defaultCenter = [38.889269, -77.050176];
        const defaultZoom = 15;
        _basemap = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}', {
            attribution: 'Tiles &copy; Esri &mdash; Esri, DeLorme, NAVTEQ, TomTom, Intermap, iPC, USGS, FAO, NPS, NRCAN, GeoBase, Kadaster NL, Ordnance Survey, Esri Japan, METI, Esri China (Hong Kong), and the GIS User Community'
        });
        _map.setView(defaultCenter, defaultZoom);
        _basemap.addTo(_map);
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

    const map = L.map('main');

    window.addEventListener('load', function () {
        window.OSL = OSL;

        OSL.init(map);
    }, false)
}())

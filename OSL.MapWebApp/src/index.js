import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

let OSL = window.OSL || {};
(function () {
    'use strict'

    let _map = null;

    this.goToCoordinates = function (latitude, longitude) {
        if (!_map) throw "OSL map is not initialized";
        _map.setView([latitude, longitude], 13);
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
        iconRetinaUrl: require('leaflet/dist/images/marker-icon-2x.png'),
        iconUrl: require('leaflet/dist/images/marker-icon.png'),
        shadowUrl: require('leaflet/dist/images/marker-shadow.png'),
    });

    const map = L.map('map');
    const defaultCenter = [38.889269, -77.050176];
    const defaultZoom = 15;
    const basemap = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}', {
        attribution: 'Tiles &copy; Esri &mdash; Esri, DeLorme, NAVTEQ, TomTom, Intermap, iPC, USGS, FAO, NPS, NRCAN, GeoBase, Kadaster NL, Ordnance Survey, Esri Japan, METI, Esri China (Hong Kong), and the GIS User Community'
    });
    const marker = L.marker(defaultCenter);

    map.setView(defaultCenter, defaultZoom);

    basemap.addTo(map);
    marker.addTo(map);

    window.addEventListener('load', function () {
        window.OSL = OSL;
        OSL.init(map);
        OSL.goToCoordinates(48.87, 2.22);
    }, false)
}())

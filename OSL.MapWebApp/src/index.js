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

    this.goToCoordinates = function (latitude, longitude) {
        if (!_map) throw "OSL map is not initialized";
        _map.setView([latitude, longitude], 13);
        if (_startMarker != null) _startMarker.remove();
        _startMarker = L.marker([48.87, 2.22]);
        _startMarker.addTo(_map);
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
        OSL.goToCoordinates(48.875, 2.22);
    }, false)
}())

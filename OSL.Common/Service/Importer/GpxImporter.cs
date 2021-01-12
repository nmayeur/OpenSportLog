/* Copyright 2021 Nicolas Mayeur

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using OSL.Common.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace OSL.Common.Service.Importer
{
    public class GpxImporter : IActivitiesImporter
    {
        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();

        private enum PARSE_CONTEXT
        {
            ROOT, GPX, METADATA, ACTIVITY_TIME, TRACK, TRACK_NAME, TRACK_SEGMENT, TRACK_POINT, TRACK_TYPE,
            TRACK_POINT_TIME, TRACK_POINT_ELEVATION, TRACK_POINT_POWER, TRACK_POINT_HR, TRACK_POINT_TEMPERATURE, TRACK_POINT_CADENCE,
            TRACK_POINT_EXTENSIONS, TRACK_POINT_GPXTPX_EXTENSION
        }
        private PARSE_CONTEXT _CurrentContext = PARSE_CONTEXT.ROOT;
        private ActivityEntity.Builder _CurrentActivityBuilder = null;
        private TrackEntity.Builder _CurrentTrackBuilder = null;
        private TrackSegmentEntity.Builder _CurrentTrackSegmentBuilder = null;
        private TrackPointVO.Builder _CurrentTrackPointBuilder = null;
        private string _CurrentCreator = null;
        private Dictionary<string, string> _XmlNamespaces = new Dictionary<string, string>();

        public IEnumerable<ActivityEntity> ImportActivitiesStream(Stream stream, IDictionary<string, ACTIVITY_SPORT> categoryMapping)
        {
            _Logger.Info("Importing data stream");
            _CurrentContext = PARSE_CONTEXT.ROOT;
            XmlReaderSettings settings = new XmlReaderSettings
            {
                Async = false
            };
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                // SAX parsing for performance
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "gpx":
                                    _CurrentContext = PARSE_CONTEXT.GPX;
                                    _CurrentCreator = reader.GetAttribute("creator");
                                    _CurrentActivityBuilder = new ActivityEntity.Builder
                                    {
                                        OriginSystem = _CurrentCreator
                                    };
                                    while (reader.MoveToNextAttribute())
                                    {
                                        if (reader.Name.StartsWith("xmlns:") && reader.Value.ToLower() == "http://www.garmin.com/xmlschemas/trackpointextension/v1")
                                        {
                                            _XmlNamespaces["gpxtpx"] = reader.Name.Replace("xmlns:", "") + ":";
                                        }
                                    }
                                    break;
                                case "metadata":
                                    _CurrentContext = PARSE_CONTEXT.METADATA;
                                    break;
                                case "time":
                                    if (_CurrentContext == PARSE_CONTEXT.METADATA)
                                    {
                                        _CurrentContext = PARSE_CONTEXT.ACTIVITY_TIME;
                                    }
                                    else if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT)
                                    {
                                        _CurrentContext = PARSE_CONTEXT.TRACK_POINT_TIME;
                                    }
                                    break;
                                case "trk":
                                    _CurrentContext = PARSE_CONTEXT.TRACK;
                                    _CurrentTrackBuilder = new TrackEntity.Builder();
                                    break;
                                case "name":
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK) _CurrentContext = PARSE_CONTEXT.TRACK_NAME;
                                    break;
                                case "trkseg":
                                    _CurrentContext = PARSE_CONTEXT.TRACK_SEGMENT;
                                    _CurrentTrackSegmentBuilder = new TrackSegmentEntity.Builder();
                                    break;
                                case "trkpt":
                                    _CurrentContext = PARSE_CONTEXT.TRACK_POINT;
                                    float latitude;
                                    var style = System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign;
                                    var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
                                    if (!float.TryParse(reader.GetAttribute("lat"), style, culture, out latitude))
                                    {
                                        break;
                                    }
                                    float longitude;
                                    if (!float.TryParse(reader.GetAttribute("lon"), style, culture, out longitude))
                                    {
                                        break;
                                    }
                                    _CurrentTrackPointBuilder = new TrackPointVO.Builder()
                                    {
                                        Latitude = latitude,
                                        Longitude = longitude
                                    };
                                    break;
                                case "type":
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK) _CurrentContext = PARSE_CONTEXT.TRACK_TYPE;
                                    break;
                                case "ele":
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT)
                                    {
                                        _CurrentContext = PARSE_CONTEXT.TRACK_POINT_ELEVATION;
                                    }
                                    break;
                                case "extensions":
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT) _CurrentContext = PARSE_CONTEXT.TRACK_POINT_EXTENSIONS;
                                    break;
                                case "power":
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_EXTENSIONS) _CurrentContext = PARSE_CONTEXT.TRACK_POINT_POWER;
                                    break;
                                default:
                                    if (reader.Name == _XmlNamespaces["gpxtpx"] + "TrackPointExtension")
                                    {
                                        if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_EXTENSIONS) _CurrentContext = PARSE_CONTEXT.TRACK_POINT_GPXTPX_EXTENSION;
                                    }
                                    else if (reader.Name == _XmlNamespaces["gpxtpx"] + "atemp")
                                    {
                                        if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_GPXTPX_EXTENSION) _CurrentContext = PARSE_CONTEXT.TRACK_POINT_TEMPERATURE;
                                    }
                                    else if (reader.Name == _XmlNamespaces["gpxtpx"] + "hr")
                                    {
                                        if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_GPXTPX_EXTENSION) _CurrentContext = PARSE_CONTEXT.TRACK_POINT_HR;
                                    }
                                    else if (reader.Name == _XmlNamespaces["gpxtpx"] + "cad")
                                    {
                                        if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_GPXTPX_EXTENSION) _CurrentContext = PARSE_CONTEXT.TRACK_POINT_CADENCE;
                                    }
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            switch (_CurrentContext)
                            {
                                case PARSE_CONTEXT.ACTIVITY_TIME:
                                    DateTimeOffset activityStartTime;
                                    string activityStartTimeAsString = reader.Value;
                                    if (DateTimeOffset.TryParse(activityStartTimeAsString, out activityStartTime))
                                    {
                                        _Logger.Debug(string.Format("Activity start time {0}", activityStartTime));
                                    }
                                    else
                                    {
                                        _Logger.Error(string.Format("Error parsing date {0}", activityStartTimeAsString));
                                    }
                                    if (_CurrentActivityBuilder != null)
                                    {
                                        _CurrentActivityBuilder.Time = activityStartTime;
                                        if (!string.IsNullOrEmpty(activityStartTimeAsString)) _CurrentActivityBuilder.OriginId = $"{_CurrentCreator ?? ""}{activityStartTimeAsString}";
                                    }
                                    break;
                                case PARSE_CONTEXT.TRACK_POINT_TIME:
                                    DateTimeOffset segmentTime;
                                    string segmentTimeAsString = reader.Value;
                                    if (DateTimeOffset.TryParse(segmentTimeAsString, out segmentTime))
                                    {
                                        _Logger.Debug(string.Format("Segment start time {0}", segmentTime));
                                    }
                                    else
                                    {
                                        _Logger.Error(string.Format("Error parsing date {0}", segmentTimeAsString));
                                    }
                                    if (_CurrentTrackPointBuilder != null)
                                    {
                                        _CurrentTrackPointBuilder.Time = segmentTime;
                                    }
                                    break;
                                case PARSE_CONTEXT.TRACK_NAME:
                                    if (_CurrentActivityBuilder != null) _CurrentActivityBuilder.Name = reader.Value;
                                    if (_CurrentTrackBuilder != null) _CurrentTrackBuilder.Name = reader.Value;
                                    break;
                                case PARSE_CONTEXT.TRACK_TYPE:
                                    if (_CurrentActivityBuilder != null)
                                    {
                                        ACTIVITY_SPORT sport;
                                        if (!categoryMapping.TryGetValue(reader.Value, out sport)) sport = ACTIVITY_SPORT.OTHER;
                                        if (_CurrentActivityBuilder != null) _CurrentActivityBuilder.Sport = sport;
                                        _Logger.Debug(string.Format("Activity sport {0}", sport));
                                    }
                                    break;
                                case PARSE_CONTEXT.TRACK_POINT_ELEVATION:
                                    if (_CurrentTrackPointBuilder != null)
                                    {
                                        float elevation;
                                        var style = System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign;
                                        var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
                                        if (!float.TryParse(reader.Value, style, culture, out elevation))
                                        {
                                            var countPoints = _CurrentTrackSegmentBuilder.TrackPoints.Count;
                                            elevation = countPoints > 0 ? _CurrentTrackSegmentBuilder.TrackPoints[countPoints - 1].Elevation : 0;
                                        }
                                        _CurrentTrackPointBuilder.Elevation = elevation;
                                    }
                                    break;
                                case PARSE_CONTEXT.TRACK_POINT_POWER:
                                    if (_CurrentTrackPointBuilder != null)
                                    {
                                        int power;
                                        if (!int.TryParse(reader.Value, out power))
                                        {
                                            var countPoints = _CurrentTrackSegmentBuilder.TrackPoints.Count;
                                            power = countPoints > 0 ? _CurrentTrackSegmentBuilder.TrackPoints[countPoints - 1].Power : 0;
                                        }
                                        _CurrentTrackPointBuilder.Power = power;
                                    }
                                    break;
                                case PARSE_CONTEXT.TRACK_POINT_TEMPERATURE:
                                    if (_CurrentTrackPointBuilder != null)
                                    {
                                        float temperature;
                                        var style = System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign;
                                        var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
                                        if (!float.TryParse(reader.Value, style, culture, out temperature))
                                        {
                                            var countPoints = _CurrentTrackSegmentBuilder.TrackPoints.Count;
                                            temperature = countPoints > 0 ? _CurrentTrackSegmentBuilder.TrackPoints[countPoints - 1].Temperature : 0;
                                        }
                                        _CurrentTrackPointBuilder.Temperature = temperature;
                                    }
                                    break;
                                case PARSE_CONTEXT.TRACK_POINT_HR:
                                    if (_CurrentTrackPointBuilder != null)
                                    {
                                        int hr;
                                        if (!int.TryParse(reader.Value, out hr))
                                        {
                                            var countPoints = _CurrentTrackSegmentBuilder.TrackPoints.Count;
                                            hr = countPoints > 0 ? _CurrentTrackSegmentBuilder.TrackPoints[countPoints - 1].HeartRate : 0;
                                        }
                                        _CurrentTrackPointBuilder.HeartRate = hr;
                                    }
                                    break;
                                case PARSE_CONTEXT.TRACK_POINT_CADENCE:
                                    if (_CurrentTrackPointBuilder != null)
                                    {
                                        int cadence;
                                        if (!int.TryParse(reader.Value, out cadence))
                                        {
                                            var countPoints = _CurrentTrackSegmentBuilder.TrackPoints.Count;
                                            cadence = countPoints > 0 ? _CurrentTrackSegmentBuilder.TrackPoints[countPoints - 1].Cadence : 0;
                                        }
                                        _CurrentTrackPointBuilder.Cadence = cadence;
                                    }
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case "gpx":
                                    var allPoints = _CurrentActivityBuilder.Tracks.SelectMany(track => track.TrackSegments.SelectMany(segment => segment.TrackPoints));
                                    _CurrentActivityBuilder.HeartRate = (int)Math.Round(allPoints.Where(point => point.HeartRate > 0).DefaultIfEmpty().Average(point => point?.HeartRate) ?? 0);
                                    _CurrentActivityBuilder.Cadence = (int)Math.Round(allPoints.Where(point => point.Cadence > 0).DefaultIfEmpty().Average(point => point?.Cadence) ?? 0);
                                    _CurrentActivityBuilder.Power = (int)Math.Round(allPoints.Where(point => point.Power > 0).DefaultIfEmpty().Average(point => point?.Power) ?? 0);
                                    _CurrentActivityBuilder.Temperature = (int)Math.Round(allPoints.Where(point => point.Temperature > 0).DefaultIfEmpty().Average(point => point?.Temperature) ?? 0);
                                    _CurrentActivityBuilder.TracksPointsCount = allPoints.Count();
                                    var activity = _CurrentActivityBuilder.Build();
                                    _CurrentActivityBuilder = null;
                                    _CurrentContext = PARSE_CONTEXT.ROOT;
                                    yield return activity;
                                    break;
                                case "time":
                                    if (_CurrentContext == PARSE_CONTEXT.ACTIVITY_TIME) _CurrentContext = PARSE_CONTEXT.METADATA;
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_TIME) _CurrentContext = PARSE_CONTEXT.TRACK_POINT;
                                    break;
                                case "metadata":
                                    if (_CurrentContext == PARSE_CONTEXT.METADATA) _CurrentContext = PARSE_CONTEXT.GPX;
                                    break;
                                case "trk":
                                    _CurrentContext = PARSE_CONTEXT.GPX;
                                    if (_CurrentActivityBuilder != null && _CurrentTrackBuilder != null)
                                    {
                                        _CurrentActivityBuilder.Tracks.Add(_CurrentTrackBuilder.Build());
                                    }
                                    _CurrentTrackBuilder = null;
                                    break;
                                case "name":
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK_NAME)
                                    {
                                        _CurrentContext = PARSE_CONTEXT.TRACK;
                                    }
                                    break;
                                case "trkseg":
                                    _CurrentContext = PARSE_CONTEXT.TRACK;
                                    if (_CurrentTrackBuilder != null && _CurrentTrackSegmentBuilder != null)
                                    {
                                        _CurrentTrackBuilder.TrackSegments.Add(_CurrentTrackSegmentBuilder.Build());
                                    }
                                    _CurrentTrackSegmentBuilder = null;
                                    break;
                                case "trkpt":
                                    _CurrentContext = PARSE_CONTEXT.TRACK_SEGMENT;
                                    if (_CurrentTrackSegmentBuilder != null && _CurrentTrackPointBuilder != null)
                                    {
                                        _CurrentTrackSegmentBuilder.TrackPoints.Add(_CurrentTrackPointBuilder.Build());
                                    }
                                    _CurrentTrackPointBuilder = null;
                                    break;
                                case "type":
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK_TYPE) _CurrentContext = PARSE_CONTEXT.TRACK;
                                    break;
                                case "ele":
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_ELEVATION) _CurrentContext = PARSE_CONTEXT.TRACK_POINT;
                                    break;
                                case "extensions":
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_EXTENSIONS) _CurrentContext = PARSE_CONTEXT.TRACK_POINT;
                                    break;
                                case "power":
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_POWER) _CurrentContext = PARSE_CONTEXT.TRACK_POINT_EXTENSIONS;
                                    break;
                                default:
                                    if (reader.Name == _XmlNamespaces["gpxtpx"] + "TrackPointExtension")
                                    {
                                        if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_GPXTPX_EXTENSION) _CurrentContext = PARSE_CONTEXT.TRACK_POINT_EXTENSIONS;
                                    }
                                    else if (reader.Name == _XmlNamespaces["gpxtpx"] + "atemp")
                                    {
                                        if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_TEMPERATURE) _CurrentContext = PARSE_CONTEXT.TRACK_POINT_GPXTPX_EXTENSION;
                                    }
                                    else if (reader.Name == _XmlNamespaces["gpxtpx"] + "hr")
                                    {
                                        if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_HR) _CurrentContext = PARSE_CONTEXT.TRACK_POINT_GPXTPX_EXTENSION;
                                    }
                                    else if (reader.Name == _XmlNamespaces["gpxtpx"] + "cad")
                                    {
                                        if (_CurrentContext == PARSE_CONTEXT.TRACK_POINT_CADENCE) _CurrentContext = PARSE_CONTEXT.TRACK_POINT_GPXTPX_EXTENSION;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public IDictionary<string, string> GetSports(Stream stream)
        {
            IDictionary<string, string> sports = new Dictionary<string, string>();

            XmlReaderSettings settings = new XmlReaderSettings
            {
                Async = false
            };
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                // SAX parsing for performance
                _CurrentContext = PARSE_CONTEXT.ROOT;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "trk":
                                    _CurrentContext = PARSE_CONTEXT.TRACK;
                                    break;
                                case "type":
                                    _CurrentContext = PARSE_CONTEXT.TRACK_TYPE;
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            switch (_CurrentContext)
                            {
                                case PARSE_CONTEXT.TRACK_TYPE:
                                    string type = reader.Value;
                                    if (!sports.ContainsKey(type))
                                    {
                                        sports.Add(type, type);
                                        _Logger.Debug($"Activity sport {type}");
                                    }
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case "trk":
                                    _CurrentContext = PARSE_CONTEXT.ROOT;
                                    break;
                                case "type":
                                    if (_CurrentContext == PARSE_CONTEXT.TRACK_TYPE) _CurrentContext = PARSE_CONTEXT.TRACK;
                                    break;
                            }
                            break;
                    }
                }
            }
            return sports;
        }
    }
}

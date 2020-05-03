/* Copyright 2020 Nicolas Mayeur

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
using System.Xml;

namespace OSL.Common.Service.Importer
{
    public class FitLogImporter : IActivitiesImporter
    {
        private readonly ILoggerService _LoggerService;


        public FitLogImporter(ILoggerService loggerService)
        {
            _LoggerService = loggerService ?? throw new ArgumentNullException("logger service must not be null");
        }

        private enum PARSE_CONTEXT
        {
            ROOT,
            FITNESS_WORKBOOK, ATHLETE_LOG, ATHLETE,
            ACTIVITY, ACTIVITY_METADATA, ACTIVITY_CALORIES, ACTIVITY_NAME, ACTIVITY_CATEGORY,
            ACTIVITY_LOCATION, ACTIVITY_EQUIPMENT_USED, ACTIVITY_EQUIPMENT_ITEM, ACTIVITY_TRACK, ACTIVITY_TRACK_PT
        }
        private PARSE_CONTEXT _CurrentContext = PARSE_CONTEXT.ROOT;
        private ActivityEntity.Builder _CurrentActivityBuilder = null;
        private TrackEntity.Builder _CurrentTrackBuilder = null;
        //private AthleteEntity _CurrentAthlete = null;
        private DateTimeOffset _CurrentTrackStartTime;

        public IEnumerable<ActivityEntity> ImportActivitiesStream(Stream stream, IDictionary<string, ACTIVITY_SPORT> categoryMapping)
        {
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
                                case "FitnessWorkbook":
                                    _CurrentContext = PARSE_CONTEXT.FITNESS_WORKBOOK;
                                    break;
                                case "AthleteLog":
                                    _CurrentContext = PARSE_CONTEXT.ATHLETE_LOG;
                                    break;
                                case "Athlete":
                                    _CurrentContext = PARSE_CONTEXT.ATHLETE;
                                    //_CurrentAthlete = new AthleteEntity(new List<ActivityVO>(), reader.GetAttribute("Name"), reader.GetAttribute("Id"));
                                    break;
                                case "Activity":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY;
                                    _CurrentActivityBuilder = new ActivityEntity.Builder
                                    {
                                        OriginId = reader.GetAttribute("Id"),
                                        OriginSystem = "FITLOG"
                                    };
                                    break;
                                case "Metadata":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY_METADATA;
                                    break;
                                case "Calories":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY_CALORIES;
                                    int calories;
                                    if (int.TryParse(reader.GetAttribute("TotalCal"), out calories))
                                    {
                                        if (_CurrentActivityBuilder != null) _CurrentActivityBuilder.Calories = calories;
                                        _LoggerService.Debug(string.Format("Activity calories {0}", calories));
                                    }
                                    break;
                                case "Name":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY_NAME;
                                    break;
                                case "Category":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY_CATEGORY;
                                    string category = reader.GetAttribute("Id");
                                    ACTIVITY_SPORT sport;
                                    if (!categoryMapping.TryGetValue(category, out sport)) sport = ACTIVITY_SPORT.OTHER;
                                    if (_CurrentActivityBuilder != null) _CurrentActivityBuilder.Sport = sport;
                                    _LoggerService.Debug(string.Format("Activity sport {0}", sport));
                                    break;
                                case "Location":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY_LOCATION;
                                    string location = reader.GetAttribute("Name");
                                    if (_CurrentActivityBuilder != null) _CurrentActivityBuilder.Location = location;
                                    _LoggerService.Debug(string.Format("Activity location {0}", location));
                                    break;
                                case "EquipmentUsed":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY_EQUIPMENT_USED;
                                    break;
                                case "EquipmentItem":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY_EQUIPMENT_ITEM;
                                    break;
                                case "Track":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY_TRACK;
                                    _CurrentTrackBuilder = new TrackEntity.Builder();
                                    string startTimeAsString = reader.GetAttribute("StartTime");
                                    DateTimeOffset startTime;
                                    if (DateTimeOffset.TryParse(startTimeAsString, out startTime))
                                    {
                                        _CurrentTrackStartTime = startTime;
                                        _LoggerService.Debug(string.Format("Activity start time {0}", startTime));
                                    }
                                    else
                                    {
                                        _LoggerService.Error(string.Format("Error parsing date {0}", startTimeAsString));
                                    }
                                    break;
                                case "pt":
                                    if (_CurrentTrackBuilder == null) break;
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY_TRACK_PT;
                                    int tm;
                                    var tmAsString = reader.GetAttribute("tm");
                                    DateTimeOffset tmTime;
                                    if (int.TryParse(tmAsString, out tm))
                                    {
                                        tmTime = _CurrentTrackStartTime.AddSeconds(tm);
                                    }
                                    else
                                    {
                                        _LoggerService.Error(string.Format("Error parsing tm (number of seconds since start {0}", tmAsString));
                                        break;
                                    }
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
                                    float elevation;
                                    if (!float.TryParse(reader.GetAttribute("ele"), style, culture, out elevation))
                                    {
                                        break;
                                    }
                                    int hr;
                                    if (!int.TryParse(reader.GetAttribute("hr"), out hr))
                                    {
                                        break;
                                    }
                                    int cadence;
                                    if (!int.TryParse(reader.GetAttribute("cadence"), out cadence))
                                    {
                                        break;
                                    }
                                    TrackPointVO trackPoint = new TrackPointVO.Builder
                                    {
                                        Time = tmTime,
                                        Latitude = latitude,
                                        Longitude = longitude,
                                        Elevation = elevation,
                                        HeartRate = hr,
                                        Cadence = cadence
                                    };
                                    _CurrentTrackBuilder.TrackPoints.Add(trackPoint);
                                    _LoggerService.Debug(string.Format("Trackpoint time {0}, latitude {1}, longitude {2}, elevation {3}, heart-rate {4}, cadence {5}",
                                        tmTime, latitude, longitude, elevation, hr, cadence));
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            switch (_CurrentContext)
                            {
                                case PARSE_CONTEXT.ACTIVITY_NAME:
                                    var name = reader.Value;
                                    if (_CurrentActivityBuilder != null) _CurrentActivityBuilder.Name = name;
                                    _LoggerService.Debug(string.Format("Activity name {0}", name));
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case "FitnessWorkbook":
                                    _CurrentContext = PARSE_CONTEXT.ROOT;
                                    break;
                                case "AthleteLog":
                                    _CurrentContext = PARSE_CONTEXT.FITNESS_WORKBOOK;
                                    break;
                                case "Athlete":
                                    _CurrentContext = PARSE_CONTEXT.FITNESS_WORKBOOK;
                                    break;
                                case "Activity":
                                    _CurrentContext = PARSE_CONTEXT.FITNESS_WORKBOOK;
                                    var activity = _CurrentActivityBuilder.Build();
                                    yield return activity;
                                    _CurrentActivityBuilder = null;
                                    break;
                                case "Metadata":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY;
                                    break;
                                case "Calories":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY;
                                    break;
                                case "Name":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY;
                                    break;
                                case "Category":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY;
                                    break;
                                case "Location":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY;
                                    break;
                                case "EquipmentUsed":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY;
                                    break;
                                case "EquipmentItem":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY_EQUIPMENT_USED;
                                    break;
                                case "Track":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY;
                                    var track = _CurrentTrackBuilder.Build();
                                    if (_CurrentActivityBuilder != null) _CurrentActivityBuilder.Track = track;
                                    _CurrentTrackBuilder = null;
                                    break;
                                case "pt":
                                    _CurrentContext = PARSE_CONTEXT.ACTIVITY_TRACK;
                                    break;
                            }
                            //_LoggerService.Debug(string.Format("End Element {0}", reader.Name));
                            break;
                        default:
                            //_LoggerService.Debug(string.Format("Other node {0} with value {1}",
                            //    reader.NodeType, reader.Value));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Get all sports present in the stream. This should be used to preprare a mapping between ActivityEntity.ACTIVITY_SPORT and sports defined in the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
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
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "Category":
                                    string categoryId = reader.GetAttribute("Id");
                                    string categoryName = reader.GetAttribute("Name");
                                    if (!sports.ContainsKey(categoryId)) sports.Add(categoryId, categoryName);
                                    _LoggerService.Debug($"Activity sport {categoryName} with Id {categoryId}");
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

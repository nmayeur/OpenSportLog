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
using System.Globalization;
using System.Linq;
using System.Text;

namespace OSL.Common.Service
{
    public class GeoJsonConverter : IGeoJsonConverter
    {
        public string GetGeoJsonFromTrackPoints(IEnumerable<TrackPointVO> TrackPoints)
        {
            var geoJson = @"{
            ""type"": ""FeatureCollection"",
            ""features"": [
                ";
            var enCulture = CultureInfo.GetCultureInfo("en-US");
            var isFirst = true;
            geoJson += @"{""type"": ""Feature"",""geometry"": {""type"": ""LineString"",""coordinates"": [";
            foreach (var tp in TrackPoints)
            {
                if (!isFirst)
                {
                    geoJson += ",";
                }
                else
                {
                    isFirst = false;
                }
                FormattableString coordinates2 = $"[{tp.Longitude:N6},{tp.Latitude:N6}]";
                geoJson += coordinates2.ToString(enCulture);
            }
            geoJson += $@"]}},""properties"":{{""zoomed"":false}},""id"": 1}}";
            geoJson += @"]}";
            return geoJson;
        }

        public string GetGeoJsonZoomFromTrackPoints(IEnumerable<TrackPointVO> TrackPoints, long StartEpochZoom, long EndEpochZoom)
        {
            var geoJson = @"{
            ""type"": ""FeatureCollection"",
            ""features"": [
                ";
            var enCulture = CultureInfo.GetCultureInfo("en-US");
            var isFirst = true;
            geoJson += @"{""type"": ""Feature"",""geometry"": {""type"": ""LineString"",""coordinates"": [";
            foreach (var tp in TrackPoints.SkipWhile(tp => tp.Time < DateTimeOffset.FromUnixTimeMilliseconds(StartEpochZoom)).TakeWhile(tp => tp.Time <= DateTimeOffset.FromUnixTimeMilliseconds(EndEpochZoom)))
            {
                if (!isFirst)
                {
                    geoJson += ",";
                } else
                {
                    isFirst = false;
                }
                FormattableString coordinates2 = $"[{tp.Longitude:N6},{tp.Latitude:N6}]";
                geoJson += coordinates2.ToString(enCulture);
            }
            geoJson += $@"]}},""properties"":{{""zoomed"":true}},""id"": 1}}";
            geoJson += @"]}";
            return geoJson;
        }
    }
}

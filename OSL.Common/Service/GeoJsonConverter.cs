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
            foreach (var tp in TrackPoints.SkipWhile(tp => tp.Time < DateTimeOffset.FromUnixTimeSeconds(StartEpochZoom)).TakeWhile(tp => tp.Time <= DateTimeOffset.FromUnixTimeSeconds(EndEpochZoom)))
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
            geoJson += $@"]}},""properties"":{{""zoomed"":false}},""id"": 1}}";
            geoJson += @"]}";
            return geoJson;
        }
    }
}

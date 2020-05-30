using OSL.Common.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                {
                ""type"": ""Feature"",
                    ""geometry"": {
                    ""type"": ""LineString"",
                        ""coordinates"": [";
            var enCulture = CultureInfo.GetCultureInfo("en-US");
            foreach (var tp in TrackPoints)
            {
                FormattableString coordinates = $"[{tp.Longitude:N6},{tp.Latitude:N6}],";
                geoJson += coordinates.ToString(enCulture);
            }
            geoJson += @"]
                    },
                    ""properties"": {
                        ""popupContent"": ""test popup"",
                        ""zoomed"": false
                    },
                    ""id"": 1
                }]}";
            return geoJson;
        }
    }
}

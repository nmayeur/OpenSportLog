using GeoSports.Common.model;
using System.Collections.Generic;
using System.IO;

namespace GeoSports.Common.service.importer
{
    public interface IActivitiesImporter
    {
        IEnumerable<ActivityVO> ImportActivitiesStream(Stream stream);
    }
}

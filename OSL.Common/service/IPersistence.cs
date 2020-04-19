using GeoSports.Common.model;

namespace GeoSports.Common.service
{
    public interface IPersistence
    {
        AthleteEntity GetAthlete(string Name);
        void SaveAthlete(AthleteEntity athlete);
    }
}

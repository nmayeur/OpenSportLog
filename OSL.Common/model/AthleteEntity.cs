using System;
using System.Collections.Generic;

namespace GeoSports.Common.model
{
    public class AthleteEntity
    {
        public string Id { get; private set; }
        public List<ActivityVO> Activities { get; private set; }
        public string Name { get; private set; }

        /// <summary>
        /// Entity Framework (proxy) constructor
        /// </summary>
        public AthleteEntity() { }

        public AthleteEntity(List<ActivityVO> activities, string name, string id)
        {
            Activities = activities;
            Name = name;
            Id = id;
        }
    }
}

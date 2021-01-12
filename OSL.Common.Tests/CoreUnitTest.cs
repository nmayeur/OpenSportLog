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
using FakeItEasy;
using FluentAssertions;
using NLog;
using NLog.Config;
using OSL.Common.Model;
using OSL.Common.Service;
using OSL.Common.Service.Importer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace OSL.Common.Tests
{
    public class CoreUnitTest
    {
        private readonly ILogger _Logger;

        public CoreUnitTest(ITestOutputHelper output)
        {
            LogFactory logFactory = new LogFactory();
            logFactory.Configuration = new XmlLoggingConfiguration(@"NLog.config");
            logFactory.ThrowConfigExceptions = false;
            _Logger = logFactory.GetCurrentClassLogger();
        }

        [Fact]
        public void TestImportFitlogSports()
        {
            string path = @"data\data_mini.fitlog";
            FitLogImporter importer = new FitLogImporter();

            IDictionary<string, string> sports;
            using (FileStream fs = File.OpenRead(path))
            {
                sports = importer.GetSports(fs);
            }
            sports.Should().HaveCountGreaterOrEqualTo(1, "expected data_mini.fitlog to contain at least 1 sport");
        }

        [Fact]
        public void TestImportGpxSports()
        {
            string path = @"data\data_tiny.gpx";
            IActivitiesImporter importer = new GpxImporter();

            IDictionary<string, string> sports;
            using (FileStream fs = File.OpenRead(path))
            {
                sports = importer.GetSports(fs);
            }
            sports.Should().HaveCountGreaterOrEqualTo(1, "expected data_tiny.gpx to contain at least 1 sport");
        }

        [Fact]
        public void TestImportGpxData()
        {
            string path = @"data\data_tiny.gpx";
            GpxImporter importer = new GpxImporter();

            IList<ActivityEntity> activities;
            using (FileStream fs = File.OpenRead(path))
            {
                activities = new List<ActivityEntity>(importer.ImportActivitiesStream(fs, new Dictionary<string, ACTIVITY_SPORT> {
                { "1", ACTIVITY_SPORT.BIKING},
                { "2",ACTIVITY_SPORT.RUNNING},
                { "3",ACTIVITY_SPORT.SWIMMING} }));
            }
            activities.Should().HaveCountGreaterOrEqualTo(1, "expected data_mini.fitlog to contain at least 1 activity");
            var activity = activities.Where(a => a.OriginId == "StravaGPX2020-05-17T07:24:45Z").Single();
            activity.Tracks.Should().ContainSingle();
        }

        [Fact]
        public void TestImportFitlogData()
        {
            string path = @"data\data_mini.fitlog";
            FitLogImporter importer = new FitLogImporter();

            IList<ActivityEntity> activities;
            using (FileStream fs = File.OpenRead(path))
            {
                activities = new List<ActivityEntity>(importer.ImportActivitiesStream(fs, new Dictionary<string, ACTIVITY_SPORT> {
                { "e41b80e4-fa5f-48e3-95be-d0e66b72ab7c", ACTIVITY_SPORT.BIKING},
                { "eca38408-cb82-42ed-b242-166b43b785a6",ACTIVITY_SPORT.RUNNING},
                { "6f2fdaf9-4c5a-4c2c-a4fa-5be42e9733dd",ACTIVITY_SPORT.SWIMMING} }));
            }
            activities.Should().HaveCountGreaterOrEqualTo(1, "expected data_mini.fitlog to contain at least 1 activity");
            activities.Should().ContainSingle(a => a.OriginId == "81f11f5f-fa73-42d4-a7f9-e71a74313ba6");
            var activity = activities.Where(a => a.OriginId == "81f11f5f-fa73-42d4-a7f9-e71a74313ba6").Single();
            activity.Tracks.Should().ContainSingle();
            activity.Tracks.Single().TrackSegments.Should().ContainSingle();
            var tp = activity.Tracks.Single().TrackSegments.Single().TrackPoints;
            tp.Should().HaveCountGreaterOrEqualTo(10);
            activity.Should().Match<ActivityEntity>(a => (a.TimeSpan.Ticks > 0));
        }

        [Fact]
        public void TestImportFitlogStream()
        {
            string fitlog = @"<?xml version=""1.0"" encoding=""utf-8""?>
<FitnessWorkbook xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://www.zonefivesoftware.com/xmlschemas/FitnessLogbook/v3"">
 <AthleteLog>
  <Athlete Id=""989703a1-f7b8-4af3-b4ae-9dd862fed016"" Name=""Nicolas Mayeur"" />
  <Activity StartTime=""2019-12-25T09:45:07Z"" Id=""9fe01c40-fe66-48e2-b5fa-97370c94e9e8"">
   <Metadata Source="""" Created=""2020-01-19T10:16:01Z"" Modified=""2020-01-19T10:16:17Z"" />
   <Calories TotalCal=""2153"" />
   <Name>Lonchamp de Noël</Name>
   <Category Id=""e41b80e4-fa5f-48e3-95be-d0e66b72ab7c"" Name=""Vélo"" />
   <Location Name=""Longchamps"" />
   <EquipmentUsed>
    <EquipmentItem Id=""67713a66-5c4d-43b6-9386-df3e431d06fa"" Name=""Giant - TCR advanced 2007"" />
   </EquipmentUsed>
   <Track StartTime=""2019-12-25T09:45:07Z"">
    <pt tm=""0"" lat=""48.8676490783691"" lon=""2.20366406440735"" ele=""91.4405212402344"" hr=""71"" cadence=""0"" />
    <pt tm=""1"" lat=""48.8676490783691"" lon=""2.2036669254303"" ele=""91.4514389038086"" hr=""71"" cadence=""0"" />
    <pt tm=""8"" lat=""48.8676490783691"" lon=""2.2036759853363"" ele=""91.4860000610352"" hr=""74"" cadence=""0"" />
    <pt tm=""10"" lat=""48.8676147460938"" lon=""2.20368194580078"" ele=""91.4503021240234"" hr=""79"" cadence=""27"" />
    <pt tm=""11"" lat=""48.8675918579102"" lon=""2.20368409156799"" ele=""91.4193572998047"" hr=""81"" cadence=""47"" />
    <pt tm=""13"" lat=""48.8675346374512"" lon=""2.20368790626526"" ele=""91.3357162475586"" hr=""84"" cadence=""58"" />
    <pt tm=""16"" lat=""48.8674278259277"" lon=""2.20366907119751"" ele=""91.1220474243164"" hr=""85"" cadence=""55"" />
    <pt tm=""19"" lat=""48.867317199707"" lon=""2.20367789268494"" ele=""91.0210494995117"" hr=""85"" cadence=""60"" />
    <pt tm=""21"" lat=""48.8672409057617"" lon=""2.2037239074707"" ele=""91.0951538085938"" hr=""86"" cadence=""54"" />
    <pt tm=""23"" lat=""48.8671646118164"" lon=""2.20372295379639"" ele=""91.0001678466797"" hr=""89"" cadence=""22"" />
    <pt tm=""26"" lat=""48.8670387268066"" lon=""2.20373702049255"" ele=""90.8997421264648"" hr=""91"" cadence=""67"" />
    <pt tm=""30"" lat=""48.8668174743652"" lon=""2.20377111434937"" ele=""90.7569808959961"" hr=""93"" cadence=""77"" />
    <pt tm=""31"" lat=""48.866756439209"" lon=""2.20379495620728"" ele=""90.7695693969726"" hr=""93"" cadence=""77"" />
    <pt tm=""32"" lat=""48.8667068481445"" lon=""2.20384001731873"" ele=""90.8722763061524"" hr=""93"" cadence=""18"" />
    <pt tm=""34"" lat=""48.8666305541992"" lon=""2.20396900177002"" ele=""91.2986831665039"" hr=""95"" cadence=""79"" />
    <pt tm=""37"" lat=""48.8665428161621"" lon=""2.20420503616333"" ele=""92.0136871337891"" hr=""99"" cadence=""78"" />
   </Track>
  </Activity>
 </AthleteLog>
</FitnessWorkbook>";
            byte[] byteArray = Encoding.UTF8.GetBytes(fitlog);

            FitLogImporter importer = new FitLogImporter();

            List<ActivityEntity> activities;
            using (Stream fs = new MemoryStream(byteArray))
            {
                activities = new List<ActivityEntity>(importer.ImportActivitiesStream(fs, new Dictionary<string, ACTIVITY_SPORT> {
                { "e41b80e4-fa5f-48e3-95be-d0e66b72ab7c", ACTIVITY_SPORT.BIKING},
                { "eca38408-cb82-42ed-b242-166b43b785a6",ACTIVITY_SPORT.RUNNING},
                { "6f2fdaf9-4c5a-4c2c-a4fa-5be42e9733dd",ACTIVITY_SPORT.SWIMMING} }));
            }

            activities.Should().HaveCountLessOrEqualTo(1, "expected data_tiny.fitlog to contain at least 1 activity");
            var activity = activities[0];
            activity.Name.Should().Be("Lonchamp de Noël");
            activity.Location.Should().Be("Longchamps");
            activity.Sport.Should().Be(ACTIVITY_SPORT.BIKING);
            activity.Calories.Should().Be(2153);
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].Time.Should().Be(DateTimeOffset.Parse("2019-12-25T09:45:07Z"));
            var style = System.Globalization.NumberStyles.AllowDecimalPoint;
            var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].Latitude.Should().Be(float.Parse("48.8676490783691", style, culture));
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].Longitude.Should().Be(float.Parse("2.20366406440735", style, culture));
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].Elevation.Should().Be(float.Parse("91.4405212402344", style, culture));
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].HeartRate.Should().Be(71);
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].Cadence.Should().Be(0);
            activity.Tracks[0].TrackSegments[0].TrackPoints[3].Cadence.Should().Be(27);
        }

        [Fact]
        public void TestImportFitlogAverages()
        {
            string fitlog = @"<?xml version=""1.0"" encoding=""utf-8""?>
<FitnessWorkbook xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://www.zonefivesoftware.com/xmlschemas/FitnessLogbook/v3"">
 <AthleteLog>
  <Athlete Id=""989703a1-f7b8-4af3-b4ae-9dd862fed016"" Name=""Nicolas Mayeur"" />
  <Activity StartTime=""2019-12-25T09:45:07Z"" Id=""9fe01c40-fe66-48e2-b5fa-97370c94e9e8"">
   <Metadata Source="""" Created=""2020-01-19T10:16:01Z"" Modified=""2020-01-19T10:16:17Z"" />
   <Calories TotalCal=""2153"" />
   <Name>Lonchamp de Noël</Name>
   <Category Id=""e41b80e4-fa5f-48e3-95be-d0e66b72ab7c"" Name=""Vélo"" />
   <Location Name=""Longchamps"" />
   <EquipmentUsed>
    <EquipmentItem Id=""67713a66-5c4d-43b6-9386-df3e431d06fa"" Name=""Giant - TCR advanced 2007"" />
   </EquipmentUsed>
   <Track StartTime=""2019-12-25T09:45:07Z"">
    <pt tm=""0"" lat=""48.8676490783691"" lon=""2.20366406440735"" ele=""91.4405212402344"" hr=""140"" cadence=""0"" />
    <pt tm=""1"" lat=""48.8676490783691"" lon=""2.2036669254303"" ele=""91.4514389038086"" hr=""140"" cadence=""100"" />
    <pt tm=""8"" lat=""48.8676490783691"" lon=""2.2036759853363"" ele=""91.4860000610352"" hr=""150"" cadence=""100"" />
    <pt tm=""10"" lat=""48.8676147460938"" lon=""2.20368194580078"" ele=""91.4503021240234"" hr=""150"" cadence=""160"" />
   </Track>
  </Activity>
 </AthleteLog>
</FitnessWorkbook>";
            byte[] byteArray = Encoding.UTF8.GetBytes(fitlog);

            FitLogImporter importer = new FitLogImporter();

            List<ActivityEntity> activities;
            using (Stream fs = new MemoryStream(byteArray))
            {
                activities = new List<ActivityEntity>(importer.ImportActivitiesStream(fs, new Dictionary<string, ACTIVITY_SPORT> {
                { "e41b80e4-fa5f-48e3-95be-d0e66b72ab7c", ACTIVITY_SPORT.BIKING},
                { "eca38408-cb82-42ed-b242-166b43b785a6",ACTIVITY_SPORT.RUNNING},
                { "6f2fdaf9-4c5a-4c2c-a4fa-5be42e9733dd",ACTIVITY_SPORT.SWIMMING} }));
            }

            activities.Should().HaveCountLessOrEqualTo(1, "expected data_tiny.fitlog to contain at least 1 activity");
            var activity = activities[0];
            activity.Cadence.Should().Be(120);
            activity.HeartRate.Should().Be(145);
        }

        [Fact]
        public void TestImportMultiTracksFitlogStream()
        {
            string fitlog = @"<?xml version=""1.0"" encoding=""utf-8""?>
<FitnessWorkbook xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://www.zonefivesoftware.com/xmlschemas/FitnessLogbook/v3"">
 <AthleteLog>
  <Athlete Id=""989703a1-f7b8-4af3-b4ae-9dd862fed016"" Name=""Nicolas Mayeur"" />
  <Activity StartTime=""2019-12-25T09:45:07Z"" Id=""9fe01c40-fe66-48e2-b5fa-97370c94e9e8"">
   <Metadata Source="""" Created=""2020-01-19T10:16:01Z"" Modified=""2020-01-19T10:16:17Z"" />
   <Calories TotalCal=""2153"" />
   <Name>Lonchamp de Noël</Name>
   <Category Id=""e41b80e4-fa5f-48e3-95be-d0e66b72ab7c"" Name=""Vélo"" />
   <Location Name=""Longchamps"" />
   <EquipmentUsed>
    <EquipmentItem Id=""67713a66-5c4d-43b6-9386-df3e431d06fa"" Name=""Giant - TCR advanced 2007"" />
   </EquipmentUsed>
   <Track StartTime=""2019-12-25T09:45:07Z"">
    <pt tm=""0"" lat=""48.8676490783691"" lon=""2.20366406440735"" ele=""91.4405212402344"" hr=""71"" cadence=""0"" />
    <pt tm=""1"" lat=""48.8676490783691"" lon=""2.2036669254303"" ele=""91.4514389038086"" hr=""71"" cadence=""0"" />
    <pt tm=""8"" lat=""48.8676490783691"" lon=""2.2036759853363"" ele=""91.4860000610352"" hr=""74"" cadence=""0"" />
    <pt tm=""10"" lat=""48.8676147460938"" lon=""2.20368194580078"" ele=""91.4503021240234"" hr=""79"" cadence=""27"" />
    <pt tm=""11"" lat=""48.8675918579102"" lon=""2.20368409156799"" ele=""91.4193572998047"" hr=""81"" cadence=""47"" />
    <pt tm=""13"" lat=""48.8675346374512"" lon=""2.20368790626526"" ele=""91.3357162475586"" hr=""84"" cadence=""58"" />
    <pt tm=""16"" lat=""48.8674278259277"" lon=""2.20366907119751"" ele=""91.1220474243164"" hr=""85"" cadence=""55"" />
    <pt tm=""19"" lat=""48.867317199707"" lon=""2.20367789268494"" ele=""91.0210494995117"" hr=""85"" cadence=""60"" />
    <pt tm=""21"" lat=""48.8672409057617"" lon=""2.2037239074707"" ele=""91.0951538085938"" hr=""86"" cadence=""54"" />
    <pt tm=""23"" lat=""48.8671646118164"" lon=""2.20372295379639"" ele=""91.0001678466797"" hr=""89"" cadence=""22"" />
    <pt tm=""26"" lat=""48.8670387268066"" lon=""2.20373702049255"" ele=""90.8997421264648"" hr=""91"" cadence=""67"" />
    <pt tm=""30"" lat=""48.8668174743652"" lon=""2.20377111434937"" ele=""90.7569808959961"" hr=""93"" cadence=""77"" />
    <pt tm=""31"" lat=""48.866756439209"" lon=""2.20379495620728"" ele=""90.7695693969726"" hr=""93"" cadence=""77"" />
    <pt tm=""32"" lat=""48.8667068481445"" lon=""2.20384001731873"" ele=""90.8722763061524"" hr=""93"" cadence=""18"" />
    <pt tm=""34"" lat=""48.8666305541992"" lon=""2.20396900177002"" ele=""91.2986831665039"" hr=""95"" cadence=""79"" />
    <pt tm=""37"" lat=""48.8665428161621"" lon=""2.20420503616333"" ele=""92.0136871337891"" hr=""99"" cadence=""78"" />
   </Track>
   <Track StartTime=""2020-04-13T08:02:13Z"">
    <pt tm=""0"" lat=""40.7790069580078"" lon=""-73.9657745361328"" ele=""26.6031761169434"" hr=""101"" cadence=""69"" />
    <pt tm=""1"" lat=""40.7789878845215"" lon=""-73.9657897949219"" ele=""26.5597133636475"" hr=""102"" cadence=""77"" />
    <pt tm=""2"" lat=""40.7789611816406"" lon=""-73.9658050537109"" ele=""26.4984683990478"" hr=""103"" cadence=""78"" />
    <pt tm=""3"" lat=""40.7789344787598"" lon=""-73.9658279418945"" ele=""26.4409465789795"" hr=""104"" cadence=""78"" />
    <pt tm=""4"" lat=""40.7789077758789"" lon=""-73.9658508300781"" ele=""26.4481735229492"" hr=""104"" cadence=""78"" />
    <pt tm=""5"" lat=""40.7788696289062"" lon=""-73.9658813476562"" ele=""26.4804992675781"" hr=""104"" cadence=""80"" />
    <pt tm=""6"" lat=""40.7788391113281"" lon=""-73.9659042358398"" ele=""26.5025596618652"" hr=""103"" cadence=""79"" />
    <pt tm=""7"" lat=""40.77880859375"" lon=""-73.9659423828125"" ele=""26.5894317626953"" hr=""103"" cadence=""80"" />
    <pt tm=""8"" lat=""40.7787704467773"" lon=""-73.9659729003906"" ele=""26.6311454772949"" hr=""102"" cadence=""80"" />
    <pt tm=""9"" lat=""40.7787322998047"" lon=""-73.9660110473633"" ele=""26.7084484100342"" hr=""102"" cadence=""80"" />
    <pt tm=""10"" lat=""40.7786903381348"" lon=""-73.9660491943359"" ele=""26.7819728851318"" hr=""101"" cadence=""75"" />
    <pt tm=""11"" lat=""40.7786521911621"" lon=""-73.9660873413086"" ele=""26.8678684234619"" hr=""101"" cadence=""75"" />
    <pt tm=""12"" lat=""40.7786102294922"" lon=""-73.9661254882812"" ele=""26.950403213501"" hr=""102"" cadence=""76"" />
    <pt tm=""13"" lat=""40.7785682678223"" lon=""-73.9661712646484"" ele=""27.0715885162354"" hr=""102"" cadence=""77"" />
    <pt tm=""14"" lat=""40.7785263061523"" lon=""-73.9662094116211"" ele=""27.1638050079346"" hr=""102"" cadence=""77"" />
   </Track>
  </Activity>
 </AthleteLog>
</FitnessWorkbook>";
            byte[] byteArray = Encoding.UTF8.GetBytes(fitlog);

            FitLogImporter importer = new FitLogImporter();

            List<ActivityEntity> activities;
            using (Stream fs = new MemoryStream(byteArray))
            {
                activities = new List<ActivityEntity>(importer.ImportActivitiesStream(fs, new Dictionary<string, ACTIVITY_SPORT> {
                { "e41b80e4-fa5f-48e3-95be-d0e66b72ab7c", ACTIVITY_SPORT.BIKING},
                { "eca38408-cb82-42ed-b242-166b43b785a6",ACTIVITY_SPORT.RUNNING},
                { "6f2fdaf9-4c5a-4c2c-a4fa-5be42e9733dd",ACTIVITY_SPORT.SWIMMING} }));
            }

            activities.Should().HaveCountLessOrEqualTo(1, "expected data_tiny.fitlog to contain at least 1 activity");
            var activity = activities[0];
            activity.Name.Should().Be("Lonchamp de Noël");
            activity.Location.Should().Be("Longchamps");
            activity.Sport.Should().Be(ACTIVITY_SPORT.BIKING);
            activity.Calories.Should().Be(2153);
            activity.Tracks.Should().HaveCount(2);
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].Time.Should().Be(DateTimeOffset.Parse("2019-12-25T09:45:07Z"));
            var style = System.Globalization.NumberStyles.AllowDecimalPoint;
            var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].Latitude.Should().Be(float.Parse("48.8676490783691", style, culture));
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].Longitude.Should().Be(float.Parse("2.20366406440735", style, culture));
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].Elevation.Should().Be(float.Parse("91.4405212402344", style, culture));
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].HeartRate.Should().Be(71);
            activity.Tracks[0].TrackSegments[0].TrackPoints[0].Cadence.Should().Be(0);
            activity.Tracks[0].TrackSegments[0].TrackPoints[3].Cadence.Should().Be(27);
            activity.Tracks[1].TrackSegments[0].TrackPoints[5].Cadence.Should().Be(80);
        }

        [Fact]
        public void TestLoadAthlete()
        {
            //setup mock persistence
            var trackBuilder = new TrackEntity.Builder();
            var trackSegmentBuilder = new TrackSegmentEntity.Builder();

            trackSegmentBuilder.TrackPoints.Add(new TrackPointVO.Builder { Time = DateTimeOffset.Now, Cadence = 92, HeartRate = 140, Latitude = 40, Longitude = 8 });
            trackSegmentBuilder.TrackPoints.Add(new TrackPointVO.Builder { Time = DateTimeOffset.Now.AddSeconds(3), Cadence = 92, HeartRate = 140, Latitude = 40, Longitude = 8 });
            trackSegmentBuilder.TrackPoints.Add(new TrackPointVO.Builder { Time = DateTimeOffset.Now.AddSeconds(5), Cadence = 95, HeartRate = 140, Latitude = 40, Longitude = 8 });

            trackBuilder.TrackSegments.Add(trackSegmentBuilder.Build());

            var activities = new List<ActivityEntity>();
            var activityEntity = new ActivityEntity.Builder
            {
                Name = "A bike training",
                Sport = Model.ACTIVITY_SPORT.BIKING
            };
            activityEntity.Tracks.Add(trackBuilder.Build());

            var persistence = A.Fake<IPersistence>();
            A.CallTo(() => persistence.GetAthlete("Test")).Returns(new AthleteEntity(new List<ActivityEntity>() { activityEntity }, "Test", 1));

            //test
            var athlete = persistence.GetAthlete("Test");
            athlete.Name.Should().Be("Test");
            athlete.Activities.Should().HaveCount(1);
            athlete.Activities[0].Tracks[0].TrackSegments[0].TrackPoints.Should().HaveCount(3);
            athlete.Activities[0].Tracks[0].TrackSegments[0].TrackPoints[2].Cadence.Should().Be(95);
        }

        [Fact]
        public void TestPrepareEchartsAthleteData()
        {
            string path = @"data\data_mini.fitlog";
            FitLogImporter importer = new FitLogImporter();

            IList<ActivityEntity> activities;
            using (FileStream fs = File.OpenRead(path))
            {
                activities = new List<ActivityEntity>(importer.ImportActivitiesStream(fs, new Dictionary<string, ACTIVITY_SPORT> {
                { "e41b80e4-fa5f-48e3-95be-d0e66b72ab7c", ACTIVITY_SPORT.BIKING},
                { "eca38408-cb82-42ed-b242-166b43b785a6",ACTIVITY_SPORT.RUNNING},
                { "6f2fdaf9-4c5a-4c2c-a4fa-5be42e9733dd",ACTIVITY_SPORT.SWIMMING} }));
            }

            IEChartsService eChartsService = new EChartsService();

            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTimeOffset StartingDate = new DateTime(now.Year - 2, 1, 1);
            DateTimeOffset EndingDate = now;

            var json = eChartsService.SerializeAthleteData(activities,
                new SerializeAthleteDataConfig { StartingDate = StartingDate, EndingDate = EndingDate });
            json.Length.Should().BeGreaterThan(0);
        }
    }
}
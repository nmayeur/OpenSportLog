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
using Dynastream.Fit;
using OSL.Common.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OSL.Common.Service.Importer
{
    public class FitImporter : IActivitiesImporter
    {
        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();
        static Dictionary<ushort, int> mesgCounts = new Dictionary<ushort, int>();
        private ActivityEntity.Builder _CurrentActivityBuilder = null;


        public IDictionary<string, string> GetSports(Stream stream)
        {
            throw new NotImplementedException();
        }

        private class _FitDecode : Decode
        {
            public ActivityEntity.Builder CurrentActivityBuilder;
            public TrackEntity.Builder CurrentTrackBuilder = null;
            public TrackSegmentEntity.Builder CurrentTrackSegmentBuilder = null;
        }

        public IEnumerable<ActivityEntity> ImportActivitiesStream(Stream stream, IDictionary<string, ACTIVITY_SPORT> categoryMapping)
        {
            _CurrentActivityBuilder = new ActivityEntity.Builder
            {
                OriginSystem = "Fit"
            };
            _FitDecode decoder = new _FitDecode();
            decoder.CurrentActivityBuilder = _CurrentActivityBuilder;

            MesgBroadcaster mesgBroadcaster = new MesgBroadcaster();

            decoder.MesgEvent += mesgBroadcaster.OnMesg;
            decoder.MesgDefinitionEvent += mesgBroadcaster.OnMesgDefinition;
            decoder.DeveloperFieldDescriptionEvent += OnDeveloperFieldDescriptionEvent;

            mesgBroadcaster.MesgEvent += OnMesg;
            mesgBroadcaster.MesgDefinitionEvent += OnMesgDefn;

            mesgBroadcaster.FileIdMesgEvent += OnFileIDMesg;
            mesgBroadcaster.UserProfileMesgEvent += OnUserProfileMesg;
            mesgBroadcaster.MonitoringMesgEvent += OnMonitoringMessage;
            mesgBroadcaster.DeviceInfoMesgEvent += OnDeviceInfoMessage;
            mesgBroadcaster.RecordMesgEvent += OnRecordMessage;

            bool status = decoder.IsFIT(stream);
            status &= decoder.CheckIntegrity(stream);

            if (status)
            {
                _Logger.Debug("Decoding...");
                decoder.Read(stream);
                _Logger.Debug("Decoded FIT file");
            }
            else
            {
                try
                {
                    if (decoder.InvalidDataSize)
                    {
                        _Logger.Warn("Invalid Size Detected, Attempting to decode...");
                        decoder.Read(stream);
                    }
                    else
                    {
                        _Logger.Warn("Attempting to decode by skipping the header...");
                        decoder.Read(stream, DecodeMode.InvalidHeader);
                    }
                }
                catch (FitException ex)
                {
                    _Logger.Error("DecodeDemo caught FitException: {0}", ex.Message);
                    throw ex;
                }
            }

            yield return _CurrentActivityBuilder.Build();
        }

        private static void OnDeveloperFieldDescriptionEvent(object sender, DeveloperFieldDescriptionEventArgs args)
        {
            _Logger.Debug("New Developer Field Description");
            _Logger.Debug("   App Id: {0}", args.Description.ApplicationId);
            _Logger.Debug("   App Version: {0}", args.Description.ApplicationVersion);
            _Logger.Debug("   Field Number: {0}", args.Description.FieldDefinitionNumber);
        }

        #region Message Handlers
        // Client implements their handlers of interest and subscribes to MesgBroadcaster events
        static void OnMesgDefn(object sender, MesgDefinitionEventArgs e)
        {
            _Logger.Debug("OnMesgDef: Received Defn for local message #{0}, global num {1}", e.mesgDef.LocalMesgNum, e.mesgDef.GlobalMesgNum);
            _Logger.Debug("It has {0} fields {1} developer fields and is {2} bytes long",
                e.mesgDef.NumFields,
                e.mesgDef.NumDevFields,
                e.mesgDef.GetMesgSize());
        }

        static void OnMesg(object sender, MesgEventArgs e)
        {
            _FitDecode decoder = (_FitDecode)sender;
            _Logger.Debug("OnMesg: Received Mesg with global ID#{0}, its name is {1}", e.mesg.Num, e.mesg.Name);

            if (e.mesg.Name.Equals("Record"))
            {
                OnMesgRecord(decoder, e.mesg);
            }
            else if (e.mesg.Name.Equals("FileId"))
            {
                OnMesgFileId(decoder, e.mesg);
            }
            else if (e.mesg.Name.Equals("Event"))
            {
                OnMesgEvent(decoder, e.mesg);
            }

            foreach (var devField in e.mesg.DeveloperFields)
            {
                for (int j = 0; j < devField.GetNumValues(); j++)
                {
                    _Logger.Debug("Developer{0} Field#{1} Index{2} (\"{3}\") Value: {4} (raw value {5})",
                        devField.DeveloperDataIndex,
                        devField.Num,
                        j,
                        devField.Name,
                        devField.GetValue(j),
                        devField.GetRawValue(j));
                }
            }

            if (mesgCounts.ContainsKey(e.mesg.Num))
            {
                mesgCounts[e.mesg.Num]++;
            }
            else
            {
                mesgCounts.Add(e.mesg.Num, 1);
            }
        }

        private static void OnMesgEvent(_FitDecode decoder, Mesg mesg)
        {
            EventType? eventType = null;
            foreach (Field field in mesg.Fields)
            {
                for (int j = 0; j < field.GetNumValues(); j++)
                {
                    switch (field.GetName())
                    {
                        case "EventType":
                            byte eventTypeByte;
                            if (byte.TryParse(field.GetValue().ToString(), out eventTypeByte))
                                eventType = (EventType)eventTypeByte;
                            break;
                    }
                }
            }
            if (eventType == EventType.Start)
            {
                //decoder.CurrentTrackPointBuilder = new TrackPointVO.Builder()
                //{
                //};
            }
        }

        private static void OnMesgRecord(_FitDecode decoder, Mesg mesg)
        {
            TrackPointVO.Builder currentTrackPointBuilder = null;
            if (currentTrackPointBuilder == null)
            {
                _Logger.Warn("Record Message, while track point builder is not initialized");
                return;
            }
            bool hasLatitude = false;
            bool hasLongitude = false;
            int i = 0;
            foreach (Field field in mesg.Fields)
            {
                for (int j = 0; j < field.GetNumValues(); j++)
                {
                    switch (field.GetName())
                    {
                        case "Timestamp":
                            long epoch;
                            if (long.TryParse(field.GetValue().ToString(), out epoch))
                            {
                                currentTrackPointBuilder.Time = DateTimeOffset.FromUnixTimeSeconds(epoch);
                            }
                            break;
                        case "PositionLat":
                            long latitude;
                            if (long.TryParse(field.GetValue().ToString(), out latitude))
                            {
                                currentTrackPointBuilder.Latitude = latitude * (180 / 2 ^ 31);
                                hasLatitude = true;
                            }
                            break;
                        case "PositionLong":
                            long longitude;
                            if (long.TryParse(field.GetValue().ToString(), out longitude))
                            {
                                currentTrackPointBuilder.Longitude = longitude * (180 / 2 ^ 31);
                                hasLongitude = true;
                            }
                            break;
                        case "Altitude":
                            float altitude;
                            if (float.TryParse(field.GetValue().ToString(), out altitude))
                            {
                                currentTrackPointBuilder.Elevation = altitude;
                            }
                            break;
                        case "HeartRate":
                            int hr;
                            if (int.TryParse(field.GetValue().ToString(), out hr))
                            {
                                currentTrackPointBuilder.HeartRate = hr;
                            }
                            break;
                    }
                    i++;
                    _Logger.Debug("Field{0} Index{1} (\"{2}\" Field#{4}) Value: {3} (raw value {5})",
                    i,
                    j,
                    field.GetName(),
                    field.GetValue(j),
                    field.Num,
                    field.GetRawValue(j));
                }
            }
            if (hasLatitude && hasLongitude)
            {
                if (decoder.CurrentTrackBuilder == null)
                {
                    decoder.CurrentTrackSegmentBuilder = new TrackSegmentEntity.Builder();
                    decoder.CurrentTrackBuilder = new TrackEntity.Builder()
                    {
                        TrackSegments
                        = new List<TrackSegmentEntity>() { decoder.CurrentTrackSegmentBuilder }
                    };
                }
                else if (decoder.CurrentTrackSegmentBuilder == null)
                {
                    decoder.CurrentTrackSegmentBuilder = new TrackSegmentEntity.Builder();
                    decoder.CurrentTrackBuilder.TrackSegments.Add(decoder.CurrentTrackSegmentBuilder);
                }
                decoder.CurrentTrackSegmentBuilder.TrackPoints.Add(currentTrackPointBuilder.Build());
            }
        }

        private static void OnMesgFileId(_FitDecode decoder, Mesg mesg)
        {
            int i = 0;
            foreach (Field field in mesg.Fields)
            {
                for (int j = 0; j < field.GetNumValues(); j++)
                {
                    if (mesg.Name.Equals("FileId") && field.GetName().Equals("SerialNumber"))
                    {
                        decoder.CurrentActivityBuilder.OriginId = field.GetValue(j).ToString();
                    }
                    _Logger.Debug("Field{0} Index{1} (\"{2}\" Field#{4}) Value: {3} (raw value {5})",
                    i,
                    j,
                    field.GetName(),
                    field.GetValue(j),
                    field.Num,
                    field.GetRawValue(j));
                }
                i++;
            }
        }

        static void OnFileIDMesg(object sender, MesgEventArgs e)
        {
            _Logger.Debug("FileIdHandler: Received {1} Mesg with global ID#{0}", e.mesg.Num, e.mesg.Name);
            FileIdMesg myFileId = (FileIdMesg)e.mesg;
            try
            {
                _Logger.Debug("Type: {0}", myFileId.GetType());
                _Logger.Debug("Manufacturer: {0}", myFileId.GetManufacturer());
                _Logger.Debug("Product: {0}", myFileId.GetProduct());
                _Logger.Debug("SerialNumber {0}", myFileId.GetSerialNumber());
                _Logger.Debug("Number {0}", myFileId.GetNumber());
                _Logger.Debug("TimeCreated {0}", myFileId.GetTimeCreated());

                //Make sure properties with sub properties arent null before trying to create objects based on them
                if (myFileId.GetTimeCreated() != null)
                {
                    Dynastream.Fit.DateTime dtTime = new Dynastream.Fit.DateTime(myFileId.GetTimeCreated().GetTimeStamp());
                }
            }
            catch (FitException exception)
            {
                _Logger.Debug(exception.InnerException, "OnFileIDMesg Error {0}", exception.Message);
            }
        }

        static void OnUserProfileMesg(object sender, MesgEventArgs e)
        {
            _Logger.Debug("UserProfileHandler: Received {1} Mesg, it has global ID#{0}", e.mesg.Num, e.mesg.Name);
            UserProfileMesg myUserProfile = (UserProfileMesg)e.mesg;
            string friendlyName;
            try
            {
                try
                {
                    friendlyName = myUserProfile.GetFriendlyNameAsString();
                }
                catch (ArgumentNullException)
                {
                    //There is no FriendlyName property
                    friendlyName = "";
                }
                _Logger.Debug("FriendlyName \"{0}\"", friendlyName);
                _Logger.Debug("Gender {0}", myUserProfile.GetGender().ToString());
                _Logger.Debug("Age {0}", myUserProfile.GetAge());
                _Logger.Debug("Weight  {0}", myUserProfile.GetWeight());
            }
            catch (FitException exception)
            {
                _Logger.Debug(exception.InnerException, "OnUserProfileMesg Error {0}", exception.Message);
            }
        }

        static void OnDeviceInfoMessage(object sender, MesgEventArgs e)
        {
            _Logger.Debug("DeviceInfoHandler: Received {1} Mesg, it has global ID#{0}", e.mesg.Num, e.mesg.Name);
            DeviceInfoMesg myDeviceInfoMessage = (DeviceInfoMesg)e.mesg;
            try
            {
                _Logger.Debug("Timestamp  {0}", myDeviceInfoMessage.GetTimestamp());
                _Logger.Debug("Battery Status{0}", myDeviceInfoMessage.GetBatteryStatus());
            }
            catch (FitException exception)
            {
                _Logger.Debug(exception.InnerException, "OnDeviceInfoMesg Error {0}", exception.Message);
            }
        }

        static void OnMonitoringMessage(object sender, MesgEventArgs e)
        {
            _Logger.Debug("MonitoringHandler: Received {1} Mesg, it has global ID#{0}", e.mesg.Num, e.mesg.Name);
            MonitoringMesg myMonitoringMessage = (MonitoringMesg)e.mesg;
            try
            {
                _Logger.Debug("Timestamp  {0}", myMonitoringMessage.GetTimestamp());
                _Logger.Debug("ActivityType {0}", myMonitoringMessage.GetActivityType());
                switch (myMonitoringMessage.GetActivityType()) // Cycles is a dynamic field
                {
                    case ActivityType.Walking:
                    case ActivityType.Running:
                        _Logger.Debug("Steps {0}", myMonitoringMessage.GetSteps());
                        break;
                    case ActivityType.Cycling:
                    case ActivityType.Swimming:
                        _Logger.Debug("Strokes {0}", myMonitoringMessage.GetStrokes());
                        break;
                    default:
                        _Logger.Debug("Cycles {0}", myMonitoringMessage.GetCycles());
                        break;
                }
            }
            catch (FitException exception)
            {
                _Logger.Debug(exception.InnerException, "OnDeviceInfoMesg Error {0}", exception.Message);
            }
        }

        private static void OnRecordMessage(object sender, MesgEventArgs e)
        {
            _Logger.Debug("Record Handler: Received {0} Mesg, it has global ID#{1}",
                e.mesg.Num,
                e.mesg.Name);

            var recordMessage = (RecordMesg)e.mesg;

            WriteFieldWithOverrides(recordMessage, RecordMesg.FieldDefNum.HeartRate);
            WriteFieldWithOverrides(recordMessage, RecordMesg.FieldDefNum.Cadence);
            WriteFieldWithOverrides(recordMessage, RecordMesg.FieldDefNum.Speed);
            WriteFieldWithOverrides(recordMessage, RecordMesg.FieldDefNum.Distance);

            WriteDeveloperFields(recordMessage);
        }

        private static void WriteDeveloperFields(Mesg mesg)
        {
            foreach (var devField in mesg.DeveloperFields)
            {
                if (devField.GetNumValues() <= 0)
                {
                    continue;
                }

                if (devField.IsDefined)
                {
                    Console.Write("{0}", devField.Name);

                    if (devField.Units != null)
                    {
                        Console.Write(" [{0}]", devField.Units);
                    }
                    Console.Write(": ");
                }
                else
                {
                    Console.Write("Undefined Field: ");
                }

                Console.Write("{0}", devField.GetValue(0));
                for (int i = 1; i < devField.GetNumValues(); i++)
                {
                    Console.Write(",{0}", devField.GetValue(i));
                }
            }
        }

        private static void WriteFieldWithOverrides(Mesg mesg, byte fieldNumber)
        {
            Field profileField = Profile.GetField(mesg.Num, fieldNumber);
            bool nameWritten = false;

            if (null == profileField)
            {
                return;
            }

            IEnumerable<FieldBase> fields = mesg.GetOverrideField(fieldNumber);

            foreach (FieldBase field in fields)
            {
                if (!nameWritten)
                {
                    _Logger.Debug("   {0}", profileField.GetName());
                    nameWritten = true;
                }

                if (field is Field)
                {
                    _Logger.Debug("      native: {0}", field.GetValue());
                }
                else
                {
                    _Logger.Debug("      override: {0}", field.GetValue());
                }
            }
        }

        #endregion
    }
}

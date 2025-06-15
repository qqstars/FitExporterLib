#define NET35

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Dynastream.Fit;
using CyclingAnalyzer.FitExporterLib.DataModel;

namespace CyclingAnalyzer.FitExporterLib.GarminFit
{
    public enum GarminFitValidation
    {
        ValidFile = 0,
        IntegrityCheckFailed = 1,
        Unknown = 99,
        NotFitFile = 100,
        Bad = 10000
    }

    public class GarminFitReader : IDisposable
    {
        //private string _filePath;
        ////private FileStream fitSource;

        //public GarminFitReader(string filePath)
        //{
        //    this._filePath = filePath;

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isValidFile"></param>
        /// <param name="cyclingData">if is not valid file, return null</param>
        /// <returns></returns>
        public static bool ReadFile(string filePath, out GarminFitValidation isValidFile, out CyclingData cyclingData, out System.DateTime firstPointDateTime, double lthr = 160, double ftp = 200)
        {
            System.DateTime dt;
            bool result = ReadFile(filePath, false, out dt, out isValidFile, out cyclingData, out firstPointDateTime);
            cyclingData.GetExtraData(cyclingData.ParamSlopeSplitMeter);
            cyclingData.GetRangeData(lthr, ftp);

            return result;
        }

        public static System.DateTime GetID(string filePath, out System.DateTime firstPointDateTime)
        {
            GarminFitValidation isValidFile;
            CyclingData cyclingData;

            System.DateTime dt;

            ReadFile(filePath, true, out dt, out isValidFile, out cyclingData, out firstPointDateTime);

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isValidFile"></param>
        /// <param name="cyclingData">if is not valid file, return null</param>
        /// <returns></returns>
        private static bool ReadFile(string filePath, bool isOnlyGetID, out System.DateTime id, out GarminFitValidation isValidFile, out CyclingData cyclingData, out System.DateTime firstPointDateTime, double? lthr = null, double? ftp = null)
        {
            isValidFile = GarminFitValidation.Bad;
            cyclingData = null;
            id = new System.DateTime();
            firstPointDateTime = new System.DateTime();
            System.DateTime firstTime = new System.DateTime();
            System.DateTime realID = id;

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return false;
            }

            // Attempt to open .FIT file
            using (MemoryStream fitSource = new MemoryStream(ReadAllBytes(filePath)))//new FileStream(filePath, FileMode.Open))
            {
                isValidFile = GarminFitValidation.Unknown;

                Decode decoder = new Decode();
                MesgBroadcaster mesgBroadcaster = new MesgBroadcaster();

                // Connect the Broadcaster to our event (message) source (in this case the Decoder)
                decoder.MesgEvent += mesgBroadcaster.OnMesg;
                decoder.MesgDefinitionEvent += mesgBroadcaster.OnMesgDefinition;

                bool status = decoder.IsFIT(fitSource);

                if (!status)
                {
                    isValidFile = GarminFitValidation.NotFitFile;
                    return false;
                }

                cyclingData = new CyclingData(lthr, ftp);

                CyclingData result = cyclingData;

                result.StandEvent = new List<StandEvent>();
                //CyclingDataPoint tempPoint = null;

                //mesgBroadcaster.MesgDefinitionEvent += new MesgDefinitionEventHandler((obj, e) =>
                //{
                //    Console.WriteLine("OnMesgDef: Received Defn for local message #{0}, global num {1}", e.mesgDef.LocalMesgNum, e.mesgDef.GlobalMesgNum);
                //    Console.WriteLine("\tIt has {0} fields and is {1} bytes long", e.mesgDef.NumFields, e.mesgDef.GetMesgSize());


                //});


                // Subscribe to message events of interest by connecting to the Broadcaster
                MesgEventHandler handler = null;
                handler = new MesgEventHandler((obj, e) =>
                {
                    //Console.WriteLine("OnMesg: Received Mesg with global ID#{0}, its name is {1}", e.mesg.Num, e.mesg.Name);

                    //for (byte i = 0; i < e.mesg.GetNumFields(); i++)
                    //{
                    //    for (int j = 0; j < e.mesg.fields[i].values.Count; j++)
                    //    {
                    //        Console.WriteLine("\tField{0} Index{1} (\"{2}\" Field#{4}) Value: {3}", i, j, e.mesg.fields[i].GetName(), e.mesg.fields[i].values[j], e.mesg.fields[i].Num);
                    //    }
                    //}
                    #region FileId
                    if (e.mesg.Name == "FileId")
                    {
                        for (byte i = 0; i < e.mesg.GetNumFields(); i++)
                        {
                            ////for (int j = 0; j < e.mesg.fields[i].values.Count; j++)
                            ////{
                            Field f = e.mesg.fields[i];

                            if (f.Name == "TimeCreated")
                            {
                                Dynastream.Fit.DateTime dt = new Dynastream.Fit.DateTime((uint)f.GetValue());
                                System.DateTime idTime = dt.GetDateTime().ToLocalTime();

                                ////if (isOnlyGetID)
                                ////{
                                ////    mesgBroadcaster.MesgEvent -= handler;
                                ////    realID = idTime;
                                ////    break;
                                ////}
                            }

                            ////Console.WriteLine("\tField{0} Index{1} (\"{2}\" Field#{4}) Value: {3}", i, j, e.mesg.fields[i].GetName(), e.mesg.fields[i].values[j], e.mesg.fields[i].Num);
                            ////}
                        }
                    }
                    #endregion

                    #region Activity
                    else if (e.mesg.Name == "Activity")
                    {
                        for (byte i = 0; i < e.mesg.GetNumFields(); i++)
                        {
                            ////for (int j = 0; j < e.mesg.fields[i].values.Count; j++)
                            ////{
                            Field f = e.mesg.fields[i];

                            if (f.Name == "Timestamp")
                            {
                                ////Dynastream.Fit.DateTime dt = new Dynastream.Fit.DateTime((uint)f.GetValue());
                                ////System.DateTime idTime = dt.GetDateTime().ToLocalTime();

                                ////if (isOnlyGetID)
                                ////{
                                ////    mesgBroadcaster.MesgEvent -= handler;
                                ////    realID = idTime;
                                ////}
                            }

                            ////Console.WriteLine("\tField{0} Index{1} (\"{2}\" Field#{4}) Value: {3}", i, j, e.mesg.fields[i].GetName(), e.mesg.fields[i].values[j], e.mesg.fields[i].Num);
                            ////}
                        }
                    }
                    #endregion

                    #region Lap
                    else if (e.mesg.Name == "Lap")
                    {
                        System.DateTime lapTime = new System.DateTime();
                        double cal = 0;
                        double timeStandingSeconds = 0;
                        int standCount = 0;

                        #region get value
                        for (byte i = 0; i < e.mesg.GetNumFields(); i++)
                        {
                            Field f = e.mesg.fields[i];
                            object value = f.GetValue();//f.values[j];

                            if (f.Name == "StartTime")
                            {
                                Dynastream.Fit.DateTime dt = new Dynastream.Fit.DateTime((uint)value);
                                lapTime = dt.GetDateTime().ToLocalTime();

                                if (isOnlyGetID)
                                {
                                    if (realID == new System.DateTime())
                                    {
                                        realID = lapTime;
                                    }

                                    if (firstTime != new System.DateTime())
                                    {
                                        mesgBroadcaster.MesgEvent -= handler;
                                    }

                                    break;
                                }
                            }
                            else if (f.Name == "TotalCalories")
                            {
                                cal = Convert.ToDouble(value);
                            }
                            else if (f.Name == "TimeStanding")
                            {
                                timeStandingSeconds = Convert.ToDouble(value);

                                if (timeStandingSeconds >= 4294967.5)
                                {
                                    timeStandingSeconds = 0;
                                }
                            }
                            else if (f.Name == "StandCount")
                            {
                                standCount = (int)Convert.ToDouble(value);
                                if (standCount >= 65535)
                                {
                                    standCount = 0;
                                }
                            }

                        }
                        #endregion

                        result.Laps.Add(new CyclingLapData(ref result, lapTime) { Calories = cal, TimeStandingSeconds = timeStandingSeconds, StandCount = standCount });

                        //if (lapTime.Subtract(new System.DateTime()).TotalSeconds > 10000 && lapTime != lastLapTime)
                        //{
                        //    result.Laps.Add(new CyclingLapData(ref result, lastLapTime) { Calories = cal });

                        //    lastLapTime = lapTime.AddSeconds(1);
                        //}
                    }
                    #endregion

                    #region Record
                    else if (e.mesg.Name == "Record")
                    {
                        CyclingDataPoint point = new CyclingDataPoint();

                        #region Get data
                        for (byte i = 0; i < e.mesg.GetNumFields(); i++)
                        {
                            Field f = e.mesg.fields[i];
                            object value = f.GetValue();//f.values[j];

                            //for (int j = 0; j < e.mesg.fields[i].values.Count; j++)
                            //{
                            if (f.Name == "Timestamp")
                            {
                                Dynastream.Fit.DateTime dt = new Dynastream.Fit.DateTime((uint)value);
                                point.Time = dt.GetDateTime().ToLocalTime();

                                if (isOnlyGetID)
                                {
                                    if (firstTime == new System.DateTime())
                                    {
                                        firstTime = point.Time;
                                    }

                                    if (realID != new System.DateTime())
                                    {
                                        mesgBroadcaster.MesgEvent -= handler;
                                    }

                                    break;
                                }
                            }
                            else if (f.Name == "PositionLat")
                            {
                                // semicircles convert to degree
                                // semicircles = degrees * ( 2^31 / 180 )
                                // degrees = semicircles * ( 180 / 2^31 )
                                double lat = Convert.ToDouble(value) * (180 / ((double)int.MaxValue + 1));
                                point.Position.LatitudeDegrees = lat;
                            }
                            else if (f.Name == "PositionLong")
                            {
                                double lng = Convert.ToDouble(value) * (180 / ((double)int.MaxValue + 1));
                                point.Position.LongitudeDegrees = lng;
                            }
                            else if (f.Name == "Altitude")
                            {
                                double alti = Convert.ToDouble(value);
                                point.Position.AltitudeMeters = alti;
                            }
                            else if (f.Name == "Distance")
                            {
                                double dist = Convert.ToDouble(value);

                                point.DistanceMeter = dist;
                            }
                            else if (f.Name == "HeartRate")
                            {
                                point.HeartRateBpm = Convert.ToDouble(value);
                            }
                            else if (f.Name == "Cadence")
                            {
                                point.Cadence = Convert.ToDouble(value);
                            }
                            else if (f.Name == "Speed")
                            {
                                point.Speed = Convert.ToDouble(value) * 3.6;
                            }
                            else if (f.Name == "Power")
                            {
                                point.Power.TotalPower = Convert.ToDouble(value);
                            }
                            else if (f.Name == "Temperature")
                            {
                                point.Temperature = Convert.ToDouble(value);
                            }
                            else if (f.Name == "LeftRightBalance")
                            {
                                byte pb = Convert.ToByte(value);
                                bool isRight = (pb & LeftRightBalance.Right) == LeftRightBalance.Right;
                                int realV = Convert.ToInt32(pb & LeftRightBalance.Mask);

                                if (0 <= realV && 100 >= realV)
                                {
                                    if (isRight)
                                    {
                                        point.Power.RightLegPower = realV;
                                    }
                                    else
                                    {
                                        point.Power.RightLegPower = 100 - realV;
                                    }
                                }
                                else
                                { }
                            }
                            else if (f.Name == "LeftTorqueEffectiveness")
                            {
                                point.Power.LeftTorqueEffectiveness = Convert.ToDouble(value);
                            }
                            else if (f.Name == "RightTorqueEffectiveness")
                            {
                                point.Power.RightTorqueEffectiveness = Convert.ToDouble(value);
                            }
                            else if (f.Name == "LeftPedalSmoothness")
                            {
                                point.Power.LeftPedalSmoothness = Convert.ToDouble(value);
                            }
                            else if (f.Name == "RightPedalSmoothness")
                            {
                                point.Power.RightPedalSmoothness = Convert.ToDouble(value);
                            }
                            else if (f.Name == "AccumulatedPower")
                            {
                                point.Power.AccumulatedPower = Convert.ToDouble(value);
                            }
                            else if (f.Name == "LeftPco")
                            {
                                point.Power.LeftPlatformCenterOffset = Convert.ToDouble(value);
                            }
                            else if (f.Name == "LeftPowerPhase" && f.values != null && f.values.Count > 1)
                            {
                                point.Power.LeftPowerPhaseStart = Convert.ToDouble(f.GetValue(0));
                                point.Power.LeftPowerPhaseEnd = Convert.ToDouble(f.GetValue(1));

                                if (point.Power.LeftPowerPhasePeakStart > 360)
                                    point.Power.LeftPowerPhasePeakStart -= 360;
                                if (point.Power.LeftPowerPhasePeakEnd > 360)
                                    point.Power.LeftPowerPhasePeakEnd -= 360;
                            }
                            else if (f.Name == "LeftPowerPhasePeak" && f.values != null && f.values.Count > 1)
                            {
                                point.Power.LeftPowerPhasePeakStart = Convert.ToDouble(f.GetValue(0));
                                point.Power.LeftPowerPhasePeakEnd = Convert.ToDouble(f.GetValue(1));

                                if (point.Power.LeftPowerPhaseStart > 360)
                                    point.Power.LeftPowerPhaseStart -= 360;
                                if (point.Power.LeftPowerPhaseEnd > 360)
                                    point.Power.LeftPowerPhaseEnd -= 360;
                            }
                            else if (f.Name == "RightPco")
                            {
                                point.Power.RightPlatformCenterOffset = Convert.ToDouble(value);
                            }
                            else if (f.Name == "RightPowerPhase" && f.values != null && f.values.Count > 1)
                            {
                                point.Power.RightPowerPhaseStart = Convert.ToDouble(f.GetValue(0));
                                point.Power.RightPowerPhaseEnd = Convert.ToDouble(f.GetValue(1));
                            }
                            else if (f.Name == "RightPowerPhasePeak" && f.values != null && f.values.Count > 1)
                            {
                                point.Power.RightPowerPhasePeakStart = Convert.ToDouble(f.GetValue(0));
                                point.Power.RightPowerPhasePeakEnd = Convert.ToDouble(f.GetValue(1));
                            }
                            ////else if (f.Name == "StanceTime")
                            ////{
                            ////    point.Power.StanceTime = Convert.ToDouble(value);
                            ////}

                            //Console.WriteLine("\tField{0} Index{1} (\"{2}\" Field#{4}) Value: {3}", i, j, e.mesg.fields[i].GetName(), e.mesg.fields[i].values[j], e.mesg.fields[i].Num);
                            //}
                        }
                        #endregion

                        point.Power.RightLegPower = (double)point.Power.RightLegPower * point.Power.TotalPower / 100;
                        point.Power.LeftLegPower = point.Power.TotalPower - point.Power.RightLegPower;

                        result.AddCyclingDataPointWithFixing(point);
                    }

                    #endregion

                    #region Events
                    else if (e.mesg.Name == "Event")
                    {
                        var eventMsg = new EventMesg(e.mesg);

                        var type = eventMsg.GetEventType();

                        var eventGet = eventMsg.GetEvent();

                        if (type == EventType.Marker && eventGet.ToString() == "RiderPositionChange")
                        {
                            RiderPositionType position = RiderPositionType.Invalid;
                            System.DateTime timestemp = new System.DateTime();

                            for (byte i = 0; i < eventMsg.GetNumFields(); i++)
                            {
                                ////for (int j = 0; j < e.mesg.fields[i].values.Count; j++)
                                ////{
                                Field f = eventMsg.fields[i];

                                if (f.Name == "Timestamp")
                                {
                                    Dynastream.Fit.DateTime dt = new Dynastream.Fit.DateTime((uint)f.GetValue());
                                    timestemp = dt.GetDateTime().ToLocalTime();

                                    ////if (isOnlyGetID)
                                    ////{
                                    ////    mesgBroadcaster.MesgEvent -= handler;
                                    ////    realID = idTime;
                                    ////    break;
                                    ////}
                                }
                                else if (f.Name == "Data")
                                {
                                    var positionValue = Convert.ToByte(f.GetValue());
                                    position = (RiderPositionType)positionValue;
                                }

                                ////    Console.WriteLine("\tField{0} Index{1} (\"{2}\" Field#{4}) Value: {3}", i, j, e.mesg.fields[i].GetName(), e.mesg.fields[i].values[j], e.mesg.fields[i].Num);
                                ////}
                            }

                            if (position == RiderPositionType.Standing)
                            {
                                result.StandEvent.Add(new StandEvent() { StartTime = timestemp });
                            }
                            else if (position == RiderPositionType.Seated && result.StandEvent.Count > 0 && result.StandEvent.Last().EndTime <= new System.DateTime())
                            {
                                result.StandEvent[result.StandEvent.Count - 1].EndTime = timestemp;
                                ////result.ProcessStanding(result.StandEvent[result.StandEvent.Count - 1]);
                            }
                        }
                    }
                    #endregion

                });

                mesgBroadcaster.MesgEvent += handler;

                //mesgBroadcaster.FileIdMesgEvent += new MesgEventHandler(OnFileIDMesg);
                //mesgBroadcaster.UserProfileMesgEvent += new MesgEventHandler(OnUserProfileMesg);

                status = decoder.CheckIntegrity(fitSource);

                // Process the file
                if (status == true)
                {
                    isValidFile = GarminFitValidation.ValidFile;
                    decoder.Read(fitSource);
                }
                else
                {
                    try
                    {
                        isValidFile = GarminFitValidation.IntegrityCheckFailed;
                        decoder.Read(fitSource);
                    }
                    catch (FitException ex)
                    {
                        Console.WriteLine("DecodeDemo caught FitException: " + ex.Message);
                    }
                }
                fitSource.Close();
            }

            id = realID;

            firstPointDateTime = firstTime;

            if (cyclingData != null)
            {
                cyclingData.ProcessAllStanding();
            }

            return true;
        }

        public static byte[] ReadAllBytes(string filePath)
        {
            List<byte> resultLst = new List<byte>();
            byte[] result = new byte[0];
            if (System.IO.File.Exists(filePath))
            {
                using (System.IO.FileStream fs = GetFileStream(filePath))
                {
                    byte[] results = new byte[fs.Length];
                    int readCount = 0;
                    while ((readCount = fs.Read(results, 0, (int)results.Length)) > 0)
                    {
                        resultLst.AddRange(results);
                    }

                    fs.Close();

                    result = resultLst.ToArray();
                }
            }

            return result;
        }

        public static FileStream GetFileStream(string filePath, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read)
        {
            return new System.IO.FileStream(filePath, mode, access, FileShare.ReadWrite);
        }

        public void Dispose()
        {
            //if (null != fitSource)
            //{
            //    fitSource.Close();
            //}
        }
    }
}

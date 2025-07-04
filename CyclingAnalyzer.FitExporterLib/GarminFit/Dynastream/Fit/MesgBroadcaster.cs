#region Copyright
////////////////////////////////////////////////////////////////////////////////
// The following FIT Protocol software provided may be used with FIT protocol
// devices only and remains the copyrighted property of Dynastream Innovations Inc.
// The software is being provided on an "as-is" basis and as an accommodation,
// and therefore all warranties, representations, or guarantees of any kind
// (whether express, implied or statutory) including, without limitation,
// warranties of merchantability, non-infringement, or fitness for a particular
// purpose, are specifically disclaimed.
//
// Copyright 2015 Dynastream Innovations Inc.
////////////////////////////////////////////////////////////////////////////////
// ****WARNING****  This file is auto-generated!  Do NOT edit this file.
// Profile Version = 14.20Release
// Tag = development/akw/14.20.00
////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Dynastream.Fit
{
   // Define our necessary event types (EventArgs and the delegate)
   public delegate void MesgEventHandler(object sender, MesgEventArgs e);
   public delegate void MesgDefinitionEventHandler(object sender, MesgDefinitionEventArgs e);   
   
   public class MesgEventArgs : EventArgs
   {
      public Mesg mesg = null;

      public MesgEventArgs()
      {
      }

      public MesgEventArgs(Mesg newMesg)
      {         
         mesg = new Mesg(newMesg);
      }
   }

   public class MesgDefinitionEventArgs : EventArgs
   {         
      public MesgDefinition mesgDef = null;

      public MesgDefinitionEventArgs()
      {
      }

      public MesgDefinitionEventArgs(MesgDefinition newDefn)
      {
         mesgDef = new MesgDefinition(newDefn);
      }
   }

   /// <summary>
   /// The MesgBroadcaster manages Mesg and MesgDefinition events.  Its
   /// handlers should be connected to the source of Mesg and MesgDef events
   /// (such as a file decoder). 
   /// Clients may subscribe to the Broadcasters events (Mesg, Mesg Def
   /// or specofic Profile Mesg)   
   /// </summary>  
   public class MesgBroadcaster
   {
      #region Methods & Events
      public event MesgDefinitionEventHandler MesgDefinitionEvent;
      public event MesgEventHandler MesgEvent;      
      // One event for every Profile Mesg 
      public event MesgEventHandler FileIdMesgEvent;
      public event MesgEventHandler FileCreatorMesgEvent;
      public event MesgEventHandler SoftwareMesgEvent;
      public event MesgEventHandler SlaveDeviceMesgEvent;
      public event MesgEventHandler CapabilitiesMesgEvent;
      public event MesgEventHandler FileCapabilitiesMesgEvent;
      public event MesgEventHandler MesgCapabilitiesMesgEvent;
      public event MesgEventHandler FieldCapabilitiesMesgEvent;
      public event MesgEventHandler DeviceSettingsMesgEvent;
      public event MesgEventHandler UserProfileMesgEvent;
      public event MesgEventHandler HrmProfileMesgEvent;
      public event MesgEventHandler SdmProfileMesgEvent;
      public event MesgEventHandler BikeProfileMesgEvent;
      public event MesgEventHandler ZonesTargetMesgEvent;
      public event MesgEventHandler SportMesgEvent;
      public event MesgEventHandler HrZoneMesgEvent;
      public event MesgEventHandler SpeedZoneMesgEvent;
      public event MesgEventHandler CadenceZoneMesgEvent;
      public event MesgEventHandler PowerZoneMesgEvent;
      public event MesgEventHandler MetZoneMesgEvent;
      public event MesgEventHandler GoalMesgEvent;
      public event MesgEventHandler ActivityMesgEvent;
      public event MesgEventHandler SessionMesgEvent;
      public event MesgEventHandler LapMesgEvent;
      public event MesgEventHandler LengthMesgEvent;
      public event MesgEventHandler RecordMesgEvent;
      public event MesgEventHandler EventMesgEvent;
      public event MesgEventHandler DeviceInfoMesgEvent;
      public event MesgEventHandler TrainingFileMesgEvent;
      public event MesgEventHandler HrvMesgEvent;
      public event MesgEventHandler CourseMesgEvent;
      public event MesgEventHandler CoursePointMesgEvent;
      public event MesgEventHandler WorkoutMesgEvent;
      public event MesgEventHandler WorkoutStepMesgEvent;
      public event MesgEventHandler ScheduleMesgEvent;
      public event MesgEventHandler TotalsMesgEvent;
      public event MesgEventHandler WeightScaleMesgEvent;
      public event MesgEventHandler BloodPressureMesgEvent;
      public event MesgEventHandler MonitoringInfoMesgEvent;
      public event MesgEventHandler MonitoringMesgEvent;
      public event MesgEventHandler MemoGlobMesgEvent;
      public event MesgEventHandler PadMesgEvent;
      

      public void OnMesg(object sender, MesgEventArgs e)
      {
         // Notify any subscribers of either our general mesg event or specific profile mesg event
         if (MesgEvent != null)
         {
            MesgEvent(sender, e);
         }

         switch (e.mesg.Num)
         {            
            case (ushort)MesgNum.FileId:
               if (FileIdMesgEvent != null)
               {
                  FileIdMesg fileIdMesg = new FileIdMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = fileIdMesg;
                  FileIdMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.FileCreator:
               if (FileCreatorMesgEvent != null)
               {
                  FileCreatorMesg fileCreatorMesg = new FileCreatorMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = fileCreatorMesg;
                  FileCreatorMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Software:
               if (SoftwareMesgEvent != null)
               {
                  SoftwareMesg softwareMesg = new SoftwareMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = softwareMesg;
                  SoftwareMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.SlaveDevice:
               if (SlaveDeviceMesgEvent != null)
               {
                  SlaveDeviceMesg slaveDeviceMesg = new SlaveDeviceMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = slaveDeviceMesg;
                  SlaveDeviceMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Capabilities:
               if (CapabilitiesMesgEvent != null)
               {
                  CapabilitiesMesg capabilitiesMesg = new CapabilitiesMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = capabilitiesMesg;
                  CapabilitiesMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.FileCapabilities:
               if (FileCapabilitiesMesgEvent != null)
               {
                  FileCapabilitiesMesg fileCapabilitiesMesg = new FileCapabilitiesMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = fileCapabilitiesMesg;
                  FileCapabilitiesMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.MesgCapabilities:
               if (MesgCapabilitiesMesgEvent != null)
               {
                  MesgCapabilitiesMesg mesgCapabilitiesMesg = new MesgCapabilitiesMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = mesgCapabilitiesMesg;
                  MesgCapabilitiesMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.FieldCapabilities:
               if (FieldCapabilitiesMesgEvent != null)
               {
                  FieldCapabilitiesMesg fieldCapabilitiesMesg = new FieldCapabilitiesMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = fieldCapabilitiesMesg;
                  FieldCapabilitiesMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.DeviceSettings:
               if (DeviceSettingsMesgEvent != null)
               {
                  DeviceSettingsMesg deviceSettingsMesg = new DeviceSettingsMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = deviceSettingsMesg;
                  DeviceSettingsMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.UserProfile:
               if (UserProfileMesgEvent != null)
               {
                  UserProfileMesg userProfileMesg = new UserProfileMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = userProfileMesg;
                  UserProfileMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.HrmProfile:
               if (HrmProfileMesgEvent != null)
               {
                  HrmProfileMesg hrmProfileMesg = new HrmProfileMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = hrmProfileMesg;
                  HrmProfileMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.SdmProfile:
               if (SdmProfileMesgEvent != null)
               {
                  SdmProfileMesg sdmProfileMesg = new SdmProfileMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = sdmProfileMesg;
                  SdmProfileMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.BikeProfile:
               if (BikeProfileMesgEvent != null)
               {
                  BikeProfileMesg bikeProfileMesg = new BikeProfileMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = bikeProfileMesg;
                  BikeProfileMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.ZonesTarget:
               if (ZonesTargetMesgEvent != null)
               {
                  ZonesTargetMesg zonesTargetMesg = new ZonesTargetMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = zonesTargetMesg;
                  ZonesTargetMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Sport:
               if (SportMesgEvent != null)
               {
                  SportMesg sportMesg = new SportMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = sportMesg;
                  SportMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.HrZone:
               if (HrZoneMesgEvent != null)
               {
                  HrZoneMesg hrZoneMesg = new HrZoneMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = hrZoneMesg;
                  HrZoneMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.SpeedZone:
               if (SpeedZoneMesgEvent != null)
               {
                  SpeedZoneMesg speedZoneMesg = new SpeedZoneMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = speedZoneMesg;
                  SpeedZoneMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.CadenceZone:
               if (CadenceZoneMesgEvent != null)
               {
                  CadenceZoneMesg cadenceZoneMesg = new CadenceZoneMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = cadenceZoneMesg;
                  CadenceZoneMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.PowerZone:
               if (PowerZoneMesgEvent != null)
               {
                  PowerZoneMesg powerZoneMesg = new PowerZoneMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = powerZoneMesg;
                  PowerZoneMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.MetZone:
               if (MetZoneMesgEvent != null)
               {
                  MetZoneMesg metZoneMesg = new MetZoneMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = metZoneMesg;
                  MetZoneMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Goal:
               if (GoalMesgEvent != null)
               {
                  GoalMesg goalMesg = new GoalMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = goalMesg;
                  GoalMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Activity:
               if (ActivityMesgEvent != null)
               {
                  ActivityMesg activityMesg = new ActivityMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = activityMesg;
                  ActivityMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Session:
               if (SessionMesgEvent != null)
               {
                  SessionMesg sessionMesg = new SessionMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = sessionMesg;
                  SessionMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Lap:
               if (LapMesgEvent != null)
               {
                  LapMesg lapMesg = new LapMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = lapMesg;
                  LapMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Length:
               if (LengthMesgEvent != null)
               {
                  LengthMesg lengthMesg = new LengthMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = lengthMesg;
                  LengthMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Record:
               if (RecordMesgEvent != null)
               {
                  RecordMesg recordMesg = new RecordMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = recordMesg;
                  RecordMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Event:
               if (EventMesgEvent != null)
               {
                  EventMesg eventMesg = new EventMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = eventMesg;
                  EventMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.DeviceInfo:
               if (DeviceInfoMesgEvent != null)
               {
                  DeviceInfoMesg deviceInfoMesg = new DeviceInfoMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = deviceInfoMesg;
                  DeviceInfoMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.TrainingFile:
               if (TrainingFileMesgEvent != null)
               {
                  TrainingFileMesg trainingFileMesg = new TrainingFileMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = trainingFileMesg;
                  TrainingFileMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Hrv:
               if (HrvMesgEvent != null)
               {
                  HrvMesg hrvMesg = new HrvMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = hrvMesg;
                  HrvMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Course:
               if (CourseMesgEvent != null)
               {
                  CourseMesg courseMesg = new CourseMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = courseMesg;
                  CourseMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.CoursePoint:
               if (CoursePointMesgEvent != null)
               {
                  CoursePointMesg coursePointMesg = new CoursePointMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = coursePointMesg;
                  CoursePointMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Workout:
               if (WorkoutMesgEvent != null)
               {
                  WorkoutMesg workoutMesg = new WorkoutMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = workoutMesg;
                  WorkoutMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.WorkoutStep:
               if (WorkoutStepMesgEvent != null)
               {
                  WorkoutStepMesg workoutStepMesg = new WorkoutStepMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = workoutStepMesg;
                  WorkoutStepMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Schedule:
               if (ScheduleMesgEvent != null)
               {
                  ScheduleMesg scheduleMesg = new ScheduleMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = scheduleMesg;
                  ScheduleMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Totals:
               if (TotalsMesgEvent != null)
               {
                  TotalsMesg totalsMesg = new TotalsMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = totalsMesg;
                  TotalsMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.WeightScale:
               if (WeightScaleMesgEvent != null)
               {
                  WeightScaleMesg weightScaleMesg = new WeightScaleMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = weightScaleMesg;
                  WeightScaleMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.BloodPressure:
               if (BloodPressureMesgEvent != null)
               {
                  BloodPressureMesg bloodPressureMesg = new BloodPressureMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = bloodPressureMesg;
                  BloodPressureMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.MonitoringInfo:
               if (MonitoringInfoMesgEvent != null)
               {
                  MonitoringInfoMesg monitoringInfoMesg = new MonitoringInfoMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = monitoringInfoMesg;
                  MonitoringInfoMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Monitoring:
               if (MonitoringMesgEvent != null)
               {
                  MonitoringMesg monitoringMesg = new MonitoringMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = monitoringMesg;
                  MonitoringMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.MemoGlob:
               if (MemoGlobMesgEvent != null)
               {
                  MemoGlobMesg memoGlobMesg = new MemoGlobMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = memoGlobMesg;
                  MemoGlobMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         case (ushort)MesgNum.Pad:
               if (PadMesgEvent != null)
               {
                  PadMesg padMesg = new PadMesg(e.mesg);
                  MesgEventArgs mesgEventArgs = new MesgEventArgs();
                  mesgEventArgs.mesg = padMesg;
                  PadMesgEvent(sender, mesgEventArgs);
               }
               break;
            
         }
      }

      public void OnMesgDefinition(object sender, MesgDefinitionEventArgs e)
      {
         // Notify any subscribers         
         if (MesgDefinitionEvent != null)
         {            
            MesgDefinitionEvent(sender, e);
         }
      }
      #endregion // Methods
   } // Class
} // namespace

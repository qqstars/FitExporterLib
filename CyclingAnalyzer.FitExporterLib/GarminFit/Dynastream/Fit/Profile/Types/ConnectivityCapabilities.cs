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

namespace Dynastream.Fit
{
   /// <summary>
   /// Implements the profile ConnectivityCapabilities type as a class
   /// </summary>  
   public static class ConnectivityCapabilities 
   {
      public const uint Bluetooth = 0x00000001;
      public const uint BluetoothLe = 0x00000002;
      public const uint Ant = 0x00000004;
      public const uint ActivityUpload = 0x00000008;
      public const uint CourseDownload = 0x00000010;
      public const uint WorkoutDownload = 0x00000020;
      public const uint LiveTrack = 0x00000040;
      public const uint WeatherConditions = 0x00000080;
      public const uint WeatherAlerts = 0x00000100;
      public const uint GpsEphemerisDownload = 0x00000200;
      public const uint ExplicitArchive = 0x00000400;
      public const uint SetupIncomplete = 0x00000800;
      public const uint ContinueSyncAfterSoftwareUpdate = 0x00001000;
      public const uint ConnectIqAppDownload = 0x00002000;
      public const uint Invalid = (uint)0x00000000;   
      
   }
}


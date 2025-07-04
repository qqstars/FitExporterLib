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
   /// <summary>
   /// Implements the CoursePoint profile message.
   /// </summary>  
   public class CoursePointMesg : Mesg 
   {    
      #region Fields     
      #endregion

      #region Constructors                 
      public CoursePointMesg() : base(Profile.mesgs[Profile.CoursePointIndex])               
      {                 
      }
      
      public CoursePointMesg(Mesg mesg) : base(mesg)
      {
      }
      #endregion // Constructors

      #region Methods    
      ///<summary>      
      /// Retrieves the MessageIndex field</summary>
      /// <returns>Returns nullable ushort representing the MessageIndex field</returns>      
      public ushort? GetMessageIndex()   
      {                
         return (ushort?)GetFieldValue(254, 0, Fit.SubfieldIndexMainField);                     
      }

      /// <summary>        
      /// Set MessageIndex field</summary>
      /// <param name="messageIndex_">Nullable field value to be set</param>      
      public void SetMessageIndex(ushort? messageIndex_) 
      {  
         SetFieldValue(254, 0, messageIndex_, Fit.SubfieldIndexMainField);
      }
          
      ///<summary>      
      /// Retrieves the Timestamp field</summary>
      /// <returns>Returns DateTime representing the Timestamp field</returns>      
      public DateTime GetTimestamp()   
      {                
         return TimestampToDateTime((uint?)GetFieldValue(1, 0, Fit.SubfieldIndexMainField));                     
      }

      /// <summary>        
      /// Set Timestamp field</summary>
      /// <param name="timestamp_">Nullable field value to be set</param>      
      public void SetTimestamp(DateTime timestamp_) 
      {  
         SetFieldValue(1, 0, timestamp_.GetTimeStamp(), Fit.SubfieldIndexMainField);
      }
          
      ///<summary>      
      /// Retrieves the PositionLat field
      /// Units: semicircles</summary>
      /// <returns>Returns nullable int representing the PositionLat field</returns>      
      public int? GetPositionLat()   
      {                
         return (int?)GetFieldValue(2, 0, Fit.SubfieldIndexMainField);                     
      }

      /// <summary>        
      /// Set PositionLat field
      /// Units: semicircles</summary>
      /// <param name="positionLat_">Nullable field value to be set</param>      
      public void SetPositionLat(int? positionLat_) 
      {  
         SetFieldValue(2, 0, positionLat_, Fit.SubfieldIndexMainField);
      }
          
      ///<summary>      
      /// Retrieves the PositionLong field
      /// Units: semicircles</summary>
      /// <returns>Returns nullable int representing the PositionLong field</returns>      
      public int? GetPositionLong()   
      {                
         return (int?)GetFieldValue(3, 0, Fit.SubfieldIndexMainField);                     
      }

      /// <summary>        
      /// Set PositionLong field
      /// Units: semicircles</summary>
      /// <param name="positionLong_">Nullable field value to be set</param>      
      public void SetPositionLong(int? positionLong_) 
      {  
         SetFieldValue(3, 0, positionLong_, Fit.SubfieldIndexMainField);
      }
          
      ///<summary>      
      /// Retrieves the Distance field
      /// Units: m</summary>
      /// <returns>Returns nullable float representing the Distance field</returns>      
      public float? GetDistance()   
      {                
         return (float?)GetFieldValue(4, 0, Fit.SubfieldIndexMainField);                     
      }

      /// <summary>        
      /// Set Distance field
      /// Units: m</summary>
      /// <param name="distance_">Nullable field value to be set</param>      
      public void SetDistance(float? distance_) 
      {  
         SetFieldValue(4, 0, distance_, Fit.SubfieldIndexMainField);
      }
          
      ///<summary>      
      /// Retrieves the Type field</summary>
      /// <returns>Returns nullable CoursePoint enum representing the Type field</returns>      
      new public CoursePoint? GetType()   
      { 
         object obj = GetFieldValue(5, 0, Fit.SubfieldIndexMainField);
         CoursePoint? value = obj == null ? (CoursePoint?)null : (CoursePoint)obj;
         return value;                     
      }

      /// <summary>        
      /// Set Type field</summary>
      /// <param name="type_">Nullable field value to be set</param>      
      public void SetType(CoursePoint? type_) 
      {  
         SetFieldValue(5, 0, type_, Fit.SubfieldIndexMainField);
      }
          
      ///<summary>      
      /// Retrieves the Name field</summary>
      /// <returns>Returns byte[] representing the Name field</returns>      
      public byte[] GetName()   
      {                
         return (byte[])GetFieldValue(6, 0, Fit.SubfieldIndexMainField);                     
      }

      /// <summary>        
      /// Set Name field</summary>
      /// <param name="name_">field value to be set</param>      
      public void SetName(byte[] name_) 
      {  
         SetFieldValue(6, 0, name_, Fit.SubfieldIndexMainField);
      }
                        
      #endregion // Methods
   } // Class
} // namespace

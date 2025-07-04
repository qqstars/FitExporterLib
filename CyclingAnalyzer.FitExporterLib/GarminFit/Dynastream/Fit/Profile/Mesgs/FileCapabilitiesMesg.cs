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
   /// Implements the FileCapabilities profile message.
   /// </summary>  
   public class FileCapabilitiesMesg : Mesg 
   {    
      #region Fields     
      #endregion

      #region Constructors                 
      public FileCapabilitiesMesg() : base(Profile.mesgs[Profile.FileCapabilitiesIndex])               
      {                 
      }
      
      public FileCapabilitiesMesg(Mesg mesg) : base(mesg)
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
      /// Retrieves the Type field</summary>
      /// <returns>Returns nullable File enum representing the Type field</returns>      
      new public File? GetType()   
      { 
         object obj = GetFieldValue(0, 0, Fit.SubfieldIndexMainField);
         File? value = obj == null ? (File?)null : (File)obj;
         return value;                     
      }

      /// <summary>        
      /// Set Type field</summary>
      /// <param name="type_">Nullable field value to be set</param>      
      public void SetType(File? type_) 
      {  
         SetFieldValue(0, 0, type_, Fit.SubfieldIndexMainField);
      }
          
      ///<summary>      
      /// Retrieves the Flags field</summary>
      /// <returns>Returns nullable byte representing the Flags field</returns>      
      public byte? GetFlags()   
      {                
         return (byte?)GetFieldValue(1, 0, Fit.SubfieldIndexMainField);                     
      }

      /// <summary>        
      /// Set Flags field</summary>
      /// <param name="flags_">Nullable field value to be set</param>      
      public void SetFlags(byte? flags_) 
      {  
         SetFieldValue(1, 0, flags_, Fit.SubfieldIndexMainField);
      }
          
      ///<summary>      
      /// Retrieves the Directory field</summary>
      /// <returns>Returns byte[] representing the Directory field</returns>      
      public byte[] GetDirectory()   
      {                
         return (byte[])GetFieldValue(2, 0, Fit.SubfieldIndexMainField);                     
      }

      /// <summary>        
      /// Set Directory field</summary>
      /// <param name="directory_">field value to be set</param>      
      public void SetDirectory(byte[] directory_) 
      {  
         SetFieldValue(2, 0, directory_, Fit.SubfieldIndexMainField);
      }
          
      ///<summary>      
      /// Retrieves the MaxCount field</summary>
      /// <returns>Returns nullable ushort representing the MaxCount field</returns>      
      public ushort? GetMaxCount()   
      {                
         return (ushort?)GetFieldValue(3, 0, Fit.SubfieldIndexMainField);                     
      }

      /// <summary>        
      /// Set MaxCount field</summary>
      /// <param name="maxCount_">Nullable field value to be set</param>      
      public void SetMaxCount(ushort? maxCount_) 
      {  
         SetFieldValue(3, 0, maxCount_, Fit.SubfieldIndexMainField);
      }
          
      ///<summary>      
      /// Retrieves the MaxSize field
      /// Units: bytes</summary>
      /// <returns>Returns nullable uint representing the MaxSize field</returns>      
      public uint? GetMaxSize()   
      {                
         return (uint?)GetFieldValue(4, 0, Fit.SubfieldIndexMainField);                     
      }

      /// <summary>        
      /// Set MaxSize field
      /// Units: bytes</summary>
      /// <param name="maxSize_">Nullable field value to be set</param>      
      public void SetMaxSize(uint? maxSize_) 
      {  
         SetFieldValue(4, 0, maxSize_, Fit.SubfieldIndexMainField);
      }
                        
      #endregion // Methods
   } // Class
} // namespace

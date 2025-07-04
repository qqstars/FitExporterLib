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
   /// The Subfield class represents an alternative field definition used
   /// by dynamic fields.  They can only be associated with a containing 
   /// field object.  
   /// </summary>  
   public class Subfield
   {
      #region Internal Classes
      /// <summary>
      /// The SubfieldMap class tracks the reference field/value pairs which indicate a field
      /// should use the alternate subfield definition rather than the usual defn (allows Dynamic Fields)     
      /// </summary>
      private class SubfieldMap
      {        
         private byte refFieldNum;                  
         private object refFieldValue;

         internal SubfieldMap(byte refFieldNum, object refFieldValue)
         {
            this.refFieldNum = refFieldNum;
            this.refFieldValue = refFieldValue;
         }

         internal SubfieldMap(SubfieldMap subfieldMap)
         {
            this.refFieldNum = subfieldMap.refFieldNum;
            this.refFieldValue = subfieldMap.refFieldValue;
         }

         /// <summary>
         /// Checks if the reference fields in a given message indicate the subfield (alternate)
         /// definition should be used
         /// </summary>
         /// <param name="mesg">message of interest</param>
         /// <returns>true if the subfield is active</returns>                  
         internal bool CanMesgSupport(Mesg mesg) 
         {
            Field field = mesg.GetField(refFieldNum);

            if (field != null) 
            {
               object value = field.GetValue(0, Fit.SubfieldIndexMainField);
               // Float refvalues are not supported
               if (Convert.ToInt64(value) == Convert.ToInt64(refFieldValue))
               {                
                  return true;                
               }            
            }  
            return false;
         }
      }
      #endregion Internal Classes

      #region Fields      
      private string name;
      private byte type;
      private float scale;
      private float offset;
      private string units;
      private List<SubfieldMap> maps;
      private List<FieldComponent> components;
      #endregion // Fields

      #region Properties      
      internal string Name 
      { 
         get
         {
            return name; 
         }
      }

      internal byte Type 
      { 
         get
         {
            return type;         
         }
      }

      internal float Scale
      { 
         get
         {
            return scale;         
         }
      }

      internal float Offset 
      { 
         get
         {
            return offset;         
         }
      }

      internal string Units 
      {
         get
         {
            return units;
         }
      }

      internal List<FieldComponent> Components
      {
         get
         {
            return components;
         }
      }
      #endregion // Properties

      #region Constructors
      internal Subfield(Subfield subfield)
      {
         if (subfield == null)   
         {
            this.name = "unknown";
            this.type = 0;
            this.scale = 1f;
            this.offset = 0f;
            this.units = "";
            this.maps = new List<SubfieldMap>();
            this.components = new List<FieldComponent>();
            return;
         }

         this.name = subfield.name;
         this.type = subfield.type;
         this.scale = subfield.scale;
         this.offset = subfield.offset;
         this.units = subfield.units;
         
         this.maps = new List<SubfieldMap>();
         foreach (SubfieldMap map in subfield.maps)
         {
            this.maps.Add(new SubfieldMap(map));
         }
         this.components = new List<FieldComponent>();
         foreach (FieldComponent comp in subfield.components)
         {
            this.components.Add(new FieldComponent(comp));
         }         
      }

      internal Subfield(string name, byte type, float scale, float offset, string units) 
      {
         this.name = name;
         this.type = type;
         this.scale = scale;
         this.offset = offset;
         this.units = units;
         this.maps = new List<SubfieldMap>();
         this.components = new List<FieldComponent>();
      }
      #endregion // Constructors

      #region Methods
      internal void AddMap(byte refFieldNum, object refFieldValue) 
      {
         maps.Add(new SubfieldMap(refFieldNum, refFieldValue));
      }

      internal void AddComponent(FieldComponent newComponent)
      {
         components.Add(newComponent);
      }

      /// <summary>
      /// Checks if the reference fields in a given message indicate the subfield (alternate)
      /// definition should be used
      /// </summary>
      /// <param name="mesg">message of interest</param>
      /// <returns>true if the subfield is active</returns>
      public bool CanMesgSupport(Mesg mesg) 
      {
         foreach (SubfieldMap map in maps) 
         {
            if (map.CanMesgSupport(mesg)) 
            {
               return true;
            }
         }
         return false;
      }
      #endregion // Methods
   } // Class
} // namespace

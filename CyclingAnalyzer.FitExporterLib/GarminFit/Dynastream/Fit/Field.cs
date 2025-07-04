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
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Dynastream.Fit
{   
   public class Field
   {      
      #region Fields
      private string name;
      private byte num;
      private byte type;
      private float scale;
      private float offset;
      private string units;

      internal List<object> values  = new List<object>();      
      internal List<Subfield> subfields = new List<Subfield>();
      internal List<FieldComponent> components = new List<FieldComponent>();
      #endregion

      #region Properties      
      public string Name
      { 
         get
         {
            return name;
         }
      }

      public byte Num
      { 
         get
         {
            return num;
         }
         set
         {
            num = value;
         }
      }      

      public byte Type
      { 
         get
         {
            return type;
         }
         set
         {
            type = value;
         }
      }      

      public float Scale
      { 
         get
         {
            return scale;
         }
      }

      public float Offset
      { 
         get
         {
            return offset;
         }
      }      
      public string Units
      { 
         get
         {
            return units;
         }
      }      
      #endregion

      #region Constructors
      public Field(Field field)
      {
         if (field == null)
         {
            this.name = "unknown";
            this.num = Fit.FieldNumInvalid;            
            this.type = 0;
            this.scale = 1f;
            this.offset = 0f;
            this.units = "";            
            return;
         }

         this.name = field.Name;
         this.num = field.Num;
         this.type = field.Type;
         this.scale = field.Scale;
         this.offset = field.Offset;
         this.units = field.units;
         
         foreach (object obj in field.values)
         {            
            this.values.Add(obj);
         }              
         foreach (Subfield subfield in field.subfields)
         {
            this.subfields.Add(new Subfield(subfield));
         }
         foreach (FieldComponent component in field.components)
         {
            this.components.Add(new FieldComponent(component));
         }         
      }

      internal Field(string name, byte num, byte type, float scale, float offset, string units)
      {
         this.name = name;
         this.num = num;
         this.type = type;
         this.scale = scale;
         this.offset = offset;
         this.units = units;         
      }
      #endregion

      #region Methods
      public string GetName()
      {
         return GetName((Subfield)null);
      }

      public string GetName(byte subfieldIndex)
      {
         return GetName(GetSubfield(subfieldIndex));       
      }

      public string GetName(string subFieldName)
      {
         return GetName(GetSubfield(subFieldName));
      }

      private string GetName(Subfield subfield)
      {
         if (subfield == null)
         {
            return name;
         }
         else
         {
            return subfield.Name;
         }
      }

      public byte GetNum()
      {
         return num;
      }

      new public byte GetType()
      {
         return GetType((Subfield)null);
      }

      public byte GetType(byte subfieldIndex)
      {
         return GetType(GetSubfield(subfieldIndex));       
      }

      public byte GetType(string subFieldName)
      {
         return GetType(GetSubfield(subFieldName));
      }

      private byte GetType(Subfield subfield)
      {
         if (subfield == null)
         {
            return type;
         }
         else
         {
            return subfield.Type;
         }
      }

      public string GetUnits()
      {
         return GetUnits((Subfield)null);
      }

      public string GetUnits(byte subfieldIndex)
      {
         return GetUnits(GetSubfield(subfieldIndex));       
      }

      public string GetUnits(string subFieldName)
      {
         return GetUnits(GetSubfield(subFieldName));
      }

      private string GetUnits(Subfield subfield)
      {
         if (subfield == null)
         {
            return units;
         }
         else
         {
            return subfield.Units;
         }
      }

      public byte GetSize()
      {
         byte size = 0;

         switch (Type & Fit.BaseTypeNumMask)
         {
            case Fit.Enum:
            case Fit.SInt8:
            case Fit.UInt8:
            case Fit.SInt16:
            case Fit.UInt16:
            case Fit.SInt32:
            case Fit.UInt32:
            case Fit.Float32:
            case Fit.Float64:
            case Fit.UInt8z:
            case Fit.UInt16z:
            case Fit.UInt32z:
            case Fit.Byte:
               size = (byte)(GetNumValues() * Fit.BaseType[Type & Fit.BaseTypeNumMask].size);
               break;

            case Fit.String:                
               // Each string may be of differing length
               // The fit binary must also include a null terminator
               foreach (byte[] element in values)
               {                                    
                  size += (byte)(element.Length + 1);
               }               
               break;

            default:
               break;
         }
         return size;
      }

      internal Subfield GetSubfield(string subfieldName)
      {
         foreach (Subfield subfield in subfields)
         {
            if (subfield.Name == subfieldName)
            {
               return subfield;
            }
         }
         return null;
      }

      internal Subfield GetSubfield(int subfieldIndex)
      {
         // SubfieldIndexActiveSubfield and SubfieldIndexMainField
         // will be out of this range
         if (subfieldIndex >=0 && subfieldIndex < subfields.Count)
         {            
            return subfields[subfieldIndex];            
         }
         return null;
      }

      internal bool IsSigned()
      {
         return IsSigned((Subfield)null);
      }

      internal bool IsSigned(int subfieldIndex)
      {
         return IsSigned(GetSubfield(subfieldIndex));         
      }

      internal bool IsSigned(string subfieldName)
      {
         return IsSigned(GetSubfield(subfieldName));     
      }

      internal bool IsSigned(Subfield subfield)
      {
         if (subfield == null)
         {
            return Fit.BaseType[Type & Fit.BaseTypeNumMask].isSigned;         
         }
         else
         {
            return Fit.BaseType[subfield.Type & Fit.BaseTypeNumMask].isSigned;         
         }
      }

       public void AddValue(Object value) 
       {          
          values.Add(value);            
       }

      public int GetNumValues()
      {
         return this.values.Count;
      }

      public long? GetBitsValue(int offset, int bits, byte componentType)
      {
         long? value = 0;
         long data = 0;
         long mask;
         int index = 0;
         int bitsInValue = 0;
         int bitsInData;
         
         // Ensure the destination type can hold the desired number of bits.
         // We don't support arrays in the destination at this time.
         if ((Fit.BaseType[componentType & Fit.BaseTypeNumMask].size * 8) < bits)
         {
            bits = Fit.BaseType[componentType & Fit.BaseTypeNumMask].size * 8;
         }

         while (bitsInValue < bits)
         {
            try
            {
               data = Convert.ToInt64(this.values[index++]);
            }
            catch (ArgumentOutOfRangeException)
            {
               // If we run out of bits it likely is because our profile is newer and defines 
               // additional components not present in the field
               return null;               
            }            

            // Shift data to reach desired bits starting at 'offset'
            // If offset is larger than the containing types size, 
            // we must grab additional elements
            data >>= offset;
            bitsInData = Fit.BaseType[Type & Fit.BaseTypeNumMask].size * 8 - offset;
            offset -= Fit.BaseType[Type & Fit.BaseTypeNumMask].size * 8;

            if (bitsInData > 0)
            {
               // We have reached desired data, pull off bits until we
               // get enough
               offset = 0;
               // If there are more bits available in data than we need
               // just capture those we need
               if (bitsInData > (bits - bitsInValue))
               {
                  bitsInData = bits - bitsInValue;
               }
               mask = (1L << bitsInData) - 1;
               value |= ((data & mask) << bitsInValue);
               bitsInValue += bitsInData;
            }
         }     
         // Sign extend if needed         
         if (Fit.BaseType[componentType & Fit.BaseTypeNumMask].isSigned == true && 
             Fit.BaseType[componentType & Fit.BaseTypeNumMask].isInteger == true)
         {
            long signBit = (1L << (bits - 1));

            if ((value & signBit) != 0)
            {
               value = -signBit + (value & (signBit - 1));
            }
         }        
         return value;
      }


      public object GetValue()
      {         
         return GetValue(0, (Subfield)null);
      }

      public object GetValue(int index)
      {       
         return GetValue(index, (Subfield)null);
      }

      public object GetValue(int index, int subfieldIndex)
      {       
         return GetValue(index, GetSubfield(subfieldIndex));
      }

      public object GetValue(int index, string subfieldName)
      {       
         return GetValue(index, GetSubfield(subfieldName));
      }

      public object GetValue(int index, Subfield subfield)
      {  
         float scale, offset;
              
         if (index >= values.Count || index < 0)
         {
            return null;
         }

         if (subfield == null)
         {
            scale = this.Scale;
            offset = this.Offset; 
         }
         else 
         {
            scale = subfield.Scale;
            offset = subfield.Offset; 
         }
            
         object value = values[index];         
         if (IsNumeric())
         {                        
            if (scale != 1.0 || Offset != 0.0)
            {               
               value = Convert.ToSingle(value);
               value = (float)value / scale - offset;
            }
         }            
         return value;         
      }

      public void SetValue(object value)
      {                           
         SetValue(0, value, (Subfield)null);                 
      }      

      public void SetValue(object value, int subfieldIndex)
      {
         SetValue(0, value, GetSubfield(subfieldIndex));
      }

      public void SetValue(object value, string subfieldName)
      {
         SetValue(0, value, GetSubfield(subfieldName));
      }      

      public void SetValue(int index, object value)
      {
         SetValue(index, value, (Subfield)null);
      }
 
      public void SetValue(int index, object value, int subfieldIndex)
      {
         SetValue(index, value, GetSubfield(subfieldIndex));
      }

      public void SetValue(int index, object value, string subfieldName)
      {
         SetValue(index, value, GetSubfield(subfieldName));
      }      

      public void SetValue(int index, object value, Subfield subfield)
      {
         float scale, offset;

         while (index >= GetNumValues())
         {            
            // Add placeholders of the correct type so GetSize() will 
            // still compute correctly
            switch (Type & Fit.BaseTypeNumMask)
            {
               case Fit.Enum:
               case Fit.Byte:
               case Fit.UInt8:
               case Fit.UInt8z:
                  values.Add(new byte());
                  break;

               case Fit.SInt8:
                  values.Add(new sbyte());                  
                  break;

               case Fit.SInt16:
                  values.Add(new short());                  
                  break;

               case Fit.UInt16:
               case Fit.UInt16z:
                  values.Add(new ushort());                  
                  break;

               case Fit.SInt32:
                  values.Add(new int());                  
                  break;

               case Fit.UInt32:
               case Fit.UInt32z:
                  values.Add(new uint());                  
                  break;

               case Fit.Float32:
                  values.Add(new float());                  
                  break;

               case Fit.Float64:
                  values.Add(new double());                  
                  break;

               case Fit.String:            
                  values.Add(new byte[0]);                  
               break;

               default:                     
                  break;
            }                        
         }
         
         if (subfield == null)
         {
            scale = this.Scale;
            offset = this.Offset; 
         }
         else
         {
            scale = subfield.Scale;
            offset = this.Offset; 
         }         
         
         if (IsNumeric())
         {
            if (scale != 1.0 || Offset != 0.0)
            {
               value = Convert.ToSingle(value);        
               value = ((float)value + offset) * scale;                              
            }
         }         
         // Must convert value back to the base type, if there was a scale or offset it will
         // have been converted to float.  Caller also may have passed in an unexpected type.
         switch (Type & Fit.BaseTypeNumMask)
         {            
            case Fit.Enum:
            case Fit.Byte:
            case Fit.UInt8:
            case Fit.UInt8z:               
               value = Convert.ToByte(value);
               break;

            case Fit.SInt8:               
               value = Convert.ToSByte(value);
               break;

            case Fit.SInt16:               
               value = Convert.ToInt16(value);
               break;

            case Fit.UInt16:
            case Fit.UInt16z:               
               value = Convert.ToUInt16(value);
               break;

            case Fit.SInt32:               
               value = Convert.ToInt32(value);
               break;

            case Fit.UInt32:
            case Fit.UInt32z:            
               value = Convert.ToUInt32(value);
               break;

            case Fit.Float32:               
               value = Convert.ToSingle(value);
               break;

            case Fit.Float64:               
               value = Convert.ToDouble(value);
               break;            

            default:                     
               break;
         }
         values[index] = value; 
      }

      public void SetRawValue(int index, object value)
      {         
         while (index >= GetNumValues())
         {            
            // Add placeholders of the correct type so GetSize() will 
            // still compute correctly
            switch (Type & Fit.BaseTypeNumMask)
            {
               case Fit.Enum:
               case Fit.Byte:
               case Fit.UInt8:
               case Fit.UInt8z:
                  values.Add(new byte());
                  break;

               case Fit.SInt8:
                  values.Add(new sbyte());                  
                  break;

               case Fit.SInt16:
                  values.Add(new short());                  
                  break;

               case Fit.UInt16:
               case Fit.UInt16z:
                  values.Add(new ushort());                  
                  break;

               case Fit.SInt32:
                  values.Add(new int());                  
                  break;

               case Fit.UInt32:
               case Fit.UInt32z:
                  values.Add(new uint());                  
                  break;

               case Fit.Float32:
                  values.Add(new float());                  
                  break;

               case Fit.Float64:
                  values.Add(new double());                  
                  break;

               case Fit.String:            
                  values.Add(new byte[0]);                  
               break;

               default:                     
                  break;
            }                                    
         }                                             
         // Must convert value back to the base type, caller may have passed in an unexpected type.
         switch (Type & Fit.BaseTypeNumMask)
         {
            case Fit.Enum:
            case Fit.Byte:
            case Fit.UInt8:
            case Fit.UInt8z:
               value = Convert.ToByte(value);
               break;

            case Fit.SInt8:
               value = Convert.ToSByte(value);
               break;

            case Fit.SInt16:
               value = Convert.ToInt16(value);
               break;

            case Fit.UInt16:
            case Fit.UInt16z:
               value = Convert.ToUInt16(value);
               break;

            case Fit.SInt32:
               value = Convert.ToInt32(value);
               break;

            case Fit.UInt32:
            case Fit.UInt32z:
               value = Convert.ToUInt32(value);
               break;

            case Fit.Float32:
               value = Convert.ToSingle(value);
               break;

            case Fit.Float64:
               value = Convert.ToDouble(value);
               break;            

            default:                     
               break;

         }
         values[index] = value; 
      }

      public object GetRawValue(int index)
      {                         
         if (index >= values.Count || index < 0)
         {
            return null;
         }                     
         object value = values[index];                  
         return value;         
      }
      
      public bool IsNumeric()
      {         
         bool isNumeric;
         switch (this.Type & Fit.BaseTypeNumMask)
         {
            case Fit.Enum:
            case Fit.String:
               isNumeric = false;
               break;
            
            case Fit.SInt8:            
            case Fit.UInt8:
            case Fit.SInt16:
            case Fit.UInt16:
            case Fit.SInt32:
            case Fit.UInt32:
            case Fit.Float32:
            case Fit.Float64:
            case Fit.UInt8z:
            case Fit.UInt16z:
            case Fit.UInt32z:
            case Fit.Byte:
               isNumeric = true;
               break;            

            default:
               throw new FitException("Field:IsNumeric - Unexpected Fit Type" + this.Type);               
             
         }
         return isNumeric;
      }
      #endregion
   }
} // namespace

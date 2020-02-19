using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Molinos.DataAgro.Entities.Validations
{
    public class EntityValid
    {
        public static bool ValidateAll(object oObject, Resultado oErrorMessages)
        {
            bool flag = true;
            Type[] types = new Type[1]
            {
                oErrorMessages.GetType()
            };
            MethodInfo method1 = oObject.GetType().GetMethod("ValidateKey", BindingFlags.Instance | BindingFlags.Public, (Binder)null, CallingConventions.Any, types, (ParameterModifier[])null);
            if ((object)method1 != null)
            {
                object[] parameters = new object[1]
                {
          (object) oErrorMessages
                };
                method1.Invoke(RuntimeHelpers.GetObjectValue(oObject), parameters);
            }
            MethodInfo method2 = oObject.GetType().GetMethod("Validate", BindingFlags.Instance | BindingFlags.Public, (Binder)null, CallingConventions.Any, types, (ParameterModifier[])null);
            if ((object)method2 != null)
            {
                object[] parameters = new object[1]
                {
          (object) oErrorMessages
                };
                method2.Invoke(RuntimeHelpers.GetObjectValue(oObject), parameters);
            }
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
            PropertyInfo[] properties = oObject.GetType().GetProperties();
            int index1 = 0;
            while (index1 < properties.Length)
            {
                PropertyInfo oPropertyInfo = properties[index1];
                if (EntityValid.EsTipoColeccion(oPropertyInfo))
                    propertyInfoList.Add(oPropertyInfo);
                checked { ++index1; }
            }
            int num1 = 0;
            int num2 = checked(propertyInfoList.Count - 1);
            int index2 = num1;
            while (index2 <= num2)
            {
                //EntityValid.ConvertCollection(RuntimeHelpers.GetObjectValue(propertyInfoList[index2].GetValue(RuntimeHelpers.GetObjectValue(oObject), (object[])null)), oErrorMessages);
                checked { ++index2; }
            }
            if (oErrorMessages.HayErrores)
                flag = false;
            return flag;
        }

        private static bool EsTipoColeccion(PropertyInfo oPropertyInfo)
        {
            bool flag = false;
            if ((object)oPropertyInfo.PropertyType != null)
            {
                if (Operators.CompareString(oPropertyInfo.PropertyType.Name, "List`1", false) == 0)
                    flag = true;
                else if (Operators.CompareString(oPropertyInfo.PropertyType.Name, "MSBindingList`1", false) == 0)
                    flag = true;
                else if (Operators.CompareString(Strings.Right(oPropertyInfo.PropertyType.Name, 2), "[]", false) == 0 & Operators.CompareString(oPropertyInfo.PropertyType.Name, "Byte[]", false) != 0)
                    flag = true;
            }
            return flag;
        }

        //private static void ConvertCollection(object oColleccion, Resultado oErrorMessages)
        //{
        //    if (oColleccion == null)
        //        return;
        //    int integer = Conversions.ToInteger(oColleccion.GetType().GetProperty("Count").GetValue(RuntimeHelpers.GetObjectValue(oColleccion), (object[])null));
        //    if (integer <= 0)
        //        return;
        //    PropertyInfo property = oColleccion.GetType().GetProperty("Item");
        //    object[] index1 = new object[1] { (object)0 };
        //    PropertyInfo[] properties = RuntimeHelpers.GetObjectValue(property.GetValue(RuntimeHelpers.GetObjectValue(oColleccion), index1)).GetType().GetProperties();
        //    List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
        //    PropertyInfo[] propertyInfoArray = properties;
        //    int index2 = 0;
        //    while (index2 < propertyInfoArray.Length)
        //    {
        //        PropertyInfo oPropertyInfo = propertyInfoArray[index2];
        //        if (EntityValid.EsTipoColeccion(oPropertyInfo))
        //            propertyInfoList.Add(oPropertyInfo);
        //        checked { ++index2; }
        //    }
        //    int num1 = 0;
        //    int num2 = checked(integer - 1);
        //    int num3 = num1;
        //    while (num3 <= num2)
        //    {
        //        index1[0] = (object)num3;
        //        object objectValue = RuntimeHelpers.GetObjectValue(property.GetValue(RuntimeHelpers.GetObjectValue(oColleccion), index1));
        //        Type[] types = new Type[1]
        //        {
        //  oErrorMessages.GetType()
        //        };
        //        MethodInfo method = objectValue.GetType().GetMethod("Validate", BindingFlags.Instance | BindingFlags.Public, (Binder)null, CallingConventions.Any, types, (ParameterModifier[])null);
        //        if ((object)method != null)
        //        {
        //            int count = oErrorMessages.Errores.Count;
        //            object[] parameters = new object[1]
        //            {
        //    (object) oErrorMessages
        //            };
        //            method.Invoke(RuntimeHelpers.GetObjectValue(objectValue), parameters);
        //            int num4 = count;
        //            int num5 = checked(oErrorMessages.Errores.Count - 1);
        //            int index3 = num4;
        //            while (index3 <= num5)
        //            {
        //                oErrorMessages.Errores[index3].Item = num3;
        //                checked { ++index3; }
        //            }
        //        }
        //        int num6 = 0;
        //        int num7 = checked(propertyInfoList.Count - 1);
        //        int index4 = num6;
        //        while (index4 <= num7)
        //        {
        //            EntityValid.ConvertCollection(RuntimeHelpers.GetObjectValue(propertyInfoList[index4].GetValue(RuntimeHelpers.GetObjectValue(objectValue), (object[])null)), oErrorMessages);
        //            checked { ++index4; }
        //        }
        //        checked { ++num3; }
        //    }
        //}
    }
}

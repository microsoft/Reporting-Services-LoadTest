// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Reflection;

namespace RSAccessor
{
    /// <summary>
    /// Utility that convert proxy class to RS* test class and convert back by copying properties
    /// </summary>
    public class SoapStructureConvert
    {
        string m_assemblyName = string.Empty;
        string m_targetNamespace = string.Empty;
        public SoapStructureConvert(string assemblyName, string targetNamespace)
        {
            m_assemblyName = assemblyName;
            m_targetNamespace = targetNamespace;
        }

        /// <summary>
        /// If target type is not specified, it will guess possible target type according to type name
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <returns></returns>
        public object Convert(object sourceObject)
        {
            if (sourceObject == null)
            {
                return null;
            }

            Type targetType = GetPossibleTargetType(sourceObject.GetType());
            if (targetType == null)
            {
                // throw new TestException(TestResult.Failed, "Please specify which type sourceObject convert to.");
                return (null);
            }
            else
            {
                return Convert(sourceObject, targetType);
            }
        }

        /// <summary>
        /// Convert sourceOjbect to TTarget type by copying properties
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="sourceObject"></param>
        /// <returns></returns>
        public TTarget Convert<TTarget>(object sourceObject)
        {
            if (sourceObject == null)
            {
                return default(TTarget);
            }

            if (!(InScope<TTarget>() && InScope(sourceObject.GetType())))
            {
                throw new NotSupportedException(String.Format("Can't convert {0} to {1}.",
                    sourceObject.GetType().Name, typeof(TTarget).Name));
            }

            return (TTarget)Convert(sourceObject, typeof(TTarget));
        }

        /// <summary>
        /// Convert sourceObject.propertyName to TTarget type by copying properties
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="sourceObject"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public TTarget Convert<TTarget>(object sourceObject, string propertyName)
        {
            if (sourceObject == null)
            {
                throw new ArgumentNullException("sourceObject");
            }

            object actualObject = GetProperty(sourceObject, propertyName);
            return Convert<TTarget>(actualObject);
        }

        /// <summary>
        /// Convert sourceObject to targetType by copying properties
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public object Convert(object sourceObject, Type targetType)
        {
            if (sourceObject == null)
            {
                return null;
            }

            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            Type sourceType = sourceObject.GetType();
            if (sourceType == targetType)
            {
                return sourceObject;
            }

            if (targetType.IsEnum)
            {
                return ConvertEnum(sourceObject, targetType);
            }

            if (targetType == typeof(string))
            {
                return sourceObject.ToString();
            }

            if (targetType.IsArray)
            {
                Array sourceArray = sourceObject as Array;
                Type targetElementType = targetType.GetElementType();
                Array targetArray = Array.CreateInstance(targetElementType, sourceArray.Length);
                for (int i = 0; i < targetArray.Length; i++)
                {
                    targetArray.SetValue(Convert(sourceArray.GetValue(i), targetElementType), i);
                }

                return targetArray;
            }

            // We can assign a subclass instance to base class reference
            Type possibleTargetType = GetPossibleTargetType(sourceType);
            if (possibleTargetType != null && possibleTargetType.IsSubclassOf(targetType))
            {
                return Convert(sourceObject, possibleTargetType);
            }

            object targetObject = CreateTargetObject(targetType);
            foreach (PropertyInfo pi in targetType.GetProperties())
            {
                PropertyInfo sourcePi;
                if (HasProperty(sourceObject, pi.Name, out sourcePi))
                {
                    object targetValue = Convert(GetProperty(sourceObject, sourcePi), pi.PropertyType);
                    SetProperty(targetObject, pi, targetValue);
                }
                else
                {
                    SetProperty(targetObject, pi, null);
                }
            }
            
            return targetObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public object ConvertEnum(object sourceObject, Type targetType)
        {
            return Enum.Parse(targetType, sourceObject.ToString());
        }

        /// <summary>
        /// Try to get target type by guessing according to naming convention.
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns>RSSomeType to SomeType and SomeType to RSSomeType.
        /// Null if result type is found in proxy class assembly.</returns>
        private Type GetPossibleTargetType(Type sourceType)
        {
            Type possibleTarget = null;
            
            // SomeType to RSSomeType
            if (sourceType.Namespace.StartsWith(DefaultProductNamespace))
            {
                string targetTypeName = String.Format("{0}{1}, {2}",
                    DefaultTestNamespace,
                    sourceType.Name,
                    m_assemblyName);
                possibleTarget = Type.GetType(targetTypeName);
            }
            // RSSomeType to SomeType
            else if (sourceType.FullName.StartsWith(DefaultTestNamespace))
            {
                string nameSpace = m_targetNamespace;  //target namespace
                string targetTypeName = String.Format("{0}.{1}, {2}",
                    nameSpace,
                    sourceType.Name.Substring(2),
                    m_assemblyName);
                possibleTarget = Type.GetType(targetTypeName);
            }

            return possibleTarget;
        }

        private bool InScope<T>()
        {
            return InScope(typeof(T));
        }

        private bool InScope(Type type)
        {
            return type.Namespace.StartsWith(DefaultProductNamespace)
                || type.FullName.StartsWith(DefaultTestNamespace);
        }

        private object GetProperty(object obj, string propertyName)
        {
            PropertyInfo pi = GetPropertyInfo(obj, propertyName);
            return GetProperty(obj, pi);
        }

        /// <summary>
        /// Return property value of obj. Index is not considered here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        private object GetProperty(object obj, PropertyInfo pi)
        {
            return pi.GetValue(obj, null);
        }

        private void SetProperty(object obj, string propertyName, object value)
        {
            SetProperty(obj, GetPropertyInfo(obj, propertyName), value);
        }

        private void SetProperty(object obj, PropertyInfo pi, object value)
        {
            try
            {
                pi.SetValue(obj, value, null);
            }
            catch (TargetException)
            {
                throw new Exception(
                    String.Format("Can't assign {0} to {1}'s {2} property, whose type is {3}.",
                    value.GetType().FullName,
                    obj.GetType().FullName,
                    pi.Name,
                    pi.PropertyType.FullName));
                
            }
            catch (MethodAccessException)
            {
                throw new Exception("PropertyInfo is null");

                throw new Exception(
                    String.Format("Can't access to set_property {0} on {1}.",
                    pi.Name,
                    obj.GetType().FullName));
            }
            catch (TargetInvocationException)
            {
                throw new Exception(
                    String.Format("Error when setting {0} to {1}'s property {2}",
                    value.GetType().FullName,
                    obj.GetType().FullName,
                    pi.Name));
            }
        }

        /// <summary>
        /// Get PropertyInfo from obj's type
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns>not null</returns>
        private PropertyInfo GetPropertyInfo(object obj, string propertyName)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName);
            if (pi == null)
            {
                throw new Exception(string.Format("PropertyInfo for {0} is null", propertyName));
            }
            return pi; 
        }

        /// <summary>
        /// Whether obj contains property named propertyName
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        private bool HasProperty(object obj, string propertyName, out PropertyInfo pi)
        {
            pi = obj.GetType().GetProperty(propertyName);
            return pi != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <returns></returns>
        private TTarget CreateTargetObject<TTarget>()
        {
            TTarget targetObject;
            try
            {
                targetObject = Activator.CreateInstance<TTarget>();
            }
            catch (MissingMethodException e)
            {
                throw new InvalidOperationException(String.Format("Target type {0} doesn't contains default public constructor.", typeof(TTarget).FullName), e);
            }

            return targetObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private object CreateTargetObject(Type targetType)
        {
            object targetObject;
            try
            {
                targetObject = Activator.CreateInstance(targetType);
            }
            catch (MissingMethodException e)
            {
                throw new InvalidOperationException(String.Format("Target type {0} doesn't contains default public constructor.", targetType.FullName), e);
            }

            return targetObject;
        }

        private const string DefaultProductNamespace = "Microsoft.SqlServer.ReportingServices";
        private const string DefaultTestNamespace = "RSAccessor.SoapAccessor.RS"; // Is really hack, namespace is really namespace (.RS is for making the type).
    }
}

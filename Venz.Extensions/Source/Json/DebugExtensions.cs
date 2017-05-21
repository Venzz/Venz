using System;
using System.Collections;
using System.Reflection;
using Windows.Data.Json;

namespace Venz.Extensions
{
    public static class DebugExtensions
    {
        public static IJsonValue AsJsonObject(this Object obj)
        {
            if (obj == null)
            {
                return JsonValue.CreateStringValue("null");
            }

            var typeInfo = obj.GetType().GetTypeInfo();
            if (obj is IJsonConvertible)
            {
                return ((IJsonConvertible)obj).ToJsonValue();
            }
            else if ((obj is String) || typeInfo.IsEnum || typeInfo.IsPrimitive || typeInfo.IsValueType)
            {
                return JsonValue.CreateStringValue(obj.ToString());
            }
            else if (obj is Array)
            {
                var array = (Array)obj;
                var jsonArray = new JsonArray();
                for (var i = 0; i < array.Length; i++)
                    jsonArray.Add(array.GetValue(i).AsJsonObject());
                return jsonArray;
            }
            else if (obj is IEnumerable)
            {
                var jsonArray = new JsonArray();
                foreach (var item in (IEnumerable)obj)
                    jsonArray.Add(item.AsJsonObject());
                return jsonArray;
            }
            else if (typeInfo.IsClass)
            {
                var value = new JsonObject();
                foreach (var property in obj.GetType().GetRuntimeProperties())
                {
                    var propertyValue = property.GetValue(obj, null);
                    value.Add(property.Name, propertyValue.AsJsonObject());
                }
                return value;
            }
            else
            {
                return JsonValue.CreateStringValue("Not implemented");
            }
        }

        public static String AsJsonString(this Object obj)
        {
            try
            {
                return (obj == null) ? "NULL" : obj.AsJsonObject().Stringify();
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public static String AsDebugMessage(this Exception exception)
        {
            if (exception == null)
                return "";

            var message = $"{exception.GetType().FullName}: {exception.Message}\n{exception.StackTrace}\n";
            message += CreateInnerExceptionMessage(exception.InnerException);
            return message;
        }

        private static String CreateInnerExceptionMessage(Exception exception)
        {
            if (exception == null)
                return "";

            var message = $"Caused by {exception.GetType().FullName}: {exception.Message}\n{exception.StackTrace}\n";
            message += CreateInnerExceptionMessage(exception.InnerException);
            return message;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Newtonsoft.Json.Linq;

namespace PsJson
{
    public class JsonObjectConverter : PSTypeConverter
    {
        public override bool CanConvertFrom(object sourceValue, Type destinationType)
        {
            return destinationType == typeof(JsonObject) &&
                   IsString(sourceValue);
        }

        public override bool CanConvertTo(object sourceValue, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(object sourceValue, Type destinationType, IFormatProvider formatProvider, bool ignoreCase)
        {
            if (IsString(sourceValue) && destinationType == typeof(JsonObject))
            {
                string json = MakeString(sourceValue);
                return new JsonObject(JObject.Parse(json));
            }
            throw new NotSupportedException();
        }

        public override object ConvertTo(object sourceValue, Type destinationType, IFormatProvider formatProvider, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        private string MakeString(object sourceValue)
        {
            if (sourceValue.GetType() == typeof(string))
            {
                return (string)sourceValue;
            }
            else if (sourceValue.GetType() == typeof(PSObject))
            {
                PSObject psobj = (PSObject)sourceValue;
                if (psobj.BaseObject.GetType() == typeof(String))
                {
                    return (string)psobj.BaseObject;
                }
            }
            else if (sourceValue.GetType().IsArray)
            {
                object[] vals = (object[])sourceValue;
                return String.Join(Environment.NewLine, vals.Select(o => o.ToString()).ToArray());
            }
            throw new NotSupportedException();
        }

        private bool IsString(object sourceValue)
        {
            if (sourceValue.GetType() == typeof(string))
            {
                return true;
            }
            else if (sourceValue.GetType() == typeof(PSObject))
            {
                PSObject psobj = (PSObject)sourceValue;
                return psobj.BaseObject.GetType() == typeof(String);
            }
            else if (sourceValue.GetType().IsArray)
            {
                object[] vals = (object[])sourceValue;
                return vals.All(IsString);
            }
            return false;
        }
    }
}

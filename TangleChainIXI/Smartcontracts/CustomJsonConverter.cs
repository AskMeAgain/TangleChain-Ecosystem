﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TangleChainIXI.Classes;
using TangleChainIXI.Smartcontracts.Classes;

namespace TangleChainIXI.Smartcontracts
{
    public class CustomJsonConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            var ss = reader.Value.ToString();

            ;
            var list = new List<Expression>();

            foreach (var exp in ss.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var arr = exp.Split('.');

                list.Add(new Expression(int.Parse(arr[0]), arr[1], arr[2], arr[3]));

            }

            ;
            return list;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            var list = (List<Expression>)value;
            writer.WriteValue(list.ToFlatList());
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<Expression>));

        }
    }
}

﻿using Newtonsoft.Json;
using System;
using System.Text;

namespace ORMMap.Model.Entitites
{
    public class Vector3<T> where T : struct
    {
        public T X { get; set; }
        public T Y { get; set; }
        public T Z { get; set; }

        public Vector3(T x, T y, T z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public string EncodeToString()
        {
            return Convert.ToBase64String(ToJsonBytes());
        }

        public static Vector3<T> DecodeFromString(string str)
        {
            string str1 = Encoding.ASCII.GetString(Convert.FromBase64String(str));
            return JsonConvert.DeserializeObject<Vector3<T>>(str1);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        private byte[] ToJsonBytes()
        {
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(this));
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}";
        }
    }
}

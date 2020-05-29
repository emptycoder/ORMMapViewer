using Newtonsoft.Json;
using System;
using System.Text;

namespace ORMMap.Model.Entitites
{
    public struct Vector3<T> where T : struct
    {
        public readonly T X;
        public readonly T Y;
        public readonly T Z;
        public Vector3(T x, T y, T z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public string EncodeToString()
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(this)));
        }

        public static Vector3<T> DecodeFromString(string str)
        {
            return (Vector3<T>)JsonConvert.DeserializeObject(Encoding.ASCII.GetString(Convert.FromBase64String(str)));
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}";
        }
    }
}

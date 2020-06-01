using System;
using System.ComponentModel;
using System.Reflection;

namespace ORMMap.VectorTile.ExtensionMethods
{
	/// <summary>
	///     Extension method to extract the [Description] attribute from an Enum
	/// </summary>
	public static class EnumExtensions
	{
		public static string Description(this Enum value)
		{
			Type enumType = value.GetType();
			FieldInfo field = enumType.GetField(value.ToString());
			object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attributes.Length == 0 ? value.ToString() : ((DescriptionAttribute) attributes[0]).Description;
		}
	}
}

using System.Reflection;
using System.Text;

namespace MonamourWeb.Models
{
    public static class Helper
    {
        private static PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties();
        }

        public static string GetObjectDescription(object obj)
        {
            var properties = GetProperties(obj);
            
            var stringBuilder = new StringBuilder();

            foreach (var p in properties)
            {
                var name = p.Name;
                var value = p.GetValue(obj, null);
                stringBuilder.Append($"[{name}]: {value}; ");
            }

            return stringBuilder.ToString();
        }

        public static string BoolToStringConverter(bool value) => value ? "Да" : "Нет";
    }
}
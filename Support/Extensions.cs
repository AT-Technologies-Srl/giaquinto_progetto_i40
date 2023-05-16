using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG40.Support
{
    public static class DateTimeExt
    {
        public static string Format(this DateTime date, string separator = "/")
        {
            if (new DateTime(date.Year, date.Month, date.Day) == DateTime.MinValue)
                return "";
            else
                return date.ToString($"dd{separator}MM{separator}yyyy");
        }

        public static string FormatWithTime(this DateTime date)
        {
            if (new DateTime(date.Year, date.Month, date.Day) == DateTime.MinValue)
                return "";
            else
                return date.ToString("dd/MM/yyyy HH:mm");
        }
    }

    public static class StringExt
    {
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value?.Trim());
        }

        public static bool Contains(this string value, string contains)
        {
            return !value.IsEmpty() && value.ToLower().Contains(contains.ToLower());
        }

        public static bool EqualsIgnoreCase(this string value, string compare)
        {
            string str1 = value != null ? value.Trim() : "";
            string str2 = compare != null ? compare.Trim() : "";
            return str1.Equals(str2, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public static class ListExt
    {
        public static bool IsEmpty<T>(this List<T> list)
        {
            return list.Count == 0;
        }
    }

    public static class DoubleExt
    {
        public const double EPSILON = 0.0001;

        public static bool IsEqual(this double x, double y, double epsilon = EPSILON)
        {
            if (x == y) return true;
            return Math.Abs(x - y) < epsilon;
        }

        public static int CompareTo(this double x, double y, double epsilon = EPSILON)
        {
            return IsEqual(x, y, epsilon) ? 0 : (x < y) ? -1 : +1;
        }

        public static bool GreaterThen(this double x, double y, double epsilon = EPSILON)
        {
            return x.CompareTo(y, epsilon) > 0;
        }
        public static bool GreaterThenEqual(this double x, double y, double epsilon = EPSILON)
        {
            return x.CompareTo(y, epsilon) >= 0;
        }

        public static bool LowerThen(this double x, double y, double epsilon = EPSILON)
        {
            return x.CompareTo(y, epsilon) < 0;
        }

        public static bool LowerThenEqual(this double x, double y, double epsilon = EPSILON)
        {
            return x.CompareTo(y, epsilon) <= 0;
        }        
    }
}

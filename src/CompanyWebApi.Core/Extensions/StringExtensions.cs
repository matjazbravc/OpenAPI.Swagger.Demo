using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CompanyWebApi.Core.Extensions
{
    /// <summary>
    /// Extension class for <see cref="string" />.
    /// </summary>
    [DebuggerStepThrough]
    [ExcludeFromCodeCoverage]
    public static class StringExtensions
    {
        /// <summary>
        /// Contains with StringComparison
        /// </summary>
        /// <param name="value">Value to compare.</param>
        /// <param name="comparer">Comparing value.</param>
        /// <param name="comparison">StringComparison</param>
        /// <returns>
        ///     Returns <c>True</c>, if the string value contains the comparer, otherwise returns
        ///     <c>False</c>.
        /// </returns>
        public static bool Contains(this string value, string comparer, StringComparison comparison)
        {
            return value != null && comparer != null && value.IndexOf(comparer, comparison) >= 0;
        }

        /// <summary>
        ///     Checks whether the string value ends with the comparer, regardless of casing.
        /// </summary>
        /// <param name="value">Value to compare.</param>
        /// <param name="comparer">Comparing value.</param>
        /// <returns>
        ///     Returns <c>True</c>, if the string value ends with the comparer, regardless of casing; otherwise returns
        ///     <c>False</c>.
        /// </returns>
        public static bool EndsWithEquivalent(this string value, string comparer)
        {
            value.ThrowIfNullOrWhiteSpace();
            return !comparer.IsNullOrWhiteSpace() && value.EndsWith(comparer, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Returns the size of the given <paramref name="input"/> encoded 
        /// as <c>UTF-16</c> characters in bytes.
        /// </summary>
        /// <param name="input">The string</param>
        public static int GetSize(this string input) => input.Length * sizeof(char);

        /// <summary>
        ///     Checks whether the string value is equal to the comparer, regardless of casing.
        /// </summary>
        /// <param name="value">Value to compare.</param>
        /// <param name="comparer">Comparing value.</param>
        /// <returns>
        ///     Returns <c>True</c>, if the string value is equal to the comparer, regardless of casing; otherwise returns
        ///     <c>False</c>.
        /// </returns>
        public static bool IsEquivalentTo(this string value, string comparer)
        {
            return value.Equals(comparer, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// A nice way of calling the inverse of <see cref="string.IsNullOrEmpty(string)"/>
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>
        /// <see langword="true"/> if the format parameter is not null or an empty string (""); otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsNotNullOrEmpty(this string value) => !value.IsNullOrEmpty();

        /// <summary>
        /// A nice way of checking the inverse of (if a string is null, empty or whitespace) 
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>
        /// <see langword="true"/> if the format parameter is not null or an empty string (""); otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsNotNullOrEmptyOrWhiteSpace(this string value) => !value.IsNullOrEmptyOrWhiteSpace();

        /// <summary>
        /// A nicer way of calling <see cref="string.IsNullOrEmpty(string)"/>
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>
        /// <see langword="true"/> if the format parameter is null or an empty string (""); otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>
        /// A nice way of checking if a string is null, empty or whitespace 
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>
        /// <see langword="true"/> if the format parameter is null or an empty string (""); otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsNullOrEmptyOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        /// <summary>
        ///     Checks whether the string value is either <c>null</c> or white space.
        /// </summary>
        /// <param name="value"><see cref="string" /> value to check.</param>
        /// <returns>Returns <c>True</c>, if the string value is either <c>null</c> or white space; otherwise returns <c>False</c>.</returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// This method fulfills following requirements:
        ///  - returns empty string if start-index is out of range
        ///  - returns string from start to given length, if start-index is smaller than 0
        ///  - returns string from start-index to end of string, if length argument is beyond number of characters left
        ///  - returns appropriate string if arguments in valid range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns>string</returns>
        public static string SafeSubstring(this string value, int startIndex, int length)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            if (value.Length >= startIndex + length)
            {
                return value.Substring(startIndex, length);
            }
            return value.Length >= startIndex ? value.Substring(startIndex) : string.Empty;
        }

        /// <summary>
        ///     Checks whether the string value starts with the comparer, regardless of casing.
        /// </summary>
        /// <param name="value">Value to compare.</param>
        /// <param name="comparer">Comparing value.</param>
        /// <returns>
        ///     Returns <c>True</c>, if the string value starts with the comparer, regardless of casing; otherwise returns
        ///     <c>False</c>.
        /// </returns>
        public static bool StartsWithEquivalent(this string value, string comparer)
        {
            value.ThrowIfNullOrWhiteSpace();
            return !comparer.IsNullOrWhiteSpace() && value.StartsWith(comparer, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException" /> if the given value is <c>null</c> or white space.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///     Returns the original value, if the value is NOT <c>null</c>; otherwise throws an
        ///     <see cref="ArgumentNullException" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" /></exception>
        public static string ThrowIfNullOrWhiteSpace(this string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(value));
            }
            return value;
        }

        /// <summary>
        ///     Converts the string value to <see cref="bool" /> value.
        /// </summary>
        /// <param name="value">String value to convert.</param>
        /// <returns>Returns the <see cref="bool" /> value converted.</returns>
        public static bool ToBoolean(this string value)
        {
            return !value.IsNullOrWhiteSpace() && Convert.ToBoolean(value);
        }

        /// <summary>
        ///     Converts the string value to <see cref="int" /> value.
        /// </summary>
        /// <param name="value">String value to convert.</param>
        /// <returns>Returns the <see cref="int" /> value converted.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" /></exception>
        public static int ToInt32(this string value)
        {
            value.ThrowIfNullOrWhiteSpace();
            return Convert.ToInt32(value);
        }

        public static string TrimWhiteSpace(this string value)
        {
            return Regex.Replace(value, @"\s{2,}", " ").Trim();
        }

        /// <summary>
        /// Truncate string to max bytes length
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="maxBytesLength">Max bytes length</param>
        /// <returns></returns>
	    public static string Truncate(this string value, int maxBytesLength)
        {
            var result = value;
            var byteCount = Encoding.UTF8.GetByteCount(value);
            if (byteCount <= maxBytesLength)
            {
                return result;
            }
            var byteArray = Encoding.UTF8.GetBytes(value);
            result = Encoding.UTF8.GetString(byteArray, 0, maxBytesLength);
            return result;
        }

        /// <summary>
        ///     Decodes the string value.
        /// </summary>
        /// <param name="value">String value to decode.</param>
        /// <returns>Returns URL decoded value.</returns>
        public static string UrlDecode(this string value)
        {
            return value.IsNullOrWhiteSpace()
                       ? value
                       : WebUtility.UrlDecode(value.Replace("%20", "+"));
        }

        /// <summary>
        ///     Encodes the string value.
        /// </summary>
        /// <param name="value">String value to encode.</param>
        /// <returns>Returns URL encoded value.</returns>
        public static string UrlEncode(this string value)
        {
            return value.IsNullOrWhiteSpace()
                       ? value
                       : WebUtility.UrlEncode(value)?.Replace("+", "%20");
        }
    }
}

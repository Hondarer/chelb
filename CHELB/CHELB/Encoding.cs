using System;
using System.Text;

namespace CHELB
{
    /// <summary>
    /// Provides auxiliary functions related to encoding.
    /// </summary>
    public class Encoding
    {
        /// <summary>
        /// UTF-8 (without BOM)
        /// </summary>
        public static readonly System.Text.Encoding UTF8nEncoding = new UTF8Encoding(false);

        /// <summary>
        /// UTF-16LE (without BOM)
        /// </summary>
        public static readonly System.Text.Encoding UTF16LEnEncoding = new UnicodeEncoding(false,false);

        /// <summary>
        /// UTF-16BE (without BOM)
        /// </summary>
        public static readonly System.Text.Encoding UTF16BEnEncoding = new UnicodeEncoding(true,false);

        /// <summary>
        /// UTF-32LE (without BOM)
        /// </summary>
        public static readonly System.Text.Encoding UTF32LEnEncoding = new UTF32Encoding(false, false);

        /// <summary>
        /// UTF-32BE (without BOM)
        /// </summary>
        public static readonly System.Text.Encoding UTF32BEnEncoding = new UTF32Encoding(true, false);

        /// <summary>
        /// Determines the character code from the byte array.
        /// </summary>
        /// <param name="buffer">A byte array that determines the character code.</param>
        /// <returns>The character code expected from <paramref name="buffer"/>.</returns>
        public static System.Text.Encoding AutoDetect(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (buffer.Length == 0)
            {
                // Since character code can not be discriminated, it is assumed to be the representative encoding
                return System.Text.Encoding.ASCII;
            }

            if (TestEncoding(buffer, System.Text.Encoding.ASCII) == true)
            {
                return System.Text.Encoding.ASCII;
            }

            if (TestEncoding(buffer, System.Text.Encoding.GetEncoding("Shift_JIS")) == true)
            {
                return System.Text.Encoding.GetEncoding("Shift_JIS");
            }

            if (TestEncoding(buffer, System.Text.Encoding.GetEncoding("EUC-JP")) == true)
            {
                return System.Text.Encoding.GetEncoding("EUC-JP");
            }

            if (TestEncoding(buffer, System.Text.Encoding.GetEncoding("iso-2022-jp")) == true)
            {
                return System.Text.Encoding.GetEncoding("iso-2022-jp");
            }

            if (TestEncoding(buffer, System.Text.Encoding.UTF7))
            {
                return System.Text.Encoding.UTF7;
            }

            if (TestEncoding(buffer, System.Text.Encoding.UTF32))
            {
                return System.Text.Encoding.UTF32;
            }

            if (TestEncoding(buffer, System.Text.Encoding.GetEncoding("utf-32BE")))
            {
                return System.Text.Encoding.GetEncoding("utf-32BE");
            }

            if (TestEncoding(buffer, System.Text.Encoding.UTF8))
            {
                return System.Text.Encoding.UTF8;
            }

            if (TestEncoding(buffer, System.Text.Encoding.Unicode))
            {
                return System.Text.Encoding.Unicode;
            }

            if (TestEncoding(buffer, System.Text.Encoding.BigEndianUnicode))
            {
                return System.Text.Encoding.BigEndianUnicode;
            }

            return null;
        }

        public static System.Text.Encoding GetEncodingWithoutBOM(System.Text.Encoding encoding)
        {
            if (IsEncodingWithBOM(encoding) == false)
            {
                return encoding;
            }

            if (encoding.WebName == System.Text.Encoding.UTF8.WebName)
            {
                return UTF8nEncoding;
            }

            if (encoding.WebName == System.Text.Encoding.Unicode.WebName)
            {
                return UTF16LEnEncoding;
            }

            if (encoding.WebName == System.Text.Encoding.BigEndianUnicode.WebName)
            {
                return UTF16BEnEncoding;
            }

            if (encoding.WebName == System.Text.Encoding.UTF32.WebName)
            {
                return UTF32LEnEncoding;
            }

            if (encoding.WebName == System.Text.Encoding.GetEncoding("utf-32BE").WebName)
            {
                return UTF32BEnEncoding;
            }

            // Fallback, i will not pass through.
            return encoding;
        }

        private static bool TestEncoding(byte[] buffer, System.Text.Encoding encoding)
        {
            if (IsMatchBOM(buffer, encoding) == true)
            {
                return true;
            }

            return TestEncodingDecodeEncode(buffer, encoding);
        }

        public static bool TestEncodingDecodeEncode(byte[] buffer, System.Text.Encoding encoding)
        {
            return PInvoke.ByteArrayCompare(buffer, encoding.GetBytes(encoding.GetString(buffer)));
        }

        public static bool TestEncodingDecodeEncodeDecode(byte[] buffer, System.Text.Encoding encodingFrom, System.Text.Encoding encodingTo)
        {
            string decodeString = encodingFrom.GetString(buffer);
            return string.Equals(decodeString, encodingTo.GetString(encodingTo.GetBytes(decodeString)));
        }

        public static bool TestEncodingEncodeDecode(string buffer, System.Text.Encoding encoding)
        {
            return string.Equals(buffer, encoding.GetString(encoding.GetBytes(buffer)));
        }

        public static bool IsEncodingWithBOM(System.Text.Encoding encoding)
        {
            return encoding.GetPreamble().Length > 0;
        }

        public static bool IsMatchBOM(byte[] buffer, System.Text.Encoding encoding)
        {
            if (IsEncodingWithBOM(encoding) == false)
            {
                return false;
            }

            // Check if the head is BOM
            return PInvoke.ByteArrayCompareStartWith(encoding.GetPreamble(), buffer);
        }
    }
}

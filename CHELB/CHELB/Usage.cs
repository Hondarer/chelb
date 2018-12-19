using System;
using System.Linq;
using System.Reflection;

namespace CHELB
{
    partial class CHELB
    {
        static void Usage()
        {
            Assembly assem = Assembly.GetEntryAssembly();

            string title = null;
            object titleObject = assem.GetCustomAttributes(typeof(AssemblyTitleAttribute), false).FirstOrDefault();
            if (titleObject != null)
            {
                title = ((AssemblyTitleAttribute)titleObject).Title;
            }

            string description = null;
            object descriptionObject = assem.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).FirstOrDefault();
            if (descriptionObject != null)
            {
                description = ((AssemblyDescriptionAttribute)descriptionObject).Description;
            }

            string copyright = null;
            object copyrightObject = assem.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).FirstOrDefault();
            if (copyrightObject != null)
            {
                copyright = ((AssemblyCopyrightAttribute)copyrightObject).Copyright;
            }

            string version = "Build: " + assem.GetName().Version.ToString();
#if DEBUG
            version += " - Debug";
#endif

            int width = Console.WindowWidth;
            if (width > 0)
            {
                width -= 1;
            }

            Console.WriteLine(
                $"{title} - {description}\r\n" +
                $"{version.PadLeft(width, ' ')}\r\n" +
                $"{copyright.PadLeft(width, ' ')}\r\n\r\n" +
                $"{title} [/IE:AUTO | /IE:encodingname] [/OE:encodingname]\r\n" +
                $"{new string(' ', title.Length)} [/OB:TAKEOVER | /OB:NO | /OB:YES]\r\n" + "" +
                $"{new string(' ', title.Length)} [/LB:NOCARE | /LB:LF | /LB:CR | /LB:CRLF] [/F] [/FC] [/T]\r\n" +
                $"{new string(' ', title.Length)} path [...]\r\n\r\n" +
                "  path  Specify the file (s) to be converted.\r\n" +
                "  /IE   Specify the encoding of the original file.\r\n" +
                "  /OE   Specify the encoding you want to convert.\r\n" +
                "  /OB   Specify the granting the BOM.\r\n" +
                "  /LB   Specify conversion of line break code.\r\n" +
                "  /F    If a backup file exists, overwrite it and continue the conversion.\r\n" +
                "  /FC   Bypass the check and allow irreversible conversions.\r\n" +
                "  /T    Change the file update date and time.\r\n\r\n" +
                "Wildcards can be used as the conversion source file name.\r\n" +
                "You can also specify multiple paths.\r\n\r\n" +
                "If /IE is not specified, the encoding of the conversion source file is\r\n" +
                "automatically detected.\r\n\r\n" +
                "By default, conversion is performed only when there is no backup file,\r\n" +
                "and if overwriting the backup file and converting, specify the /F option.\r\n\r\n" +
                "By default, the last write date of the file is not updated.\r\n" +
                "If /T option is specified, it is updated.\r\n\r\n" +
                "The encodingname specifies the encoding name specified by IANA.\r\n" +
                "For example, shift_jis, iso-2022-jp, euc-jp, utf-8, utf-16."
                );
            //foreach (System.Text.EncodingInfo encoding in System.Text.Encoding.GetEncodings())
            //{
            //    Console.WriteLine($"{encoding.Name}\t{encoding.GetEncoding().WebName}\t{Encoding.IsEncodingWithBOM(encoding.GetEncoding())}");
            //}
        }
    }
}

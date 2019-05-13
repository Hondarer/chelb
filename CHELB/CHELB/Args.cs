using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CHELB
{
    partial class CHELB
    {
        public class Parameters
        {
            public System.Text.Encoding InputEncoding { get; set; } = null;

            public System.Text.Encoding OutputEncoding { get; set; } = null;

            public bool OverwriteBackupfile { get; set; } = false;

            public bool TouchLastWriteTime { get; set; } = false;

            public bool BypassCheck { get; set; } = false;

            public LineBreaks LineBreak { get; set; } = LineBreaks.NoCare;

            public BomKinds BomKind { get; set; } = BomKinds.TakeOver;
        }

        static bool PrepareArgs(string[] args, out Parameters parameters, out List<string> files)
        {
            parameters = new Parameters();
            files = new List<string>();

            List<string> filesForDuplicationCheck = new List<string>();

            bool noWarn = true;

            foreach (string arg in args)
            {
                if (arg.StartsWith("/IE:", StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    // Input Encoding

                    string inEncodingString = arg.Substring("/IE:".Count());
                    if (inEncodingString.Equals("Auto", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        parameters.InputEncoding = null;
                    }
                    else
                    {
                        try
                        {
                            parameters.InputEncoding = System.Text.Encoding.GetEncoding(inEncodingString);
                        }
                        catch
                        {
                            Console.Error.WriteLine("/IE parameter is invalid.");
                            throw new ArgumentException("/IE");
                        }
                    }
                }
                else if (arg.StartsWith("/OE:", StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    // Output Encoding

                    try
                    {
                        parameters.OutputEncoding = System.Text.Encoding.GetEncoding(arg.Substring("/OE:".Count()));
                    }
                    catch
                    {
                        Console.Error.WriteLine("/OE parameter is invalid.");
                        throw new ArgumentException("/OE");
                    }
                }
                else if (arg.StartsWith("/OB:", StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    // Output BOM

                    string outputBOMString = arg.Substring("/OB:".Count());
                    if (outputBOMString.Equals("TakeOver", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        parameters.BomKind = BomKinds.TakeOver;
                    }
                    else if (outputBOMString.Equals("No", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        parameters.BomKind = BomKinds.No;
                    }
                    else if (outputBOMString.Equals("Yes", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        parameters.BomKind = BomKinds.Yes;
                    }
                    else
                    {
                        Console.Error.WriteLine("/OB parameter is invalid.");
                        throw new ArgumentException("/OB");
                    }
                }
                else if (arg.Equals("/F", StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    // Overwrite Bak file

                    parameters.OverwriteBackupfile = true;
                }
                else if (arg.Equals("/FC", StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    // Bypass check

                    parameters.BypassCheck = true;
                }
                else if (arg.Equals("/T", StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    // Touch last write time

                    parameters.TouchLastWriteTime = true;
                }
                else if (arg.StartsWith("/LB:", StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    // Line Break

                    string lineBreak = arg.Substring("/LB:".Count());

                    if (lineBreak.Equals("NoCare", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        parameters.LineBreak = LineBreaks.NoCare;
                    }
                    else if (lineBreak.Equals("LF", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        parameters.LineBreak = LineBreaks.LF;
                    }
                    else if (lineBreak.Equals("CR", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        parameters.LineBreak = LineBreaks.CR;
                    }
                    else if (lineBreak.Equals("CRLF", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        parameters.LineBreak = LineBreaks.CRLF;
                    }
                    else
                    {
                        Console.Error.WriteLine("/LB parameter is invalid.");
                        throw new ArgumentException("/LB");
                    }
                }
                else
                {
                    string basePath = Path.GetDirectoryName(arg);
                    bool emptyPath = false;
                    if (string.IsNullOrWhiteSpace(basePath) == true)
                    {
                        basePath = Directory.GetCurrentDirectory();
                        emptyPath = true;
                    }

                    bool foundFile = false;
                    foreach (string filePath in Directory.GetFiles(basePath, Path.GetFileName(arg)))
                    {
                        foundFile = true;

                        // When evaluating with full path and the same file has already been registered,
                        // skip not to convert more than once.

                        // At this time, no warning or the like is made.

                        if (filesForDuplicationCheck.Contains(Path.GetFullPath(filePath)) != true)
                        {
                            filesForDuplicationCheck.Add(Path.GetFullPath(filePath));

                            if (emptyPath == true)
                            {
                                files.Add(Path.GetFileName(filePath));
                            }
                            else
                            {
                                files.Add(filePath);
                            }
                        }
                    }

                    if (foundFile == false)
                    {
                        noWarn = false;
                        Console.Error.WriteLine($"{arg} is an invalid file name or parameter.");
                    }
                }
            }

            return noWarn;
        }
    }
}

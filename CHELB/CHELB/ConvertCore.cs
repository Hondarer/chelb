using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CHELB
{
    partial class CHELB
    {
        static bool ConvertCore(string filePath, Parameters parameters)
        {
            string bakFileName = $"{filePath}.bak";

            DateTime creationTime;
            DateTime lastWriteTime;
            DateTime lastAccessTime;

            StringBuilder sb = new StringBuilder();

            bool noWarn = true;

            try
            {
                sb.Append(filePath);

                if (Path.GetExtension(filePath) == Path.GetExtension(bakFileName))
                {
                    noWarn = false;
                    sb.Append($" -> backup file, skip.");
                }
                else
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        creationTime = File.GetCreationTime(filePath);
                        lastWriteTime = File.GetLastWriteTime(filePath);
                        lastAccessTime = File.GetLastAccessTime(filePath);

                        FileMode bakFileMode = FileMode.CreateNew;
                        if (parameters.OverwriteBackupfile == true)
                        {
                            bakFileMode = FileMode.Create;
                        }

                        try
                        {
                            using (FileStream fsbak = new FileStream(bakFileName, bakFileMode, FileAccess.Write, FileShare.None))
                            {
                                byte[] buffer = new byte[fs.Length];

                                fs.Read(buffer, 0, (int)fs.Length);
                                fsbak.Write(buffer, 0, buffer.Length);
                                fsbak.Close();

                                File.SetCreationTime(bakFileName, creationTime);
                                File.SetLastWriteTime(bakFileName, lastWriteTime);
                                File.SetLastAccessTime(bakFileName, lastAccessTime);

                                try
                                {
                                    System.Text.Encoding inputEncoding;
                                    string encodingAuto = null;
                                    if (parameters.InputEncoding == null)
                                    {
                                        inputEncoding = Encoding.AutoDetect(buffer);

                                        if (inputEncoding == null)
                                        {
                                            sb.Append($" -> Failed to automatically determine encoding");
                                            throw new Exception();
                                        }

                                        encodingAuto = ", auto-detect";
                                    }
                                    else
                                    {
                                        // 入力エンコードの妥当性検証
                                        if ((parameters.BypassCheck != true) && (parameters.InputEncoding != null))
                                        {
                                            if (Encoding.TestEncodingDecodeEncode(buffer, parameters.InputEncoding) == false)
                                            {
                                                sb.Append($" -> The specified input encoding can not read the file correctly");
                                                throw new Exception();
                                            }
                                        }

                                        inputEncoding = parameters.InputEncoding;
                                    }

                                    System.Text.Encoding outputEncoding;
                                    if (parameters.OutputEncoding == null)
                                    {
                                        // Same encoding as input
                                        outputEncoding = inputEncoding;
                                    }
                                    else
                                    {
                                        // 出力エンコードの妥当性検証
                                        if (parameters.BypassCheck != true)
                                        {
                                            if (Encoding.TestEncodingDecodeEncodeDecode(buffer, inputEncoding, parameters.OutputEncoding) == false)
                                            {
                                                sb.Append($" -> The specified output encoding can not write the file correctly");
                                                throw new Exception();
                                            }
                                        }
                                        outputEncoding = parameters.OutputEncoding;
                                    }

                                    string inWithBOM = null;
                                    if (Encoding.IsMatchBOM(buffer, inputEncoding) == true)
                                    {
                                        inWithBOM = ", BOM";
                                    }
                                    else
                                    {
                                        if (Encoding.IsEncodingWithBOM(inputEncoding) == true)
                                        {
                                            // 入力が BOM サポートの Encoding で BOM がない
                                            if (parameters.BomKind == BomKinds.TakeOver)
                                            {
                                                outputEncoding = Encoding.GetEncodingWithoutBOM(outputEncoding);
                                            }
                                        }
                                    }

                                    if (parameters.BomKind == BomKinds.No)
                                    {
                                        outputEncoding = Encoding.GetEncodingWithoutBOM(outputEncoding);
                                    }

                                    sb.Append($" -> {inputEncoding.WebName}{inWithBOM}{encodingAuto} to {outputEncoding.WebName}");

                                    if (Encoding.IsEncodingWithBOM(outputEncoding) == true)
                                    {
                                        sb.Append($", BOM");
                                    }

                                    fs.Position = 0;

                                    using (StreamReader sr = new StreamReader(fs, inputEncoding))
                                    {
                                        string text = sr.ReadToEnd();


                                        // 文字コードの変換
                                        if (parameters.LineBreak != LineBreaks.NoCare)
                                        {
                                            // LF に揃える
                                            text = Regex.Replace(text, "\r\n|\r", "\n");

                                            if (parameters.LineBreak == LineBreaks.CR)
                                            {
                                                // LF -> CR
                                                text = Regex.Replace(text, "\n", "\r");
                                            }
                                            else if (parameters.LineBreak == LineBreaks.CRLF)
                                            {
                                                // LF -> CRLF
                                                text = Regex.Replace(text, "\n", "\r\n");
                                            }
                                            else
                                            {
                                                // NOP(通過しないケース)
                                            }

                                            sb.Append($", {parameters.LineBreak}");
                                        }

                                        fs.SetLength(0);
                                        fs.Position = 0;

                                        using (StreamWriter sw = new StreamWriter(fs, outputEncoding))
                                        {
                                            sw.Write(text);
                                        }
                                    }

                                    File.SetCreationTime(filePath, creationTime);
                                    if (parameters.TouchLastWriteTime == false)
                                    {
                                        File.SetLastWriteTime(filePath, lastWriteTime);
                                        File.SetLastAccessTime(filePath, lastAccessTime);
                                    }

                                    sb.Append($" -> done.");
                                }
                                catch
                                {
                                    noWarn = false;
                                    sb.Append($" -> convert failed.");

                                    try
                                    {
                                        fs.Position = 0;
                                        fs.Write(buffer, 0, buffer.Length);
                                        File.SetCreationTime(filePath, creationTime);
                                        File.SetLastWriteTime(filePath, lastWriteTime);
                                        File.SetLastAccessTime(filePath, lastAccessTime);
                                    }
                                    catch
                                    {
                                        // NOP
                                    }
                                }
                            }
                        }
                        catch
                        {
                            noWarn = false;
                            sb.Append($" -> backup failed.");
                        }
                    }
                }
            }
            catch
            {
                noWarn = false;
                sb.Append($" -> open failed.");
            }

            if (noWarn == false)
            {
                Console.Error.WriteLine(sb.ToString());
            }
            else
            {
                Console.WriteLine(sb.ToString());
            }

            return noWarn;
        }
    }
}

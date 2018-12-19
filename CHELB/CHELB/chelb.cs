using System.Collections.Generic;

namespace CHELB
{
    partial class CHELB
    {
        static int Main(string[] args)
        {
            List<string> files;
            Parameters parameters;

            bool noWarn = true;

            if ((args.Length == 0) || ((args.Length == 1) && (args[0] == "/?")))
            {
                Usage();
                return 0;
            }

            try
            {
                if (PrepareArgs(args, out parameters, out files) == false)
                {
                    return 1;
                }
            }
            catch
            {
                return 1;
            }

            foreach (string file in files)
            {
                if (ConvertCore(file, parameters) == false)
                {
                    noWarn = false;
                }
            }

            if (noWarn == false)
            {
                return 1;
            }

            return 0;
        }
    }
}

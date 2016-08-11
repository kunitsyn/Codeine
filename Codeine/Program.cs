using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Codeine
{
    public static class Program
    {
        public struct DiffInfo
        {
            public string File1, File2, Title1, Title2;
        }
        public struct MergeInfo
        {
            public string FileBase, File1, File2, FileMerged, TitleBase, Title1, Title2;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            object commandLineInfo = ParseCommandLine();
            if (commandLineInfo == null)
            {
                ShowUsage();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (commandLineInfo as DiffInfo? != null)
            {
                Application.Run(new DiffForm((DiffInfo)commandLineInfo));
            }
            else if (commandLineInfo as MergeInfo? != null)
            {
                Application.Run(new MergeForm((MergeInfo)commandLineInfo));
            }
        }

        static object ParseCommandLine()
        {
            Dictionary<string, string> namedArgs = new Dictionary<string, string>();
            List<string> namelessArgs = new List<string>();

            var args = Environment.GetCommandLineArgs().Skip(1);
            foreach (string arg in args)
            {
                if (arg.Length == 0)
                {
                    continue;
                }

                if (arg[0] == '/')
                {
                    int colonIdx = arg.IndexOf(':');
                    if (colonIdx == -1)
                    {
                        return null;
                    }
                    string key = arg.Substring(1, colonIdx - 1);
                    string value = arg.Substring(colonIdx + 1);

                    namedArgs[key] = value;
                }
                else
                {
                    namelessArgs.Add(arg);
                }
            }

            if (namelessArgs.Count == 2)
            {
                DiffInfo result;
                result.File1 = namelessArgs[0];
                result.File2 = namelessArgs[1];
                namedArgs.TryGetValue("title1", out result.Title1);
                namedArgs.TryGetValue("title2", out result.Title2);
                return result;
            }
            else if (namelessArgs.Count == 4)
            {
                MergeInfo result;
                result.FileBase = namelessArgs[0];
                result.File1 = namelessArgs[1];
                result.File2 = namelessArgs[2];
                result.FileMerged = namelessArgs[3];
                namedArgs.TryGetValue("title1", out result.TitleBase);
                namedArgs.TryGetValue("title2", out result.Title1);
                namedArgs.TryGetValue("title3", out result.Title2);
                return result;
            }
            else
            {
                return null;
            }
        }

        static void ShowUsage()
        {
            MessageBox.Show("Incorrect command line parameters.\nUsage:\n\n" +
                "Codeine %file1 %file2 [/title1:%title1] [/title2:%title2] - run diff between %file1 and %file2.\n\n" +
                "Codeine %base %theirs %yours %merged [/title1:%titlebase] [/title2:%titletheirs] [/title3:%titleyours] - " +
                "run merge into %merged file with %theirs and %yours file, and %base file as base.", "Codeine");
        }
    }
}

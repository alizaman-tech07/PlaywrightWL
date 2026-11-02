using System;
using System.IO;

namespace PlaywrightTest2.Utilities
{
    public static class PathHelper
    {
        /// <summary>
        /// Returns the TestResults directory at the project root.
        /// Base dir is bin/Debug/net10.0 so we walk up three levels to the project root,
        /// then append "TestResults". Works locally and in CI.
        /// </summary>
        public static string GetRoot()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            DirectoryInfo dir = new DirectoryInfo(baseDir);
            for (int i = 0; i < 3; i++)
            {
                if (dir.Parent != null)
                    dir = dir.Parent;
            }

            return Path.Combine(dir.FullName, "TestResults");
        }
    }
}

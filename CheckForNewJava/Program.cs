using System;
using System.Net;
using System.Text.RegularExpressions;

namespace CheckForNewJava
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Java Version Check";
            WriteLineInMiddle(".oO Java version check - Made by Stiig Gade Oo.");
            Console.WriteLine();

            Java latestVersion = null;
            Java currentVersion = null;

            //Getting current Java version from local PC
            try
            {
                currentVersion = GetJavaVersionInformation();

                if (currentVersion == null)
                    WriteLineInMiddle("Could not find current version!");
                else
                    WriteLineInMiddle("Current: " + currentVersion);
            }
            catch (Exception)
            {
                WriteInMiddle("Something went wrong trying to get the current version!");
            }

            //Getting latest Java version from Java's website
            try
            {
                latestVersion = GetLatestJavaVersion();

                if (latestVersion == null)
                    WriteLineInMiddle("Could not find latest version!");
                else
                    WriteLineInMiddle("Latest:  " + latestVersion);
            }
            catch (Exception)
            {
                WriteInMiddle("Something went wrong trying to get the latest version!");
            }
            
            //If both versions was retrieved, check if local version is up to date
            if (latestVersion != null && currentVersion != null)
            {
                if (currentVersion == latestVersion)
                {
                    WriteLineInMiddle("You are up to date!");
                    WaitTimeout(10);
                }
                else
                {
                    WriteLineInMiddle("You are not up to date!");
                    Console.ReadKey();
                }
            }
            else
                WaitTimeout(10);
        }

        static void WriteLineInMiddle(string text)
        {
            int textLength = text.Length;

            if (Console.WindowWidth <= textLength)
                Console.WriteLine(text);
            else
                Console.WriteLine(new string(' ', (Console.WindowWidth - textLength) / 2) + text);
        }

        static void WriteInMiddle(string text)
        {
            int textLength = text.Length;

            if (Console.WindowWidth <= textLength)
                Console.Write(text);
            else
            {
                int leftSpaceCount = Convert.ToInt32(Math.Ceiling((double)(Console.WindowWidth - textLength) / 2));

                string space = new string(' ', leftSpaceCount);
                string newText = space + text;
                Console.Write("\r" + newText);
            }
        }

        private static Java GetJavaVersionInformation()
        {
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("java", "-version ");

                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.RedirectStandardError = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                string text = proc.StandardError.ReadToEnd();

                Regex getJavaVersion = new Regex(@"java version ""1\.(\d+)\.0_(\d+)""");
                Match m = getJavaVersion.Match(text);

                return new Java(m);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Java GetLatestJavaVersion()
        {
            Regex findJavaVersion = new Regex(@"Version (\d+) Update (\d+)");

            WebClient client = new WebClient();
            string downloadString = client.DownloadString("http://www.java.com/en/download/win8.jsp");
            Match m = findJavaVersion.Match(downloadString);

            return new Java(m);
        }

        public class Java : IEquatable<Java>
        {
            public int Version { get; set; }
            public int Update { get; set; }
            

            public Java(Match m)
            {
                if (m.Groups.Count == 3)
                {
                    Version = int.Parse(m.Groups[1].Value);
                    Update = int.Parse(m.Groups[2].Value);
                }
                else
                    throw new Exception();
            }
            public override string ToString()
            {
                return String.Format("Version {0} Update {1}", Version, Update);
            }

            public bool Equals(Java other)
            {
                if (other == null)
                    return false;

                return this.Version == other.Version && this.Update == other.Update;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj as Java);
            }

            public override int GetHashCode()
            {
                return this.Version * this.Update;
            }

            public static bool operator ==(Java a, Java b)
            {
                if (System.Object.ReferenceEquals(a, b))
                    return true;

                // If one is null, but not both, return false.
                if (((object)a == null) || ((object)b == null))
                    return false;

                // Return true if the fields match:
                return a.Version == b.Version && a.Update == b.Update;
            }

            public static bool operator !=(Java a, Java b)
            {
                return !(a == b);
            }
        }

        public static void WaitTimeout(int timeOutSec)
        {
            Console.WriteLine('\r');
            timeOutSec *= 1000;
            DateTime timeoutvalue = DateTime.Now.AddMilliseconds(timeOutSec);

            int counter = timeOutSec / 100;

            while (DateTime.Now < timeoutvalue)
            {
                int rest = counter-- % 10;

                if (rest == 0)
                    WriteInMiddle(((counter + 1) / 10).ToString());

                if (Console.KeyAvailable)
                    break;
                else
                    System.Threading.Thread.Sleep(100);
            }

            Console.WriteLine();
        }
    }
}

using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CSharpLambda
{
    public class CSharpLambdaInspector
    {
        public string call_shell_wrapper(string processName, string args = null)
        {
            string results = "";
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = processName,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                results = proc.StandardOutput.ReadLine();
            }
            return results;
        }

        public string contents_of_file(string fileName)
        {
            return File.ReadAllText(fileName, Encoding.UTF8);
        }

        public string get_etc_issue()
        {
            return contents_of_file("/etc/issue");
        }

        public string get_pwd()
        {
            return call_shell_wrapper("pwd");
        }

        public string get_uname()
        {
            return call_shell_wrapper("uname", "-a");
        }

        public string get_release_version()
        {
            return null;
        }

        public string get_env()
        {
            return Environment.GetEnvironmentVariables().ToString();
        }

        public string get_df()
        {
            return call_shell_wrapper("df", "-h");
        }

        public string get_dmesg()
        {
            return call_shell_wrapper("dmesg");
        }

        public string get_cpuinfo()
        {
            return contents_of_file("/proc/cpuinfo");
        }

        public string get_meminfo()
        {
            return contents_of_file("/proc/meminfo");
        }

        public string get_processes()
        {
            return call_shell_wrapper("ps", "aux");
        }

        public string get_packages()
        {
            return null;
        }

        public string get_package_count()
        {
            return null;
        }

        public string get_timestamp()
        {
            return DateTime.UtcNow.ToString();
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public void Handler()
        {
            Dictionary<string, object> runtimeInformation = new Dictionary<string, object>();

            runtimeInformation.Add("dmesg", get_dmesg());
            runtimeInformation.Add("meminfo", get_meminfo());
            runtimeInformation.Add("cpuinfo", get_cpuinfo());
            runtimeInformation.Add("df", get_df());
            runtimeInformation.Add("timestamp", get_timestamp());
            runtimeInformation.Add("ps", get_processes());
            runtimeInformation.Add("/etc/issue", get_etc_issue());
            runtimeInformation.Add("uname", get_uname());
            runtimeInformation.Add("pwd", get_pwd());

            LambdaLogger.Log("\n");
            LambdaLogger.Log(JsonConvert.SerializeObject(runtimeInformation, Formatting.Indented));
            LambdaLogger.Log("\n");
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace webviewtestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string ResourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Resources");

        [ComVisible(true)]
        public class ScriptManager
        {
            private readonly WebBrowser _webview;
            public ScriptManager(WebBrowser webview)
            {
                _webview = webview;
            }

            public void Test(String message)
            {
                MessageBox.Show(message, "client code");
            }

            public void docker(String command)
            {
                var output = DockerClient(command);
                var firstWord = command.Split(' ')[0];
                _webview.InvokeScript($"docker_{firstWord}", output);
            }

            public string DockerClient(string command)
            {
                var exePath = Path.Combine(MainWindow.ResourcePath, "docker-client.exe");
                var pei = Run(exePath, $"-compact -{command}", 0);
                return pei.StandardOutput.Replace("\r\n", "");
            }

            public class ProcessExecutionInfo
            {
                public int ExitCode;
                public string StandardOutput;
                public string ErrorOutput;
                public string CombinedOutput;
                public bool TimedOut;
            }
            public ProcessExecutionInfo Run(string filename, string arguments, int timeout)
            {
                var process = CreateProcess(filename, arguments);
                return StartProcessAndCaptureOutput(timeout, process);
            }
            private static Process CreateProcess(string filename, string arguments)
            {
                return new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        FileName = filename,
                        Arguments = arguments
                    }
                };
            }
            private static ProcessExecutionInfo StartProcessAndCaptureOutput(int timeout, Process process)
            {
                process.Start();

                var standardOutput = "";
                var errorOutput = "";
                var combinedOutput = "";
                var combinedOutputLock = new object();

                process.OutputDataReceived += (sender, args) =>
                {
                    var line = args.Data;
                    if (line == null) return;

                    standardOutput += line + Environment.NewLine;
                    lock (combinedOutputLock)
                    {
                        combinedOutput += line + Environment.NewLine;
                    }
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    var line = args.Data;
                    if (line == null) return;

                    errorOutput += line + Environment.NewLine;
                    lock (combinedOutputLock)
                    {
                        combinedOutput += line + Environment.NewLine;
                    }
                };

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                var timedOut = false;
                if (timeout == 0)
                    process.WaitForExit();
                else if (!process.WaitForExit(timeout)) // process timed out
                {
                    try { process.Kill(); }
                    catch
                    {
                        // ignored
                    }
                    timedOut = true;
                }

                return new ProcessExecutionInfo
                {
                    ExitCode = process.ExitCode,
                    StandardOutput = standardOutput,
                    ErrorOutput = errorOutput,
                    CombinedOutput = combinedOutput,
                    TimedOut = timedOut
                };
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            WebView.ObjectForScripting = new ScriptManager(WebView);
            // Uncomment the following line when you are finished debugging.
            //webBrowser1.ScriptErrorsSuppressed = true;

            //WebView.NavigateToString(
            //    "<html><head><script>" +
            //    "function test(message) { alert(message); }" +
            //    "</script></head><body><button " +
            //    "onclick=\"window.external.Test('called from script code')\">" +
            //    "call client code from script code</button>" +
            //    "</body></html>");
            var indexPath = Path.Combine(ResourcePath, "index.html");
            WebView.Navigate(indexPath);
        }
    }
}

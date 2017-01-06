using System;
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
                var js = $"docker_{firstWord}( JSON.parse(\'{output}\') );";
                _webview.InvokeScript(js);
            }

            public string DockerClient(string command)
            {
                return "";
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            WebView.ObjectForScripting = new ScriptManager(WebView);
            // Uncomment the following line when you are finished debugging.
            //webBrowser1.ScriptErrorsSuppressed = true;

            WebView.NavigateToString(
                "<html><head><script>" +
                "function test(message) { alert(message); }" +
                "</script></head><body><button " +
                "onclick=\"window.external.Test('called from script code')\">" +
                "call client code from script code</button>" +
                "</body></html>");
        }
    }
}

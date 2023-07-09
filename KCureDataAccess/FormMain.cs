
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KCureDataAccess
{
    public partial class MainForm : Form
    {
        public Observer observer;
        public Controller controller;
        public Config config;
        public Store? store;

        public MainForm()
        {
            InitializeComponent();
            //
            config = new Config();
            //
            observer = new Observer();
            observer.Add(this);
            //
            controller = new Controller(observer, config);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            webView2.Source = new Uri(config.webRoot + "01-login.html");
            webView2.WebMessageReceived += WebView2_WebMessageReceived;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            int width = this.Width;
            int height = this.Height;

            //int scrollbarWidth = SystemInformation.VerticalScrollBarWidth;
            //int scrollbarHeight = SystemInformation.HorizontalScrollBarHeight;
            //width -= scrollbarWidth;
            //height -= scrollbarHeight;

            webView2.Size = new Size(width, height);
        }

        private void WebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string strJson = e.WebMessageAsJson;
                Console.WriteLine("\nDebug>>> Read Page JSON");
                Console.WriteLine(strJson);
                controller.Parse(strJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DEBUG>>> ({0}) {1}", "error", ex.ToString());
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Console.WriteLine("DEBUG>>> ({0}) {1}", "error", "error");
            }
        }

        public void Listener(string target, string action, string message, dynamic data)
        {
            Console.WriteLine("\nDebug>>> MainForm Listner");
            Console.WriteLine("(target) " + target);
            Console.WriteLine("(action) " + action);
            Console.WriteLine("(message) " + message);
            //
            if (target != "formMain")
                return;
            //
            if (action == "page")
            {
                webView2.Source = new Uri(config.webRoot + message + ".html");
            }
            else if(action == "api")
            {
                List<Dictionary<string, object>> listDicData = (List<Dictionary<string, object>>) data;
                string jsonData = JsonSerializer.Serialize(listDicData);
                webView2.CoreWebView2.ExecuteScriptAsync($"reciveUsersReserchData('{jsonData}');");
            }
        }
    }
}

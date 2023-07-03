
using Microsoft.Web.WebView2.Core;
using System;


namespace KCureDataAccess
{
    public partial class MainForm : Form
    {
        public Observer observer;
        public Controller controller;
        private string urlPrefix = @"D:/workspaces/vs/web/";

        public MainForm()
        {
            InitializeComponent();
            //
            observer = new Observer();
            observer.Add(this);
            //
            controller = new Controller(observer);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            webView2.Source = new Uri(urlPrefix + "index.html");
            webView2.WebMessageReceived += WebView2_WebMessageReceived;
        }

        private void WebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string strJson = e.WebMessageAsJson;
                Console.WriteLine(strJson);
                controller.Parse(strJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Listen(string type, string message, dynamic data)
        {
            if (type == "page")
            {
                if(message == "test")
                {
                    webView2.Source = new Uri(urlPrefix + "test.html");
                }
            }
        }
    }
}

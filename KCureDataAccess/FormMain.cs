
using Microsoft.Web.WebView2.Core;


namespace KCureDataAccess
{
    public partial class MainForm : Form
    {
        public Observer observer;
        public Controller controller;
        public Config config;
        public Store store;

        public MainForm()
        {
            InitializeComponent();
            //
            config = new Config();
            //
            observer = new Observer();
            observer.Add(this);
            //
            controller = new Controller(observer);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            webView2.Source = new Uri(config.webRoot + "01-login.html");
            webView2.WebMessageReceived += WebView2_WebMessageReceived;
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
                MessageBox.Show(ex.ToString());
            }
        }

        public void Listener(string target, string action, string message, dynamic data)
        {
            Console.WriteLine("\nDebug>>> MainForm Listner");
            Console.WriteLine("(target) " + target);
            Console.WriteLine("(action) " + action);
            Console.WriteLine("(message) " + message);

            if (target != "formMain")
                return;

            if (action == "page")
            {
                webView2.Source = new Uri(config.webRoot + message + ".html");
            }
        }
    }
}

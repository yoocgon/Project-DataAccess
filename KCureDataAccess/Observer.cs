
namespace KCureDataAccess
{
    public class Observer
    {
        public MainForm formMain;
        public Controller controller;

        public void Send(String target, String action, String message, dynamic data)
        {
            Console.WriteLine("\nDebug>>> Observer Sender");
            Console.WriteLine("(target) " + target);
            Console.WriteLine("(action) " + action);
            Console.WriteLine("(message) " + message);

            if (target == "formMain")
            {
                formMain.Listener(target, action, message, data);
            }
            else if(target == "controller")
            {
                controller.Listener(target, action, message, data);
            }
        }

        public void Add(MainForm formMain)
        {
            this.formMain = formMain;
        }

        public void Add(Controller controller)
        {
            this.controller = controller;
        }
    }
}

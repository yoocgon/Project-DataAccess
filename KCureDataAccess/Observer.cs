
namespace KCureDataAccess
{
    public class Observer
    {
        public MainForm form1;
        public Controller controller;

        public void Send(String type, String message, String target, dynamic data)
        {
            if(target == "form1")
            {
                form1.Listen(type, message, data);
            }
            else if(target == "controller")
            {
                controller.Listen(type, message, data);
            }
        }

        public void Add(MainForm form1)
        {
            this.form1 = form1;
        }

        public void Add(Controller controller)
        {
            this.controller = controller;
        }
    }
}

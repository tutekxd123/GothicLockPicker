namespace GothicLockPicker
{
    public partial class Form1 : Form
    {
        void RenderTable(List<Lock> StateObject)
        {
            //TableView
            foreach (Lock lockObject in StateObject)
            {
                //Add Row


            }
            //Render Table
        }
        public Form1()
        {
            InitializeComponent();
            //Initalize Table View
            TableView.Columns.Add("Position","Position");
            TableView.Columns.Add("ValueLock", "ValueLock"); //7 RoundBoxy?
            TableView.Columns.Add("ConnectionLock1", "ConnectionLock1");
            TableView.Columns.Add("ConnectionLockRightOrLeft", "Lock1Left?");
            TableView.Columns.Add("ConnectionLock2", "ConnectionLock2");
            TableView.Columns.Add("Connection2LockRightOrLeft", "Lock2Left?");
            List<Lock> StateObject = new();
            StateObject.Add(new Lock(1)); //For Debug Only
            RenderTable(StateObject);
            //Render Table?
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

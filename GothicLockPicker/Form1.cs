using MonitorKeyboard;
using System.ComponentModel;

namespace GothicLockPicker
{

    public partial class Form1 : Form
    {
        static public KeyBoardManager keyBoardManager = KeyBoardManager.GetInstance();
        public BindingList<LockRow> lockRows = new BindingList<LockRow> { new(0, 2), new(1, 2) };
        static public CancellationTokenSource tokenCancel = new CancellationTokenSource();
        public AutoSolver solver = new AutoSolver(tokenCancel, 30);

        public Form1()
        {
            InitializeComponent();

            if (keyBoardManager != null)
            {
                keyBoardManager.StartHooking();
                keyBoardManager.KeyBoardEvent += OnClickSolve;
            }

            TableView.AutoGenerateColumns = false;

            //Position.DataPropertyName = "Position";
            Position.DataPropertyName = "HumanPosition";
            ValueLock.DataPropertyName = "HumanValue";
            ValueLock.Items.Clear();
            for (int i = 1; i < 8; i++)
                ValueLock.Items.Add(i);

            TableView.DataSource = lockRows;
            //Render Table?sda
        }
        public void OnClickSolve(object? sender, EventHandlerKeyBoard e)
        {
            if (e.EventType == EventsKeyboard.WM_KEYDOWN && e.KeyCode == KeyBoardManager.KeyBoardMap["F2"])
            {
                button_Solve_Click();
            }
            else if (e.EventType == EventsKeyboard.WM_KEYDOWN && e.KeyCode == KeyBoardManager.KeyBoardMap["F3"])
            {
                tokenCancel.Cancel();
                tokenCancel.Dispose();
            }


        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Limit_Gscore_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lockRows.Count() < 7)
            {
                lockRows.Add(new(lockRows.Count(), 4));
            }

            Console.WriteLine("test");
        }
        private void button_Solve_Click()
        {
            bool OptimalzerCheckbox = checkBox1_Optimalizer.Checked;
            int[,] MatrixConnections = new int[7, 7];
            for (int i = 1; i < 7; i++)
            {
                for (int j = 1; j < 7; j++)
                {
                    var control = this.Controls.Find($"numericUpDown{i}_{j}", true);
                    if (control.Length > 0 && control[0] is NumericUpDown nud)
                    {
                        MatrixConnections[i - 1, j - 1] = (int)nud.Value;
                    }
                }

            }
            string ResultFind = AstarFinder.GetResolve(MatrixConnections, lockRows, (int)Limit_Steps.Value, true, OptimalzerCheckbox);
            solver.ClickFromString(ResultFind);

        }

        private void button_Solve_Click(object sender, EventArgs e)
        {
            //Solve!
            //Convert Data From Matrix View to 2D Array
            bool OptimalzerCheckbox = checkBox1_Optimalizer.Checked;
            bool HumanReadableCheckbox = !checkBox1_HumanReadable.Checked;
            int[,] MatrixConnections = new int[7, 7];
            for (int i = 1; i < 7; i++)
            {
                for (int j = 1; j < 7; j++)
                {
                    var control = this.Controls.Find($"numericUpDown{i}_{j}", true);
                    if (control.Length > 0 && control[0] is NumericUpDown nud)
                    {
                        MatrixConnections[i - 1, j - 1] = (int)nud.Value;
                    }
                }

            }
            string ResultFind = AstarFinder.GetResolve(MatrixConnections, lockRows, (int)Limit_Steps.Value, HumanReadableCheckbox, OptimalzerCheckbox);
            textBox_Result.Text = ResultFind;


        }

        private void TableView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in TableView.SelectedRows)
            {
                if (row.DataBoundItem != null)
                {
                    lockRows.Remove((LockRow)row.DataBoundItem);
                }

            }
            //Fix the positions of the remaining rows
            for (int i = 0; i < lockRows.Count; i++)
            {
                lockRows[i].Position = i;
            }
        }

        private void button_Reset_Click(object sender, EventArgs e)
        {
            lockRows.Clear();
            lockRows.Add(new(0, 2));
            lockRows.Add(new(1, 2));
            for (int i = 1; i <= 7; i++)
            {
                for (int j = 1; j <= 7; j++)
                {
                    var control = this.Controls.Find($"numericUpDown{i}_{j}", true);

                    if (control.Length > 0 && control[0] is NumericUpDown nud)
                    {
                        nud.Value = 0;
                    }
                }
            } //Reset Matrix

            textBox_Result.Text = "";

            Limit_Steps.Value = 100;
            checkBox1_Optimalizer.Checked = true;
            checkBox1_HumanReadable.Checked = true;

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void TableView_KeyPress(object sender, KeyPressEventArgs e)
        {
            //pick up the selected row;
            int numberTry = -1;
            char key = e.KeyChar;
            string keytest = e.KeyChar.ToString();
            bool status = int.TryParse(keytest, out numberTry);
            if(!status || numberTry<1 || numberTry>7)
            {
                return;
            }
            foreach (DataGridViewRow row in TableView.SelectedRows)
            {
                if (row.DataBoundItem!=null && row.DataBoundItem is LockRow lockRow)
                {
                    lockRow.ValueLock = numberTry - 1;
                }
            }
            TableView.Invalidate();
        }
    }
    public class LockRow
    {
        public int ValueLock { get; set; }
        public int Position { get; set; }

        public int HumanValue
        {
            get
            {
                return ValueLock + 1;
            }
            set
            {
                ValueLock = value - 1;
            }
        }
        public int HumanPosition
        {
            get
            {
                return Position+1;
            }
            set
            {
                Position= value-1;
            }
        }
        public LockRow(int ID, int Value)
        {
            this.Position = ID;
            this.ValueLock = Value;
        }
        public override string ToString()
        {
            return this.Position.ToString();
        }
    }
}

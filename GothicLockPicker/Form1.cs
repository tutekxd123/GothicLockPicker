using System.ComponentModel;

namespace GothicLockPicker
{

    public partial class Form1 : Form
    {
        public BindingList<LockRow> lockRows = new BindingList<LockRow> { new(0, 2), new(1, 2) };
        public Form1()
        {
            InitializeComponent();
            TableView.AutoGenerateColumns = false;

            Position.DataPropertyName = "Position";
            ValueLock.DataPropertyName = "ValueLock";
            ValueLock.Items.Clear();
            for (int i = 0; i < 7; i++)
                ValueLock.Items.Add(i);

            TableView.DataSource = lockRows;
            //Render Table?
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Limit_Gscore_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lockRows.Count()<7)
            {
                lockRows.Add(new(lockRows.Count(), 4));
            }

            Console.WriteLine("test");
        }

        private void button_Solve_Click(object sender, EventArgs e)
        {
            //Solve!
            //Convert Data From Matrix View to 2D Array
            int[,] MatrixConnections = new int[7, 7];
            for (int i = 1; i < 7; i++)
            {
                for(int j = 1; j < 7; j++)
                {
                    var control = this.Controls.Find($"numericUpDown{i}_{j}", true);
                    if (control.Length > 0 && control[0] is NumericUpDown nud)
                    {
                        MatrixConnections[i-1, j-1] = (int)nud.Value;
                    }
                }

            }
            string ResultFind = AstarFinder.GetResolve(MatrixConnections, lockRows, (int)Limit_Steps.Value);
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
            for(int i = 0; i < lockRows.Count; i++)
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


        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
    public class LockRow
    {
        public int ValueLock { get; set; }
        public int Position { get; set; }
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

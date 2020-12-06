using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskProgressExample
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            this.Text = Application.ProductName;
            this.StartPosition = FormStartPosition.CenterScreen;

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
                Padding = new Padding(20),
            };
            Controls.Add(tableLayout);

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            var buttonExample1 = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Font = new Font("Arial", 10),
                Text = "Example 1: no progress report, no cancel button",
            };
            buttonExample1.Click += ButtonExample1_Click;
            tableLayout.Controls.Add(buttonExample1);

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            var buttonExample2 = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Font = new Font("Arial", 10),
                Text = "Example 2: has progress report, no cancel button",
            };
            buttonExample2.Click += ButtonExample2_Click;
            tableLayout.Controls.Add(buttonExample2);

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            var buttonExample3 = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Font = new Font("Arial", 10),
                Text = "Example 3: has progress report, has cancel button",
            };
            buttonExample3.Click += ButtonExample3_Click;
            tableLayout.Controls.Add(buttonExample3);

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        }

        private async void ButtonExample1_Click(object sender, EventArgs e)
        {
            int n = 50;
            var result = await RunTask("Example 1", "No progress report, please wait for 5 seconds.", n, 0, false);
            MessageBox.Show("The " + n + "th Fibonacci number is " + result, "Finished!");
        }

        private async void ButtonExample2_Click(object sender, EventArgs e)
        {
            int n = 50;
            var result = await RunTask("Example 2", "", n, n, false);
            MessageBox.Show("The " + n + "th Fibonacci number is " + result, "Finished!");
        }

        private async void ButtonExample3_Click(object sender, EventArgs e)
        {
            int n = 50;
            var result = await RunTask("Example 3", "", n, n, true);
            if(result > 0)
                MessageBox.Show("The " + n + "th Fibonacci number is " + result, "Finished!");
            else
                MessageBox.Show("The calculation has been cancelled.", "Cancelled!");
        }

        public async Task<long> RunTask(string info1, string info2, int n, int maxProgress, bool isCancellable)
        {
            long result;
            using (var formProgress = new FormProgress(info1, info2, maxProgress, isCancellable))
            {
                formProgress.Show();

                result = await Task.Run(() => Fibonacci(n,
                     maxProgress > 0 ? formProgress.Progress : null,
                     isCancellable ? formProgress.CancellationTokenSource : null));

                formProgress.Close();
            }

            return result;
        }

        public static long Fibonacci(int len, IProgress<string> progress, CancellationTokenSource cancellationTokenSource)
        {
            long a = 0, b = 1, c = 0;
            for (int i = 2; i < len; i++)
            {
                if (cancellationTokenSource != null && cancellationTokenSource.Token.IsCancellationRequested)
                    return 0;

                // For demonstration only
                Thread.Sleep(100);

                c = a + b;
                a = b;
                b = c;

                if (progress != null)
                    progress.Report(i.ToString() + "|" + c.ToString());
            }

            return c;
        }
    }
}

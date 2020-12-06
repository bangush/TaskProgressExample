using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            var buttonExample1 = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Font = new Font("Arial", 10),
                Text = "Example 1: no progress report, no cancel button",
            };
            buttonExample1.Click += ButtonExample1_Click;
            tableLayout.Controls.Add(buttonExample1);

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            var buttonExample2 = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Font = new Font("Arial", 10),
                Text = "Example 2: has progress report, no cancel button",
            };
            buttonExample2.Click += ButtonExample2_Click;
            tableLayout.Controls.Add(buttonExample2);

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
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
            long result;
            using (var formProgress = new FormProgress(this, "Example 1", "No progress report, please wait for 5 seconds.", false, false))
            {
                formProgress.Show();
                result = await formProgress.RunTask(() => Fibonacci(n));
            }
            MessageBox.Show("The " + n + "th Fibonacci number is " + result, "Finished!");
        }

        public static long Fibonacci(int len)
        {
            long a = 0, b = 1, c = 0;
            for (int i = 2; i < len; i++)
            {
                // For demonstration only
                Thread.Sleep(100);

                c = a + b;
                a = b;
                b = c;
            }

            return c;
        }

        private async void ButtonExample2_Click(object sender, EventArgs e)
        {
            int n = 50;
            long result;
            using (var formProgress = new FormProgress(this, "Example 2", "", true, false))
            {
                formProgress.ProgressBar.Maximum = n;
                formProgress.Show();
                result = await formProgress.RunTask(() => Fibonacci(n, formProgress.Progress));
            }
            MessageBox.Show("The " + n + "th Fibonacci number is " + result, "Finished!");
        }

        public static long Fibonacci(int len, IProgress<string> progress)
        {
            long a = 0, b = 1, c = 0;
            for (int i = 2; i < len; i++)
            {
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

        private async void ButtonExample3_Click(object sender, EventArgs e)
        {
            int n = 50;
            long result;
            using (var formProgress = new FormProgress(this, "Example 3", "", true, true))
            {
                formProgress.ProgressBar.Maximum = n;
                formProgress.Show();
                result = await formProgress.RunTask(() => Fibonacci(n, formProgress.Progress, formProgress.CancellationTokenSource.Token));
            }
            if(result > 0)
                MessageBox.Show("The " + n + "th Fibonacci number is " + result, "Finished!");
            else
                MessageBox.Show("The calculation has been cancelled.", "Cancelled!");
        }

        public static long Fibonacci(int len, IProgress<string> progress, CancellationToken token)
        {
            long a = 0, b = 1, c = 0;
            for (int i = 2; i < len; i++)
            {
                if (token.IsCancellationRequested)
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

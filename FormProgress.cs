using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskProgressExample
{
    public partial class FormProgress : Form
    {
        private Label LabelInfo1;
        private Label LabelInfo2;
        private ProgressBar ProgressBar;
        private Form parentForm;

        private int maxProgress;

        public Progress<string> Progress;
        public CancellationTokenSource CancellationTokenSource;

        public FormProgress(Form parentForm, string info1, string info2, int maxProgress, bool isCancellable)
        {
            InitializeComponent();

            this.Size = new Size(500, 160);
            this.ShowInTaskbar = false;
            this.Text = "";
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.Shown += FormProgress_Shown;

            this.parentForm = parentForm;
            this.maxProgress = maxProgress;

            CancellationTokenSource = new CancellationTokenSource();

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
                Padding = new Padding(20),
            };
            Controls.Add(tableLayout);

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            LabelInfo1 = new Label
            {
                Dock = DockStyle.Fill,
                Text = info1,
            };
            tableLayout.Controls.Add(LabelInfo1);

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            LabelInfo2 = new Label
            {
                Dock = DockStyle.Fill,
                Text = info2,
            };
            tableLayout.Controls.Add(LabelInfo2);

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            ProgressBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Maximum = maxProgress > 0 ? maxProgress : 100,
                Style = maxProgress > 0 ? ProgressBarStyle.Blocks : ProgressBarStyle.Marquee,
            };
            tableLayout.Controls.Add(ProgressBar);

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            var ButtonCancel = new Button
            {
                Anchor = AnchorStyles.None,
                Text = isCancellable ? "Cancel" : "Please wait",
                Enabled = isCancellable
            };
            ButtonCancel.Click += (s, e) => { CancellationTokenSource.Cancel(); };
            tableLayout.Controls.Add(ButtonCancel);
        }

        private void FormProgress_Shown(object sender, System.EventArgs e)
        {
            this.Left = parentForm.Left + parentForm.Width / 2 - this.Width / 2;
            this.Top = parentForm.Top + parentForm.Height / 2 - this.Height / 2;
        }

        public async Task<long> RunTask(Func<long> action)
        {
            if (maxProgress > 0)
            {
                Progress = new Progress<string>(info =>
                {
                    var segs = info.Split('|');
                    ProgressBar.Value = int.Parse(segs[0]);
                    if (segs.Length > 1)
                        LabelInfo2.Text = segs[0] + ": " + segs[1];
                });
            }

            var result = await Task<long>.Run(action);

            this.Close();

            return result;
        }
    }
}
using System;
using System.Windows.Forms;

namespace Simulador_FIFO___LIFO
{
    public partial class MainForm : Form
    {
        private TabControl tabControl;
        private TabPage fifotab;
        private TabPage lifotab; 
        public MainForm()
        {
            InitializeComponent();

            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Simulador Interactivo FIFO & LIFO";
            this.Size = new System.Drawing.Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(28, 28, 28);

            fifotab = new TabPage("FIFO ");
            lifotab = new TabPage("LIFO");

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new System.Drawing.Font("Segoe UI", 12F);

            FIFOSimulatorPanel fifoPanel = new FIFOSimulatorPanel();
            fifoPanel.Dock = DockStyle.Fill;
            fifotab.Controls.Add(fifoPanel);

            LIFOSimulatorPanel lifoPanel = new LIFOSimulatorPanel();
            lifoPanel.Dock = DockStyle.Fill;
            lifotab.Controls.Add(lifoPanel);

            tabControl.TabPages.Add(fifotab);
            tabControl.TabPages.Add(lifotab);

            this.Controls.Add(tabControl);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}

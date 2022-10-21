namespace Test
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        private void businessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BizContacts frm = new BizContacts(); //Make new business contacts form
            frm.MdiParent = this;
            frm.Show();
        }

        private void tileHorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
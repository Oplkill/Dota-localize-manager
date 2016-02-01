using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DotaLocalizerManager
{
    public partial class OpenFilesDialogForm : Form
    {
        private List<string> paths;
        private bool canceled = false;

        public OpenFilesDialogForm()
        {
            InitializeComponent();
            paths = new List<string>();
        }

        //Okey
        private void button4_Click(object sender, EventArgs e)
        {
            if (paths.Count <= 1)
            {
                MessageBox.Show("Need minimum 2 files!");
                return;
            }

            this.Hide();
        }

        //Cancel
        private void button3_Click(object sender, EventArgs e)
        {
            canceled = true;
            this.Hide();
        }

        /// <summary>
        /// Button add
        /// </summary>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string path = "";

            openFileDialog1.ShowDialog();
            path = openFileDialog1.FileName;

            if (paths.Contains(path))
            {
                MessageBox.Show("This file already in list!");
                return;
            }
            paths.Add(path);
            listBox1.Items.Add(path);
        }

        /// <summary>
        /// Button remove
        /// </summary>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex == -1)
                return;

            int itemId = listBox1.SelectedIndex;
            paths.RemoveAt(itemId);
            listBox1.Items.RemoveAt(itemId);
        }

        private void OpenFilesDialogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                button3_Click(null, null);
                e.Cancel = true;
            }
        }

        public static FileOpener ShowAndGet()
        {
            FileOpener fileOpener = new FileOpener();

            OpenFilesDialogForm dialog = new OpenFilesDialogForm();
            dialog.ShowDialog();

            if (dialog.canceled)
                return null;

            foreach (var path in dialog.paths)
            {
                fileOpener.LoadFile(path);
            }

            dialog.Close();

            return fileOpener;
        }

        
    }
}

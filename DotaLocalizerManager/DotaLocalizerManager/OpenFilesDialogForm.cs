using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DotaLocalizerManager.Properties;

namespace DotaLocalizerManager
{
    public partial class OpenFilesDialogForm : Form
    {
        private List<string> paths;
        private List<bool> lockedList; 
        private bool canceled = false;

        public OpenFilesDialogForm()
        {
            InitializeComponent();
            paths = new List<string>();
            lockedList = new List<bool>();
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
                MessageBox.Show(Resources.ErrorFileInListExist);
                return;
            }
            paths.Add(path);
            lockedList.Add(false);
            listBox1.Items.Add(path);
        }

        /// <summary>
        /// Button remove
        /// </summary>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int itemId = listBox1.SelectedIndex;
            if (itemId == -1)
                return;

            paths.RemoveAt(itemId);
            lockedList.RemoveAt(itemId);
            listBox1.Items.RemoveAt(itemId);
        }

        /// <summary>
        /// Button lock
        /// </summary>
        private void buttonLock_Click(object sender, EventArgs e)
        {
            int itemId = listBox1.SelectedIndex;
            if (itemId == -1)
                return;

            lockedList[itemId] = !lockedList[itemId];
            listBox1.Items[itemId] = ((lockedList[itemId]) ? "[locked] - " : "") + paths[itemId];
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

            for (int i = 0; i < dialog.paths.Count; i++)
            {
                fileOpener.LoadFile(dialog.paths[i], dialog.lockedList[i]);
            }

            dialog.Close();

            return fileOpener;
        }

        
    }
}

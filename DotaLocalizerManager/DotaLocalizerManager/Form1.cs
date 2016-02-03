using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DotaLocalizerManager.Properties;
using KV3Lib;

namespace DotaLocalizerManager
{
    public partial class Form1 : Form
    {
        private DataBase db;
        private int firstFile = 0;
        private int secondFile = 1;
        private bool loading = false;

        public Form1()
        {
            InitializeComponent();
        }

#region TopMenus

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loading = true;

            clearAll();
            db = new DataBase();
            var fileOpener = OpenFilesDialogForm.ShowAndGet();

            if (fileOpener == null)
                return;

            db.FileOpener = fileOpener;
            db.FileOpener.CompareAllFiles();
            loadTable();
            loadComboBoxes();

            loading = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (db == null)
                return;

            db.SaveFiles();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loading = true;

            clearAll();

            loading = false;
        }

        #endregion

        #region SecondMenu

        /// <summary>
        /// Button Switch
        /// </summary>
        private void buttonSwitchLangs_Click(object sender, EventArgs e)
        {
            if (editing)
                return;

            if (comboBoxFile1.Items.Count == 0 || comboBoxFile2.Items.Count == 0 || 
                comboBoxFile1.SelectedIndex == -1 || comboBoxFile2.SelectedIndex == -1)
                return;

            loading = true;

            comboBoxFile1.SelectedIndex = secondFile;
            comboBoxFile2.SelectedIndex = firstFile;
            firstFile = comboBoxFile1.SelectedIndex;
            secondFile = comboBoxFile2.SelectedIndex;

            loading = false;

            updateTable();
        }

        /// <summary>
        /// Button Save
        /// </summary>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(null, null);
        }

        /// <summary>
        /// ComboBoxFile1. Changed
        /// </summary>
        private void comboBoxFile1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading)
                return;

            if (comboBoxFile1.SelectedIndex == firstFile)
                return;

            if (comboBoxFile1.SelectedIndex == comboBoxFile2.SelectedIndex)
            {
                comboBoxFile1.SelectedIndex = firstFile;
                return;
            }

            firstFile = comboBoxFile1.SelectedIndex;
            updateTable();
        }

        /// <summary>
        /// ComboBoxFile2. Changed
        /// </summary>
        private void comboBoxFile2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading)
                return;

            if (comboBoxFile2.SelectedIndex == secondFile)
                return;

            if (comboBoxFile1.SelectedIndex == comboBoxFile2.SelectedIndex)
            {
                comboBoxFile2.SelectedIndex = secondFile;
                return;
            }

            secondFile = comboBoxFile2.SelectedIndex;
            updateTable();
        }

        /// <summary>
        /// Button Add
        /// </summary>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (db == null)
                return;

            string tempKey = DateTime.Now.ToLongTimeString();
            if (db.FileOpener.Files[0].LocalizationKeys.HasKeyInChildren(tempKey))
                return;

            foreach (var file in db.FileOpener.Files)
            {
                file.LocalizationKeys.Children.Add(new KeyValue()
                {
                    Key = tempKey,
                    Value = "",
                    Parent = file.LocalizationKeys,
                });
            }
            var dtGrigTmp = new DataGridViewRow();
            dtGrigTmp.Cells.Add(new DataGridViewTextBoxCell());
            dtGrigTmp.Cells.Add(new DataGridViewTextBoxCell());
            dtGrigTmp.Cells.Add(new DataGridViewTextBoxCell());
            dtGrigTmp.Cells[0].Value = tempKey;
            dtGrigTmp.Cells[1].Value = "";
            dtGrigTmp.Cells[2].Value = "";
            dataGridView1.Rows.Add(dtGrigTmp);
            db.Edited = true;
        }

        /// <summary>
        /// Button remove
        /// </summary>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
                return;

            int collId = dataGridView1.SelectedCells[0].ColumnIndex;
            int rowId = dataGridView1.SelectedCells[0].RowIndex;

            string key = dataGridView1.Rows[rowId].Cells[0].Value.ToString();
            foreach (var file in db.FileOpener.Files)
            {
                int i = file.LocalizationKeys.FindChildrenId(key);
                file.LocalizationKeys.Children.RemoveAt(i);
            }
            dataGridView1.Rows.RemoveAt(rowId);
            db.Edited = true;
        }

        #endregion

        private bool exit()
        {
            if (db == null)
                return false;

            if (db.Edited)
            {
                var mbox = MessageBox.Show("Save before exit?", "Not saved", MessageBoxButtons.YesNoCancel);
                switch (mbox)
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(null, null);
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        return true;
                }
            }

            return false;
        }

        private void clearAll()
        {
            comboBoxFile1.Items.Clear();
            comboBoxFile2.Items.Clear();
            dataGridView1.Rows.Clear();
            db = null;
        }

        private void loadComboBoxes()
        {
            foreach (var file in db.FileOpener.Files)
            {
                comboBoxFile1.Items.Add(file.Language);
                comboBoxFile2.Items.Add(file.Language);
            }
            comboBoxFile1.SelectedIndex = firstFile;
            comboBoxFile2.SelectedIndex = secondFile;
        }

        private void loadTable()
        {
            foreach (var kv in db.FileOpener.Files[firstFile].LocalizationKeys.Children)
            {
                var dtGrigTmp = new DataGridViewRow();
                dtGrigTmp.Cells.Add(new DataGridViewTextBoxCell());
                dtGrigTmp.Cells.Add(new DataGridViewTextBoxCell());
                dtGrigTmp.Cells.Add(new DataGridViewTextBoxCell());
                dtGrigTmp.Cells[0].Value = kv.Key;
                dtGrigTmp.Cells[1].Value = kv.Value;
                dtGrigTmp.Cells[2].Value = db.FileOpener.Files[secondFile].LocalizationKeys.FindChildren(kv.Key).Value;
                dataGridView1.Rows.Add(dtGrigTmp);
            }
        }

        private void updateTable()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string key = dataGridView1.Rows[i].Cells[0].Value.ToString();
                dataGridView1.Rows[i].Cells[1].Value =
                    db.FileOpener.Files[firstFile].LocalizationKeys.FindChildren(key).Value;
                dataGridView1.Rows[i].Cells[2].Value =
                    db.FileOpener.Files[secondFile].LocalizationKeys.FindChildren(key).Value;
            }
        }

        private string lastKey;
        private bool editing;
        /// <summary>
        /// Таблица. Редактирование ячейки начато
        /// </summary>
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            editing = true;
            if (e.ColumnIndex != 0)
                return;

            lastKey = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        /// <summary>
        /// Таблица. Редактирование ячейки завершено
        /// </summary>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (loading || !editing)
                return;

            if (e.ColumnIndex == 0)
            { // редактирование ключа
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = lastKey;
                    MessageBox.Show(Resources.ErrorEmptyKey);
                    return;
                }
                string newKey = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                if (lastKey == newKey)
                    return;

                if (db.FileOpener.Files.First().LocalizationKeys.HasKeyInChildren(newKey))
                {
                    MessageBox.Show(Resources.ErrorKeyAreNotFree);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = lastKey;
                    return;
                }
                if (newKey.Contains(" "))
                {
                    MessageBox.Show(Resources.ErrorKeyContainsSpacesOrOtherBlockedSymbols);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = lastKey;
                    return;
                }
                foreach (var file in db.FileOpener.Files)
                {
                    file.LocalizationKeys.FindChildren(lastKey).Key = newKey;
                }

                db.Edited = true;
                return;
            }

            // редактирование значения
            string key = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            string value = (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null) ? "" : dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            int fileId = (e.ColumnIndex == firstFile) ? firstFile : secondFile;

            if (db.FileOpener.Files[fileId].LocalizationKeys.FindChildren(key).Value != value)
            {
                db.FileOpener.Files[fileId].LocalizationKeys.FindChildren(key).Value = value;
                db.Edited = true;
            }

            editing = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opFile = new OpenFileDialog();
            opFile.ShowDialog();
            string path = opFile.FileName;
            string fileText = System.IO.File.ReadAllText(path);
            List<KeyValue> keys = (List<KeyValue>) KVParser.Parse(fileText);
            string str = keys[0].ToString();
        }
    }
}

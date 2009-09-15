using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Inquisitor.Db;
using log4net;
using System.IO;

namespace Inquisitor.Ui
{
    public partial class MainForm : Form
    {
        private static ILog log = LogManager.GetLogger(typeof(MainForm));
        private const int fetchSize = 500; // TODO: parameterise fetch size

        private SqlRunner sqlRunner;
        private DataTable dataGridModel = new DataTable();
        private SqlResult currentResult;
        private BindingSource binding;
        private bool gridEventsEnabled;

        public MainForm()
        {
            InitializeComponent();
            InitializeDataGrid();
            ClearStatus();
        }

        private void InitializeDataGrid()
        {
            binding = new BindingSource();
            binding.DataSource = dataGridModel;
            dataGrid.DataSource = binding;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sqlRunner != null) sqlRunner.Dispose(); // TODO: do this on any type of app close
            Application.Exit();
        }


        /// <summary>
        /// Find current active block. Active block is:
        /// 1. currently highlighted text (not implemented yet); or
        /// 2. text around current cursor up to a blank line on each side
        /// </summary>
        /// <returns>current active block in the code box</returns>
        private string FindCurrentBlock()
        {
            int pos = codeBox.CurrentPos;
            int blockStart = pos == 0 ? 0 : pos - 1;
            int blockEnd = pos;
            int state = 0;
            while (blockStart > 0)
            {
                switch (codeBox.Text[blockStart]) 
                {
                    case '\n': state = (state % 2 == 0) ? state + 1 : 0; break;
                    case '\r': state = (state % 2 == 1) ? state + 1 : 0; break;
                    case ' ': break;
                    case '\t': break;
                    default: state = 0; break;
                }
                if (state == 4) break;
                blockStart--;
            }
            state = 0;
            while (blockEnd < codeBox.TextLength)
            {
                switch (codeBox.Text[blockEnd])
                {
                    case '\n': state = (state % 2 == 1) ? state + 1 : 0; break;
                    case '\r': state = (state % 2 == 0) ? state + 1 : 0; break;
                    case ' ': break;
                    case '\t': break;
                    default: state = 0; break;
                }
                if (state == 4) break;
                blockEnd++;
            }
            return blockStart >= 0 ? codeBox.Text.Substring(blockStart, blockEnd - blockStart).Trim() : null;
        }

        private void codeBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.Enter)
            {
                ExecuteCurrentBlock();
            }
        }

        private void ExecuteCurrentBlock()
        {
            Cursor = Cursors.WaitCursor;
            string currentBlock = FindCurrentBlock();
            SetStatusMarquee("running query...");
            SqlResult result = sqlRunner.RunSql(currentBlock);
            ClearStatus();
            if (result.Exception != null)
            {
                MessageBox.Show(result.Exception.Message);
            }
            if (result.Message != null)
            {
                SetStatus(result.Message);
            }
            if (result.HasData)
            {
                // set columns
                binding.DataSource = null; // adding columns while bound to the view is very slow. Do the work and then re-bind
                dataGridModel.Clear();
                dataGridModel.Columns.Clear();
                dataGridModel.Columns.AddRange(result.Columns);
                // populate
                if (currentResult != null) currentResult.Dispose();
                currentResult = result;
                LoadNextBatchOfRows();
                binding.DataSource = dataGridModel; // re-bind
                dataGrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            }
            Cursor = Cursors.Default;
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectDialog connDlg = new ConnectDialog();
            if (sqlRunner != null) connDlg.ConnectionDetails = sqlRunner.ConnectionDetails;
            connDlg.ShowDialog();
            if (connDlg.ConnectionDetails != null && (sqlRunner == null || !sqlRunner.ConnectionDetails.Equals(connDlg.ConnectionDetails)))
            {
                if (sqlRunner != null) sqlRunner.Dispose();
                try
                {
                    sqlRunner = new SqlRunner(connDlg.ConnectionDetails);
                    tabConnections.TabPages[0].BackColor = sqlRunner.ConnectionDetails.FrameColour;
                    tabConnections.TabPages[0].Text = sqlRunner.ConnectionDetails.Name;
                }
                catch (Oracle.DataAccess.Client.OracleException ex) // TODO: Db to provide its own exceptions
                {
                    log.Error("exception when connecting: ", ex);
                    MessageBox.Show(ex.Message, "Unable to connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dlgExport.ShowDialog() != DialogResult.OK) return;
            string fileName = dlgExport.FileName;
         
            // TODO: this only exports the visible rows. Do we want to force loading of all results?
            Cursor = Cursors.WaitCursor;
            SetStatus("exporting to " + fileName + "...", dataGridModel.Rows.Count);
            
            TextWriter tw = new StreamWriter(fileName);

            // write column names
            bool first = true;
            foreach (DataColumn col in dataGridModel.Columns)
            {
                if (first) first = false; else tw.Write(",");
                tw.Write(col.ColumnName);
            }
            tw.WriteLine();
            
            // write data
            foreach (DataRow row in dataGridModel.Rows) 
            {
                first = true;
                foreach (DataColumn col in dataGridModel.Columns)
                {
                    if (first) first = false; else tw.Write(",");
                    tw.Write(row[col]);
                }
                tw.WriteLine();
                prbProgress.Value++;
            }
            tw.Close();
            Cursor = Cursors.Default;
            ClearStatus();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            codeBox.UndoRedo.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            codeBox.UndoRedo.Redo();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            codeBox.FindReplace.ShowFind();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            codeBox.FindReplace.ShowReplace();
        }

        private void aboutInquisitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        private void dataGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (gridEventsEnabled && dataGridModel.Rows.Count - e.RowIndex < 20 && currentResult.HasMoreRows)
            {
                LoadNextBatchOfRows();
            }
        }

        private void dataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            // FIXME: doesn't work well if the scrolling rectangle is dragged -- seems like in that situation the handler is invoked repeatedly
            int firstRow = dataGrid.FirstDisplayedScrollingRowIndex;
            if (gridEventsEnabled && e.ScrollOrientation == ScrollOrientation.VerticalScroll && dataGridModel.Rows.Count - firstRow < 50 && currentResult.HasMoreRows)
            {
                LoadNextBatchOfRows();
            }
        }

        private void LoadNextBatchOfRows()
        {
            Cursor = Cursors.WaitCursor;
            gridEventsEnabled = false;
            SetStatus("loading rows...", fetchSize);
            IList<object[]> rows = currentResult.GetRows(fetchSize); 
            foreach (object[] row in rows)
            {
                dataGridModel.LoadDataRow(row, LoadOption.OverwriteChanges);
                prbProgress.Value++;
            }
            ClearStatus();
            gridEventsEnabled = true;
            Cursor = Cursors.Default;
        }

        private void SetStatus(string message)
        {
            lblStatus.Text = message;
            prbProgress.Visible = false;
            statusStrip.Refresh();
        }

        private void SetStatus(string message, int maxProgress)
        {
            lblStatus.Text = message;
            prbProgress.Style = ProgressBarStyle.Continuous;
            prbProgress.Maximum = maxProgress;
            prbProgress.Value = 0;
            prbProgress.Visible = true;
            statusStrip.Refresh();
        }

        private void SetStatusMarquee(string message)
        {
            lblStatus.Text = message;
            prbProgress.Style = ProgressBarStyle.Marquee;
            prbProgress.Visible = true;
            statusStrip.Refresh();
        }

        private void ClearStatus()
        {
            SetStatus("");
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(dataGrid.GetClipboardContent());
        }

    }
}

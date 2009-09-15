using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Inquisitor.Db;
using System.Collections;
using Inquisitor.Properties;

namespace Inquisitor.Ui
{
    public partial class ConnectDialog : Form
    {
        private ConnectionDetails connDetails;

        public ConnectionDetails ConnectionDetails 
        {
            get
            {
                return connDetails;
            }
            set
            {
                connDetails = value;
                UpdateConnDetailsForm();
            }
        }

        private void UpdateConnDetailsForm()
        {
            txtName.Text = connDetails.Name;
            txtHost.Text = connDetails.Host;
            txtPort.Text = connDetails.Port.ToString();
            txtSid.Text = connDetails.Sid;
            txtUser.Text = connDetails.User;
            txtPassword.Text = connDetails.Password;
        }

        public ConnectDialog()
        {
            InitializeComponent();
            if (Settings.Default.Connections == null) Settings.Default.Connections = new SortedList();
            PopulateConnectionList();
        }

        private void PopulateConnectionList()
        {
            SortedList conns = Settings.Default.Connections;
            foreach (string key in conns.Keys)
            {
                ConnectionDetails cd = (ConnectionDetails)conns[key];
                ListViewItem item = new ListViewItem(new string[] { cd.Name, cd.Host, cd.Port.ToString(), cd.Sid, cd.User });
                item.BackColor = cd.FrameColour;
                item.Tag = key;
                lstConnections.Items.Add(item);
            }
            lstConnections.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            connDetails = null;
            Close();
        }

        private void lstConnections_DoubleClick(object sender, EventArgs e)
        {
            SetConnectionDetailsFromSelectedItem();
            Connect();
        }

        private void Connect()
        {
            // TODO: colour and prescript
            connDetails = new ConnectionDetails(txtName.Text, txtHost.Text, int.Parse(txtPort.Text), txtSid.Text, txtUser.Text, txtPassword.Text, null, Color.Transparent);
            Settings.Default.Connections.Remove(connDetails.Name);
            Settings.Default.Connections.Add(connDetails.Name, connDetails);
            Settings.Default.Save();
            Close();
        }

        private void SetConnectionDetailsFromSelectedItem()
        {
            if (lstConnections.SelectedItems.Count == 1)
            {
                ConnectionDetails = (ConnectionDetails)Settings.Default.Connections[lstConnections.SelectedItems[0].Tag];
            }
        }

        private void lstConnections_Click(object sender, EventArgs e)
        {
            SetConnectionDetailsFromSelectedItem();
        }

    }
}

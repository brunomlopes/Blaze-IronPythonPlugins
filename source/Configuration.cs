﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IronPythonPlugins
{
    public partial class Configuration : Form
    {
        private readonly ConfigurationObject _originalConfiguration;

        public class ConfigurationObject
        {
            public ConfigurationObject()
            {
                IronPythonScriptPaths = new string[] {};
            }

            public string[] IronPythonScriptPaths { get; set; }
        }
        public Configuration(ConfigurationObject originalConfiguration)
        {
            _originalConfiguration = originalConfiguration;
            InitializeComponent();
            Paths.Items.AddRange(originalConfiguration.IronPythonScriptPaths);
        }

        public ConfigurationObject ShowConfigurationDialog()
        {
            if (this.ShowDialog() == DialogResult.OK)
            {
                return new ConfigurationObject()
                           {
                               IronPythonScriptPaths = Paths.Items.OfType<string>().ToArray()
                           };
            }
            else
            {
                return _originalConfiguration;
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            var newPath = PathToAdd.Text;
            if (string.IsNullOrEmpty(newPath))
            {
                MessageBox.Show("Please add a directory (press the  b' button to browse to it).", "Error", MessageBoxButtons.OK);
            }
            else if(!System.IO.Directory.Exists(newPath))
            {
                if (System.IO.File.Exists(newPath))
                {
                    MessageBox.Show(string.Format("Path '{0}' is a file. Please add a directory.", newPath), "Error",
                                    MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show(
                        string.Format("Path '{0}' doesn't exist. Please browse to an existing path", newPath), "Error",
                        MessageBoxButtons.OK);
                }
            }
            else
            {
                Paths.Items.Add(newPath);
            }
        }

        private void Paths_Click(object sender, EventArgs e)
        {
            
        }

        private void Browser_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            if(System.IO.Directory.Exists(PathToAdd.Text))
            {
                folderBrowserDialog.SelectedPath = PathToAdd.Text;
            }
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                PathToAdd.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void deleteToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Paths.SelectedItem != null)
            {
                Paths.Items.RemoveAt(Paths.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select a path to delete");
            }
        }
    }
}

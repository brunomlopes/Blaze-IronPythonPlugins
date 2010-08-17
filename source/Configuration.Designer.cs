namespace IronPythonPlugins
{
    partial class Configuration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Paths = new System.Windows.Forms.ListBox();
            this.Add = new System.Windows.Forms.Button();
            this.PathToAdd = new System.Windows.Forms.TextBox();
            this.Browser = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.Ok = new System.Windows.Forms.Button();
            this.PathsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PathsContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // Paths
            // 
            this.Paths.ContextMenuStrip = this.PathsContextMenuStrip;
            this.Paths.FormattingEnabled = true;
            this.Paths.Location = new System.Drawing.Point(12, 32);
            this.Paths.Name = "Paths";
            this.Paths.Size = new System.Drawing.Size(263, 355);
            this.Paths.TabIndex = 0;
            this.Paths.Click += new System.EventHandler(this.Paths_Click);
            // 
            // Add
            // 
            this.Add.Location = new System.Drawing.Point(253, 3);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(22, 23);
            this.Add.TabIndex = 1;
            this.Add.Text = "+";
            this.Add.UseVisualStyleBackColor = true;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // PathToAdd
            // 
            this.PathToAdd.Location = new System.Drawing.Point(13, 5);
            this.PathToAdd.Name = "PathToAdd";
            this.PathToAdd.Size = new System.Drawing.Size(206, 20);
            this.PathToAdd.TabIndex = 2;
            // 
            // Browser
            // 
            this.Browser.Location = new System.Drawing.Point(225, 3);
            this.Browser.Name = "Browser";
            this.Browser.Size = new System.Drawing.Size(22, 23);
            this.Browser.TabIndex = 3;
            this.Browser.Text = "b";
            this.Browser.UseVisualStyleBackColor = true;
            this.Browser.Click += new System.EventHandler(this.Browser_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(145, 394);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(62, 23);
            this.Cancel.TabIndex = 5;
            this.Cancel.Text = "cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Ok
            // 
            this.Ok.Location = new System.Drawing.Point(213, 394);
            this.Ok.Name = "Ok";
            this.Ok.Size = new System.Drawing.Size(62, 23);
            this.Ok.TabIndex = 6;
            this.Ok.Text = "ok";
            this.Ok.UseVisualStyleBackColor = true;
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // ContextMenuStrip
            // 
            this.PathsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.PathsContextMenuStrip.Name = "PathsContextMenuStrip";
            this.PathsContextMenuStrip.Size = new System.Drawing.Size(153, 48);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.deleteToolStripMenuItem_DropDownItemClicked);
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // Configuration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 423);
            this.Controls.Add(this.Ok);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Browser);
            this.Controls.Add(this.PathToAdd);
            this.Controls.Add(this.Add);
            this.Controls.Add(this.Paths);
            this.Name = "Configuration";
            this.Text = "Configuration";
            this.PathsContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox Paths;
        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.TextBox PathToAdd;
        private System.Windows.Forms.Button Browser;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Ok;
        private System.Windows.Forms.ContextMenuStrip PathsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}
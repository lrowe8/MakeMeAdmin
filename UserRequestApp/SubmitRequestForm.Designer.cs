// 
// Copyright © 2010-2019, Sinclair Community College
// Licensed under the GNU General Public License, version 3.
// See the LICENSE file in the project root for full license information.  
//
// This file is part of Make Me Admin.
//
// Make Me Admin is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// Make Me Admin is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Make Me Admin. If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SinclairCC.MakeMeAdmin
{
    /// <summary>
    /// This form allows the user to submit a request for administrator-level rights.
    /// </summary>
    internal partial class SubmitRequestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// The "add me" button.
        /// </summary>
        private System.Windows.Forms.Button requestButton;

        /// <summary>
        /// A tooltip to explain other controls.
        /// </summary>
        private System.Windows.Forms.ToolTip toolTip;

        /// <summary>
        /// The "remove me" button.
        /// </summary>
        private System.Windows.Forms.Button cancelButton;

        /// <summary>
        /// A background worker to control the state of the various buttons.
        /// </summary>
        private System.ComponentModel.BackgroundWorker buttonStateWorker;

        /// <summary>
        /// A background workr to add the current user to the Administrators group.
        /// </summary>
        private System.ComponentModel.BackgroundWorker addUserBackgroundWorker;

        /// <summary>
        /// A background workr to remove the current user from the Administrators group.
        /// </summary>
        private System.ComponentModel.BackgroundWorker removeUserBackgroundWorker;

        /// <summary>
        /// notification area icon
        /// </summary>
        private System.Windows.Forms.NotifyIcon notifyIcon;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubmitRequestForm));
            this.requestButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.buttonStateWorker = new System.ComponentModel.BackgroundWorker();
            this.addUserBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.removeUserBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.min5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.min10ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.min20ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hour1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hours2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.messageLabel = new System.Windows.Forms.Label();
            this.appStatus = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // requestButton
            // 
            resources.ApplyResources(this.requestButton, "requestButton");
            this.requestButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.requestButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.requestButton.Name = "requestButton";
            this.toolTip.SetToolTip(this.requestButton, resources.GetString("requestButton.ToolTip"));
            this.requestButton.UseVisualStyleBackColor = false;
            this.requestButton.Click += new System.EventHandler(this.ClickRequestButton);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(38)))), ((int)(((byte)(84)))));
            this.cancelButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cancelButton.Name = "cancelButton";
            this.toolTip.SetToolTip(this.cancelButton, resources.GetString("cancelButton.ToolTip"));
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.ClickCancelButton);
            // 
            // buttonStateWorker
            // 
            this.buttonStateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoButtonStateWork);
            this.buttonStateWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ButtonStateWorkCompleted);
            // 
            // addUserBackgroundWorker
            // 
            this.addUserBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.addUserBackgroundWorker_DoWork);
            this.addUserBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.addUserBackgroundWorker_RunWorkerCompleted);
            // 
            // removeUserBackgroundWorker
            // 
            this.removeUserBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.removeUserBackgroundWorker_DoWork);
            this.removeUserBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.removeUserBackgroundWorker_RunWorkerCompleted);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            resources.ApplyResources(this.notifyIcon, "notifyIcon");
            this.notifyIcon.BalloonTipClosed += new System.EventHandler(this.notifyIcon_BalloonTipClosed);
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.ForeColor = System.Drawing.Color.White;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.min5ToolStripMenuItem,
            this.min10ToolStripMenuItem,
            this.min20ToolStripMenuItem,
            this.hour1ToolStripMenuItem,
            this.hours2ToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            // 
            // min5ToolStripMenuItem
            // 
            this.min5ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.min5ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.min5ToolStripMenuItem.Name = "min5ToolStripMenuItem";
            resources.ApplyResources(this.min5ToolStripMenuItem, "min5ToolStripMenuItem");
            this.min5ToolStripMenuItem.Click += new System.EventHandler(this.MinToolStripMenuItem_Click);
            // 
            // min10ToolStripMenuItem
            // 
            this.min10ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.min10ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.min10ToolStripMenuItem.Name = "min10ToolStripMenuItem";
            resources.ApplyResources(this.min10ToolStripMenuItem, "min10ToolStripMenuItem");
            this.min10ToolStripMenuItem.Click += new System.EventHandler(this.MinToolStripMenuItem_Click);
            // 
            // min20ToolStripMenuItem
            // 
            this.min20ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.min20ToolStripMenuItem.Checked = true;
            this.min20ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.min20ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.min20ToolStripMenuItem.Name = "min20ToolStripMenuItem";
            resources.ApplyResources(this.min20ToolStripMenuItem, "min20ToolStripMenuItem");
            this.min20ToolStripMenuItem.Click += new System.EventHandler(this.MinToolStripMenuItem_Click);
            // 
            // hour1ToolStripMenuItem
            // 
            this.hour1ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.hour1ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.hour1ToolStripMenuItem.Name = "hour1ToolStripMenuItem";
            resources.ApplyResources(this.hour1ToolStripMenuItem, "hour1ToolStripMenuItem");
            this.hour1ToolStripMenuItem.Click += new System.EventHandler(this.MinToolStripMenuItem_Click);
            // 
            // hours2ToolStripMenuItem
            // 
            this.hours2ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.hours2ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.hours2ToolStripMenuItem.Name = "hours2ToolStripMenuItem";
            resources.ApplyResources(this.hours2ToolStripMenuItem, "hours2ToolStripMenuItem");
            this.hours2ToolStripMenuItem.Click += new System.EventHandler(this.MinToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.aboutToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.imageList1, "imageList1");
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SinclairCC.MakeMeAdmin.Properties.Resources.Locked_Img;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // messageLabel
            // 
            resources.ApplyResources(this.messageLabel, "messageLabel");
            this.messageLabel.ForeColor = System.Drawing.Color.White;
            this.messageLabel.Name = "messageLabel";
            // 
            // appStatus
            // 
            this.appStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            resources.ApplyResources(this.appStatus, "appStatus");
            this.appStatus.ForeColor = System.Drawing.Color.White;
            this.appStatus.Name = "appStatus";
            // 
            // SubmitRequestForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.Controls.Add(this.appStatus);
            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.requestButton);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubmitRequestForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.FormLoad);
            this.VisibleChanged += new System.EventHandler(this.SubmitRequestForm_VisibleChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SubmitForm_MouseDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

      #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem min5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem min10ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem min20ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hour1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hours2ToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label messageLabel;
        private Label appStatus;
    }
}


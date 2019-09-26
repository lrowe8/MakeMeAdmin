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

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Reflection;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Windows.Forms;

    /// <summary>
    /// This form allows the user to submit a request for administrator-level rights.
    /// </summary>
    internal partial class SubmitRequestForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        /// <summary>
        /// Whether the user is a direct member of the Administrator's group.
        /// </summary>
        /// <remarks>
        /// This is stored in a variable because it is a rather expensive operation to check.
        /// </remarks>
        private bool userIsDirectAdmin = false;

        /// <summary>
        /// Whether the user is a member of the Administrator's group, either directly or
        /// via nested group memberships.
        /// </summary>
        /// <remarks>
        /// This is stored in a variable because it is a rather expensive operation to check.
        /// </remarks>
        private bool userIsAdmin = false;

        /// <summary>
        /// Whether the user had administrator rights the last time the check was performed.
        /// </summary>
        /// <remarks>
        /// This is used to determine when the user's administrator status changes.
        /// </remarks>
        private bool userWasAdminOnLastCheck = false;

        /// <summary>
        /// Variable that is used to store the admin expiration time
        /// </summary>
        private DateTime expireTime = DateTime.MinValue;

        /// <summary>
        /// Timer to notify users when their administrator rights expire.
        /// </summary>
        private System.Timers.Timer notifyIconTimer;


        /// <summary>
        /// Initializes a new instance of the SubmitRequestForm class.
        /// </summary>
        public SubmitRequestForm()
        {
            this.InitializeComponent();

            this.SetFormText();

            // Configure the notification timer.
            this.notifyIconTimer = new System.Timers.Timer()
            {
                Interval = 5000
            };
            this.notifyIconTimer.AutoReset = true;
            this.notifyIconTimer.Elapsed += NotifyIconTimerElapsed;
        }


        /// <summary>
        /// Handles the Elapsed event for the notification area icon.
        /// </summary>
        /// <param name="sender">
        /// The timer whose Elapsed event is firing.
        /// </param>
        /// <param name="e">
        /// Data related to the event.
        /// </param>
        void NotifyIconTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.UpdateUserAdministratorStatus();
            TooltipUpdate();

            if (this.userIsAdmin != this.userWasAdminOnLastCheck)
            { // The user's administrator status has changed.

                NetNamedPipeBinding namedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
                ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(namedPipeBinding, Settings.NamedPipeServiceBaseAddress);
                IAdminGroup namedPipeChannel = namedPipeFactory.CreateChannel();

                this.userWasAdminOnLastCheck = this.userIsAdmin;

                if ((!this.userIsAdmin) && (!namedPipeChannel.UserIsInList()))
                {
                    this.notifyIconTimer.Stop();
                    notifyIcon.ShowBalloonTip(5000, Properties.Resources.ApplicationName, string.Format(Properties.Resources.UIMessageRemovedFromGroup, LocalAdministratorGroup.LocalAdminGroupName), ToolTipIcon.Info);
                    this.notifyIcon.Text = "Privileges 2.3.0";
                }

                namedPipeFactory.Close();
            }
        }

        /// <summary>
        /// Handles the admin countdown event for the notification area icon.
        /// </summary>
        /// <param name="sender">
        /// The timer whose Elapsed event is firing.
        /// </param>
        /// <param name="e">
        /// Data related to the event.
        /// </param>
        void TooltipUpdate()
        {
            DateTime now = DateTime.Now;

            if (now < expireTime)
            {
                String toolText = "Time Remaining: ";
                TimeSpan diff = expireTime.Subtract(now);

                if ((diff.Hours) > 0)
                {
                    toolText += (diff.Hours.ToString() + " hour(s) ");
                }

                if (diff.Minutes >= 1 || (toolText.Length == 0))
                {
                    toolText += (diff.Minutes.ToString() + " minute(s) ");
                }
                else
                {
                    toolText += (diff.Seconds.ToString() + " seconds");
                }

                this.notifyIcon.Text = toolText;
            }
        }

        /// <summary>
        /// Sets the form's text to the name of the application plus a partial version number.
        /// </summary>
        private void SetFormText()
        {
            System.Text.StringBuilder formText = new System.Text.StringBuilder();
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length == 0)
            {
                formText.Append(Properties.Resources.ApplicationName);
            }
            else
            {
                formText.Append(((AssemblyProductAttribute)attributes[0]).Product);
            }
            formText.Append(' ');
            formText.Append(Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

            this.Text = formText.ToString();
            this.notifyIcon.Text = formText.ToString();
        }


        /// <summary>
        /// This function handles the Click event for the Submit button.
        /// </summary>
        /// <param name="sender">
        /// The button being clicked.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void ClickRequestButton(object sender, EventArgs e)
        {
            String buttonText = this.requestButton.Text;
            this.DisableButtons();

            if (buttonText == "Request Privileges")
            {
                TooltipUpdate();
                this.appStatus.Text = string.Format(Properties.Resources.UIMessageAddingToGroup, LocalAdministratorGroup.LocalAdminGroupName);
                addUserBackgroundWorker.RunWorkerAsync();
            }
            else
            {
                this.appStatus.Text = string.Format(Properties.Resources.UIMessageRemovingFromGroup, LocalAdministratorGroup.LocalAdminGroupName);
                removeUserBackgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// This function checks to see which item in the edit menu is selected.
        /// </summary>
        private void SetExpireTime()
        {
            if (min5ToolStripMenuItem.Checked)
            {
                expireTime = DateTime.Now.AddMinutes(5);
            }
            else if (min10ToolStripMenuItem.Checked)
            {
                expireTime = DateTime.Now.AddMinutes(10);
            }
            else if (min20ToolStripMenuItem.Checked)
            {
                expireTime = DateTime.Now.AddMinutes(20);
            }
            else if (hour1ToolStripMenuItem.Checked)
            {
                expireTime = DateTime.Now.AddHours(1);
            }
            else if (hours2ToolStripMenuItem.Checked)
            {
                expireTime = DateTime.Now.AddHours(2);
            }
        }

        /// <summary>
        /// This function runs when RunWorkerAsync() is called by the "grant admin rights" BackgroundWorker object.
        /// </summary>
        /// <param name="sender">
        /// The BackgroundWorker that triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void addUserBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(binding, Settings.NamedPipeServiceBaseAddress);
            IAdminGroup channel = namedPipeFactory.CreateChannel();

            SetExpireTime();

            try
            {
                channel.AddUserToAdministratorsGroup(expireTime);
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
                // TODO: What do we do with this error?
            }

            namedPipeFactory.Close();
        }


        /// <summary>
        /// Occurs when the background operation has completed, has been canceled, or has raised an exception.
        /// </summary>
        /// <param name="sender">
        /// The "grant admin rights" BackgroundWorker object, which triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void addUserBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Text.StringBuilder message = new System.Text.StringBuilder(Properties.Resources.UIMessageErrorWhileAdding);
                message.Append(System.Environment.NewLine);
                message.Append(Properties.Resources.ErrorMessage);
                message.Append(": ");
                message.Append(e.Error.Message);

                MessageBox.Show(this, message.ToString(), Properties.Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, 0);
            }

            this.UpdateUserAdministratorStatus();

            if (this.userIsAdmin)
            { // Display a message that the user now has administrator rights.
                this.appStatus.Text = Properties.Resources.ApplicationIsReady;
                this.userWasAdminOnLastCheck = this.userIsAdmin;
                this.notifyIconTimer.Start();
                notifyIcon.Visible = true;
                this.Visible = false;
                this.ShowInTaskbar = false;
                notifyIcon.ShowBalloonTip(5000, Properties.Resources.ApplicationName, string.Format(Properties.Resources.UIMessageAddedToGroup, LocalAdministratorGroup.LocalAdminGroupName), ToolTipIcon.Info);
            }
            else if (!buttonStateWorker.IsBusy)
            {
                buttonStateWorker.RunWorkerAsync();
            }
        }


        /// <summary>
        /// Handles the Click event for the rights removal button.
        /// </summary>
        /// <param name="sender">
        /// The button being clicked.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void ClickCancelButton(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// This function runs when RunWorkerAsync() is called by the rights removal BackgroundWorker object.
        /// </summary>
        /// <param name="sender">
        /// The BackgroundWorker that triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void removeUserBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(binding, Settings.NamedPipeServiceBaseAddress);
            IAdminGroup channel = namedPipeFactory.CreateChannel();
            channel.RemoveUserFromAdministratorsGroup(RemovalReason.UserRequest);
            namedPipeFactory.Close();
        }


        /// <summary>
        /// Occurs when the background operation has completed, has been canceled, or has raised an exception.
        /// </summary>
        /// <param name="sender">
        /// The rights removal BackgroundWorker object, which triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void removeUserBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Text.StringBuilder message = new System.Text.StringBuilder(Properties.Resources.UIMessageErrorWhileRemoving);
                message.Append(System.Environment.NewLine);
                message.Append(Properties.Resources.UIMessageEnsureServiceRunning);
                message.Append(System.Environment.NewLine);
                message.Append(Properties.Resources.ErrorMessage);
                message.Append(": ");
                message.Append(e.Error.Message);
                message.Append(System.Environment.NewLine);
                message.Append(Properties.Resources.StackTrace);
                message.Append(": ");
                message.Append(e.Error.StackTrace);
                message.Append(System.Environment.NewLine);

                if (e.Error.InnerException != null)
                {
                    message.Append(System.Environment.NewLine);
                    message.Append(e.Error.InnerException.Message);
                    if (e.Error.InnerException.InnerException != null)
                    { // This is quite ridiculous.
                        message.Append(System.Environment.NewLine);
                        message.Append(e.Error.InnerException.InnerException.Message);
                    }
                }

                MessageBox.Show(this, message.ToString(), Properties.Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, 0);
            }

            if (!buttonStateWorker.IsBusy)
            {
                buttonStateWorker.RunWorkerAsync();
            }
        }

        
        /// <summary>
        /// Disables the add and remove buttons.
        /// </summary>
        private void DisableButtons()
        {
            this.requestButton.Enabled = false;
            this.cancelButton.Enabled = false;
        }


        /// <summary>
        /// Handles the Load event for the form.
        /// </summary>
        /// <param name="sender">
        /// The form being loaded.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void FormLoad(object sender, EventArgs e)
        {
            this.DisableButtons();
            this.appStatus.Text = Properties.Resources.CheckingAdminStatus;
            buttonStateWorker.RunWorkerAsync();
        }


        /// <summary>
        /// Updates the variables which store the user's administrator status.
        /// </summary>
        private void UpdateUserAdministratorStatus()
        {
            this.userIsAdmin = LocalAdministratorGroup.IsMemberOfAdministrators(WindowsIdentity.GetCurrent());
            this.userIsDirectAdmin = LocalAdministratorGroup.IsMemberOfAdministratorsDirectly(WindowsIdentity.GetCurrent());
        }


        /// <summary>
        /// This function runs when RunWorkerAsync() is called by the button state BackgroundWorker object.
        /// </summary>
        /// <param name="sender">
        /// The BackgroundWorker that triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void DoButtonStateWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.UpdateUserAdministratorStatus();
        }


        /// <summary>
        /// Occurs when the background operation has completed, has been canceled, or has raised an exception.
        /// </summary>
        /// <param name="sender">
        /// The button state BackgroundWorker object, which triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void ButtonStateWorkCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(binding, Settings.NamedPipeServiceBaseAddress);
            IAdminGroup channel = namedPipeFactory.CreateChannel();
            bool userIsAuthorizedLocally = channel.UserIsAuthorized(Settings.LocalAllowedEntities, Settings.LocalDeniedEntities);
            namedPipeFactory.Close();



            // Enable the "grant admin rights" button, if the user is not already
            // an administrator and is authorized to obtain those rights.
            if (!userIsAuthorizedLocally)
            {
                requestButton.Text = Properties.Resources.UIMessageUnauthorized;
            }
            else
            {
                if (!this.userIsAdmin)
                {
                    this.Icon = Properties.Resources.Locked;
                    this.notifyIcon.Icon = Properties.Resources.Locked;
                    this.requestButton.Text = "Request Privileges";
                    this.pictureBox1.Image = Properties.Resources.Locked_Img;
                    messageLabel.Text = Properties.Resources.GrantRightsButtonText;

                    settingsToolStripMenuItem.Enabled = true;
                }
                else if (this.userIsAdmin)
                {
                    this.Icon = Properties.Resources.Unlocked;
                    this.notifyIcon.Icon = Properties.Resources.Unlocked;
                    requestButton.Text = "Remove Privileges";
                    this.pictureBox1.Image = Properties.Resources.Unlocked_Img;
                    messageLabel.Text = Properties.Resources.UIMessageAlreadyHaveRights;

                    settingsToolStripMenuItem.Enabled = false;
                }

                /* If the edit menu is disable, disable everything within it */
                foreach (ToolStripMenuItem item in settingsToolStripMenuItem.DropDownItems)
                {
                    item.Enabled = settingsToolStripMenuItem.Enabled;
                }

                this.requestButton.Enabled = true;
                this.cancelButton.Enabled = true;

                this.requestButton.Focus();
            }

            this.appStatus.Text = Properties.Resources.ApplicationIsReady;
        }


        /// <summary>
        /// Handles the MouseDoubleClick event for the notification area icon.
        /// </summary>
        /// <param name="sender">
        /// The notification icon that is being double-clicked.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Visible = true;
        }


        /// <summary>
        /// Handles the VisibleChanged event for the form.
        /// </summary>
        /// <param name="sender">
        /// The form whose visibility has changed.
        /// </param>
        /// <param name="e">
        /// Data specific to the event.
        /// </param>
        private void SubmitRequestForm_VisibleChanged(object sender, EventArgs e)
        {
            // Update the enabled/disabled state of the buttons, if the worker
            // is not already doing so.
            if (!buttonStateWorker.IsBusy)
            {
                buttonStateWorker.RunWorkerAsync();
            }
        }


        /// <summary>
        /// Handles the BalloonTipClosed event for the notification icon.
        /// </summary>
        /// <param name="sender">
        /// The notification icon whose balloon tip was closed.
        /// </param>
        /// <param name="e">
        /// Data specific to the event.
        /// </param>
        private void notifyIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            if (!this.userIsAdmin)
            {
                /*
                notifyIcon.Visible = false;
                this.Visible = true;
                this.ShowInTaskbar = true;
                */
                this.Close();
            }
        }

        private void MinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in settingsToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }

            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }

        private void SubmitForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}

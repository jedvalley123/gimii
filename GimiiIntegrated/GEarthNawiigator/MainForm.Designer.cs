﻿namespace GEarthNawiigator
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.icoNotify = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmGestureMode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmExit = new System.Windows.Forms.ToolStripMenuItem();
            this.gestureTimer = new System.Windows.Forms.Timer(this.components);
            this.cmsIcon.SuspendLayout();
            this.SuspendLayout();
            // 
            // updateTimer
            // 
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // icoNotify
            // 
            this.icoNotify.ContextMenuStrip = this.cmsIcon;
            this.icoNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("icoNotify.Icon")));
            this.icoNotify.Text = "EarthNawiigator";
            this.icoNotify.Visible = true;
            // 
            // cmsIcon
            // 
            this.cmsIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmGestureMode,
            this.tsmExit});
            this.cmsIcon.Name = "cmsIcon";
            this.cmsIcon.Size = new System.Drawing.Size(149, 48);
            // 
            // tsmGestureMode
            // 
            this.tsmGestureMode.CheckOnClick = true;
            this.tsmGestureMode.Name = "tsmGestureMode";
            this.tsmGestureMode.Size = new System.Drawing.Size(148, 22);
            this.tsmGestureMode.Text = "Gesture Mode";
            this.tsmGestureMode.ToolTipText = "Switch to gesture recognition mode.";
            this.tsmGestureMode.CheckedChanged += new System.EventHandler(this.tsmGestureMode_CheckedChanged);
            // 
            // tsmExit
            // 
            this.tsmExit.Name = "tsmExit";
            this.tsmExit.Size = new System.Drawing.Size(148, 22);
            this.tsmExit.Text = "Exit";
            this.tsmExit.Click += new System.EventHandler(this.tsmExit_Click);
            // 
            // gestureTimer
            // 
            this.gestureTimer.Interval = 5;
            this.gestureTimer.Tick += new System.EventHandler(this.gestureTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(183, 59);
            this.Name = "MainForm";
            this.Text = "Earth Nawiigator";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.cmsIcon.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.NotifyIcon icoNotify;
        private System.Windows.Forms.ContextMenuStrip cmsIcon;
        private System.Windows.Forms.ToolStripMenuItem tsmExit;
        private System.Windows.Forms.ToolStripMenuItem tsmGestureMode;
        private System.Windows.Forms.Timer gestureTimer;
    }
}
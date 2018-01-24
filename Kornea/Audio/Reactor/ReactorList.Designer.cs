namespace Kornea.Audio.Reactor
{
    partial class ReactorList
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
            this.lst = new System.Windows.Forms.ListBox();
            this.l2 = new System.Windows.Forms.ListBox();
            this.l = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lst
            // 
            this.lst.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lst.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lst.ForeColor = System.Drawing.SystemColors.Info;
            this.lst.FormattingEnabled = true;
            this.lst.Location = new System.Drawing.Point(0, 0);
            this.lst.Name = "lst";
            this.lst.Size = new System.Drawing.Size(426, 182);
            this.lst.TabIndex = 0;
            // 
            // l2
            // 
            this.l2.BackColor = System.Drawing.SystemColors.GrayText;
            this.l2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.l2.ForeColor = System.Drawing.SystemColors.Info;
            this.l2.FormattingEnabled = true;
            this.l2.Location = new System.Drawing.Point(0, 188);
            this.l2.Name = "l2";
            this.l2.Size = new System.Drawing.Size(426, 169);
            this.l2.TabIndex = 1;
            // 
            // l
            // 
            this.l.BackColor = System.Drawing.SystemColors.GrayText;
            this.l.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.l.ForeColor = System.Drawing.SystemColors.Info;
            this.l.FormattingEnabled = true;
            this.l.Location = new System.Drawing.Point(429, 1);
            this.l.Name = "l";
            this.l.Size = new System.Drawing.Size(120, 351);
            this.l.TabIndex = 2;
            // 
            // ReactorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 355);
            this.Controls.Add(this.l);
            this.Controls.Add(this.l2);
            this.Controls.Add(this.lst);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ReactorList";
            this.Text = "ReactorList";
            this.Load += new System.EventHandler(this.ReactorList_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lst;
        private System.Windows.Forms.ListBox l2;
        private System.Windows.Forms.ListBox l;
    }
}
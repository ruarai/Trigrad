namespace TrigradEditor
{
    partial class Form1
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
            this.outputPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.outputPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // outputPictureBox
            // 
            this.outputPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputPictureBox.Location = new System.Drawing.Point(0, 0);
            this.outputPictureBox.Name = "outputPictureBox";
            this.outputPictureBox.Size = new System.Drawing.Size(752, 730);
            this.outputPictureBox.TabIndex = 0;
            this.outputPictureBox.TabStop = false;
            this.outputPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.outputPictureBox_MouseDown);
            this.outputPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.outputPictureBox_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 730);
            this.Controls.Add(this.outputPictureBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.outputPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox outputPictureBox;
    }
}


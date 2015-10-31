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
            this.buildButton = new System.Windows.Forms.Button();
            this.samplesTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.outputPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // outputPictureBox
            // 
            this.outputPictureBox.Location = new System.Drawing.Point(12, 12);
            this.outputPictureBox.Name = "outputPictureBox";
            this.outputPictureBox.Size = new System.Drawing.Size(512, 512);
            this.outputPictureBox.TabIndex = 0;
            this.outputPictureBox.TabStop = false;
            this.outputPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.outputPictureBox_MouseDown);
            // 
            // buildButton
            // 
            this.buildButton.Location = new System.Drawing.Point(530, 501);
            this.buildButton.Name = "buildButton";
            this.buildButton.Size = new System.Drawing.Size(75, 23);
            this.buildButton.TabIndex = 1;
            this.buildButton.Text = "build";
            this.buildButton.UseVisualStyleBackColor = true;
            this.buildButton.Click += new System.EventHandler(this.buildButton_Click);
            // 
            // samplesTextBox
            // 
            this.samplesTextBox.Location = new System.Drawing.Point(546, 22);
            this.samplesTextBox.Name = "samplesTextBox";
            this.samplesTextBox.Size = new System.Drawing.Size(86, 20);
            this.samplesTextBox.TabIndex = 2;
            this.samplesTextBox.Text = "10000";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 730);
            this.Controls.Add(this.samplesTextBox);
            this.Controls.Add(this.buildButton);
            this.Controls.Add(this.outputPictureBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.outputPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox outputPictureBox;
        private System.Windows.Forms.Button buildButton;
        private System.Windows.Forms.TextBox samplesTextBox;
    }
}


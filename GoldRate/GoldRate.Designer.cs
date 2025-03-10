using System;
using System.Windows.Forms;

namespace GoldRatesExtractor
{
    partial class GoldRate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnSelectFile;
        private TextBox statusTextBox; // Added this


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
            this.statusTextBox = new System.Windows.Forms.TextBox(); // Initialize TextBox
            this.btnSelectFile = new System.Windows.Forms.Button(); // Initialize Button

            // Form settings
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Gold Rate Application";

            // statusTextBox settings
            this.statusTextBox.Location = new System.Drawing.Point(10, 400);
            this.statusTextBox.Size = new System.Drawing.Size(780, 30);
            this.statusTextBox.ReadOnly = true;
            this.Controls.Add(this.statusTextBox);

            // btnSelectFile settings
            this.btnSelectFile.Location = new System.Drawing.Point(10, 350);
            this.btnSelectFile.Size = new System.Drawing.Size(150, 30);
            this.btnSelectFile.Text = "Select File";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click); // Attach event
            this.Controls.Add(this.btnSelectFile);
        }


        #endregion
    }
}

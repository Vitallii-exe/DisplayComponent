namespace ImageDisplayComponent
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.displayUserControl = new ImageDisplayComponent.DisplayUserControl();
            this.SuspendLayout();
            // 
            // displayUserControl
            // 
            this.displayUserControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.displayUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayUserControl.Location = new System.Drawing.Point(0, 0);
            this.displayUserControl.Name = "displayUserControl";
            this.displayUserControl.Size = new System.Drawing.Size(933, 514);
            this.displayUserControl.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(933, 514);
            this.Controls.Add(this.displayUserControl);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private DisplayUserControl displayUserControl;
    }
}
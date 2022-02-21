namespace ImageDisplayComponent
{
    partial class DisplayUserControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // vScrollBar
            // 
            this.vScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar.Location = new System.Drawing.Point(562, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(25, 359);
            this.vScrollBar.TabIndex = 0;
            this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBarValueChanged);
            // 
            // hScrollBar
            // 
            this.hScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar.Location = new System.Drawing.Point(0, 334);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(562, 25);
            this.hScrollBar.TabIndex = 1;
            this.hScrollBar.ValueChanged += new System.EventHandler(this.hScrollBarValueChanged);
            // 
            // DisplayUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hScrollBar);
            this.Controls.Add(this.vScrollBar);
            this.Name = "DisplayUserControl";
            this.Size = new System.Drawing.Size(587, 359);
            this.Load += new System.EventHandler(this.DisplayUserControlLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private VScrollBar vScrollBar;
        private HScrollBar hScrollBar;
    }
}

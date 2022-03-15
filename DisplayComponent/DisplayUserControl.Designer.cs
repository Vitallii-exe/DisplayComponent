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
            this.scaleLabel = new System.Windows.Forms.Label();
            this.defaultLocationButton = new System.Windows.Forms.Button();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // vScrollBar
            // 
            this.vScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar.Location = new System.Drawing.Point(562, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(25, 377);
            this.vScrollBar.TabIndex = 0;
            this.vScrollBar.ValueChanged += new System.EventHandler(this.ScrollBarsValueChanged);
            // 
            // hScrollBar
            // 
            this.hScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar.Location = new System.Drawing.Point(0, 352);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(562, 25);
            this.hScrollBar.TabIndex = 1;
            this.hScrollBar.ValueChanged += new System.EventHandler(this.ScrollBarsValueChanged);
            // 
            // scaleLabel
            // 
            this.scaleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.scaleLabel.AutoSize = true;
            this.scaleLabel.Location = new System.Drawing.Point(517, 2);
            this.scaleLabel.Name = "scaleLabel";
            this.scaleLabel.Size = new System.Drawing.Size(45, 20);
            this.scaleLabel.TabIndex = 3;
            this.scaleLabel.Text = "100%";
            // 
            // defaultLocationButton
            // 
            this.defaultLocationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.defaultLocationButton.BackColor = System.Drawing.Color.Transparent;
            this.defaultLocationButton.BackgroundImage = global::ImageDisplayComponent.Properties.Resources.target;
            this.defaultLocationButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.defaultLocationButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.defaultLocationButton.Location = new System.Drawing.Point(463, 0);
            this.defaultLocationButton.Name = "defaultLocationButton";
            this.defaultLocationButton.Size = new System.Drawing.Size(24, 24);
            this.defaultLocationButton.TabIndex = 0;
            this.defaultLocationButton.UseCompatibleTextRendering = true;
            this.defaultLocationButton.UseVisualStyleBackColor = false;
            this.defaultLocationButton.Click += new System.EventHandler(this.defaultLocationButtonClick);
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.Color.DimGray;
            this.bottomPanel.Controls.Add(this.defaultLocationButton);
            this.bottomPanel.Controls.Add(this.scaleLabel);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 375);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(587, 24);
            this.bottomPanel.TabIndex = 4;
            // 
            // DisplayUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.hScrollBar);
            this.Controls.Add(this.vScrollBar);
            this.DoubleBuffered = true;
            this.Name = "DisplayUserControl";
            this.Size = new System.Drawing.Size(587, 399);
            this.SizeChanged += new System.EventHandler(this.DisplayUserControlSizeChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DisplayUserControlMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DisplayUserControlMouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DisplayUserControlMouseUp);
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private VScrollBar vScrollBar;
        private HScrollBar hScrollBar;
        private Label scaleLabel;
        private Button defaultLocationButton;
        private Panel bottomPanel;
    }
}

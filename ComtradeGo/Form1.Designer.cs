namespace ComtradeGo
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
            this.TipPicBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.TipPicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // TipPicBox
            // 
            this.TipPicBox.Location = new System.Drawing.Point(0, 0);
            this.TipPicBox.Name = "TipPicBox";
            this.TipPicBox.Size = new System.Drawing.Size(256, 256);
            this.TipPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.TipPicBox.TabIndex = 0;
            this.TipPicBox.TabStop = false;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(256, 256);
            this.Controls.Add(this.TipPicBox);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Comtrade Go";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileDragEnter);
            this.DragLeave += new System.EventHandler(this.FileDragLeave);
            ((System.ComponentModel.ISupportInitialize)(this.TipPicBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox TipPicBox;
    }
}


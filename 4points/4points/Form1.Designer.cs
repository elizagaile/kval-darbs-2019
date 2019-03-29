namespace _4points
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.lblC1 = new System.Windows.Forms.Label();
            this.lblC2 = new System.Windows.Forms.Label();
            this.lblC3 = new System.Windows.Forms.Label();
            this.lblC4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(789, 601);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // btnNew
            // 
            this.btnNew.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.Location = new System.Drawing.Point(807, 12);
            this.btnNew.Name = "btnNew";
            this.btnNew.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnNew.Size = new System.Drawing.Size(126, 45);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "new";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblC1
            // 
            this.lblC1.AutoSize = true;
            this.lblC1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblC1.Location = new System.Drawing.Point(803, 60);
            this.lblC1.Name = "lblC1";
            this.lblC1.Size = new System.Drawing.Size(35, 23);
            this.lblC1.TabIndex = 3;
            this.lblC1.Text = "C1";
            // 
            // lblC2
            // 
            this.lblC2.AutoSize = true;
            this.lblC2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblC2.Location = new System.Drawing.Point(803, 83);
            this.lblC2.Name = "lblC2";
            this.lblC2.Size = new System.Drawing.Size(35, 23);
            this.lblC2.TabIndex = 4;
            this.lblC2.Text = "C2";
            // 
            // lblC3
            // 
            this.lblC3.AutoSize = true;
            this.lblC3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblC3.Location = new System.Drawing.Point(803, 106);
            this.lblC3.Name = "lblC3";
            this.lblC3.Size = new System.Drawing.Size(35, 23);
            this.lblC3.TabIndex = 5;
            this.lblC3.Text = "C3";
            // 
            // lblC4
            // 
            this.lblC4.AutoSize = true;
            this.lblC4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblC4.Location = new System.Drawing.Point(803, 129);
            this.lblC4.Name = "lblC4";
            this.lblC4.Size = new System.Drawing.Size(35, 23);
            this.lblC4.TabIndex = 6;
            this.lblC4.Text = "C4";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(966, 625);
            this.Controls.Add(this.lblC4);
            this.Controls.Add(this.lblC3);
            this.Controls.Add(this.lblC2);
            this.Controls.Add(this.lblC1);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "4 punkti";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Label lblC1;
        private System.Windows.Forms.Label lblC2;
        private System.Windows.Forms.Label lblC3;
        private System.Windows.Forms.Label lblC4;
    }
}


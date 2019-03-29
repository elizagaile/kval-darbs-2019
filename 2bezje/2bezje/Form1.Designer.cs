namespace _2bezje
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
            this.label1 = new System.Windows.Forms.Label();
            this.lbl1C1 = new System.Windows.Forms.Label();
            this.lbl1C2 = new System.Windows.Forms.Label();
            this.lbl1C3 = new System.Windows.Forms.Label();
            this.lbl1C4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl2C1 = new System.Windows.Forms.Label();
            this.lbl2C2 = new System.Windows.Forms.Label();
            this.lbl2C3 = new System.Windows.Forms.Label();
            this.lbl2C4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(684, 560);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(703, 12);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(107, 47);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "NEW";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(704, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "1st bezier:";
            this.label1.MouseEnter += new System.EventHandler(this.label1_MouseEnter);
            this.label1.MouseLeave += new System.EventHandler(this.label1_MouseLeave);
            // 
            // lbl1C1
            // 
            this.lbl1C1.AutoSize = true;
            this.lbl1C1.Location = new System.Drawing.Point(704, 90);
            this.lbl1C1.Name = "lbl1C1";
            this.lbl1C1.Size = new System.Drawing.Size(29, 20);
            this.lbl1C1.TabIndex = 3;
            this.lbl1C1.Text = "C1";
            // 
            // lbl1C2
            // 
            this.lbl1C2.AutoSize = true;
            this.lbl1C2.Location = new System.Drawing.Point(704, 114);
            this.lbl1C2.Name = "lbl1C2";
            this.lbl1C2.Size = new System.Drawing.Size(29, 20);
            this.lbl1C2.TabIndex = 4;
            this.lbl1C2.Text = "C2";
            // 
            // lbl1C3
            // 
            this.lbl1C3.AutoSize = true;
            this.lbl1C3.Location = new System.Drawing.Point(704, 138);
            this.lbl1C3.Name = "lbl1C3";
            this.lbl1C3.Size = new System.Drawing.Size(29, 20);
            this.lbl1C3.TabIndex = 5;
            this.lbl1C3.Text = "C3";
            // 
            // lbl1C4
            // 
            this.lbl1C4.AutoSize = true;
            this.lbl1C4.Location = new System.Drawing.Point(704, 162);
            this.lbl1C4.Name = "lbl1C4";
            this.lbl1C4.Size = new System.Drawing.Size(29, 20);
            this.lbl1C4.TabIndex = 6;
            this.lbl1C4.Text = "C4";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(703, 205);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "2nd bezier:";
            this.label2.MouseEnter += new System.EventHandler(this.label2_MouseEnter);
            this.label2.MouseLeave += new System.EventHandler(this.label2_MouseLeave);
            // 
            // lbl2C1
            // 
            this.lbl2C1.AutoSize = true;
            this.lbl2C1.Location = new System.Drawing.Point(704, 229);
            this.lbl2C1.Name = "lbl2C1";
            this.lbl2C1.Size = new System.Drawing.Size(29, 20);
            this.lbl2C1.TabIndex = 8;
            this.lbl2C1.Text = "C1";
            // 
            // lbl2C2
            // 
            this.lbl2C2.AutoSize = true;
            this.lbl2C2.Location = new System.Drawing.Point(704, 253);
            this.lbl2C2.Name = "lbl2C2";
            this.lbl2C2.Size = new System.Drawing.Size(29, 20);
            this.lbl2C2.TabIndex = 9;
            this.lbl2C2.Text = "C2";
            // 
            // lbl2C3
            // 
            this.lbl2C3.AutoSize = true;
            this.lbl2C3.Location = new System.Drawing.Point(704, 277);
            this.lbl2C3.Name = "lbl2C3";
            this.lbl2C3.Size = new System.Drawing.Size(29, 20);
            this.lbl2C3.TabIndex = 10;
            this.lbl2C3.Text = "C3";
            // 
            // lbl2C4
            // 
            this.lbl2C4.AutoSize = true;
            this.lbl2C4.Location = new System.Drawing.Point(704, 301);
            this.lbl2C4.Name = "lbl2C4";
            this.lbl2C4.Size = new System.Drawing.Size(29, 20);
            this.lbl2C4.TabIndex = 11;
            this.lbl2C4.Text = "C4";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 585);
            this.Controls.Add(this.lbl2C4);
            this.Controls.Add(this.lbl2C3);
            this.Controls.Add(this.lbl2C2);
            this.Controls.Add(this.lbl2C1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbl1C4);
            this.Controls.Add(this.lbl1C3);
            this.Controls.Add(this.lbl1C2);
            this.Controls.Add(this.lbl1C1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl1C1;
        private System.Windows.Forms.Label lbl1C2;
        private System.Windows.Forms.Label lbl1C3;
        private System.Windows.Forms.Label lbl1C4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl2C1;
        private System.Windows.Forms.Label lbl2C2;
        private System.Windows.Forms.Label lbl2C3;
        private System.Windows.Forms.Label lbl2C4;
    }
}


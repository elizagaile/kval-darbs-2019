namespace caur_4
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
            this.lblP1 = new System.Windows.Forms.Label();
            this.lblP2 = new System.Windows.Forms.Label();
            this.lblP3 = new System.Windows.Forms.Label();
            this.lblP4 = new System.Windows.Forms.Label();
            this.error = new System.Windows.Forms.Label();
            this.btnChord2 = new System.Windows.Forms.Button();
            this.btnUniform1 = new System.Windows.Forms.Button();
            this.lblParam = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox1.Location = new System.Drawing.Point(9, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(652, 523);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(671, 13);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(117, 56);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "new";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // lblC1
            // 
            this.lblC1.AutoSize = true;
            this.lblC1.Location = new System.Drawing.Point(671, 76);
            this.lblC1.Name = "lblC1";
            this.lblC1.Size = new System.Drawing.Size(29, 20);
            this.lblC1.TabIndex = 2;
            this.lblC1.Text = "C1";
            // 
            // lblC2
            // 
            this.lblC2.AutoSize = true;
            this.lblC2.Location = new System.Drawing.Point(671, 100);
            this.lblC2.Name = "lblC2";
            this.lblC2.Size = new System.Drawing.Size(29, 20);
            this.lblC2.TabIndex = 3;
            this.lblC2.Text = "C2";
            // 
            // lblC3
            // 
            this.lblC3.AutoSize = true;
            this.lblC3.Location = new System.Drawing.Point(671, 124);
            this.lblC3.Name = "lblC3";
            this.lblC3.Size = new System.Drawing.Size(29, 20);
            this.lblC3.TabIndex = 4;
            this.lblC3.Text = "C3";
            // 
            // lblC4
            // 
            this.lblC4.AutoSize = true;
            this.lblC4.Location = new System.Drawing.Point(671, 148);
            this.lblC4.Name = "lblC4";
            this.lblC4.Size = new System.Drawing.Size(29, 20);
            this.lblC4.TabIndex = 5;
            this.lblC4.Text = "C4";
            // 
            // lblP1
            // 
            this.lblP1.AutoSize = true;
            this.lblP1.Location = new System.Drawing.Point(671, 192);
            this.lblP1.Name = "lblP1";
            this.lblP1.Size = new System.Drawing.Size(28, 20);
            this.lblP1.TabIndex = 6;
            this.lblP1.Text = "P1";
            // 
            // lblP2
            // 
            this.lblP2.AutoSize = true;
            this.lblP2.Location = new System.Drawing.Point(671, 216);
            this.lblP2.Name = "lblP2";
            this.lblP2.Size = new System.Drawing.Size(28, 20);
            this.lblP2.TabIndex = 7;
            this.lblP2.Text = "P2";
            // 
            // lblP3
            // 
            this.lblP3.AutoSize = true;
            this.lblP3.Location = new System.Drawing.Point(671, 240);
            this.lblP3.Name = "lblP3";
            this.lblP3.Size = new System.Drawing.Size(28, 20);
            this.lblP3.TabIndex = 8;
            this.lblP3.Text = "P3";
            // 
            // lblP4
            // 
            this.lblP4.AutoSize = true;
            this.lblP4.Location = new System.Drawing.Point(671, 264);
            this.lblP4.Name = "lblP4";
            this.lblP4.Size = new System.Drawing.Size(28, 20);
            this.lblP4.TabIndex = 9;
            this.lblP4.Text = "P4";
            // 
            // error
            // 
            this.error.AutoSize = true;
            this.error.Location = new System.Drawing.Point(667, 515);
            this.error.Name = "error";
            this.error.Size = new System.Drawing.Size(18, 20);
            this.error.TabIndex = 10;
            this.error.Text = "+";
            // 
            // btnChord2
            // 
            this.btnChord2.Location = new System.Drawing.Point(670, 479);
            this.btnChord2.Name = "btnChord2";
            this.btnChord2.Size = new System.Drawing.Size(91, 33);
            this.btnChord2.TabIndex = 11;
            this.btnChord2.Text = "Chord";
            this.btnChord2.UseVisualStyleBackColor = true;
            this.btnChord2.Click += new System.EventHandler(this.btnChord2_Click);
            // 
            // btnUniform1
            // 
            this.btnUniform1.Location = new System.Drawing.Point(671, 440);
            this.btnUniform1.Name = "btnUniform1";
            this.btnUniform1.Size = new System.Drawing.Size(90, 33);
            this.btnUniform1.TabIndex = 12;
            this.btnUniform1.Text = "Uniform";
            this.btnUniform1.UseVisualStyleBackColor = true;
            this.btnUniform1.Click += new System.EventHandler(this.btnUniform1_Click);
            // 
            // lblParam
            // 
            this.lblParam.AutoSize = true;
            this.lblParam.Location = new System.Drawing.Point(5, 9);
            this.lblParam.Name = "lblParam";
            this.lblParam.Size = new System.Drawing.Size(84, 20);
            this.lblParam.TabIndex = 13;
            this.lblParam.Text = "UNIFORM";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 547);
            this.Controls.Add(this.lblParam);
            this.Controls.Add(this.btnUniform1);
            this.Controls.Add(this.btnChord2);
            this.Controls.Add(this.error);
            this.Controls.Add(this.lblP4);
            this.Controls.Add(this.lblP3);
            this.Controls.Add(this.lblP2);
            this.Controls.Add(this.lblP1);
            this.Controls.Add(this.lblC4);
            this.Controls.Add(this.lblC3);
            this.Controls.Add(this.lblC2);
            this.Controls.Add(this.lblC1);
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
        private System.Windows.Forms.Label lblC1;
        private System.Windows.Forms.Label lblC2;
        private System.Windows.Forms.Label lblC3;
        private System.Windows.Forms.Label lblC4;
        private System.Windows.Forms.Label lblP1;
        private System.Windows.Forms.Label lblP2;
        private System.Windows.Forms.Label lblP3;
        private System.Windows.Forms.Label lblP4;
        private System.Windows.Forms.Label error;
        private System.Windows.Forms.Button btnChord2;
        private System.Windows.Forms.Button btnUniform1;
        private System.Windows.Forms.Label lblParam;
    }
}


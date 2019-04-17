namespace BezierTool
{
    partial class Form_KeyboardAdd
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
            this.lbl_1 = new System.Windows.Forms.Label();
            this.lbl_2 = new System.Windows.Forms.Label();
            this.lbl_3 = new System.Windows.Forms.Label();
            this.lbl_4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_SubmitInput = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.input_y4 = new System.Windows.Forms.TextBox();
            this.input_x2 = new System.Windows.Forms.TextBox();
            this.input_x4 = new System.Windows.Forms.TextBox();
            this.input_y3 = new System.Windows.Forms.TextBox();
            this.input_x3 = new System.Windows.Forms.TextBox();
            this.input_y1 = new System.Windows.Forms.TextBox();
            this.input_x1 = new System.Windows.Forms.TextBox();
            this.input_y2 = new System.Windows.Forms.TextBox();
            this.btn_ResetInput = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_1
            // 
            this.lbl_1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_1.AutoSize = true;
            this.lbl_1.Location = new System.Drawing.Point(33, 41);
            this.lbl_1.Name = "lbl_1";
            this.lbl_1.Size = new System.Drawing.Size(0, 20);
            this.lbl_1.TabIndex = 1;
            // 
            // lbl_2
            // 
            this.lbl_2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_2.AutoSize = true;
            this.lbl_2.Location = new System.Drawing.Point(33, 75);
            this.lbl_2.Name = "lbl_2";
            this.lbl_2.Size = new System.Drawing.Size(0, 20);
            this.lbl_2.TabIndex = 2;
            // 
            // lbl_3
            // 
            this.lbl_3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_3.AutoSize = true;
            this.lbl_3.Location = new System.Drawing.Point(33, 109);
            this.lbl_3.Name = "lbl_3";
            this.lbl_3.Size = new System.Drawing.Size(0, 20);
            this.lbl_3.TabIndex = 3;
            // 
            // lbl_4
            // 
            this.lbl_4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_4.AutoSize = true;
            this.lbl_4.Location = new System.Drawing.Point(33, 145);
            this.lbl_4.Name = "lbl_4";
            this.lbl_4.Size = new System.Drawing.Size(0, 20);
            this.lbl_4.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(62, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 20);
            this.label6.TabIndex = 5;
            this.label6.Text = "X";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(136, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = "Y";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_ResetInput);
            this.groupBox1.Controls.Add(this.btn_SubmitInput);
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(434, 230);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set control point coordinates:";
            // 
            // btn_SubmitInput
            // 
            this.btn_SubmitInput.Location = new System.Drawing.Point(210, 121);
            this.btn_SubmitInput.Name = "btn_SubmitInput";
            this.btn_SubmitInput.Size = new System.Drawing.Size(75, 32);
            this.btn_SubmitInput.TabIndex = 8;
            this.btn_SubmitInput.Text = "OK";
            this.btn_SubmitInput.UseVisualStyleBackColor = true;
            this.btn_SubmitInput.Click += new System.EventHandler(this.btn_SubmitInput_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.Controls.Add(this.input_y4, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.input_x2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lbl_4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lbl_3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbl_1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbl_2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label7, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.input_x4, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.input_y3, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.input_x3, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.input_y1, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.input_x1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.input_y2, 2, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(20, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(184, 174);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // input_y4
            // 
            this.input_y4.Location = new System.Drawing.Point(112, 139);
            this.input_y4.Name = "input_y4";
            this.input_y4.Size = new System.Drawing.Size(69, 26);
            this.input_y4.TabIndex = 12;
            // 
            // input_x2
            // 
            this.input_x2.Location = new System.Drawing.Point(39, 71);
            this.input_x2.Name = "input_x2";
            this.input_x2.Size = new System.Drawing.Size(67, 26);
            this.input_x2.TabIndex = 8;
            // 
            // input_x4
            // 
            this.input_x4.Location = new System.Drawing.Point(39, 139);
            this.input_x4.Name = "input_x4";
            this.input_x4.Size = new System.Drawing.Size(67, 26);
            this.input_x4.TabIndex = 11;
            // 
            // input_y3
            // 
            this.input_y3.Location = new System.Drawing.Point(112, 105);
            this.input_y3.Name = "input_y3";
            this.input_y3.Size = new System.Drawing.Size(69, 26);
            this.input_y3.TabIndex = 10;
            // 
            // input_x3
            // 
            this.input_x3.Location = new System.Drawing.Point(39, 105);
            this.input_x3.Name = "input_x3";
            this.input_x3.Size = new System.Drawing.Size(67, 26);
            this.input_x3.TabIndex = 9;
            // 
            // input_y1
            // 
            this.input_y1.Location = new System.Drawing.Point(112, 37);
            this.input_y1.Name = "input_y1";
            this.input_y1.Size = new System.Drawing.Size(69, 26);
            this.input_y1.TabIndex = 7;
            // 
            // input_x1
            // 
            this.input_x1.Location = new System.Drawing.Point(39, 37);
            this.input_x1.Name = "input_x1";
            this.input_x1.Size = new System.Drawing.Size(67, 26);
            this.input_x1.TabIndex = 6;
            // 
            // input_y2
            // 
            this.input_y2.Location = new System.Drawing.Point(112, 71);
            this.input_y2.Name = "input_y2";
            this.input_y2.Size = new System.Drawing.Size(69, 26);
            this.input_y2.TabIndex = 8;
            // 
            // btn_ResetInput
            // 
            this.btn_ResetInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btn_ResetInput.ForeColor = System.Drawing.Color.White;
            this.btn_ResetInput.Location = new System.Drawing.Point(210, 158);
            this.btn_ResetInput.Name = "btn_ResetInput";
            this.btn_ResetInput.Size = new System.Drawing.Size(75, 38);
            this.btn_ResetInput.TabIndex = 9;
            this.btn_ResetInput.Text = "Reset";
            this.btn_ResetInput.UseVisualStyleBackColor = false;
            this.btn_ResetInput.Click += new System.EventHandler(this.btn_ResetInput_Click);
            // 
            // Form_KeyboardAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 246);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form_KeyboardAdd";
            this.Text = "Add new <4 cPoints> line";
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbl_1;
        private System.Windows.Forms.Label lbl_2;
        private System.Windows.Forms.Label lbl_3;
        private System.Windows.Forms.Label lbl_4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox input_x1;
        private System.Windows.Forms.TextBox input_y2;
        private System.Windows.Forms.TextBox input_y1;
        private System.Windows.Forms.TextBox input_x3;
        private System.Windows.Forms.TextBox input_x2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox input_y3;
        private System.Windows.Forms.TextBox input_x4;
        private System.Windows.Forms.TextBox input_y4;
        private System.Windows.Forms.Button btn_SubmitInput;
        private System.Windows.Forms.Button btn_ResetInput;
    }
}
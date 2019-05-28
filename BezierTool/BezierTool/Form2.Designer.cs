namespace BezierTool
{
    partial class FormCoordinates
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
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.gbCoordinates = new System.Windows.Forms.GroupBox();
            this.btnDeleteRow = new System.Windows.Forms.Button();
            this.flpCoordinates = new System.Windows.Forms.FlowLayoutPanel();
            this.tlpCoordinates = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSubmitInput = new System.Windows.Forms.Button();
            this.btnResetInput = new System.Windows.Forms.Button();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.gbCoordinates.SuspendLayout();
            this.flpCoordinates.SuspendLayout();
            this.tlpCoordinates.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(80, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 20);
            this.label6.TabIndex = 5;
            this.label6.Text = "X";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(160, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = "Y";
            // 
            // gbCoordinates
            // 
            this.gbCoordinates.Controls.Add(this.btnDeleteRow);
            this.gbCoordinates.Controls.Add(this.flpCoordinates);
            this.gbCoordinates.Controls.Add(this.btnSubmitInput);
            this.gbCoordinates.Controls.Add(this.btnResetInput);
            this.gbCoordinates.Controls.Add(this.btnAddRow);
            this.gbCoordinates.Location = new System.Drawing.Point(13, 13);
            this.gbCoordinates.Name = "gbCoordinates";
            this.gbCoordinates.Size = new System.Drawing.Size(462, 457);
            this.gbCoordinates.TabIndex = 7;
            this.gbCoordinates.TabStop = false;
            this.gbCoordinates.Text = "Set control point coordinates:";
            // 
            // btnDeleteRow
            // 
            this.btnDeleteRow.Location = new System.Drawing.Point(303, 67);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(125, 36);
            this.btnDeleteRow.TabIndex = 11;
            this.btnDeleteRow.Text = "Delete Row";
            this.btnDeleteRow.UseVisualStyleBackColor = true;
            this.btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);
            // 
            // flpCoordinates
            // 
            this.flpCoordinates.Controls.Add(this.tlpCoordinates);
            this.flpCoordinates.Location = new System.Drawing.Point(40, 25);
            this.flpCoordinates.Name = "flpCoordinates";
            this.flpCoordinates.Size = new System.Drawing.Size(257, 432);
            this.flpCoordinates.TabIndex = 9;
            // 
            // tlpCoordinates
            // 
            this.tlpCoordinates.ColumnCount = 4;
            this.tlpCoordinates.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpCoordinates.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpCoordinates.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpCoordinates.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpCoordinates.Controls.Add(this.label7, 2, 0);
            this.tlpCoordinates.Controls.Add(this.label6, 1, 0);
            this.tlpCoordinates.Controls.Add(this.label1, 0, 0);
            this.tlpCoordinates.Controls.Add(this.label2, 3, 0);
            this.tlpCoordinates.Location = new System.Drawing.Point(3, 3);
            this.tlpCoordinates.Name = "tlpCoordinates";
            this.tlpCoordinates.RowCount = 5;
            this.tlpCoordinates.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCoordinates.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCoordinates.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCoordinates.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCoordinates.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCoordinates.Size = new System.Drawing.Size(231, 423);
            this.tlpCoordinates.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 20);
            this.label1.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(213, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 20);
            this.label2.TabIndex = 8;
            // 
            // btnSubmitInput
            // 
            this.btnSubmitInput.Location = new System.Drawing.Point(303, 401);
            this.btnSubmitInput.Name = "btnSubmitInput";
            this.btnSubmitInput.Size = new System.Drawing.Size(125, 32);
            this.btnSubmitInput.TabIndex = 8;
            this.btnSubmitInput.Text = "OK";
            this.btnSubmitInput.UseVisualStyleBackColor = true;
            this.btnSubmitInput.Click += new System.EventHandler(this.btnSubmitInput_Click);
            // 
            // btnResetInput
            // 
            this.btnResetInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnResetInput.ForeColor = System.Drawing.Color.White;
            this.btnResetInput.Location = new System.Drawing.Point(303, 357);
            this.btnResetInput.Name = "btnResetInput";
            this.btnResetInput.Size = new System.Drawing.Size(125, 38);
            this.btnResetInput.TabIndex = 9;
            this.btnResetInput.Text = "Reset";
            this.btnResetInput.UseVisualStyleBackColor = false;
            this.btnResetInput.Click += new System.EventHandler(this.btnResetInput_Click);
            // 
            // btnAddRow
            // 
            this.btnAddRow.Location = new System.Drawing.Point(303, 25);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(125, 36);
            this.btnAddRow.TabIndex = 10;
            this.btnAddRow.Text = "Add New Row";
            this.btnAddRow.UseVisualStyleBackColor = true;
            this.btnAddRow.Click += new System.EventHandler(this.btnAddRow_Click);
            // 
            // FormCoordinates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 482);
            this.Controls.Add(this.gbCoordinates);
            this.Name = "FormCoordinates";
            this.gbCoordinates.ResumeLayout(false);
            this.flpCoordinates.ResumeLayout(false);
            this.tlpCoordinates.ResumeLayout(false);
            this.tlpCoordinates.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gbCoordinates;
        private System.Windows.Forms.TableLayoutPanel tlpCoordinates;
        private System.Windows.Forms.Button btnSubmitInput;
        private System.Windows.Forms.Button btnResetInput;
        private System.Windows.Forms.FlowLayoutPanel flpCoordinates;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDeleteRow;
    }
}
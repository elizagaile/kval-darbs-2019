﻿namespace BezierTool
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
            this.btn_Background = new System.Windows.Forms.Button();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.btn_cPointsAdd = new System.Windows.Forms.Button();
            this.btn_pPointsAdd = new System.Windows.Forms.Button();
            this.btn_LeastSquaresAdd = new System.Windows.Forms.Button();
            this.btn_CompositeAdd = new System.Windows.Forms.Button();
            this.rbtn_MouseAdd = new System.Windows.Forms.RadioButton();
            this.rbtn_TypeAdd = new System.Windows.Forms.RadioButton();
            this.rbtn_FileAdd = new System.Windows.Forms.RadioButton();
            this.error = new System.Windows.Forms.Label();
            this.cbox_ShowBackground = new System.Windows.Forms.CheckBox();
            this.btn_cPointsModify = new System.Windows.Forms.Button();
            this.btn_pPointsModify = new System.Windows.Forms.Button();
            this.rbtn_MouseModify = new System.Windows.Forms.RadioButton();
            this.rbtn_TypeModify = new System.Windows.Forms.RadioButton();
            this.group_AddInput = new System.Windows.Forms.GroupBox();
            this.group_AddType = new System.Windows.Forms.GroupBox();
            this.group_ModifyType = new System.Windows.Forms.GroupBox();
            this.group_ModifyInput = new System.Windows.Forms.GroupBox();
            this.btn_DoneModify = new System.Windows.Forms.Button();
            this.group_Param = new System.Windows.Forms.GroupBox();
            this.rbtn_Uniform = new System.Windows.Forms.RadioButton();
            this.rbtn_Chord = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.group_AddInput.SuspendLayout();
            this.group_AddType.SuspendLayout();
            this.group_ModifyType.SuspendLayout();
            this.group_ModifyInput.SuspendLayout();
            this.group_Param.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1096, 1035);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // btn_Background
            // 
            this.btn_Background.Location = new System.Drawing.Point(1115, 923);
            this.btn_Background.Name = "btn_Background";
            this.btn_Background.Size = new System.Drawing.Size(303, 36);
            this.btn_Background.TabIndex = 1;
            this.btn_Background.Text = "Upload Background Image";
            this.btn_Background.UseVisualStyleBackColor = true;
            this.btn_Background.Click += new System.EventHandler(this.btnBackground_Click);
            // 
            // btn_Reset
            // 
            this.btn_Reset.Location = new System.Drawing.Point(1115, 1011);
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.Size = new System.Drawing.Size(303, 37);
            this.btn_Reset.TabIndex = 2;
            this.btn_Reset.Text = "Reset All";
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // btn_cPointsAdd
            // 
            this.btn_cPointsAdd.Location = new System.Drawing.Point(6, 33);
            this.btn_cPointsAdd.Name = "btn_cPointsAdd";
            this.btn_cPointsAdd.Size = new System.Drawing.Size(159, 37);
            this.btn_cPointsAdd.TabIndex = 4;
            this.btn_cPointsAdd.Text = "4 cPoints";
            this.btn_cPointsAdd.UseVisualStyleBackColor = true;
            this.btn_cPointsAdd.Click += new System.EventHandler(this.btn_cPointsAdd_Click);
            // 
            // btn_pPointsAdd
            // 
            this.btn_pPointsAdd.Location = new System.Drawing.Point(6, 76);
            this.btn_pPointsAdd.Name = "btn_pPointsAdd";
            this.btn_pPointsAdd.Size = new System.Drawing.Size(159, 37);
            this.btn_pPointsAdd.TabIndex = 5;
            this.btn_pPointsAdd.Text = "4 pPoints";
            this.btn_pPointsAdd.UseVisualStyleBackColor = true;
            this.btn_pPointsAdd.Click += new System.EventHandler(this.btn_pPointsAdd_Click);
            // 
            // btn_LeastSquaresAdd
            // 
            this.btn_LeastSquaresAdd.Location = new System.Drawing.Point(6, 119);
            this.btn_LeastSquaresAdd.Name = "btn_LeastSquaresAdd";
            this.btn_LeastSquaresAdd.Size = new System.Drawing.Size(159, 37);
            this.btn_LeastSquaresAdd.TabIndex = 6;
            this.btn_LeastSquaresAdd.Text = "Least Squares";
            this.btn_LeastSquaresAdd.UseVisualStyleBackColor = true;
            // 
            // btn_CompositeAdd
            // 
            this.btn_CompositeAdd.Location = new System.Drawing.Point(6, 162);
            this.btn_CompositeAdd.Name = "btn_CompositeAdd";
            this.btn_CompositeAdd.Size = new System.Drawing.Size(159, 37);
            this.btn_CompositeAdd.TabIndex = 7;
            this.btn_CompositeAdd.Text = "Composite";
            this.btn_CompositeAdd.UseVisualStyleBackColor = true;
            // 
            // rbtn_MouseAdd
            // 
            this.rbtn_MouseAdd.AutoSize = true;
            this.rbtn_MouseAdd.Checked = true;
            this.rbtn_MouseAdd.Location = new System.Drawing.Point(6, 25);
            this.rbtn_MouseAdd.Name = "rbtn_MouseAdd";
            this.rbtn_MouseAdd.Size = new System.Drawing.Size(118, 24);
            this.rbtn_MouseAdd.TabIndex = 8;
            this.rbtn_MouseAdd.TabStop = true;
            this.rbtn_MouseAdd.Text = "With Mouse";
            this.rbtn_MouseAdd.UseVisualStyleBackColor = true;
            // 
            // rbtn_TypeAdd
            // 
            this.rbtn_TypeAdd.AutoSize = true;
            this.rbtn_TypeAdd.Location = new System.Drawing.Point(6, 55);
            this.rbtn_TypeAdd.Name = "rbtn_TypeAdd";
            this.rbtn_TypeAdd.Size = new System.Drawing.Size(68, 24);
            this.rbtn_TypeAdd.TabIndex = 9;
            this.rbtn_TypeAdd.Text = "Type";
            this.rbtn_TypeAdd.UseVisualStyleBackColor = true;
            // 
            // rbtn_FileAdd
            // 
            this.rbtn_FileAdd.AutoSize = true;
            this.rbtn_FileAdd.Location = new System.Drawing.Point(6, 85);
            this.rbtn_FileAdd.Name = "rbtn_FileAdd";
            this.rbtn_FileAdd.Size = new System.Drawing.Size(95, 24);
            this.rbtn_FileAdd.TabIndex = 10;
            this.rbtn_FileAdd.Text = "From file";
            this.rbtn_FileAdd.UseVisualStyleBackColor = true;
            // 
            // error
            // 
            this.error.AutoSize = true;
            this.error.Location = new System.Drawing.Point(1193, 519);
            this.error.Name = "error";
            this.error.Size = new System.Drawing.Size(18, 20);
            this.error.TabIndex = 12;
            this.error.Text = "+";
            // 
            // cbox_ShowBackground
            // 
            this.cbox_ShowBackground.AutoSize = true;
            this.cbox_ShowBackground.Location = new System.Drawing.Point(1121, 965);
            this.cbox_ShowBackground.Name = "cbox_ShowBackground";
            this.cbox_ShowBackground.Size = new System.Drawing.Size(165, 24);
            this.cbox_ShowBackground.TabIndex = 13;
            this.cbox_ShowBackground.Text = "Show Background";
            this.cbox_ShowBackground.UseVisualStyleBackColor = true;
            this.cbox_ShowBackground.CheckStateChanged += new System.EventHandler(this.cbox_ShowBackground_CheckStateChanged);
            // 
            // btn_cPointsModify
            // 
            this.btn_cPointsModify.Location = new System.Drawing.Point(6, 25);
            this.btn_cPointsModify.Name = "btn_cPointsModify";
            this.btn_cPointsModify.Size = new System.Drawing.Size(159, 37);
            this.btn_cPointsModify.TabIndex = 14;
            this.btn_cPointsModify.Text = "Drag cPoints";
            this.btn_cPointsModify.UseVisualStyleBackColor = true;
            this.btn_cPointsModify.Click += new System.EventHandler(this.btn_cPointsModify_Click);
            // 
            // btn_pPointsModify
            // 
            this.btn_pPointsModify.Location = new System.Drawing.Point(6, 68);
            this.btn_pPointsModify.Name = "btn_pPointsModify";
            this.btn_pPointsModify.Size = new System.Drawing.Size(159, 37);
            this.btn_pPointsModify.TabIndex = 15;
            this.btn_pPointsModify.Text = "Drag pPoints";
            this.btn_pPointsModify.UseVisualStyleBackColor = true;
            this.btn_pPointsModify.Click += new System.EventHandler(this.btn_pPointsModify_Click);
            // 
            // rbtn_MouseModify
            // 
            this.rbtn_MouseModify.AutoSize = true;
            this.rbtn_MouseModify.Checked = true;
            this.rbtn_MouseModify.Location = new System.Drawing.Point(6, 25);
            this.rbtn_MouseModify.Name = "rbtn_MouseModify";
            this.rbtn_MouseModify.Size = new System.Drawing.Size(118, 24);
            this.rbtn_MouseModify.TabIndex = 17;
            this.rbtn_MouseModify.TabStop = true;
            this.rbtn_MouseModify.Text = "With Mouse";
            this.rbtn_MouseModify.UseVisualStyleBackColor = true;
            // 
            // rbtn_TypeModify
            // 
            this.rbtn_TypeModify.AutoSize = true;
            this.rbtn_TypeModify.Location = new System.Drawing.Point(6, 55);
            this.rbtn_TypeModify.Name = "rbtn_TypeModify";
            this.rbtn_TypeModify.Size = new System.Drawing.Size(68, 24);
            this.rbtn_TypeModify.TabIndex = 18;
            this.rbtn_TypeModify.Text = "Type";
            this.rbtn_TypeModify.UseVisualStyleBackColor = true;
            // 
            // group_AddInput
            // 
            this.group_AddInput.Controls.Add(this.rbtn_MouseAdd);
            this.group_AddInput.Controls.Add(this.rbtn_TypeAdd);
            this.group_AddInput.Controls.Add(this.rbtn_FileAdd);
            this.group_AddInput.Location = new System.Drawing.Point(1292, 13);
            this.group_AddInput.Name = "group_AddInput";
            this.group_AddInput.Size = new System.Drawing.Size(140, 115);
            this.group_AddInput.TabIndex = 19;
            this.group_AddInput.TabStop = false;
            this.group_AddInput.Text = "Choose points:";
            // 
            // group_AddType
            // 
            this.group_AddType.Controls.Add(this.btn_cPointsAdd);
            this.group_AddType.Controls.Add(this.btn_pPointsAdd);
            this.group_AddType.Controls.Add(this.btn_LeastSquaresAdd);
            this.group_AddType.Controls.Add(this.btn_CompositeAdd);
            this.group_AddType.Location = new System.Drawing.Point(1115, 13);
            this.group_AddType.Name = "group_AddType";
            this.group_AddType.Size = new System.Drawing.Size(171, 205);
            this.group_AddType.TabIndex = 20;
            this.group_AddType.TabStop = false;
            this.group_AddType.Text = "New Bezier of type:";
            // 
            // group_ModifyType
            // 
            this.group_ModifyType.Controls.Add(this.btn_cPointsModify);
            this.group_ModifyType.Controls.Add(this.btn_pPointsModify);
            this.group_ModifyType.Location = new System.Drawing.Point(1115, 240);
            this.group_ModifyType.Name = "group_ModifyType";
            this.group_ModifyType.Size = new System.Drawing.Size(171, 114);
            this.group_ModifyType.TabIndex = 21;
            this.group_ModifyType.TabStop = false;
            this.group_ModifyType.Text = "Modify existing line:";
            // 
            // group_ModifyInput
            // 
            this.group_ModifyInput.Controls.Add(this.rbtn_MouseModify);
            this.group_ModifyInput.Controls.Add(this.rbtn_TypeModify);
            this.group_ModifyInput.Location = new System.Drawing.Point(1292, 240);
            this.group_ModifyInput.Name = "group_ModifyInput";
            this.group_ModifyInput.Size = new System.Drawing.Size(140, 114);
            this.group_ModifyInput.TabIndex = 22;
            this.group_ModifyInput.TabStop = false;
            this.group_ModifyInput.Text = "Modify points:";
            // 
            // btn_DoneModify
            // 
            this.btn_DoneModify.Location = new System.Drawing.Point(1115, 360);
            this.btn_DoneModify.Name = "btn_DoneModify";
            this.btn_DoneModify.Size = new System.Drawing.Size(317, 37);
            this.btn_DoneModify.TabIndex = 23;
            this.btn_DoneModify.Text = "Done";
            this.btn_DoneModify.UseVisualStyleBackColor = true;
            this.btn_DoneModify.Click += new System.EventHandler(this.btn_DoneModify_Click);
            // 
            // group_Param
            // 
            this.group_Param.Controls.Add(this.rbtn_Chord);
            this.group_Param.Controls.Add(this.rbtn_Uniform);
            this.group_Param.Location = new System.Drawing.Point(1292, 134);
            this.group_Param.Name = "group_Param";
            this.group_Param.Size = new System.Drawing.Size(140, 84);
            this.group_Param.TabIndex = 24;
            this.group_Param.TabStop = false;
            this.group_Param.Text = "Paramterization:";
            // 
            // rbtn_Uniform
            // 
            this.rbtn_Uniform.AutoSize = true;
            this.rbtn_Uniform.Checked = true;
            this.rbtn_Uniform.Location = new System.Drawing.Point(6, 25);
            this.rbtn_Uniform.Name = "rbtn_Uniform";
            this.rbtn_Uniform.Size = new System.Drawing.Size(90, 24);
            this.rbtn_Uniform.TabIndex = 0;
            this.rbtn_Uniform.TabStop = true;
            this.rbtn_Uniform.Text = "Uniform";
            this.rbtn_Uniform.UseVisualStyleBackColor = true;
            // 
            // rbtn_Chord
            // 
            this.rbtn_Chord.AutoSize = true;
            this.rbtn_Chord.Location = new System.Drawing.Point(6, 55);
            this.rbtn_Chord.Name = "rbtn_Chord";
            this.rbtn_Chord.Size = new System.Drawing.Size(131, 24);
            this.rbtn_Chord.TabIndex = 1;
            this.rbtn_Chord.Text = "Chord Length";
            this.rbtn_Chord.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1444, 1060);
            this.Controls.Add(this.group_Param);
            this.Controls.Add(this.btn_DoneModify);
            this.Controls.Add(this.group_ModifyInput);
            this.Controls.Add(this.group_ModifyType);
            this.Controls.Add(this.group_AddType);
            this.Controls.Add(this.group_AddInput);
            this.Controls.Add(this.cbox_ShowBackground);
            this.Controls.Add(this.error);
            this.Controls.Add(this.btn_Reset);
            this.Controls.Add(this.btn_Background);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.group_AddInput.ResumeLayout(false);
            this.group_AddInput.PerformLayout();
            this.group_AddType.ResumeLayout(false);
            this.group_ModifyType.ResumeLayout(false);
            this.group_ModifyInput.ResumeLayout(false);
            this.group_ModifyInput.PerformLayout();
            this.group_Param.ResumeLayout(false);
            this.group_Param.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btn_Background;
        private System.Windows.Forms.Button btn_Reset;
        private System.Windows.Forms.Button btn_cPointsAdd;
        private System.Windows.Forms.Button btn_pPointsAdd;
        private System.Windows.Forms.Button btn_LeastSquaresAdd;
        private System.Windows.Forms.Button btn_CompositeAdd;
        private System.Windows.Forms.RadioButton rbtn_MouseAdd;
        private System.Windows.Forms.RadioButton rbtn_TypeAdd;
        private System.Windows.Forms.RadioButton rbtn_FileAdd;
        private System.Windows.Forms.Label error;
        private System.Windows.Forms.CheckBox cbox_ShowBackground;
        private System.Windows.Forms.Button btn_cPointsModify;
        private System.Windows.Forms.Button btn_pPointsModify;
        private System.Windows.Forms.RadioButton rbtn_MouseModify;
        private System.Windows.Forms.RadioButton rbtn_TypeModify;
        private System.Windows.Forms.GroupBox group_AddInput;
        private System.Windows.Forms.GroupBox group_AddType;
        private System.Windows.Forms.GroupBox group_ModifyType;
        private System.Windows.Forms.GroupBox group_ModifyInput;
        private System.Windows.Forms.Button btn_DoneModify;
        private System.Windows.Forms.GroupBox group_Param;
        private System.Windows.Forms.RadioButton rbtn_Chord;
        private System.Windows.Forms.RadioButton rbtn_Uniform;
    }
}

namespace CensusManager
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.listBoxVillage = new System.Windows.Forms.ListBox();
            this.listBoxBuild = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridViewPerson = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPerson)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxVillage
            // 
            this.listBoxVillage.AllowDrop = true;
            this.listBoxVillage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxVillage.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxVillage.FormattingEnabled = true;
            this.listBoxVillage.ItemHeight = 27;
            this.listBoxVillage.Location = new System.Drawing.Point(12, 48);
            this.listBoxVillage.Name = "listBoxVillage";
            this.listBoxVillage.Size = new System.Drawing.Size(169, 571);
            this.listBoxVillage.TabIndex = 2;
            this.listBoxVillage.SelectedIndexChanged += new System.EventHandler(this.listBoxVillage_SelectedIndexChanged);
            this.listBoxVillage.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxVillage_DragDrop);
            this.listBoxVillage.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBoxVillage_DragEnter);
            this.listBoxVillage.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxVillage_MouseDoubleClick);
            // 
            // listBoxBuild
            // 
            this.listBoxBuild.AllowDrop = true;
            this.listBoxBuild.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxBuild.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxBuild.FormattingEnabled = true;
            this.listBoxBuild.ItemHeight = 27;
            this.listBoxBuild.Location = new System.Drawing.Point(187, 48);
            this.listBoxBuild.Name = "listBoxBuild";
            this.listBoxBuild.Size = new System.Drawing.Size(150, 571);
            this.listBoxBuild.TabIndex = 2;
            this.listBoxBuild.SelectedIndexChanged += new System.EventHandler(this.listBoxBuild_SelectedIndexChanged);
            this.listBoxBuild.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxBuild_DragDrop);
            this.listBoxBuild.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBoxBuild_DragEnter);
            this.listBoxBuild.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxBuild_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 26);
            this.label1.TabIndex = 3;
            this.label1.Text = "村庄";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(182, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 26);
            this.label2.TabIndex = 3;
            this.label2.Text = "房屋";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(338, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 26);
            this.label3.TabIndex = 3;
            this.label3.Text = "住户";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // dataGridViewPerson
            // 
            this.dataGridViewPerson.AllowDrop = true;
            this.dataGridViewPerson.AllowUserToAddRows = false;
            this.dataGridViewPerson.AllowUserToDeleteRows = false;
            this.dataGridViewPerson.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPerson.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewPerson.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPerson.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dataGridViewPerson.Location = new System.Drawing.Point(343, 48);
            this.dataGridViewPerson.MultiSelect = false;
            this.dataGridViewPerson.Name = "dataGridViewPerson";
            this.dataGridViewPerson.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(3);
            this.dataGridViewPerson.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewPerson.RowTemplate.Height = 30;
            this.dataGridViewPerson.RowTemplate.ReadOnly = true;
            this.dataGridViewPerson.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewPerson.Size = new System.Drawing.Size(802, 571);
            this.dataGridViewPerson.TabIndex = 5;
            this.dataGridViewPerson.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPerson_CellClick);
            this.dataGridViewPerson.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewPerson_CellMouseDoubleClick);
            this.dataGridViewPerson.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridViewPerson_DragDrop);
            this.dataGridViewPerson.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridViewPerson_DragEnter);
            // 
            // Column1
            // 
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Column1.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column1.HeaderText = "关系";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Column2.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column2.HeaderText = "姓名";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Column3.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column3.HeaderText = "身份证";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 260;
            // 
            // Column4
            // 
            dataGridViewCellStyle5.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Column4.DefaultCellStyle = dataGridViewCellStyle5;
            this.Column4.HeaderText = "地址";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 300;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1157, 635);
            this.Controls.Add(this.dataGridViewPerson);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxBuild);
            this.Controls.Add(this.listBoxVillage);
            this.Name = "Form1";
            this.Text = "户籍管理";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPerson)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxVillage;
        private System.Windows.Forms.ListBox listBoxBuild;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridViewPerson;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    }
}


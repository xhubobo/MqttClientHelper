namespace RecvMessageSample
{
    partial class DrawForm
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
            this.labelDisplay = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panelDrawArea = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.labelRecvNumber = new System.Windows.Forms.Label();
            this.labelRecvTip = new System.Windows.Forms.Label();
            this.checkBoxDraw = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelDisplay
            // 
            this.labelDisplay.AutoSize = true;
            this.labelDisplay.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelDisplay.Location = new System.Drawing.Point(3, 43);
            this.labelDisplay.Name = "labelDisplay";
            this.labelDisplay.Size = new System.Drawing.Size(55, 62);
            this.labelDisplay.TabIndex = 3;
            this.labelDisplay.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "接收数字";
            // 
            // panelDrawArea
            // 
            this.panelDrawArea.BackColor = System.Drawing.Color.LightGray;
            this.panelDrawArea.Location = new System.Drawing.Point(2, 133);
            this.panelDrawArea.Name = "panelDrawArea";
            this.panelDrawArea.Size = new System.Drawing.Size(530, 266);
            this.panelDrawArea.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(130, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "接收次数";
            // 
            // labelRecvNumber
            // 
            this.labelRecvNumber.AutoSize = true;
            this.labelRecvNumber.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelRecvNumber.Location = new System.Drawing.Point(121, 43);
            this.labelRecvNumber.Name = "labelRecvNumber";
            this.labelRecvNumber.Size = new System.Drawing.Size(55, 62);
            this.labelRecvNumber.TabIndex = 3;
            this.labelRecvNumber.Text = "0";
            // 
            // labelRecvTip
            // 
            this.labelRecvTip.Location = new System.Drawing.Point(244, 43);
            this.labelRecvTip.Name = "labelRecvTip";
            this.labelRecvTip.Size = new System.Drawing.Size(248, 51);
            this.labelRecvTip.TabIndex = 2;
            this.labelRecvTip.Text = "接收次数";
            // 
            // checkBoxDraw
            // 
            this.checkBoxDraw.AutoSize = true;
            this.checkBoxDraw.Location = new System.Drawing.Point(246, 18);
            this.checkBoxDraw.Name = "checkBoxDraw";
            this.checkBoxDraw.Size = new System.Drawing.Size(78, 16);
            this.checkBoxDraw.TabIndex = 5;
            this.checkBoxDraw.Text = "DrawImage";
            this.checkBoxDraw.UseVisualStyleBackColor = true;
            // 
            // DrawForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 411);
            this.Controls.Add(this.checkBoxDraw);
            this.Controls.Add(this.panelDrawArea);
            this.Controls.Add(this.labelRecvNumber);
            this.Controls.Add(this.labelDisplay);
            this.Controls.Add(this.labelRecvTip);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(560, 450);
            this.Name = "DrawForm";
            this.Text = "DrawForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DrawForm_FormClosing);
            this.Load += new System.EventHandler(this.DrawForm_Load);
            this.SizeChanged += new System.EventHandler(this.DrawForm_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDisplay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panelDrawArea;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelRecvNumber;
        private System.Windows.Forms.Label labelRecvTip;
        private System.Windows.Forms.CheckBox checkBoxDraw;
    }
}
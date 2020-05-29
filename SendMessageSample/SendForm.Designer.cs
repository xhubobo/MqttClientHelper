namespace SendMessageSample
{
    partial class SendForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelMqttConnState = new System.Windows.Forms.Label();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.textBoxPwd = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxLoop = new System.Windows.Forms.CheckBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBoxFps = new System.Windows.Forms.TextBox();
            this.textBoxStopValue = new System.Windows.Forms.TextBox();
            this.textBoxStartValue = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelMqttConnState);
            this.groupBox1.Controls.Add(this.buttonLogin);
            this.groupBox1.Controls.Add(this.textBoxPwd);
            this.groupBox1.Controls.Add(this.textBoxPort);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBoxUserName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxIp);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(447, 91);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MQTT配置";
            // 
            // labelMqttConnState
            // 
            this.labelMqttConnState.AutoSize = true;
            this.labelMqttConnState.Location = new System.Drawing.Point(355, 59);
            this.labelMqttConnState.Name = "labelMqttConnState";
            this.labelMqttConnState.Size = new System.Drawing.Size(65, 12);
            this.labelMqttConnState.TabIndex = 3;
            this.labelMqttConnState.Text = "MQTT已连接";
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(357, 24);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(75, 23);
            this.buttonLogin.TabIndex = 2;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // textBoxPwd
            // 
            this.textBoxPwd.Location = new System.Drawing.Point(229, 56);
            this.textBoxPwd.Name = "textBoxPwd";
            this.textBoxPwd.Size = new System.Drawing.Size(100, 21);
            this.textBoxPwd.TabIndex = 1;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(45, 56);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(100, 21);
            this.textBoxPort.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(170, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Password";
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(229, 21);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(100, 21);
            this.textBoxUserName.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(170, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "UserName";
            // 
            // textBoxIp
            // 
            this.textBoxIp.Location = new System.Drawing.Point(45, 20);
            this.textBoxIp.Name = "textBoxIp";
            this.textBoxIp.Size = new System.Drawing.Size(100, 21);
            this.textBoxIp.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxLoop);
            this.groupBox2.Controls.Add(this.buttonSend);
            this.groupBox2.Controls.Add(this.textBoxFps);
            this.groupBox2.Controls.Add(this.textBoxStopValue);
            this.groupBox2.Controls.Add(this.textBoxStartValue);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(13, 108);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(444, 93);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "发送";
            // 
            // checkBoxLoop
            // 
            this.checkBoxLoop.AutoSize = true;
            this.checkBoxLoop.Location = new System.Drawing.Point(10, 25);
            this.checkBoxLoop.Name = "checkBoxLoop";
            this.checkBoxLoop.Size = new System.Drawing.Size(72, 16);
            this.checkBoxLoop.TabIndex = 2;
            this.checkBoxLoop.Text = "循环发送";
            this.checkBoxLoop.UseVisualStyleBackColor = true;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(354, 26);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 50);
            this.buttonSend.TabIndex = 2;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBoxFps
            // 
            this.textBoxFps.Location = new System.Drawing.Point(42, 55);
            this.textBoxFps.Name = "textBoxFps";
            this.textBoxFps.Size = new System.Drawing.Size(100, 21);
            this.textBoxFps.TabIndex = 1;
            // 
            // textBoxStopValue
            // 
            this.textBoxStopValue.Location = new System.Drawing.Point(226, 55);
            this.textBoxStopValue.Name = "textBoxStopValue";
            this.textBoxStopValue.Size = new System.Drawing.Size(100, 21);
            this.textBoxStopValue.TabIndex = 1;
            // 
            // textBoxStartValue
            // 
            this.textBoxStartValue.Location = new System.Drawing.Point(226, 23);
            this.textBoxStartValue.Name = "textBoxStartValue";
            this.textBoxStartValue.Size = new System.Drawing.Size(100, 21);
            this.textBoxStartValue.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(167, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "结束值";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "帧率";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(167, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "开始值";
            // 
            // SendForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 216);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "SendForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MQTT消息发送";
            this.Load += new System.EventHandler(this.SendForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxIp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPwd;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Label labelMqttConnState;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxLoop;
        private System.Windows.Forms.TextBox textBoxStartValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxFps;
        private System.Windows.Forms.TextBox textBoxStopValue;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonSend;
    }
}



namespace TronLightCycleGameUdpTool
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnSpawn = new System.Windows.Forms.Button();
            this.btnDead = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.inputIRemotePAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.inputRemotePort = new System.Windows.Forms.TextBox();
            this.inputLocalPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboPlayerId = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(65, 129);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(48, 41);
            this.btnUp.TabIndex = 0;
            this.btnUp.Text = "↑";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(110, 176);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(48, 41);
            this.btnRight.TabIndex = 1;
            this.btnRight.Text = "→";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(65, 223);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(48, 41);
            this.btnDown.TabIndex = 3;
            this.btnDown.Text = "↓";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(17, 176);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(48, 41);
            this.btnLeft.TabIndex = 2;
            this.btnLeft.Text = "←";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnSpawn
            // 
            this.btnSpawn.Location = new System.Drawing.Point(174, 129);
            this.btnSpawn.Name = "btnSpawn";
            this.btnSpawn.Size = new System.Drawing.Size(99, 41);
            this.btnSpawn.TabIndex = 4;
            this.btnSpawn.Text = "Spawn";
            this.btnSpawn.UseVisualStyleBackColor = true;
            this.btnSpawn.Click += new System.EventHandler(this.btnSpawn_Click);
            // 
            // btnDead
            // 
            this.btnDead.Location = new System.Drawing.Point(174, 176);
            this.btnDead.Name = "btnDead";
            this.btnDead.Size = new System.Drawing.Size(99, 41);
            this.btnDead.TabIndex = 5;
            this.btnDead.Text = "Die";
            this.btnDead.UseVisualStyleBackColor = true;
            this.btnDead.Click += new System.EventHandler(this.btnDead_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "宛先アドレス";
            // 
            // inputIRemotePAddress
            // 
            this.inputIRemotePAddress.Location = new System.Drawing.Point(84, 10);
            this.inputIRemotePAddress.Name = "inputIRemotePAddress";
            this.inputIRemotePAddress.Size = new System.Drawing.Size(100, 19);
            this.inputIRemotePAddress.TabIndex = 7;
            this.inputIRemotePAddress.Text = "127.0.0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(200, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "宛先ポート";
            // 
            // inputRemotePort
            // 
            this.inputRemotePort.Location = new System.Drawing.Point(263, 10);
            this.inputRemotePort.Name = "inputRemotePort";
            this.inputRemotePort.Size = new System.Drawing.Size(100, 19);
            this.inputRemotePort.TabIndex = 9;
            this.inputRemotePort.Text = "30000";
            // 
            // inputLocalPort
            // 
            this.inputLocalPort.Location = new System.Drawing.Point(263, 35);
            this.inputLocalPort.Name = "inputLocalPort";
            this.inputLocalPort.Size = new System.Drawing.Size(100, 19);
            this.inputLocalPort.TabIndex = 11;
            this.inputLocalPort.Text = "30001";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(200, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "受信ポート";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "playerId";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboPlayerId
            // 
            this.comboPlayerId.FormattingEnabled = true;
            this.comboPlayerId.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.comboPlayerId.Location = new System.Drawing.Point(84, 63);
            this.comboPlayerId.Name = "comboPlayerId";
            this.comboPlayerId.Size = new System.Drawing.Size(100, 20);
            this.comboPlayerId.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 470);
            this.Controls.Add(this.comboPlayerId);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.inputLocalPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.inputRemotePort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.inputIRemotePAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDead);
            this.Controls.Add(this.btnSpawn);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnUp);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnSpawn;
        private System.Windows.Forms.Button btnDead;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox inputIRemotePAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox inputRemotePort;
        private System.Windows.Forms.TextBox inputLocalPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboPlayerId;
    }
}


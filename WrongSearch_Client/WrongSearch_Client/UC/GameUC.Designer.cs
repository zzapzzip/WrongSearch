﻿namespace WrongSearch_Client.UC
{
    partial class GameUC
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tbChatSend = new System.Windows.Forms.TextBox();
            this.rtbChatting = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pnBoard = new System.Windows.Forms.Panel();
            this.pbRight = new System.Windows.Forms.PictureBox();
            this.pbLeft = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnResult = new System.Windows.Forms.Panel();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeft)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(882, 601);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // tbChatSend
            // 
            this.tbChatSend.Location = new System.Drawing.Point(752, 562);
            this.tbChatSend.Name = "tbChatSend";
            this.tbChatSend.Size = new System.Drawing.Size(338, 21);
            this.tbChatSend.TabIndex = 1;
            this.tbChatSend.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbChatSend_KeyPress);
            // 
            // rtbChatting
            // 
            this.rtbChatting.BackColor = System.Drawing.Color.White;
            this.rtbChatting.Location = new System.Drawing.Point(752, 426);
            this.rtbChatting.Name = "rtbChatting";
            this.rtbChatting.ReadOnly = true;
            this.rtbChatting.Size = new System.Drawing.Size(338, 130);
            this.rtbChatting.TabIndex = 2;
            this.rtbChatting.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "label3";
            // 
            // pnBoard
            // 
            this.pnBoard.Location = new System.Drawing.Point(752, 103);
            this.pnBoard.Name = "pnBoard";
            this.pnBoard.Size = new System.Drawing.Size(338, 300);
            this.pnBoard.TabIndex = 11;
            // 
            // pbRight
            // 
            this.pbRight.Location = new System.Drawing.Point(389, 103);
            this.pbRight.Name = "pbRight";
            this.pbRight.Size = new System.Drawing.Size(330, 480);
            this.pbRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbRight.TabIndex = 4;
            this.pbRight.TabStop = false;
            this.pbRight.Paint += new System.Windows.Forms.PaintEventHandler(this.pbRight_Paint);
            this.pbRight.MouseLeave += new System.EventHandler(this.pbRight_MouseLeave);
            this.pbRight.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbRight_MouseMove);
            this.pbRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbRight_MouseUp);
            // 
            // pbLeft
            // 
            this.pbLeft.Location = new System.Drawing.Point(32, 103);
            this.pbLeft.Name = "pbLeft";
            this.pbLeft.Size = new System.Drawing.Size(330, 480);
            this.pbLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLeft.TabIndex = 3;
            this.pbLeft.TabStop = false;
            this.pbLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.pbLeft_Paint);
            this.pbLeft.MouseLeave += new System.EventHandler(this.pbLeft_MouseLeave);
            this.pbLeft.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbLeft_MouseMove);
            this.pbLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbLeft_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(337, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 24);
            this.label2.TabIndex = 13;
            this.label2.Text = "Count";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pnResult
            // 
            this.pnResult.Location = new System.Drawing.Point(121, 147);
            this.pnResult.Name = "pnResult";
            this.pnResult.Size = new System.Drawing.Size(523, 365);
            this.pnResult.TabIndex = 15;
            this.pnResult.Visible = false;
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // GameUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::WrongSearch_Client.etc.lobby_background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.pnResult);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnBoard);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pbRight);
            this.Controls.Add(this.pbLeft);
            this.Controls.Add(this.rtbChatting);
            this.Controls.Add(this.tbChatSend);
            this.Controls.Add(this.progressBar1);
            this.Name = "GameUC";
            this.Size = new System.Drawing.Size(1200, 700);
            ((System.ComponentModel.ISupportInitialize)(this.pbRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeft)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox tbChatSend;
        public System.Windows.Forms.RichTextBox rtbChatting;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Panel pnBoard;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.PictureBox pbLeft;
        public System.Windows.Forms.PictureBox pbRight;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Panel pnResult;
        public System.Windows.Forms.Timer timer2;
    }
}

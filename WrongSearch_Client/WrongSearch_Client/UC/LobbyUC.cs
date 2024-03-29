﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;

namespace WrongSearch_Client.UC
{
    public partial class LobbyUC : UserControl
    {
        // LobbyUC에서 사용되는 전역변수를 관리합니다.
        #region 변수 모음

        ClientForm clientForm = null;

        #endregion

        // Form의 켜짐에 관련된 함수를 관리합니다.
        #region Form Load

        public LobbyUC(ClientForm cf)
        {
            InitializeComponent();
            clientForm = cf;
        }

        #endregion

        // 클라이언트에서 서버로 버퍼를 보내는 함수를 관리합니다.
        #region Send~

        //private void 대전신청ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    ListViewItem lvt = lvWaiting.SelectedItems[0]; // 선택한 유저를 받아옴

        //    // 자신에게 대전신청을 했을 경우
        //    if (clientForm.user.classNumber == lvt.SubItems[1].Text)
        //    {
        //        MessageBox.Show("자기자신과의 대전은 할 수 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }
        //    // 상대방이 이미 게임 중인 경우
        //    if (lvt.SubItems[4].Text != "대기")
        //    {
        //        MessageBox.Show("이미 게임 중인 사용자입니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //    DialogResult dr = MessageBox.Show(lvt.SubItems[0].Text + "님에게 대전신청을 하시겠습니까?",
        //        "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        //    if (dr == DialogResult.Yes)
        //    {
        //        // msg: battle√이름√학번√받는학번√받는이름√
        //        NetworkStream ns = clientForm.clientSocket.GetStream();
        //        string msg = "battle√" + clientForm.user.name + "√" + clientForm.user.classNumber + "√" +
        //            lvt.SubItems[1].Text + "√" + lvt.SubItems[0].Text + "√";
        //        byte[] buf = Encoding.UTF8.GetBytes(msg);
        //        ns.Write(buf, 0, buf.Length);
        //        ns.Flush();
        //    }

        //    // 중복안되는 방번호 배정
        //    // "대기" -> 방번호
        //    // 버퍼 전송
        //    // pnBoard 변경
        //    // chatOn 변경
        //}

        #endregion

        // 기타함수를 관리합니다.
        #region Etc

        // 사용자를 선택한 후, 오른쪽클릭을 하면 `대전신청`을 할 수 있습니다.
        private void lvWaiting_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right &&
                lvWaiting.SelectedItems.Count == 1)
            {
                cmsBattle.Show((Control)sender, e.Location);
            }
        }

        #endregion

        // 방만들기
        private void btnCreate_Click(object sender, EventArgs e)
        {
            // 대기실 -> 게임진행/
            clientForm.gameUC.rtbChatting.Clear();
            clientForm.gameUC.pnBoard.Controls.Clear();
            clientForm.statementUC = new StatementUC(clientForm);
            clientForm.gameUC.pnBoard.Controls.Add(clientForm.statementUC);
            clientForm.gameUC.BackgroundImage = etc.waiting_background;

            // 플레이어 이름
            clientForm.statementUC.lbUserName1.Text = clientForm.user.name;
            clientForm.statementUC.lbClassNumber1.Text = clientForm.user.classNumber;

            /////////////////////////////
            // 서버에 방상태 전송
            // 방번호변경, 플레이어순서, 플레이어 숫자
            NetworkStream ns = clientForm.clientSocket.GetStream();
            string msg = "roomCreate√" + clientForm.user.classNumber + "√";
            byte[] buf = Encoding.UTF8.GetBytes(msg);
            ns.Write(buf, 0, buf.Length);
            ns.Flush();
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            if (lvWaiting.SelectedItems.Count != 0)
            {
                ListViewItem lvt = lvWaiting.SelectedItems[0]; // 선택한 유저를 받아옴
                if (clientForm.user.classNumber == lvt.SubItems[1].Text)
                {
                    MessageBox.Show("자기자신과의 대전은 할 수 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // 상대방이 이미 게임 중인 경우
                if (lvt.SubItems[4].Text == "대기")
                {
                    MessageBox.Show("방이 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // msg: JoinRoom√상대학번√상대이름√
                NetworkStream ns = clientForm.clientSocket.GetStream();
                string msg = "JoinRoom√" + lvt.SubItems[1].Text + "√" + lvt.SubItems[0].Text + "√";
                byte[] buf = Encoding.UTF8.GetBytes(msg);
                ns.Write(buf, 0, buf.Length);
                ns.Flush();
            }
            else
            {
                MessageBox.Show("참가하려는 방을 선택하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 정보보기ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Resources;

using WrongSearch_Client.UC;
using WrongSearch_Server.Class;
using WrongSearch_Client.Class;
using System.Media;
using System.IO;

namespace WrongSearch_Client
{
    public partial class ClientForm : Form
    {
        // ClientForm에서 사용되는 전역변수를 관리합니다.
        #region 변수 모음

        public TcpClient clientSocket = new TcpClient();

        public clsPerson user = new clsPerson(); // 로그인한 사람의 정보를 보관할 clsPerson
        public clsPerson opponent = new clsPerson(); //상대방 정보
        public Thread thr = null;

        public RegistForm registForm = null;
        public LoginUC loginUC = null; // LoginUC와 연결
        public GameUC gameUC = null; // GameUC와 연결
        public LobbyUC lobbyUC = null; //LobbyUC와 연결
        public StatementUC statementUC = null;//SatementUC와 연결
        public ResultUC resultUC = null;//ResultUC와 연결

        public string roomNumber = "0";   //숫자:방번호 0:로비 -1:로그인
        public string[] pictureNum ;         //받아온 그림 번호 배열

        public List<Answer> answer = new List<Answer>();    //정답관리

        public SoundPlayer player = new SoundPlayer(); // 음악을 재생합니다.

        public Point mouse_point;

        #endregion

        // Form의 켜짐과 꺼짐에 관련된 함수를 관리합니다.
        #region Form Load & Closing

        public ClientForm()
        {
            InitializeComponent();
        }

        // 폼 로드시, loginUC를 패널에 넣은 후,
        // 클라이언트도 버퍼를 받아들이는 쓰레드를 실행한다.
        private void ClientForm_Load(object sender, EventArgs e)
        {
            loginUC = new LoginUC(this);
            pnMain.Controls.Add(loginUC);

            player.Stop();
            player.Stream = etc.opening;
            player.PlayLooping();

            thr = new Thread(RecvBuffer);
            thr.Start();
        }

        // 프로그램 종료시 모든 쓰레드 종료
        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                NetworkStream ns = clientSocket.GetStream();

                // 방종료
                // 방에 들어가있을경우
                if (user.room[0] > 0)
                {
                    string msg = "exitRoom√" + user.classNumber + "," + user.name + "," + user.room[0] + "√";
                    byte[] buf = Encoding.UTF8.GetBytes(msg);

                    ns.Write(buf, 0, buf.Length);
                    ns.Flush();
                }

                // 클라이언트 종료
                string msg2 = "exit√";
                byte[] buf2 = Encoding.UTF8.GetBytes(msg2);

                ns.Write(buf2, 0, buf2.Length);
                ns.Flush();
            }
            catch { }
            
            Application.ExitThread();
            Environment.Exit(0);
        }

        #endregion

        // 반복문을 사용하여 서버에서 클라이언트로 버퍼를 받아들입니다.
        #region RecvBuffer

        // 모든 버퍼를 받아들여, 처리합니다.
        private void RecvBuffer()
        {
            while (true)
            {
                byte[] buf = new byte[1024];
                string msg = null;

                try
                {
                    NetworkStream ns = clientSocket.GetStream();

                    ns.Read(buf, 0, buf.Length);
                    msg = Encoding.UTF8.GetString(buf);

                    // 여기부터 받아오는 버퍼의 종류마다 다르게 처리.
                    RecvConnect(msg);
                    RecvRegist(msg);
                    RecvLogin(msg);
                    RecvHello(msg);
                    RecvChat(msg);
                    //RecvBattle(msg);
                    //RecvRoom(msg);
                    RecvRoomCreated(msg);
                    RecvRoomJoin(msg);
                    RecvRandom(msg);
                    RecvAnwser(msg);
                    RecvReady(msg);
                    RecvStatement(msg);
                    RecvGameEnd(msg);
                    RecvExit(msg);
                    RecvExitRoom(msg);
                    RecvClientOUT(msg);
                    RecvResult(msg);
                }
                catch (Exception e) {//MessageBox.Show(e.Message); 
                }
            }
        }

        

        #region Recv~

        // 연결가능 여부를 받아옵니다.
        // msg: true or false√
        private void RecvConnect(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "True")
            {
                loginUC.connectOK = true;

                loginUC.ConnectTry();
            }
        }

        // 회원가입
        private void RecvRegist(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "regist")
            {
                registForm.SendRegist2(msgArray[1]);
            }
        }

        // 로그인 성공여부
        // msg: login√pass√학번√이름√ or login√fail√ or login√duplicate√학번
        private void RecvLogin(string msg)
        {
            string[] msgArray = msg.Split('√');
            
            if (msgArray[0] == "login")
            {
                NetworkStream ns = clientSocket.GetStream();
                byte[] buf = null;
                string send = null;
                
                if (msgArray[1] == "fail")
                {
                    MessageBox.Show("입력한 정보가 올바르지 않습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // 로그인 실패 시, 실패 버퍼를 보냄.
                    send = "fail√";
                    buf = Encoding.UTF8.GetBytes(send);
                    ns.Write(buf, 0, buf.Length);
                    ns.Flush();

                    return;
                }
                else if (msgArray[1] == "duplicate")
                {
                    MessageBox.Show("이미 접속 중입니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    // 로그인 실패 시, 실패 버퍼를 보냄.
                    send = "fail√";
                    buf = Encoding.UTF8.GetBytes(send);
                    ns.Write(buf, 0, buf.Length);
                    ns.Flush();

                    return;
                }

                user.classNumber = msgArray[2];
                user.name = msgArray[3];
                user.gender = msgArray[4];
                user.elo = Convert.ToInt32(msgArray[5]);

                // 채팅켜라
                send = "chatOn√";
                buf = Encoding.UTF8.GetBytes(send);
                ns.Write(buf, 0, buf.Length);
                ns.Flush();

                MessageBox.Show(msgArray[3] + "님\n환영합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Invoke(new GameStartDelegate(GameStart));
            }
        }

        private void RecvHello(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "hello")
            {
                this.Invoke(new HelloDelegate(_RecvHello), new object[] { msgArray[1] });
                this.Invoke(new WaitDelegate(_RecvWait), new object[] { msg });
            }
        }
        
        private void RecvChat(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "chat")
            {
                this.Invoke(new ChatDelegate(_RecvChat), new object[] { msgArray[1] });
            }
        }

        // msg: battle√상대방이름√상대방학번√학번√이름√
        // msg: battleYes√상대방학번√학번√방번호√ or battleNo√학번√
        //private void RecvBattle(string msg)
        //{
        //    string[] msgArray = msg.Split('√');

        //    if (msgArray[0] == "battle")
        //    {
        //        DialogResult dr = MessageBox.Show(msgArray[1] + "(" + msgArray[2] + ")님이 대전신청을 하셨습니다.\n" +
        //            "수락하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        //        NetworkStream ns = clientSocket.GetStream();
        //        string msg2 = null;
        //        byte[] buf = null;  
        //        if (dr == DialogResult.Yes)
        //        {
        //            // 수락
        //            msg2 = "battleYes√" + msgArray[1] + "√" + msgArray[2] + "√" + msgArray[3] + "√" + msgArray[4] + "√";
        //            buf = Encoding.UTF8.GetBytes(msg2);
        //            ns.Write(buf, 0, buf.Length);
        //            ns.Flush();

        //            //수락시 변경
        //            user.room[1] = 1;

        //            //lobbyUC.lvWaiting.
        //            this.Invoke(new BattleDelegate(_RecvBattle), new object[] { msg });
        //        }
        //        else if (dr == DialogResult.No)
        //        {
        //            // 거절
        //            msg2 = "battleNo√" + msgArray[2] + "√";
        //            buf = Encoding.UTF8.GetBytes(msg2);
        //            ns.Write(buf, 0, buf.Length);
        //            ns.Flush();
        //        }
        //    }
        //    else if (msgArray[0] == "battleYes")
        //    {
        //        MessageBox.Show("수락");
        //        //수락시 변경
        //        user.room[1] = 0;

        //        this.Invoke(new BattleDelegate(_RecvBattle), new object[] { msg });
        //    }
        //    else if (msgArray[0] == "battleNo")
        //    {
        //        MessageBox.Show("거절");
        //    }
        //}

        //방번호를 받고 리스트뷰 수정
        //private void RecvRoom(string msg)
        //{
        //    string[] msgArray = msg.Split('√');

        //    if (msgArray[0] == "room")
        //    {
        //        if (msgArray[2] == user.classNumber || msgArray[3] == user.classNumber)
        //        {
        //            //roomNumber = msgArray[1];
        //            //user.room = Convert.ToInt32(roomNumber); //사용자의 방번호
        //            user.room[0] = Convert.ToInt32(msgArray[1]);
        //        }
        //        else
        //        {
        //            for (int i = 0; i < lobbyUC.lvWaiting.Items.Count; i++)
        //            {
        //                if (lobbyUC.lvWaiting.Items[i].SubItems[1].Text == msgArray[2] ||
        //                    lobbyUC.lvWaiting.Items[i].SubItems[1].Text == msgArray[3])
        //                {
        //                    lobbyUC.lvWaiting.Items[i].SubItems[4].Text = msgArray[1] + "번방";
        //                }
        //            }
        //            lobbyUC.lvWaiting.Refresh();
        //        }
        //    }
        //}

        // 방만들기를 눌렀을때 서버로부터 방번호를 받아옵니다.
        // msg: roomCreated√RoomNumber√방장_학번√방장_학번_대기실용√
        private void RecvRoomCreated(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "roomCreated")
            {
                if (msgArray[2] == user.classNumber)
                {
                    user.room[0] = int.Parse(msgArray[1]);
                }
                else if(user.room[0] == 0)
                {
                    this.Invoke(new LobbyRefreshDelegate(_LobbyRefresh), new object[] { msg });
                }
            }
        }

        //msg: JoinRoom√방번호1√방장학번2√방장이름3√접속자학번4√접속자이름5√접속자elo6√방장elo7
        //msg:
        private void RecvRoomJoin(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "JoinRoom")
            {
                
                if (msgArray[4] == user.classNumber)        //접속자일때
                {
                    opponent.classNumber = msgArray[2];
                    opponent.elo = Convert.ToInt32(msgArray[7]);
                    user.room[0] = int.Parse(msgArray[1]);
                    user.room[1] = 1;
                    this.Invoke(new BattleDelegate(_RecvBattle), new object[] { msg });

                }
                else if (msgArray[2] == user.classNumber)       //방장일때
                {
                    opponent.classNumber = msgArray[4];
                    opponent.elo = Convert.ToInt32(msgArray[6]);
                    statementUC.lbUserName2.Text = msgArray[5];
                    statementUC.lbClassNumber2.Text = msgArray[4];
                }
                else if (user.room[0] == 0)
                {
                    this.Invoke(new LobbyRefreshDelegate(_LobbyRefresh2), new object[] { msg });
                }
            }
            else if (msgArray[0] == "JoinRoomFalse")
            {
                MessageBox.Show("방이 꽉 찼습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RecvAnwser(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "ans")
            {
                for (int i = 0; i < 25 ; i++)
                {
                    Point p = new Point(int.Parse(msgArray[i * 6 + 3]), int.Parse(msgArray[i * 6 + 4]));

                    Answer answer1 = new Answer
                    {
                        fname = ("picture"+msgArray[i * 6 + 1]+"_1.PNG"),
                        num = int.Parse(msgArray[i * 6 + 2]),
                        Spoint = p,
                        W = int.Parse(msgArray[i * 6 + 5]),
                        H = int.Parse(msgArray[i * 6 + 6]),
                        check = false
                    };

                    answer.Add(answer1);
                }
            }
        }

        private void RecvRandom(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "start")
            {
                if (msgArray[1] == "시작")
                {
                    this.Invoke(new RandomDelegate(_RecvRandom), new object[] { msg });
                }
            }
        }

        private void RecvReady(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "ready")
            {
                this.Invoke(new ReadyDelegate(_RecvReady), new object[] { msgArray[1] });
                if (msgArray[1] == "시작")
                {
                    this.Invoke(new RandomDelegate(_RecvRandom), new object[] { msg });
                }
            }
        }

        private void RecvStatement(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "statement")
            {
                this.Invoke(new StatementDelegate(_RecvStatement), new object[] { msg });
            }
        }

        private void RecvResult(string msg)
        {
            string[] msgArray = msg.Split('√');
            if (msgArray[0] == "result")
            {
                this.Invoke(new ResultDelegate(_RecvResult), new object[] { msg });
            }
        }

        private void RecvGameEnd(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "gameend")
            {
                gameUC.stageNum = 3;

                gameUC.answerN = 0;
                gameUC.Cpoint = new Point(0, 0);

                user.room[0] = 0;

                gameUC.circle.RemoveRange(0, gameUC.circle.Count);
                answer.RemoveRange(0, answer.Count);

                gameUC.pbLeft.Invalidate();
                gameUC.pbRight.Invalidate();

                gameUC.isRunning = false;

                this.Invoke(new ExitRoomDelegate(_RecvExitRoom), new object[] { statementUC.lbClassNumber1 + "," + statementUC.lbUserName1 + "," + user.room[0], true });
                this.Invoke(new ExitRoomDelegate(_RecvExitRoom), new object[] { statementUC.lbClassNumber2 + "," + statementUC.lbUserName2 + "," + user.room[0], true });
                this.Invoke(new WaitDelegate(_RecvWait), new object[] { msg });
            }

        }

        private void RecvExit(string msg)
        {
            string[] msgArray = msg.Split('√');
            if (msgArray[0] == "exit")
            {
                this.Invoke(new ExitDelegate(_RecvExit), new object[] { msgArray[1] });
                this.Invoke(new WaitDelegate(_RecvWait), new object[] { msg });
            }
        }

        // 방을 나갑니다.
        // msg: exitRoom√나가는 사람의 학번 + 이름+방번호√
        private void RecvExitRoom(string msg)
        {
            string[] msgArray = msg.Split('√');
            string[] msgArray2 = msgArray[1].Split(',');
            if (msgArray[0] == "exitRoom")
            {
                //user.room[0] = 0;
                this.Invoke(new ExitRoomDelegate(_RecvExitRoom), new object[] { msgArray[1], (msgArray2[0] == user.classNumber)?true:false });
                this.Invoke(new WaitDelegate(_RecvWait), new object[] { msg });
            }
        }

        private void RecvClientOUT(string msg)
        {
            string[] msgArray = msg.Split('√');

            if (msgArray[0] == "ClientOUT")
            {
                MessageBox.Show("서버와의 연결이 끊겼습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Invoke(new ClientOUTDelegate(_RecvClientOUT));
            }
        }

        #endregion

        #endregion

        // 크로스 스레딩 오류를 방지하는 Delegate와 본 함수를 관리합니다.
        #region RecvDelegate

        // 로그인 성공 시,
        // 게임모드로 변환하기 위한 창 크기 변환
        delegate void GameStartDelegate();
        private void GameStart()
        {
            this.Text = "틀린그림찾기";
            this.Size = new Size(1200, 700);
            pnMain.Size = new Size(1190, 669);
            this.Location = new Point(50, 30);
            pbExit.Location = new Point(pnMenuBar.Width - 30, pbExit.Location.Y);
            pbMinimum.Location = new Point(pbExit.Location.X - 30, pbMinimum.Location.Y);

            pnMain.Controls.Clear();
            gameUC = new GameUC(this);
            pnMain.Controls.Add(gameUC);

            lobbyUC = new LobbyUC(this);
            gameUC.pnBoard.Controls.Add(lobbyUC);

            resultUC = new ResultUC(this);
            resultUC.label1.Text = gameUC.result;
            gameUC.pnResult.Controls.Add(resultUC);
            resultUC.label1.Text = gameUC.result;
            gameUC.pnResult.Visible = false;
        }

        delegate void HelloDelegate(string msg);
        private void _RecvHello(string msg)
        {
            gameUC.rtbChatting.AppendText("\n" + msg);
            gameUC.rtbChatting.ScrollToCaret();
        }

        // msg: hello√접속메세지√이름√학번√성별√승수√방번호√
        // msg: exit√퇴장메세지√..................√
        // msg: exitRoom√나가는 사람 학번+이름+방번호√....................√
        delegate void WaitDelegate(string msg);
        private void _RecvWait(string msg)
        {
            lobbyUC.lvWaiting.Items.Clear();
            string[] msgArray = msg.Split('√');
            int wait_person = 0;

            //이름학번성별승
            for (int i = 2; i < msgArray.Length - 1; wait_person++)
            {
                string[] pArray = new string[5];
                pArray[0] = msgArray[i++];
                pArray[1] = msgArray[i++];
                pArray[2] = msgArray[i++];
                pArray[3] = msgArray[i++];
                int roomNum = int.Parse(msgArray[i++]);
                if (roomNum == 0)
                    pArray[4] = "대기";
                else
                    pArray[4] = roomNum.ToString() + "번방";
                ListViewItem newList = new ListViewItem(pArray);
                lobbyUC.lvWaiting.Items.Add(newList);
            }

            try
            {
                if (player.Stream.Length != etc.opening.Length)
                {
                    player.Stop();
                    player.Stream = etc.opening;
                    player.PlayLooping();
                }
            }
            catch
            {
                player.Stop();
                player.Stream = etc.opening;
                player.PlayLooping();
            }
        }

        // 서버로 부터 전해진 채팅버퍼를 처리합니다.
        // msg: chat√채팅내용
        delegate void ChatDelegate(string msg);
        private void _RecvChat(string msg)
        {
            gameUC.rtbChatting.AppendText("\n" + msg);
            gameUC.rtbChatting.ScrollToCaret();
        }

        //msg: JoinRoom√방번호√방장학번√방장이름√접속자학번√접속자이름
        delegate void BattleDelegate(string msg);
        private void _RecvBattle(string msg)
        {
            string[] msgArray = msg.Split('√');

            gameUC.rtbChatting.Clear();
            gameUC.pnBoard.Controls.Clear();
            statementUC = new StatementUC(this);
            gameUC.pnBoard.Controls.Add(statementUC);
            gameUC.BackgroundImage = etc.waiting_background;

            statementUC.lbUserName1.Text = msgArray[3];
            statementUC.lbClassNumber1.Text = msgArray[2];
            statementUC.lbUserName2.Text = user.name;
            statementUC.lbClassNumber2.Text = user.classNumber;

            ////label에 플레이어 이름 표시
            //switch (user.room[1])
            //{
            //    case 0:
            //        statementUC.lbUserName1.Text = user.name;
            //        statementUC.lbUserName2.Text = msgArray[4];
            //        break;
            //    case 1:
            //        statementUC.lbUserName1.Text = msgArray[2];
            //        statementUC.lbUserName2.Text = user.name;
            //        break;
            //}
        }

        delegate void RandomDelegate(string msg);
        private void _RecvRandom(string msg)
        {
            gameUC.label3.Text = msg;
            pictureNum = msg.Split('√');

            player.Stop();
            player.Stream = etc.countdown;
            player.Play();
            Thread.Sleep(3000);

            ResourceManager rm = Properties.Resources.ResourceManager;
            gameUC.pbLeft.Image = (Image)rm.GetObject("picture" + pictureNum[3] + "_1");
            gameUC.pbRight.Image = (Image)rm.GetObject("picture" + pictureNum[3] + "_2");

            player.Stop();
            player.Stream = etc.gaming;
            player.PlayLooping();

            gameUC.BackgroundImage = etc.start_background;

            gameUC.result = "패배";
            gameUC.isRunning = true;
        }

        delegate void LobbyRefreshDelegate(string msg);
        private void _LobbyRefresh(string msg)
        {
            string[] msgArray = msg.Split('√');

            for (int i = 0; i < lobbyUC.lvWaiting.Items.Count; i++)
            {
                if (lobbyUC.lvWaiting.Items[i].SubItems[1].Text == msgArray[3])
                {
                    lobbyUC.lvWaiting.Items[i].SubItems[4].Text = msgArray[1] + "번방";
                }
            }
            lobbyUC.lvWaiting.Refresh();
        }

        delegate void LobbyRefreshDelegate2(string msg);
        private void _LobbyRefresh2(string msg)
        {
            string[] msgArray = msg.Split('√');

            for (int i = 0; i < lobbyUC.lvWaiting.Items.Count; i++)
            {
                if (lobbyUC.lvWaiting.Items[i].SubItems[1].Text == msgArray[2] ||
                     lobbyUC.lvWaiting.Items[i].SubItems[1].Text == msgArray[4])
                {
                    lobbyUC.lvWaiting.Items[i].SubItems[4].Text = msgArray[1] + "번방";
                }
            }
            lobbyUC.lvWaiting.Refresh();
        }

        delegate void ResultDelegate(string msg);
        private void _RecvResult(string msg)
        {
            resultUC = new ResultUC(this);
            resultUC.label1.Text = gameUC.result;
            gameUC.pnResult.Controls.Clear();
            gameUC.pnResult.Controls.Add(resultUC);
            gameUC.pnResult.Visible = true; 
        }

        delegate void ReadyDelegate(string msg);
        private void _RecvReady(string msg)
        {
            gameUC.label1.Text = msg;
        }
        delegate void StatementDelegate(string msg);
        private void _RecvStatement(string msg)
        {
            Point pTemp = new Point();
            string[] msgArray = msg.Split('√');
            if (Convert.ToInt32(msgArray[2]) == 0)
            {
                pTemp.X = Convert.ToInt32(msgArray[3]);
                pTemp.Y = statementUC.pbUser1.Location.Y;

                statementUC.pbUser1.Location = pTemp;
            }
            else if (Convert.ToInt32(msgArray[2]) == 1)
            {
                pTemp.X = Convert.ToInt32(msgArray[3]);
                pTemp.Y = statementUC.pbUser2.Location.Y;

                statementUC.pbUser2.Location = pTemp;
            }
        }
        delegate void ExitDelegate(string msg);
        private void _RecvExit(string msg)
        {
            gameUC.rtbChatting.AppendText("\n" + msg);
            gameUC.rtbChatting.ScrollToCaret();
        }

        delegate void ExitRoomDelegate(string class_name,  bool check);
        private void _RecvExitRoom(string class_name,  bool check)//check==false면 남는사람
        {
            string[] msgArray2 = class_name.Split(',');
            if (!check)
            {
                if (statementUC.lbUserName1.Text == msgArray2[1] && statementUC.lbClassNumber1.Text == msgArray2[0])
                {
                    statementUC.lbUserName1.Text = statementUC.lbUserName2.Text;
                    statementUC.lbClassNumber1.Text = statementUC.lbClassNumber2.Text;
                    statementUC.lbUserName2.Text = "";
                    statementUC.lbClassNumber2.Text = "";
                }
                else if (statementUC.lbUserName2.Text == msgArray2[1] && statementUC.lbClassNumber2.Text == msgArray2[0])
                {
                    statementUC.lbUserName2.Text = "";
                    statementUC.lbClassNumber2.Text = "";
                }
                return ;
            }
            //gameUC.rtbChatting.Clear();
            gameUC.pbLeft.Image = null;
            gameUC.pbRight.Image = null;
            gameUC.isRunning = false;

            gameUC.answerN = 0;
            gameUC.stageNum = 3;
            gameUC.Cpoint = new Point(0, 0);

            answer.RemoveRange(0, gameUC.answer.Count);
            gameUC.circle.RemoveRange(0, gameUC.circle.Count);

            gameUC.pbLeft.Invalidate();
            gameUC.pbRight.Invalidate();

            gameUC.pnBoard.Controls.Clear();
            lobbyUC = new LobbyUC(this);
            gameUC.pnBoard.Controls.Add(lobbyUC);

            gameUC.BackgroundImage = etc.lobby_background;
        }

        delegate void ClientOUTDelegate();
        private void _RecvClientOUT()
        {
            this.Close();
        }

        #endregion

        private void pnMenuBar_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_point = new Point(e.X, e.Y);
        }

        private void pnMenuBar_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Location = new Point(this.Left - (mouse_point.X - e.X), this.Top - (mouse_point.Y - e.Y));
            }
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbMinimum_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}


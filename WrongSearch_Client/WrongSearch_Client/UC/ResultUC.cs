﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;

namespace WrongSearch_Client.UC
{
    public partial class ResultUC : UserControl
    {
        private GameUC gameUC;
        private ClientForm clientForm;
        

        public ResultUC()
        {
            InitializeComponent();
        }

        public ResultUC(ClientForm clientForm)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            this.clientForm = clientForm;
            clientForm.gameUC.pnResult.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NetworkStream ns = clientForm.clientSocket.GetStream();

            //gameend√학번 √
            string msg = "gameend√" + clientForm.user.room[0] + "√" + clientForm.user.room[1] + "√";
            byte[] buf = Encoding.UTF8.GetBytes(msg);

            ns.Write(buf, 0, buf.Length);
            ns.Flush();

            this.Visible = false;
            clientForm.gameUC.pnResult.Controls.Clear();
            clientForm.gameUC.pnResult.Visible = false;
            
        }
    }
}

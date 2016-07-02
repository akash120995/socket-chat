using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace ServerClientApp
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader str;
        public StreamWriter stw;
        public string recieve;
        public String textToSend;
        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());      //Get any IP
            foreach (IPAddress address in localIP)
            {
                   if(address.AddressFamily == AddressFamily.InterNetwork)
                   {
                       textBox3.Text = address.ToString();
                   }
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)       //Start Server
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(textBox5.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            str = new StreamReader(client.GetStream());
            stw = new StreamWriter(client.GetStream());
            stw.AutoFlush = true;

            backgroundWorker1.RunWorkerAsync();                         //Start recieving data 
            backgroundWorker2.WorkerSupportsCancellation = true;        //Ability to cancel this thread     
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)     //Recieve data 
        {
            while(client.Connected)
            {
                try
                {
                    recieve = str.ReadLine();
                    this.textBox2.Invoke(new MethodInvoker(delegate() { textBox2.AppendText("You : " + recieve + "\n"); }));
                }
                catch(Exception x)
                {
                    MessageBox.Show(x.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)     //Send data
        {
            if(client.Connected)
            {
                stw.WriteLine(textToSend);
                this.textBox2.Invoke(new MethodInvoker(delegate() { textBox2.AppendText("Me :  " + textToSend + "\n"); }));
            }
            else
            {
                MessageBox.Show("Send Failed!");
            }
            backgroundWorker2.CancelAsync();
        }

        private void button3_Click(object sender, EventArgs e)          //connect to server
        {
            client = new TcpClient();
            IPEndPoint ip_End = new IPEndPoint(IPAddress.Parse(textBox6.Text), int.Parse(textBox4.Text));

            try
            {
                client.Connect(ip_End);
                if (client.Connected)
                {
                    textBox2.AppendText("Connected to server" + "\n");
                    stw = new StreamWriter(client.GetStream());
                    str = new StreamReader(client.GetStream());
                    stw.AutoFlush = true;

                    backgroundWorker1.RunWorkerAsync();                         //Start recieving data 
                    backgroundWorker2.WorkerSupportsCancellation = true;        //Ability to cancel this thread    
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)         //send button
        {
            if(textBox1.Text != "")
            {
                textToSend = textBox1.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            textBox1.Text = "";
        }
    }
}

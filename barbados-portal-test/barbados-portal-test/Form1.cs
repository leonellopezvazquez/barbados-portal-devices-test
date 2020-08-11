using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net.Sockets;

namespace barbados_portal_test
{
    public partial class Form1 : Form
    {
        FTPWatcher ftpwatcher;
        private bool EstadoSerial = false;
        TcpClient client;
        public static SerialPort port { get; set; }
        public Form1()
        {
            ftpwatcher = new FTPWatcher("C:/ftp/inAxi", "*.axi");
            InitializeComponent();
            port = new SerialPort();

            
        }

        private void btstart_Click(object sender, EventArgs e)
        {
            ftpwatcher.eventoFTPWatcher += evento;
            ftpwatcher.StartFTPWatcher();
            
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            string com = "COM4";
            if (com == null)
            {
                DisconnectAuto(com);              
                return;
            }
            EstadoSerial = ConnectAuto(com);
        }

        private void btstop_Click(object sender, EventArgs e)
        {
            ftpwatcher.StartFTPWatcher();
            ftpwatcher.eventoFTPWatcher -= evento;
            port.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
            string com = "COM4";

            EstadoSerial = DisconnectAuto(com);
        }

        public Bitmap ByteToImage(byte[] blob)
        {
            using (var mStream = new MemoryStream())
            {
                Bitmap bm;
                byte[] pData = blob;
                mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
                bm = new Bitmap(mStream, false);
                mStream.Dispose();
                return bm;
            }
        }

        public void evento(object sender,EventArgs e)
        {
            PIPSEvent pips = (PIPSEvent)sender;

            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                pbPatch.Image = ByteToImage(pips.Patch);
                pbIR.Image = ByteToImage(pips.IR);
                pbOverview.Image = ByteToImage(pips.Overview);
            }));
        }



        private bool ConnectAuto(string com)
        {
            try
            {
                ///connect
                port.ReadTimeout = 3000;
                port.PortName = com;
                port.BaudRate = 9600;
                port.Parity = Parity.None;
                port.DataBits = 8;
                port.StopBits = StopBits.One;
                port.Open();
                //eventLog1.WriteEntry("connected.", EventLogEntryType.Information, eventId++);

                return true;
            }
            catch (Exception ex)
            {


                if (ex.Message.Equals("'PortName' cannot be set while the port is open.") || ex.Message.Equals("'PortName' no se puede establecer mientras el puerto esté abierto."))
                {

                }
                else
                {
                    //Logs.Errores("Cannot connect to serial port " + com + "  " + ex.Message);
                    //port.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                    //port.Dispose();
                    port.Close();
                    //port = new SerialPort();
                    //port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                }

                return false;

            }

        }

        private bool DisconnectAuto(string com)
        {
            try
            {
                ///connect


                port.Close();
                port.Dispose();
                //eventLog1.WriteEntry("disconnected.");
                //Logs.WriteLog("disconnected from serial port " + com);
                return true;
            }
            catch (Exception ex)
            {

                return false;

            }
        }

        private String GetPortNames()
        {
            string[] strCom = System.IO.Ports.SerialPort.GetPortNames();

            if (strCom.Length == 0)
            {
                return null;
            }

            foreach (string s in strCom)
            {
                return s;
            }

            return null;
        }


        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string cmd = "";
                string data = "";

                SerialPort sp = (SerialPort)sender;
                string mensaje = sp.ReadLine();

                string[] valores;
                valores = mensaje.Split('|');
                //string msg = "Tension: = " + valores[0] + " mV" + "  corriente = " + valores[1];
                //eventLog1.WriteEntry(msg, EventLogEntryType.Information, eventId++);
                cmd = valores[0];
                data = valores[1];
                

                sp.DiscardInBuffer();

            }
            catch (Exception ex)
            {
                //Logs.Errores("Serial reception error " + ex.Message);

            }

        }

        void Connect(String server, String message)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.

                client = new TcpClient("127.0.0.1", 3820);
                client.Client.SendTimeout = 2000;
                client.Client.ReceiveTimeout = 2000;
                

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                //Int32 bytes = stream.Read(data, 0, data.Length);
                //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();
                client.Dispose();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        private void sendSerialTrig(string data)
        {
            try
            {
                port.Write(data);
            }
            catch(Exception ex)
            {

            }
        }

        private void bttrigtcp_Click(object sender, EventArgs e)
        {
            Connect("127.0.0.1","ya\n");
        }

        private void btstrigserial_Click(object sender, EventArgs e)
        {
            sendSerialTrig("test\n");
        }
    }
}

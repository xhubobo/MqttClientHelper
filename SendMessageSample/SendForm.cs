using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SendMessageSample
{
    public partial class SendForm : Form
    {
        //UI线程的同步上下文
        private readonly SynchronizationContext _syncContext;

        private readonly MqttClientHelper _mqttClientHelper;
        private readonly SendMessageHelper _sendMessageHelper;

        private string _ip = "127.0.0.1";
        private int _port = 61613;
        private string _userName = "admin";
        private string _password = "password";

        private bool _loop = false;
        private int _fps = 20;
        private int _start = 0;
        private int _stop = 50 * 60 * 10;

        public SendForm()
        {
            InitializeComponent();

            _syncContext = SynchronizationContext.Current;
            _mqttClientHelper = new MqttClientHelper();
            _sendMessageHelper = new SendMessageHelper();
            _mqttClientHelper.OnMqttConnect += OnMqttConnect;
            _sendMessageHelper.OnSendMessage += OnSendMessage;
        }

        private void SendForm_Load(object sender, EventArgs e)
        {
            textBoxIp.Text = _ip;
            textBoxPort.Text = _port.ToString();
            textBoxUserName.Text = _userName;
            textBoxPwd.Text = _password;
            labelMqttConnState.Text = "MQTT未连接";
            labelMqttConnState.ForeColor = Color.Black;

            checkBoxLoop.Checked = _loop;

            _mqttClientHelper.InitMqttParas();
        }

        #region MQTT

        private void OnSendMessage(string message)
        {
            _mqttClientHelper.SendMessage(message);
        }

        private void OnMqttConnect(bool ret)
        {
            _mqttClientHelper.MqttConnected = ret;
            _syncContext.Post(OnMqttConnectSafePost, null);
        }

        private void OnMqttConnectSafePost(object state)
        {
            if (_mqttClientHelper.MqttConnected)
            {
                labelMqttConnState.Text = "MQTT已连接";
                labelMqttConnState.ForeColor = Color.Green;
                buttonLogin.Text = "Logout";
            }
            else
            {
                labelMqttConnState.Text = "MQTT已断开";
                labelMqttConnState.ForeColor = Color.Red;
                buttonLogin.Text = "Login";
            }

            buttonLogin.Enabled = true;
        }

        #endregion

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (!CheckMqttParas())
            {
                return;
            }

            buttonLogin.Enabled = false;

            if (!_mqttClientHelper.MqttConnected)
            {
                _mqttClientHelper.StartMqtt(_ip, _port, _userName, _password);
            }
            else
            {
                _mqttClientHelper.StopMqtt();
            }
        }

        private bool CheckMqttParas()
        {
            _ip = textBoxIp.Text.Trim();
            if (string.IsNullOrEmpty(_ip))
            {
                MessageBox.Show("请输入IP");
                return false;
            }

            var port = textBoxPort.Text.Trim();
            if (!int.TryParse(port, out _port))
            {
                MessageBox.Show("端口格式不正确");
                return false;
            }

            _userName = textBoxUserName.Text.Trim();
            if (string.IsNullOrEmpty(_userName))
            {
                MessageBox.Show("请输入UserName");
                return false;
            }

            _password = textBoxPwd.Text.Trim();
            if (string.IsNullOrEmpty(_password))
            {
                MessageBox.Show("请输入Password");
                return false;
            }

            return true;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            //_sendMessageHelper.Start();
        }

        private bool CheckSendParas()
        {
            _ip = textBoxIp.Text.Trim();
            if (string.IsNullOrEmpty(_ip))
            {
                MessageBox.Show("请输入IP");
                return false;
            }

            var port = textBoxPort.Text.Trim();
            if (!int.TryParse(port, out _port))
            {
                MessageBox.Show("端口格式不正确");
                return false;
            }

            _userName = textBoxUserName.Text.Trim();
            if (string.IsNullOrEmpty(_userName))
            {
                MessageBox.Show("请输入UserName");
                return false;
            }

            _password = textBoxPwd.Text.Trim();
            if (string.IsNullOrEmpty(_password))
            {
                MessageBox.Show("请输入Password");
                return false;
            }

            return true;
        }
    }
}
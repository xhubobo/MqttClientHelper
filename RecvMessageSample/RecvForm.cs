using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SendMessageSample;

namespace RecvMessageSample
{
    public partial class RecvForm : Form
    {
        //UI线程的同步上下文
        private readonly SynchronizationContext _syncContext;

        private readonly MqttClientHelper _mqttClientHelper;
        private readonly SendMessageHelper _sendMessageHelper;
        private readonly RecvMessageHelper _recvMessageHelper;

        private string _ip = "127.0.0.1";
        private int _port = 61613;
        private string _userName = "admin";
        private string _password = "password";

//         public event Action<string> OnErrorMsg = (msg) => { };
//         public event Action<string> OnLogMsg = (msg) => { };
//         public event Action<string> OnPublishMsg = (msg) => { };
//         public event Action<int> OnRecvValueMsg = (msg) => { };

        public RecvForm()
        {
            InitializeComponent();

            _syncContext = SynchronizationContext.Current;

            _mqttClientHelper = new MqttClientHelper();
            _sendMessageHelper = new SendMessageHelper();
            _recvMessageHelper = new RecvMessageHelper();

            _mqttClientHelper.OnMqttConnect += OnMqttConnect;
            _sendMessageHelper.OnSendMessage += OnSendMessage;
        }

        private void RecvForm_Load(object sender, EventArgs e)
        {
            textBoxIp.Text = _ip;
            textBoxPort.Text = _port.ToString();
            textBoxUserName.Text = _userName;
            textBoxPwd.Text = _password;
            labelMqttConnState.Text = "MQTT未连接";
            labelMqttConnState.ForeColor = Color.Black;
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

        #region RecvMessage

        private void OnErrorMsg(string msg)
        {
            _syncContext.Post(AddLog, $"[ERR] {msg}");
        }

        private void OnLogMsg(string msg)
        {
            _syncContext.Post(AddLog, $"[LOG] {msg}");
        }

        private void OnPublishMsg(string msg)
        {
            _mqttClientHelper.SendMessage(msg);
        }

        private void OnRecvValueMsg(string msg)
        {
            _syncContext.Post(OnRecvValueMsgSafePost, msg);
        }

        private void AddLog(object state)
        {
            var msg = state?.ToString() ?? string.Empty;
            var time = DateTime.Now.ToString("HH:mm:ss");
            listBoxLog.Items.Insert(0, $"{time} {msg}");
        }

        private void OnRecvValueMsgSafePost(object state)
        {
            var msg = state?.ToString() ?? string.Empty;
            int value;
            if (!int.TryParse(msg, out value))
            {
                AddLog($"接收值有误：{msg}.");
                return;
            }

            labelDisplay.Text = value.ToString();
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
    }
}

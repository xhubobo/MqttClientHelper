using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MqttClientModules;
using MqttClientUtil;

namespace RecvMessageSample
{
    public partial class RecvForm : Form
    {
        //UI线程的同步上下文
        private readonly SynchronizationContext _syncContext;

        private readonly MqttClientUtil.MqttClientHelper _mqttClientHelper;
        private readonly MessageHelper _recvMessageHelper;
        private readonly RecvMqttMsgHandler _mqttMsgHandler;

        private string _ip = "127.0.0.1";
        private int _port = 61613;
        private string _userName = "admin";
        private string _password = "password";

        private DrawForm _drawForm;
        private DateTime _beginRecvTime;

        public RecvForm()
        {
            InitializeComponent();

            _syncContext = SynchronizationContext.Current;

            _mqttClientHelper = new MqttClientUtil.MqttClientHelper();
            _recvMessageHelper = new MessageHelper();
            _mqttMsgHandler = new RecvMqttMsgHandler();

            _mqttClientHelper.OnMqttConnect += OnMqttConnect; //MQTT连接
            _mqttClientHelper.OnMqttMessage += OnMqttMessage; //MQTT接收消息
            _recvMessageHelper.OnMessage += OnRecvMessage; //接收队列消息
            _mqttMsgHandler.OnLogMsg += OnLogMsg;
            _mqttMsgHandler.OnErrorMsg += OnErrorMsg;
            _mqttMsgHandler.OnPublishMsg += OnPublishMsg;
            _mqttMsgHandler.OnRecvValueMsg += OnRecvValueMsg;
            _mqttMsgHandler.OnRecvValueBeginMsg += OnRecvValueBeginMsg;
            _mqttMsgHandler.OnRecvValueEndMsg += OnRecvValueEndMsg;

            _mqttClientHelper.InitMqttParas(
                MqttClientConstants.MqttClientRecvTopic,
                MqttClientConstants.MqttClientSendTopic,
                MqttClientConstants.MqttClientHeartbeatTopic,
                MqttMessageHandler.GetWillMessage());
            _recvMessageHelper.Start();
            _mqttMsgHandler.Init();
        }

        private void RecvForm_Load(object sender, EventArgs e)
        {
            LogHelper.InitLogPath();
            LogHelper.AddLog("Start");

            textBoxIp.Text = _ip;
            textBoxPort.Text = _port.ToString();
            textBoxUserName.Text = _userName;
            textBoxPwd.Text = _password;
            labelMqttConnState.Text = "MQTT未连接";
            labelMqttConnState.ForeColor = Color.Black;
            labelRecvTip.Text = string.Empty;

            _drawForm = new DrawForm
            {
                StartPosition = FormStartPosition.Manual
            };
            _drawForm.Show();
            AsyncDrawForm();
        }

        private void RecvForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _recvMessageHelper.Stop();
            LogHelper.Stop();
        }

        private void RecvForm_SizeChanged(object sender, EventArgs e)
        {
            var width = ClientSize.Width - 20;
            var height = ClientSize.Height - listBoxLog.Top - 10;

            listBoxLog.Width = width > 0 ? width : 0;
            listBoxLog.Height = height > 0 ? height : 0;
            listBoxLog.Left = 10;

            listBoxLog.Visible = listBoxLog.Height >= 20;

            AsyncDrawForm();
        }

        private void RecvForm_LocationChanged(object sender, EventArgs e)
        {
            AsyncDrawForm();
        }

        private void AsyncDrawForm()
        {
            if (_drawForm == null)
            {
                return;
            }

            //_drawForm.Width = Width;
            //_drawForm.Height = Height;
            //_drawForm.Left = Right;
            //_drawForm.Top = Top;
        }

        #region MQTT

        public void SendMessage(string msg)
        {
            _mqttClientHelper.SendMessage(msg);
        }

        private void OnRecvMessage(string msg)
        {
            _mqttMsgHandler.HandleMessage(msg);
        }

        private void OnMqttConnect(bool ret)
        {
            _mqttClientHelper.MqttConnected = ret;
            _syncContext.Post(OnMqttConnectSafePost, null);
        }

        private void OnMqttMessage(string msg)
        {
            _recvMessageHelper.AddMessage(msg);
        }

        private void OnMqttConnectSafePost(object state)
        {
            if (_mqttClientHelper.MqttConnected)
            {
                _mqttMsgHandler.PublishOnLine();
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

        private void OnRecvValueMsg(int value)
        {
            _drawForm?.SetValue(value);
            _syncContext.Post(OnRecvValueMsgSafePost, value);
        }

        private void OnRecvValueBeginMsg()
        {
            LogHelper.AddLog($"RecvValueBegin:\t{DateTime.Now:HH:mm:ss fff}");

            _drawForm?.BeginSetValue();
            _syncContext.Post(BeginSetValueSafePost, null);
        }

        private void OnRecvValueEndMsg()
        {
            LogHelper.AddLog($"RecvValueEnd:\t{DateTime.Now:HH:mm:ss fff}");

            _drawForm?.EndSetValue();
            _syncContext.Post(EndSetValueSafePost, null);
        }

        private void AddLog(object state)
        {
            var msg = state?.ToString() ?? string.Empty;
            var time = DateTime.Now.ToString("HH:mm:ss fff");
            listBoxLog.Items.Insert(0, $"{time} {msg}");
        }

        private void OnRecvValueMsgSafePost(object state)
        {
            var value = (int) state;
            labelDisplay.Text = value.ToString();
            AddLog($"RecvValue: {value}.");
        }

        private void BeginSetValueSafePost(object state)
        {
            _beginRecvTime = DateTime.Now;
            var time = _beginRecvTime.ToString("HH:mm:ss fff");
            labelRecvTip.Text = $"Recv value begin: {time}";
        }

        private void EndSetValueSafePost(object state)
        {
            var endRecvTime = DateTime.Now;
            var beginTime = _beginRecvTime.ToString("HH:mm:ss fff");
            var endTime = endRecvTime.ToString("HH:mm:ss fff");
            var span = (endRecvTime - _beginRecvTime).TotalMilliseconds;
            labelRecvTip.Text = $"Recv value begin: {beginTime}" +
                                Environment.NewLine +
                                $"Recv value end:   {endTime}" +
                                Environment.NewLine +
                                $"Totally costs:    {span}ms";
        }

        #endregion

        #region 按钮事件

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
                _mqttMsgHandler.PublishOffLine();
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

        #endregion

        #region 日志事件

        private void listBoxLog_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && listBoxLog.Items.Count > 0)
            {
                contextMenuStripLog.Show(listBoxLog, e.Location);
            }
        }

        private void toolStripMenuItemClear_Click(object sender, EventArgs e)
        {
            listBoxLog.Items.Clear();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MqttClientModules;
using MqttClientUtil;

namespace SendMessageSample
{
    public partial class SendForm : Form
    {
        //UI线程的同步上下文
        private readonly SynchronizationContext _syncContext;

        private readonly MqttClientUtil.MqttClientHelper _mqttClientHelper;
        private readonly SendMessageHelper _sendMessageHelper;
        private readonly MessageHelper _recvMessageHelper;
        private readonly SendMqttMsgHandler _mqttMsgHandler;

        private string _ip = "127.0.0.1";
        private int _port = 61613;
        private string _userName = "admin";
        private string _password = "password";

        private bool _loop = false;
        private int _fps = 20;
        private int _start = 0;
        private int _stop = 50 * 60 * 10;

        private readonly Dictionary<string, PingInfo> _pingDic;

        public SendForm()
        {
            InitializeComponent();

            _pingDic = new Dictionary<string, PingInfo>();
            _syncContext = SynchronizationContext.Current;

            _mqttClientHelper = new MqttClientUtil.MqttClientHelper();
            _sendMessageHelper = new SendMessageHelper();
            _recvMessageHelper = new MessageHelper();
            _mqttMsgHandler = new SendMqttMsgHandler();

            _mqttClientHelper.OnMqttConnect += OnMqttConnect;
            _mqttClientHelper.OnMqttMessage += OnMqttRecvMessage;
            _sendMessageHelper.OnSendMessage += OnMqttSendMessage;
            _recvMessageHelper.OnMessage += OnRecvMessage;
            _mqttMsgHandler.OnLogMsg += OnLogMsg;
            _mqttMsgHandler.OnErrorMsg += OnErrorMsg;
            _mqttMsgHandler.OnPublishMsg += OnPublishMsg;
            _mqttMsgHandler.OnPangMsg += OnPangMsg;

            _mqttClientHelper.InitMqttParas(
                MqttClientConstants.MqttClientSendTopic,
                MqttClientConstants.MqttClientRecvTopic,
                MqttMessageHandler.GetWillMessage());
            _recvMessageHelper.Start();
            _mqttMsgHandler.Init();
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
        }

        private void SendForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _recvMessageHelper.Stop();
        }

        private void SendForm_SizeChanged(object sender, EventArgs e)
        {
            var width = ClientSize.Width - 20;
            var height = ClientSize.Height - listBoxLog.Top - 10;

            listBoxLog.Width = width > 0 ? width : 0;
            listBoxLog.Height = height > 0 ? height : 0;
            listBoxLog.Left = 10;

            listBoxLog.Visible = listBoxLog.Height >= 20;
        }

        #region MQTT

        //MQTT连接结果
        private void OnMqttConnect(bool ret)
        {
            _mqttClientHelper.MqttConnected = ret;
            _syncContext.Post(OnMqttConnectSafePost, null);
        }

        //MQTT接收消息
        private void OnMqttRecvMessage(string msg)
        {
            _recvMessageHelper.AddMessage(msg);
        }

        //MQTT发送消息
        private void OnMqttSendMessage(string message)
        {
            _mqttClientHelper.SendMessage(message);
        }

        //MQTT消息队列再分发
        private void OnRecvMessage(string msg)
        {
            _mqttMsgHandler.HandleMessage(msg);
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

        private void OnPangMsg(string msg)
        {
            _syncContext.Post(OnPangMsgSafePost, msg);
        }

        private void AddLog(object state)
        {
            var msg = state?.ToString() ?? string.Empty;
            var time = DateTime.Now.ToString("HH:mm:ss");
            listBoxLog.Items.Insert(0, $"{time} {msg}");
        }

        private void OnPangMsgSafePost(object state)
        {
            var key = state?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(key))
            {
                AddLog("OnPangMsgSafePost para is null or empty.");
                return;
            }

            if (!_pingDic.ContainsKey(key))
            {
                AddLog($"OnPangMsgSafePost undefined key {key}.");
                return;
            }

            _pingDic[key].RecvTime = DateTime.Now;
            _pingDic[key].Received = true;

            var count = _pingDic.Where(t => t.Value.Received).ToList().Count;
            if (count == _pingDic.Count)
            {
                DisplayPingInfo();
            }
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

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (!CheckSendParas())
            {
                return;
            }

            _sendMessageHelper.Stop();
            _sendMessageHelper.Start(1000 / _fps, _start, _stop);
        }

        private bool CheckSendParas()
        {
            _loop = checkBoxLoop.Checked;

            var fps = textBoxFps.Text.Trim();
            if (!int.TryParse(fps, out _fps))
            {
                MessageBox.Show("帧率格式不正确");
                return false;
            }

            if (_fps <= 0)
            {
                MessageBox.Show("帧率必须大于0");
                return false;
            }

            var start = textBoxStartValue.Text.Trim();
            if (!int.TryParse(start, out _start))
            {
                MessageBox.Show("开始值不正确");
                return false;
            }

            if (_start < 0)
            {
                MessageBox.Show("开始值必须大于等于0");
                return false;
            }

            var stop = textBoxStopValue.Text.Trim();
            if (!int.TryParse(stop, out _stop))
            {
                MessageBox.Show("结束值不正确");
                return false;
            }

            if (_stop < 0)
            {
                MessageBox.Show("结束值必须大于等于0");
                return false;
            }

            return true;
        }

        private void buttonPing_Click(object sender, EventArgs e)
        {
            _pingDic.Clear();
            labelPingResult.Text = "Ping...";

            for (var i = 0; i < 10; i++)
            {
                var key = Guid.NewGuid().ToString();
                _pingDic.Add(key, new PingInfo()
                {
                    Key = key,
                    SendTime = DateTime.Now
                });

                _mqttMsgHandler.PublishPing(key);
            }
        }

        #endregion

        private void DisplayPingInfo()
        {
            var resultList = _pingDic.Values.ToList();
            resultList.Sort((x, y) => x.Span.CompareTo(y.Span));

            var min = resultList[0].Span.Milliseconds;
            var max = resultList[resultList.Count - 1].Span.Milliseconds;
            var avg = resultList.Sum(t => t.Span.Milliseconds) / resultList.Count;
            labelPingResult.Text = $"min: {min}ms, max: {max}ms, avg: {avg}ms.";

            foreach (var result in resultList)
            {
                Console.WriteLine($"span: {result.Span.Milliseconds}.");
            }
        }

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
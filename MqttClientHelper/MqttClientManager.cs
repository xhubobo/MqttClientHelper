using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using Newtonsoft.Json.Linq;

namespace MqttClientHelper
{
    public sealed class MqttClientManager : IDisposable
    {
        //回调方法
        private Action<string> _msgAction;
        private Action<bool> _connAction;

        private MqttClient _mqttClient;
        private readonly string _clientId = Guid.NewGuid().ToString();
        private bool _mqttConnected;
        private readonly object _mqttConnectedLockHelper = new object();

        //守护定时器
        private Timer _daemonTimer;

        private bool MqttConnected
        {
            get
            {
                var connected = _mqttClient != null && _mqttClient.IsConnected;
                lock (_mqttConnectedLockHelper)
                {
                    connected = _mqttConnected && connected;
                }

                return connected;
            }
            set
            {
                lock (_mqttConnectedLockHelper)
                {
                    _mqttConnected = value;
                }
            }
        }

        #region 参数设置
        public bool EnableHeartBeat { get; set; } = true;

        public string PublishTopic { get; set; } = string.Empty;
        public string SubscribeTopic { get; set; } = string.Empty;
        public string HeartbeatTopic { get; set; } = string.Empty;
        public string LostPayLoadCmd { get; set; } = string.Empty;
        public string LostPayLoadTopic { get; set; } = string.Empty;

        //守护线程时间间隔默认5s
        public int DaemonInterval { get; set; } = 5;

        public string BrokerAddress { get; set; }
        public int BrokerPort { get; set; }
        public string BrokerUserName { get; set; }
        public string BrokerPassword { get; set; }
        #endregion

        /// <summary>
        /// 添加消息回调方法
        /// </summary>
        /// <param name="callback">消息回调</param>
        public void SetMessageAction(Action<string> callback)
        {
            _msgAction = callback;
        }

        /// <summary>
        /// 设置MQTT连接回调方法
        /// </summary>
        /// <param name="callback">连接回调</param>
        public void SetConnectAction(Action<bool> callback)
        {
            _connAction = callback;
        }

        public void Start()
        {
            //异步连接
            Task.Run(async () => { await ConnectAsync(); });
        }

        public void Stop()
        {
            //移除守护定时器
            _daemonTimer?.Dispose();
            _daemonTimer = null;

            try
            {
                //退出时，断开连接
                _mqttClient?.DisconnectAsync();
                _mqttClient = null;
            }
            catch (Exception e)
            {
                LogHelper.AddLog(e);
            }
        }

        private async Task ConnectAsync()
        {
            // create client instance
            _mqttClient = new MqttClientFactory().CreateMqttClient() as MqttClient;
            if (_mqttClient == null)
            {
                return;
            }

            _mqttClient.ApplicationMessageReceived += OnMessageReceived;
            _mqttClient.Connected += OnMqttConnected;
            _mqttClient.Disconnected += OnMqttDisconnected;

            try
            {
                var jObj = new JObject()
                {
                    [LostPayLoadCmd] = LostPayLoadTopic
                };
                var willMsg = new MqttApplicationMessage(PublishTopic,
                    Encoding.UTF8.GetBytes(jObj.ToString()), MqttQualityOfServiceLevel.ExactlyOnce,
                    false);

                var options = new MqttClientTcpOptions()
                {
                    Server = BrokerAddress,
                    Port = BrokerPort,
                    ClientId = _clientId,
                    UserName = BrokerUserName,
                    Password = BrokerPassword,
                    WillMessage = willMsg,
                    CleanSession = true
                };

                await _mqttClient.ConnectAsync(options);
            }
            catch (Exception e)
            {
                MqttConnected = false;
                LogHelper.AddLog(e);
            }
        }

        public void Publish(string message)
        {
            try
            {
                var msg = new MqttApplicationMessage(PublishTopic,
                    Encoding.UTF8.GetBytes(message), MqttQualityOfServiceLevel.ExactlyOnce, false);
                _mqttClient?.PublishAsync(msg);
            }
            catch (Exception e)
            {
                LogHelper.AddLog(e);
            }
        }

        #region MQTT回调

        //MQTT接收信息
        private void OnMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            if (!SubscribeTopic.Equals(e.ApplicationMessage.Topic))
            {
                return;
            }

            //支持中文
            var msg = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            _msgAction?.Invoke(msg);
        }

        //MQTT连接成功
        private void OnMqttConnected(object sender, EventArgs e)
        {
            MqttConnected = true;

            try
            {
                //订阅
                _mqttClient?.SubscribeAsync(new List<TopicFilter>
                {
                    new TopicFilter(SubscribeTopic,
                        MqttQualityOfServiceLevel.ExactlyOnce)
                });
            }
            catch (Exception ex)
            {
                LogHelper.AddLog(ex);
            }

            //发送心跳数据
            PublishHeartbeat();

            //创建守护定时器，DaemonInterval秒后执行
            _daemonTimer = new Timer(DaemonFunc, null,
                TimeSpan.FromSeconds(DaemonInterval), Timeout.InfiniteTimeSpan);

            _connAction?.Invoke(true);
        }

        //MQTT断开
        private void OnMqttDisconnected(object sender, EventArgs e)
        {
            MqttConnected = false;
            _connAction?.Invoke(false);
        }

        #endregion

        #region 守护及心跳

        private void DaemonFunc(object state)
        {
            if (!MqttConnected)
            {
                //移除守护定时器
                _daemonTimer?.Dispose();
                _daemonTimer = null;

                //断线重连
                Task.Run(async () => { await ConnectAsync(); });

                return;
            }

            //发送心跳数据
            PublishHeartbeat();

            //执行下次定时器
            _daemonTimer?.Change(
                TimeSpan.FromSeconds(DaemonInterval),
                Timeout.InfiniteTimeSpan);
        }

        private void PublishHeartbeat()
        {
            if (!EnableHeartBeat)
            {
                return;
            }

            try
            {
                var msg = new MqttApplicationMessage(HeartbeatTopic,
                    new byte[] { 0 }, MqttQualityOfServiceLevel.AtMostOnce, false);
                _mqttClient?.PublishAsync(msg);
            }
            catch (Exception e)
            {
                LogHelper.AddLog(e);
            }
        }

        #endregion

        #region IDisposable Support

        //检测冗余调用
        private bool _disposedValue;

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    //释放托管状态(托管对象)。
                    _msgAction = null;
                    _connAction = null;
                }

                //释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                //将大型字段设置为 null。
                Stop();

                _disposedValue = true;
            }
        }

        ~MqttClientManager()
        {
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region 单例模式

        private static MqttClientManager _instance;

        private static readonly object LockHelper = new object();

        private MqttClientManager()
        {
        }

        public static MqttClientManager Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (LockHelper)
                {
                    _instance = _instance ?? new MqttClientManager();
                }

                return _instance;
            }
        }

        #endregion
    }
}

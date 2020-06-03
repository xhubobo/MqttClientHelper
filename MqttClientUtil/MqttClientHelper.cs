using System;
using MqttClientHelper;

namespace MqttClientUtil
{
    public sealed class MqttClientHelper
    {
        public event Action<bool> OnMqttConnect = (ret) => { };
        public event Action<string> OnMqttMessage = (msg) => { };

        #region MqttConnected

        private bool _mqttConnected;
        private static readonly object MqttConnectedLockHelper = new object();

        public bool MqttConnected
        {
            get
            {
                lock (MqttConnectedLockHelper)
                {
                    return _mqttConnected;
                }
            }
            set
            {
                lock (MqttConnectedLockHelper)
                {
                    _mqttConnected = value;
                }
            }
        }

        #endregion

        public void InitMqttParas(string publishTopic, string subscribeTopic,
            string heartbeatTopic, string willMessage)
        {
            MqttClientManager.Instance.PublishTopic = publishTopic;
            MqttClientManager.Instance.SubscribeTopic = subscribeTopic;
            MqttClientManager.Instance.HeartbeatTopic = heartbeatTopic;

            MqttClientManager.Instance.WillMessage = willMessage;
            MqttClientManager.Instance.EnableHeartBeat = false;
            MqttClientManager.Instance.DaemonInterval = 5;

            MqttClientManager.Instance.SetConnectAction(ret => { OnMqttConnect?.Invoke(ret); });
            MqttClientManager.Instance.SetMessageAction(msg => { OnMqttMessage?.Invoke(msg); });
        }

        public void StartMqtt(string ip, int port, string userName, string password)
        {
            MqttClientManager.Instance.BrokerAddress = ip;
            MqttClientManager.Instance.BrokerPort = port;
            MqttClientManager.Instance.BrokerUserName = userName;
            MqttClientManager.Instance.BrokerPassword = password;
            MqttClientManager.Instance.Start();
        }

        public void StopMqtt()
        {
            MqttClientManager.Instance.Stop();
        }

        public void SendMessage(string message)
        {
            MqttClientManager.Instance.Publish(message);
        }
    }
}

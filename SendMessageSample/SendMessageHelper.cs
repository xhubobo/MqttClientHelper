using System;
using System.Threading;
using MqttClientModules;
using Newtonsoft.Json.Linq;

namespace SendMessageSample
{
    internal sealed class SendMessageHelper
    {
        public event Action<string> OnSendMessage = (msg) => { };

        //守护定时器
        private Timer _sendTimer;

        private int _interval;
        private int _stop;
        private int _current;

        public void Start(int interval, int sendValue)
        {
            if (sendValue < 0)
            {
                return;
            }

            _interval = interval;
            _current = 0;
            _stop = sendValue;

            //创建定时器，interval毫秒后执行
            _sendTimer = new Timer(SendFunc, null,
                TimeSpan.FromMilliseconds(interval), Timeout.InfiniteTimeSpan);
        }

        public void Stop()
        {
            //移除定时器
            _sendTimer?.Dispose();
            _sendTimer = null;
        }

        private void SendFunc(object state)
        {
            //执行下次定时器
            _sendTimer?.Change(
                Timeout.InfiniteTimeSpan,
                Timeout.InfiniteTimeSpan);

            //SendMessage
            OnSendMessage?.Invoke(GetSendMessage(_current));

            //校验是否发送完毕
            if (++_current >= _stop)
            {
                Stop();
                return;
            }

            //执行下次定时器
            _sendTimer?.Change(
                TimeSpan.FromMilliseconds(_interval),
                Timeout.InfiniteTimeSpan);
        }

        public static string GetSendMessage(int value)
        {
            var parasObj = new JObject()
            {
                [MqttClientConstants.Para.Value] = value
            };
            var cmdObj = new JObject()
            {
                [MqttClientConstants.Command] = MqttClientConstants.Cmd.SendValue,
                [MqttClientConstants.Paras] = parasObj.ToString()
            };
            var jObj = new JObject()
            {
                [MqttClientConstants.CmdType] = MqttClientConstants.Topic.Operation,
                [MqttClientConstants.Paras] = cmdObj.ToString()
            };
            return jObj.ToString();
        }
    }
}
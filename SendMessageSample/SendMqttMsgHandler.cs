using System;
using System.Collections.Generic;
using MqttClientModules;
using Newtonsoft.Json.Linq;

namespace SendMessageSample
{
    internal sealed class SendMqttMsgHandler
    {
        public event Action<string> OnErrorMsg = (msg) => { };
        public event Action<string> OnLogMsg = (msg) => { };
        public event Action<string> OnPublishMsg = (msg) => { };
        public event Action<string> OnPangMsg = (msg) => { };

        private readonly Dictionary<string, Action<string>> _topicActionDic;
        private readonly Dictionary<string, Action<string>> _cmdActionDic;

        public SendMqttMsgHandler()
        {
            _topicActionDic = new Dictionary<string, Action<string>>();
            _cmdActionDic = new Dictionary<string, Action<string>>();

            InitActionDic();
        }

        public void HandleMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                AddErrorMsg("Receive empty msg");
                return;
            }

            JObject msgObjects;
            try
            {
                msgObjects = JObject.Parse(msg);
            }
            catch (Exception)
            {
                AddErrorMsg($"Json解析失败: {msg}");
                return;
            }

            //获取Topic
            JToken topicToken;
            msgObjects.TryGetValue(MqttClientConstants.CmdType, out topicToken);
            var topic = topicToken?.ToObject<string>();

            if (!string.IsNullOrEmpty(topic) && _topicActionDic.ContainsKey(topic))
            {
                JToken parasToken;
                msgObjects.TryGetValue(MqttClientConstants.Paras, out parasToken);
                var paras = parasToken?.ToObject<string>();

                _topicActionDic[topic]?.Invoke(paras);
            }
            else
            {
                AddErrorMsg($"未识别的主题：{topic}");
            }
        }

        #region InternalMethods

        private void InitActionDic()
        {
            _topicActionDic.Clear();
            _cmdActionDic.Clear();

            //Topic
            _topicActionDic.Add(MqttClientConstants.Topic.OnLine, HandleTopicOnLine);
            _topicActionDic.Add(MqttClientConstants.Topic.OffLine, HandleTopicOffLine);
            _topicActionDic.Add(MqttClientConstants.Topic.Pang, HandleTopicPang);
            _topicActionDic.Add(MqttClientConstants.Topic.LostPayLoad, HandleTopicLostPayLoad);
            _topicActionDic.Add(MqttClientConstants.Topic.Operation, HandleTopicOperation);

            //Command
        }

        private void AddErrorMsg(string msg)
        {
            OnErrorMsg?.Invoke(msg);
        }

        private void AddLogMsg(string msg)
        {
            OnLogMsg?.Invoke(msg);
        }

        #endregion

        #region Handle Topic

        private void HandleTopicOnLine(string paras)
        {
            try
            {
                var jObject = JObject.Parse(paras);

                JToken remoteServerNameToken;
                jObject.TryGetValue(MqttClientConstants.Para.RemoteServerName, out remoteServerNameToken);
                var remoteServerName = remoteServerNameToken?.ToObject<string>();
                AddLogMsg($"{remoteServerName}上线");
            }
            catch (Exception)
            {
                AddErrorMsg($"HandleTopicOnLine - JSON解析失败: {paras}");
            }
        }

        private void HandleTopicOffLine(string paras)
        {
            try
            {
                var jObject = JObject.Parse(paras);

                JToken remoteServerNameToken;
                jObject.TryGetValue(MqttClientConstants.Para.RemoteServerName, out remoteServerNameToken);
                var remoteServerName = remoteServerNameToken?.ToObject<string>();
                AddLogMsg($"{remoteServerName}下线");
            }
            catch (Exception)
            {
                AddErrorMsg($"HandleTopicOffLine - JSON解析失败: {paras}");
            }
        }

        private void HandleTopicLostPayLoad(string paras)
        {
            try
            {
                var jObject = JObject.Parse(paras);

                JToken remoteServerNameToken;
                jObject.TryGetValue(MqttClientConstants.Para.RemoteServerName, out remoteServerNameToken);
                var remoteServerName = remoteServerNameToken?.ToObject<string>();
                AddLogMsg($"{remoteServerName}断线");
            }
            catch (Exception)
            {
                AddErrorMsg($"HandleTopicOffLine - JSON解析失败: {paras}");
            }
        }

        private void HandleTopicPang(string paras)
        {
            try
            {
                var jObject = JObject.Parse(paras);

                JToken keyToken;
                jObject.TryGetValue(MqttClientConstants.Para.Key, out keyToken);
                var key = keyToken?.ToObject<string>();

                //回应
                OnPangMsg?.Invoke(key);
            }
            catch (Exception)
            {
                AddErrorMsg($"HandleTopicPang - JSON解析失败: {paras}");
            }
        }

        private void HandleTopicOperation(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                AddErrorMsg("HandleTopicOperation empty msg");
                return;
            }

            JObject msgObjects;
            try
            {
                msgObjects = JObject.Parse(msg);
            }
            catch (Exception)
            {
                AddErrorMsg($"Json解析失败: {msg}");
                return;
            }

            //获取Command
            JToken cmdToken;
            msgObjects.TryGetValue(MqttClientConstants.Command, out cmdToken);
            var cmd = cmdToken?.ToObject<string>();

            if (!string.IsNullOrEmpty(cmd) && _cmdActionDic.ContainsKey(cmd))
            {
                JToken parasToken;
                msgObjects.TryGetValue(MqttClientConstants.Paras, out parasToken);
                var paras = parasToken?.ToObject<string>();

                _cmdActionDic[cmd]?.Invoke(paras);
            }
            else
            {
                AddErrorMsg($"未识别的Command：{cmd}");
            }
        }

        #endregion

        #region Publish

        public void PublishPing(string key)
        {
            var parasObj = new JObject()
            {
                [MqttClientConstants.Para.Key] = key
            };
            var jsonObj = new JObject()
            {
                [MqttClientConstants.CmdType] = MqttClientConstants.Topic.Ping,
                [MqttClientConstants.Paras] = parasObj.ToString()
            };
            OnPublishMsg?.Invoke(jsonObj.ToString());
        }

        public void PublishPang(string key)
        {
            var parasObj = new JObject()
            {
                [MqttClientConstants.Para.Key] = key
            };
            var jsonObj = new JObject()
            {
                [MqttClientConstants.CmdType] = MqttClientConstants.Topic.Pang,
                [MqttClientConstants.Paras] = parasObj.ToString()
            };
            OnPublishMsg?.Invoke(jsonObj.ToString());
        }

        public void PublishTopic(string topic)
        {
            var jsonObj = new JObject()
            {
                [MqttClientConstants.CmdType] = topic
            };
            OnPublishMsg?.Invoke(jsonObj.ToString());
        }

        #endregion
    }
}

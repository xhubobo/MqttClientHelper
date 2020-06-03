using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using MqttClientModules;

namespace MqttClientUtil
{
    public abstract class MqttMessageHandler
    {
        public event Action<string> OnErrorMsg = (msg) => { };
        public event Action<string> OnLogMsg = (msg) => { };
        public event Action<string> OnPublishMsg = (msg) => { };
        public event Action<string> OnPangMsg = (msg) => { };

        protected readonly Dictionary<string, Action<string>> TopicActionDic;
        protected readonly Dictionary<string, Action<string>> CmdActionDic;

        protected MqttMessageHandler()
        {
            TopicActionDic = new Dictionary<string, Action<string>>();
            CmdActionDic = new Dictionary<string, Action<string>>();
        }

        public void Init()
        {
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

            if (!string.IsNullOrEmpty(topic) && TopicActionDic.ContainsKey(topic))
            {
                JToken parasToken;
                msgObjects.TryGetValue(MqttClientConstants.Paras, out parasToken);
                var paras = parasToken?.ToObject<string>();

                TopicActionDic[topic]?.Invoke(paras);
            }
            else
            {
                AddErrorMsg($"未识别的主题：{topic}");
            }
        }

        #region InternalMethods

        protected virtual void InitActionDic()
        {
            TopicActionDic.Clear();
            CmdActionDic.Clear();

            //Topic
            TopicActionDic.Add(MqttClientConstants.Topic.OnLine, HandleTopicOnLine);
            TopicActionDic.Add(MqttClientConstants.Topic.OffLine, HandleTopicOffLine);
            TopicActionDic.Add(MqttClientConstants.Topic.Ping, HandleTopicPing);
            TopicActionDic.Add(MqttClientConstants.Topic.Pang, HandleTopicPang);
            TopicActionDic.Add(MqttClientConstants.Topic.LostPayLoad, HandleTopicLostPayLoad);
            TopicActionDic.Add(MqttClientConstants.Topic.Operation, HandleTopicOperation);
        }

        protected void AddErrorMsg(string msg)
        {
            OnErrorMsg?.Invoke(msg);
        }

        protected void AddLogMsg(string msg)
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

                JToken token;
                jObject.TryGetValue(MqttClientConstants.Para.MachineName, out token);
                var machineName = token?.ToObject<string>();
                AddLogMsg($"{machineName}上线");
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

                JToken token;
                jObject.TryGetValue(MqttClientConstants.Para.MachineName, out token);
                var machineName = token?.ToObject<string>();
                AddLogMsg($"{machineName}下线");
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

                JToken token;
                jObject.TryGetValue(MqttClientConstants.Para.MachineName, out token);
                var machineName = token?.ToObject<string>();
                AddLogMsg($"{machineName}断线");
            }
            catch (Exception)
            {
                AddErrorMsg($"HandleTopicOffLine - JSON解析失败: {paras}");
            }
        }

        private void HandleTopicPing(string paras)
        {
            try
            {
                var jObject = JObject.Parse(paras);

                JToken keyToken;
                jObject.TryGetValue(MqttClientConstants.Para.Key, out keyToken);
                var key = keyToken?.ToObject<string>();
                AddLogMsg($"Received ping cmd: {key}.");

                //回应
                PublishPang(key);
            }
            catch (Exception)
            {
                AddErrorMsg($"HandleTopicPang - JSON解析失败: {paras}");
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

            if (!string.IsNullOrEmpty(cmd) && CmdActionDic.ContainsKey(cmd))
            {
                JToken parasToken;
                msgObjects.TryGetValue(MqttClientConstants.Paras, out parasToken);
                var paras = parasToken?.ToObject<string>();

                CmdActionDic[cmd]?.Invoke(paras);
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

        public void PublishOnLine()
        {
            var parasObj = new JObject()
            {
                [MqttClientConstants.Para.MachineName] = Environment.MachineName
            };
            var jsonObj = new JObject()
            {
                [MqttClientConstants.CmdType] = MqttClientConstants.Topic.OnLine,
                [MqttClientConstants.Paras] = parasObj.ToString()
            };
            OnPublishMsg?.Invoke(jsonObj.ToString());
        }

        public void PublishOffLine()
        {
            var parasObj = new JObject()
            {
                [MqttClientConstants.Para.MachineName] = Environment.MachineName
            };
            var jsonObj = new JObject()
            {
                [MqttClientConstants.CmdType] = MqttClientConstants.Topic.OffLine,
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

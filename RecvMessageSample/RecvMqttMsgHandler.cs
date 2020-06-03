using System;
using MqttClientModules;
using MqttClientUtil;
using Newtonsoft.Json.Linq;

namespace RecvMessageSample
{
    internal sealed class RecvMqttMsgHandler : MqttMessageHandler
    {
        public event Action<int> OnRecvValueMsg = (msg) => { };
        public event Action OnRecvValueBeginMsg = () => { };
        public event Action OnRecvValueEndMsg = () => { };

        #region InternalMethods

        protected override void InitActionDic()
        {
            base.InitActionDic();

            //Command
            CmdActionDic.Add(MqttClientConstants.Cmd.SendValue, OnSendValueCmd);
            CmdActionDic.Add(MqttClientConstants.Cmd.SendValueBegin, OnSendValueBeginCmd);
            CmdActionDic.Add(MqttClientConstants.Cmd.SendValueEnd, OnSendValueEndCmd);
        }

        #endregion

        #region Command

        private void OnSendValueCmd(string paras)
        {
            try
            {
                var jObject = JObject.Parse(paras);

                JToken valueToken;
                jObject.TryGetValue(MqttClientConstants.Para.Value, out valueToken);
                var value = valueToken?.ToObject<int>() ?? 0;
                OnRecvValueMsg?.Invoke(value);
            }
            catch (Exception)
            {
                AddErrorMsg($"OnSendValueCmd - JSON解析失败: {paras}");
            }
        }

        private void OnSendValueBeginCmd(string paras)
        {
            OnRecvValueBeginMsg?.Invoke();
        }

        private void OnSendValueEndCmd(string paras)
        {
            OnRecvValueEndMsg?.Invoke();
        }

        #endregion
    }
}

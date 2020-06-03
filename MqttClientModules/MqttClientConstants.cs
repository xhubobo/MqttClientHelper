namespace MqttClientModules
{
    public static class MqttClientConstants
    {
        //系统配置
        public static readonly string MqttClientSendTopic = "MQTT_CLIENT_SEND_MSG";
        public static readonly string MqttClientRecvTopic = "MQTT_CLIENT_RECV_MSG";
        public static readonly string MqttClientHeartbeatTopic = "MQTT_CLIENT_HEARTBEAT_MSG";

        //MqttClient配置
        public static readonly string CmdType = "MqttClientCmd";
        public static readonly string LostPayLoadCmd = CmdType;
        public static readonly string LostPayLoadTopic = "TopicLostPayLoad";

        //主题
        public static class Topic
        {
            public static readonly string OnLine = "TopicOnLine";
            public static readonly string OffLine = "TopicOffLine";
            public static readonly string LostPayLoad = "TopicLostPayLoad";
            public static readonly string Ping = "TopicPing";
            public static readonly string Pang = "TopicPang";
            public static readonly string Exit = "TopicExit";
            public static readonly string Operation = "TopicOperation";
            public static readonly string Callback = "TopicCallback";
        }

        public static readonly string Command = "Command";
        public static readonly string Paras = "Paras";

        public static class Cmd
        {
            public static readonly string SendValue = "SendValue";
        }

        public static class Para
        {
            public static readonly string MachineName = "MachineName";
            public static readonly string Key = "Key";
            public static readonly string Value = "Value";
        }
    }
}

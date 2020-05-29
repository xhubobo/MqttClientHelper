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

        //命令主题
        public static readonly string CmdPing = "Ping";
        public static readonly string CmdCloseAll = "CloseAll";
        public static readonly string CmdExit = "Exit";
        public static readonly string CmdOperation = "Operation";

        //返回主题
        public static readonly string TopicPang = "TopicPang";
        public static readonly string TopicError = "TopicError";
        public static readonly string TopicConnect = "TopicConnect";
        public static readonly string TopicCallback = "TopicCallback";
        public static readonly string TopicOnLine = "TopicOnLine";
        public static readonly string TopicOffLine = "TopicOffLine";
        public static readonly string TopicLostPayLoad = "TopicLostPayLoad";
    }
}

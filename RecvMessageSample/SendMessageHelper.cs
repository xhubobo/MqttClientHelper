using System;
using System.Threading;

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

        public void Start(int interval, int start, int stop)
        {
            if (start > stop)
            {
                return;
            }

            _interval = interval;
            _current = start;
            _stop = stop;

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
            //SendMessage
            OnSendMessage?.Invoke(_current.ToString());

            //校验是否发送完毕
            if (++_current > _stop)
            {
                Stop();
                return;
            }

            //执行下次定时器
            _sendTimer?.Change(
                TimeSpan.FromSeconds(_interval),
                Timeout.InfiniteTimeSpan);
        }
    }
}
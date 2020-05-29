using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RecvMessageSample
{
    internal sealed class RecvMessageHelper
    {
        public event Action<string> OnRecvMessage = (msg) => { };

        private readonly Queue<string> _msgQueue = new Queue<string>();
        private readonly Semaphore _msgSemaphore = new Semaphore(0, int.MaxValue);

        private Thread _threadWorker;
        private bool _threadWorking;
        private readonly object _threadWorkingLockHelper = new object();

        private bool IsThreadWorking
        {
            get
            {
                bool ret;
                lock (_threadWorkingLockHelper)
                {
                    ret = _threadWorking;
                }

                return ret;
            }
            set
            {
                lock (_threadWorkingLockHelper)
                {
                    _threadWorking = value;
                }
            }
        }

        public void Start()
        {
            IsThreadWorking = true;
            _threadWorker = new Thread(DoWork)
            {
                IsBackground = true
            };
            _threadWorker.Start();
        }

        public void Stop()
        {
            IsThreadWorking = false;

            _threadWorker?.Join();
            _threadWorker = null;

            ClearMessage();
        }

        public void HandleMessage(string msg)
        {
            AddMessage(msg);
        }

        #region 消息队列操作

        private void AddMessage(string msg)
        {
            lock (_msgQueue)
            {
                _msgQueue.Enqueue(msg);
                _msgSemaphore.Release();
            }
        }

        private bool HasMessage()
        {
            bool ret;
            lock (_msgQueue)
            {
                ret = _msgQueue.Any();
            }

            return ret;
        }

        private string PickMessage()
        {
            string msg = null;
            lock (_msgQueue)
            {
                if (_msgQueue.Count > 0)
                {
                    msg = _msgQueue.Peek();
                    _msgQueue.Dequeue();
                }
            }

            return msg;
        }

        private void ClearMessage()
        {
            lock (_msgQueue)
            {
                _msgQueue.Clear();
            }
        }

        #endregion

        /// <summary>
        /// 线程执行方法
        /// </summary>
        private void DoWork()
        {
            while (IsThreadWorking || HasMessage())
            {
                if (!_msgSemaphore.WaitOne(1))
                {
                    continue;
                }

                var msg = PickMessage();
                OnRecvMessage.Invoke(msg);
            }
        }
    }
}

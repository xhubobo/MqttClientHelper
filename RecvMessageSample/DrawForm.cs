using System.Threading;
using System.Windows.Forms;

namespace RecvMessageSample
{
    public partial class DrawForm : Form
    {
        //UI线程的同步上下文
        private readonly SynchronizationContext _syncContext;

        public DrawForm()
        {
            InitializeComponent();

            _syncContext = SynchronizationContext.Current;
        }

        private void DrawForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        public void SetValue(int value)
        {
            _syncContext.Post(SetValueSafePost, value);
        }

        private void SetValueSafePost(object state)
        {
            var value = (int)state;
            labelDisplay.Text = value.ToString();
        }
    }
}

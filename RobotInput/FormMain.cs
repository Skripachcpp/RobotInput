using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core;
using Devices.Keyboard;

namespace RobotInput
{
    public partial class FormMain : Form
    {
        private readonly KeyboardEvents _keyboardEvents = KeyboardEvents.Create();
        private readonly Library _library;

        public FormMain()
        {
            InitializeComponent();

            _library = new Library();
            _library.HotKey(true);

            _library.ChangeStatus += Library_ChangeStatus;

            Resize += FormMain_Resize;
            TextChanged += FormMain_TextChanged;

            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            Load += FormMain_Load;

            Disposed += FormMain_Disposed;

            _keyboardEvents.KeyChangeAsync += KeyChangeAsync;
        }

        private readonly List<string> _keyEventLog = new List<string>();

        private void KeyChangeAsync(object sender, KeyArgsAsync args)
        {
            _keyEventLog.Insert(0, string.Format("{0} {1}", args.KeyCode, args.KeyEventType));

            const int maxLogItems = 10;

            lock (_keyEventLog)
                if (_keyEventLog.Count > maxLogItems)
                    _keyEventLog.RemoveRange(maxLogItems, _keyEventLog.Count - maxLogItems);

            lock (label_log)
                if (!label_log.IsDisposed)
                    label_log.Invoke(new Action(() => { label_log.Text = string.Join(Environment.NewLine, _keyEventLog); }));
        }

        private void FromTrey()
        {
            Show();
            notifyIcon.Visible = false;

            WindowState = FormWindowState.Normal;
        }

        private void ToTrey()
        {
            Hide();
            notifyIcon.Visible = true;

            WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Горячие клавиши
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (base.ProcessCmdKey(ref msg, keyData))
                return true;

            switch (keyData)
            {
                case Keys.Escape:
                    ToTrey();
                    break;
            }

            return false;
        }


        private void FormMain_Load(object sender, EventArgs e)
        {
            notifyIcon.Text = Text;
        }

        private void FormMain_TextChanged(object sender, EventArgs e)
        {
            notifyIcon.Text = Text;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                ToTrey();
        }

        public void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            FromTrey();
        }

        private void Library_ChangeStatus(object sender, ChangeStatusArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon.BalloonTipText = string.Format("Macro mashine {0}", e.Status.ToString().ToLower());
                notifyIcon.ShowBalloonTip(0); //параметром указываем количество миллисекунд, которое будет показываться подсказка
            }

            labelStatus.Invoke(new Action<Label, ChangeStatusArgs>((label, args) => { label.Text = args.Status.ToString(); }), labelStatus, e);
        }

        private void FormMain_Disposed(object sender, EventArgs e)
        {
            if (_library != null) _library.Dispose();
        }
    }
}
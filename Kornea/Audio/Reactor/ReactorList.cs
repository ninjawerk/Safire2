using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using Un4seen.Bass;

namespace Kornea.Audio.Reactor
{
    public partial class ReactorList : Form
    {
        public ReactorList()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Pen p = new Pen(Color.AliceBlue);
            e.Graphics.DrawLine(p, new Point(20, 10), new Point(500, 100));
        }

        private void ReactorList_Load(object sender, EventArgs e)
        {
            DispatcherTimer dpt = new DispatcherTimer();
            dpt.Interval = new TimeSpan(0,0,1);
            dpt.Tick += dpt_Tick;
            dpt.IsEnabled = true;
            dpt.Start();
            Player.Instance.PropertyChanged += Instance_PropertyChanged;
            l2.Items.Add("Kornea->Audio-*");
            l2.Items.Add("Safire II");
            l2.Items.Add("critical event triggers");
            l2.Items.Add("developed by Deshan Alahakoon");
        }

        public void Proc(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            l.Items.Add("c" + channel + "-L" + length);
        }

        void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ActiveStreamHandle":
                    l2.Items.Add(e.PropertyName  );
                    l2.Items.Add("handle " + Player.Instance.Handle  );
                    l2.Items.Add("output " + Player.Instance.Output);
                    l2.Items.Add("driver ind " + Bass.BASS_ChannelGetDevice(Player.Instance.Handle));
                   // l2.Items.Add("driver ind " + Bass.BASS_GetDeviceInfo(Bass.BASS_ChannelGetDevice(Player.Instance.Handle)).driver);
                    l2.Items.Add("device " + Bass.BASS_GetDeviceInfo(1).name);
                    l2.Items.Add("device flags" + Bass.BASS_GetDeviceInfo(1).flags);
                    

                    break;
            }
            
        }

        void dpt_Tick(object sender, EventArgs e)
        {
             lst.Items.Clear();
            foreach (var VARIABLE in Reactor.ReactorPool.Pool)
            {
                lst.Items.Add(VARIABLE.Key);
            }
        
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolygonEditor.Notifications
{
    public partial class NotificationError : Form
    {
        public NotificationError()
        {
            InitializeComponent();
        }

        private enum State
        {
            show,
            wait,
            close
        }

        private State state;
        private int x, y;

        private void closeButton_Click(object sender, EventArgs e)
        {
            timer.Interval = 1;
            state = State.close;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            switch(this.state)
            {
                case State.wait:
                    timer.Interval = 5000;
                    state = State.close;
                    break;
                case State.show:
                    timer.Interval = 1;
                    this.Opacity += 0.1;
                    if(this.x < this.Location.X)
                    {
                        this.Left--;
                    }
                    else
                    {
                        if(this.Opacity == 1.0)
                        {
                            state = State.wait;
                        }
                    }
                    break;
                case State.close:
                    timer.Interval = 1;
                    this.Opacity -= 0.1;

                    this.Left -= 3;
                    if(base.Opacity ==0.0)
                    {
                        base.Close();
                    }
                    break;

            }
        }

        public void Show(string message, Screen screen)
        {
            this.Opacity = 0.0;
            this.StartPosition = FormStartPosition.Manual;
            string fname;

            for(int i=0; i<10; i++)
            {
                fname = "error" + i.ToString();
                NotificationError form = (NotificationError)Application.OpenForms[fname];
                if(form == null)
                {
                    this.Name = fname;
                    this.x = screen.WorkingArea.Location.X + screen.WorkingArea.Width - this.Width + 15;
                    this.y = screen.WorkingArea.Location.Y + screen.WorkingArea.Height - this.Height * (i+1) - 5;
                    this.Location = new Point(this.x, this.y);
                    break;
                }
            }
            this.x = screen.WorkingArea.Location.X + screen.WorkingArea.Width - base.Width - 5;
            this.notificationMessage.Text = message;

            this.Show();
            this.state = State.show;
            this.timer.Interval = 1;
            timer.Start();

        }
    }
}

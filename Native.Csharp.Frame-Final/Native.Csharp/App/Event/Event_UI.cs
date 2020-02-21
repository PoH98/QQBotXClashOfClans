using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;

namespace Native.Csharp.App.Event
{
    class Event_UI : ICallMenu
    {
        private Form form;
        public void CallMenu(object sender, CqCallMenuEventArgs e)
        {
            if(form == null)
            {
                form = new Setting();
                form.FormClosing += (s, ev) =>
                {
                    form = null;
                };
                form.Show();
            }
        }


    }
}

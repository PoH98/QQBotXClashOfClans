using Native.Csharp.App.Bot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Native.Csharp.App
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GameAPI.SaveData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GameAPI.ReadData();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(GameAPI.Instance.gameMembers.Keys.Select(x => x.ToString()).ToArray());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (var clanID in BaseData.Instance.config["部落冲突"])
            {
                if (clanID.KeyName.All(char.IsDigit))
                {
                    GameAPI.GetGroupMembers(Convert.ToInt64(clanID.KeyName));
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.AddRange(GameAPI.Instance.gameMembers[Convert.ToInt64(comboBox1.SelectedItem.ToString())].Select(x => x.Member.QQId.ToString() + "|" + x.Member.Card).ToArray());
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var member = GameAPI.Instance.gameMembers[Convert.ToInt64(comboBox1.SelectedItem.ToString())].Where(x => x.Member.QQId.ToString() == comboBox2.SelectedItem.ToString().Split('|')[0]).FirstOrDefault();
            if(member != null)
            {
                numericUpDown1.Value = member.Exp;
                numericUpDown2.Value = member.Cash;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var member = GameAPI.Instance.gameMembers[Convert.ToInt64(comboBox1.SelectedItem.ToString())].Where(x => x.Member.QQId.ToString() == comboBox2.SelectedItem.ToString().Split('|')[0]).FirstOrDefault();
            if (member != null)
            {
                member.Exp = Convert.ToInt32(numericUpDown1.Value);
            }
            GameAPI.SaveData();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            var member = GameAPI.Instance.gameMembers[Convert.ToInt64(comboBox1.SelectedItem.ToString())].Where(x => x.Member.QQId.ToString() == comboBox2.SelectedItem.ToString().Split('|')[0]).FirstOrDefault();
            if (member != null)
            {
                member.Cash = Convert.ToInt32(numericUpDown2.Value);
            }
            GameAPI.SaveData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GameAPI.ReadData();
        }
    }
}

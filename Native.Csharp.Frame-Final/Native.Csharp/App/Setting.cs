using Native.Csharp.App.Bot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Native.Csharp.App
{
    public partial class Setting : Form
    {
        private ProcessStartInfo cmd = new ProcessStartInfo()
        {
            FileName = "cmd.exe",
            Arguments = "/c bin\\node.exe .src/decompressApk.js i --loglevel error",
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };
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
                    try
                    {
                        GameAPI.GetGroupMembers(Convert.ToInt64(clanID.KeyName));
                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "群组加载", clanID.KeyName + "加载完毕！");
                    }
                    catch(Exception exception)
                    {
                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "群组加载失败", clanID.KeyName + "Exception: " + exception.ToString());
                        continue;
                    }
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

        private void button5_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Apk")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Apk"));
                MessageBox.Show("请把apk丢到文件夹内");
                Process.Start("explorer.exe",Path.Combine(Environment.CurrentDirectory, "Apk"));
                return;
            }
            if (!Directory.Exists("bin\\src"))
            {
                File.WriteAllBytes("sc-compression.zip", Resource.sc_compression);
                ZipFile.ExtractToDirectory("sc-compression.zip", "bin");
                File.Delete("sc-compression.zip");
            }
            var apks = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Apk"), "*.apk");
            if (apks.Length > 0)
            {
                foreach(var apk in apks)
                {
                    textBox1.AppendText("Extracting files " + apk + Environment.NewLine);
                    File.Move(apk, apk.Replace(".apk", ".zip"));
                    try
                    {
                        ZipFile.ExtractToDirectory(apk.Replace(".apk", ".zip"), "Apk");
                    }
                    catch
                    {

                    }
                    File.Move(apk.Replace(".apk", ".zip"), apk);
                    Decompress(apk.Remove(apk.Length-4));
                }
            }
            textBox1.AppendText("\n============================\nDone!\n============================\n");
        }

        private void Decompress (string path)
        {
            if(Directory.GetDirectories(path).Length > 0)
            {
                foreach(var subPath in Directory.GetDirectories(path))
                {
                    Decompress(subPath);
                }
            }
            textBox1.AppendText("Decompress " + path);
            File.WriteAllText("bin\\src\\decompressApk.js", Resource.decompressApk.Replace("#PATH#", $"const anyPath = '{path.Replace("\\", "/")}'"));
            Process.Start(cmd);
            textBox1.AppendText("Decompress done!");
        }
    }
}

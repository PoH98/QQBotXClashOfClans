using Native.Csharp.App.Bot;
using Native.Csharp.App.GameData;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SC_Compression;

namespace Native.Csharp.App
{
    public partial class Setting : Form
    {
        private ProcessStartInfo cmd = new ProcessStartInfo()
        {
            FileName = "cmd.exe",
            Arguments = "/c bin\\node.exe bin/src/decompressApk.js i --loglevel error",
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
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

        private async void button5_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Apk")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Apk"));
                MessageBox.Show("请把apk丢到文件夹内");
                Process.Start("explorer.exe",Path.Combine(Environment.CurrentDirectory, "Apk"));
                return;
            }
            var apks = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Apk"), "*.apk");
            if (apks.Length > 0)
            {
                foreach(var apk in apks)
                {
                    textBox1.AppendText("Extracting files " + apk + "\n");
                    File.Move(apk, apk.Replace(".apk", ".zip"));
                    try
                    {
                        ZipFile.ExtractToDirectory(apk.Replace(".apk", ".zip"), "Apk");
                    }
                    catch
                    {

                    }
                    File.Move(apk.Replace(".apk", ".zip"), apk);
                    Thread t = new Thread(()=>
                    {
                        Decompress(apk.Remove(apk.Length - 4));
                    });
                    t.IsBackground = true;
                    t.Start();
                    int waitTime = 0;
                    do
                    {
                        await Task.Delay(1000);
                        if(waitTime > 30)
                        {
                            textBox1.AppendText("\n============================\nFailed!\n============================\n");
                            return;
                        }
                        waitTime++;
                    }
                    while (t.ThreadState == System.Threading.ThreadState.Running);
                }
            }
            textBox1.AppendText("\n============================\nDone!\n============================\n");
        }

        private void Decompress(string path)
        {
            if (!Directory.Exists(path))
            {
                path = "Apk";
            }
            if (Directory.GetDirectories(path).Length > 0)
            {
                foreach (var subPath in Directory.GetDirectories(path))
                {
                    Decompress(subPath);
                }
            }
            foreach(var file in Directory.GetFiles(path).Where(x => x.EndsWith(".csv") || x.EndsWith(".sc")))
            {
                textBox1.Invoke((MethodInvoker)delegate {
                    textBox1.AppendText("Decompressing "+file+" !\n");
                });
                File.WriteAllBytes(file, scCompression.Decompress(file));
                textBox1.Invoke((MethodInvoker)delegate {
                    textBox1.AppendText("Decompress done!\n");
                });
            }
        }

        private void Compress(string path)
        {
            if (!Directory.Exists(path))
            {
                path = "Apk";
            }
            if (Directory.GetDirectories(path).Length > 0)
            {
                foreach (var subPath in Directory.GetDirectories(path))
                {
                    Compress(subPath);
                }
            }
            foreach (var file in Directory.GetFiles(path, "*.csv"))
            {
                textBox1.Invoke((MethodInvoker)delegate {
                    textBox1.AppendText("Compressing " + file + " !\n");
                });
                File.WriteAllBytes(file, scCompression.Compress(file));
                textBox1.Invoke((MethodInvoker)delegate {
                    textBox1.AppendText("Compress done!\n");
                });
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "强制召唤Boss", "必须选择群后才能召唤Boss!");
                return;
            }
            if(!BossFight.Instance.boss.ContainsKey(Convert.ToInt64(comboBox1.SelectedItem.ToString())))
            {
                BossFight.Instance.boss.Add(Convert.ToInt64(comboBox1.SelectedItem.ToString()), new Boss(Convert.ToInt64(comboBox1.SelectedItem.ToString())));
                Common.CqApi.SendGroupMessage(Convert.ToInt64(comboBox1.SelectedItem.ToString()), "Boss被Admin强制召唤到了村庄！召集勇士一起打败Boss吧！Boss逃离时间: " + BossFight.Instance.boss[Convert.ToInt64(comboBox1.SelectedItem.ToString())].metTime.AddHours(3));
            }
            else if(BossFight.Instance.boss[Convert.ToInt64(comboBox1.SelectedItem.ToString())].HP > 0)
            {
                BossFight.Instance.boss.Add(Convert.ToInt64(comboBox1.SelectedItem.ToString()), new Boss(Convert.ToInt64(comboBox1.SelectedItem.ToString())));
                Common.CqApi.SendGroupMessage(Convert.ToInt64(comboBox1.SelectedItem.ToString()), "Boss被Admin强制召唤到了村庄！召集勇士一起打败Boss吧！Boss逃离时间: " + BossFight.Instance.boss[Convert.ToInt64(comboBox1.SelectedItem.ToString())].metTime.AddHours(3));
            }
            else
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "强制召唤Boss", "Boss已经存在，无法再次召唤！");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.ScrollToCaret();
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Apk")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Apk"));
                MessageBox.Show("请把要打包的文件丢到文件夹内");
                Process.Start("explorer.exe", Path.Combine(Environment.CurrentDirectory, "Apk"));
                return;
            }
            foreach(var apk in Directory.GetFiles("Apk", "*.apk"))
            {
                File.Move(apk, apk.Replace("\\Apk\\", "\\"));
            }
            Thread t = new Thread(() =>
            {
                Compress("Apk");
            });
            t.IsBackground = true;
            t.Start();
            int waitTime = 0;
            do
            {
                await Task.Delay(1000);
                if (waitTime > 30)
                {
                    textBox1.AppendText("\n============================\nFailed!\n============================\n");
                    return;
                }
                waitTime++;
            }
            while (t.ThreadState == System.Threading.ThreadState.Running);
            try
            {
                ZipFile.CreateFromDirectory("Apk", "ClashOfClans_MOD.apk");
            }
            catch
            {

            }
            try
            {
                foreach (var apk in Directory.GetFiles(Environment.CurrentDirectory, "*.apk"))
                {
                    File.Move(apk, apk.Replace(Environment.CurrentDirectory, "\\Apk\\"));
                }
            }
            catch
            {

            }
            textBox1.AppendText("\n============================\nDone!\n============================\n");
        }
    }
}

using Native.Csharp.App.Bot;
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
using Unity.Interception.Utilities;
using Native.Csharp.App.Bot.Game;
using IniParser;

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

        private void Setting_Load(object sender, EventArgs e)
        {
            if(Directory.Exists("com.coc.groupadmin"))
                Directory.GetDirectories("com.coc.groupadmin").ForEach(x => comboBox1.Items.Add(x.Remove(0, x.LastIndexOf('\\') + 1)));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Directory.GetFiles("com.coc.groupadmin\\" + comboBox1.SelectedItem.ToString()).ForEach(x => comboBox2.Items.Add(x.Remove(0, x.LastIndexOf('\\') + 1).Replace(".bin", "")));
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (var member = new GameAPI(long.Parse(comboBox1.SelectedItem.ToString()), long.Parse(comboBox2.SelectedItem.ToString())))
            {
                numericUpDown1.Value = member.member.Exp;
                numericUpDown2.Value = member.member.Cash;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            using (var member = new GameAPI(long.Parse(comboBox1.SelectedItem.ToString()), long.Parse(comboBox2.SelectedItem.ToString())))
            {
                member.member.Exp = Convert.ToInt32(numericUpDown1.Value);
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            using (var member = new GameAPI(long.Parse(comboBox1.SelectedItem.ToString()), long.Parse(comboBox2.SelectedItem.ToString())))
            {
                member.member.Cash = Convert.ToInt32(numericUpDown2.Value);
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            /*if(comboBox1.SelectedIndex > -1)
            {
                ICocCoreClans clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var members = clan.GetClansMembers(BaseData.valuePairs(configType.部落冲突)[comboBox1.SelectedItem.ToString()]);
                //Threading.UpdateMemberInClanStatus(Convert.ToInt64(comboBox1.SelectedItem.ToString()), members);
            }*/
            FileIniDataParser parse = new FileIniDataParser();
            BaseData.UpdateTranslate(parse);
            BaseData.UpdateTownhallINI(parse);
        }
    }
}

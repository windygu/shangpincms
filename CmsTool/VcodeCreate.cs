using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Framework.Common.Cache;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Service.Shangpin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CmsTool
{
    public partial class VcodeCreate : Form
    {
        public VcodeCreate()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 生产微码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("你确定一定以及肯定要生成微码吗？", "警告！！！！！！！", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (confirm != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            int outCount = 0;
            try
            {
                VCodeService Vcode = new VCodeService();
                string activityId = textBox1.Text;//活动ID
                int count = int.Parse(textBox2.Text);//生成数量
                string activityCode = textBox3.Text;//活动编码
                string operatorId = textBox4.Text;//操作人


                string code = string.Empty;
                for (int i = 0; i < count; i++)
                {
                    SWfsVActivityCodesRef obj = new SWfsVActivityCodesRef();
                    code = activityCode + GetRandomCodeII(6);
                    obj.ActivityId = activityId;
                    obj.VCode = code;
                    obj.Source = "系统生成";
                    obj.DateCreate = DateTime.Now;
                    obj.OperatorId = operatorId;
                    Vcode.CreateCrode(obj);

                    outCount += 1;
                    System.Threading.Thread.Sleep(10);
                }

                MessageBox.Show("生成完毕，本次共生成微码数量为：" + outCount);
            }
            catch(Exception ex)
            {
                MessageBox.Show("【本次已生成" + outCount + "个微码】，但目前" + "出现异常：" + ex.Message + ex.Source + ex.StackTrace);
            }
        }

        /// <summary>
        /// 随机数
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string GetRandomCodeII(int N)
        {
            char[] arrChar = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            StringBuilder num = new StringBuilder();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < N; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }
    }
}

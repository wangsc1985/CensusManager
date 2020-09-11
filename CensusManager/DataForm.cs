using CensusManager.helper;
using CensusManager.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CensusManager
{
    public partial class DataForm : Form
    {
        private List<Village> allVillageList;
        public DataForm()
        {
            InitializeComponent();
            allVillageList = CensusContext.GetVillages();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
        }

        private void CoreWebView2_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            webView.CoreWebView2.ExecuteScriptAsync("fun01();");
        }

        private void DataForm_Load(object sender, EventArgs e)
        {
            listBox1.Items.Add("东场村");
            listBox1.Items.Add("二屯村");
            listBox1.Items.Add("任珩村");
            listBox1.Items.Add("八屯村");
            listBox1.Items.Add("兰庄村");
            listBox1.Items.Add("刘庄村");
            listBox1.Items.Add("国庄村");
            listBox1.Items.Add("师庄村");
            listBox1.Items.Add("朱庄村");
            listBox1.Items.Add("李堂村");
            listBox1.Items.Add("杨楼村");
            listBox1.Items.Add("林庄村");
            listBox1.Items.Add("桑庄村");
            listBox1.Items.Add("滕庄村");
            listBox1.Items.Add("满庄村");
            listBox1.Items.Add("潘庄村");
            listBox1.Items.Add("燕庄村");
            listBox1.Items.Add("王珩村");
            listBox1.Items.Add("石庄村");
            listBox1.Items.Add("管珩村");
            listBox1.Items.Add("闫庄村");
            listBox1.Items.Add("韩家洼村");
            listBox1.Items.Add("青苏厂村");
            listBox1.Items.Add("罗厂村");
            listBox1.Items.Add("袁厂村");
            listBox1.Items.Add("辛厂村");
            listBox1.Items.Add("大吴庄村");
            listBox1.Items.Add("邢庄村");
            listBox1.Items.Add("南甘泉村");
            listBox1.Items.Add("北甘泉村");
            listBox1.Items.Add("李贤屯村");
            listBox1.Items.Add("苏庄村");
            listBox1.Items.Add("沙虎庄村");
            listBox1.Items.Add("漳南镇村");
            listBox1.Items.Add("王贤屯村");
            listBox1.Items.Add("郭庄村");
            listBox1.Items.Add("小堤口村");
            listBox1.Items.Add("西赵庄村");
            listBox1.Items.Add("鲁珩村");
            listBox1.Items.Add("陈珩村");
            listBox1.Items.Add("管辛庄村");
            listBox1.Items.Add("小李庄村");
            listBox1.Items.Add("大李庄村");
            listBox1.Items.Add("西王庄村");
            listBox1.Items.Add("张官屯村");
            listBox1.Items.Add("高官屯村");
            listBox1.Items.Add("刘军户村");
            listBox1.Items.Add("付家楼村");
            listBox1.Items.Add("付家坊村");
            listBox1.Items.Add("东王屯村");
            listBox1.Items.Add("西王屯村");
            listBox1.Items.Add("宋王庄村");
            listBox1.Items.Add("马庄村");
            listBox1.Items.Add("陈庄村");
            listBox1.Items.Add("小魏庄村");
            listBox1.Items.Add("东赵庄村");
            listBox1.Items.Add("小张庄村");
            listBox1.Items.Add("孔官屯村");
            listBox1.Items.Add("张郭秦村");
            listBox1.Items.Add("小贾庄村");
            listBox1.Items.Add("代官屯村");
            listBox1.Items.Add("小史庄村");
            listBox1.Items.Add("胡家洼村");
            listBox1.Items.Add("南郑庄村");
            listBox1.Items.Add("北郑庄村");
            listBox1.Items.Add("张马尧村");
            listBox1.Items.Add("北宋庄村");
            listBox1.Items.Add("前九村");
            listBox1.Items.Add("后九村");
            listBox1.Items.Add("东良村");
            listBox1.Items.Add("西良村");
            listBox1.Items.Add("西吴庄村");
            listBox1.Items.Add("西邱村");
            listBox1.Items.Add("南宋庄村");
            listBox1.Items.Add("小于庄村");
            listBox1.Items.Add("北赵庄村");
            listBox1.Items.Add("北伍旗村");
            listBox1.Items.Add("大于庄村");
            listBox1.Items.Add("西郑庄村");
            listBox1.Items.Add("大史庄村");
            listBox1.Items.Add("第十屯村");
            listBox1.Items.Add("朱家圈村");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            foreach (var aa in allVillageList)
            {
                if (aa.name.Contains(this.listBox1.SelectedItem.ToString()))
                {
                    StringBuilder fun01 = new StringBuilder();
                    fun01.Append("function fun01() {");
                    fun01.Append("  $('#dsbm').val('371400');");
                    fun01.Append("  $('#qxbm').val('371428');");
                    fun01.Append("  $('#ds').val('德州市');");
                    fun01.Append("  $('#qx').val('武城县');");
                    fun01.Append($"  $('#dzms').val('鲁权屯镇{this.listBox1.SelectedItem.ToString()}');");
                    fun01.Append("  DoSubmit();");
                    fun01.Append("}");

                    StringBuilder fun02 = new StringBuilder();
                    fun02.Append("function fun02() {");
                    fun02.Append("   alert($('.dataList').html());");
                    fun02.Append("}");
                    webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(fun01.ToString());
                    webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(fun02.ToString());
                    webView.CoreWebView2.Navigate("https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/dzcx.jsp");
                    webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

                }
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.ExecuteScriptAsync("fun02();");
        }
    }
}

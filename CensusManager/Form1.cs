using CensusManager.helper;
using CensusManager.model;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using 人口普查;

namespace CensusManager
{

    //第0列：户号，无用。第1列：关系。第2列：名字。第3列：身份证号。第4列：民族。第5列：住址。

    public partial class Form1 : Form
    {
        private string url = "";
        private int step = 0;


        private delegate void FormControlInvoker();
        private List<Village> currentVillageList;
        private List<Build> currentBuildList;
        private List<Person> currentPersonList;
        public Form1()
        {
            //string abc = "dzyslx= \"'>  d+ \" </ div > \"";
            //Regex reg = new Regex("(?<=dzyslx=.*>).{1,10}(?=</div>)");
            //var numberCollection = reg.Matches(abc);

            InitializeComponent();

            //string a = "https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/zzsb.jsp?data=pVC3T2lZmIc4jlXXaJyILe5mZPxaaeqx9L43ge5lIGEZynQ860PsjuB9O9RTeDRDDd7uGjeiovBVYWXnFq7eP9BbvWenwLF4GtiXgcdDRyjwT42sYSdvrIEumJkdNvmzWajK9tYQDXfa7nbpvKwtQ11FBaTHxyEVYr1c1mefBapa9FyqcyMhKVGfcbYd%2BL5v58KJHHTREyHElKceWNCgQUd7IhmaglLkaiTu2Awqjq0%3D";
            //Process.Start(a);

            CensusContext.Connect();
            currentVillageList = CensusContext.GetVillages();
        }

        string[] dragFiles;
        /**
         * 解析出村庄GUID
         */

        private void parsePerson()
        {
            try
            {
                int newCount = 0, redo = 0, illegal = 0;

                foreach (var dragFile in dragFiles)
                {
                    ExcelHelper.ParseExcel(dragFile, "常住人口", (sheet) =>
                    {
                        int length = sheet.Rows.Count;

                        /**
                         * 第0列：户号。第1列：关系。第2列：名字。第3列：身份证号。第4列：住址。
                         */
                        int currentRowIndex = 0;
                        while (currentRowIndex < length)
                        {
                            this.Invoke(new FormControlInvoker(() =>
                            {
                                statusLabel.Text = dragFile + "，" + currentRowIndex + "行";
                            }));
                            string no = sheet.Rows[currentRowIndex][0].ToString().Trim();
                            string ralation = sheet.Rows[currentRowIndex][1].ToString().Trim();
                            string name = sheet.Rows[currentRowIndex][2].ToString().Trim();
                            string id = sheet.Rows[currentRowIndex][3].ToString().Trim();
                            string address = sheet.Rows[currentRowIndex][4].ToString().Trim();

                            currentRowIndex++;
                            if (no.Trim().Length != 9 || id.Trim().Length != 18)
                            {
                                illegal++;
                                continue;
                            }
                            if (!CensusContext.IsExistPersons(id))
                            {
                                newCount++;
                                CensusContext.AddPerson(new Person(ralation, name, id, "汉族", address));
                            }
                            else
                            {
                                redo++;
                            }
                        }
                    });
                }
                MessageBox.Show("新数据 " + newCount + " 条，重复数据 " + redo + " 条，不合格数据 " + illegal + " 条");
            }
            catch (Exception)
            {
            }
            finally
            {
                this.Invoke(new FormControlInvoker(() =>
                {
                    statusLabel.Text = "";
                }));
            }
        }
#if false
        private void parseVillage()
        {
            StreamReader reader = null;
            try
            {
                string fileNameEx = Path.GetFileName(dragFiles[0]), fileName = Path.GetFileNameWithoutExtension(dragFiles[0]);

                if (!fileName.Equals("村庄"))
                {
                    MessageBox.Show("请拖入文件名为【村庄】的文件");
                    throw new Exception();
                }


                reader = File.OpenText(dragFiles[0]);
                string content = reader.ReadToEnd();
                Regex reg = new Regex("[0-9A-Za-z]{8}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{12}");
                var guidCollection = reg.Matches(content);
                reg = new Regex("[\u4E00-\u9FA5]+");
                var nameCollection = reg.Matches(content);


                int newCount = 0, redo = 0;
                for (int i = 0; i < guidCollection.Count; i++)
                {
                    string guid = guidCollection[i].Value.Trim();
                    string name = nameCollection[i].Value.Trim();
                    if (allVillageList.FirstOrDefault(model => model.guid.Equals(guid)) == null)
                    {
                        newCount++;
                        allVillageList.Add(new Village(guid, name));

                    }
                    else
                    {
                        redo++;
                    }
                }

                this.Invoke(new FormControlInvoker(() =>
                {
                    listBoxVillage.Items.Clear();
                }));
                foreach (var village in allVillageList)
                {
                    this.Invoke(new FormControlInvoker(() =>
                    {
                        this.listBoxVillage.Items.Add(new Village(village.guid, village.name));
                    }));
                }


                //开始序列化
                saveVillage(allVillageList);

                MessageBox.Show("导入新数据 " + newCount + " 条，重复数据 " + redo + " 条");
            }
            catch (Exception)
            {
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /**
         * 解析出房屋guid数据
         * */
        private void parseBuild()
        {
            StreamReader reader = null;
            try
            {
                int allNewCount = 0, allRedo = 0, fileCount = 0;
                StringBuilder sb = new StringBuilder();
                foreach (var dragFile in dragFiles)
                {
                    reader = File.OpenText(dragFile);
                    string content = reader.ReadToEnd();
                    Regex reg = new Regex("[0-9A-Za-z]{8}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{12}");
                    var guidCollection = reg.Matches(content);
                    //dzyslx = "50" > 009号 </ div >
                    reg = new Regex("(?<=dzyslx=.*>).{1,10}(?=</div>)");
                    var numberCollection = reg.Matches(content);
                    reg = new Regex("[0-9]{21}");
                    var midCollection = reg.Matches(content);
                    Village village = null;

                    int newCount = 0, redo = 0;
                    string fileNameEx = Path.GetFileName(dragFile), fileName = Path.GetFileNameWithoutExtension(dragFile);

                    if (guidCollection.Count != numberCollection.Count || guidCollection.Count != midCollection.Count || midCollection.Count != numberCollection.Count)
                    {
                        MessageBox.Show("【" + fileName + "】的房屋数据不平衡，请联系管理员。GUID：" + guidCollection.Count + "，MID：" + midCollection.Count + "，NUMBER：" + numberCollection.Count);
                        throw new Exception("【" + fileName + "】的房屋数据不平衡。");
                    }


                    village = allVillageList.FirstOrDefault(model => model.name.Contains(fileName));
                    if (village == null)
                    {
                        MessageBox.Show("不存在【" + fileName + "】的村庄数据，请上传新的村庄信息，或者检查文件名是否正确。");
                        throw new Exception("不存在【" + fileName + "】的村庄数据");
                    }

                    for (int i = 0; i < guidCollection.Count; i++)
                    {
                        string guid = guidCollection[i].Value.Trim();
                        string mid = midCollection[i].Value.Trim();
                        string number = numberCollection[i].Value.Trim();
                        if (allBuildList.FirstOrDefault(model => model.guid.Equals(guid)) == null)
                        {
                            newCount++;
                            //allBuildList.Add(new Build(guid, mid, Regex.Match(number, "[0-9]{1,4}号").Value, village.guid));
                            allBuildList.Add(new Build(guid, mid, number, village.guid));
                        }
                        else
                        {
                            redo++;
                        }
                    }

                    saveBuild(allBuildList);
                    sb.Append(village.name + "导入新数据 " + newCount + " 条，重复数据 " + redo + " 条\n\r");
                    allNewCount += newCount;
                    allRedo += redo;
                    fileCount++;
                }

                MessageBox.Show(sb.Append("共导入 " + fileCount + "个文件，包含新数据 " + allNewCount + " 条，重复数据 " + allRedo + " 条").ToString());
            }
            catch (Exception)
            {

            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            //
        }
#endif
        private void parseVillage()
        {
            StreamReader reader = null;
            List<MoveFile> moveFiles = new List<MoveFile>();
            try
            {
                int newVillageCount = 0, newBuildCount = 0, oldVillageCount = 0, oldBuildCount = 0, fileCount = 0;
                StringBuilder sb = new StringBuilder();

                int ic = 0, size = dragFiles.Length;
                foreach (var dragFile in dragFiles)
                {
                    ic++;
                    this.Invoke(new FormControlInvoker(() =>
                    {
                        statusLabel.Text = $"{dragFile}，{ic}/{size}";
                    }));
                    reader = File.OpenText(dragFile);
                    string content = reader.ReadToEnd();
                    Regex reg = new Regex("(?<=guid=)[0-9A-Za-z]{8}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{12}");
                    var villageGuidCollection = reg.Matches(content);
                    reg = new Regex("(?<=guid=\")[0-9A-Za-z]{8}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{12}");
                    var guidCollection = reg.Matches(content);
                    reg = new Regex("(?<=dzyslx=[^>]{1,10}>)[^<]{0,10}");
                    var numberCollection = reg.Matches(content);
                    reg = new Regex("(?<=mid=\")[0-9]{21}");
                    var midCollection = reg.Matches(content);
                    reg = new Regex("(?<=<span>)[\u4E00-\u9FA5]+(?=</span>)");
                    var villageNameCollection = reg.Matches(content);

                    string fileNameEx = Path.GetFileName(dragFile), fileName = Path.GetFileNameWithoutExtension(dragFile);
                    if (!(numberCollection.Count == midCollection.Count&& midCollection.Count == guidCollection.Count&&villageGuidCollection.Count==1&&villageNameCollection.Count==1))
                    {
                        MessageBox.Show($"file : {fileName};village guid count : {villageGuidCollection.Count} ;village name count : {villageNameCollection.Count} ;guid count : {guidCollection.Count}; number count : {numberCollection.Count} ;mid count : {midCollection.Count} . ");
                        throw new Exception("【" + fileName + "】的房屋数据不平衡。");
                    }

                    if (numberCollection.Count == 0)
                    {
                        MessageBox.Show($"file : {dragFile}");
                        throw new Exception("【" + fileName + "】不存在数据。");
                    }

                    /**
                     * 解析村庄
                     * */
                    string villageName = villageNameCollection[0].Value.Trim();
                    string villageGuid = villageGuidCollection[0].Value.Trim();

                    Village village = CensusContext.GetVillage(villageName);
                    if (village == null)
                    {
                        village = new Village(villageGuid, villageName);
                        CensusContext.AddVillage(village);
                        newVillageCount++;
                    }
                    else
                    {
                        oldVillageCount++;
                    }

                    FileInfo fi = new FileInfo(dragFile);
                    MoveFile mf = new MoveFile();
                    mf.oldPath = dragFile;
                    mf.newPath = fi.Directory.FullName + "\\" + villageName.Replace("鲁权屯镇", "") + ".html";
                    moveFiles.Add(mf);

                    /**
                     * 解析房屋
                     * */
                    for (int i = 0; i < numberCollection.Count; i++)
                    {
                        string guid = guidCollection[i].Value.Trim();
                        string mid = midCollection[i].Value.Trim();
                        string number = numberCollection[i].Value.Trim();
                        if (!CensusContext.IsExistBuild(guid))
                        {
                            newBuildCount++;
                            //allBuildList.Add(new Build(guid, mid, Regex.Match(number, "[0-9]{1,4}号").Value, village.guid));
                            CensusContext.AddBuild(new Build(guid, mid, number, village.guid));
                        }
                        else
                        {
                            oldBuildCount++;
                        }
                    }

                    //saveBuild(allBuildList);
                    //sb.Append(village.name + "导入新数据 " + newCount + " 条，重复数据 " + redo + " 条\n\r");
                    fileCount++;
                }
                currentVillageList = CensusContext.GetVillages();
                this.Invoke(new FormControlInvoker(() =>
                {
                    listBoxVillage.Items.Clear();
                }));
                foreach (var v in currentVillageList)
                {
                    this.Invoke(new FormControlInvoker(() =>
                    {
                        this.listBoxVillage.Items.Add(new Village(v.guid, v.name.Replace("鲁权屯镇", "")));
                    }));
                }

                MessageBox.Show(sb.Append($"共导入{fileCount}个文件。\r\n{newVillageCount}个村庄新数据，{oldVillageCount}个已存在村庄数据。\r\n{newBuildCount}个房屋新数据，{oldBuildCount}个已存在房屋数据。").ToString());
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (reader != null)
                    reader.Close();

                foreach (var f in moveFiles)
                {
                    FileInfo fi = new FileInfo(f.oldPath);
                    if (!f.oldPath.Equals(f.newPath))
                        fi.MoveTo(f.newPath);
                }
                this.Invoke(new FormControlInvoker(() =>
                {
                    statusLabel.Text = "";
                }));
            }
            //
        }

        private void listBoxBuild_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //var currentBuild = this.listBoxBuild.SelectedItem as Build;
            //url = "https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/rysb.jsp?guid=" + currentBuild.guid;
            //Process.Start(url);
            //Process.Start("iexplore.exe", url);

        }

        private void label房屋_Click(object sender, EventArgs e)
        {
            DataForm dataForm = new DataForm();
            dataForm.Show();
        }

        private void label住户_Click(object sender, EventArgs e)
        {
            //allowClick = !allowClick;
            //if (allowClick)
            //{
            //    label3.BackColor = Color.Red;
            //}
            //else
            //{
            //    label3.BackColor = Color.Transparent;
            //}
            //http();
            //MessageBox.Show(HttpHelper.PostHttpByJson("https://localhost:44342/Home/Json", "{'Name':'wangsc','Age':'18'}"));
            //MessageBox.Show(HttpHelper.PostHttpByJson("https://localhost:44342/Users/Create", "{'Name':'wangsc','Age':'18'}"));


            /**
             * ajax获取户籍信息
             */
            //string json = "{'pid':371428198412174015,'name':王世起}";
            //string uri = "https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/cxthr.jsp";
            //string html = HttpHelper.PostHttpByJson(uri, json);

        }

        private void listBoxVillage_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var currentVillage = this.listBoxVillage.SelectedItem as Village;
            string url = "https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/rysb.jsp?guid=" + currentVillage.guid;
            Process.Start(url);
            //Process.Start("iexplore.exe", url);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (var v in currentVillageList)
            {
                v.name = v.name.Replace("鲁权屯镇", "");
                this.listBoxVillage.Items.Add(v);
            }
            //url = "https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/rysb.jsp";
            //web.CoreWebView2.Navigate(url);
        }

        private void listBoxVillage_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.listBoxBuild.Items.Clear();
            this.dataGridViewPerson.Rows.Clear();
            var currentVillage = this.listBoxVillage.SelectedItem as Village;
            //currentVillage = villageList.First(model => model.name.Equals(vn));
            if (currentVillage != null)
            {
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                currentBuildList = CensusContext.GetBuilds(currentVillage.guid);
                sw.Stop();
                var span = sw.ElapsedMilliseconds;

                foreach (var b in currentBuildList)
                {
                    this.listBoxBuild.Items.Add(b);
                }
            }
        }
        private void listBoxBuild_SelectedIndexChanged(object sender, EventArgs e)
        {
            step = 0;
            var currentBuild = this.listBoxBuild.SelectedItem as Build;
            var currentVillage = this.listBoxVillage.SelectedItem as Village;
            this.dataGridViewPerson.Rows.Clear();
            url = "https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/rysb.jsp?guid=" + currentBuild.guid;

            if (currentBuild != null)
            {
                currentPersonList = CensusContext.GetPersons(currentVillage.name, currentBuild.number);


                StringBuilder fun02 = new StringBuilder();
                fun02.Append("function fun02() {");
                fun02.Append("  $('.xshcyxx').remove();");
                fun02.Append("  $('.hjcyList').remove();");
                fun02.Append("  $(\"[class='sbrTit bbb vvv']\").remove();");
                fun02.Append("  $(\"[class='addtzrBtn tianjia']\").remove();");
                fun02.Append("  $('#hkwt_whcd').val('');");
                fun02.Append("  $('#hkwt_whcd').attr('code', '');");
                fun02.Append("  $('#hkwt_byzk').val('');");
                fun02.Append("  $('#hkwt_byzk').attr('code', '');");
                fun02.Append("  $('#hkwt_hyzk').val('');");
                fun02.Append("  $('#hkwt_hyzk').attr('code', '');");
                fun02.Append("  $('#hkwt_xx').val('');");
                fun02.Append("  $('#hkwt_xx').attr('code', '');");
                fun02.Append("  $('#hkwt_sg').val('');");
                fun02.Append("  $('#hkwt_zy').val('');");
                fun02.Append("  $('#hkwt_fwcs').val('');");
                fun02.Append("}");
                //fun02.Append("fun02();");

                StringBuilder fun01 = new StringBuilder();
                fun01.Append("function fun01() {");
                fun01.Append("$('#mySwitch2').removeClass('mui-active');");
                fun01.Append("$('.mui-switch-handle').attr('style', 'transition-duration: 0.2s; transform: translate(0px, 0px);');");

                fun01.Append("$('#pid').attr('readonly', false);");
                fun01.Append("$('#name').attr('readonly', false);");




                int i = 0;
                int hz = 0;
                foreach (var b in currentPersonList)
                {
                    int index = this.dataGridViewPerson.Rows.Add();
                    this.dataGridViewPerson.Rows[index].Cells[0].Value = b.relation;
                    this.dataGridViewPerson.Rows[index].Cells[1].Value = b.name;
                    this.dataGridViewPerson.Rows[index].Cells[2].Value = b.id;
                    this.dataGridViewPerson.Rows[index].Cells[3].Value = b.address;
                    if (b.relation.Equals("户主") && hz == 0)
                    {
                        hz++;
                        DataGridViewCellStyle cs = new DataGridViewCellStyle();
                        cs.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                        this.dataGridViewPerson.Rows[index].Cells[0].Style = cs;
                        this.dataGridViewPerson.Rows[index].Cells[1].Style = cs;
                        this.dataGridViewPerson.Rows[index].Cells[2].Style = cs;
                        this.dataGridViewPerson.Rows[index].Cells[3].Style = cs;

                        fun01.Append($"$('#pid').val('{b.id}');");
                        fun01.Append($"$('#name').val('{b.name}');");
                        //builder.Append($"$('#info').trigger('blur');");
                    }
                    else
                    {
                        //builder.Append("if($('.addBox').size()>0) return;");
                        fun01.Append("$('.tianjia').trigger('tap');");
                        fun01.Append($" $('.addBox').eq({i}).find('.tzrPid').first().val('{b.id}');");
                        fun01.Append($" $('.addBox').eq({i}).find('.tzrName').first().val('{b.name}');");
                        i++;
                    }
                }
                fun01.Append("}");
                web.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(fun01.ToString());
                web.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(fun02.ToString());
                web.CoreWebView2.Navigate(url);

                //web.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
                //web.CoreWebView2.Settings.IsWebMessageEnabled = true;
                //web.CoreWebView2.ScriptDialogOpening += CoreWebView2_ScriptDialogOpening1;
                //web.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                //web.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;

                System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
                dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                dataGridViewCellStyle1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                dataGridViewCellStyle1.BackColor = Color.DarkRed;
                dataGridViewCellStyle1.ForeColor = Color.White;


                int index1 = this.dataGridViewPerson.Rows.Add();
                this.dataGridViewPerson.Rows[index1].Cells[0].Value = "";
                this.dataGridViewPerson.Rows[index1].Cells[1].Value = "";
                this.dataGridViewPerson.Rows[index1].Cells[2].Value = "同步";
                this.dataGridViewPerson.Rows[index1].Cells[2].Style = dataGridViewCellStyle1;
                this.dataGridViewPerson.Rows[index1].Cells[3].Value = "提交";
                this.dataGridViewPerson.Rows[index1].Cells[3].Style = dataGridViewCellStyle1;
            }
        }

        private void listBoxBuild_DragDrop(object sender, DragEventArgs e)
        {
            listBoxBuild.Items.Clear();
            dragFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            new Thread(new ThreadStart(parseVillage)).Start();
        }

        private void listBoxBuild_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listBoxVillage_DragDrop(object sender, DragEventArgs e)
        {
            dragFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            new Thread(new ThreadStart(parseVillage)).Start();

        }

        private void listBoxVillage_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void dataGridViewPerson_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                switch (dataGridViewPerson[e.ColumnIndex, e.RowIndex].Value.ToString())
                {
                    case "同步":
                        //new Thread(new ThreadStart(() =>
                        //{
                        step = 1;
                        web.CoreWebView2.ExecuteScriptAsync("fun02();");
                        Thread.Sleep(1000);
                        web.CoreWebView2.ExecuteScriptAsync("getTHR();");
                        //})).Start();
                        break;
                    case "提交":
                        if (step < 1)
                        {
                            MessageBox.Show("先进行同步。");
                            break;
                        }
                        //new Thread(new ThreadStart(() =>
                        //{
                        web.CoreWebView2.ExecuteScriptAsync("doSubmits();");
                        //})).Start();
                        break;
                }

            }
            catch (Exception)
            {
            }
        }

        private void dataGridViewPerson_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //MessageBox.Show(HttpHelper.PostHttpByUrl("https://localhost:44342/Home/Ask", "name='wangsc'&age=18"));

            //string json = "{'pid':'" + dataGridViewPerson[2, e.RowIndex].Value + "','name':'" + dataGridViewPerson[1, e.RowIndex].Value + "'}";
            //string uri = "https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/cxthr.jsp";
            //string html = HttpHelper.PostHttpByJson(uri, json);
            //MessageBox.Show(html);

            if (!allowClick) return;
            string mid = (this.listBoxBuild.SelectedItem as Build).mid;
            string hkwtJson = "{\"hkwt_sg\":\"\",\"hkwt_zy\":\"\",\"hkwt_fwcs\":\"\"}";
            string person = "[{ \"xm\":\"苏振红\",\"gmsfhm\":\"372424196809183530\",\"ysbrgx\":\"01\",\"rkbm\":\"\"'},{ \"xm\":\"张国英\",\"gmsfhm\":\"372424196602183541\",\"ysbrgx\":\"\",\"rkbm\":\"\"}]";
            string json = "{ \"ryxx\":\"" + person + "\",\"mid\":\"" + mid + "\",\"wt\":\"" + hkwtJson + "\" }";
            string uri = "https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/jnryxx_save.jsp";
            string html = HttpHelper.PostHttpByJson(uri, json);
            File.WriteAllText("d:\\abc.txt", html);
            MessageBox.Show("访问结束");
        }

        private void dataGridViewPerson_DragDrop(object sender, DragEventArgs e)
        {
            dataGridViewPerson.Rows.Clear();
            dragFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            new Thread(new ThreadStart(parsePerson)).Start();
        }

        private void dataGridViewPerson_DragEnter(object sender, DragEventArgs e)
        {


            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private bool allowClick = false;

        private void web_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            web.CoreWebView2.ExecuteScriptAsync("fun01();");
        }

        //private void aaa()
        //{

        //    SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
        //    //遍历所有选项卡
        //    foreach (SHDocVw.InternetExplorer Browser in shellWindows)
        //    {
        //        if (Browser.LocationURL.Contains("www.baidu.com"))
        //        {
        //            IHTMLDocument2 doc2 = (IHTMLDocument2)Browser.Document;
        //            IHTMLElementCollection inputs = (IHTMLElementCollection)doc2.all.tags("INPUT");
        //            HTMLInputElement input1 = (HTMLInputElement)inputs.item("kw", 0);
        //            input1.value = "刘德华";
        //            IHTMLElement element2 = (IHTMLElement)inputs.item("su", 0);
        //            element2.click();
        //        }
        //    }
        //}

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(() =>
            {
                web.CoreWebView2.ExecuteScriptAsync("doSubmits();");
            })).Start();
        }

        //private void web_LongRunningScriptDetected(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlLongRunningScriptDetectedEventArgs e)
        //{
        //}

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(() =>
            {
                web.CoreWebView2.ExecuteScriptAsync("getTHR();");
            })).Start();
        }

        //private void ccc()
        //{
        //    SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
        //    //遍历所有选项卡
        //    foreach (SHDocVw.InternetExplorer Browser in shellWindows)
        //    {
        //        if (Browser.LocationURL.Contains(url))
        //        {
        //            IHTMLDocument2 doc2 = (IHTMLDocument2)Browser.Document;
        //            IHTMLElementCollection divs = (IHTMLElementCollection)doc2.all.tags("input");
        //            HTMLInputElement aa = (HTMLInputElement)divs.item("pid", 0);
        //            aa.value = "1234567890";
        //            //aa.click();
        //            HTMLInputElement bb = (HTMLInputElement)divs.item("name", 0);
        //            bb.value = "ddd";
        //            //bb.click();
        //        }
        //    }
        //}

        private void http()
        {

            /**
             * 用自己的服务器测试json参数方法能否发送成功
             */
            //string json = "{'name':'wang shi chao'}";
            //string uri = "https://localhost:44342/Home/Json";
            //string html = HttpHelper.PostHttpByJson(uri, json);

            /**
             * 人员申报post
             */
            //人员申报json格式：data: { "ryxx":JSON.stringify(list),"mid":mid,"wt":{ } },
            //rysb json格式：[{ "xm":"王世起","gmsfhm":"371428198412174015","ysbrgx":"01","rkbm":""},{ "xm":"张三","gmsfhm":"123456789123456","ysbrgx":"","rkbm":""}]
            //wt json格式：{ "hkwt_whcd":"20","hkwt_hyzk":"20","hkwt_sg":"","hkwt_zy":"","hkwt_fwcs":""}


            /**
             * 登录山东微警务
             */
            //string json = "{\"loginname\": \"18509513143\", \"password\": \"wjw123321\", \"code\": \"\", \"formerurl\": \"https://msjw.gat.shandong.gov.cn/wechat/html/juzhuzhengnew.html?v=2.0\", \"webtype\": \"\" }";
            ////string json = "loginname=18509513143&assword=wjw123321&code=''&formerurl=https://msjw.gat.shandong.gov.cn/wechat/html/juzhuzhengnew.html?v=2.0&webtype=''";
            //string url = "https://msjw.gat.shandong.gov.cn//wechatservice/wxlogin/login";
            ////string html = HttpHelper.PostHttpByUrl(url, json);
            //string html = HttpHelper.PostHttpByJson(url, json);



            //if (html.Length > 1000)
            //{
            //    File.WriteAllText("d:\\aaa.txt", html);
            //    MessageBox.Show("访问结束，d:\\aaa.txt");
            //}
            //else
            //{
            //    MessageBox.Show(html);
            //}
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CensusContext.DisConnect();
        }
    }
    class MoveFile
    {
        public string oldPath;
        public string newPath;
    }
}

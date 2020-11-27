using CensusManager.helper;
using CensusManager.model;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
                                statusLabel.Text = dragFile + "，" + (currentRowIndex + 1) + "行";
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
                    if (!(numberCollection.Count == midCollection.Count && midCollection.Count == guidCollection.Count && villageGuidCollection.Count == 1 && villageNameCollection.Count == 1))
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

        private void web_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // 网页加载完毕后，执行funInit()
            web.CoreWebView2.ExecuteScriptAsync("funInit();");
            //web.CoreWebView2.ExecuteScriptAsync("funWatcher();");
            web.CoreWebView2.ExecuteScriptAsync("funClearInfo();");
            web.CoreWebView2.ExecuteScriptAsync("doSubmits();");

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
                if (currentPersonList.Count <= 0)
                    return;

                StringBuilder funClearInfo = new StringBuilder();
                funClearInfo.Append("function funClearInfo() {");

                // 删除选择同住人div
                funClearInfo.Append("  $('.xshcyxx').hide(); $('.hjcyList').empty();");

                // 删除添加同住人div和img按钮
                funClearInfo.Append("  $(\"[class='sbrTit bbb vvv']\").remove();");
                funClearInfo.Append("  $(\"[class='addtzrBtn tianjia']\").remove();");

                // 清空所有个人信息
                funClearInfo.Append("  $('#hkwt_whcd').val('');");
                funClearInfo.Append("  $('#hkwt_whcd').attr('code', '');");
                funClearInfo.Append("  $('#hkwt_byzk').val('');");
                funClearInfo.Append("  $('#hkwt_byzk').attr('code', '');");
                funClearInfo.Append("  $('#hkwt_hyzk').val('');");
                funClearInfo.Append("  $('#hkwt_hyzk').attr('code', '');");
                funClearInfo.Append("  $('#hkwt_xx').val('');");
                funClearInfo.Append("  $('#hkwt_xx').attr('code', '');");
                funClearInfo.Append("  $('#hkwt_sg').val('');");
                funClearInfo.Append("  $('#hkwt_zy').val('');");
                funClearInfo.Append("  $('#hkwt_fwcs').val('');");
                funClearInfo.Append("}");

                /**
                 * 打开网页。设置身高999999，。开始watcher。
                 * 当身高!=999999并且step=0，说明第一次load数据完毕。执行{step=1，设置身高999999，并且getTHR()}
                 * 当身高!=999999并且step=1，说明第二次load数据完毕。执行{$('.xshcyxx').hide(); $('.hjcyList').empty(); doSubmits();window.clearInterval(watcherIndex);}
                 */

                StringBuilder funWatcher = new StringBuilder();
                funWatcher.Append("var watcherIndex=0; ");
                funWatcher.Append("var step=0; ");
                funWatcher.Append("var insertCount=0;");
                funWatcher.Append("function funWatcher() {");
                funWatcher.Append("  $('#hkwt_fwcs').val(999999);");
                funWatcher.Append("  watcherIndex = window.setInterval('aaa()',200);");
                funWatcher.Append("}");

                StringBuilder funAAA = new StringBuilder();
                funAAA.Append("function aaa() {");
                funAAA.Append("  if ($('#hkwt_fwcs').val() != 999999){");
                funAAA.Append("      if(step==0){");
                if (checkBox1.Checked)
                {
                    funAAA.Append("         step=1; $('#hkwt_fwcs').val(999999); getTHR();");
                    funAAA.Append("      }else if(step=1){");
                    funAAA.Append("         $('.xshcyxx').hide(); $('.hjcyList').empty();  ");
                    funAAA.Append("         doSubmits();");
                    funAAA.Append("         window.clearInterval(watcherIndex);");
                    //funAAA.Append("         window.postMessage('aaaa');");
                }
                else
                {
                    funAAA.Append("         funClearInfo();");
                    funAAA.Append("         doSubmits();");
                    funAAA.Append("         window.clearInterval(watcherIndex);");
                }
                funAAA.Append("      }");
                funAAA.Append("  }");
                funAAA.Append("}");


                StringBuilder funInit = new StringBuilder();
                funInit.Append("function funInit() {");

                // 切换是否本人
                funInit.Append("$('#mySwitch2').removeClass('mui-active');");
                funInit.Append("$('.mui-switch-handle').attr('style', 'transition-duration: 0.2s; transform: translate(0px, 0px);');");

                // 个人身份证只读去除
                funInit.Append("$('#pid').attr('readonly', false);");
                funInit.Append("$('#name').attr('readonly', false);");




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

                        funInit.Append($"$('#pid').val('{b.id}');");
                        funInit.Append($"$('#name').val('{b.name}');");
                    }
                    else
                    {
                        funInit.Append("$('.tianjia').trigger('tap');");
                        funInit.Append($" $('.addBox').eq({i}).find('.tzrPid').first().val('{b.id}');");
                        funInit.Append($" $('.addBox').eq({i}).find('.tzrName').first().val('{b.name}');");
                        i++;
                    }
                }
                funInit.Append("}");

                web.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(funInit.ToString());
                web.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(funClearInfo.ToString());
                //web.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(funAAA.ToString());
                //web.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(funWatcher.ToString());
                web.CoreWebView2.Navigate(url);

                web.CoreWebView2.Settings.IsWebMessageEnabled = true;
                web.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                //web.CoreWebView2.ScriptDialogOpening += CoreWebView2_ScriptDialogOpening1;
                //web.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;

            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            MessageBox.Show("aaaaa");
            int a = 0;
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
                        //web.CoreWebView2.ExecuteScriptAsync("aaa();");
                        //Thread.Sleep(1000);
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


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CensusContext.DisConnect();
        }

        private void label2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataForm dataForm = new DataForm();
            dataForm.Show();
        }
    }
    class MoveFile
    {
        public string oldPath;
        public string newPath;
    }
}

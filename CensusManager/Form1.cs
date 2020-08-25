using Microsoft.Office.Interop.Excel;
using CensusManager.helper;
using CensusManager.model;
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
        //private string villageDataPath = System.Windows.Forms.Application.StartupPath + @"\data\village.dat";
        //private string buildDataPath = System.Windows.Forms.Application.StartupPath + @"\data\build.dat";
        //private string personDataPath = System.Windows.Forms.Application.StartupPath + @"\data\person.dat";
        private string villageDataPath = "village.dat";
        private string buildDataPath = "build.dat";
        private string personDataPath = "person.dat";


        private delegate void FormControlInvoker();
        private List<Village> allVillageList;
        private List<Build> currentBuildList, allBuildList;
        private List<Person> currentPersonList, allPersonList;
        public Form1()
        {
            InitializeComponent();
            allVillageList = getVillageList();
            allBuildList = getBuildList();
            allPersonList = getPersonList();

            //string a = "https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/zzsb.jsp?data=pVC3T2lZmIc4jlXXaJyILe5mZPxaaeqx9L43ge5lIGEZynQ860PsjuB9O9RTeDRDDd7uGjeiovBVYWXnFq7eP9BbvWenwLF4GtiXgcdDRyjwT42sYSdvrIEumJkdNvmzWajK9tYQDXfa7nbpvKwtQ11FBaTHxyEVYr1c1mefBapa9FyqcyMhKVGfcbYd%2BL5v58KJHHTREyHElKceWNCgQUd7IhmaglLkaiTu2Awqjq0%3D";
            //Process.Start(a);
        }


        string[] dragFiles;
        /**
         * 解析出村庄GUID
         */
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

        private void parsePerson()
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
                        if (allPersonList.FirstOrDefault(model => model.id.Equals(id)) == null)
                        {
                            newCount++;
                            allPersonList.Add(new Person(ralation, name, id, "汉族", address));
                        }
                        else
                        {
                            redo++;
                        }
                    }
                    savePerson(allPersonList);


                });
            }
            MessageBox.Show("导入新数据 " + newCount + " 条，重复数据 " + redo + " 条，不合格数据 " + illegal + " 条");
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
                        MessageBox.Show("【" + fileName + "】的房屋数据不平衡，请联系管理员。GUID："+guidCollection.Count+"，MID："+midCollection.Count+"，NUMBER："+numberCollection.Count);
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

        private void saveVillage(List<Village> data)
        {
            FileStream saveFile = null;
            try
            {
                IFormatter serializer = new BinaryFormatter();
                saveFile = new FileStream(villageDataPath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(saveFile, data);
                saveFile.Close();
            }
            catch (Exception)
            {

            }
            finally
            {
                if (saveFile != null)
                    saveFile.Close();
            }
        }
        private void savePerson(List<Person> data)
        {
            FileStream saveFile = null;
            try
            {
                IFormatter serializer = new BinaryFormatter();
                saveFile = new FileStream(personDataPath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(saveFile, data);
                saveFile.Close();
            }
            catch (Exception)
            {

            }
            finally
            {
                if (saveFile != null)
                    saveFile.Close();
            }
        }

        private void saveBuild(List<Build> data)
        {
            FileStream saveFile = null;
            try
            {
                IFormatter serializer = new BinaryFormatter();
                saveFile = new FileStream(buildDataPath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(saveFile, data);
                saveFile.Close();
            }
            catch (Exception)
            {

            }
            finally
            {
                if (saveFile != null)
                    saveFile.Close();
            }
        }

        private void listBoxBuild_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var currentBuild = this.listBoxBuild.SelectedItem as Build;
            //var currentVillage = this.listBoxVillage.SelectedItem as Village;
            //this.listViewPerson.Items.Clear();
            System.Diagnostics.Process.Start("https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/rysb.jsp?guid=" + currentBuild.guid);

            //if (currentBuild != null)
            //{
            //    var allPersonList = getPersonList();
            //    currentPersonList = allPersonList.Where(model => model.address.Equals(currentVillage.name + currentBuild.number)).ToList();
            //    foreach (var b in currentPersonList)
            //    {
            //        this.listViewPerson.Items.Add(b.race);
            //        this.listViewPerson.Items.Add(b.name);
            //        this.listViewPerson.Items.Add(b.id);
            //        this.listViewPerson.Items.Add(b.address);
            //    }
            //}
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (var v in allVillageList)
            {
                this.listBoxVillage.Items.Add(v);
            }
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
                currentBuildList = allBuildList.Where(model => model.villageGuid.Equals(currentVillage.guid)).ToList();
                sw.Stop();
                var span = sw.ElapsedMilliseconds;

                foreach (var b in currentBuildList)
                {
                    this.listBoxBuild.Items.Add(b);
                }
            }
        }
        private List<Village> getVillageList()
        {
            FileStream loadFile = null;
            try
            {
                IFormatter serializer = new BinaryFormatter();
                loadFile = new FileStream(villageDataPath, FileMode.Open, FileAccess.Read);
                var villageList = serializer.Deserialize(loadFile) as List<Village>;
                return villageList;
            }
            catch (Exception)
            {
                return new List<Village>();
            }
            finally
            {
                if (loadFile != null)
                    loadFile.Close();
            }
        }

        private List<Build> getBuildList()
        {
            FileStream loadFile = null;
            try
            {
                IFormatter serializer = new BinaryFormatter();
                loadFile = new FileStream(buildDataPath, FileMode.Open, FileAccess.Read);
                var buildList = serializer.Deserialize(loadFile) as List<Build>;
                return buildList;
            }
            catch (Exception)
            {
                return new List<Build>();
            }
            finally
            {
                if (loadFile != null)
                    loadFile.Close();
            }
        }

        private List<Person> getPersonList()
        {
            FileStream loadFile = null;
            try
            {
                IFormatter serializer = new BinaryFormatter();
                loadFile = new FileStream(personDataPath, FileMode.Open, FileAccess.Read);
                var personList = serializer.Deserialize(loadFile) as List<Person>;
                return personList;
            }
            catch (Exception)
            {
                return new List<Person>();
            }
            finally
            {
                if (loadFile != null)
                    loadFile.Close();
            }
        }

        private void listBoxBuild_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentBuild = this.listBoxBuild.SelectedItem as Build;
            var currentVillage = this.listBoxVillage.SelectedItem as Village;
            this.dataGridViewPerson.Rows.Clear();

            if (currentBuild != null)
            {
                string address = currentVillage.name + currentBuild.number;
                //string a = "鲁权屯镇闫庄村 001号";
                //string b = "鲁权屯镇闫庄村001号";
                currentPersonList = allPersonList.Where(model => model.address.Equals("鲁权屯镇闫庄村001号")).ToList();


                //if(currentPersonList!=null)
                //currentPersonList.Clear();
                //foreach(var p in allPersonList)
                //{
                //    if (p.address.Contains("鲁权屯镇闫庄村001号"))
                //    {
                //        currentPersonList.Add(p);
                //    }
                //}
                foreach (var b in currentPersonList)
                {
                    int index = this.dataGridViewPerson.Rows.Add();
                    this.dataGridViewPerson.Rows[index].Cells[0].Value = b.relation;
                    this.dataGridViewPerson.Rows[index].Cells[1].Value = b.name;
                    this.dataGridViewPerson.Rows[index].Cells[2].Value = b.id;
                    this.dataGridViewPerson.Rows[index].Cells[3].Value = b.address;
                    if (b.relation.Equals("户主"))
                    {
                        DataGridViewCellStyle cs = new DataGridViewCellStyle();
                        cs.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                        this.dataGridViewPerson.Rows[index].Cells[0].Style = cs;
                        this.dataGridViewPerson.Rows[index].Cells[1].Style = cs;
                        this.dataGridViewPerson.Rows[index].Cells[2].Style = cs;
                        this.dataGridViewPerson.Rows[index].Cells[3].Style = cs;
                    }
                }
            }
        }

        private void listBoxBuild_DragDrop(object sender, DragEventArgs e)
        {
            listBoxBuild.Items.Clear();
            dragFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            new Thread(new ThreadStart(parseBuild)).Start();
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
            var cell = this.dataGridViewPerson.SelectedCells;
            string value = cell[0].Value.ToString();
            Clipboard.SetText(value);

            DataGridViewCellStyle cs1 = dataGridViewPerson[e.ColumnIndex, e.RowIndex].Style;
            DataGridViewCellStyle cs = new DataGridViewCellStyle();
            cs.Font = cs1.Font;
            cs.ForeColor = Color.FromArgb(192, 0, 0);
            dataGridViewPerson[e.ColumnIndex, e.RowIndex].Style = cs;
        }

        private void dataGridViewPerson_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //MessageBox.Show(HttpHelper.PostHttpByUrl("https://localhost:44342/Home/Ask", "name='wangsc'&age=18"));

            //string json = "{'pid':'" + dataGridViewPerson[2, e.RowIndex].Value + "','name':'" + dataGridViewPerson[1, e.RowIndex].Value + "'}";
            //string uri = "https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/cxthr.jsp";
            //string html = HttpHelper.PostHttpByJson(uri, json);
            //MessageBox.Show(html);


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

        private void listBoxVillage_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var currentVillage = this.listBoxVillage.SelectedItem as Village;
            Process.Start("https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/rysb.jsp?guid=" + currentVillage.guid);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            http();
            //MessageBox.Show(HttpHelper.PostHttpByJson("https://localhost:44342/Home/Json", "{'Name':'wangsc','Age':'18'}"));
            //MessageBox.Show(HttpHelper.PostHttpByJson("https://localhost:44342/Users/Create", "{'Name':'wangsc','Age':'18'}"));


            /**
             * ajax获取户籍信息
             */
            //string json = "{'pid':371428198412174015,'name':王世起}";
            //string uri = "https://msjw.gat.shandong.gov.cn/zayw/hkzd/stbb/cxthr.jsp";
            //string html = HttpHelper.PostHttpByJson(uri, json);

        }
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
    }
}

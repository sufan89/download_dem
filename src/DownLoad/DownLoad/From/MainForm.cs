using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace DownLoad
{
    public enum EnDownLoadState
    {
        UnDownLoaded = 0,
        DownLoaded =1,
        Error=2
    }
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        WebHttp m_mainWeb = new WebHttp();
        CookieCollection LoginCookie;
        //DataTable m_MainTable;
        private const string m_DownLoadLinkeName = "progress";
        private const string m_DownLoadState = "downloadstate";
        private const string m_ColumnUri = "uri";
        private void btnGetDataList_Click(object sender, EventArgs e)
        {
            InitialDataGridView();
        }
        /// <summary>
        /// 获取数据列表
        /// </summary>
        private void InitialDataGridView()
        {
            if (LoginCookie == null)
            {
                MessageBox.Show("请先登录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int totalSize = 22603;
                int TotalPage = 377;
                int pageSize = 60;
                int offset = 0;
                while (offset < totalSize)
                {
                    string getParam= string.Format("tableInfo=%7B%22offset%22%3A{0}%2C%22pageSize%22%3A{1}%2C%22totalPage%22%3A{2}%2C%22totalSize%22%3A{3}%2C%22sortSet%22%3A%5B%7B%22id%22%3A%22dataid%22%2C%22sort%22%3A%22asc%22%7D%5D%2C%22filterSet%22%3A%5B%5D%7D&data=%7B%22datatype%22%3A%22gdem_utm2%22%7D&datatype=gdem_utm2"
                        , offset, pageSize, TotalPage, totalSize);
                    string result = m_mainWeb.HttpGet("http://www.gscloud.cn/sources/query_dataset/421", getParam);
                    if (string.IsNullOrEmpty(result))
                    {
                        MessageBox.Show("获取列表出错！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    else
                    {
                        object t = JsonConvert.DeserializeObject(result);
                        JObject jsonobject = JObject.FromObject(t);
                        JToken jsonToken = jsonobject.GetValue("data");
                        List<JToken> listObject = jsonToken.ToList();
                        for (int i = 0; i < listObject.Count; i++)
                        {
                           
                            JToken jData = listObject[i];
                            JEnumerable<JToken> jChildren = jData.Children();
                            List<JToken> listChildren = jChildren.ToList<JToken>();
                            //初始化列
                            if (dgvAllData.Columns.Count == 0)
                            {
                                InitalDataGridColumn(listChildren);
                            }
                            Dictionary<string, string> tempList = new Dictionary<string, string>();
                            for (int j = 0; j < listChildren.Count(); j++)
                            {
                                JToken tempToken = listChildren[j];
                                JProperty tempObject = (JProperty)tempToken;
                                tempList.Add(tempObject.Name,tempObject.Value.ToString());
                            }
                            dgvAllData.Rows.Add(tempList.Values.ToArray());
                            m_mainWeb.m_DownLoadList.Add(tempList["dataid"].ToString());
                        }
                    }
                    offset += 60;
                }
                dgvAllData.Refresh();
            }
        }
        private void InitalDataGridColumn(List<JToken> listToken)
        {
            if (dgvAllData.Columns.Count==0)
            {
                for (int i = 0; i < listToken.Count; i++)
                {
                    JProperty jPorperty = (JProperty)listToken[i];
                    dgvAllData.Columns.Add(new DataGridViewTextBoxColumn()
                    {
                        DataPropertyName= jPorperty.Name,
                        HeaderText=jPorperty.Name,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                        Name= jPorperty.Name
                    });
                }
                dgvAllData.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = m_DownLoadLinkeName,
                    HeaderText = m_DownLoadLinkeName,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    Name = m_DownLoadLinkeName
                });
                dgvAllData.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = m_DownLoadState,
                    HeaderText = m_DownLoadState,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    Name = m_DownLoadState,
                    ValueType = Type.GetType("System.Int32")
            });
                dgvAllData.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = m_ColumnUri,
                    HeaderText = m_ColumnUri,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    Name = m_ColumnUri
                });
            }
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = txtUserName.Text;
            string passWord = txtPassword.Text;
            CookieCollection firstCookie = new CookieCollection();
            string tempStr = m_mainWeb.HttpGet("http://www.gscloud.cn", "");
            string url = "http://www.gscloud.cn//accounts/validate";
            LoginCookie = m_mainWeb.LoginWeb(url, userName, passWord, firstCookie);
            if (LoginCookie != null)
            {
                MessageBox.Show("登录成功！", "提示");
            }
            else
            {
                MessageBox.Show("登录失败！", "提示");
            }
        }
        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartDownload_Click(object sender, EventArgs e)
        {
            ThreadPool.SetMaxThreads(5, 5);
            if (dgvAllData.Rows.Count <= 0)
            {
                MessageBox.Show("请先获取下载列表！","提示",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(txtSaveFilePath.Text))
            {
                MessageBox.Show("请设置数据存放目录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if ( dgvAllData.Rows.Count > 0)
            {
                for (int i = 0; i < dgvAllData.Rows.Count; i++)
                {
                    DataGridViewRow dataRow = dgvAllData.Rows[i];
                    if (dgvAllData.Columns.Contains("dataid"))
                    {
                        string dataId = dataRow.Cells["dataid"].Value.ToString();
                        string strUrl = string.Format("http://www.gscloud.cn/sources/download/421/{0}/bj", dataId);
                        string result= m_mainWeb.HttpGetDownLoad(strUrl, string.Format("{0}\\{1}.zip", txtSaveFilePath.Text,dataId), dataRow, LoginCookie);
                    }
                }
            }
        }

        private void btnSelectFilePath_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog selectFolder = new FolderBrowserDialog();
            if (selectFolder.ShowDialog() == DialogResult.OK)
            {
                txtSaveFilePath.Text = selectFolder.SelectedPath;
            }
            string saveFolder = txtSaveFilePath.Text;
            //判断选择的文件夹是否为空文件夹
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(saveFolder);
            if (!System.IO.Directory.Exists(saveFolder))
            {
                if (MessageBox.Show("选择的文件夹不存在，是否新建?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
                {
                    System.IO.Directory.CreateDirectory(saveFolder);
                }
            }
            if (dirInfo.GetFiles().Length > 0 || dirInfo.GetDirectories().Length > 0)
            {
                MessageBox.Show("请选择一个空的文件夹!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSaveFilePath.Text = string.Empty;
                return;
            }
        }
        /// <summary>
        /// 将下载类别导出成Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportDataList_Click(object sender, EventArgs e)
        {
            if (m_mainWeb.m_DownLoadList.Count > 0)
            {
                MessageBox.Show("当前还有数据未下载，请全部下载完成后再进行导出!", "提示");
                return;
            }
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "*.xls|*xls";
            if (saveFile.ShowDialog() != DialogResult.OK) return;
            string fileName = saveFile.FileName;
            ExcelOperate ExportExcel = new ExcelOperate();
            if (ExportExcel.DataSetToExcel(dgvAllData, fileName, false))
            {
                MessageBox.Show("导出成功","提示");
            }
        }
        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            LoginCookie = null;
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            LoginCookie = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "*.xls|*xls";
            if (saveFile.ShowDialog() != DialogResult.OK) return;
            string fileName = saveFile.FileName;
            ExcelOperate ExportExcel = new ExcelOperate();
            if (ExportExcel.DataSetToExcel(dgvAllData, fileName, false))
            {
                MessageBox.Show("导出成功", "提示");
            }
        }
    }
}

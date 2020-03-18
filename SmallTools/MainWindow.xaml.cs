using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Spire.Pdf;
using Spire.License;
using System.IO;
using System.ComponentModel;

namespace SmallTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// pdf文件路径（含文件名）
        /// </summary>
        private static string filePath = string.Empty;
        /// <summary>
        /// 文件名
        /// </summary>
        private static string name = string.Empty;
        /// <summary>
        /// 后台运行对象
        /// </summary>
        BackgroundWorker bgMeet;


        public MainWindow()
        {
            InitializeComponent();
        }

        #region 首先得启用AllowDrop，再定义事件
        /// <summary>
        ///  首先得启用AllowDrop，再定义事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void window_dragEnter(object sender, DragEventArgs e)
        {
            filePath = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            if (!string.IsNullOrWhiteSpace(filePath))
                this.lab_pro.Content = "文件已加载";
        }
        #endregion


        #region 打开文件
        private void btn_openfile(object sender, RoutedEventArgs e)
        {
            filePath = OpenFileDialog("PDF文件|*.pdf|所有文件|*.*");
            if (!string.IsNullOrWhiteSpace(filePath))
                this.lab_pro.Content = "文件已加载";
            name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            Console.WriteLine("filePath是：" + filePath);
            Console.WriteLine("name是：" + name);
        }
        #endregion

        #region 转换
        private void btn_save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("请选择要转换的文件");
                return;
            }
            bgMeet = new BackgroundWorker();
            //能否报告进度更新
            bgMeet.WorkerReportsProgress = true;
            //要执行的后台任务
            bgMeet.DoWork += new DoWorkEventHandler(bgMeet_DoWork);
            //进度报告方法
            bgMeet.ProgressChanged += new ProgressChangedEventHandler(bgMeet_ProgressChanged);
            //后台任务执行完成时调用的方法
            bgMeet.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgMeet_RunWorkerCompleted);
            bgMeet.RunWorkerAsync(); //任务启动
        }
        #endregion

        #region 打开文件对话框
        /// <summary>
        /// 打开文件对话框
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <returns>返回文件路径</returns>
        public static string OpenFileDialog(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = filter; //"MP4文件|*.mp4|flv文件|*.flv|所有文件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "mp4";   //设置默认扩展名
            if (openFileDialog.ShowDialog() == false)
                return "";
            return openFileDialog.FileName;
        }
        #endregion

        #region 保存文件对话框
        /// <summary>
        /// 保存文件对话框
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <returns>返回文件路径</returns>
        public static string SaveFileDialog(string filter)
        {

            //创建一个保存文件式的对话框
            SaveFileDialog sfd = new SaveFileDialog();
            //设置这个对话框的起始保存路径
            sfd.InitialDirectory = @"D:\";
            //设置保存的文件的类型，注意过滤器的语法
            sfd.Filter = filter;  // "PNG图片|*.png|JPG图片|*.jpg";
            //调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮
            if (sfd.ShowDialog() == true)
                return sfd.FileName;
            else
                return "";
        }
        #endregion

        #region 执行任务
        void bgMeet_DoWork(object sender, DoWorkEventArgs e)
        {
            //开始播放等待动画
            this.Dispatcher.Invoke(new Action(() =>
            {
                loading.Visibility = System.Windows.Visibility.Visible;
            }));
            //开始后台任务
            Do();
        }
        #endregion

        #region 报告任务进度
        void bgMeet_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.lab_pro.Content = e.ProgressPercentage + "%";
            }));
        }
        //任务执行完成后更新状态
        void bgMeet_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loading.Visibility = System.Windows.Visibility.Collapsed;
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.lab_pro.Content = "转换完成!";
            }));
        }
        #endregion

        #region 将Pdf转化为word
        /// <summary>
        /// 将Pdf转化为word  耗时任务 
        /// </summary>
        private void Do()
        {
            string savePath = System.IO.Path.GetDirectoryName(filePath);
            //得到最终文件的保存位置
            string fullPath = fullPath = savePath + "\\" + name + ".doc";
            if (File.Exists(fullPath))
                fullPath = savePath + "\\" + name + "-doc" + ".doc";
                
            try
            {
                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(filePath);//pdf物理路径
                doc.SaveToFile(fullPath, FileFormat.DOC);  //生成word的物理路径
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error Message:" + ex.Message);
            }
        }
        #endregion 
    }
}

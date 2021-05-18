using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace WPFMapApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<MyPoint> MyPoints
        {
            get;
            set;
        }

        public MainWindow()
        {
            InitializeComponent();
            //MapPoint mapCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);
            //MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, 100000));
            //MainMapView.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory+"map.html");

            Uri uri = new Uri(@"pack://application:,,,/map.html");
            MainMapView.NavigateToStream(Application.GetResourceStream(uri).Stream);

            
        }

        private void MainMapView_Navigated(object sender, NavigationEventArgs e)
        {
            SuppressScriptErrors((WebBrowser)sender, true);
        }

        public void SuppressScriptErrors(WebBrowser wb, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;

            object objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });
        }

        public void SetMapCenter(float x, float y)
        {
            MainMapView.InvokeScript("setCenter",new object[] { x,y});
        }

        public void DrawLine(List<MyPoint> points)
        {
            if (points == null || points.Count <= 0)
            {
                RemoveLine();
                return;
            }
            try
            {
                //List<float> langandlats = new List<float>();
                //foreach (var p in points)
                //{
                //    langandlats.AddRange(new float[] { p.X, p.Y });
                //}
                MainMapView.InvokeScript("drawLine",JsonConvert.SerializeObject(points));
            }
            catch(Exception ex)
            {
                MessageHelper.ShowError("绘制轨迹发生错误:"+ex.Message);
            }
            
        }

        public void RemoveLine()
        {
            try
            {
                MainMapView.InvokeScript("removeLine", null);
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("清除轨迹发生错误:" + ex.Message);
            }

        }

        private void SetCenter_Click(object sender, RoutedEventArgs e)
        {
            try {
                if (string.IsNullOrEmpty(tbCenter.Text.Trim()))
                {
                    MessageHelper.ShowError("请输入中点 格式为 经度,纬度  经度-180~180，纬度 -90~90");
                    tbCenter.Focus();
                    return;
                }
                else
                {

                    string[] arr = tbCenter.Text.Trim().Split(',');
                    if (arr != null && arr.Length == 2)
                    {
                        try
                        {
                            float x = Convert.ToSingle(arr[0]);
                            float y = Convert.ToSingle(arr[1]);
                            if (x > -180 && x < 180 && y > -90 && y < 90)
                            {
                                SetMapCenter(x, y);
                            }
                            else
                            {
                                MessageHelper.ShowError("中点格式不正确 格式为 经度,纬度  经度-180~180，纬度 -90~90");
                                tbCenter.Focus();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageHelper.ShowError("中点格式不正确 格式为 经度,纬度  经度-180~180，纬度 -90~90");
                            tbCenter.Focus();
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("发生异常:" + ex.Message);
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var list = new List<MyPoint>() {
                new MyPoint(){ Index=0,X=102.34f,Y=31.32f },
                new MyPoint(){ Index=1,X=103.34f,Y=33.32f},
                new MyPoint(){ Index=2,X=105.34f,Y=32.32f},
                new MyPoint(){ Index=3,X=108.34f,Y=34.32f},
            };
            List<float> langandlats = new  List<float>();
            foreach (var p in list)
            {
                langandlats.AddRange(new float[]{ p.X, p.Y});
            }
            DrawLine(new List<MyPoint>() { 
                new MyPoint(){ Index=0,X=102.34f,Y=31.32f },
                new MyPoint(){ Index=1,X=103.34f,Y=33.32f},
                new MyPoint(){ Index=2,X=105.34f,Y=32.32f},
                new MyPoint(){ Index=3,X=108.34f,Y=34.32f},
            });
        }

        private void btClearLine_Click(object sender, RoutedEventArgs e)
        {
            RemoveLine();
        }

        private void btManagerLine_Click(object sender, RoutedEventArgs e)
        {
            ManagerPoints managerWindow = new ManagerPoints(MyPoints);
            if (managerWindow.ShowDialog()==true)
            {
                MyPoints = managerWindow.MyPoints;
                if (MyPoints.Count >= 2)
                {
                    DrawLine(MyPoints);
                }
            }
            //DrawLine(new List<MyPoint>() {
            //    new MyPoint(){ Index=0,X=102.34f,Y=31.32f },
            //    new MyPoint(){ Index=1,X=103.34f,Y=33.32f},
            //    new MyPoint(){ Index=2,X=105.34f,Y=32.32f},
            //    new MyPoint(){ Index=3,X=108.34f,Y=34.32f},
            //});
        }

        private void btExportPanel_Click(object sender, RoutedEventArgs e)
        {
            if (MyPoints == null || MyPoints.Count <= 0)
            {
                MessageHelper.ShowError("当前轨迹数据为空,无法导出！");
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = "MyPoints"; // Default file name
                sfd.DefaultExt = ".text"; // Default file extension
                sfd.Filter = "Text documents (.txt)|*.txt"; // Filter files by extensioni
                if (sfd.ShowDialog() == true)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var p in MyPoints)
                    {
                        sb.AppendLine($"{p.Index},{p.X},{p.Y}");
                    }
                    File.WriteAllText(sfd.FileName,sfd.ToString());
                }            }
        }

        private void btImportPanel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "MyPoints"; // Default file name
            ofd.DefaultExt = ".text"; // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt"; // Filter files by extensioni
            if (ofd.ShowDialog() == true)
            {
                string[] allLines= File.ReadAllLines(ofd.FileName);
                if (allLines == null || allLines.Length <= 0)
                {
                    MessageHelper.ShowError("导入失败,可能文件为空！");
                }
                else
                {
                    MyPoints = new List<MyPoint>();
                    foreach (string s in allLines)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            string[] temparr = s.Split(',');
                            if (temparr != null && temparr.Length == 3)
                            {
                                try
                                {
                                    MyPoint p = new MyPoint();
                                    p.Index = Convert.ToInt32(temparr[0]);
                                    p.X = Convert.ToSingle(temparr[1]);
                                    p.Y = Convert.ToSingle(temparr[2]);
                                    MyPoints.Add(p);
                                }
                                catch
                                { }
                            }
                        }
                    }
                    if (MyPoints.Count <= 2)
                    {
                        MessageHelper.ShowError("导入数据不正确，导入0条记录");
                    }
                    else
                    {
                        //绘制曲线
                        DrawLine(MyPoints);
                    }
                }
            }
        }
    }
}

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
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
using System.Windows.Shapes;

namespace WPFMapApp
{
    /// <summary>
    /// ManagerPoints.xaml 的交互逻辑
    /// </summary>
    public partial class ManagerPoints : Window
    {
        MainViewModel viewModel = null;
        public List<MyPoint> MyPoints
        {
            get;
            set;
        }
        public ManagerPoints(List<MyPoint> points)
        {
            InitializeComponent();
            MyPoints = points;
            viewModel = new MainViewModel(MyPoints);
            this.DataContext = viewModel;
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            foreach(var item in viewModel.Items)
            {
                if(item.X>180||item.X<-180)
                {
                    MessageHelper.ShowError("经度范围为 -180~180 请检查！");
                    return;
                }

                if (item.Y > 90 || item.Y < -90)
                {
                    MessageHelper.ShowError("纬度范围为 -90~90, 请检查！");
                    return;
                }
            }

            this.MyPoints = viewModel.Items.ToList();
            this.DialogResult = true;
        }
    }

    public class MessageToken
    {
        /// <summary>
        /// 设置DataGrid消息
        /// </summary>
        public static readonly string SetDataGrid = nameof(SetDataGrid);
    }

    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.  DataGrid
        /// </summary>
        public MainViewModel(List<MyPoint> points)
        {
            //注册设置vm里的DataGrid与界面的相关联
            Messenger.Default.Register<DataGrid>(this, MessageToken.SetDataGrid, (x) =>
            {
                DDataGrid = x;

                for (int i = 0; i < 5; i++)
                {
                    var item = new MyPoint();
                    item.X = 0;
                    item.Y = 0; ;
                    _Items.Add(item);
                }

                DDataGrid.Columns.Add(new DataGridTextColumn() { Header = "A", Binding = new Binding("A") });
                DDataGrid.Columns.Add(new DataGridTextColumn() { Header = "B", Binding = new Binding("B") });
                DDataGrid.ItemsSource = _Items;
            });

            if (points != null && points.Count > 0)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Items.Add(points[i]);
                }
            }

            //ClearDataCmd = new RelayCommand(new Action<MyPoint>((p) => {
            //    Items.Clear();
            //}));

            //AddDataCmd = new RelayCommand(new Action<MyPoint>((p) => {
            //    var item = new MyPoint();
            //    item.X = 0;
            //    item.Y = 0;
            //    Items.Add(item);
            //}));

            //InsertAtCmd = new RelayCommand(new Action<MyPoint>((p) => {
            //    int index = Items.IndexOf(SelectedItem);
            //    var item = new MyPoint();
            //    item.X = 0;
            //    item.Y = 0;
            //    if (index <= Items.Count - 1)
            //    {
            //        Items.Insert(index, item);
            //    }
            //    else
            //    {
            //        Items.Add(item);
            //    }
            //}));
        }
        /// <summary>
        /// 绑定的数据
        /// </summary>
        ObservableCollection<MyPoint> _Items = new ObservableCollection<MyPoint>();

        public DataGrid DDataGrid;

        public ObservableCollection<MyPoint> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                RaisePropertyChanged(() => Items);
            }
        }

        public MyPoint SelectedItem { get; set; }

        public RelayCommand AddDataCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(AddData)).Value;

        public RelayCommand RemoveCmd => new Lazy<RelayCommand>(() =>
             new RelayCommand(RemoveAt)).Value;

        public RelayCommand InsertAtCmd => new Lazy<RelayCommand>(() =>
              new RelayCommand(InsertAt)).Value;

        public RelayCommand ClearDataCmd => new Lazy<RelayCommand>(() =>
              new RelayCommand(ClearData)).Value;


        private void AddData()
        {
            var item = new MyPoint();
            item.X = 0;
            item.Y = 0;
            Items.Add(item);
        }

        private void InsertAt()
        {
            var item = new MyPoint();
            item.X = 0;
            item.Y = 0;
            if (SelectedItem == null)
                return;
            int index = Items.IndexOf(SelectedItem);
            if (index <= Items.Count - 1)
            {
                Items.Insert(index, item);
            }
            else
            {
                Items.Add(item);
            }
        }

        public void RemoveAt()
        {
            if (SelectedItem == null)
                return;
            int index = Items.IndexOf(SelectedItem);
            if (index <= Items.Count - 1)
            {
                Items.RemoveAt(index);
            }
        }

        private void ClearData()
        {
            Items.Clear();
            SelectedItem = null;
        }

    }
}

using GalaSoft.MvvmLight;
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
        public List<MyPoint> MyPoints
        {
            get;
            set;
        }
        public ManagerPoints(List<MyPoint> points)
        {
            InitializeComponent();
            MyPoints = points;
            this.DataContext = new MainViewModel();
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btInsert_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {

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
        public MainViewModel()
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

            RemoveCmd = new RelayCommand(new Action<MyPoint>((p) => {
                int index = Items.IndexOf(p);
                if (index <= Items.Count - 1)
                {
                    Items.RemoveAt(index);
                }
            }));

            ClearDataCmd = new RelayCommand(new Action<MyPoint>((p) => {
                Items.Clear();
            }));

            AddDataCmd = new RelayCommand(new Action<MyPoint>((p) => {
                var item = new MyPoint();
                item.X = 0;
                item.Y = 0;
                Items.Add(item);
            }));

            InsertAtCmd = new RelayCommand(new Action<MyPoint>((p) => {
                int index = Items.IndexOf(p);
                var item = new MyPoint();
                item.X = 0;
                item.Y = 0;
                if (index <= Items.Count - 1)
                {
                    Items.Insert(index, item);
                }
                else
                {
                    Items.Add(item);
                }
            }));
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
        
        public RelayCommand AddDataCmd {get;set;}

        public RelayCommand RemoveCmd {
            get;
            set;
        }

        public RelayCommand InsertAtCmd
        {
            get;
            set;
        }

        public RelayCommand ClearDataCmd
        {
            get;
            set;
        }


        private void AddData()
        {
            var item = new MyPoint();
            item.X = 0;
            item.Y = 0;
            Items.Add(item);
        }

        private void InsertAt(int index)
        {
            var item = new MyPoint();
            item.X = 0;
            item.Y = 0;
            if (index <= Items.Count - 1)
            {
                Items.Insert(index, item);
            }
            else
            {
                Items.Add(item);
            }
        }

        public void RemoveAt(MyPoint p)
        {
            int index = Items.IndexOf(p);
            if (index <= Items.Count - 1)
            {
                Items.RemoveAt(index);
            }
        }

        private void ClearData()
        {
            Items.Clear();
        }

    }
}

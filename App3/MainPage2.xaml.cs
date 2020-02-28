using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace App3
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage2 : Page
    {
        int year = DateTime.Now.Year;
        int shtm = DateTime.Now.Month < 5 ? 1 : 2;
        int line = 2;

        List<List<Border>> borderList = new List<List<Border>>();
        List<List<string>> textList = new List<List<string>>();

        Payload payload;
        public MainPage2()
        {
            this.InitializeComponent();
            textBlock_Year.Text = year.ToString() + "년 " + shtm.ToString() + "학기";
            CreateLine(1);
            borderList[0][0].Visibility = Visibility.Visible;
            borderList[0][1].Visibility = Visibility.Visible;

            for (int i=2; i<=10; i++)
            {
                CreateLine(i);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (line < 11)
            {
                borderList[line - 1][0].Visibility = Visibility.Visible;
                borderList[line - 1][1].Visibility = Visibility.Visible;
                line++;
            }
            
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb;
            for (int i=1; i<line; i++)
            {
                textList.Add(new List<string>());
                tb = (TextBox)borderList[line - 1][0].Child;
                textList[i - 1].Add(tb.Text);
                tb = (TextBox)borderList[line - 1][1].Child;
                textList[i - 1].Add(tb.Text);
            }

            Payload payload2 = new Payload();
            payload2.ans1 = payload.ans1;
            payload2.ans2 = payload.ans2;
            payload2.ans3 = year.ToString();
            payload2.ans4 = shtm.ToString();
            payload2.ans5 = textList;
            this.Frame.Navigate(typeof(MainPage3), payload2);
        }

        public Border CreateBorder(int row, int col)
        {
            Border bd = new Border();
            bd.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(bd, row);
            Grid.SetColumn(bd, col);
            TextBox tbox = new TextBox();
            tbox.Margin = new Thickness(col==0 ? 40:20, 0, 0, 0);
            tbox.PlaceholderText = col == 0 ? "010001" : "01";
            bd.Child = tbox;
            bd.Visibility = Visibility.Collapsed;

            return bd;
        }

        private void CreateLine(int index)
        {
            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(40.0);
            sugangList.RowDefinitions.Add(rd);

            borderList.Add(new List<Border>());
            borderList[index-1].Add(CreateBorder(index, 0));
            borderList[index-1].Add(CreateBorder(index, 1));
            sugangList.Children.Add(borderList[index-1][0]);
            sugangList.Children.Add(borderList[index-1][1]);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            payload = (Payload)e.Parameter;
        }
    }
}

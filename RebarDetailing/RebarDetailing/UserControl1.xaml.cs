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
using Autodesk.Revit.DB;

namespace RebarDetailing
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public double len { get; set; }
        public bool haveTag { get; set; }
        public List<Element> rebarTagFamilies { get; set; }
        public List<Element> rebarTagSymbols { get; set; }
        public Element selectedRebarTagType { get; set; }
        public bool isHaveRebarTag { get; set; }
        public bool isFirstSet { get; set; }
        public bool isMultiPick { get; set; }
        public bool isShowAgain { get; set; }
        public bool isHaveSegment { get; set; }
        public Window Window { get; set; }
        public UserControl1(List<Element> rbtFs, List<Element> rbts, Element defaultSymbol, Window window)
        {
            this.isFirstSet = true;
            InitializeComponent();
            txtLen.Text = Settings1.Default.Offset.ToString();
            chkHaveTag.IsChecked = Settings1.Default.HaveTag;
            chkMultiPick.IsChecked = Settings1.Default.MultiPick;
            chkShowAgain.IsChecked = Settings1.Default.ShowAgain;
            chkHaveSegment.IsChecked = Settings1.Default.HaveSegment;
            this.len = double.Parse(txtLen.Text);
            this.haveTag = chkHaveTag.IsChecked.HasValue ? chkHaveTag.IsChecked.Value : false;
            this.isMultiPick = chkMultiPick.IsChecked.HasValue ? chkMultiPick.IsChecked.Value : false;
            this.isShowAgain = chkShowAgain.IsChecked.HasValue ? chkShowAgain.IsChecked.Value : false;
            this.isHaveSegment = chkHaveSegment.IsChecked.HasValue ? chkHaveSegment.IsChecked.Value : false;
            this.rebarTagFamilies = rbtFs;
            this.rebarTagSymbols = rbts;
            this.Window = window;
            FamilySymbol fs = defaultSymbol != null ? defaultSymbol as FamilySymbol : null;
            if (rebarTagFamilies.Count != 0)
            {
                isHaveRebarTag = true;
                int index = 0;
                for (int i = 0; i < rebarTagFamilies.Count; i++)
                {
                    if (fs != null)
                    {
                        if (fs.FamilyName == rebarTagFamilies[i].Name)
                        {
                            index = i;
                        }
                    }
                    cbbRbf.Items.Add(rebarTagFamilies[i]);
                }
                cbbRbf.DisplayMemberPath = "Name";
                cbbRbf.SelectedIndex = index;

                int index2 = 0;
                int j = 0;
                for (int i = 0; i < rebarTagSymbols.Count; i++)
                {
                    FamilySymbol fs2 = rebarTagSymbols[i] as FamilySymbol;
                    if (fs2.FamilyName != (cbbRbf.SelectedItem as Element).Name) continue;
                    if (fs2.Name == fs.Name)
                    {
                        index2 = j;
                    }
                    cbbRbt.Items.Add(rebarTagSymbols[i].Name);
                    j++;
                }
                //cbbRbt.DisplayMemberPath = "Name";
                cbbRbt.SelectedIndex = index2;
                selectedRebarTagType = rebarTagSymbols.Where(x => x.Name == (cbbRbt.Items[index2] as string)).First();
            }
            else
            {
                isHaveRebarTag = false;
                selectedRebarTagType = null;
            }
            Settings1.Default.Save();
            isFirstSet = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isFirstSet) return;
            double l = 0;
            if (!double.TryParse(txtLen.Text, out l))
            {
                MessageBox.Show("Wrong number!");
            }
            this.len = l;
            Settings1.Default.Offset = l;
            Settings1.Default.Save();
        }

        private void chkHaveTag_Checked(object sender, RoutedEventArgs e)
        {
            if (isFirstSet) return;
            Settings1.Default.HaveTag = this.haveTag = chkHaveTag.IsChecked.HasValue ? chkHaveTag.IsChecked.Value : false;
            Settings1.Default.Save();
        }

        private void chkHaveTag_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isFirstSet) return;
            Settings1.Default.HaveTag = this.haveTag = chkHaveTag.IsChecked.HasValue ? chkHaveTag.IsChecked.Value : false;
            Settings1.Default.Save();
        }

        private void cbbRbt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isFirstSet) return;
            selectedRebarTagType = rebarTagSymbols.Where(x => x.Name == (cbbRbt.Items[cbbRbt.SelectedIndex] as string)).First();
        }

        private void cbbRbf_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isFirstSet) return;
            try
            {
                cbbRbt.Items.Clear();
            }
            catch
            { }
            for (int i = 0; i < rebarTagSymbols.Count; i++)
            {
                FamilySymbol fs = rebarTagSymbols[i] as FamilySymbol;
                if (fs.FamilyName != (cbbRbf.SelectedItem as Element).Name) continue;
                cbbRbt.Items.Add(rebarTagSymbols[i].Name);
            }
            cbbRbt.SelectedIndex = 0;
            selectedRebarTagType = rebarTagSymbols.Where(x => x.Name == (cbbRbt.Items[0] as string)).First();
        }

        private void chkMultiPick_Checked(object sender, RoutedEventArgs e)
        {
            if (isFirstSet) return;
            Settings1.Default.MultiPick = this.isMultiPick = chkMultiPick.IsChecked.HasValue ? chkMultiPick.IsChecked.Value : false;
            Settings1.Default.Save();
        }

        private void chkMultiPick_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isFirstSet) return;
            Settings1.Default.MultiPick = this.isMultiPick = chkMultiPick.IsChecked.HasValue ? chkMultiPick.IsChecked.Value : false;
            Settings1.Default.Save();
        }

        private void chkShowAgain_Checked(object sender, RoutedEventArgs e)
        {
            if (isFirstSet) return;
            Settings1.Default.ShowAgain = this.isShowAgain = chkShowAgain.IsChecked.HasValue ? chkShowAgain.IsChecked.Value : false;
            Settings1.Default.Save();
        }

        private void chkShowAgain_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isFirstSet) return;
            Settings1.Default.ShowAgain = this.isShowAgain = chkShowAgain.IsChecked.HasValue ? chkShowAgain.IsChecked.Value : false;
            Settings1.Default.Save();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Close();
        }

        private void chkHaveSegment_Checked(object sender, RoutedEventArgs e)
        {
            if (isFirstSet) return;
            Settings1.Default.HaveSegment= this.isHaveSegment = chkHaveSegment.IsChecked.HasValue ? chkHaveSegment.IsChecked.Value : false;
            Settings1.Default.Save();
        }

        private void chkHaveSegment_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isFirstSet) return;
            Settings1.Default.HaveSegment = this.isHaveSegment = chkHaveSegment.IsChecked.HasValue ? chkHaveSegment.IsChecked.Value : false;
            Settings1.Default.Save();
        }
    }
}

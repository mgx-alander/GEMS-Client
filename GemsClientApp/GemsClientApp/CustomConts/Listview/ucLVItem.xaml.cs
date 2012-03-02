﻿using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Gems.UIWPF
{
    /// <summary>
    /// Interaction logic for ucLVItem.xaml
    /// </summary>
    public partial class ucLVItem : UserControl
    {
        private ObservableCollection<clsItem> _Collection;

        public ucLVItem()
        {
            InitializeComponent();
            preprocess();
        }

        private void preprocess()
        {
            _Collection = new ObservableCollection<clsItem>();
            refresh();
        }

        private void refresh()
        {
            lv.ItemsSource = ItemCollection;
        }

        public void AddNewItem(string n, string t, double p, int s)
        {
            ItemCollection.Add(new clsItem(n,t,p,s));
        }

        public void DeleteItem()
        {
            if (lv.SelectedIndex != -1)
            {
                ItemCollection.RemoveAt(lv.SelectedIndex);
                refresh();
            }
        }

        public ObservableCollection<clsItem> ItemCollection
        { get { return _Collection; } }

    }
    public class clsItem
    {
        public clsItem(string n, string t, double p, int s)
        {
            ItemName = n;
            ItemType = t;
            ItemPrice = p;
            ItemSValue = s;
        }

        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public double ItemPrice { get; set; }
        public int ItemSValue { get; set; }
    }
}

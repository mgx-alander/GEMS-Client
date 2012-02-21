﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gems.UIWPF
{
	/// <summary>
	/// Interaction logic for frmLocationAdminRequest.xaml
	/// </summary>
	public partial class frmLocationAdminRequest : Window
	{
        int ApprovedLocIdx = -1;
		public frmLocationAdminRequest()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

        private void radApproved_Click(object sender, RoutedEventArgs e)
        {
            ApprovedLocIdx = dgLocation.SelectedIndex;
        }
	}
}
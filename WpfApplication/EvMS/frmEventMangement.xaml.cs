﻿using System.Windows;
using System.Windows.Input;
using evmsService.entities;

namespace Gems.UIWPF
{
	/// <summary>
	/// Interaction logic for frmEventMangement.xaml
	/// </summary>
	public partial class frmEventMangement : Window
	{

        frmMain mainFrame;
        User user;
		public frmEventMangement()
		{
			this.InitializeComponent();
			CreateDTPData();
		}
		
		public void CreateDTPData()
		{
			for (int i = 0; i <= 23; i++)
            {
				cboStartHr.Items.Add(string.Format("{0:00}",i));
				cboEndHr.Items.Add(string.Format("{0:00}",i));
            }

            for (int i = 0; i <= 55; i += 5)
            {
				cboStartMin.Items.Add(string.Format("{0:00}",i));
				cboEndMin.Items.Add(string.Format("{0:00}",i));
            }
		}

        public frmEventMangement(User u, frmMain f)
            : this()
        {
            this.mainFrame = f;
            this.user = u;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            mainFrame.Visibility = Visibility.Visible;
        }
	}
}
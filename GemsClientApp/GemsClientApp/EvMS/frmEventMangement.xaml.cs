﻿using System.Windows;
using System.Windows.Input;
using evmsService.entities;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Documents;

namespace Gems.UIWPF
{
    /// <summary>
    /// Interaction logic for frmEventMangement.xaml
    /// </summary>
    public partial class frmEventMangement : Window
    {

        Window mainFrame;
        User user;
        public frmEventMangement()
        {
            this.InitializeComponent();
        }
        
        public frmEventMangement(User u, frmMain f)
            : this()
        {
            this.mainFrame = f;
            this.user = u;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Event();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            mainFrame.Visibility = Visibility.Visible;
        }

        private void Load_Event()
        {
            EventHelper client = new EventHelper();
            List<Events> list = client.ViewOrganizerEvent(user).ToList<Events>();
            lstEventList.ItemsSource = list;
            client.Close();
            lstEventList.SelectedIndex = 0;
        }

        private void Load_Event(Events E)
        {
            txtEventName.Text = E.Name;
            txtDesc.Document.Blocks.Clear();
            txtDesc.AppendText(E.Description);
            txtWebsite.Text = E.Website;
            dtpStart.SelectedDateTime = E.StartDateTime;
            dtpEnd.SelectedDateTime = E.EndDateTime;
            txtTag.Text = E.Tag;
        }

        private void lstEventList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lstEventList.SelectedIndex == -1)
                return;

            Events E = ((Events)lstEventList.SelectedItem);
            Load_Event(E);
        }

        private void btnEventEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!validateInput())
                return;

            try
            {
                DateTime startTime = dtpStart.SelectedDateTime;
                DateTime endTime = dtpEnd.SelectedDateTime;

                if (startTime.CompareTo(endTime) >= 0)
                {
                    MessageBox.Show("Invalid Date Entry, End Date Must be at a Later Date Then Start Date");
                    return;
                }
                Events SelectedEvent = ((Events)lstEventList.SelectedItem);
                var textRange = new TextRange(txtDesc.Document.ContentStart, txtDesc.Document.ContentEnd);
                EventHelper client = new EventHelper();
                client.EditEvent(user, SelectedEvent,  txtEventName.Text, startTime, endTime, textRange.Text, txtWebsite.Text, txtTag.Text);
                client.Close();
                MessageBox.Show("Operation succeeded!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Load_Event();
            
        }

        private void btnEventCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!validateInput())
                return;
            EventHelper client = new EventHelper();
            try
            {
                DateTime startTime = dtpStart.SelectedDateTime;
                DateTime endTime = dtpEnd.SelectedDateTime;

                if (startTime.CompareTo(endTime) >= 0)
                {
                    MessageBox.Show("Invalid Date Entry, End Date Must be at a Later Date Then Start Date");
                    return;
                }

                var textRange = new TextRange(txtDesc.Document.ContentStart, txtDesc.Document.ContentEnd);
                client.CreateEvent(user, txtEventName.Text, startTime, endTime, textRange.Text, txtWebsite.Text, txtTag.Text);

                MessageBox.Show("Operation succeeded!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                client.Close();
            }
            Load_Event();
        }

        private void btnEventDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstEventList.SelectedIndex == -1)
                return;

            Events evnt = ((Events)lstEventList.SelectedItem);
            EventHelper client = new EventHelper();
            try
            {
              
                client.DeleteEvent(user, evnt);

                MessageBox.Show("Operation succeeded!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                client.Close();
            }
            Load_Event();
        }

        private bool validateInput()
        {
            if (txtEventName.Text.Trim().Length == 0)
            {
                MessageBox.Show("Please Enter an Event Name",
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else if (dtpStart.SelectedDateTime == default(DateTime))
            {
                MessageBox.Show("Invalid Event Start Date",
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else if (dtpEnd.SelectedDateTime == default(DateTime))
            {
                MessageBox.Show("Invalid Event End Date",
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        private void btnEventClear_Click(object sender, RoutedEventArgs e)
        {
            lstEventList.SelectedIndex = -1;
            txtDesc.Document.Blocks.Clear();
            txtEventName.Text = "";
            txtWebsite.Text = "";
            txtTag.Text = "";
            dtpStart.clear();
            dtpEnd.clear();
        }

    }
}
﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using evmsService.entities;

namespace Gems.UIWPF
{
    /// <summary>
    /// Interaction logic for frmLocationManagement.xaml
    /// </summary>
    public partial class frmManageFacility : Window
    {
        Window mainFrame;
        User user;

        public frmManageFacility()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
        }

        public frmManageFacility(User u, frmMain f)
            : this()
        {
            this.mainFrame = f;
            this.user = u;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FacilityHelper client = new FacilityHelper();
            cboEditFac.ItemsSource = cboFaculty.ItemsSource = System.Enum.GetValues(typeof(Faculty));
            cboFacAdmin.ItemsSource = client.GetFacilityAdmins();

            List<RoomTypes> rmTypes = System.Enum.GetValues(typeof(RoomTypes)).Cast<RoomTypes>().ToList();
            cboEditFacType.Items.Clear();
            foreach (RoomTypes rmType in rmTypes)
            {
                string type2display = rmType.ToString().Replace("_", " ");
                cboEditFacType.Items.Add(type2display);
            }

            if (user.isSystemAdmin)
            {
                cboFacAdmin.IsEnabled = cboEditFac.IsEnabled = cboFaculty.IsEnabled = true;
                cboEditFac.SelectedIndex = cboFaculty.SelectedIndex = 0;
            }
            else if (user.isFacilityAdmin)
            {
                Faculty f = client.GetFacilityAdmin(user.UserID);
                cboEditFac.SelectedIndex = cboFaculty.SelectedIndex = (int)f;
                cboFacAdmin.IsEnabled = cboEditFac.IsEnabled = cboFaculty.IsEnabled = false;
                cboFacAdmin.SelectedValue = user.UserID;

            }

            client.Close();
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

        private void lstVenue_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Facility f = (Facility)lstVenue.SelectedItem;
            if (f != null)
            {
                cboFaculty.SelectedIndex = (int)f.Faculty;
                txtVenue.Text = f.FacilityID;
                txtTechContact.Text = f.TechContact;
                txtSeatCapacity1.Text = f.Capacity.ToString();
                cboFacAdmin.SelectedValue = f.BookingContact;
                txtLocation.Text = f.Location;
                cboEditFacType.SelectedIndex = (int)f.RoomType;
                if (f.HasflexibleSeating)
                {
                    chkFlexibleSeating.IsChecked = true;
                }
                if (f.HasMicrophone)
                {
                    chkMicrophone.IsChecked = true;
                }
                if (f.HasProjector)
                {
                    chkProjector.IsChecked = true;
                }
                if (f.HasVideoConferencing)
                {
                    chkVideoConference.IsChecked = true;
                }
                if (f.HasVisualizer)
                {
                    chkVisualizer.IsChecked = true;
                }
                if (f.HasWebCast)
                {
                    chkRecordFacility.IsChecked = true;
                }
            }

        }

        private void cboFaculty_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FacilityHelper client = new FacilityHelper();
            lstVenue.ItemsSource = client.GetVenuesByFaculty((Faculty)cboFaculty.SelectedIndex);
            client.Close();

            cboEditFac.SelectedIndex = cboFaculty.SelectedIndex;
            clearAllTextBoxes();
            if (user.isSystemAdmin)
            {
                cboFacAdmin.SelectedValue = -1;
            }
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int cap;
            bool parseSuccess = Int32.TryParse(txtSeatCapacity1.Text.Trim(), out cap);

            if (cboFacAdmin.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the facility admin!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            FacilityHelper client = new FacilityHelper();

            if (parseSuccess)
            {
                try
                {
                    client.UpdateFacility(user, txtVenue.Text.Trim(), (Faculty)cboEditFac.SelectedIndex,
                    txtLocation.Text.Trim(), cboFacAdmin.SelectedValue.ToString(),
                    txtTechContact.Text.Trim(), cap,(RoomTypes)cboEditFacType.SelectedIndex,(bool)chkRecordFacility.IsChecked,(bool)chkFlexibleSeating.IsChecked,
                    (bool)chkVideoConference.IsChecked,(bool)chkMicrophone.IsChecked,(bool)chkProjector.IsChecked,(bool)chkVisualizer.IsChecked);
                    MessageBox.Show("Facility successfully updated", "Update Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    lstVenue.ItemsSource = client.GetVenuesByFaculty((Faculty)cboFaculty.SelectedIndex);
                    clearAllTextBoxes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error have occured: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                client.Close();
            }
            else
            {
                MessageBox.Show("Seat Capacity must be a numeric value!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            FacilityHelper client = new FacilityHelper();
            try
            {
                client.RemoveFacility(user, txtVenue.Text.Trim(), (Faculty)cboFaculty.SelectedIndex);
                MessageBox.Show("Facility successfully deleted", "Delete Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                lstVenue.ItemsSource = client.GetVenuesByFaculty((Faculty)cboFaculty.SelectedIndex);
                clearAllTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error have occured: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            client.Close();
        }

        private void clearAllTextBoxes()
        {
           txtLocation.Text = txtSeatCapacity1.Text = txtTechContact.Text = txtVenue.Text = "";
           chkFlexibleSeating.IsChecked = chkMicrophone.IsChecked =
               chkProjector.IsChecked = chkRecordFacility.IsChecked = chkVideoConference.IsChecked =
               chkVisualizer.IsChecked = false;

        }

        private void cboEditFac_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            cboFaculty.SelectedIndex = cboEditFac.SelectedIndex;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            lstVenue.SelectedIndex = -1;
            clearAllTextBoxes();
        }


    }
}
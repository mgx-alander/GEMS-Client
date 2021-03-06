﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using evmsService.entities;

namespace Gems.UIWPF
{
    /// <summary>
    /// Interaction logic for frmAddEditReview.xaml
    /// </summary>
    public partial class frmAddEditReview : Window
    {
        frmServiceContactList servicefrm;
        User user;
        Review review;
        Service service;
        Events event_;
        public Boolean created;
        
        public frmAddEditReview()
        {
            this.InitializeComponent();
        }
        
        public frmAddEditReview(User u, frmServiceContactList f, Review r, Service s, Events e)
            : this()
        {
            created = false;
            this.servicefrm = f;
            this.user = u;
            this.review = r;
            this.service = s;
            this.event_ = e;
            
            this.txtServiceName.Text = service.Name;
            if (review == null)
            {
                //today
                this.txtReviewer.Text = user.Name;
                this.txtReviewDate.Text = DateTime.Now.ToShortDateString();
            }
            else
            {
                this.txtReviewer.Text = review.UserName;
                this.txtReviewDate.Text = review.ReviewDate.ToShortDateString();
            }
            

            //select mode

            if (review == null)
            {
                //add new
                btnDelete.IsEnabled = false;
                lblReport.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (review.UserID == user.UserID)
                {
                    //edit
                    btnDelete.IsEnabled = true;
                    lblReport.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //browse
                    if (!user.isSystemAdmin)
                    {
                        
                        btnDelete.Visibility = Visibility.Visible;
                        btnSave.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btnDelete.Visibility = Visibility.Collapsed;
                        btnSave.Visibility = Visibility.Collapsed;
                        
                    }
                    rating.IsEnabled = false;
                    txtReviewDescription.IsReadOnly = true;
                    
                    
                    lblReport.Visibility = Visibility.Visible;
                }
                load_data(review);
            }
            
        }

        private void load_data(Review r)
        {
            txtReviewDescription.AppendText(r.ReviewDescription);
            rating.setRatingValue(r.Rating);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
        
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var textRange = new TextRange(txtReviewDescription.Document.ContentStart, txtReviewDescription.Document.ContentEnd);
                ServiceContactHelper client = new ServiceContactHelper();
                if (event_ != null)
                {
                    client.Review(user, event_.EventID, service.ServiceID, rating.getRatingValue(), DateTime.Now, textRange.Text);
                }
                else
                {
                    client.Review(user, -1, service.ServiceID, rating.getRatingValue(), DateTime.Now, textRange.Text);
                }
                
                client.Close();

                MessageBox.Show("Operation Success");
                servicefrm.lstReviewList.SelectedIndex = -1;
                this.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NotifHelper client = new NotifHelper();
                client.SendNotification(user.UserID, "citadmin", "Report Review Abuse", "Reviewer: " + review.UserID + ", by: " + review.UserName + " , on Service ID: " + review.ServiceID.ToString() + " was reported for abuse");
                client.Close();

                MessageBox.Show("Operation Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceContactHelper client = new ServiceContactHelper();
                client.DeleteReview(user, review.UserID, service.ServiceID);
                client.Close();

                MessageBox.Show("Operation Success");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

      

      
    }
}

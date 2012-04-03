﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using evmsService.entities;
using System.Linq;
using System.Windows.Media;
//using System.Windows.Forms;

using System.Windows;
using System.Windows.Input;
using evmsService.entities;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Gems.UIWPF
{
    /// <summary>
    /// Interaction logic for frmServiceContactList.xaml
    /// </summary>
    public partial class frmServiceContactList : Page
    {
        User user;
        Event event_;

        public frmServiceContactList(User u, Event e)
        {
            this.InitializeComponent();
            txtsearch.TextChanged += txtsearch_TextChanged;
            this.user = u;
            this.event_ = e;

            loadServices();
            if (lstServiceList.Items.Count == 0)
                Service_SaveMode();
            else
                lstServiceList.SelectedIndex = 0;
            // Insert code required on object creation below this point.
        }

      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            

        }

        private void loadServices()
        {
            try
            {
                WCFHelperClient client = new WCFHelperClient();
                List<Service> serviceList;

                if (txtsearch.Foreground != Brushes.Black)
                {
                    serviceList = client.ViewService(user, event_.EventID, null).ToList<Service>();
                }
                else
                {
                    serviceList = client.ViewService(user, event_.EventID, txtsearch.Text.Trim()).ToList<Service>();
                }

                client.Close();



                lstServiceList.ItemsSource = serviceList
                                                 .OrderBy(x => x.Name)
                                                 .ThenBy(x => x.Notes)
                                                 .ToList<Service>();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void loadPointOfContact(int ServiceID)
        {
            try
            {
                WCFHelperClient client = new WCFHelperClient();
                List<PointOfContact> POCList = client.ViewPointOfContact(user, ServiceID).ToList<PointOfContact>();


                client.Close();


                lstContactList.ItemsSource = POCList
                                                 .OrderBy(x => x.Name)
                                                 .ThenBy(x => x.Position)
                                                 .ToList<PointOfContact>();

                if (lstContactList.Items.Count != 0)
                    lstContactList.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadReview(int ServiceID)
        {
            
            try
            {
                WCFHelperClient client = new WCFHelperClient();
                List<Review> reviewList = client.ViewReview(ServiceID).ToList<Review>();


                client.Close();


                lstReviewList.ItemsSource = reviewList
                                                .OrderBy(x => x.ReviewDate)
                                                 .ThenBy(x => x.UserName)
                                                 .ToList<Review>();
                lstReviewList_SelectionChanged(null, null);

              
                    Service service = ((Service)lstServiceList.SelectedItem);
                    for (int i = 0; i < lstReviewList.Items.Count; i++)
                    {
                        Review r = ((Review)lstReviewList.Items[i]);
                        if (r.ServiceID == service.ServiceID && user.userID == r.UserID)
                        {
                            btnAddReview.Content = "Edit Own Review";
                            return;
                        }
                    }


                    btnAddReview.Content = "Add New Review";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lstServiceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Service service = ((Service)lstServiceList.SelectedItem);

            if (service != null)
            {
                
                btnAddNewPOC.IsEnabled = true;
                
                txtWebsite.Text = service.Url;
                txtNote.Document.Blocks.Clear();
                txtNote.AppendText(service.Notes);
                txtName.Text = service.Name;
                txtAddress.Text = service.Address;

                Enable_ReviewContactField();
                loadPointOfContact(service.ServiceID);
                loadReview(service.ServiceID);
                lstContactList_SelectionChanged(null, null);
                Service_BrowseMode();
                Enable_PointOfContactField();
                
                btnDelete.IsEnabled = true;
            }
            else
            {

                btnAddNewPOC.IsEnabled = false;
                Service_SaveMode();
                Disable_PointOfContactField();
                Disable_ReviewField();
                btnDelete.IsEnabled = false;
            }
        }

        private void Disable_ServiceField()
        {
            txtWebsite.IsReadOnly = true;
            txtNote.IsReadOnly = true;
            txtName.IsReadOnly = true;
            txtAddress.IsReadOnly = true;

        }

        private void Service_BrowseMode()
        {
            btnAddNewService.Content = "Save";
            //Disable_ServiceField();
        }

        private void Service_SaveMode()
        {
            ServiceFieldClear();
            lstServiceList.SelectedIndex = -1;
            btnAddNewService.Content = "Add New";
            //Enable_ServiceField();
        }

        private void Enable_ServiceField()
        {
            txtWebsite.IsReadOnly = false;
            txtNote.IsReadOnly = false;
            txtName.IsReadOnly = false;
            txtAddress.IsReadOnly = false;

        }

        private void Disable_PointOfContactField()
        {

            PointOfContactClear();
            lstContactList.ItemsSource = null;
            btnAddNewPOC.IsEnabled = false;
            btnDeletePOC.IsEnabled = false;
            btnCancelPOC.IsEnabled = false;

            lstContactList.IsEnabled = false;
            txtContactEmail.IsReadOnly = true;
            txtContactName.IsReadOnly = true;
            txtContactPosition.IsReadOnly = true;
            txtContactTel.IsReadOnly = true;
        }
        private void Enable_PointOfContactField()
        {
            btnAddNewPOC.IsEnabled = true;
            btnDeletePOC.IsEnabled = true;
            btnCancelPOC.IsEnabled = true;

            lstContactList.IsEnabled = true;
            txtContactEmail.IsReadOnly = false;
            txtContactName.IsReadOnly = false;
            txtContactPosition.IsReadOnly = false;
            txtContactTel.IsReadOnly = false;
        }



        private void Disable_ReviewField()
        {


            lstReviewList.ItemsSource = null;
            btnAddReview.IsEnabled = false;
            btnDeleteReview.IsEnabled = false;
           // btnViewReview.IsEnabled = false;
            lstReviewList.IsEnabled = false;
     
        }
        private void Enable_ReviewContactField()
        {
            btnAddReview.IsEnabled = true;
            btnDeleteReview.IsEnabled = true;
          //  btnViewReview.IsEnabled = true;

            lstReviewList.IsEnabled = true;
          
        }

        private void lstContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            PointOfContact poc = ((PointOfContact)lstContactList.SelectedItem);

            if (poc != null)
            {
                btnAddNewPOC.Content = "Save";
                
                txtContactEmail.Text = poc.Email;
                txtContactName.Text = poc.Name;
                txtContactPosition.Text = poc.Position;
                txtContactTel.Text = poc.Phone;
                btnDeletePOC.IsEnabled = true;
            }
            else
            {
                btnAddNewPOC.Content = "Add New";
                
                txtContactEmail.Text = "";
                txtContactName.Text = "";
                txtContactPosition.Text = "";
                txtContactTel.Text = "";
                btnDeletePOC.IsEnabled = false;
            }
        }

        private void Select_PointOfContact(String name, String position, String phone, String email)
        {
            for (int i = 0; i < lstContactList.Items.Count; i++)
            {
                PointOfContact poc = (PointOfContact)lstContactList.Items[i];
                if (poc.Name.Equals(name) && poc.Position.Equals(position) && poc.Phone.Equals(phone) && poc.Email.Equals(email))
                {
                    lstContactList.SelectedIndex = i;
                    return;
                }
            }
        }
        private void ServiceFieldClear()
        {
            txtWebsite.Text = "";
            txtNote.Document.Blocks.Clear();
            txtName.Text = "";
            txtAddress.Text = "";
        }

        private void PointOfContactClear()
        {
            txtContactEmail.Text = "";
            txtContactName.Text = "";
            txtContactPosition.Text = "";
            txtContactTel.Text = "";
        }

        private void btnAddNewPOC_Click(object sender, RoutedEventArgs e)
        {
            if (lstContactList.SelectedIndex == -1)
            {

                if (txtContactName.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Name of Point of Contact");
                    return;
                }
                if (txtContactPosition.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Position of Point of Contact");
                    return;
                }
                if (txtContactTel.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Phone of Point of Contact");
                    return;
                }

                try
                {
                    WCFHelperClient client = new WCFHelperClient();
                    client.AddPointOfContact(user, event_.EventID, ((Service)lstServiceList.SelectedItem).ServiceID,
                        txtContactName.Text.Trim(), txtContactPosition.Text.Trim(), txtContactTel.Text.Trim(), txtContactEmail.Text.Trim());
                    client.Close();

                    loadPointOfContact(((Service)lstServiceList.SelectedItem).ServiceID);
                    Select_PointOfContact(txtContactName.Text.Trim(), txtContactPosition.Text.Trim(), txtContactTel.Text.Trim(), txtContactEmail.Text.Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //check and save

            }
            else
            {
                PointOfContact poc = ((PointOfContact)lstContactList.SelectedItem);
                //edit
                UpdatePointOfContact(poc);
                loadPointOfContact(((Service)lstServiceList.SelectedItem).ServiceID);
            }
        }

        private void SaveService()
        {
            try
            {
                var textRange = new TextRange(txtNote.Document.ContentStart, txtNote.Document.ContentEnd);
                WCFHelperClient client = new WCFHelperClient();
                client.AddService(user, event_.EventID, txtAddress.Text.Trim(), txtName.Text.Trim(), txtWebsite.Text.Trim(), textRange.Text.Trim());


                client.Close();


               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateService(Service service)
        {
            try
            {
                var textRange = new TextRange(txtNote.Document.ContentStart, txtNote.Document.ContentEnd);
                WCFHelperClient client = new WCFHelperClient();
                client.EditService(service.ServiceID, user, event_.EventID, txtAddress.Text.Trim(), txtName.Text.Trim(), txtWebsite.Text.Trim(), textRange.Text.Trim());


                client.Close();




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdatePointOfContact(PointOfContact poc)
        {
            try
            {
                WCFHelperClient client = new WCFHelperClient();
                client.EditPointOfContact(user, event_.EventID, poc.PointOfContactID,txtContactName.Text, txtContactPosition.Text, txtContactTel.Text, txtContactEmail.Text);


                client.Close();




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteService(Service service)
        {
            try
            {
                WCFHelperClient client = new WCFHelperClient();
                client.DeleteService(user,service.ServiceID);


                client.Close();




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeletePointOfContact(PointOfContact poc)
        {
            try
            {
                WCFHelperClient client = new WCFHelperClient();
                client.DeletePointOfContact(user, poc.PointOfContactID);


                client.Close();




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnAddNewService_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Service service = ((Service)lstServiceList.SelectedItem);

            String name = txtName.Text.Trim();
            if (name.Equals(""))
            {
                MessageBox.Show("Invalid Service Name");
                return;
            }


            if (service != null)
            {
                UpdateService(service);
                //update
            }
            else
            {
                
                //checking
                SaveService();
                //save
                
            }

            loadServices();
            for (int i = 0; i < lstServiceList.Items.Count; i++)
            {
                if (((Service)lstServiceList.Items[i]).Name.Equals(name))
                {
                    lstServiceList.SelectedIndex = i;
                    break;
                }
            }
            
        }

        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ServiceFieldClear();
            lstServiceList.SelectedIndex = -1;
        }

        private void btnCancelPOC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PointOfContactClear();
            lstContactList.SelectedIndex = -1;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            
            Service service = ((Service)lstServiceList.SelectedItem);

            DeleteService(service);

            loadServices();
        }
        
        private void btnDeletePOC_Click(object sender, RoutedEventArgs e)
        {

            PointOfContact poc = ((PointOfContact)lstContactList.SelectedItem);

            DeletePointOfContact(poc);

            Service service = ((Service)lstServiceList.SelectedItem);

            loadPointOfContact(service.ServiceID);
        }

        private void btnAddReview_Click(object sender, RoutedEventArgs e)
        {
            Service service = ((Service)lstServiceList.SelectedItem);
            frmAddEditReview frm;
            Review sr=null;
            for (int i = 0; i < lstReviewList.Items.Count; i++)
            {
                Review r = ((Review)lstReviewList.Items[i]);
                if (r.UserID.Equals(user.userID))
                {
                    sr = r;
                    frm = new frmAddEditReview(user, this, r, service, event_);
                    frm.ShowDialog();
                    
                    goto End;
                }
            }

            
            
           
            
            frm = new frmAddEditReview(user, this, null, service,event_);
            frm.ShowDialog();
            //loadReview(service.ServiceID);
            //frm.created

            End:
            loadReview(service.ServiceID);
            for (int i = 0; i < lstReviewList.Items.Count; i++)
            {
                Review r = ((Review)lstReviewList.Items[i]);
               
                if (r.ServiceID == service.ServiceID && r.UserID == user.userID)
                {
                    lstReviewList.SelectedIndex = i;
                }
                //}
            }
        }

        private void btnDeleteReview_Click(object sender, RoutedEventArgs e)
        {
            Service service = ((Service)lstServiceList.SelectedItem);
            Review r = (Review)lstReviewList.SelectedItem;
            try
            {
                if(r != null)
                {
                WCFHelperClient client = new WCFHelperClient();
                client.DeleteReview(user,r.UserID,r.ServiceID);
                
                client.Close();

                loadReview(service.ServiceID);
                }





            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnCancelReview_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnViewReview_Click(object sender, RoutedEventArgs e)
        {
            
            Service service = ((Service)lstServiceList.SelectedItem);
            Review r = (Review)lstReviewList.SelectedItem;
            frmAddEditReview frm = new frmAddEditReview(user, this, r, service,event_);
            frm.ShowDialog();
            loadReview(service.ServiceID);

            if (r != null)
            {
                for (int i = 0; i < lstReviewList.Items.Count; i++)
                {
                    Review rv = ((Review)lstReviewList.Items[i]);
                    if (rv.ServiceID == r.ServiceID && rv.UserID == r.UserID)
                    {
                        lstReviewList.SelectedIndex = i;
                    }
                }
            }
        }

        private void lstReviewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Review r = (Review)lstReviewList.SelectedItem;
            if (r != null)//something is selected
            {
                //btnViewReview.IsEnabled = true;
                btnDeleteReview.IsEnabled = true;
            }
            else//nothing selected
            {
                //btnViewReview.IsEnabled = false;
                btnDeleteReview.IsEnabled = false;
            }
        }

        private void txtsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtsearch.Text.Trim().Equals(""))
            {

                var bc = new BrushConverter();

                txtsearch.TextChanged -= txtsearch_TextChanged;
                txtsearch.Text = "Search";
                txtsearch.SelectAll();
                
                txtsearch.Foreground = (Brush)bc.ConvertFrom("#69000000");
                loadServices();
                txtsearch.TextChanged += txtsearch_TextChanged;

            }
            else
            {
                txtsearch.Foreground = Brushes.Black;
                loadServices();
            }
        }
        //bool alreadyFocused;
        private void txtsearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtsearch.Foreground != Brushes.Black)
            {
                txtsearch.TextChanged -= txtsearch_TextChanged;
                txtsearch.Text = "";
                txtsearch.TextChanged += txtsearch_TextChanged;
            }
            

        }

        private void txtsearch_LostFocus(object sender, RoutedEventArgs e)
        {
            //txtsearch_TextChanged(null, null);
        }

        private void clickViewReview_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(sender.GetType().ToString());
            //MessageBox.Show(((Hyperlink)sender).Parent.GetType().ToString());
            //Label l = (Label)((Hyperlink)sender).Parent;
            //MessageBox.Show(l.Parent.GetType().ToString());
            
            String temp = ((Hyperlink)sender).Tag.ToString();
            for (int i = 0; i < lstReviewList.Items.Count; i++)
            {
                if (((Review)lstReviewList.Items[i]).UserID == temp)
                {
                    lstReviewList.SelectedIndex = i;
                    break;
                }

            }
            //Control c;
            //do{
            //    c = (Control)sender;
            //    Control pc = (Control)c.Parent;
            //    c = pc;

            //    MessageBox.Show(c.GetType().ToString());
            //}while(true);
            //for(int j=0;j<lstReviewList.Items.Count;j++)
            //{
            //    if (lstReviewList.IsMouseOver == true)
            //    {
            //        lstReviewList.SelectedIndex = j;
            //        break;
            //    }
            //}
            
           
            Service service = ((Service)lstServiceList.SelectedItem);
            Review r = (Review)lstReviewList.SelectedItem;
            frmAddEditReview frm = new frmAddEditReview(user, this, r, service, event_);
            frm.ShowDialog();
            loadReview(service.ServiceID);

            if (r != null)
            {
                for (int i = 0; i < lstReviewList.Items.Count; i++)
                {
                    Review rv = ((Review)lstReviewList.Items[i]);
                    if (rv.ServiceID == r.ServiceID && rv.UserID == r.UserID)
                    {
                        lstReviewList.SelectedIndex = i;
                    }
                }
            }
        }

        //int GetCurrentIndex(GetPositionDelegate getPosition)
        //{

        //    int index = -1;
        //    for (int i = 0; i < this.lstReviewList.Items.Count; ++i)
        //    {
        //        ListViewItem item = GetListViewItem(i);
        //        if (this.IsMouseOverTarget(item, getPosition))
        //        {
        //            index = i;
        //            break;
        //        }
        //    }
        //    return index;
        //}
        //bool IsMouseOverTarget(Visual target, GetPositionDelegate getPosition)
        //{
        //    if (target == null)
        //        return false;
        //    Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
        //    Point mousePos = getPosition((IInputElement)target);
        //    return bounds.Contains(mousePos);
        //}

        //ListViewItem GetListViewItem(int index)
        //{
        //    if (lstReviewList.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
        //        return null;

        //    return lstReviewList.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        //}

      
    }
}

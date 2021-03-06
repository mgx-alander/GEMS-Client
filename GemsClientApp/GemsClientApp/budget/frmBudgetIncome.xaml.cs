﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using evmsService.entities;

namespace Gems.UIWPF
{
    /// <summary>
    /// Interaction logic for BudgetIncome.xaml
    /// </summary>
    public partial class frmBudgetIncome : GEMSPage
    {

        User user;
        Events event_;
        int selectedIndex = -1;
        List<BudgetIncome> incomeList;

        public frmBudgetIncome()
        {
            InitializeComponent();
        }



        public frmBudgetIncome(User user, Events e)
            : this()
        {
            this.user = user;
            this.event_ = e;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lstIncomeList.SelectedValuePath = "IncomeID";
            loadIncome();
        }

        private void loadIncome()
        {
            BudgetHelper client = new BudgetHelper();
            try
            {
                txtGstPercent.Text = client.GetGST().ToString();
                incomeList = client.ViewBudgetIncome(user, event_.EventID).ToList<BudgetIncome>();
                lstIncomeList.ItemsSource = incomeList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                client.Close();
            }
            clearAll();
            CalculateNettIncome();
        }

        private void onTextChanged(object sender, TextChangedEventArgs e)
        {
            changed = true;

        }

        //TODO: Save Changes and Test Method!
        public override bool saveChanges()
        {
            if (txtName.Text.Trim().Length == 0)
            {
                MessageBox.Show("Please enter the income's name.", "Invalid Input",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else if (txtSource.Text.Trim().Length == 0)
            {
                MessageBox.Show("Please enter the source of the income.", "Invalid Input",
                  MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else if (txtIncb4Gst.Text.Trim().Length == 0)
            {
                MessageBox.Show("Income must be numeric!", "Invalid Input",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                txtIncb4Gst.Focus();
                return false;
            }
            else if (dtpReceivedDate.SelectedDate==null)
            {
                MessageBox.Show("Please Select your received date!", "Invalid Date",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            try
            {
                BudgetHelper client = new BudgetHelper();
                if (selectedIndex == -1)
                {
                    BudgetIncome bIncome = new BudgetIncome();
                    bIncome.Name = txtName.Text;
                    bIncome.Description = txtDescription.Text;
                    bIncome.DateReceived = dtpReceivedDate.SelectedDate.Value;
                    bIncome.EventID = event_.EventID;
                    bIncome.GstValue = decimal.Parse(txtGst.Text.Trim());
                    bIncome.IncomeBeforeGST = decimal.Parse(txtIncb4Gst.Text.Trim());
                    bIncome.IsGstLiable = chkGSTLiable.IsChecked.Value;
                    bIncome.Source = txtSource.Text;
                    bIncome.IncomeID = client.AddIncome(user, event_.EventID, bIncome);
                    incomeList.Add(bIncome);
                    CollectionViewSource.GetDefaultView(lstIncomeList.ItemsSource).Refresh();
                    clearAll();
                }
                else
                {
                    BudgetIncome bIncome = incomeList[selectedIndex];
                    bIncome.Name = txtName.Text;
                    bIncome.Description = txtDescription.Text;
                    bIncome.DateReceived = dtpReceivedDate.SelectedDate.Value;
                    bIncome.EventID = event_.EventID;
                    bIncome.GstValue = decimal.Parse(txtGst.Text.Trim());
                    bIncome.IncomeBeforeGST = decimal.Parse(txtIncb4Gst.Text.Trim());
                    bIncome.IsGstLiable = chkGSTLiable.IsChecked.Value;
                    bIncome.Source = txtSource.Text;
                    client.EditBudgetIncome(user, event_.EventID, bIncome.IncomeID, bIncome);
                    incomeList[selectedIndex] = bIncome;
                    CollectionViewSource.GetDefaultView(lstIncomeList.ItemsSource).Refresh();
                    changed = false;
                }
                client.Close();

                MessageBox.Show("Operation succeeded!");
                loadIncome();
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        private void lstIncomeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Helper.IdleHelper.startIdleTimer();
            if (lstIncomeList.SelectedIndex == -1)
            {
                Helper.IdleHelper.stopIdleTimer();
                selectedIndex = -1;
                return;
            }
            if (selectedIndex == lstIncomeList.SelectedIndex)
                return;
            if (changed)
            {
                MessageBoxResult answer = MessageBox.Show("There are unsaved changes. Would you like to save your changes now?", "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if ((answer == MessageBoxResult.Yes && !saveChanges()) || answer == MessageBoxResult.Cancel)
                {
                    lstIncomeList.SelectedIndex = selectedIndex;
                    return;
                }
            }
            BudgetIncome bIncome = (BudgetIncome)lstIncomeList.SelectedItem;

            txtName.Text = bIncome.Name;
            txtDescription.Text = bIncome.Description;
            txtGst.Text = bIncome.GstValue.ToString("0.00");
            txtIncb4Gst.Text = bIncome.IncomeBeforeGST.ToString("0.00");
            txtSource.Text = bIncome.Source;
            chkGSTLiable.IsChecked = bIncome.IsGstLiable;
            dtpReceivedDate.SelectedDate = bIncome.DateReceived;
            txtIncAftGst.Text = bIncome.IncomeAfterGST.ToString("0.00");

            btnAdd.Content = "Save";
            changed = false;
            selectedIndex = lstIncomeList.SelectedIndex;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            saveChanges();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstIncomeList.SelectedIndex == -1)
                return;
            BudgetHelper client = new BudgetHelper();
           
            try
            {
                BudgetIncome bIncome = (BudgetIncome)lstIncomeList.SelectedItem;
                client.DeleteBudgetIncome(user, bIncome.IncomeID, event_.EventID);
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
            loadIncome();
            CalculateNettIncome();
        }

        private void clearAll(object sender, RoutedEventArgs e)
        {
            clearAll();
        }

        private void clearAll()
        {
            lstIncomeList.SelectedIndex = -1;
            txtName.Text = "";
            txtDescription.Text = "";
            txtGst.Text = "";
            txtIncb4Gst.Text = "";
            txtSource.Text = "";
            txtIncAftGst.Text = "";
            btnAdd.Content = "Add";
            changed = false;
        }

        private void txtIncb4Gst_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtIncb4Gst.Text.Trim().Length == 0)
            {
                return;
            }
            decimal income;
            decimal gst = Decimal.Parse(txtGstPercent.Text.Trim()) / 100;
            decimal gstAmt = 0;
            if (Decimal.TryParse(txtIncb4Gst.Text.Trim(), out income))
            {
                if (chkGSTLiable.IsChecked.Value == true)
                {
                    gstAmt = gst * income;
                }
                else
                {
                    gstAmt = 0;
                }
                txtGst.Text = gstAmt.ToString("0.00");
                decimal incAfterGST = income - gstAmt;
                txtIncAftGst.Text = incAfterGST.ToString("0.00");
                CalculateNettIncome(incAfterGST);
            }
            else
            {
                MessageBox.Show("Income must be numeric! Please try again", "Invalid Input",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                txtIncb4Gst.Text = "";
                txtIncb4Gst.Focus();
            }

        }

        private void chkGSTLiable_Checked(object sender, RoutedEventArgs e)
        {
            txtIncb4Gst_LostFocus(sender, e);
        }

        private void CalculateNettIncome(decimal newIncome = 0)
        {
            decimal nett = 0;
            if (lstIncomeList.SelectedIndex == -1)
            {
                nett = newIncome;
            }
            for (int i = 0; i < lstIncomeList.Items.Count; i++)
            {
                BudgetIncome inc = (BudgetIncome)lstIncomeList.Items[i];
                if (lstIncomeList.SelectedIndex == i)
                {
                    nett += decimal.Parse(txtIncAftGst.Text);
                }
                else
                {
                    nett += inc.IncomeAfterGST;
                }
            }
            txtNettIncome.Text = nett.ToString("0.00");
        }
    }
}

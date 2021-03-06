﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using evmsService.entities;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Gems.UIWPF
{
    /// <summary>
    /// Interaction logic for frmFields.xaml
    /// </summary>
    public partial class frmFields : GEMSPage
    {
        User user;
        Events event_;
        ObservableCollection<Field> fields;

        public frmFields()
        {
            InitializeComponent();
        }

        public frmFields(User u, Events e)
            : this()
        {
            this.user = u;
            this.event_ = e;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e){
        
            RegistrationHelper client = new RegistrationHelper();
            try
            {

                List<StaticField> staticFields = new List<StaticField>() {
                    new StaticField() { FieldName = "Custom", FieldLabel = "Custom" }
                };
                staticFields.AddRange(client.ViewStaticField().ToList());
                cbStaticFields.ItemsSource = staticFields;
                Publish publish = client.ViewPublish(event_.EventID);
                fields = new ObservableCollection<Field>(client.ViewField(event_.EventID));

                if (fields.Count == 0)
                {
                    Field firstName = new Field();
                    firstName.FieldName = firstName.FieldLabel = "First Name";
                    firstName.Remarks = "";
                    Field lastName = new Field();
                    lastName.FieldName = lastName.FieldLabel = "Last Name";
                    lastName.Remarks = "";
                    firstName.IsRequired = lastName.IsRequired = true;
                    Field email = new Field();
                    email.FieldName = email.FieldLabel = "Email";
                    email.IsRequired = email.IsRequired = true;
                    email.Remarks = "";

                    fields = new ObservableCollection<Field>() { firstName, lastName, email };


                }
                fields.CollectionChanged += (s, ev) =>
                {
                    changed = true;
                };
                lstFields.ItemsSource = fields;
                DisableListItemChildren(0);
                DisableListItemChildren(1);
                DisableListItemChildren(2);

                if (publish != null)
                {
                    for (int i = 0; i < lstFields.Items.Count; i++)
                    {
                        DisableListItemChildren(i);
                    }
                    btnAdd.IsEnabled = false;
                    btnSave.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void DisableListItemChildren(int index)
        {
            this.Dispatcher.BeginInvoke(
                DispatcherPriority.Loaded,
                new Action(() =>
                {
                    ListBoxItem listBoxItem =
                        (ListBoxItem)(lstFields.ItemContainerGenerator.ContainerFromItem(lstFields.Items[index]));
                    ContentPresenter presenter = FindVisualChild<ContentPresenter>(listBoxItem);
                    DataTemplate dataTemplate = presenter.ContentTemplate;
                    TextBox txtFirstName = (TextBox)dataTemplate.FindName("txtFieldName", presenter);
                    txtFirstName.IsEnabled = false;
                    TextBox txtLastName = (TextBox)dataTemplate.FindName("txtFieldLabel", presenter);
                    txtLastName.IsEnabled = false;
                    TextBox txtRemarks = (TextBox)dataTemplate.FindName("txtRemarks", presenter);
                    txtRemarks.Visibility = Visibility.Collapsed;
                    Label lblRemarks = (Label)dataTemplate.FindName("lblRemarks", presenter);
                    lblRemarks.Visibility = Visibility.Collapsed;
                    CheckBox chkRequired = (CheckBox)dataTemplate.FindName("chkRequired", presenter);
                    chkRequired.IsEnabled = false;
                    Button btnClear = (Button)dataTemplate.FindName("btnClear", presenter);
                    btnClear.IsEnabled = false;
                    Button btnDelete = (Button)dataTemplate.FindName("btnDelete", presenter);
                    btnDelete.IsEnabled = false;
                }));
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            StackPanel parent = (StackPanel)((DockPanel)((Button)sender).Parent).Parent;
            TextBox txtFieldLabel = (TextBox)parent.FindName("txtFieldLabel");
            TextBox txtFieldName = (TextBox)parent.FindName("txtFieldName");
            TextBox txtRemarks = (TextBox)parent.FindName("txtRemarks");
            CheckBox chkRequired = (CheckBox)parent.FindName("chkRequired");
            txtFieldLabel.Text = "";
            txtFieldName.Text = "";
            txtRemarks.Text = "";
            chkRequired.IsChecked = false;
            changed = true;
        }

        T GetParent<T>(DependencyObject obj)
            where T : DependencyObject
        {
            while (obj != null)
            {
                obj = VisualTreeHelper.GetParent(obj);
                if (obj is T)
                    return (T)obj;
            }
            return null;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem listBoxItem = GetParent<ListBoxItem>((DependencyObject)sender);
            fields.Remove((Field)listBoxItem.Content);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            switch (cbStaticFields.SelectedIndex)
            {
                case -1:
                    break;
                case 0:
                    fields.Add(new Field());
                    break;
                default:
                    fields.Add(new Field()
                    {
                        FieldName = (string)cbStaticFields.SelectedValue,
                        FieldLabel = ((StaticField)cbStaticFields.SelectedItem).FieldLabel
                    });
                    break;
            }
            cbStaticFields.SelectedIndex = 0;
            lstFields.ScrollIntoView(lstFields.Items[lstFields.Items.Count - 1]);
        }

        public override bool saveChanges()
        {
            foreach (Field field in fields)
            {
                if (field.FieldName == null || field.FieldLabel == null || field.FieldName.Trim() == "" || field.FieldLabel.Trim() == "")
                {
                    MessageBox.Show("Field name and field label must not be empty.");
                    return false;
                }
            }
            RegistrationHelper client = new RegistrationHelper();
            try
            {

                client.AddField(user, event_.EventID, fields.ToArray());

                changed = false;
                MessageBox.Show("Operation succeeded!");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                client.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            saveChanges();
        }
    }
}

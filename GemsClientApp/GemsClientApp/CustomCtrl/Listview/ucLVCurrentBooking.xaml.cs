﻿using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Collections.Generic;
using evmsService.entities;

namespace Gems.UIWPF.CustomCtrl
{
    /// <summary>
    /// Interaction logic for ucLVCurrentBooking.xaml
    /// </summary>
    public partial class ucLVCurrentBooking : UserControl
    {
        ObservableCollection<BookingDetail> _details;
        public ucLVCurrentBooking()
        {
            this.InitializeComponent();
            _details = new ObservableCollection<BookingDetail>();
        }

        public void setSource(FacilityBookingRequest fbr)
        {
            _details = new ObservableCollection<BookingDetail>();
            List<FacilityBookingRequestDetails> temp = fbr.RequestDetails.ToList<FacilityBookingRequestDetails>(); ;
            int confirmedLocationIdx = 0;
            if (fbr.Status == BookingStatus.Approved)
            {
                WCFHelperClient client = new WCFHelperClient();
                confirmedLocationIdx = client.getConfirmedBooking(fbr.RequestID).RequestDetailID;
                client.Close();
            }

            for (int i = 0; i < temp.Count; i++)
            {
                BookingDetail bd = new BookingDetail();
                bd.FacilityID = temp[i].FacilityID;
                bd.Faculty = temp[i].Faculty;
                bd.Priority = temp[i].Priority;

                if ((fbr.Status == BookingStatus.Approved) && (temp[i].RequestDetailsID == confirmedLocationIdx))
                {
                    bd.Balance = 1;
                }
                _details.Add(bd);
            }

            lv.ItemsSource = _details;

            //Change the color here..
        }

        
        public void ScrollToItem(int posIdx)
        {
            var listView = lv;
            listView.SelectedItem = listView.Items.GetItemAt(posIdx);
            listView.ScrollIntoView(listView.Items[0]);
            listView.ScrollIntoView(listView.SelectedItem);
        }
    }

    public class BookingDetail
    {
        public int Priority { get; set; }
        public Faculty Faculty { get; set; }
        public string FacilityID { get; set; }
        public int Balance { get; set; }
    }
}
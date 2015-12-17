using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RT_PrintDemoII
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PrintLabel : Page
    {
        private string _strItemName = Constants.EMPTYSTRING;  // Item Label 
        private string _strItemNo = Constants.EMPTYSTRING;    // Iteem no
        private string _strURL = Constants.EMPTYSTRING;       // URL label
        private string _strTextLine1 = Constants.EMPTYSTRING; // TextLine1
        private string _strTextLine2 = Constants.EMPTYSTRING; // TextLine2
        private bool _bIsItemLabel = Constants.ISITEMLBL; // Is Item Label
        private ResourceLoader rm;
        private StatusBar prog = StatusBar.GetForCurrentView();

        public PrintLabel()
        {
            this.InitializeComponent();
            rm = ResourceLoader.GetForCurrentView("StringLibrary");
            this.btnPrintLabel.IsEnabled = false;
            EnableItemControls(false);
            EnableURLControls(false);
            App.AssignLabelObject(this);
        }

        private async void updateStatusBar(string resourceString)
        {
            if (null != rm)
                prog.ProgressIndicator.Text = resourceString;
            prog.ProgressIndicator.ProgressValue = 0;
            await prog.ProgressIndicator.ShowAsync();

        }

        /// ************************************************************************************************
        /// <summary>
        /// EnableItemControls 
        /// </summary>
        /// <remarks>
        /// EnableItem label related Controls 
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void EnableItemControls(bool bEnable)
        {
            _bIsItemLabel = bEnable;
            this.txtItemName.IsEnabled = bEnable;
            this.txtItemNo.IsEnabled = bEnable;
            UpdatePrintLabelBtn();

        }

        /// ************************************************************************************************
        /// <summary>
        /// EnableURLControls 
        /// </summary>
        /// <remarks>
        /// Enables URL Label related Controls
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void EnableURLControls(bool bEnable)
        {
            _bIsItemLabel = false;
            this.txtURL.IsEnabled = bEnable;
            this.txtTextLine1.IsEnabled = bEnable;
            this.txtTextLine2.IsEnabled = bEnable;
            UpdatePrintLabelBtn();
        }

        /// ************************************************************************************************
        /// <summary>
        /// UpdatePrintLabelBtn 
        /// </summary>
        /// <remarks>
        /// Enable PrintLabel button based on user inputs 
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void UpdatePrintLabelBtn()
        {
            if (_bIsItemLabel)
            {
                if (_strItemNo != Constants.EMPTYSTRING &&
                    _strItemName != Constants.EMPTYSTRING)
                    this.btnPrintLabel.IsEnabled = true;
                else
                    this.btnPrintLabel.IsEnabled = false;
            }
            else
            {
                if (_strURL != Constants.EMPTYSTRING &&
                    _strTextLine1 != Constants.EMPTYSTRING &&
                    _strTextLine2 != Constants.EMPTYSTRING)
                    this.btnPrintLabel.IsEnabled = true;
                else
                    this.btnPrintLabel.IsEnabled = false;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// txtItemName_TextChanged 
        /// </summary>
        /// <remarks>
        /// txtItemName_TextChanged event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void txtItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _strItemName = this.txtItemName.Text;
                UpdatePrintLabelBtn();
            }
            catch (Exception)
            {
            }
        }

        private void btnPrintLabel_Click(object sender, RoutedEventArgs e)
        {
            updateStatusBar(rm.GetString("LBL_PRINT_STATUS"));
            try
            {
                int ret = -1;
                if (_bIsItemLabel)
                {
                    if (_strItemName != "" && _strItemNo != "")
                    {
                        //define dictionary for Item label 
                        Dictionary<string, string> objKeyValue = new Dictionary<string, string>();
                        objKeyValue.Add(Constants.LBL_ITEMNAME, _strItemName);
                        objKeyValue.Add(Constants.LBL_ITEMNO, _strItemNo);
                        ret = App.LabelPrinterObject.WriteLabel(Constants.LBL_ITEM, objKeyValue, 2);
                        updateStatusBar(rm.GetString("LBL_PRINT_SUCCESS"));
                    }
                }
                else
                {
                    if (_strURL != "" && _strTextLine1 != "" && _strTextLine2 != "")
                    {
                        //define dictionary for QR label 
                        Dictionary<string, string> objKeyValue = new Dictionary<string, string>();
                        objKeyValue.Add(Constants.LBL_URL, _strURL);
                        objKeyValue.Add(Constants.LBL_TL1, _strTextLine1);
                        objKeyValue.Add(Constants.LBL_TL2, _strTextLine2);
                        ret = App.LabelPrinterObject.WriteLabel(Constants.LBL_URLQR, objKeyValue, 3);
                        updateStatusBar(rm.GetString("LBL_PRINT_SUCCESS"));
                    }

                }

            }
            catch (Exception)
            {
                 updateStatusBar(rm.GetString("LBL_ERR_STATUS"));
            }
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void rbItemLabel_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.rbItemLabel.IsChecked == true)
                {
                    EnableURLControls(false);
                    EnableItemControls(true);
                }
                else
                    EnableItemControls(false);
            }
            catch (Exception)
            {
                if (null != rm)
                   updateStatusBar(rm.GetString("INVALID_ERR"));
            }
        }


        private void rbURLLabel_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.rbURLLabel.IsChecked == true)
                {
                    EnableItemControls(false);
                    EnableURLControls(true);
                }
                else
                    EnableURLControls(false);
            }
            catch (Exception)
            {
                if (null != rm)
                    updateStatusBar(rm.GetString("INVALID_ERR"));
            }
        }

        private void txtItemName_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                _strItemName = this.txtItemName.Text;
                UpdatePrintLabelBtn();
            }
            catch (Exception)
            {
                if (null != rm)
                    updateStatusBar(rm.GetString("INVALID_ERR"));
            }
        }

        private void txtItemNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _strItemNo = this.txtItemNo.Text;
                UpdatePrintLabelBtn();
            }
            catch (Exception)
            {
                if (null != rm)
                    updateStatusBar(rm.GetString("INVALID_ERR"));
            }
        }

        private void txtURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _strURL = this.txtURL.Text;
                UpdatePrintLabelBtn();
            }
            catch (Exception)
            {
                if (null != rm)
                    updateStatusBar(rm.GetString("INVALID_ERR"));
            }
        }

        private void txtTextLine1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _strTextLine1 = this.txtTextLine1.Text;
                UpdatePrintLabelBtn();
            }
            catch (Exception)
            {
                if (null != rm)
                    updateStatusBar(rm.GetString("INVALID_ERR"));
            }
        }

        private void txtTextLine2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _strTextLine2 = this.txtTextLine2.Text;
                UpdatePrintLabelBtn();
            }
            catch (Exception)
            {
                if (null != rm)
                    updateStatusBar(rm.GetString("INVALID_ERR"));
            }
        }
    }
}

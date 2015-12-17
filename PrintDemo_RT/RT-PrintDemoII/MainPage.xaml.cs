using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using HSM.Mobility.Printing;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace RT_PrintDemoII
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //  Objects
        private StatusBar prog = StatusBar.GetForCurrentView();
        private ResourceLoader rm;
        private StackPanel _spOld;
        private LinePrinter _objLinePrinter = null; // Line printer object
        private LabelPrinter _objLabelPrinter = null; // Label printer object

        //  Variables
        private string _strMacAddr = "";      // mac addr
        private string _strPrinterName = "";   // printer name

        //  Constants
        public const string JSON_FILE = "printer_profiles.JSON";

        // error event
        public delegate void ErrorCallbackHandler(UInt64 handle, Int32 objLinePrinteraram, string msg);
        public event ErrorCallbackHandler ErrorCallbackHandlerEx = null;
        public MainPage()
        {
            this.InitializeComponent();

            
            // We use the system tray at the top of the screen to dusplay error messages and user hints
            // The resource manager uses the ProjectStrings resource file to retrieve the strings to be displayed
            rm = ResourceLoader.GetForCurrentView("StringLibrary");
            updateStatusBar(rm.GetString("BT_CONNECT"));
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        

        private async void updateStatusBar(string resourceString)
        {
            if (null != rm)
                prog.ProgressIndicator.Text = resourceString;
            prog.ProgressIndicator.ProgressValue = 0;
            await prog.ProgressIndicator.ShowAsync();          

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        /// <summary>
        /// Compares the printer selected to all of the paired devices.
        /// Returns true if the selected printer is in the paired devices list
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        private async Task<bool> getDevices(string printerName)
        {
            try
            {
                bool retVal = false;
                printerName = printerName.Substring(0, 4);
                PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                var peerList = await PeerFinder.FindAllPeersAsync();
                if (peerList.Count > 0)
                {
                    for (int i = 0; i < peerList.Count; i++)
                    {                        
                        if (peerList[i].DisplayName.Contains(printerName))
                        {
                            String sBTMAC = peerList[i].HostName.ToString();
                            sBTMAC = sBTMAC.Replace(":", "");
                            sBTMAC = sBTMAC.Replace("(", "");
                            sBTMAC = sBTMAC.Replace(")", "");
                            _strMacAddr = sBTMAC;
                            retVal = true;
                            break;
                        }
                    }
                    return retVal;
                }
                else return retVal;
            }
            catch(Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// Update the srtPrinterName varalble with the name of the selected printer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printerNameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: Make changes so that the selection the selection uses the Peer imformation to ger the printers host name and makes sure that the proter is
            //      paired. Similar to my print server and Josefs app
            if (_spOld != null)
                _spOld.Background = new SolidColorBrush(Colors.Transparent);
            StackPanel x = (StackPanel)printerNameList.SelectedItem;
            x.Background = new SolidColorBrush(Colors.Blue);
            App._printerName = _strPrinterName = x.Name.ToString();
            _spOld = x;
        }



        /// <summary>
        /// Determines if the selected printer ia a label or receipt printer and instantiates the appropriate printer object
        /// </summary>
        private void CreatePrinterObject()
        {
            try
            {
                if (_strPrinterName != "")
                {
                    int nRet = -1;
                    if (null == rm)
                        return;
                    if (_strPrinterName.ToLower().Contains(rm.GetString("FP").ToLower()) == true)
                    {
                        _objLabelPrinter = new LabelPrinter(JSON_FILE, _strPrinterName, _strMacAddr.ToUpper());
                        nRet = _objLabelPrinter.GetPrintHandle();
                        if (nRet == 0)
                        {
                            _objLabelPrinter.RegisterProgressEvent(progressStatus);
                            _objLabelPrinter.RegisterErrorEvent(ErrorStatus);
                            App.LabelPrinterObject = _objLabelPrinter;
                        }
                    }
                    else
                    {
                        _objLinePrinter = new LinePrinter(JSON_FILE, _strPrinterName, _strMacAddr.ToUpper());
                        nRet = _objLinePrinter.GetPrintHandle();
                        if (nRet == 0)
                        {
                            _objLinePrinter.RegisterProgressEvent(progressStatus);
                            _objLinePrinter.RegisterErrorEvent(ErrorStatus);
                            App.LinePrinterObject = _objLinePrinter;
                        }
                        else
                        {
                            switch (nRet)
                            {
                                case (int)PrinterError.ERROR_FILE_NOTFOUND:
                                    if (null != rm)
                                        updateStatusBar(rm.GetString("ERR_JSON_FILE"));
                                    break;
                                case (int)PrinterError.ERROR_INVALID_MACADDRESS:
                                    if (null != rm)
                                        updateStatusBar(rm.GetString("ERR_PAIRING"));
                                    break;
                                case (int)PrinterError.ERROR_INVALID_PRINTER_ID:
                                    if (null != rm)
                                        updateStatusBar(rm.GetString("ERR_PRNTR_ID"));
                                    break;
                                default:
                                    if (null != rm)
                                        updateStatusBar(rm.GetString("ERR_INVALID_PARAM"));
                                    break;
                            }
                            Connect_Disconnect.Content = "Connect";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (null != rm)
                    updateStatusBar(rm.GetString("ERR_INVALID_PARAM"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConnectToPrinter()
        {
            int nRet = -1;
            try
            {
                if (null != _objLinePrinter)
                {
                    nRet = _objLinePrinter.Connect();
                }
                else if (null != _objLabelPrinter)
                {
                    nRet = _objLabelPrinter.Connect();
                }
                if (nRet == 0)
                {
                    if (null != rm)
                    {
                        updateStatusBar(rm.GetString("CONNECT_SUCCESS"));
                    }
                }
                else
                {
                    if (null != rm)
                        updateStatusBar(rm.GetString("CONNECT_UNSUCCESS"));
                }
            }
            catch (Exception ex)
            {
                updateStatusBar(ex.Message);
            }
        }

        public void progressStatus(UInt64 handle, Int32 progStatus)
        {
            if (!UpdateStatusUI(progStatus))
            {
                switch (progStatus)
                {
                    case (int)PrinterProgress.MSG_CANCEL:
                        updateStatusBar(rm.GetString("PROGRESS_CANCEL_MSG"));
                        break;
                    case (int)PrinterProgress.MSG_COMPLETE:
                        updateStatusBar(rm.GetString("PROGRESS_COMPLETE_MSG"));
                        break;
                    case (int)PrinterProgress.MSG_ENDDOC:
                        updateStatusBar(rm.GetString("PROGRESS_ENDDOC_MSG"));
                        break;
                    case (int)PrinterProgress.MSG_FINISHED:
                        updateStatusBar(rm.GetString("PROGRESS_FINISHED_MSG"));
                        break;
                    case (int)PrinterProgress.MSG_STARTDOC:
                        {
                            if (null != rm)
                                updateStatusBar(rm.GetString("CONNECT_SUCCESS"));

                        }
                        break;
                    default:
                        updateStatusBar(rm.GetString("PROGRESS_NONE_MSG"));
                        break;
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// ErrorStatus
        /// </summary>
        /// <remarks>
        ///  event - updates Error status of printing process.
        /// <param name="handle">handle</param>
        /// <param name="Errcode">error code</param>
        /// <param name="msg">error msg</param>
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        public void ErrorStatus(UInt64 handle, Int32 Errcode, string msg)
        {
            try
            {
                if (Errcode <= 6)
                {
                    switch (Errcode)
                    {
                        case 0:
                            if (rm != null)
                                updateStatusBar(rm.GetString("CONNECT_SUCCESS"));
                            break;
                        case 1:
                            if (rm != null)
                                updateStatusBar(rm.GetString("ERR_JASON_INVALID"));
                            break;
                        case 2:
                            if (rm != null)
                                updateStatusBar(rm.GetString("ERR_JSON_FILE"));
                            break;
                        case 3:
                            if (rm != null)
                                updateStatusBar(rm.GetString("ERR_PAIRING"));
                            break;
                        case 4:
                            if (rm != null)
                                updateStatusBar(rm.GetString("ERR_PRINTER_ID"));
                            break;
                        case 5:
                            if (rm != null)
                                updateStatusBar(rm.GetString("ERR_NO_CONN"));
                            break;
                        case 6:
                            if (rm != null)
                                updateStatusBar(rm.GetString("ERR_CONN_FAILED"));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                updateStatusBar(ex.Message);
                //  throw;
            }
        }


        public bool UpdateStatusUI(Int32 progStatus)
        {
            string currentPage = (Window.Current.Content as Frame).Name;
            if (null == rm)
                return false;
            if (currentPage.Contains(rm.GetString("LBL_TITLE")) && App.objPrintLbl != null)
            {
                //App.objPrintLbl.setStatus(progStatus);
                return true;
            }
            else if (currentPage.Contains(rm.GetString("RECPT_TITLE")) && App.objPrintRecpt != null)
            {
                //App.objPrintRecpt.setStatus(progStatus);
                return true;
            }
            return false;
        }

        private async void Connect_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if ((string)Connect_Disconnect.Content == "Connect")
            {
                App.AssignMainObject(this);
                Connect_Disconnect.Content = "Disconnect";
                if (null != rm)
                    updateStatusBar(rm.GetString("CONNECT_STATUS"));
                ErrorCallbackHandlerEx = new ErrorCallbackHandler(ErrorStatus);
                //  Check to see if the printer is paired. If paired do not display the pairing wizard.
                bool devVal = await getDevices(_strPrinterName);
                if(!devVal)
                {
                    if (null != rm)
                    {
                        updateStatusBar(rm.GetString("PTR_NOT_PAIRED"));
                    }
                }
                CreatePrinterObject();
                ConnectToPrinter();
                //  If the printer is a label printer goto the label page, else go to the line print page
                if (_strPrinterName.ToLower().Contains(rm.GetString("FP").ToLower()) == true)
                {
                    try
                    {
                        if (null != rm)
                        {
                            Frame.Navigate(typeof(PrintLabel));
                            updateStatusBar("");
                        }
                    }
                    catch (Exception)
                    {
                        if (null != rm)
                            updateStatusBar(rm.GetString("INVALID_ERR"));
                    }

                }
                else
                {
                    try
                    {
                        if (null != rm)
                        {
                            Frame.Navigate(typeof(PrintReceipt));                  
                            updateStatusBar("");
                        }
                    }
                    catch (Exception)
                    {
                        if (null != rm)
                            updateStatusBar(rm.GetString("INVALID_ERR"));
                    }
                }
            }
            else
            {
                Connect_Disconnect.Content = "Connect";
                try
                {
                    if (null != _objLinePrinter)
                    {
                        _objLinePrinter.Flush();
                        if (_objLinePrinter.Disconnect() == 0)
                        {
                            if (null != rm)
                                updateStatusBar(rm.GetString("DISCONNECT_SUCCESS"));
                        }
                    }
                    else if (null != _objLabelPrinter)
                    {
                        if (_objLabelPrinter.Disconnect() == 0)
                        {
                            if (null != rm)
                                updateStatusBar(rm.GetString("DISCONNECT_SUCCESS"));
                        }
                    }
                }
                catch (Exception)
                {
                    if (null != rm)
                        updateStatusBar(rm.GetString("INVALID_ERR"));
                }
            }
        }
    }
}

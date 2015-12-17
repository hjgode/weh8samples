using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HSM.Mobility.Printing;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RT_PrintDemoII
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PrintReceipt : Page
    {
        private string _strOptional = string.Empty;
        private ResourceLoader rm;
        string sDocNumber = "123456789012";
        private StatusBar prog = StatusBar.GetForCurrentView();

        public PrintReceipt()
        {
            this.InitializeComponent();
            rm = ResourceLoader.GetForCurrentView("StringLibrary");
            
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
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                updateStatusBar(rm.GetString("RECPT_PRINT_STATUS"));
                Task.Delay(1000);
                switch (App._printerName)
                {
                    case "PR2":
                    case "PB21":
                        GenerateReciept40();
                        break;
                    case "PR3":
                    case "PB31":
                        GenerateReceipt57();
                        break;
                    case "PB42":
                    case "PB51":
                        GenerateReceipt80();
                        break;
                }
            }
            catch (Exception)
            {
                updateStatusBar(rm.GetString("RECPT_ERROR_STATUS"));
            }
        }

        private bool GenerateReciept40() // 2in receipt printers PR2, PB21
        {
            try
            {
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("    ROUTE# 123456  SALESREP# 54321");

                // Set font style to Bold + Double Wide + Double High.
                App.LinePrinterObject.SetBold(true);
                App.LinePrinterObject.SetDoubleWide(true);
                App.LinePrinterObject.SetDoubleHigh(true);
                App.LinePrinterObject.Write("SALES ORDER");
                App.LinePrinterObject.SetDoubleWide(false);
                App.LinePrinterObject.SetDoubleHigh(false);
                App.LinePrinterObject.NewLine(2);

                // The following text shall be printed in Bold font style.
                App.LinePrinterObject.Write("CUSTOMER: Casual Step");
                App.LinePrinterObject.SetBold(false);  // Returns to normal font.
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("          Distributing Co.");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("          550 2nd Street S.E.");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("          Cedar Rapids, IA 52401-1234");
                App.LinePrinterObject.NewLine(2);


                // Set font style to Compressed + Double High.
                App.LinePrinterObject.SetDoubleHigh(true);
                App.LinePrinterObject.SetCompress(true);
                App.LinePrinterObject.Write("DOCUMENT#: " + sDocNumber);
                App.LinePrinterObject.SetCompress(false);
                App.LinePrinterObject.SetDoubleHigh(false);
                App.LinePrinterObject.Write("  TAX ID# AB4001");
                App.LinePrinterObject.NewLine(2);

                // The following text shall be printed in Normal font style.
                App.LinePrinterObject.Write(" PRD. DESCRIPT.   PRC.  QTY.    NET.");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.Write(" 1501 Timer-Md1  13.15     1   13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write(" 1502 Timer-Md2  13.15     3   39.45");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write(" 1503 Timer-Md3  13.15     2   26.30");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write(" 1504 Timer-Md4  13.15     4   52.60");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write(" 1505 Timer-Md5  13.15     5   65.75");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                        ----  ------");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("              SUBTOTAL    15  197.25");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.Write("          5% State Tax          9.86");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.Write("                              ------");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("           BALANCE DUE        207.11");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.NewLine(1);

                App.LinePrinterObject.Write(" PAYMENT TYPE: CASH");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.SetDoubleHigh(true);
                App.LinePrinterObject.Write("       SIGNATURE / STORE STAMP");
                App.LinePrinterObject.SetDoubleHigh(false);
                App.LinePrinterObject.NewLine(2);
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.SetBold(true);
                if (_strOptional != "")
                {
                    // Print the text entered by user in the Optional Text field.
                    App.LinePrinterObject.Write(_strOptional);
                    App.LinePrinterObject.NewLine(2);
                }
                App.LinePrinterObject.Write("          ORIGINAL");
                App.LinePrinterObject.SetBold(false);
                App.LinePrinterObject.NewLine(2);

                // Print a Code 39 barcode containing the document number.
                App.LinePrinterObject.WriteBarcode(BarcodeType.BC_CODE39,
                        sDocNumber,   // Document# to encode in barcode
                        90,           // Desired height of the barcode in printhead dots
                        40);          // Offset in printhead dots from the left of the page

                App.LinePrinterObject.NewLine(3);
                App.LinePrinterObject.Write("            Thank You For");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("            Your Business");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("     Comments or Questions, Please");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("      Call (555) 555-1234 M-F 7-5");

                App.LinePrinterObject.NewLine(4);
                return true;
            }
            catch (Exception)
            {
                updateStatusBar(rm.GetString("RECPT_ERROR_STATUS"));
            }
            return false;
        }

        private bool GenerateReceipt57()
        {
            try
            {
                App.LinePrinterObject.Write("LOCATION# 123456 REP# 123456 " + DateTime.Now.ToString());
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.SetBold(true);
                App.LinePrinterObject.Write("                      Casual Step");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                    Distributing Co.");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.SetBold(false);
                App.LinePrinterObject.Write("                  550 2nd Street S.E.");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("              Cedar Rapids, IA 52401-1234");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.SetBold(true);
                App.LinePrinterObject.Write("CUSTOMER#   123456   Grant Adams   ");
                App.LinePrinterObject.Write("DOCUMENT#: " + sDocNumber);
                App.LinePrinterObject.SetBold(false);

                App.LinePrinterObject.Write("START TIME: 09:45    Box 123              TAX ID# AB4001");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("END TIME: 09:46    Tampa, FL");
                App.LinePrinterObject.NewLine(3);

                App.LinePrinterObject.Write("Part#       DESCRIPTION   QTY   PRICE DEPOSIT         NET");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.SetBold(true);
                App.LinePrinterObject.Write("                            SALES");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.SetBold(false);
                App.LinePrinterObject.NewLine(1);

                App.LinePrinterObject.Write("1501     Timer Module 1     1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1502     Timer Module 2     3   11.95    1.20       39.45");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1503     Timer Module 3     1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1504     Timer Module 4     4   11.95    1.20       52.60");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1505     Timer Module 5     1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1506     Timer Module 6     1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1507     Timer Module 7     1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1508     Timer Module 8     1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1509     Timer Module 9     1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);

                App.LinePrinterObject.Write("                          ____                    _______");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("    SUBTOTAL               15                      197.25");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.Write("1501     Plug-in Module 1   1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1502     Plug-in Module 2   3   11.95    1.20       39.45");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1503     Plug-in Module 3   1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1504     Plug-in Module 4   4   11.95    1.20       52.60");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1505     Plug-in Module 5   2   11.95    1.20       26.30");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1506     Plug-in Module 6   1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1507     Plug-in Module 7   1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("1508     Plug-in Module 8   1   11.95    1.20       13.15");
                App.LinePrinterObject.NewLine(1);

                App.LinePrinterObject.Write("                         ____                     _______");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   SUBTOTAL                15                      197.25");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                         ____                     _______");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   TOTAL SALES             30                      394.50");
                App.LinePrinterObject.NewLine(6);


                App.LinePrinterObject.Write("                   PRODUCT                         394.50");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                   DEPOSIT                          36.00");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                                  _______");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                   SUBTOTAL                        394.50");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                   SUBTOTAL CREDITS                  0.00");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                   OTHER ITEMS                       2.19");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                   LABOR                             4.44");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                   5% State Tax                     19.72");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                                  _______");
                App.LinePrinterObject.NewLine(1);

                App.LinePrinterObject.Write("                  BALANCE DUE: CHARGE              420.81");
                App.LinePrinterObject.NewLine(3);

                App.LinePrinterObject.SetDoubleHigh(true);
                App.LinePrinterObject.Write("                SIGNATURE / STORE STAMP");
                App.LinePrinterObject.SetDoubleHigh(false);
                App.LinePrinterObject.NewLine(3);
                App.LinePrinterObject.SetBold(true);
                if (_strOptional != "")
                {
                    // Print the text entered by user in the Optional Text field.
                    App.LinePrinterObject.Write(_strOptional);
                    App.LinePrinterObject.NewLine(2);
                }
                App.LinePrinterObject.SetBold(false);
                App.LinePrinterObject.NewLine(3);

                App.LinePrinterObject.Write("                    Thank You For");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                    Your Business");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("             Comments or Questions, Please");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("              Call (555) 555-1234 M-F 7-5");
                App.LinePrinterObject.NewLine(4);
                App.LinePrinterObject.Flush();
                return true;
            }
            catch
            {
                updateStatusBar(rm.GetString("RECPT_ERROR_STATUS"));
            }
            return false;
        }

        private bool GenerateReceipt80()
        {
            try
            {
                App.LinePrinterObject.Write("LOCATION# 123456 SERVICE # 1233456 SERVICE REP# 123456 " + DateTime.Now.ToString());
                App.LinePrinterObject.NewLine(1);

                App.LinePrinterObject.SetBold(true);
                App.LinePrinterObject.Write("                                  Casual Step");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                Distributing Co.");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.SetBold(false);
                App.LinePrinterObject.Write("                               550 2nd Street S.E.");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                             Cedar Rapids, IA 52401-1234");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.Write("CUSTOMER#   123456   Grant Adams                        ");
                App.LinePrinterObject.SetBold(true);
                App.LinePrinterObject.Write("DOCUMENT# 123456789012");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.SetBold(false);

                App.LinePrinterObject.Write("START TIME: 09:45    Box 123                            TAX ID# AB4001");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("END TIME:   09:46    Tampa, FL");
                App.LinePrinterObject.NewLine(3);

                App.LinePrinterObject.Write("  Part#               DESCRIPTION         QTY    PRICE  DEPOSIT         NET");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.SetBold(true);
                App.LinePrinterObject.Write("                                         SALES");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.SetBold(false);
                App.LinePrinterObject.NewLine(1);

                App.LinePrinterObject.Write("   1501               Timer Module 1        1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1502               Timer Module 2        3    11.95    1.20          39.45");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1503               Timer Module 3        1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1504               Timer Module 4        4    11.95    1.20          52.60");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1505               Timer Module 5        1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1506               Timer Module 6        1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1507               Timer Module 7        1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1508               Timer Module 8        1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1509               Timer Module 9        1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);

                App.LinePrinterObject.Write("                                         ____                    _____________");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("             SUBTOTAL                      15                          197.25");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1501               Plug-in Module 1      1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1502               Plug-in Module 2      3    11.95    1.20          39.45");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1503               Plug-in Module 3      1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1504               Plug-in Module 4      4    11.95    1.20          52.60");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1505               Plug-in Module 5      2    11.95    1.20          26.30");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1506               Plug-in Module 6      1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1507               Plug-in Module 7      1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("   1508               Plug-in Module 8      1    11.95    1.20          13.15");
                App.LinePrinterObject.NewLine(1);

                App.LinePrinterObject.Write("                                         ____                    _____________");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("             SUBTOTAL                      15                          197.25");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                         ____                    _____________");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("             TOTAL SALES                   30                          394.50");
                App.LinePrinterObject.NewLine(6);


                App.LinePrinterObject.Write("                                    PRODUCT                            394.50");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                    DEPOSIT                             36.00");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                                                     ________");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                    SUBTOTAL                           394.50");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                    SUBTOTAL CREDITS                     0.00");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                    OTHER ITEMS                          2.19");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                    LABOR                                4.44");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                    5% State Tax                        19.72");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                                                    _________");
                App.LinePrinterObject.NewLine(1);

                App.LinePrinterObject.Write("                                    BALANCE DUE: CHARGE                 420.81");
                App.LinePrinterObject.NewLine(3);

                App.LinePrinterObject.SetDoubleHigh(true);
                App.LinePrinterObject.Write("                               SIGNATURE / STORE STAMP");
                App.LinePrinterObject.SetDoubleHigh(false);
                App.LinePrinterObject.NewLine(3);
                App.LinePrinterObject.SetBold(true);
                if (_strOptional != "")
                {
                    // Print the text entered by user in the Optional Text field.
                    App.LinePrinterObject.Write(_strOptional);
                    App.LinePrinterObject.NewLine(2);
                }
                App.LinePrinterObject.SetBold(false); ;
                App.LinePrinterObject.NewLine(3);

                App.LinePrinterObject.Write("                                   Thank You For");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                                   Your Business");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                            Comments or Questions, Please");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                             Call (555) 555-1234 M-F 7-5");
                App.LinePrinterObject.NewLine(1);
                return true;
            }
            catch
            {
                updateStatusBar(rm.GetString("RECPT_ERROR_STATUS"));
            }
            return false;
        }

        public void setStatus(Int32 Errcode)
        {
            if (Errcode == (int)PrinterProgress.MSG_COMPLETE)
            {
                if (null != rm)
                    updateStatusBar(rm.GetString("RECPT_PRINT_SUCCESS"));
            }
            else
            {
                if (null != rm)
                    updateStatusBar(rm.GetString("RECPT_ERROR_STATUS"));
            }
        }

        private void DriverComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            _strOptional = DriverComment.Text;
        }

        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }


    }
}

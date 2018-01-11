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
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;
using System.Xml;
using System.Text;
using HtmlAgilityPack;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HtmlTransformer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string rawCode;
        public static bool isValidHtml = false;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ButtonGetHtml_Click(object sender, RoutedEventArgs e)
        {
            DataPackageView clipboardContent = Clipboard.GetContent();
            if (clipboardContent.Contains(StandardDataFormats.Text))
            {
                rawCode = await clipboardContent.GetTextAsync();
                var rawDocument = new HtmlDocument();
                rawDocument.LoadHtml(rawCode);
                var baseNode = rawDocument.DocumentNode.Element("div");

                if (baseNode.GetAttributeValue("class", "").Contains("WB_cardwrap"))
                {
                    var weiboPost = new WeiboPostParser(baseNode);
                    TextBoxMarkdownCode.Document.SetText(Windows.UI.Text.TextSetOptions.None, weiboPost.GetJekyllCode());
                }
                else
                {
                    MessageDialog dlgInvalidText = new MessageDialog("The text in system clipboard is invalid.", "Notice");
                    await dlgInvalidText.ShowAsync();
                }
            }
            else
            {
                MessageDialog dlgNoText = new MessageDialog("No text in system clipboard.", "Notice");
                await dlgNoText.ShowAsync();
            }
        }
    }
}

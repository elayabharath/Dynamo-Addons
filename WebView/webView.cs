using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.UI;
using System.Xml;

using ProtoCore.AST.AssociativeAST;

namespace Dynamo.Nodes
{
    [NodeName("WebBrowser")]
    [NodeCategory(BuiltinNodeCategories.CORE_VIEW)]
    [NodeDescription("Shows the webpage when given an address.")]
    [IsDesignScriptCompatible]
    public class webView: NodeModel, IWpfNode
    {
        private System.Windows.Controls.WebBrowser browser;

        public webView(WorkspaceModel ws): base(ws)
        {
            InPortData.Add(new PortData(">", "Enter any website that you want to browse e.g. www.google.com.sg/search?q=dynamobim", "http://dynamobim.org"));
            OutPortData.Add(new PortData(">", "Nothing to do"));

            RegisterAllPorts();
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            yield return AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), inputAstNodes[0]);
        }

        public void SetupCustomUIElements(dynNodeView nodeUi)
        {
            browser = new System.Windows.Controls.WebBrowser();
            browser.Height = 300;
            browser.Width = 300;

            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != "IsUpdated") return;
                var im = GetImageFromMirror();
                nodeUi.Dispatcher.Invoke(new Action<WebBrowser>(SetImageSource), new object[] { im });
            };

            nodeUi.PresentationGrid.Children.Add(browser);
            nodeUi.PresentationGrid.Visibility = Visibility.Visible;
        }
        private System.Windows.Forms.WebBrowser GetImageFromMirror()
        {
            if (this.InPorts[0].Connectors.Count == 0) return null;

            var data = this.CachedValue;

            if (data == null || data.IsNull) return null;
            if (data.Data is System.Windows.Forms.WebBrowser) return data.Data as System.Windows.Forms.WebBrowser;
            return null;
        }
        private void SetImageSource(System.Windows.Controls.WebBrowser bmp)
        {
            if (bmp == null)
                return;

            // how to convert a bitmap to an imagesource http://blog.laranjee.com/how-to-convert-winforms-bitmap-to-wpf-imagesource/ 
            // TODO - watch out for memory leaks using system.drawing.bitmaps in managed code, see here http://social.msdn.microsoft.com/Forums/en/csharpgeneral/thread/4e213af5-d546-4cc1-a8f0-462720e5fcde
            // need to call Dispose manually somewhere, or perhaps use a WPF native structure instead of bitmap?

            //bmp.Navigate("http://google.com");
            //var imageSource = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
            //image.Source = imageSource;
            browser.Navigate("http://google.com");
        }

#if ENABLE_DYNAMO_SCHEDULER

        protected override void RequestVisualUpdateAsyncCore(int maxTesselationDivisions)
        {
            return; // No visualization update is required for this node type.
        }



#endif

        protected override bool ShouldDisplayPreviewCore()
        {
            return false; // Previews are not shown for this node type.
        }
    }
}

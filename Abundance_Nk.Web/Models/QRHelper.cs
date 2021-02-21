using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;

namespace Abundance_Nk.Web.Models
{
    public static class QRHelper
    {
        public static IHtmlString GenerateQrCode(this HtmlHelper html,string url,string alt = "QR code",
            int height = 100,int width = 100,int margin = 0)
        {
            var qrWriter = new BarcodeWriter();
            qrWriter.Format = BarcodeFormat.QR_CODE;
            qrWriter.Options = new EncodingOptions { Height = height,Width = width,Margin = margin };

            using(Bitmap q = qrWriter.Write(url))
            {
                using(var ms = new MemoryStream())
                {
                    q.Save(ms,ImageFormat.Png);
                    var img = new TagBuilder("img");
                    img.Attributes.Add("src",
                        String.Format("data:image/png;base64,{0}",Convert.ToBase64String(ms.ToArray())));
                    img.Attributes.Add("alt",alt);
                    return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
                }
            }
        }
    }
}
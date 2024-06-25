using System;
using System.Collections.Generic;
using Sample.WebApp.SmartCAv4.Models;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using CenIT.Plugins.VNPT.SmartCAv4.Providers;
using CenIT.Plugins.VNPT.SmartCAv4.Enums;
using CenIT.Plugins.VNPT.SmartCAv4.Models;
using VnptHashSignatures.Pdf;

namespace Sample.WebApp.SmartCAv4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SmartCA(string errMsg)
        {
            ViewBag.Message = "Test sign document via VNPT SmartCA.";

            return View(new SignDocModel { ErrMsg = errMsg });
        }

        [HttpPost]
        public ActionResult SignDoc(SignDocModel signDoc)
        {
            if (signDoc.RefDoc == null)
            {
                return RedirectToAction("SmartCA", new { errMsg = "Tệp đính kèm rỗng" });
            }

            var serviceUrl = "https://gwsca.vnpt.vn/sca/sp769";
            var appId = "4540-637891593888578338.apps.smartcaapi.com";
            var appSecret = "MjAzYjZhMWY-NzRiZS00NTQw";
            var userName = "056090002082";

            var absolutePathLogs = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Logs");
            if (!Directory.Exists(absolutePathLogs))
            {
                Directory.CreateDirectory(absolutePathLogs);
            }
            MemoryStream fileStream = new MemoryStream();
            signDoc.RefDoc.InputStream.CopyTo(fileStream);

            var dataUnsign = fileStream.ToArray();

            var smartCAProvider = new VNPTSmartCAProvider(serviceUrl, appId, appSecret).SetPathFolderLog(absolutePathLogs);
            byte[] dataSigned = null;
            if (Path.GetExtension(signDoc.RefDoc.FileName) == ".pdf")
            {
                dataSigned = smartCAProvider.SignPdf(userName, string.Empty, $"Test ký file {signDoc.RefDoc.FileName}", signDoc.RefDoc.FileName, dataUnsign, new PdfOptionalSignatureModel
                {
                    RenderingMode = PdfHashSigner.RenderMode.TEXT_WITH_LOGO_TOP,
                    SignatureStyle = new SignatureFont
                    {
                        FontColor = "2c86d1",
                        FontName = PdfHashSigner.FontName.Times_New_Roman,
                        FontSize = 12,
                        FontStyle = PdfHashSigner.FontStyle.BoldItalic
                    },
                    SignatureImage = System.IO.File.ReadAllBytes(Server.MapPath("/Imgs/avartar.jpg")),
                    SignatureInfo = "Lương Công Trung",
                    SignatureViews = new List<PdfSignatureView>
                    {
                        new PdfSignatureView
                        {
                            Page = 3, Rectangle = "350,400,500,500"
                        }
                    }
                });
            }
            else if (Path.GetExtension(signDoc.RefDoc.FileName) == ".xml")
            {
            }
            else if (Path.GetExtension(signDoc.RefDoc.FileName) == ".doc" || Path.GetExtension(signDoc.RefDoc.FileName) == ".docx")
            {
                dataSigned = smartCAProvider.SignOffice(userName, string.Empty, $"Test ký file {signDoc.RefDoc.FileName}", signDoc.RefDoc.FileName, dataUnsign, EnumFileType.Word);
            }
            else if (Path.GetExtension(signDoc.RefDoc.FileName) == ".xls" || Path.GetExtension(signDoc.RefDoc.FileName) == ".xlsx")
            {
                dataSigned = smartCAProvider.SignOffice(userName, string.Empty, $"Test ký file {signDoc.RefDoc.FileName}", signDoc.RefDoc.FileName, dataUnsign, EnumFileType.Excel);
            }

            if (dataSigned != null)
            {
                return File(dataSigned, signDoc.RefDoc.ContentType, $"signed_{DateTime.Now:ddHHmmss}_{signDoc.RefDoc.FileName}");
            }

            return RedirectToAction("SmartCA", new { errMsg = "Ký thất bại" });
        }
    }
}
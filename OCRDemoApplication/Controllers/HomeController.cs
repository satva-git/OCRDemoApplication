using Newtonsoft.Json;
using OCRDemoApplication.Models;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using PdfSharp.Pdf.IO;

namespace OCRDemoApplication.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ViewFileData(string fileName)
        {
            try
            {
                ITextractService _TextractService = new TextractService();
                fileName = sampleFileFolderPath + fileName;
                ApiResult apiResult = new ApiResult();
                await _TextractService.SaveFileToBucketAndGetResponse(fileName, apiResult);
                apiResult.DataDictionary = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>
                    (JsonConvert.SerializeObject(apiResult.Data));
                return View(apiResult);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex;
                throw ex;
            }
        }

        public FileResult FillPDFForm(string fileName)
        {
            try
            {
                string templateDocPath = sampleFileFolderPath + fileName;
                PdfDocument pdfDocument = PdfReader.Open(templateDocPath, PdfDocumentOpenMode.Modify);

                FillFields(pdfDocument);

                var newFileName = GetUniqueFileName() + ".pdf";
                pdfDocument.Save(sampleFileFolderPath + newFileName);

                byte[] newFileBytes = GetBytes(newFileName);

                DeleteFile(newFileName);

                return File(newFileBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex;
                throw ex;
            }
        }

        public ActionResult SplitPDF(string fileName)
        {
            try
            {
                string filename = sampleFileFolderPath + fileName;
                PdfDocument pdfDocument = PdfReader.Open(filename, PdfDocumentOpenMode.Import);

                PdfDocument outputDocument = new PdfDocument();
                outputDocument.Version = pdfDocument.Version;
                outputDocument.Info.Title = "Splitted_PDF_" + GetUniqueFileName();
                outputDocument.Info.Creator = pdfDocument.Info.Creator;
                outputDocument.AddPage(pdfDocument.Pages[0]);

                var newFileName = "Splitted_PDF_" + GetUniqueFileName() + ".pdf";
                outputDocument.Save(sampleFileFolderPath + newFileName);

                byte[] newFileBytes = GetBytes(newFileName);
                DeleteFile(newFileName);

                return File(newFileBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex;
                throw ex;
            }
        }
    }
}
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
        // Test 2
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ViewFileData(string fileName)
        {
            try
            {
                // test commit
                ITextractService _TextractService = new TextractService();
                ApiResult apiResult = new ApiResult();
                // async Task<ActionResult> - change return type and uncomment the below code to make working

                var incomeStatementResult = SplitPDFPage(fileName, 1);
                await _TextractService.SaveFileToBucketAndGetResponse(sampleFileFolderPath + incomeStatementResult.NewFileName, apiResult);
                apiResult.DataDictionary1 = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>
                   (JsonConvert.SerializeObject(apiResult.Data));
                DeleteFile(incomeStatementResult.NewFileName);

                var balanceSheetResult = SplitPDFPage(fileName, 4);
                await _TextractService.SaveFileToBucketAndGetResponse(sampleFileFolderPath + balanceSheetResult.NewFileName, apiResult);
                apiResult.DataDictionary2 = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>
                   (JsonConvert.SerializeObject(apiResult.Data));
                DeleteFile(balanceSheetResult.NewFileName);

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
                var updatedFileResult = SplitPDFPage(fileName, 1);
                DeleteFile(updatedFileResult.NewFileName);
                return File(updatedFileResult.Bytes, "application/pdf");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex;
                throw ex;
            }
        }

        private UpdatedFileResult SplitPDFPage(string fileName, int pageNo)
        {
            UpdatedFileResult updatedFileResult = new UpdatedFileResult();
            string filename = sampleFileFolderPath + fileName;
            PdfDocument pdfDocument = PdfReader.Open(filename, PdfDocumentOpenMode.Import);

            PdfDocument outputDocument = new PdfDocument();
            outputDocument.Version = pdfDocument.Version;
            outputDocument.Info.Title = "Splitted_PDF_" + GetUniqueFileName();
            outputDocument.Info.Creator = pdfDocument.Info.Creator;
            outputDocument.AddPage(pdfDocument.Pages[pageNo - 1]);

            var newFileName = "Splitted_PDF_" + GetUniqueFileName() + ".pdf";
            outputDocument.Save(sampleFileFolderPath + newFileName);

            updatedFileResult.NewFileName = newFileName;
            updatedFileResult.Bytes = GetBytes(newFileName);

            return updatedFileResult;
        }
    }
}
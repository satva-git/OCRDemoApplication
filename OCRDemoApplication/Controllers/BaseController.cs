using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using System;
using System.IO;
using System.Web.Mvc;

namespace OCRDemoApplication.Controllers
{
    public class BaseController : Controller
    {
        public string sampleFileFolderPath = string.Empty;

        public BaseController()
        {
            sampleFileFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\SampleFiles\\";
        }

        public string GetUniqueFileName()
        {
            return DateTime.Now.Date.Ticks.ToString();
        }

        public void FillFields(PdfDocument pdfDocument)
        {
            PdfAcroForm form = pdfDocument.AcroForm;

            PdfTextField givenName = (PdfTextField)(form.Fields["Given Name Text Box"]);
            givenName.Value = new PdfString("Chintan");

            PdfTextField familyName = (PdfTextField)(form.Fields["Family Name Text Box"]);
            familyName.Value = new PdfString("Prajapati");

            PdfTextField address1Name = (PdfTextField)(form.Fields["Address 1 Text Box"]);
            address1Name.Value = new PdfString("K K Nagar");

            PdfTextField houseName = (PdfTextField)(form.Fields["Address 2 Text Box"]);
            houseName.Value = new PdfString("Nr. Ghatlodia");

            PdfTextField address2Name = (PdfTextField)(form.Fields["House nr Text Box"]);
            address2Name.Value = new PdfString("120B");

            PdfTextField postCodeName = (PdfTextField)(form.Fields["Postcode Text Box"]);
            postCodeName.Value = new PdfString("380007");

            PdfTextField cityName = (PdfTextField)(form.Fields["City Text Box"]);
            cityName.Value = new PdfString("Ahmedabad");

            PdfComboBoxField countyName = (PdfComboBoxField)(form.Fields["Country Combo Box"]);
            countyName.Value = new PdfString("France");

            PdfCheckBoxField drivingLicenseName = (PdfCheckBoxField)(form.Fields["Driving License Check Box"]);
            drivingLicenseName.Checked = true;

            PdfComboBoxField genderName = (PdfComboBoxField)(form.Fields["Gender List Box"]);
            genderName.Value = new PdfString("Male");

            PdfTextField heightName = (PdfTextField)(form.Fields["Height Formatted Field"]);
            heightName.Value = new PdfString("8ft");

            PdfCheckBoxField languageName = (PdfCheckBoxField)(form.Fields["Language 1 Check Box"]);
            languageName.Checked = true;

            PdfComboBoxField colourName = (PdfComboBoxField)(form.Fields["Favourite Colour List Box"]);
            colourName.Value = new PdfString("Black");
        }

        public void DeleteFile(string fileName)
        {
            if (System.IO.File.Exists(sampleFileFolderPath + fileName))
            {
                System.IO.File.Delete(sampleFileFolderPath + fileName);
            }
        }

        public byte[] GetBytes(string newFileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(sampleFileFolderPath + newFileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(sampleFileFolderPath + newFileName).Length;

            buff = br.ReadBytes((int)numBytes);
            fs.Close();
            return buff;
        }
    }
}
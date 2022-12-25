using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.Textract;
using Amazon.Textract.Model;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRDemoApplication.Models
{
    public class TextractService : ITextractService
    {
        private string _bucketName;
        private string _awsSecretAccessKey;
        private string _awsAccessKeyId;
        private RegionEndpoint _regionEndPoint;
        private IAmazonS3 _s3Client;

        public TextractService()
        {
            _bucketName = ConfigVariables.BucketName;
            _regionEndPoint = RegionEndpoint.GetBySystemName(ConfigVariables.SystemName);
            _awsAccessKeyId = ConfigVariables.AwsAccessKeyId;
            _awsSecretAccessKey = ConfigVariables.AwsSecretAccessKey;
            _s3Client = new AmazonS3Client(_awsAccessKeyId, _awsSecretAccessKey, _regionEndPoint);
        }

        public async Task SaveFileToBucketAndGetResponse(string fileName, ApiResult resultPT)
        {
            var keyValuePairFormResponse = new List<KeyValuePair<string, string>>();
            string fileNameTest = SaveFileToBucket(fileName);

            keyValuePairFormResponse = await ScanForForm(fileNameTest);
            resultPT.Data = keyValuePairFormResponse.Count > 0 ? keyValuePairFormResponse : new List<KeyValuePair<string, string>>();
        }

        private string SaveFileToBucket(string fileName)
        {
            TransferUtility utility = new TransferUtility(_s3Client);
            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
            string NewFileName = string.Format("{0}{1}", new[] {
                Guid.NewGuid().ToString(),
                System.IO.Path.GetExtension(fileName)
            });
            request.BucketName = _bucketName;
            request.Key = NewFileName;
            try
            {
                request.InputStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                utility.Upload(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                request.InputStream.Close();
            }
            return NewFileName;
        }

        private async Task<List<KeyValuePair<string, string>>> ScanForForm(string fileName)
        {
            var FormValues = new List<KeyValuePair<string, string>>();
            using (var textractClient = new AmazonTextractClient(_awsAccessKeyId, _awsSecretAccessKey, _regionEndPoint))
            {
                var s3File = new S3Object()
                {
                    Bucket = _bucketName,
                    Name = fileName
                };

                var textractResults = await textractClient.AnalyzeDocumentAsync(new AnalyzeDocumentRequest()
                {
                    Document = new Document()
                    {
                        S3Object = s3File
                    },
                    FeatureTypes = new List<string>() { "FORMS" }

                });
                SetFormValues(FormValues, textractResults);
            }
            return FormValues;
        }

        private static void SetFormValues(List<KeyValuePair<string, string>> FormValues, AnalyzeDocumentResponse textractResults)
        {
            var KeyValueElements = (from x in textractResults.Blocks
                                    where x.BlockType == BlockType.KEY_VALUE_SET
                                    select x).ToArray();

            foreach (var keyValue in KeyValueElements)
            {
                StringBuilder keyName = new StringBuilder();
                StringBuilder valueResult = new StringBuilder();

                var keyIdBlock = (from k in keyValue.Relationships
                                  where k.Type == RelationshipType.CHILD
                                  select k).FirstOrDefault();

                var ValueIdBlock = (from k in keyValue.Relationships
                                    where k.Type == RelationshipType.VALUE
                                    select k).FirstOrDefault();

                if (keyIdBlock != null)
                {

                    foreach (string keyId in keyIdBlock.Ids)
                    {
                        var keyElement = (from k in textractResults.Blocks
                                          where k.Id == keyId
                                          select k).FirstOrDefault();

                        keyName.Append(keyElement.Text + " ");
                    }
                }

                if (ValueIdBlock != null)
                {
                    var valueElement = (from x in textractResults.Blocks
                                        where x.Id == ValueIdBlock.Ids[0]
                                        select x).FirstOrDefault();

                    if (valueElement.Relationships.Count > 0)
                    {
                        foreach (var valuePart in valueElement.Relationships[0].Ids)
                        {
                            var valuePartBlock = (from x in textractResults.Blocks
                                                  where x.Id == valuePart
                                                  select x).FirstOrDefault();
                            valueResult.Append(valuePartBlock.Text + " ");
                        }
                    }
                }

                string formKey = keyName.ToString();
                string forValue = valueResult.ToString();

                if (!string.IsNullOrEmpty(formKey) && !string.IsNullOrEmpty(forValue))
                    FormValues.Add(new KeyValuePair<string, string>(formKey, forValue));
            }
        }
    }
}

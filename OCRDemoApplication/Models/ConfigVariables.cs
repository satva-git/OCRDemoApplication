using System.Configuration;

namespace OCRDemoApplication.Models
{
    public static class ConfigVariables
    {
        public static string BucketName
        {
            get
            {
                return ConfigurationManager.AppSettings["BucketName"];
            }
        }

        public static string SystemName
        {
            get
            {
                return ConfigurationManager.AppSettings["SystemName"];
            }
        }

        public static string AwsAccessKeyId
        {
            get
            {
                return ConfigurationManager.AppSettings["AwsAccessKeyId"];
            }
        }

        public static string AwsSecretAccessKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AwsSecretAccessKey"];
            }
        }
    }
}
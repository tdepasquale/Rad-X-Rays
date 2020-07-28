using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Application.Errors;

namespace Application.BlogPosts
{
    public class S3FileManager
    {
        public string DefaultImageUrl { get; } = "https://takeradxrays.s3.us-east-2.amazonaws.com/Loch-Ness-big.png";
        public DirectoryInfo ImageDirectory { get; }
        public int MaxSizeKB { get; } = 501;
        public string BlogBucketName { get; } = "takeradxrays";

        public async Task<string> TryImageUpload(string imageFile, string oldImageURL)
        {
            var imageURL = "";
            var imageValidationResult = ValidateImageFile(imageFile);
            if (imageValidationResult.IsSuccess == true) imageURL = await UploadImage(imageValidationResult.ImageBytes, oldImageURL, BlogBucketName);
            else imageURL = oldImageURL;

            return imageURL;
        }

        public class ImageValidationResult
        {
            public bool IsSuccess { get; }
            public byte[] ImageBytes { get; }
            public string Extension { get; set; }

            public ImageValidationResult(bool success, byte[] imageBytes, string extension)
            {
                IsSuccess = success;
                ImageBytes = imageBytes;
                Extension = extension;
            }
        }

        public ImageValidationResult ValidateImageFile(string image)
        {
            if (image == null || image == "") return new ImageValidationResult(false, Array.Empty<byte>(), "");

            //check file type stated in base64 string
            var parts = image.Split(',');
            var fileType = parts[0].Split(':')[1];
            fileType = fileType.Split(';')[0];

            if (fileType.ToLower() != "image/jpeg" && fileType != "image/png")
                throw new RestException(HttpStatusCode.BadRequest, new { Post = "Images must be either jpg or png files." });

            //convert base64 string to byte[]
            var base64 = parts[1];
            byte[] imageBytes = Convert.FromBase64String(base64);

            if (imageBytes.Length == 0) return new ImageValidationResult(false, Array.Empty<byte>(), "");

            //check file type with bytes
            var png = new byte[] { 137, 80, 78, 71 };
            var jpeg = new byte[] { 255, 216, 255, 224 };
            byte[] firstFour = imageBytes.Take(4).ToArray();
            if (!firstFour.SequenceEqual(png) && !firstFour.SequenceEqual(jpeg))
                throw new RestException(HttpStatusCode.BadRequest, new { Post = "Images must be either jpg or png files." });

            //check file size
            if (imageBytes.Length > MaxSizeKB * 1000)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Post = $"Images must be less than {MaxSizeKB}kb." });
            }

            return new ImageValidationResult(true, imageBytes, fileType.Split("/")[1]);
        }

        public async Task<string> UploadImage(byte[] imageBytes, string oldImageURL, string bucketName)
        {
            try
            {
                using (IAmazonS3 client = new AmazonS3Client(RegionEndpoint.GetBySystemName("us-east-2")))
                {
                    var newKeyName = Guid.NewGuid().ToString();

                    MemoryStream stream = new MemoryStream(imageBytes);
                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(stream, bucketName, newKeyName);

                    if (oldImageURL != DefaultImageUrl) await DeleteImage(oldImageURL, bucketName);

                    return "https://" + bucketName + ".s3.amazonaws.com/" + newKeyName;
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return string.Empty;
        }

        public async Task DeleteImage(string imageURL, string bucketName)
        {
            if (imageURL == DefaultImageUrl || string.IsNullOrWhiteSpace(imageURL.Trim())) return;

            var pattern = new Regex(@"amazonaws\.com\/(.+)");
            var match = pattern.Match(imageURL);
            var key = match.Groups[1].Value;

            try
            {
                using (IAmazonS3 client = new AmazonS3Client(RegionEndpoint.GetBySystemName("us-east-2")))
                {
                    var deleteObjectRequest = new DeleteObjectRequest
                    {
                        BucketName = bucketName,
                        Key = key
                    };
                    await client.DeleteObjectAsync(deleteObjectRequest);
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message: '{0}' when deleting an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown error encountered on server. Message: '{0}' when deleting an object", e.Message);
            }
        }
    }
}
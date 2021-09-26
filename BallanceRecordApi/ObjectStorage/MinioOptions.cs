namespace BallanceRecordApi.ObjectStorage
{
    public class MinioOptions
    {
        public string Endpoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public bool Ssl { get; set; }
    }
}
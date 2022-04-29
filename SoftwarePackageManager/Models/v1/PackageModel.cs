namespace SoftwarePackageManager.Models.v1
{
    public class PackageModel
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public string  Version { get; set; }
        public string  Status { get; set; }
        public string File { get; set; }
    }

    public enum PackageStatus { 
        CREATED = 0,
        DOWNLOADED = 1,
        ACTIVE = 2
    }
}

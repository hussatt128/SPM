using SoftwarePackageManager.Models.v1;
using System.Collections.Generic;

namespace SoftwarePackageManager.BusinessLogic
{
    public class PackageBL
    {
        public static List<PackageModel> packages = new List<PackageModel>();

        internal static void Populate()
        {
            packages.Add(new PackageModel { Id = 1, Name = "Package 1", Version = "1.0", Status = "created" });
            packages.Add(new PackageModel { Id = 2, Name = "Package 2", Version = "1.0", Status = "created" });
            packages.Add(new PackageModel { Id = 3, Name = "Package 3", Version = "1.0", Status = "created" });
            packages.Add(new PackageModel { Id = 4, Name = "Package 4", Version = "1.0", Status = "created" });
        }
    }
}

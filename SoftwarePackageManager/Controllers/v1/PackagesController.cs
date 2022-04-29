using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SoftwarePackageManager.Models.v1;
using SoftwarePackageManager.Security;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwarePackageManager.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [APIKeyAuth]
    public class PackagesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BackgroundWorkerQueue _backgroundWorkerQueue;

        public PackagesController(IConfiguration configuration, BackgroundWorkerQueue backgroundWorkerQueue)
        {
            // PackageBL.Populate();
            _configuration = configuration;
            _backgroundWorkerQueue = backgroundWorkerQueue;

        }

        #region SQLServer Database

        [HttpGet("GetPackages")]
        public JsonResult GetAllPackages()
        {
            string query = @"
            select Id, Name, Version, Status from dbo.PackageMaster";

            DataTable dt = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("YazzoomDBCon");

            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(dt);
        }


        [HttpPost("CreatePackage")]
        public JsonResult CreatePackage([FromBody]PackageModel packageModel)
        {
            string query = @"
                    insert into dbo.PackageMaster
                    (Name, Version, Status)
                    values(
                    '" + packageModel.Name + @"',
                    '" + packageModel.Version+ @"',
                    'created'
                    )";

            DataTable dt = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("YazzoomDBCon");

            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Package Created Succesfully");
        }


        [HttpPut("UpdatePackage")]
        public JsonResult Put([FromBody]PackageModel packageModel)
        {
            string query = @"
                    update dbo.PackageMaster set 
                     Name = '" + packageModel.Name + @"'
                    ,Version = '" + packageModel.Version + @"'
                    where Id = " + packageModel.Id + @"
                    ";

            DataTable dt = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("YazzoomDBCon");

            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Package updated Succesfully");
        }


        [HttpDelete("DeletePackage/{id}")]
        public JsonResult DeletePackage(int id)
        {
            string query = @"
                    Delete from dbo.PackageMaster 
                    where Id = " + id + @"                    
                    ";

            DataTable dt = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("YazzoomDBCon");

            SqlDataReader myReader;

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Deleted Succesfully");
        }

        // PUT api/<PackagesController>/5
        [HttpPut("Download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            // Call a long running time consuming API.
            // and then change the status to DOWNLOADED.
            await LogRunningApi(id, PackageStatus.DOWNLOADED);
            return Accepted();
        }


        // PUT api/<PackagesController>/5
        [HttpPut("Activate/{id}")]
        public async Task<IActionResult> Activate(int id)
        {
            // Call a long running time consuming API.
            // and then change the status to ACTIVE
            await LogRunningApi(id, PackageStatus.ACTIVE);
            return Accepted();
        }

        #endregion

        private async Task LogRunningApi(int id, PackageStatus packageStatus)
        {
            _backgroundWorkerQueue.QueueBackgroundWorkItem(async token =>
            {
                await Task.Delay(10000); // wait for 10 seconds before updating the status.
                UpdateStatus(id, packageStatus);

            });
        }

        private void UpdateStatus(int id, PackageStatus packageStatus)
        {
            string status = string.Empty;

            switch (packageStatus)
            {
                case PackageStatus.ACTIVE:
                    status = "active";
                    break;
                case PackageStatus.DOWNLOADED:
                    status = "downloaded";
                    break;
            }

            string query = @"
                    update dbo.PackageMaster set 
                     status = '" + status + @"'
                    where Id = " + id + @"";

            DataTable dt = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("YazzoomDBCon");

            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }
        }

        #region PackageBL
        //// GET: api/<PackagesController>
        //[HttpGet]
        //public IEnumerable<PackageModel> Get()
        //{
        //    return PackageBL.packages;
        //}

        //// GET api/<PackagesController>/5
        //[HttpGet("{id}")]
        //public JsonResult Get(int id)
        //{
        //    var item = PackageBL.packages.Where(x => x.Id == id).FirstOrDefault<PackageModel>();

        //    return new JsonResult(item);
        //}

        //// POST api/<PackagesController>
        //[HttpPost]
        //public void Post([FromBody] PackageModel packageModel)
        //{
        //    PackageBL.packages.Add(
        //        new PackageModel { 
        //            Id = packageModel.Id,
        //            Name = packageModel.Name, 
        //            Version = packageModel.Version, 
        //            Status = packageModel.Status 
        //        });
        //}

        //// PUT api/<PackagesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] PackageModel packageModel)
        //{
        //    PackageBL.packages.FirstOrDefault(x => x.Id == id).Name = packageModel.Name;
        //    PackageBL.packages.FirstOrDefault(x => x.Id == id).Status = packageModel.Status;
        //    PackageBL.packages.FirstOrDefault(x => x.Id == id).Version = packageModel.Version;
        //}

        //// PUT api/<PackagesController>/5
        //[HttpPut("Download/{id}")]
        //public async void Download(int id)
        //{
        //    PackageBL.packages.FirstOrDefault(x => x.Id == id).Status = "downloaded";

        //    await PackageBL.DownloadLongTaskAsync(id);
        //}

        //// PUT api/<PackagesController>/5
        //[HttpPut("Activate/{id}")]
        //public async Task ActivateAsync(int id)
        //{
        //    PackageBL.packages.FirstOrDefault(x => x.Id == id).Status = "active";

        //    await PackageBL.ActivateLongTaskAsync(id);

        //}

        //// DELETE api/<PackagesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //    PackageBL.packages.Remove(PackageBL.packages.FirstOrDefault(x => x.Id == id));
        //}

        #endregion

    }

    public class BackgroundWorkerQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new ConcurrentQueue<Func<CancellationToken, Task>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }
    }

    public class LongRunningService : BackgroundService
    {
        private readonly BackgroundWorkerQueue queue;

        public LongRunningService(BackgroundWorkerQueue queue)
        {
            this.queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await queue.DequeueAsync(stoppingToken);

                await workItem(stoppingToken);
            }
        }
    }
}

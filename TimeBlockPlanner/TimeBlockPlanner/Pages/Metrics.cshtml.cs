using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using UserData;
using UserData.Models;

namespace TimeBlockPlanner.Pages
{
    public class MetricsModel : PageModel
    {
        [BindProperty]
        public UserMetricForm UserMetric { get; set; }

        [BindProperty]
        public MetricTimeframeForm MetricTimeframe { get; set; }

        private const string connectionString = @"Server=(localdb)\reaganlocal;Database=rphazell;Integrated Security=SSPI;";
        private IUserMetricRepository UserMetricRepo = new SqlUserMetricRepository(connectionString);
        private IMetricTimeframeRepository MetricTimeframeRepo = new SqlMetricTimeframeRepository(connectionString);
        private IMetricRepository MetricRepositoryRepo = new SqlMetricRepository(connectionString);

        /// <summary>
        /// The metrics being displayed from the server to the user
        /// </summary>
        public IEnumerable<UserMetricFront> UMetrics { get { return UserMetricRepo.RetrieveUserMetricsFront(UserId); } }

        /// <summary>
        /// The metricTimeframes being displayed from the server to the user
        /// </summary>
        public IEnumerable<MetricTimeframe> MetricTimeframes { get { return MetricTimeframeRepo.RetrieveAllMetricTimeframes(); } }

        /// <summary>
        /// Cache Storage to retrieve the userId
        /// </summary>
        private readonly IMemoryCache _cache;

        public int UserId
        {
            get
            {
                int uId;
                _cache.TryGetValue("userId", out uId);
                return uId;
            }
        }

        /// <summary>
        /// Initializes the private Cache field
        /// </summary>
        public MetricsModel(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Method to handle get requests on the page
        /// </summary>
        public void OnGet()
        {
            // Check for the userId to see if the user is logged in.
            int uId;
            _cache.TryGetValue("userId", out uId);
            if (uId == default(int))
            {
                Response.Redirect("SignIn");
            }
            _cache.Get("");
        }

        /// <summary>
        /// Method to handle post requests on the page
        /// </summary>
        public void OnPostMetric(int metricTimeframeId, int metricId, string metricName, string metricTimeframeName, DateTime date, int value)
        {
            Metric m = MetricRepositoryRepo.CreateMetric(metricName, 1);
            MetricTimeframe mtf = MetricTimeframeRepo.GetMetricTimeframeIdGivenName(metricTimeframeName);
            UserMetricRepo.SaveUserMetric(UserId, mtf.MetricTimeframeId, m.MetricId, date, value);
        }
         
        public void OnPostMetricTimeframe()
        {
            if (MetricTimeframe.IsDeleted)
            {
                MetricTimeframeRepo.CreateMetricTimeframe(MetricTimeframe.name, 0);
            }
            else
            {
                MetricTimeframeRepo.CreateMetricTimeframe(MetricTimeframe.name, 1);
            }
        }
    }
}
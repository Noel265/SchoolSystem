using Microsoft.AspNetCore.Mvc;
using SchoolSystem.API.Services;

namespace SchoolSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Obsolete("Functionality moved to GradesController")]
    public class ReportCardsController : ControllerBase
    {
        private readonly ReportCardService _reportCardService;

        public ReportCardsController(ReportCardService reportCardService)
        {
            _reportCardService = reportCardService;
        }
    }
}

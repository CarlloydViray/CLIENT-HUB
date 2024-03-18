using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using BPOI_HUB.model;

namespace BPOI_HUB.modules.apps.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : Controller
    {

        

        // For updating status
        [Route("{id}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateLog(
            [FromRoute] int id,
            [FromQuery] string status,
            [FromQuery] string? title="", 
            [FromQuery] string? body="", 
            [FromQuery] string? requestor_account="", 
            [FromQuery] string? request_date_time="", 
            [FromQuery] string? machine_id="",
            [FromQuery] string? details="", 
            [FromQuery] int? retry_count=0, 
            [FromQuery] string? execution_start_time="", 
            [FromQuery] string? execution_end_time="", 
            [FromQuery] string? client_code="", 
            [FromQuery] string? application_id="",
            [FromQuery] string? system_type=""
            )
        {

            Log request = new()
            {
                Id = id,
                Title = title,
                Body = body,
                RequestorAccount = requestor_account,
                MachineId = machine_id,
                Details = details,
                RetryCount = retry_count.GetValueOrDefault(),
                ClientCode = client_code,
                ApplicationId = application_id,
                SystemType = system_type,
                Status = status

            };

            if (!request_date_time.Equals("")) request.RequestDateTime = DateTime.Parse(request_date_time);

            
            if (!execution_start_time.Equals("")) request.ExecutionStartTime = DateTime.Parse(execution_start_time);
            if (!execution_end_time.Equals("")) request.ExecuttionEndTime = DateTime.Parse(execution_end_time);
            

            Boolean result = request.UpdateStatus();

            if (!result) return BadRequest();
            else return Ok();
        }

        // Create log and get assigned id
        [Route("id")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateLog(
            [FromQuery] string status,
            [FromQuery] string? title = "",
            [FromQuery] string? body = "",
            [FromQuery] string? requestor_account = "",
            [FromQuery] string? request_date_time = "",
            [FromQuery] string? machine_id = "",
            [FromQuery] string? details = "",
            [FromQuery] int? retry_count = 0,
            [FromQuery] string? execution_start_time = "",
            [FromQuery] string? execution_end_time = "",
            [FromQuery] string? client_code = "",
            [FromQuery] string? application_id = "",
            [FromQuery] string? system_type = ""
            )
        {

            Log request = new()
            {
                Title = title,
                Body = body,
                RequestorAccount = requestor_account,
                MachineId = machine_id,
                Details = details,
                RetryCount = retry_count.GetValueOrDefault(),
                ClientCode = client_code,
                ApplicationId = application_id,
                SystemType = system_type,
                Status = status

            };

            if (!request_date_time.Equals("")) request.RequestDateTime = DateTime.Parse(request_date_time);
            if (!execution_start_time.Equals("")) request.ExecutionStartTime = DateTime.Parse(execution_start_time);
            if (!execution_end_time.Equals("")) request.ExecuttionEndTime = DateTime.Parse(execution_end_time);
 

            int result = request.AssignId();

            var data = new { status = StatusCode(200), id = result };
            return new JsonResult(data);
        }

    }
}

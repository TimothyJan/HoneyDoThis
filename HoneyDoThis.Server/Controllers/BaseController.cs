using Microsoft.AspNetCore.Mvc;
using HoneyDoThis.Server.Models;

namespace HoneyDoThis.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "")
        {
            return Ok(ApiResponse<T>.SuccessResponse(data, message));
        }

        protected IActionResult Created<T>(T data, string message = "")
        {
            return StatusCode(201, ApiResponse<T>.SuccessResponse(data, message));
        }

        protected IActionResult Error(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, ApiResponse<object>.ErrorResponse(message));
        }

        protected IActionResult NotFound(string message)
        {
            return base.NotFound(ApiResponse<object>.ErrorResponse(message));
        }

        protected IActionResult BadRequest(string message, List<string>? errors = null)
        {
            return base.BadRequest(ApiResponse<object>.ErrorResponse(message, errors));
        }

        protected IActionResult InternalServerError(string message, Exception? ex = null)
        {
            // Log the exception here if needed
            return StatusCode(500, ApiResponse<object>.ErrorResponse(message));
        }
    }
}
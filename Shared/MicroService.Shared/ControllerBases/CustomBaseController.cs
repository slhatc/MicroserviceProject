using System;
using System.Collections.Generic;
using System.Text;
using MicroService.Shared.Response;
using Microsoft.AspNetCore.Mvc;

namespace MicroService.Shared.ControllerBases
{
    public class CustomBaseController : ControllerBase
    {
        public IActionResult CreateActionResultInstance<T>(Response<T> response)
        {
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}

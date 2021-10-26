using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController] //these are attributes, this attribute helps in building apis, this also 
    // apicontroller is smart enough to understand from where the parameters r coming is from body or querystring
    [Route("api/[controller]")]
    public class BaseAPIController: ControllerBase
    {
        
    }
}
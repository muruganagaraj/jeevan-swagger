using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

using ReferenceApp.Models;

namespace ReferenceApp.Controllers
{
    public sealed class CustomerController : ApiController
    {
        [ActionName("GetCustomers")]
        [Route("api/customer")]
        public Options[] Get(Options option)
        {
            var customers = new[] { new Customer() };
            return new[] { Options.Option1 };
        }
    }

    public enum Options
    {
        Option1 = 4,
        Option2 = 5,
        Option3 = 6
    }
}
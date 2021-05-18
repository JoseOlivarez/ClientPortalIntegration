using ClientPortalApi.Controllers.DataTransferObjects;
using ClientPortalApi.Core;
using ClientPortalApi.Core.Domain;
using ClientPortalApi.Exceptions;
using ClientPortalApi.Helpers;
using ClientPortalApi.Managers;
using ClientPortalApi.Persistence;
using ClientPortalApi.WebFilters;
using NetworkLabelMaker;
using NetworkLabelMaker.Entities;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace ClientPortalApi.Controllers
{
    public class INVOICEMASTsController : ApiController
    {
        private ClientPortalContext db = new ClientPortalContext();
        private readonly IUserHelper _userHelper; 
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ITokenManager _token;
        public INVOICEMASTsController(IUserHelper userHelper, IUnitOfWork unitOfWork, ILoggerFactory loggerFactory, ITokenManager tokenManager)
        {
            _userHelper = userHelper;
            _token = tokenManager;
            _unitOfWork = unitOfWork;
            _logger = loggerFactory.GetLogger(this.GetType().FullName);
        } 
        [HttpGet]
        [Route("api/InvoiceMast/ViewInvoiceHistory")]
        [ValidateModel]
        public async Task<IHttpActionResult> viewInvoiceHistory()
        {
            ClientInfo clientUser;
            clientUser = _userHelper.GetClientUser(User);

            try
            {
                var invoiceHistory = await _unitOfWork.InvoiceHistory.invoicehistory(clientUser.ClientId);
                _logger.Info(CustomLogMessageGenerator.GetLogMessageBase(), "ViewInvoiceHistory", clientUser.UserId, "none", invoiceHistory);

                return Ok(invoiceHistory);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, CustomLogMessageGenerator.GetLogMessageBase(), "ViewInvoiceHistory", clientUser.UserId, "none", "Exception thrown.");

                return InternalServerError();
            }
          
            return BadRequest();
        }

    }
}
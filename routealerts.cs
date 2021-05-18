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
    public class REGIONsController : ApiController
    {
        private ClientPortalContext db = new ClientPortalContext();
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserHelper _userHelper;
        private readonly ITokenManager _tokenManager;
        private readonly ILabelManager _labelManager;
        private readonly ILogger _logger;


        public REGIONsController(IUnitOfWork unitOfWork, IUserHelper userHelper, ILabelManager labelManager, ILoggerFactory loggerFactory, ITokenManager tokenManager)
        {
            _unitOfWork = unitOfWork;
            _userHelper = userHelper;
            _labelManager = labelManager;
            _tokenManager = tokenManager;
            _logger = loggerFactory.GetLogger(this.GetType().FullName); 
        }
    

        [HttpGet]
        [Route("api/Regions1/SelectRegionsS")]
        [ValidateModel]

        public async Task<ClientPortalApi.Core.Domain.REGION> selectRegions()
        {
            ClientInfo clientUser; clientUser = _userHelper.GetClientUser(User);

            //grab client 
            try
            {
                var RouteAlert = await _unitOfWork.RouteAlerts.GetRouteAlerts(clientUser.ClientId);
                if(RouteAlert == null)
                {
                    return null;
                }
                var token = _tokenManager.GenerateToken(clientUser); //need to add customer by account later

                _logger.Info(CustomLogMessageGenerator.GetLogMessageBase(), "SelectRegionsS", clientUser.UserId, "None", "Selectiong route alerts" + clientUser.UserId);

                return (RouteAlert);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, CustomLogMessageGenerator.GetLogMessageBase(), "SelectRegionsS", clientUser.UserId, "None", "Exception thrown");

                return null;
            }
            if (!ModelState.IsValid)
            {
                return null;
            }
         

        }
        // PUT: api/REGIONs/5
       

    }
}
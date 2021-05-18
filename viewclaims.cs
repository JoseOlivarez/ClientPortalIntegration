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
    public class CLAIMTRANsController : ApiController
    {
        private ClientPortalContext db = new ClientPortalContext();

        private readonly IUserHelper _userHelper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ITokenManager _token;
        public CLAIMTRANsController(IUserHelper userHelper, IUnitOfWork unitOfWork, ILoggerFactory loggerFactory, ITokenManager tokenManager)
        {
            _userHelper = userHelper;
            _token = tokenManager;
            _unitOfWork = unitOfWork;
            _logger = loggerFactory.GetLogger(this.GetType().FullName);

        }
        [ValidateModel]
        [HttpGet]
        [Route("api/claimtrans/ViewClaimsDescription")]
        public async Task<IHttpActionResult> viewClaimsDescription( int TransId)
        {
           // var viewClaimsDescriptionAuth = db.CLAIMTRANS.SqlQuery("SELECT @codfax = b.codfax FROM client_master c, branch b WHERE c.branchid = b.branchid and c.clientid = (SELECT clientid FROM claimtrans WHERE tranid = @TID) SELECT CONVERT(char(10),recdate,101) recdate,claimno + '-' + invoice claimno,customer,CONVERT(char(10), cdate, 101) cdate,route,dealer,gstno,amount,partno,pdesc,checkno,comment,caller,entered, CONVERT(char(10), datecl, 101) datecl,type,stat, codfax FROM claimtrans WHERE tranid = @TID", new SqlParameter("TID", TransId)
            //    , new SqlParameter("@codfax",codFax )).FirstOrDefault();
            //declare client user
            ClientInfo clientUser;

            try
            {
                clientUser = _userHelper.GetClientUser(User);
            }
            catch (Exception ex)
            {

                return InternalServerError();
            }

            try
            {
                var token = _token.GenerateToken(clientUser); //need to add customer by account later
                var ClaimTransDescription = await _unitOfWork.ClaimTrans.ViewClaimsDescription(TransId, clientUser.ClientId);
                if(ClaimTransDescription == null)
                {
                    return BadRequest();
                }
                _logger.Info(CustomLogMessageGenerator.GetLogMessageBase(), "ViewClaimsDescription", clientUser.UserId, TransId, ClaimTransDescription);

                return Ok(ClaimTransDescription);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, CustomLogMessageGenerator.GetLogMessageBase(), "ViewClaimsDescription", clientUser.UserId, TransId, "Exception thrown.");
                return BadRequest();

            }
           
            return BadRequest();

           
        }
        [ValidateModel]
        [HttpGet]
        [Route("api/claimtrans/ViewClaims")]
        public async Task<IHttpActionResult> viewClaims(  ViewClaims claim)
        {
            ClientInfo clientUser; clientUser = _userHelper.GetClientUser(User);

            try
            {
                var claimResults = await _unitOfWork.ClaimTrans.ViewClaims(claim, clientUser.ClientId);
                if(claimResults == null)
                {
                    return BadRequest();
                }
                var token = _token.GenerateToken(clientUser);
                _logger.Info(CustomLogMessageGenerator.GetLogMessageBase(), "ViewClaims", clientUser.UserId, claim, claimResults);

                return Ok(claimResults);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, CustomLogMessageGenerator.GetLogMessageBase(), "ViewClaims", clientUser.UserId, claim, "Exception thrown.");

                return InternalServerError();
            }

        }

    }
}
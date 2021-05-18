using ClientPortalApi.Core.Domain;
using ClientPortalApi.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ClientPortalApi.Controllers.DataTransferObjects;
using NLog.Filters;

namespace ClientPortalApi.Persistence.Repositories
{
    public class ClaimTranRepository : DomainRepository<CLAIMTRAN>, IClaimTranRepository
    {
        public ClaimTranRepository(ClientPortalContext context) : base(context)
        {

        }



        public async Task<List<ClaimTranDTO>> ViewClaims(ViewClaims claim, int ClientId)
        {
            IQueryable<CLAIMTRAN> baseQuery = (
                from c in _context.CLAIMTRANS
                where ClientId == c.CLIENTID
                select c
            );

            var claimStats = new string[] { "A", "D", "C" };

            //STAT append
            if (claimStats.Contains(claim.Stat))
            {
                baseQuery = baseQuery.Where(c => c.STAT == claim.Stat);
            }

            //Customer append
            if (!string.IsNullOrWhiteSpace(claim.Customer))
            {
                baseQuery = baseQuery.Where(c => c.CUSTOMER == (claim.Customer));
            }
            if (!string.IsNullOrWhiteSpace(claim.ClaimNumber))
            {
                baseQuery = baseQuery.Where(c => c.CLAIMNO.Contains(claim.ClaimNumber));
            }
            if(claim.FromDate != null)
            {
                if(claim.ToDate!= null)
                {
                    baseQuery = baseQuery.Where(c => c.RECDATE >= claim.FromDate && c.RECDATE <= claim.ToDate);
                }
                else
                {
                    baseQuery = baseQuery.Where(c => c.RECDATE >= claim.FromDate);
                }
            }
            //TODO ClaimNo Append

            //TODO FromDate Append

            //TODO ToDate Append

            //Select To DTO
            List<ClaimTranDTO> finalResult = await baseQuery.Select(c => new ClaimTranDTO
            {
                TranId = c.TRANID,
                BranchId = c.BRANCHID,
                RecDate = c.RECDATE,
                Dealer = c.DEALER,
                ClientId = c.CLIENTID,
                GstNo = c.GSTNO,
                PartNo = c.PARTNO,
                PDesc = c.PDESC,
                ClaimNo = c.CLAIMNO,
                Invoice = c.INVOICE,
                CDate = c.CDATE,
                Customer = c.CUSTOMER,
                Route = c.ROUTE,
                Amount = c.AMOUNT,
                Type = c.TYPE,
                UnCod = c.UNCOD,
                DateCl = c.DATECL,
                CheckNo = c.CHECKNO,
                Comment = c.COMMENT,
                PNotes = c.PNOTES,
                Caller = c.CALLER,
                PaidBy = c.PAIDBY,
                Closed = c.CLOSED,
                //Moded
                //ModBy
                //Entered
                //EnterBy
                GWL = c.GWL,

                Stat = (
                    c.STAT == "A" ? "Approved" :
                    c.STAT == "D" ? "Denied" :
                    c.STAT == "C" ? "Canceled" : "Pending"
                )
            }).ToListAsync();

            return finalResult;
            /*
            if (!string.IsNullOrWhiteSpace(claim.ClaimNumber) && !string.IsNullOrWhiteSpace(claim.Customer) && (claim.FromDate) != null && claim.ToDate != null)
            {
                if (claim.ClaimNumber.Length > 1)
                {//if claim number is legit we are going to grab all the orders where the claim number matches the claim number passed for a particular clientid
                    try
                    {
                        var viewClaims =
                        (
                          from c in _context.CLAIMTRANS
                          where (c.STAT != null && c.CLAIMNO == claim.ClaimNumber && ClientId == c.CLIENTID)

                          select new ClaimTranDTO
                          {
                              TranId = c.TRANID,
                              BranchId = c.BRANCHID,
                              RecDate = c.RECDATE,
                              Dealer = c.DEALER,
                              ClientId = c.CLIENTID,
                              GstNo = c.GSTNO,
                              PartNo = c.PARTNO,
                              PDesc = c.PDESC,
                              ClaimNo = c.CLAIMNO,
                              Invoice = c.INVOICE,
                              CDate = c.CDATE,
                              Customer = c.CUSTOMER,
                              Route = c.ROUTE,
                              Amount = c.AMOUNT,
                              Type = c.TYPE,
                              UnCod = c.UNCOD,
                              DateCl = c.DATECL,
                              CheckNo = c.CHECKNO,
                              Comment = c.COMMENT,
                              PNotes = c.PNOTES,
                              Caller = c.CALLER,
                              PaidBy = c.PAIDBY,
                              Closed = c.CLOSED,
                              //Moded
                              //ModBy
                              //Entered
                              //EnterBy
                              GWL = c.GWL,

                              Stat =
                                                                   (
                                                                       c.STAT == "A" ? "Approved" :
                                                                       c.STAT == "D" ? "Denied" :
                                                                       c.STAT == "C" ? "Canceled" : "Pending"
                                                                   )

                          }
                      ).SingleOrDefaultAsync();
                        return await viewClaims;
                    }// return value
                    catch (Exception ex)
                    {
                        return await Task.FromResult<ClaimTranDTO>(null);
                    }
                }
                else
                {
                    var viewClaims =
                        (

                        from c in _context.CLAIMTRANS
                        where (c.CUSTOMER == claim.Customer && c.RECDATE >= claim.FromDate && c.RECDATE <= claim.ToDate && c.CLOSED == claim.Stat && ClientId == c.CLIENTID)
                        select new ClaimTranDTO
                        {
                            TranId = c.TRANID,
                            BranchId = c.BRANCHID,
                            RecDate = c.RECDATE,
                            Dealer = c.DEALER,
                            ClientId = c.CLIENTID,
                            GstNo = c.GSTNO,
                            PartNo = c.PARTNO,
                            PDesc = c.PDESC,
                            ClaimNo = c.CLAIMNO,
                            Invoice = c.INVOICE,
                            CDate = c.CDATE,
                            Customer = c.CUSTOMER,
                            Route = c.ROUTE,
                            Amount = c.AMOUNT,
                            Type = c.TYPE,
                            UnCod = c.UNCOD,
                            DateCl = c.DATECL,
                            CheckNo = c.CHECKNO,
                            Comment = c.COMMENT,
                            PNotes = c.PNOTES,
                            Caller = c.CALLER,
                            PaidBy = c.PAIDBY,
                            Closed = c.CLOSED,
                                        //Moded
                                        //ModBy
                                        //Entered
                                        //EnterBy
                                        GWL = c.GWL,

                            Stat =
                                                       (
                                                           c.STAT == "A" ? "Approved" :
                                                           c.STAT == "D" ? "Denied" :
                                                           c.STAT == "C" ? "Canceled" : "Pending"
                                                       )
                        }
                        ).SingleOrDefaultAsync();
                    return await viewClaims;
                }

            }
            if (!string.IsNullOrWhiteSpace(claim.Customer) && !string.IsNullOrWhiteSpace(claim.ClaimNumber))
            {
                var selectFromCustomerNameAndCustomer = (from c in _context.CLAIMTRANS
                                                         where (c.CUSTOMER == claim.Customer && c.CLAIMNO == claim.ClaimNumber)
                                                         select new ClaimTranDTO
                                                         {
                                                             TranId = c.TRANID,
                                                             BranchId = c.BRANCHID,
                                                             RecDate = c.RECDATE,
                                                             Dealer = c.DEALER,
                                                             ClientId = c.CLIENTID,
                                                             GstNo = c.GSTNO,
                                                             PartNo = c.PARTNO,
                                                             PDesc = c.PDESC,
                                                             ClaimNo = c.CLAIMNO,
                                                             Invoice = c.INVOICE,
                                                             CDate = c.CDATE,
                                                             Customer = c.CUSTOMER,
                                                             Route = c.ROUTE,
                                                             Amount = c.AMOUNT,
                                                             Type = c.TYPE,
                                                             UnCod = c.UNCOD,
                                                             DateCl = c.DATECL,
                                                             CheckNo = c.CHECKNO,
                                                             Comment = c.COMMENT,
                                                             PNotes = c.PNOTES,
                                                             Caller = c.CALLER,
                                                             PaidBy = c.PAIDBY,
                                                             Closed = c.CLOSED,
                                                             //Moded
                                                             //ModBy
                                                             //Entered
                                                             //EnterBy
                                                             GWL = c.GWL,

                                                             Stat =
                                                                   (
                                                                       c.STAT == "A" ? "Approved" :
                                                                       c.STAT == "D" ? "Denied" :
                                                                       c.STAT == "C" ? "Canceled" : "Pending"
                                                                   )
                                                         }
               ).SingleOrDefaultAsync();
                return await (selectFromCustomerNameAndCustomer);
            } //if customer and date ranges  are passed
            if (!string.IsNullOrWhiteSpace(claim.Customer) && claim.ToDate != null && claim.FromDate != null)
            {
                var selectFromCustomerName = (from c in _context.CLAIMTRANS
                                              where (c.CUSTOMER == claim.Customer && c.RECDATE >= claim.FromDate && c.RECDATE <= claim.ToDate)
                                              select new ClaimTranDTO
                                              {
                                                  TranId = c.TRANID,
                                                  BranchId = c.BRANCHID,
                                                  RecDate = c.RECDATE,
                                                  Dealer = c.DEALER,
                                                  ClientId = c.CLIENTID,
                                                  GstNo = c.GSTNO,
                                                  PartNo = c.PARTNO,
                                                  PDesc = c.PDESC,
                                                  ClaimNo = c.CLAIMNO,
                                                  Invoice = c.INVOICE,
                                                  CDate = c.CDATE,
                                                  Customer = c.CUSTOMER,
                                                  Route = c.ROUTE,
                                                  Amount = c.AMOUNT,
                                                  Type = c.TYPE,
                                                  UnCod = c.UNCOD,
                                                  DateCl = c.DATECL,
                                                  CheckNo = c.CHECKNO,
                                                  Comment = c.COMMENT,
                                                  PNotes = c.PNOTES,
                                                  Caller = c.CALLER,
                                                  PaidBy = c.PAIDBY,
                                                  Closed = c.CLOSED,
                                                  //Moded
                                                  //ModBy
                                                  //Entered
                                                  //EnterBy
                                                  GWL = c.GWL,

                                                  Stat =
                                                                   (
                                                                       c.STAT == "A" ? "Approved" :
                                                                       c.STAT == "D" ? "Denied" :
                                                                       c.STAT == "C" ? "Canceled" : "Pending"
                                                                   )
                                              }
                        ).SingleOrDefaultAsync();
                return await selectFromCustomerName;
            } //.if claim number and date ranges are passed
            if (!string.IsNullOrWhiteSpace(claim.ClaimNumber) && claim.ToDate != null && claim.FromDate != null)
            {
                var selectFromCustomerName = (from c in _context.CLAIMTRANS
                                              where (c.CLAIMNO == claim.ClaimNumber && c.RECDATE >= claim.FromDate && c.RECDATE <= claim.ToDate)
                                              select new ClaimTranDTO
                                              {
                                                  TranId = c.TRANID,
                                                  BranchId = c.BRANCHID,
                                                  RecDate = c.RECDATE,
                                                  Dealer = c.DEALER,
                                                  ClientId = c.CLIENTID,
                                                  GstNo = c.GSTNO,
                                                  PartNo = c.PARTNO,
                                                  PDesc = c.PDESC,
                                                  ClaimNo = c.CLAIMNO,
                                                  Invoice = c.INVOICE,
                                                  CDate = c.CDATE,
                                                  Customer = c.CUSTOMER,
                                                  Route = c.ROUTE,
                                                  Amount = c.AMOUNT,
                                                  Type = c.TYPE,
                                                  UnCod = c.UNCOD,
                                                  DateCl = c.DATECL,
                                                  CheckNo = c.CHECKNO,
                                                  Comment = c.COMMENT,
                                                  PNotes = c.PNOTES,
                                                  Caller = c.CALLER,
                                                  PaidBy = c.PAIDBY,
                                                  Closed = c.CLOSED,
                                                  //Moded
                                                  //ModBy
                                                  //Entered
                                                  //EnterBy
                                                  GWL = c.GWL,

                                                  Stat =
                                                                   (
                                                                       c.STAT == "A" ? "Approved" :
                                                                       c.STAT == "D" ? "Denied" :
                                                                       c.STAT == "C" ? "Canceled" : "Pending"
                                                                   )
                                              }
                        ).SingleOrDefaultAsync();
                return await (selectFromCustomerName);
            } //if claim number is passed
            if (!string.IsNullOrWhiteSpace(claim.ClaimNumber))
            {
                var selectFromClaimNo = (from c in _context.CLAIMTRANS
                                         where (c.CLAIMNO == claim.ClaimNumber)
                                         select new ClaimTranDTO
                                         {
                                             TranId = c.TRANID,
                                             BranchId = c.BRANCHID,
                                             RecDate = c.RECDATE,
                                             Dealer = c.DEALER,
                                             ClientId = c.CLIENTID,
                                             GstNo = c.GSTNO,
                                             PartNo = c.PARTNO,
                                             PDesc = c.PDESC,
                                             ClaimNo = c.CLAIMNO,
                                             Invoice = c.INVOICE,
                                             CDate = c.CDATE,
                                             Customer = c.CUSTOMER,
                                             Route = c.ROUTE,
                                             Amount = c.AMOUNT,
                                             Type = c.TYPE,
                                             UnCod = c.UNCOD,
                                             DateCl = c.DATECL,
                                             CheckNo = c.CHECKNO,
                                             Comment = c.COMMENT,
                                             PNotes = c.PNOTES,
                                             Caller = c.CALLER,
                                             PaidBy = c.PAIDBY,
                                             Closed = c.CLOSED,
                                             //Moded
                                             //ModBy
                                             //Entered
                                             //EnterBy
                                             GWL = c.GWL,

                                             Stat =
                                                                   (
                                                                       c.STAT == "A" ? "Approved" :
                                                                       c.STAT == "D" ? "Denied" :
                                                                       c.STAT == "C" ? "Canceled" : "Pending"
                                                                   )
                                         }
                   ).SingleOrDefaultAsync();
                return await (selectFromClaimNo);

            } // if only date ranges are passed
            if (!string.IsNullOrWhiteSpace(claim.Customer))
            {
                var selectFromClaimNo = (from c in _context.CLAIMTRANS
                                         where (c.CUSTOMER == claim.Customer)
                                         select new ClaimTranDTO
                                         {
                                             TranId = c.TRANID,
                                             BranchId = c.BRANCHID,
                                             RecDate = c.RECDATE,
                                             Dealer = c.DEALER,
                                             ClientId = c.CLIENTID,
                                             GstNo = c.GSTNO,
                                             PartNo = c.PARTNO,
                                             PDesc = c.PDESC,
                                             ClaimNo = c.CLAIMNO,
                                             Invoice = c.INVOICE,
                                             CDate = c.CDATE,
                                             Customer = c.CUSTOMER,
                                             Route = c.ROUTE,
                                             Amount = c.AMOUNT,
                                             Type = c.TYPE,
                                             UnCod = c.UNCOD,
                                             DateCl = c.DATECL,
                                             CheckNo = c.CHECKNO,
                                             Comment = c.COMMENT,
                                             PNotes = c.PNOTES,
                                             Caller = c.CALLER,
                                             PaidBy = c.PAIDBY,
                                             Closed = c.CLOSED,
                                             //Moded
                                             //ModBy
                                             //Entered
                                             //EnterBy
                                             GWL = c.GWL,

                                             Stat =
                                                                   (
                                                                       c.STAT == "A" ? "Approved" :
                                                                       c.STAT == "D" ? "Denied" :
                                                                       c.STAT == "C" ? "Canceled" : "Pending"
                                                                   )
                                         }
                   ).SingleOrDefaultAsync();
                return await (selectFromClaimNo);

            }
            if (claim.ToDate != null && claim.FromDate != null)
            {
                var viewClaimsFromClaimDates =
                        (

                        from c in _context.CLAIMTRANS
                        where (c.RECDATE >= claim.FromDate && c.RECDATE <= claim.ToDate)

                        select new ClaimTranDTO
                        {
                            TranId = c.TRANID,
                            BranchId = c.BRANCHID,
                            RecDate = c.RECDATE,
                            Dealer = c.DEALER,
                            ClientId = c.CLIENTID,
                            GstNo = c.GSTNO,
                            PartNo = c.PARTNO,
                            PDesc = c.PDESC,
                            ClaimNo = c.CLAIMNO,
                            Invoice = c.INVOICE,
                            CDate = c.CDATE,
                            Customer = c.CUSTOMER,
                            Route = c.ROUTE,
                            Amount = c.AMOUNT,
                            Type = c.TYPE,
                            UnCod = c.UNCOD,
                            DateCl = c.DATECL,
                            CheckNo = c.CHECKNO,
                            Comment = c.COMMENT,
                            PNotes = c.PNOTES,
                            Caller = c.CALLER,
                            PaidBy = c.PAIDBY,
                            Closed = c.CLOSED,
                            //Moded
                            //ModBy
                            //Entered
                            //EnterBy
                            GWL = c.GWL,

                            Stat =
                                                                   (
                                                                       c.STAT == "A" ? "Approved" :
                                                                       c.STAT == "D" ? "Denied" :
                                                                       c.STAT == "C" ? "Canceled" : "Pending"
                                                                   )


                        }
                        ).SingleOrDefaultAsync();
                return await (viewClaimsFromClaimDates);
            }
            return await Task.FromResult<ClaimTranDTO>(null);*/
        }

        // if customer and claim numb        }
        public async Task<ClaimTranDTO> ViewClaimsDescription(int TransId, int ClientId)
        {
            var viewClaimsDescription =
                   (
                   from claim in _context.CLAIMTRANS
                   from client in _context.CLIENT_MASTER
                   join  branch in _context.BRANCHes on client.BRANCHID equals branch.BRANCHID
                    where claim.TRANID == (TransId) && client.CLIENTID == ClientId
                   select new ClaimTranDTO
                   {
                       ClientId = client.CLIENTID,
                       Closed = claim.CLOSED,
                       RecDate = claim.RECDATE,
                       ClaimNo = claim.CLAIMNO,
                       CDate = claim.CDATE,
                       Dealer = claim.DEALER,
                       Route = claim.ROUTE,
                       Customer = claim.CUSTOMER,
                       GstNo = claim.GSTNO,
                       Amount = claim.AMOUNT,
                       PartNo = claim.PARTNO,
                       PDesc = claim.PDESC,
                       CheckNo = claim.CHECKNO,
                       Comment = claim.COMMENT,
                       Caller = claim.CALLER,
                       Entered = claim.ENTERED,
                       CodFax = branch.CODFAX,
                       DateCl = claim.DATECL,
                       Type = claim.TYPE,
                       Stat = claim.STAT
                   }
                   ).SingleOrDefaultAsync();


            return await (viewClaimsDescription);
        }
    }
}
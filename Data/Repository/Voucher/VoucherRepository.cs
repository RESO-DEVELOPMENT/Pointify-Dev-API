using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace Infrastructure.Repository
{
    public interface IVoucherRepository
    {
        public Task DeleteBulk(Guid voucherGroupId);
        public Task InsertBulk(List<Voucher> vouchers);
        public Task<bool> RejectVoucher(Guid voucherGroupId, Guid promotionId);
        public Task<bool> UpdateVoucherGroupWhenDeletetier(Guid voucherGroupId, Guid tierId);
    }
    public class VoucherRepositoryImp : IVoucherRepository
    {
        private const string connectionString = AppConstant.CONNECTION_STRING;
        public async Task DeleteBulk(Guid voucherGroupId)
        {
            using (var context = new PromotionEngineContext(options: GetDbOption()))
            {
                try
                {
                    Debug.WriteLine("\n>>>>>> DELETE VOUCHER: " + DateTime.Now.ToString("HH:mm:ss"));
                    var vouchers = context.Voucher.Where(x => x.VoucherGroupId.Equals(voucherGroupId)
                                                         && !x.IsUsed
                                                         && !x.IsRedemped);
                    var voucherGroup = context.VoucherGroup.FirstOrDefault(x => x.VoucherGroupId.Equals(voucherGroupId));
                    context.Voucher.RemoveRange(vouchers);
                    foreach (var voucher in vouchers)
                    {
                        var item = voucherGroup.Voucher.Where(el => el.VoucherId.Equals(voucher.VoucherId)).FirstOrDefault();
                        voucherGroup.Voucher.Remove(item);
                    }
                    context.SaveChanges();
                    Debug.WriteLine("\n>>>>>> DELETE VOUCHER GROUP: " + DateTime.Now.ToString("HH:mm:ss"));
                    context.Entry(voucherGroup).State = EntityState.Modified;
                    voucherGroup.DelFlg = true;
                    context.SaveChanges();
                }
                finally
                {
                    if (context != null)
                        context.Dispose();
                }

            }
        }

        public async Task InsertBulk(List<Voucher> vouchers)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    PromotionEngineContext context = null;
                    try
                    {
                        context = new PromotionEngineContext(options: GetDbOption());
                        context.ChangeTracker.AutoDetectChangesEnabled = false;

                        int count = 0;
                        foreach (var entityToInsert in vouchers)
                        {
                            ++count;
                            context = AddToContext(context, entityToInsert, count, 100, true);
                        }

                        context.SaveChanges();
                    }
                    finally
                    {
                        if (context != null)
                            context.Dispose();
                    }

                    scope.Complete();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error voucher repository: ", ex.Message);
                Debug.WriteLine("Error voucher repository: ", ex.InnerException);
                Debug.WriteLine("Error voucher repository: ", ex.StackTrace);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: ex.Message);
            }
        }

        private PromotionEngineContext AddToContext(PromotionEngineContext context,
            Voucher entity,
            int count,
            int commitCount,
            bool recreateContext)
        {
            context.Set<Voucher>().Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new PromotionEngineContext(options: GetDbOption());
                    context.ChangeTracker.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }
        private DbContextOptions<PromotionEngineContext> GetDbOption()
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<PromotionEngineContext>(), connectionString: connectionString).Options;
        }
        public async Task<bool> RejectVoucher(Guid voucherGroupId, Guid promotionId)
        {
            using (var context = new PromotionEngineContext())
            {
                /*  var now = DateTime.Now;
                  var voucher = await context.VoucherGroup.FindAsync(voucherGroupId);
                  var promo = await context.Promotion.FindAsync(promotionId);
                  if (voucher != null)
                  {
                      context.Entry(voucher).State = EntityState.Modified;
                      voucher.Promotion = null;
                      voucher.PromotionId = null;
                      voucher.UpdDate = now;
                      context.VoucherGroup.Update(voucher);
                  }
                  if (promo != null)
                  {
                      context.Entry(promo).State = EntityState.Modified;
                      promo.VoucherGroup = null;
                      promo.UpdDate = now;
                  }*/



                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateVoucherGroupWhenDeletetier(Guid voucherGroupId, Guid tierId)
        {

            using (var context = new PromotionEngineContext(options: GetDbOption()))
            {
                IQueryable<VoucherGroup> query = context.Set<VoucherGroup>();
                var voucherGroup = await query.Where(el => el.VoucherGroupId.Equals(voucherGroupId)).Include("Voucher").FirstOrDefaultAsync();
                if (voucherGroup != null)
                {
                    var now = DateTime.UtcNow.AddHours(7);
                    context.Entry(voucherGroup).State = EntityState.Modified;
                    var vouchers = voucherGroup.Voucher.Where(el => el.PromotionTierId.Equals(tierId)).ToList();
                    if (vouchers != null && vouchers.Count() > 0)
                    {

                        foreach (var voucher in vouchers)
                        {
                            if (voucher.IsUsed || voucher.IsRedemped)
                            {
                                voucher.PromotionTierId = null;
                            }
                            else
                            {
                                voucher.PromotionTierId = null;
                                voucher.PromotionId = null;
                                voucher.Promotion = null;
                            }
                            voucher.UpdDate = now;
                        }
                        voucherGroup.UpdDate = now;
                    }
                }

                return await context.SaveChangesAsync() > 0;
            }

        }
    }
}

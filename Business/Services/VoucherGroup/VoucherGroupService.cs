
using ApplicationCore.Utils;
using ApplicationCore.Worker;
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using MlkPwgen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Services
{
    public class VoucherGroupService : BaseService<VoucherGroup, VoucherGroupDto>, IVoucherGroupService

    {
        private readonly IVoucherWorker _voucherWorker;
        private readonly PromotionEngineContext _context;
        public VoucherGroupService(IUnitOfWork unitOfWork, IMapper mapper, IVoucherWorker voucherWorker,
            PromotionEngineContext context) : base(unitOfWork, mapper)
        {
            _voucherWorker = voucherWorker;
            _context = context;
        }

        protected override IGenericRepository<VoucherGroup> _repository => _unitOfWork.VoucherGroupRepository;
        //protected IGenericRepository<Voucher> _repositoryVoucher => _unitOfWork.VoucherRepository;
        public List<VoucherDto> GenerateBulkCodeVoucher(VoucherGroupDto dto)
        {
            List<VoucherDto> result = new List<VoucherDto>();
            for (var i = 0; i < dto.Quantity; i++)
            {
                VoucherDto voucher = new VoucherDto();

                string randomVoucher = RandomString(dto.Charset, dto.CustomCharset, dto.CodeLength);
                voucher.VoucherCode = dto.Prefix + randomVoucher + dto.Postfix;
                voucher.VoucherGroupId = dto.VoucherGroupId;
                result.Add(voucher);
            }
            return result;
        }
        // private Random Random = new Random();
        public string RandomString(string charset, string customCode, int length)
        {
            if (length == 0)
            {
                length = 10;
            }
            if (!string.IsNullOrEmpty(customCode))
            {
                charset = customCode;
            }
            var str = PasswordGenerator.Generate(length: length, allowed: charset);
            return str;

            /*string randomCode = "";
           string chars = "";
           switch (charset)
           {
               case AppConstant.EnvVar.CharsetType.ALPHABETIC:
                   chars = AppConstant.EnvVar.CharsetChars.ALPHABETIC;

                   randomCode = new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
                   break;
               case AppConstant.EnvVar.CharsetType.ALPHANUMERIC:
                   chars = AppConstant.EnvVar.CharsetChars.ALPHANUMERIC;

                   randomCode = new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
                   break;
               case AppConstant.EnvVar.CharsetType.ALPHABETIC_UPERCASE:
                   chars = AppConstant.EnvVar.CharsetChars.ALPHABETIC_UPERCASE;

                   randomCode = new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
                   break;
               case AppConstant.EnvVar.CharsetType.ALPHABETIC_LOWERCASE:
                   chars = AppConstant.EnvVar.CharsetChars.ALPHABETIC_LOWERCASE;

                   randomCode = new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
                   break;
               case AppConstant.EnvVar.CharsetType.NUMBERS:
                   chars = AppConstant.EnvVar.CharsetChars.NUMBERS;

                   randomCode = new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
                   break;
               case AppConstant.EnvVar.CharsetType.CUSTOM:
                   chars = customCode;
                   randomCode = new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
                   break;
           }
           return randomCode;*/
        }



        public async Task<bool> DeleteVoucherGroup(Guid id)
        {
            try
            {
                var group = await _repository.GetFirst(filter: o => o.VoucherGroupId.Equals(id));
                group.DelFlg = true;
                _repository.Update(group);
                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (Exception e)
            {
                //chạy bằng debug mode để xem log
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.Message);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }

        public async Task<bool> RejectVoucherGroup(Guid voucherGroupId, Guid promotionId)
        {
            try
            {
                IVoucherRepository voucherRepository = new VoucherRepositoryImp();
                var result = await voucherRepository.RejectVoucher(voucherGroupId: voucherGroupId, promotionId: promotionId);
                return result;
            }

            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }

        public async Task<bool> AssignVoucherGroup(Guid promotionId, Guid voucherGroupId)
        {
            try
            {
                // Assign voucher group
                /*  var now = DateTime.Now;
                  var voucherGroupEntity = await _repository.GetFirst(filter: o => o.VoucherGroupId.Equals(voucherGroupId)
                                          && (o.PromotionId.Equals(Guid.Empty) || o.PromotionId == null)
                                          && !o.DelFlg);
                  if (voucherGroupEntity != null)
                  {
                      IGenericRepository<Promotion> promoRepo = _unitOfWork.PromotionRepository;
                      var promo = await promoRepo.GetFirst(filter: o => o.PromotionId.Equals(promotionId) && !o.DelFlg);
                      if (promo != null)
                      {
                          promo.VoucherGroup = voucherGroupEntity;
                          promo.UpdDate = now;
                          promoRepo.Update(promo);
                          voucherGroupEntity.PromotionId = promotionId;
                          voucherGroupEntity.UpdDate = now;
                          _repository.Update(voucherGroupEntity);
                      }

                  }*/
                return await _unitOfWork.SaveAsync() > 0;
            }

            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }

        public async Task<bool> HideVoucherGroup(Guid id)
        {
            try
            {
                // Hide voucher group
                var voucherGroupEntity = await _repository.GetFirst(filter: o => o.VoucherGroupId.Equals(id) && !o.DelFlg);
                if (voucherGroupEntity != null)
                {
                    voucherGroupEntity.DelFlg = true;
                    _repository.Update(voucherGroupEntity);
                }
                return await _unitOfWork.SaveAsync() > 0;
            }

            catch (Exception e)
            {
                //chạy bằng debug mode để xem log
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.Message);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }

        public async Task<List<VoucherDto>> CreateVoucherBulk(List<VoucherDto> vouchers)
        {
            try
            {
                IVoucherRepository voucherRepository = new VoucherRepositoryImp();
                List<Voucher> listEntities = _mapper.Map<List<Voucher>>(vouchers);
                await voucherRepository.InsertBulk(listEntities);
                return vouchers;
            }
            catch (Exception e)
            {
                //chạy bằng debug mode để xem log
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.Message);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }

        public async Task<List<Voucher>> MapVoucher(List<VoucherDto> vouchers)
        {
            return _mapper.Map<List<Voucher>>(vouchers);
        }

        public async Task<string> GetPromotionCode(Guid promotionId)
        {
            try
            {
                IGenericRepository<Promotion> promotionRepo = _unitOfWork.PromotionRepository;
                return (await promotionRepo.GetFirst(filter: x => x.PromotionId.Equals(promotionId))).PromotionCode;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }

        public async Task UpdateRedempedQuantity(VoucherGroup voucherGroup, int RedempedQuantity)
        {
            try
            {
                if (voucherGroup != null)
                {
                    voucherGroup.RedempedQuantity += RedempedQuantity;
                    voucherGroup.UpdDate = DateTime.Now;
                    _repository.Update(voucherGroup);
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }

        public async Task UpdateVoucherGroupForApplied(VoucherGroup voucherGroup)
        {
            try
            {
                if (voucherGroup != null)
                {
                    _repository.Update(voucherGroup);
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception e)
            {

                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }

        public async Task<bool> AddMoreVoucher(Guid voucherGroupId, int quantityParam)
        {
            try
            {
                var voucherGroup = await _repository.GetFirst(filter: el => el.VoucherGroupId.Equals(voucherGroupId), includeProperties: "Voucher");
                IGenericRepository<Voucher> voucherRepo = _unitOfWork.VoucherRepository;
                // Voucher cũ
                var oldVouchers = voucherGroup.Voucher.OrderBy(el => el.InsDate);
                var oldVoucherCode = oldVouchers.Select(el => el.VoucherCode);

                // Tổng voucher sau khi tạo xong
                var completeTotal = voucherGroup.Quantity + quantityParam;

                // DTO tạo voucher mới
                var dto = _mapper.Map<VoucherGroupDto>(voucherGroup);
                dto.Voucher = null;
                dto.Quantity = quantityParam;

                // Voucher mới
                var newVoucherCodes = _voucherWorker.GenerateVoucher(dto, isAddMore: true).Select(el => el.VoucherCode);

                // Combine voucher cũ và mới
                var combineList = oldVoucherCode.Concat(newVoucherCodes).Distinct(StringComparer.CurrentCulture).ToList();
                var currentTotal = combineList.Count();

                while (currentTotal < completeTotal)
                {
                    // Số lượng còn lại
                    var remainTotal = completeTotal - currentTotal;
                    dto.Voucher = null;
                    dto.Quantity = remainTotal;

                    // Voucher mới
                    newVoucherCodes = _voucherWorker.GenerateVoucher(dto, isAddMore: true).Select(el => el.VoucherCode);

                    // Combine voucher cũ và mới
                    combineList = combineList.Concat(newVoucherCodes).Distinct(StringComparer.CurrentCulture).ToList();
                    currentTotal = combineList.Count();
                }

                // Danh sách voucher mới
                newVoucherCodes = combineList.Where(el => !oldVoucherCode.Any(code => code.Equals(el))).ToList();
                var now = Common.GetCurrentDatetime();
                List<Voucher> insList = new List<Voucher>();
                var startIndex = oldVoucherCode.Count();
                for (int i = 1; i <= newVoucherCodes.Count(); i++)
                {
                    var code = newVoucherCodes.ElementAt(i - 1);

                    var voucherEntity = new Voucher()
                    {
                        VoucherId = Guid.NewGuid(),
                        VoucherCode = code,
                        InsDate = now,
                        UpdDate = now,
                        Index = startIndex + i,
                    };
                    voucherRepo.Add(voucherEntity);
                    insList.Add(voucherEntity);
                    voucherGroup.Voucher.Add(voucherEntity);
                }
                //voucherGroup.Voucher.ToList().AddRange(insList);
                insList.Count();
                voucherGroup.UpdDate = now;
                voucherGroup.Quantity = voucherGroup.Voucher.Count();
                _repository.Update(voucherGroup);
                var result = await _unitOfWork.SaveAsync() > 0;

                //_voucherWorker.InsertVouchers(voucherDto: dto, isAddMore: true, vouchersAdd: insList);
                return result;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }


        }

        public async Task<GenericRespones<AvailableVoucherDto>> GetAvailable(int PageSize, int PageIndex, Guid BrandId)
        {
            try
            {
                var availGroups = (await _repository.Get(pageSize: PageSize, pageIndex: PageIndex,
                                        filter: o => o.BrandId.Equals(BrandId))).ToList();
                var result = new List<AvailableVoucherDto>();
                if (availGroups.Count() > 0)
                {
                    foreach (var group in availGroups)
                    {
                        var dto = new AvailableVoucherDto()
                        {
                            VoucherGroupId = group.VoucherGroupId,
                            Name = group.VoucherName,
                            Quantity = Convert.ToInt16(group.Quantity)
                        };
                        result.Add(dto);
                    }
                }
                var totalItems = await _repository.CountAsync(
                                             filter: o => o.BrandId.Equals(BrandId));
                GenericRespones<AvailableVoucherDto> response = new GenericRespones<AvailableVoucherDto>(items: result.ToList(), size: PageSize, page: PageIndex, total: totalItems, totalpage: (int)Math.Ceiling(totalItems / (double)PageSize));;
                return response;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }

        public async Task<List<VoucherGroupForPromo>> GetVoucherGroupForPromo(Guid brandId)
        {
            try
            {
                var result = new List<VoucherGroupForPromo>();
                var groups = (await _repository.Get(
                    filter: o => o.BrandId.Equals(brandId)
                    && o.Quantity > o.RedempedQuantity
                    && o.Quantity > o.UsedQuantity
                    && !o.DelFlg,
                    includeProperties: "Action,Gift")).ToList();
                if (groups.Count > 0)
                {
                    foreach (var group in groups)
                    {
                        var dto = new VoucherGroupForPromo()
                        {
                            VoucherGroupId = group.VoucherGroupId,
                            VoucherName = group.VoucherName,
                            Quantity = group.Quantity,
                        };
                        if (group.Action != null)
                        {
                            dto.ActionType = group.Action.ActionType;
                            dto.ActionId = group.ActionId;
                        }
                        else if (group.Gift != null)
                        {
                            dto.PostActionType = group.Gift.PostActionType;
                            dto.GiftId = group.GiftId;
                        }
                        result.Add(dto);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }

        public async Task<VoucherIndexInfo> CheckAvailableIndex(Guid voucherGroupId)
        {
            try
            {
                var result = new VoucherIndexInfo()
                {
                    Available = false,
                    Total = 0,
                    Remain = 0,
                };
                var group = await _repository.GetFirst(filter: o => o.VoucherGroupId.Equals(voucherGroupId) && !o.DelFlg, includeProperties: "Voucher");
                if (group != null)
                {
                    var vouchers = group.Voucher;
                    if (vouchers.Count() > 0)
                    {
                        var remainVouchers = vouchers.Where(o => (o.PromotionTierId == null || o.PromotionTierId.Equals(Guid.Empty))
                                                                  && (o.PromotionId == null || o.PromotionId.Equals(Guid.Empty)));
                        var remain = remainVouchers.Count();
                        var total = vouchers.Count();
                        var avail = total - remain > 0;
                        result = new VoucherIndexInfo()
                        {
                            Available = avail,
                            Total = total,
                            Remain = remain,
                        };
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }

        #region get voucher group detail
        public async Task<VoucherGroupDetailDto> GetDetail(Guid id)
        {
            try
            {
                var result = new VoucherGroupDetailDto();
                var group = await _repository.GetFirst(filter: o => o.VoucherGroupId.Equals(id) && !o.DelFlg,
                                                        includeProperties: "Action,Gift");
                if (group != null)
                {
                    result = new VoucherGroupDetailDto()
                    {
                        VoucherGroupId = group.VoucherGroupId,
                        VoucherName = group.VoucherName,
                        BrandId = group.BrandId,
                        ActionId = group.ActionId,
                        GiftId = group.GiftId,
                        RedempedQuantity = group.RedempedQuantity,
                        Total = group.Quantity,
                        UsedQuantity = group.UsedQuantity,
                        Remain = 0,

                    };
                    if (group.Action != null)
                    {
                        result.ActionType = group.Action.ActionType;
                        result.Action = group.Action;
                    }
                    if (group.Gift != null)
                    {
                        result.PostActionType = group.Gift.PostActionType;
                        result.Gift = group.Gift;
                    }
                    if (result.UsedQuantity > result.RedempedQuantity)
                    {
                        result.Remain = result.Total - result.UsedQuantity;
                    }
                    else
                    {
                        result.Remain = result.Total - result.RedempedQuantity;
                    }
                    result.PromoList = await GetPromoList(id);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }
        private async Task<List<PromoOfVoucher>> GetPromoList(Guid voucherGroupId)
        {
            try
            {
                IGenericRepository<PromotionTier> tierRepo = _unitOfWork.PromotionTierRepository;
                var result = new List<PromoOfVoucher>();
                var tiers = await tierRepo.Get(filter: o => o.VoucherGroupId.Equals(voucherGroupId), includeProperties: "Promotion");
                if (tiers.Count() > 0)
                {
                    foreach (var tier in tiers)
                    {
                        var exist = result.Any(o => o.PromotionId.Equals(tier.PromotionId));
                        if (!exist)
                        {
                            if (tier.PromotionId != null && !tier.PromotionId.Equals(Guid.Empty) && !tier.Promotion.DelFlg)
                            {
                                var dto = new PromoOfVoucher()
                                {
                                    PromotionId = (Guid)tier.PromotionId,
                                    PromoName = tier.Promotion.PromotionName,
                                    PromoCode = tier.Promotion.PromotionCode,
                                };
                                result.Add(dto);
                            }

                        }

                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }


        #endregion

        public async Task<CheckAddMoreDto> GetAddMoreInfo(Guid id)
        {
            var MAX = AppConstant.MAX_VOUCHER_QUANTITY;
            var result = new CheckAddMoreDto();
            var group = await _repository.GetFirst(filter: o => o.VoucherGroupId.Equals(id));
            if (group != null)
            {
                var quantity = group.Quantity;
                if (quantity < MAX)
                {
                    var codeLength = group.CodeLength;
                    var charset = group.Charset;
                    var charsetChars = "";
                    switch (charset)
                    {
                        case AppConstant.EnvVar.CharsetType.ALPHABETIC:
                            {
                                charsetChars = AppConstant.EnvVar.CharsetChars.ALPHABETIC;
                                break;
                            }
                        case AppConstant.EnvVar.CharsetType.ALPHABETIC_LOWERCASE:
                            {
                                charsetChars = AppConstant.EnvVar.CharsetChars.ALPHABETIC_LOWERCASE;
                                break;
                            }
                        case AppConstant.EnvVar.CharsetType.ALPHABETIC_UPERCASE:
                            {
                                charsetChars = AppConstant.EnvVar.CharsetChars.ALPHABETIC_UPERCASE;
                                break;
                            }
                        case AppConstant.EnvVar.CharsetType.ALPHANUMERIC:
                            {
                                charsetChars = AppConstant.EnvVar.CharsetChars.ALPHANUMERIC;
                                break;
                            }
                        case AppConstant.EnvVar.CharsetType.NUMBERS:
                            {
                                charsetChars = AppConstant.EnvVar.CharsetChars.NUMBERS;
                                break;
                            }
                        case AppConstant.EnvVar.CharsetType.CUSTOM:
                            {
                                charsetChars = group.CustomCharset;
                                break;
                            }
                    }
                    double generateQuantity = Math.Ceiling(Math.Pow(charsetChars.Length, (double)codeLength));
                    //if (generateQuantity < 0)
                    //{
                    //    generateQuantity = generateQuantity * (-1);
                    //}
                    if (generateQuantity < MAX)
                    {
                        result = new CheckAddMoreDto()
                        {
                            AvailableQuantity = (int)(generateQuantity - quantity),
                        };
                    }
                    else
                    {
                        result = new CheckAddMoreDto()
                        {
                            AvailableQuantity = MAX - quantity,
                        };
                    }

                }
            }
            return result;
        }

        public async Task<VoucherGroupDto> UpdateVoucherGroup(VoucherGroupDto dto)
        {
            IGenericRepository<Infrastructure.Models.Action> actionRepo = _unitOfWork.ActionRepository;
            IGenericRepository<Gift> giftRepo = _unitOfWork.GiftRepository;
            var entity = await _repository.GetFirst(filter: el => el.VoucherGroupId.Equals(dto.VoucherGroupId));
            var now = Common.GetCurrentDatetime();
            entity.UpdDate = now;
            //entity.Voucher = null;
            if (dto.ActionId != null && !dto.ActionId.Equals(Guid.Empty))
            {
                var action = await actionRepo.GetFirst(filter: el => el.ActionId.Equals(dto.ActionId));
                entity.Action = action;
            }
            else if (dto.GiftId != null && !dto.GiftId.Equals(Guid.Empty))
            {
                var action = await giftRepo.GetFirst(filter: el => el.GiftId.Equals(dto.GiftId));
                entity.Gift = action;
            }
            await UpdateTierOfVoucherGroup(entity);
            _repository.Update(entity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<VoucherGroupDto>(entity);
        }

        public async Task<bool> GenerateVoucher(Guid voucherGroupId, int quantityParam)
        {
            try
            {
                // var voucherGroup = await _repository.GetFirst(filter: el => el.VoucherGroupId.Equals(voucherGroupId), includeProperties: "Voucher");
                var voucherGroup = _context.VoucherGroup.Where(x => x.VoucherGroupId.Equals(voucherGroupId))
                    .Include(x => x.Voucher).FirstOrDefault();
                IGenericRepository<Voucher> voucherRepo = _unitOfWork.VoucherRepository;
                // Voucher cũ
                var oldVouchers = voucherGroup.Voucher.OrderBy(el => el.InsDate);
                var oldVoucherCode = oldVouchers.Select(el => el.VoucherCode);

                // Tổng voucher sau khi tạo xong
                var completeTotal = voucherGroup.Quantity + quantityParam;

                // DTO tạo voucher mới
                var dto = _mapper.Map<VoucherGroupDto>(voucherGroup);
                dto.Voucher = null;
                dto.Quantity = quantityParam;

                // Voucher mới
                var newVoucherCodes = _voucherWorker.GenerateVoucher(dto, isAddMore: true).Select(el => el.VoucherCode);

                // Combine voucher cũ và mới
                var combineList = oldVoucherCode.Concat(newVoucherCodes).Distinct(StringComparer.CurrentCulture).ToList();
                var currentTotal = combineList.Count();

                while (currentTotal < completeTotal)
                {
                    // Số lượng còn lại
                    var remainTotal = completeTotal - currentTotal;
                    dto.Voucher = null;
                    dto.Quantity = remainTotal;

                    // Voucher mới
                    newVoucherCodes = _voucherWorker.GenerateVoucher(dto, isAddMore: true).Select(el => el.VoucherCode);

                    // Combine voucher cũ và mới
                    combineList = combineList.Concat(newVoucherCodes).Distinct(StringComparer.CurrentCulture).ToList();
                    currentTotal = combineList.Count();
                }

                // Danh sách voucher mới
                newVoucherCodes = combineList.Where(el => !oldVoucherCode.Any(code => code.Equals(el))).ToList();
                var now = Common.GetCurrentDatetime();
                List<Voucher> insList = new List<Voucher>();
                var startIndex = oldVoucherCode.Count();
                for (int i = 1; i <= newVoucherCodes.Count(); i++)
                {
                    var code = newVoucherCodes.ElementAt(i - 1);

                    var voucherEntity = new Voucher()
                    {
                        VoucherId = Guid.NewGuid(),
                        VoucherCode = code,
                        InsDate = now,
                        UpdDate = now,
                        Index = startIndex + i,
                    };
                    voucherRepo.Add(voucherEntity);
                    insList.Add(voucherEntity);
                    voucherGroup.Voucher.Add(voucherEntity);
                }
                //voucherGroup.Voucher.ToList().AddRange(insList);
                insList.Count();
                voucherGroup.UpdDate = now;
                voucherGroup.Quantity = voucherGroup.Voucher.Count();
                _context.Voucher.AddRange(voucherGroup.Voucher);
                _context.SaveChanges();

                // _context.Entry(voucherGroup.Voucher).State = EntityState.Unchanged;
                _context.VoucherGroup.Update(voucherGroup);
                await _context.SaveChangesAsync();
                var result = _context.SaveChanges() > 0;

                //_voucherWorker.InsertVouchers(voucherDto: dto, isAddMore: true, vouchersAdd: insList);
                return result;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }

        private async Task UpdateTierOfVoucherGroup(VoucherGroup entity)
        {
            IGenericRepository<PromotionTier> tierRepo = _unitOfWork.PromotionTierRepository;
            var tiers = await tierRepo.Get(filter: el => el.VoucherGroupId.Equals(entity.VoucherGroupId));
            if (tiers.Count() > 0)
            {
                foreach (var tier in tiers)
                {
                    if (tier.ActionId != null && !tier.ActionId.Equals(Guid.Empty))
                    {
                        tier.Action = entity.Action;
                    }
                    else if (tier.GiftId != null && !tier.GiftId.Equals(Guid.Empty))
                    {
                        tier.Gift = entity.Gift;
                    }
                    tierRepo.Update(tier);
                }
                await _unitOfWork.SaveAsync();
            }
        }
    }
}

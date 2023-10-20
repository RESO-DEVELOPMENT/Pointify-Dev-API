using Infrastructure.Models;
using Infrastructure.DTOs;
using AutoMapper;
using Infrastructure.UnitOfWork;
using Infrastructure.Repository;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

using Infrastructure.Helper;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Mime;
using System.Text.Json;
using Infrastructure.DTOs.ScanMembership;
using static Infrastructure.Helper.AppConstant.NOTIFY_MESSAGE;

namespace ApplicationCore.Services
{
    public class StoreService : BaseService<Store, StoreDto>, IStoreService
    {
        public StoreService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        protected override IGenericRepository<Store> _repository => _unitOfWork.StoreRepository;

        public async Task<List<PromotionInfomationJsonFile>> GetPromotionsForStore(string brandCode, string storeCode)
        {
            List<PromotionInfomationJsonFile> result = null;
            var store = await _repository.GetFirst(filter: el =>
                    !el.DelFlg
                    && el.StoreCode.Equals(storeCode)
                    && el.Brand.BrandCode.Equals(brandCode),
                includeProperties: "Brand.Promotion," +
                "PromotionStoreMapping.Promotion," +
                "PromotionStoreMapping.Promotion.PromotionTier");

            var promotions = store.PromotionStoreMapping.Where(w => w.Store.StoreCode.Equals(storeCode)
                    && w.Promotion.Status == (int)AppConstant.EnvVar.PromotionStatus.PUBLISH)
                    .Where(w => DateTime.Now <= (w.Promotion.EndDate != null ? w.Promotion.EndDate : DateTime.MaxValue))
                        .Select(s => s.Promotion);

            if (promotions != null && promotions.Count() > 0)
            {
                foreach (var promotion in promotions)
                {
                    var promotionInfomation = _mapper.Map<PromotionInfomationJsonFile>(promotion);
                    if (result == null)
                    {
                        result = new List<PromotionInfomationJsonFile>();
                    }
                    result.Add(promotionInfomation);
                }
            }

            return result;
        }

        public async Task<List<GroupStoreOfPromotion>> GetStoreOfPromotion(Guid promotionId, Guid brandId)
        {
            try
            {
                IGenericRepository<PromotionStoreMapping> mappRepo = _unitOfWork.PromotionStoreMappingRepository;
                // Lấy danh sách store của cửa hàng
                var brandStore = (await _repository.Get(filter: el => el.BrandId.Equals(brandId) && !el.DelFlg)).ToList();

                // Lấy danh sách store của promotion
                var promoStore = (await mappRepo.Get(filter: el => el.PromotionId.Equals(promotionId), includeProperties: "Store")).ToList();

                // Map data cho reponse
                var mappResult = _mapper.Map<List<StoreOfPromotion>>(brandStore);
                foreach (var store in mappResult)
                {
                    var strs = promoStore.Where(s => s.StoreId.Equals(store.StoreId));

                    if (strs.Count() > 0)
                    {
                        store.IsCheck = true;
                    }
                }

                // Group các store
                var result = new List<GroupStoreOfPromotion>();
                var groups = mappResult.GroupBy(el => el.Group).Select(el => el.Distinct()).ToList();
                foreach (var group in groups)
                {

                    var listStore = group.ToList();
                    var groupStore = new GroupStoreOfPromotion
                    {
                        Stores = listStore,
                        Group = listStore.First().Group
                    };
                    result.Add(groupStore);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }
        }

        public async Task<List<GroupStoreOfPromotion>> UpdateStoreOfPromotion(UpdateStoreOfPromotion dto)
        {
            try
            {
                IGenericRepository<PromotionStoreMapping> mappRepo = _unitOfWork.PromotionStoreMappingRepository;

                // Xóa data trong bảng store mapping
                mappRepo.Delete(id: Guid.Empty, filter: el => el.PromotionId.Equals(dto.PromotionId));

                // Insert data mới vào bảng store mapping
                var stores = dto.ListStoreId;
                foreach (var store in stores)
                {
                    PromotionStoreMapping obj = new PromotionStoreMapping
                    {
                        Id = Guid.NewGuid(),
                        PromotionId = dto.PromotionId,
                        StoreId = store,
                        InsDate = DateTime.Now,
                        UpdDate = DateTime.Now
                    };
                    mappRepo.Add(obj);
                }

                await _unitOfWork.SaveAsync();
                var result = await GetStoreOfPromotion(brandId: dto.BrandId, promotionId: dto.PromotionId);

                return result;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }

        }

        public Stream Get(string brandCode, string storeCode)
        {
            var listPromotion = GetPromotionsForStore(brandCode, storeCode).Result;
            byte[] jsonString = JsonSerializer.SerializeToUtf8Bytes(listPromotion);
            var memStream = new MemoryStream(jsonString);

            return memStream;
        }

        public async Task<ScanMembershipResponse> ScanMembership(string code, int codeType)
        {
            if (codeType == 1)
            {
                var membership = await _unitOfWork.MembershipRepository.GetFirst(filter: el => el.PhoneNumber.Equals(code) && !el.DelFlg);
                if (membership == null) return null;
                else
                {
                    var membershipLevel = await _unitOfWork.MemberLevelRepository.GetFirst(filter: el => el.MemberLevelId.Equals(membership.MemberLevelId) && !el.DelFlg);
                    var memberWallet = await _unitOfWork.MemberWalletRepository.Get(filter: el => el.MemberId.Equals(membership.MembershipId) && !el.DelFlag);
                    ScanMembershipResponse res = new ScanMembershipResponse()
                    {
                        MembershipId = membership.MembershipId,
                        PhoneNumber = membership.PhoneNumber,
                        Email = membership.Email,
                        FullName = membership.Fullname,
                        Gender = membership.Gender,
                        MemberLevelName = membershipLevel.Name,
                        Point = memberWallet.FirstOrDefault(memberWallet => memberWallet.Name == "Ví Điểm").Balance,
                        Balance = memberWallet.FirstOrDefault(memberWallet => memberWallet.Name == "Ví Tiền").Balance
                    };
                    return res;
                }
            }
            else if (codeType == 2)
            {
                var membershipcard = await _unitOfWork.MemberShipCardRepository.GetFirst(filter: el => el.MembershipCardCode.Equals(code) && el.Active);
                if (membershipcard == null) return null;
                else
                {
                    var membership = await _unitOfWork.MembershipRepository.GetFirst(filter: el => el.MembershipId.Equals(membershipcard.MemberId) && !el.DelFlg);
                    var membershipLevel = await _unitOfWork.MemberLevelRepository.GetFirst(filter: el => el.MemberLevelId.Equals(membership.MemberLevelId) && !el.DelFlg);
                    var memberWallet = await _unitOfWork.MemberWalletRepository.Get(filter: el => el.MemberId.Equals(membership.MembershipId) && !el.DelFlag);
                    ScanMembershipResponse res = new ScanMembershipResponse()
                    {
                        MembershipId = membership.MembershipId,
                        PhoneNumber = membership.PhoneNumber,
                        Email = membership.Email,
                        FullName = membership.Fullname,
                        Gender = membership.Gender,
                        MemberLevelName = membershipLevel.Name,
                        Point = memberWallet.FirstOrDefault(memberWallet => memberWallet.Name == "Ví Điểm").Balance,
                        Balance = memberWallet.FirstOrDefault(memberWallet => memberWallet.Name == "Ví Tiền").Balance
                    };
                    return res;
                }
            }
            else
            {
                return null;
            }
        }
        public async Task<List<GroupChannelOfPromotion>> GetChannelOfPromotions(Guid promotionId, Guid brandId)
        {
            try
            {
                IGenericRepository<PromotionChannelMapping> mappRepo = _unitOfWork.VoucherChannelRepository;
                // Lấy danh sách channel của cửa hàng
                var brandStore = (await _repository.Get(filter: el => el.BrandId.Equals(brandId) && !el.DelFlg)).ToList();

                // Lấy danh sách store của promotion
                var promoStore = (await mappRepo.Get(filter: el => el.PromotionId.Equals(promotionId), includeProperties: "Channel")).ToList();

                // Map data cho reponse
                var mappResult = _mapper.Map<List<ChannelOfPromotion>>(brandStore);
                foreach (var channel in mappResult)
                {
                    var strs = promoStore.Where(s => s.ChannelId.Equals(channel.ChannelId));

                    if (strs.Count() > 0)
                    {
                        channel.IsCheck = true;
                    }
                }

                // Group các store
                var result = new List<GroupChannelOfPromotion>();
                var groups = mappResult.GroupBy(el => el.Group).Select(el => el.Distinct()).ToList();
                foreach (var group in groups)
                {

                    var listChannel = group.ToList();
                    var groupStore = new GroupChannelOfPromotion
                    {
                        Channels = listChannel,
                        Group = listChannel.First().Group
                    };
                    result.Add(groupStore);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }
        }
    }

}




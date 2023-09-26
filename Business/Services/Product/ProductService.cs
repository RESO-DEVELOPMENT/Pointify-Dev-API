using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Support;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApplicationCore.Worker;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Services
{
    public class ProductService : BaseService<Product, ProductDto>, IProductService
    {
        private readonly IActionService _actionService;

        private readonly IConditionRuleService _conditionRuleService;

        private readonly IVoucherWorker _voucherService;
        private readonly IVoucherGroupService _voucherGroupService;

        private readonly IPromotionTierService _promotionTierService;

        private readonly IProductConditionService _productConditionService;

        private readonly PromotionEngineContext _context;
        //IProductCateService _cateService;
        //const string PASSIO_PRODUCT_HOST = "http://localhost:6789/localservice/getproducts";
        //const string PASSIO_LOGIN_HOST = "http://localhost:6789/localservice/login";

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IProductCateService cateService, IActionService actionService,
            IConditionRuleService conditionRuleService,
            IVoucherWorker voucherService,
            IPromotionTierService promotionTierService,
            IProductConditionService productConditionService,
            IVoucherGroupService voucherGroupService,
            PromotionEngineContext context) : base(unitOfWork, mapper)
        {
            //this._cateService = cateService;
            _actionService = actionService;
            _conditionRuleService = conditionRuleService;
            _voucherService = voucherService;
            _promotionTierService = promotionTierService;
            _productConditionService = productConditionService;
            _voucherGroupService = voucherGroupService;
            _context = context;
        }

        protected override IGenericRepository<Product> _repository => _unitOfWork.ProductRepository;


        protected IGenericRepository<ProductCategory> _cateRepos => _unitOfWork.ProductCategoryRepository;
        public async Task<bool> CheckExistin(string code, Guid brandId, Guid productId)
        {
            try
            {
                var isExist = false;
                if (productId != Guid.Empty)
                {
                    isExist = (await _repository.Get(filter: o => o.Code.Equals(code)
                          && o.ProductCate.BrandId.Equals(brandId)
                          && !o.ProductId.Equals(productId)
                          && !o.DelFlg)).ToList().Count > 0;
                }
                else
                {
                    isExist = (await _repository.Get(filter: o => o.Code.Equals(code)
                          && o.ProductCate.BrandId.Equals(brandId)
                          && !o.DelFlg)).ToList().Count > 0;
                }

                return isExist;
            }
            catch (Exception e)
            {
                //chạy bằng debug mode để xem log
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.StackTrace);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }

        public async Task<List<BrandProductDto>> GetAllBrandProduct(Guid brandId)
        {
            try
            {
                IGenericRepository<ProductCategory> cateRepo = _unitOfWork.ProductCategoryRepository;
                var result = new List<BrandProductDto>();
                var categories = (await cateRepo.Get(filter: o => o.BrandId.Equals(brandId)
                && !o.DelFlg, includeProperties: "Product")).ToList();
                if (categories != null && categories.Count > 0)
                {
                    foreach (var cate in categories)
                    {
                        var products = cate.Product.ToList();
                        if (products != null && products.Count > 0)
                        {
                            foreach (var product in products)
                            {
                                var dto = new BrandProductDto()
                                {
                                    CateId = cate.CateId,
                                    CateName = cate.Name,
                                    ProductCateId = cate.ProductCateId,
                                    Code = product.Code,
                                    ProductId = product.ProductId,
                                    ProductName = product.Name

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
                //chạy bằng debug mode để xem log
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.StackTrace);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }

        public async Task<GenericRespones<BrandProductDto>> GetBrandProduct(int PageSize, int PageIndex, Guid brandId)
        {
            try
            {
                IGenericRepository<ProductCategory> cateRepo = _unitOfWork.ProductCategoryRepository;
                var result = new List<BrandProductDto>();
                //var categories = (await cateRepo.Get(pageIndex: PageIndex, pageSize: PageSize,
                //    filter: o => o.BrandId.Equals(brandId) && !o.DelFlg, includeProperties: "Product")).ToList();
                //if (categories != null && categories.Count > 0)
                //{
                //    foreach (var cate in categories)
                //    {
                //        var products = cate.Product.ToList();

                //    }
                //}
                var products = (await _repository.Get(pageIndex: PageIndex, pageSize: PageSize,
                    filter: o => !o.DelFlg && o.ProductCate.BrandId.Equals(brandId), includeProperties: "ProductCate")).ToList();
                if (products != null && products.Count > 0)
                {
                    foreach (var product in products)
                    {
                        if (!product.DelFlg)
                        {
                            var dto = new BrandProductDto()
                            {
                                CateId = product.ProductCate.CateId,
                                CateName = product.ProductCate.Name,
                                ProductCateId = product.ProductCate.ProductCateId,
                                Code = product.Code,
                                ProductId = product.ProductId,
                                ProductName = product.Name

                            };
                            result.Add(dto);
                        }
                    }
                }
                var list = result;
                var totalItem = (await _repository.Get(filter: o => !o.DelFlg, includeProperties: "ProductCate")).Where(o => o.ProductCate.BrandId.Equals(brandId)).ToList().Count();
                MetaData metadata = new MetaData(pageIndex: PageIndex, pageSize: PageSize, totalItems: totalItem);
                GenericRespones<BrandProductDto> reponse = new GenericRespones<BrandProductDto>(items: list.ToList(), size: PageSize, page: PageIndex, total: totalItem, totalpage: (int)Math.Ceiling(totalItem / (double)PageSize));
                return reponse;
            }
            catch (Exception e)
            {
                //chạy bằng debug mode để xem log
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.StackTrace);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }

        public async Task addCateFromSync(Guid brandId, ProductSyncParamDTO productSyncs)
        {
            var listCateBefore = _cateRepos.Get(filter: el => el.BrandId.Equals(brandId) && !el.DelFlg).Result.ToList();
            foreach (CategorySync cateSync in productSyncs.data.Categories)
            {
                if (listCateBefore.Any(a => a.CateId.Equals(cateSync.Id.ToString())))
                {
                    var productCate = listCateBefore.Where(w => w.CateId.Equals(cateSync.Id.ToString())).First();
                    productCate.Name = cateSync.Name;
                    _cateRepos.Update(productCate);

                }
                else
                {
                    ProductCategoryDto cateDto = new ProductCategoryDto
                    {
                        ProductCateId = Guid.NewGuid(),
                        BrandId = brandId,
                        CateId = cateSync.Id.ToString(),
                        Name = cateSync.Name,
                        InsDate = DateTime.Now,
                        UpdDate = DateTime.Now
                    };

                    _cateRepos.Add(_mapper.Map<ProductCategory>(cateDto));
                }
            }
            await _unitOfWork.SaveAsync();
        }
        public async Task addProductFromSync(Guid brandId, ProductSyncParamDTO productSyncs)
        {
            var listProduct = _repository.Get(filter: el => !el.DelFlg && el.ProductCate.BrandId.Equals(brandId)).Result.ToList();
            var listCateAfter = _cateRepos.Get(filter: el => el.BrandId.Equals(brandId) && !el.DelFlg).Result.ToList();
            foreach (var productSync in productSyncs.data.Products)
            {
                if (productSync.ChildrenProducts.Count < 1 || productSync.ChildrenProducts == null)
                {
                    if (!listProduct.Any(a => a.Code.Equals(productSync.Code)))
                    {
                        var existCate = listCateAfter.Where(w => w.CateId.Equals(productSync.CateId.ToString())).ToList();
                        if (existCate != null && existCate.Count > 0)
                        {
                            ProductDto productDto = new ProductDto
                            {
                                CateId = productSync.CateId.ToString(),
                                Name = productSync.ProductName,
                                Code = productSync.Code,
                                ProductId = Guid.NewGuid(),
                                InsDate = DateTime.Now,
                                UpdDate = DateTime.Now,
                                ProductCateId = existCate.First().ProductCateId
                            };
                            _repository.Add(_mapper.Map<Product>(productDto));
                        }
                        //await _unitOfWork.SaveAsync();
                    }
                    else
                    {
                        var product = listProduct.Where(w => w.Code.Equals(productSync.Code)).First();
                        product.Name = productSync.ProductName;
                        _repository.Update(product);
                    }

                }
                else
                {
                    foreach (var child in productSync.ChildrenProducts)
                    {
                        if (!listProduct.Any(a => a.Code.Equals(child.Code)))
                        {
                            var existCate = listCateAfter.Where(w => w.CateId.Equals(child.CatID.ToString())).ToList();
                            if (existCate != null && existCate.Count > 0)
                            {
                                ProductDto productDto = new ProductDto
                                {
                                    CateId = child.CatID.ToString(),
                                    Name = child.ProductName,
                                    Code = child.Code,
                                    ProductId = Guid.NewGuid(),
                                    InsDate = DateTime.Now,
                                    UpdDate = DateTime.Now,
                                    ProductCateId = existCate.First().ProductCateId
                                };

                                _repository.Add(_mapper.Map<Product>(productDto));
                            }

                        }
                        else
                        {
                            var product = listProduct.Where(w => w.Code.Equals(child.Code)).First();
                            product.Name = productSync.ProductName;
                            _repository.Update(product);
                        }
                    }
                }

            }
            await _unitOfWork.SaveAsync();
        }
        public async Task<ProductSyncParamDTO> SyncProduct(
            Guid brandId, ProductRequestParam productRequestParam)
        {

            try
            {
                var productSyncs = await getProductFromApiUrl(productRequestParam);
                //var cateList = _cateRepos.Get(filter: el => el.BrandId.Equals(brandId)).Result.ToList();
                await addCateFromSync(brandId, productSyncs);
                await addProductFromSync(brandId, productSyncs);
                return productSyncs;
            }
            catch (Exception e)
            {
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.Message);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }
        private async Task<ProductSyncParamDTO> getProductFromApiUrl(ProductRequestParam productRequestParam)
        {
            var client = new HttpClient();
            try
            {
                var token = await getToken(productRequestParam);
                ProductSyncParamDTO productSyncParamDTO = null;

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                //var url = (PASSIO_PRODUCT_HOST);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(productRequestParam.SyncUrl),
                    //Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
                request.Headers.TryAddWithoutValidation("Authorization", String.Format("Bearer {0}", token));
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    response.Content.Headers.ContentType.CharSet = @"utf-8";
                    var responseBody = await response.Content.ReadAsStringAsync();
                    productSyncParamDTO = JsonConvert.DeserializeObject<ProductSyncParamDTO>(responseBody);
                }
                return productSyncParamDTO;
            }
            catch (Exception e)
            {
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.Message);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
            finally
            {
                //Teminate
                client.Dispose();
            }

        }
        private async Task<string> getToken(ProductRequestParam productRequestParam)
        {
            try
            {
                string bearerToken = "";
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var json = "{" + string.Format("\"username\":\"{0}\"," +
                                               "\"password\":\"{1}\"," +
                                               "\"device_id\":\"{2}\"",
                                               productRequestParam.TokenBody.UserName,
                                               productRequestParam.TokenBody.PassWord,
                                               productRequestParam.TokenBody.Device_Id) + "}";
                Debug.WriteLine(json);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(productRequestParam.TokenUrl),
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
                HttpResponseMessage bearerResult = await client.SendAsync(request);

                if (bearerResult.IsSuccessStatusCode)
                {
                    var bearerData = await bearerResult.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(bearerData);
                    TokenData tokenData = jObject.ToObject<TokenData>();
                    bearerToken = tokenData.data.Token;
                }
                return bearerToken;
            }
            catch (Exception e)
            {
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.Message);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);

            }

        }

        public async Task<ProductDto> Update(ProductDto dto)
        {
            try
            {
                var entity = await _repository.GetFirst(filter: o => o.ProductId.Equals(dto.ProductId) && !o.DelFlg);
                if (entity != null)
                {
                    entity.UpdDate = DateTime.Now;
                    if (dto.Name != null)
                    {
                        entity.Name = dto.Name;
                    }
                    if (dto.Code != null)
                    {
                        entity.Code = dto.Code;
                    }
                    if (!dto.ProductCateId.Equals(Guid.Empty) && !dto.ProductCateId.Equals(entity.ProductCateId))
                    {
                        IGenericRepository<ProductCategory> cateRepo = _unitOfWork.ProductCategoryRepository;
                        var oldCate = await cateRepo.GetFirst(filter: o => o.ProductCateId.Equals(entity.ProductCateId) && !o.DelFlg);
                        oldCate.Product.Remove(entity);
                        var newCate = await cateRepo.GetFirst(filter: o => o.ProductCateId.Equals(dto.ProductCateId) && !o.DelFlg);
                        newCate.Product.Add(entity);
                        entity.ProductCateId = dto.ProductCateId;
                        /*entity.CateId = newCate.CateId;*/
                    }
                    entity.DelFlg = dto.DelFlg;
                    _repository.Update(entity);
                    await _unitOfWork.SaveAsync();

                    return _mapper.Map<ProductDto>(entity);
                }
                else
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Product_Cate_NotFound, message: AppConstant.ErrMessage.Product_Cate_NotFound);
                }
            }
            catch (Exception e)
            {
                //chạy bằng debug mode để xem log
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.Message);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }
        public async Task<PromotionInfomation> CheckGiftProduct(ProductDto productDto)
        {
            try
            {
                Promotion promotion = new Promotion();
                PromotionInfomation promotionInfomation = new PromotionInfomation();
                var product = _repository.GetFirst(x => x.Code == productDto.Code && x.DelFlg == false).Result;
                if (product != null)
                {
                    promotion = await _promotionTierService.GetPromotionTierByProductCode(product.Code);
                    // _voucherGroupService.GenerateVoucher(promotion., 1);
                    var promotionTier = _context.PromotionTier.Where(x => x.ProductCode == productDto.Code)
                        .Include(x => x.VoucherGroup).FirstOrDefault();
                    if (promotionTier.VoucherGroupId == null)
                    {
                        throw new ErrorObj(code: (int)HttpStatusCode.NotFound, message: "Không tìm thấy promotionTier");
                    }

                    promotionTier.VoucherGroup.Quantity += 1;
                    _context.VoucherGroup.Update(promotionTier.VoucherGroup);
                    promotionTier.VoucherQuantity += 1;
                    _context.PromotionTier.Update(promotionTier);
                    _context.SaveChanges();
                    var voucherGroup = _mapper.Map<VoucherGroupDto>(promotionTier.VoucherGroup);
                    _voucherService.InsertVouchers(voucherDto: voucherGroup);
                    await _voucherGroupService.AddMoreVoucher((Guid)promotionTier.VoucherGroupId, 1);
                    // await _voucherGroupService.GenerateVoucher((Guid)promotionTier.VoucherGroupId, 1);
                    var voucher = UpdateVoucherRedeem((Guid)promotionTier.VoucherGroupId);
                    voucher.PromotionTierId = promotionTier.PromotionTierId;
                    voucher.PromotionId = promotion.PromotionId;
                    _context.Voucher.Update(voucher);
                    _context.SaveChanges();
                    promotionInfomation.PromotionId = promotion.PromotionId;
                    promotionInfomation.PromotionTierId = promotionTier.PromotionTierId;
                    promotionInfomation.PromotionName = promotion.PromotionName;
                    promotionInfomation.Description = promotion.Description;
                    promotionInfomation.PromotionCode = promotion.PromotionCode;
                    promotionInfomation.VoucherName = promotionTier.VoucherGroup.VoucherName;
                    promotionInfomation.ImgUrl = promotion.ImgUrl;
                    promotionInfomation.VoucherCode =
                        promotion.PromotionCode + promotionTier.TierIndex + "-" + voucher.VoucherCode;
                    promotionInfomation.StartDate = promotion.StartDate;
                    promotionInfomation.EndDate = promotion.EndDate;
                    return promotionInfomation;
                }
                else
                {
                    productDto.ProductId = Guid.NewGuid();
                    productDto.InsDate = DateTime.Now;
                    productDto.UpdDate = DateTime.Now;
                    await this.CreateAsync(productDto);

                    var actionDto = CreateAction(productDto);

                    var conditionRule = CreateCondition(productDto, actionDto);

                    var voucherGroupDto = CreateVoucherGroup(actionDto, productDto);

                    var voucher = UpdateVoucherRedeem(voucherGroupDto.VoucherGroupId);

                    #region Thêm tier vào promotion

                    PromotionTierDto promotionTierDto = new PromotionTierDto
                    {
                        ActionId = actionDto.ActionId,
                        ConditionRuleId = conditionRule.ConditionRuleId,
                        Priority = 1,
                        PromotionId = Guid.Parse("6b53e6ec-840c-46d7-af7e-854b4e86ba57"),
                        TierIndex = 1,
                        VoucherGroupId = voucherGroupDto.VoucherGroupId,
                        VoucherQuantity = 1,
                        ProductCode = productDto.Code,
                    };
                    await _promotionTierService.CreateGiftTier(promotionTierDto);

                    #endregion
                    promotion = await _promotionTierService.GetPromotionTierByProductCode(productDto.Code);

                    promotionInfomation.PromotionId = promotion.PromotionId;
                    promotionInfomation.PromotionTierId = promotionTierDto.PromotionTierId;
                    promotionInfomation.PromotionName = promotion.PromotionName;
                    promotionInfomation.Description = promotion.Description;
                    promotionInfomation.PromotionCode = promotion.PromotionCode;
                    promotionInfomation.VoucherName = voucherGroupDto.VoucherName;
                    promotionInfomation.ImgUrl = promotion.ImgUrl;
                    promotionInfomation.VoucherCode =
                        promotion.PromotionCode + promotionTierDto.TierIndex + "-" + voucher.VoucherCode;
                    promotionInfomation.StartDate = promotion.StartDate;
                    promotionInfomation.EndDate = promotion.EndDate;

                }

                return promotionInfomation;
            }
            catch (Exception e)
            {
                //chạy bằng debug mode để xem log
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.StackTrace);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }
        private ActionDto CreateAction(ProductDto productDto)
        {
            ActionProductMap actionProductMap = new ActionProductMap
            {
                ProductId = productDto.ProductId,
                Quantity = 1
            };
            List<ActionProductMap> actionProductMaps = new List<ActionProductMap>();
            actionProductMaps.Add(actionProductMap);
            ActionDto actDto = new ActionDto
            {
                ActionType = 5,
                BrandId = productDto.BrandId,
                DiscountPercentage = 100,
                ListProduct = actionProductMaps,
                MaxAmount = 9999999,
                Name = $"Giảm {100}% cho sản phẩm {productDto.Name}",
                BundlePrice = 0,
                BundleQuantity = 0,
                BundleStrategy = 0,
                DiscountAmount = 0,
                DiscountQuantity = 0,
                FixedPrice = 0,
                LadderPrice = 0,
                MinPriceAfter = 0,
                OrderLadderProduct = 0
            };
            try
            {
                var actionDto = _actionService.MyAddAction(actDto);
                return actionDto.Result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "||" + "Create action failed");
            }
        }
        private ConditionRuleDto CreateCondition(ProductDto productDto, ActionDto actionDto)
        {
            ProductConditionMapping productConditionMapping = new ProductConditionMapping
            {
                ProductId = productDto.ProductId,
                InsDate = (DateTime)productDto.InsDate,
                UpdDate = (DateTime)productDto.UpdDate
            };
            List<ProductConditionMapping> listProductConditionMapping = new List<ProductConditionMapping>();
            listProductConditionMapping.Add(productConditionMapping);
            ProductConditionDto productConditionDto = new ProductConditionDto
            {
                IndexGroup = 0,
                NextOperator = 1,
                ProductConditionMapping = listProductConditionMapping,
                ProductConditionType = 1,
                ProductQuantity = 0,
                QuantityOperator = ">"
            };
            List<ProductConditionDto> listProductConditionDto = new List<ProductConditionDto>();
            listProductConditionDto.Add(productConditionDto);
            ConditionGroupDto conditionGroupDto = new ConditionGroupDto
            {
                GroupNo = 0,
                NextOperator = 1,
                //ProductCondition = listProductConditionDto,
            };
            List<ConditionGroupDto> listConditionGroupDto = new List<ConditionGroupDto>();
            listConditionGroupDto.Add(conditionGroupDto);
            ConditionRuleDto conditionRuleDto = new ConditionRuleDto
            {
                BrandId = actionDto.BrandId,
                Description = $"Order có {productDto.Name}",
                RuleName = $"Order có {productDto.Name}",
                ConditionGroup = listConditionGroupDto
            };
            try
            {
                var conditionRule = _conditionRuleService.InsertConditionRule(conditionRuleDto);
                return conditionRule.Result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " || " + "Create condition failed");
            }
        }
        private VoucherGroupDto CreateVoucherGroup(ActionDto actionDto, ProductDto productDto)
        {
            VoucherGroupDto voucherGroupDto = new VoucherGroupDto
            {
                VoucherGroupId = Guid.NewGuid(),
                ActionId = actionDto.ActionId,
                BrandId = actionDto.BrandId,
                Charset = "Alphanumeric",
                CodeLength = 8,
                CustomCharset = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
                VoucherName = $"Voucher giảm {actionDto.DiscountPercentage}% cho {productDto.Name}",
                Quantity = 0,
                Postfix = "",
                Prefix = ""
            };
            try
            {
                var voucherGroup = _mapper.Map<VoucherGroup>(voucherGroupDto);
                _context.VoucherGroup.Add(voucherGroup);
                _context.SaveChanges();
                _voucherService.InsertVouchers(voucherDto: voucherGroupDto);
                // _voucherService.AddMoreVoucher(voucherGroupDto.VoucherGroupId, 10000);
                _voucherGroupService.GenerateVoucher(voucherGroupDto.VoucherGroupId, 1);
                return voucherGroupDto;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " || " + "Create voucher group failed");
            }

        }

        private Voucher UpdateVoucherRedeem(Guid voucherGroupId)
        {
            var voucher = _context.Voucher
                .FirstOrDefault(x => x.VoucherGroupId == voucherGroupId && x.IsUsed == false &&
                                     x.IsRedemped == false);
            if (voucher == null)
            {
                _voucherGroupService.GenerateVoucher(voucherGroupId, 1);
                voucher = _context.Voucher
                    .FirstOrDefault(x => x.VoucherGroupId == voucherGroupId && x.IsUsed == false &&
                                         x.IsRedemped == false);
            }
            voucher.IsRedemped = true;
            _context.Voucher.Update(voucher);
            _context.SaveChanges();
            return voucher;
        }
    }
}

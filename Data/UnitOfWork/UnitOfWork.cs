using Infrastructure.Models;
using Infrastructure.Repository;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private PromotionEngineContext _context;
        private bool _disposed = false;

        public UnitOfWork(PromotionEngineContext context)
        {
            _context = context;
            initRepository();
        }
        //Account
        public IGenericRepository<Account> AccountRepository { get; set; }
        //Action
        public IGenericRepository<Models.Action> ActionRepository { get; set; }
        //Brand
        public IGenericRepository<Brand> BrandRepository { get; set; }
        //Channel
        public IGenericRepository<Channel> ChannelRepository { get; set; }
        //ConditionRule
        public IGenericRepository<ConditionRule> ConditionRuleRepository { get; set; }
        //Holiday
        public IGenericRepository<Holiday> HolidayRepository { get; set; }
        //Membership
        public IGenericRepository<Membership> MembershipRepository { get; set; }
        //MembershipAction
        public IGenericRepository<Gift> GiftRepository { get; set; }
        //OrderCondition
        public IGenericRepository<OrderCondition> OrderConditionRepository { get; set; }
        //ProductCondition
        public IGenericRepository<ProductCondition> ProductConditionRepository { get; set; }
        //Promotion
        public IGenericRepository<Promotion> PromotionRepository { get; set; }

        //PromotionStoreMapping
        public IGenericRepository<PromotionStoreMapping> PromotionStoreMappingRepository { get; set; }

        //PromotionTier
        public IGenericRepository<PromotionTier> PromotionTierRepository { get; set; }


        //RoleEntity
        public IGenericRepository<Role> RoleEntityRepository { get; set; }


        //Store
        public IGenericRepository<Store> StoreRepository { get; set; }


        //Voucher
        public IGenericRepository<Voucher> VoucherRepository { get; set; }
        //VoucherChannel
        public IGenericRepository<PromotionChannelMapping> VoucherChannelRepository { get; set; }
        //VoucherGroup
        public IGenericRepository<VoucherGroup> VoucherGroupRepository { get; set; }

        //Condition Group
        public IGenericRepository<ConditionGroup> ConditionGroupRepository { get; set; }

        public IGenericRepository<ProductCategory> ProductCategoryRepository { get; set; }
        public IGenericRepository<Product> ProductRepository { get; set; }
        public IGenericRepository<MemberLevel> MemberLevelRepository { get; set; }

        public IGenericRepository<Device> DeviceRepository { get; set; }

        public IGenericRepository<ActionProductMapping> ActionProductMappingRepository { get; set; }

        public IGenericRepository<MemberLevelMapping> MemberLevelMappingRepository { get; set; }
        public IGenericRepository<GiftProductMapping> GiftProductMappingRepository { get; set; }
        public IGenericRepository<ProductConditionMapping> ProductConditionMappingRepository { get; set; }

        public IGenericRepository<GameCampaign> GameConfigRepository { get; set; }

        public IGenericRepository<GameItems> GameItemsRepository { get; set; }

        public IGenericRepository<GameMaster> GameMasterRepository { get; set; }

        public IGenericRepository<Transaction> TransactionRepository { get; set; }

        public IGenericRepository<StoreGameCampaignMapping> StoreGameCampaignMappingRepository { get; set; }

        public IGenericRepository<Member> MemberRepository { get; set; }

        // MembershipProgram
        public IGenericRepository<MembershipProgram> MembershipProgramRepository { get; set; }
        //WalletType
        public IGenericRepository<WalletType> WalletTypeRepository { get; set; }
        //MemberActionType
        public IGenericRepository<MemberActionType> MemberActionTypeRepository { get; set; }
        //MemberAction
        public IGenericRepository<MemberAction> MemberActionRepository { get; set; }
        //MembershipLevel
        public IGenericRepository<MembershipLevel> MembershipLevelRepository { get; set; }
        private void initRepository()
        {
            //Account
            AccountRepository = new GenericRepository<Account>(_context);
            //Action
            ActionRepository = new GenericRepository<Models.Action>(_context);
            //Brand
            BrandRepository = new GenericRepository<Brand>(_context);
            //Channel
            ChannelRepository = new GenericRepository<Channel>(_context);
            //ConditionRule
            ConditionRuleRepository = new GenericRepository<ConditionRule>(_context);
            //Holiday
            HolidayRepository = new GenericRepository<Holiday>(_context);
            //Membership
            MembershipRepository = new GenericRepository<Membership>(_context);
            //MembershipAction
            GiftRepository = new GenericRepository<Gift>(_context);
            //OrderCondition
            OrderConditionRepository = new GenericRepository<OrderCondition>(_context);
            //ProductCondition
            ProductConditionRepository = new GenericRepository<ProductCondition>(_context);
            //Promotion
            PromotionRepository = new GenericRepository<Promotion>(_context);
            //PromotionStoreMapping
            PromotionStoreMappingRepository = new GenericRepository<PromotionStoreMapping>(_context);
            //PromotionTier
            PromotionTierRepository = new GenericRepository<PromotionTier>(_context);
            //RoleEntity
            RoleEntityRepository = new GenericRepository<Role>(_context);
            //Store
            StoreRepository = new GenericRepository<Store>(_context);
            //Voucher
            VoucherRepository = new GenericRepository<Voucher>(_context);
            //VoucherChannel
            VoucherChannelRepository = new GenericRepository<PromotionChannelMapping>(_context);
            //VoucherGroup
            VoucherGroupRepository = new GenericRepository<VoucherGroup>(_context);
            // Condition Group
            ConditionGroupRepository = new GenericRepository<ConditionGroup>(_context);
            ProductRepository = new GenericRepository<Product>(_context);
            ProductCategoryRepository = new GenericRepository<ProductCategory>(_context);
            MemberLevelRepository = new GenericRepository<MemberLevel>(_context);
            DeviceRepository = new GenericRepository<Device>(_context);
            ActionProductMappingRepository = new GenericRepository<ActionProductMapping>(_context);
            MemberLevelMappingRepository = new GenericRepository<MemberLevelMapping>(_context);
            GiftProductMappingRepository = new GenericRepository<GiftProductMapping>(_context);
            ProductConditionMappingRepository = new GenericRepository<ProductConditionMapping>(_context);
            GameMasterRepository = new GenericRepository<GameMaster>(_context);
            GameConfigRepository = new GenericRepository<GameCampaign>(_context);
            GameItemsRepository = new GenericRepository<GameItems>(_context);
            TransactionRepository = new GenericRepository<Transaction>(_context);
            StoreGameCampaignMappingRepository = new GenericRepository<StoreGameCampaignMapping>(_context);
            //MembershipProgram
            MembershipProgramRepository = new GenericRepository<MembershipProgram>(_context);
            //WalletType
            WalletTypeRepository = new GenericRepository<WalletType>(_context);
            //MemberActionType
            MemberActionTypeRepository = new GenericRepository<MemberActionType>(_context);
            //MemberAction
            MemberActionRepository = new GenericRepository<MemberAction>(_context);
            //MembershipLevel
            MembershipLevelRepository = new GenericRepository<MembershipLevel>(_context);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Debug.WriteLine("\n\nError at unitOfWork: \n" + e.InnerException);
            }
            return 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}

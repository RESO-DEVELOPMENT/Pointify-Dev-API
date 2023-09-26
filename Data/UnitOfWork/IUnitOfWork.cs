using Infrastructure.Models;
using Infrastructure.Repository;
using System.Threading.Tasks;

namespace Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        //Account
        IGenericRepository<Account> AccountRepository { get; }
        //Action
        IGenericRepository<Models.Action> ActionRepository { get; }
        //Brand
        IGenericRepository<Brand> BrandRepository { get; }
        //Channel
        IGenericRepository<Channel> ChannelRepository { get; }
        //ConditionRule
        IGenericRepository<ConditionRule> ConditionRuleRepository { get; }
        //Holiday
        IGenericRepository<Holiday> HolidayRepository { get; }
        //Membership
        IGenericRepository<Membership> MembershipRepository { get; }
        //MembershipAction
        IGenericRepository<Gift> GiftRepository { get; }
        //OrderCondition
        IGenericRepository<OrderCondition> OrderConditionRepository { get; }
        //ProductCondition
        IGenericRepository<ProductCondition> ProductConditionRepository { get; }
        //Promotion
        IGenericRepository<Promotion> PromotionRepository { get; }
        //PromotionStoreMapping
        IGenericRepository<PromotionStoreMapping> PromotionStoreMappingRepository { get; }
        //PromotionTier
        IGenericRepository<PromotionTier> PromotionTierRepository { get; }
        //RoleEntity
        IGenericRepository<Role> RoleEntityRepository { get; }
        //Store
        IGenericRepository<Store> StoreRepository { get; }
        //Voucher
        IGenericRepository<Voucher> VoucherRepository { get; }
        //VoucherChannel
        IGenericRepository<PromotionChannelMapping> VoucherChannelRepository { get; }
        //VoucherGroup
        IGenericRepository<VoucherGroup> VoucherGroupRepository { get; }
        //Condition group
        IGenericRepository<ConditionGroup> ConditionGroupRepository { get; }
        IGenericRepository<ProductCategory> ProductCategoryRepository { get; }
        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<MemberLevel> MemberLevelRepository { get; }

        IGenericRepository<Device> DeviceRepository { get; }
        IGenericRepository<ActionProductMapping> ActionProductMappingRepository { get; }
        IGenericRepository<MemberLevelMapping> MemberLevelMappingRepository { get; }

        IGenericRepository<GiftProductMapping> GiftProductMappingRepository { get; }
        IGenericRepository<ProductConditionMapping> ProductConditionMappingRepository { get; }
        IGenericRepository<StoreGameCampaignMapping> StoreGameCampaignMappingRepository { get; }
        IGenericRepository<GameCampaign> GameConfigRepository { get; }
        IGenericRepository<GameItems> GameItemsRepository { get; }
        IGenericRepository<GameMaster> GameMasterRepository { get; }
        IGenericRepository<Transaction> TransactionRepository { get; }

        Task<int> SaveAsync();

        //Member
        IGenericRepository<Member> MemberRepository { get; }

        //MembershipProgram
        IGenericRepository<MembershipProgram> MembershipProgramRepository { get; }
        //WalletType
        IGenericRepository<WalletType> WalletTypeRepository { get; }

        //MemberActionType
        IGenericRepository<MemberActionType> MemberActionTypeRepository { get; }
        //MemberAction
        IGenericRepository<MemberAction> MemberActionRepository { get; }
        //MembershipLevel
        IGenericRepository<MembershipLevel> MembershipLevelRepository { get; }
    }
}

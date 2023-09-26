using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Helper
{
    public static class AppConstant
    {
        public const int MAX_VOUCHER_QUANTITY = 10000;
        public struct DelFlg
        {
            public const string HIDE = "1";
            public const string UNHIDE = "0";


        }
        public const string ACTIVE = "1";
        public const string UNREDEMPED = "0";
        public const string UNUSED = "0";
        public const string USED = "1";
        public const string Err_Prefix = "E-";
        public const string VietNamPhoneCode = "+84";
        public const string FormatGameCode = "HHyyddMMmm";
        public struct Status
        {
            public const string ALL = "0";
        }
        public const string BeginPublic = "-----BEGIN PUBLIC KEY-----\n";
        public const string EndPublic = "-----END PUBLIC KEY-----";
        public const string BeginPrivate = "-----BEGIN RSA PRIVATE KEY-----\n";
        public const string EndPrivate = "-----END RSA PRIVATE KEY-----";
        public const int RSA_LENGTH_2048 = 2048;
        public enum ChannelType
        {
            In_Store = 1,
            Online = 2,
            Other = 3
        }
        public const string Sender = "Promotion Engine";
        public const string Sender_Email = "promotion.engine.fpt@gmail.com";
        public const string Sender_Email_Pwd = "promotionengine";
        public const string Subject = "Promotion Engine send you a voucher. Enjoy it!";
        public const string Url_Gen_QR = "http://chart.googleapis.com/chart?cht=qr&chs=300x300&chl=";
        public const string Url_Get_Voucher = "https://promotion-engine.netlify.app/#/";
        public class SmtpHostAddress
        {
            public const string Host = "smtp.gmail.com";
            public const int Port = 465;

        }
        public struct EnvVar
        {
            public const int SUCCESS = 1;
            public const int FAIL = 0;
            public const int NOT_FOUND = -1;
            public const int DUPLICATE = 2;
            public const string DELETED = "1";
            public const string UNLIMIT = "0";
            public const string CONNECTOR = "-";
            public const bool ISLIMIT = true;
            public const bool IS_FOR_GAME = true;

            public const string NO_FILTER = "0";
            public const string FOR_MEMBER = "1";
            public const string INCLUDE = "1";
            public const string EXCLUDE = "0";
            public const string ApplyForAllStore = "Khuyến mãi áp dụng tại tất cả cửa hàng toàn quốc";
            public const int BrandId = 1;
            public const int AdminId = 2;
            public const string Admin = "Admin";
            public const string BrandManager = "Brand Manager";
            public const string Success_Message = "Thành công";
            public enum Holiday_Env
            {
                FOR_WEEKEND = 1,
                FOR_HOLIDAY = 1
            }
            public enum PostActionType
            {
                Gift_Product = 1,
                Gift_Voucher = 2,
                Gift_Point = 3,
                Gift_GameCode = 4
            }
            public enum PromotionType
            {
                Using_Voucher = 3,
                Using_PromoCode = 2,
                Automatic = 1,
            }
            public enum ForMemberShip
            {
                Both = 0,
                MemberShip = 1,
                Guest = 2
            }
            public enum ActionType
            {
                Amount_Order = 1,
                Percentage_Order = 2,
                Shipping = 3,
                Amount_Product = 4,
                Percentage_Product = 5,
                Unit = 6,
                Fixed = 7,
                Ladder = 8,
                Bundle = 9,

            }
            public struct CharsetType
            {
                public const string ALPHANUMERIC = "Alphanumeric";
                public const string ALPHABETIC = "Alphabetic";
                public const string ALPHABETIC_LOWERCASE = "Alphabetic Lowercase";
                public const string ALPHABETIC_UPERCASE = "Alphabetic Uppercase";
                public const string NUMBERS = "Numbers";
                public const string CUSTOM = "Custom";
            }
            public struct CharsetChars
            {
                public const string ALPHANUMERIC = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                public const string ALPHABETIC = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                public const string ALPHABETIC_LOWERCASE = "abcdefghijklmnopqrstuvwxyz";
                public const string ALPHABETIC_UPERCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                public const string NUMBERS = "0123456789";
            }

            public enum VoucherType
            {
                BULK_CODE = 1,
                STANDALONE_CODE = 2,
            }
            public struct Voucher
            {
                public const bool USED = true;
                public const bool UNUSED = false;
                public const bool REDEEMPED = true;
                public const bool UNREDEEM = false;
            }

            public enum PromotionStatus
            {
                DRAFT = 1,
                PUBLISH = 2,
                UNPUBLISH = 3,
                EXPIRED = 4,
            }

            public enum Exclusive
            {
                NoneExclusive = 0,
                GlobalExclusive = 1,
                ClassExclusiveOrder = 2,
                ClassExclusiveProduct = 3,
                ClassExclusiveShipping = 4,
                ClassExclusiveGift = 5,
            }
            public enum Holiday
            {
                FRIDAY = 5,
                SATURDAY = 6,
                SUNDAY = 0,
            }
        }
        public enum ErrCode
        {
            Invalid_Product = 101,
            Invalid_Quantity = 102,
            Invalid_Min_Quantity = 103,
            Invalid_Max_Quantity = 104,
            Invalid_Extra = 105,
            Invalid_NotExtra = 106,
            Invalid_Parent = 107,
            Invalid_Require = 108,
            Invalid_Group = 109,
            Duplicate_VoucherCode = 110,
            Invalid_VoucherCode = 111,
            Expire_Promotion = 112,
            Exclusive_Promotion = 113,
            Invalid_Gender = 114,
            InActive_Promotion = 115,
            Unmatch_Promotion = 116,
            Invalid_Holiday = 117,
            Invalid_SaleMode = 118,
            Invalid_MinAmount = 119,
            Invalid_Time = 120,
            Invalid_Early = 121,
            Invalid_HourFrame = 122,
            Invalid_DayInWeek = 123,
            Invalid_PaymentType = 124,
            NotExisted_Product = 125,
            Quantity_Product = 126,
            NotMatchCondition = 127,
            Invalid_Store = 128,
            Invalid_MemberLevel = 129,
            Duplicate_Promotion = 130,
            Invalid_TimeFrame = 131,
            Invalid_Operator = 132,
            Invalid_CustomerLevel = 133,
            Invalid_Effect = 134,
            Invalid_ProductCondition = 135,
            Invalid_ProductAction = 136,
            Invalid_VoucherQuantity = 137,
            Voucher_OutOfStock = 138,
            Login_Success = 139,
            Login_Fail = 140,
            Device_Access_Fail = 141,
            Device_Access_Server_Fail = 142,
            Exist_ProductCategory = 143,
            MemberLevel_Exist = 144,
            Product_Exist = 145,
            Product_Cate_NotFound = 146,
            MemberLevel_NotFound = 147,
            Order_Fail = 148,
            Channel_Not_Exist = 149,
            ApiKey_Not_Exist = 150,
            BrandCode_Mismatch = 151,
            Signature_Err = 152,
            HashData_Not_Valid = 153,
            Internal_Server_Error = 500
        }
        public class ErrMessage
        {

            public const string Invalid_Product = "Sản phẩm không tồn tại!";
            public const string Invalid_Quantity = "Số lượng không phải kiểu số!";
            public const string Invalid_Min_Quantity = "Số lượng sản phẩm ít hơn số lượng quy định";
            public const string Invalid_Max_Quantity = "Số lượng sản phẩm nhiều hơn số lượng quy định";
            public const string Invalid_Extra = "Sản phẩm extra cần đi kèm với sản phẩm chính";
            public const string Invalid_NotExtra = "Sản phẩm extra không thuộc sản phẩm chính";
            public const string Invalid_Parent = "Không tìm thấy sản phẩm chính";
            public const string Invalid_Require = "Đơn hàng chưa có sản phẩm bắt buộc";
            public const string Invalid_Group = "Sản phẩm chưa được chọn nhóm";
            public const string Duplicate_VoucherCode = "Trùng mã voucher.";
            public const string Invalid_VoucherCode = "Không có chương trình khuyến mãi phù hợp với mã voucher này.";
            public const string Used_VoucherCode = "Voucher đã được sử dụng.";
            public const string Expire_Promotion = "Khuyến mãi tạm dừng.";
            public const string Exclusive_Promotion = "Khuyến mãi không dùng chung với các khuyến mãi khác";
            public const string Invalid_Gender = "Giới tính không phù hợp với khuyến mãi";
            public const string InActive_Promotion = "Khuyến mãi chưa được kích hoạt";
            public const string Unmatch_Promotion = "Không có khuyến mãi phù hợp với voucher này";
            public const string Invalid_Holiday = "Khuyến mãi không áp dụng vào ngày lễ.";
            public const string Invalid_SaleMode = "Khuyến mãi không dành cho loại đơn hàng này.";
            public const string Invalid_MinAmount = "Giá đơn hàng thấp hơn quy định.";
            public const string Invalid_Time = "Thời gian khuyến mãi không phù hợp";
            public const string Invalid_Early = "Thời gian khuyễn mãi vẫn chưa bắt đầu";
            public const string Invalid_HourFrame = "Khung giờ khuyến mãi không phù hợp";
            public const string Invalid_DayInWeek = "Ngày trong tuần của khuyến mãi không phù hợp";
            public const string Invalid_PaymentType = "Loại hình thanh toán không phù hợp.";
            public const string NotExisted_Product = "Đơn hàng không có sản phẩm yêu cầu.";
            public const string Quantity_Product = "Sản phẩm yêu cầu thấp hơn quy định.";
            public const string NotMatchCondition = "Đơn hàng không thỏa mãn các điều kiện của khuyến mãi.";
            public const string Invalid_Store = "Đơn hàng không áp dụng tại cửa hàng này.";
            public const string Invalid_MemberLevel = "Đơn hàng không áp dụng cho loại khách hàng này.";
            public const string Duplicate_Promotion = "Khuyến mãi không áp dụng cho các voucher thuộc cùng một chương trình khuyến mãi.";
            public const string Invalid_TimeFrame = "Khuyến mãi không áp dụng vào thời gian này.";
            public const string Invalid_Operator = "Invalid Logic Operator";
            public const string Invalid_CustomerLevel = "Khuyến mãi không áp dụng cho loại khách hàng này";
            public const string Invalid_Effect = "Khuyến mãi không ảnh hưởng";
            public const string Invalid_ProductCondition = "Điều kiện sản phẩm không hợp lệ, xin hãy kiểm tra lại điều kiện lúc tạo";
            public const string Invalid_ProductAction = "Hành động khuyến mãi không hợp lệ, xin hãy kiểm tra lại hành động lúc tạo";
            public const string Invalid_VoucherQuantity = "Khuyến mãi không có đủ số lượng Voucher yêu cầu, số lượng còn lại ";
            public const string Voucher_OutOfStock = "Voucher đã được phát hết";
            public const string Login_Success = "Success";
            public const string Login_Fail = "Unauthorized";
            public const string Device_Access_Fail = "Không tìm thấy thiết bị, vui lòng thử lại";
            public const string Device_Access_Server_Fail = "Hệ thống xảy ra lỗi, vui lòng thử lại";
            public const string Internal_Server_Error = "Hệ thống xảy ra lỗi, vui lòng thử lại";
            public const string Opps = "Oops !!! Something Wrong. Try Again.";
            public const string Not_Found_Resource = "The server can not find the requested resource.";
            public const string Bad_Request = "The server could not understand the request due to invalid syntax.";
            public const string Unauthorized = "Unauthorized.";
            public const string No_Game_Campaign = "Hiện tại không có chương trình khuyến mãi dành cho game, bạn vui lòng quay lại lần sau nhé !";
            public const string Device_Not_Found = "Không tìm thấy thiết bị !";
            public const string Order_Fail = "Order failed !.";
            public const string MemberLevel_Exist = "MemberLevel exist.";
            public const string MemberLevel_NotFound = "MemberLevel notfound.";
            public const string Product_Exist = "Product exist.";
            public const string Product_Cate_NotFound = "Product category not found";
            public const string Exist_ProductCategory = "ProductCategory exist";
            public const string ApiKey_Not_Exist = "APIKey is not exist in the system";
            public const string ApiKey_Required = "APIKey is not required";
            public const string Brand_Not_Exist = "Brand does not exist!";
            public const string Length_Param = "length";
            public const string Length_Error_Message = "Length must be non-negative";
            public const string BrandCode_Mismatch = "Brand code không đúng";
            public const string Signature_Err = "Signature mismatch";
            public const string Signature_Err_Description = "Check your [BrandCode] or [ChannelCode]";
            public const string HashData_Not_Valid = "Hash data is not valid";
            public const string Invalid_ChannelCode = "Channel không tồn tại!";
            public const string Invalid_Channel_Distribute = "Channel không được phân phối loại voucher này!";
            public const string Empty_CustomerInfo = "Thông tin khách hàng không hợp lệ [CustomerName] [CustomerPhoneNo]!";

        }
        public class QueueMessage
        {
            public const string Running = "Queued Hosted Service is running.";
            public const string Stopping = "Queued Hosted Service is stopping.";
            public const string Error_Excuting = "Error occurred executing {WorkItem}.";

        }
        public class EffectMessage
        {
            //Khi tier thỏa hết điều kiện
            public const string AcceptCoupon = "acceptCoupon";
            //Auto apply promotion
            public const string AutoPromotion = "autoPromotion";
            //Chỉ có 1 set discount => action có discount lớn nhất tùy vào Discount Type
            public const string SetDiscount = "setDiscount";
            public const string SetUnit = "setDiscountUnit";
            public const string SetFixed = "setDiscountFixed";
            public const string SetLadder = "setDiscountLadder";
            public const string SetBundle = "setDiscountBundle";
            //Chỉ có 1 set shipping fee => action có discount lớn nhất tùy vào Discount Type
            public const string SetShippingFee = "setShippingFee";
            //Chỉ có 1 lần add điểm => action có discount lớn nhất tùy vào Discount Type
            public const string AddPoint = "addPoint";
            public const string AddGift = "addGift";
            public const string AddGiftProduct = "addGiftProduct";
            public const string AddGiftVoucher = "addGiftVoucher";
            public const string AddGiftPoint = "addGiftPoint";
            public const string AddGiftGameCode = "addGiftGameCode";
            public const string NoProductMatch = "actionProductMismatch";
            //Không có automatic promotion nào cả
            public const string NoAutoPromotion = "noAutoPromotion";


        }
        public class Operator
        {
            public const string GREATER_THAN = ">";
            public const string GREATER_THAN_OR_EQUAL = ">=";
            public const string LESS_THAN = "<";
            public const string LESS_THAN_OR_EQUAL = "<=";
            public const string EQUAL = "=";


        }
        public enum NextOperator
        {
            OR = 1,
            AND = 2,
        }
        public enum BundleStrategy
        {
            CHEAPEST = 1,
            MOST_EXPENSIVE = 2,
            DEFAULT = 3
        }

        //public const string URL = "https://localhost:44367/";
        //public const string URL = "https://promoengine.azurewebsites.net/";
        public const string URL = "http://54.151.235.125:14687/";

        public struct NotiMess
        {
            public const string PROCESSING_MESS = "is processing";
            public const string PROCESSED_MESS = "is finished";
            public const string ERROR_MESS = "is error";
            public const string VOUCHER_GENERATE_MESS = "Generating voucher";
            public const string VOUCHER_INSERT_MESS = "Inserting voucher";
            public const string VOUCHER_DELETE_MESS = "Deleting voucher";
            public struct Type
            {
                public const string INSERT_VOUCHERS = "INSERT_VOUCHERS";
                public const string DELETE_VOUCHERS = "DELETE_VOUCHERS";
            }
        }

        #region Statistic error message
        public struct StatisticMessage
        {
            public const string PROMO_COUNT_ERR = "Promotion statistic error, please try again.";
            public const string BRAND_ID_INVALID = "Brand invalid, please try again.";
        }
        #endregion

        public class SwapVisitor : ExpressionVisitor
        {
            private readonly Expression from, to;
            public SwapVisitor(Expression from, Expression to)
            {
                this.from = from;
                this.to = to;
            }
            public override Expression Visit(Expression node)
            {
                return node == from ? to : base.Visit(node);
            }
        }
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            InvocationExpression invocationExpression = Expression.Invoke((Expression)expression2, expression1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>((Expression)Expression.OrElse(expression1.Body, (Expression)invocationExpression), (IEnumerable<ParameterExpression>)expression1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            InvocationExpression invocationExpression = Expression.Invoke((Expression)expression2, expression1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>((Expression)Expression.AndAlso(expression1.Body, (Expression)invocationExpression), (IEnumerable<ParameterExpression>)expression1.Parameters);
        }
        public struct VoucherStatus
        {
            public const int ALL = 1;
            public const int UNUSED = 2;
            public const int USED = 3;
            public const int REDEMPED = 4;
        }
        public struct NOTIFY_MESSAGE
        {
            public struct TYPE
            {
                public const string WARNING = "warning";
                public const string SUCCESS = "success";
                public const string INFO = "info";
            }
            public struct ICON
            {
                public const string LOADING = "el-icon-loading";
                public const string WARNING = "el-icon-warning-outline";
                public const string SUCCESS = "el-icon-circle-check";
            }
            public struct TITLE
            {
                public const string WARNING = "Warning";
                public const string SUCCESS = "Success";
            }
            public struct MESSAGE
            {
                public const string GENERATE_VOUCHER = "Generating voucher";
                public const string GENERATE_VOUCHER_ERROR = "Error occurs generate voucher, please try again";
                public const string INSERT_VOUCHER = "Inserting voucher";
                public const string INSERT_VOUCHER_ERROR = "Error occurs when insert voucher, please try again";
                public const string INSERT_VOUCHER_COMPLETED = "Create voucher completed";
            }
        }
        //public const string CONNECTION_STRING = "Server=tcp:promotionengine.database.windows.net,1433;Database=PromotionEngine;User ID=adm;Password=Abcd1234;Trusted_Connection=false;MultipleActiveResultSets=true";
        public const string CONNECTION_STRING = "Server=13.229.223.108;Database=PromotionEngine;User ID=promotion_engine;Password=H5!d8Q?%s(RPB)J=m:Uug#4x_~fp;Trusted_Connection=false;MultipleActiveResultSets=true";
    }
}

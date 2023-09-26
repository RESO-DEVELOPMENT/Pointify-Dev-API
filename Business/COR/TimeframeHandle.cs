using ApplicationCore.Request;
using ApplicationCore.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using System.IO;
using System.Linq;
using System.Net;
using System;
using ApplicationCore.Services;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ApplicationCore.Chain
{
    public interface ITimeframeHandle : IHandler<Order>
    {
        //void SetHolidays(List<Holiday> holidays);
        void SetPromotions(List<Promotion> promotions);

    }
    public class TimeframeHandle : Handler<Order>, ITimeframeHandle
    {
        private readonly IConditionHandle _conditionHandle;

        public TimeframeHandle(IConditionHandle conditionHandle)
        {
            _conditionHandle = conditionHandle;
        }

        private List<Promotion> _promotions;
        public void SetPromotions(List<Promotion> promotions)
        {
            _promotions = promotions;
        }
        public override void Handle(Order order)
        {

            //Trường hợp auto apply
            if (order.CustomerOrderInfo.Vouchers == null || order.CustomerOrderInfo.Vouchers.Count == 0)
            {
                var acceptPromotions = new List<Promotion>();
                int invalidPromotions = 0;

                foreach (var promotion in _promotions)
                {
                    try
                    {
                        HandleDayOfWeek(promotion, order.CustomerOrderInfo.BookingDate.DayOfWeek);
                        HandleHour(promotion, order.CustomerOrderInfo.BookingDate.Hour);
                        acceptPromotions.Add(promotion);
                    }
                    catch (ErrorObj)
                    {
                        invalidPromotions++;
                    }
                }
                if (acceptPromotions.Count > 0)
                {
                    _promotions = acceptPromotions;
                }
            }
            else
            {
                foreach (var promotion in _promotions)
                {
                    HandleDayOfWeek(promotion, order.CustomerOrderInfo.BookingDate.DayOfWeek);
                    HandleHour(promotion, order.CustomerOrderInfo.BookingDate.Hour);
                }
            }
            _conditionHandle.SetPromotions(_promotions);
            base.Handle(order);
        }

        public void HandleDayOfWeek(Promotion promotion, DayOfWeek dayOfWeek)
        {
            if (!Common.CompareBinary((int)(Math.Pow(2, (int)dayOfWeek)), promotion.DayFilter))
            {
                throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_DayInWeek, message: AppConstant.ErrMessage.Invalid_DayInWeek);
            }
        }
        public void HandleHour(Promotion promotion, int hourOfDay)
        {
            if (!Common.CompareBinary((int)Math.Pow(2, hourOfDay), promotion.HourFilter))
            {
                throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_HourFrame, message: AppConstant.ErrMessage.Invalid_HourFrame);
            }
        }
    }
}


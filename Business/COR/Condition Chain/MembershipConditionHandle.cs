using ApplicationCore.Models;
using ApplicationCore.Request;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApplicationCore.Chain
{
    public interface IMembershipConditionHandle : IHandler<Order>
    {
        void SetConditionModel(ConditionModel conditions);
    }
    public class MembershipConditionHandle : Handler<Order>, IMembershipConditionHandle
    {
        private ConditionModel _condition;

        public override void Handle(Order order)
        {
            if (_condition is MembershipConditionModel)
            {
                var customer = order.CustomerOrderInfo.Users;
                HandleMembershipLevel((MembershipConditionModel)_condition, customer);
            }
            else
            {
                base.Handle(order);
            }
        }

        public void HandleMembershipLevel(MembershipConditionModel membershipCondition, Users user)
        {
            if (!string.IsNullOrEmpty(user.UserLevel))
            {
                if (!membershipCondition.MembershipLevel.Contains(user.UserLevel))
                {
                    membershipCondition.IsMatch = false;
                }
            }
            else membershipCondition.IsMatch = false;
        }
        public void SetConditionModel(ConditionModel conditions)
        {
            _condition = conditions;
        }
    }
}

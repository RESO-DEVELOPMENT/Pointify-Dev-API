using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IDeviceService : IBaseService<Device, DeviceDto>
    {
        public Task<bool> CheckExistingDevice(string imei);
        public Task<DeviceDto> Update(DeviceDto dto);
        public Task<GenericRespones<BrandDeviceDto>> GetBrandDevice(int PageSize, int PageIndex, Guid brandId);
        public string GenerateCode(Guid deviceId);
        public Task<PairResponseDto> GetTokenDevice(string deviceCode,string channelCode);
    }
}

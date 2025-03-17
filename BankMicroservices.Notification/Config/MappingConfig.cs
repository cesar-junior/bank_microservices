using AutoMapper;
using BankMicroservices.Notification.Data.ValueObjects;
using BankMicroservices.Notification.Model;

namespace BankMicroservices.Notification.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config => {
                config.CreateMap<NotificationVO, NotificationModel>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}

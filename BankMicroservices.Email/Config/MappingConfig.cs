using AutoMapper;
using BankMicroservices.Email.Data.ValueObjects;
using BankMicroservices.Email.Model;

namespace BankMicroservices.Email.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config => {
                config.CreateMap<EmailVO, EmailLog>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}

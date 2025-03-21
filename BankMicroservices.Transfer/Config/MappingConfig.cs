using AutoMapper;
using BankMicroservices.Transfer.Data.ValueObjects;
using BankMicroservices.Transfer.Model;

namespace BankMicroservices.Transfer.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config => {
                config.CreateMap<TransferVO, TransferModel>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}

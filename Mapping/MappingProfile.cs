using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using vega.Controllers.Resources;
using vega.Models;
using static vega.Controllers.Resources.VehicleResource;

namespace vega.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Domain to Api
            CreateMap<Make, MakeResource>();
            CreateMap<Model, ModelResource>();
            CreateMap<Feature, KeyValuePairResource>();
            CreateMap<Vehicle, VehicleResource>()
            .ForMember(vr => vr.Contact, opt => opt.MapFrom(v => new ContactResource{Name = v.ContactName, Email = v.ContactEmail, Phone = v.ContactPhone}))
            .ForMember(vr => vr.Features, opt => opt.MapFrom(v => v.Features.Select(vf => vf.FeatureId)));

            //Api to Domain
            CreateMap<VehicleResource, Vehicle>()
            .ForMember(v=>v.Id, opt => opt.Ignore())
            .ForMember(v => v.ContactName, opt => opt.MapFrom(vr =>vr.Contact.Name));
            CreateMap<VehicleResource, Vehicle>()
            .ForMember(v => v.ContactEmail, opt => opt.MapFrom(vr =>vr.Contact.Email));
            CreateMap<VehicleResource, Vehicle>()
            .ForMember(v => v.ContactPhone, opt => opt.MapFrom(vr =>vr.Contact.Phone));
            CreateMap<VehicleResource, Vehicle>()
            .ForMember(v => v.Features, opt => opt.Ignore())
            .AfterMap((vr, v) => {
                //seçilmeyen featuresleri sil                
               var removedFeatures = v.Features.Where(item => !vr.Features.Contains(item.FeatureId));
                foreach(var item in removedFeatures)        
                    v.Features.Remove(item);
                    
                //yeni feature ekleme  
                var addedFeatures = vr.Features.Where(id => !v.Features.Any(item => item.FeatureId == id)).Select(id => new VehicleFeature{ FeatureId = id });
                foreach (var item in addedFeatures)
                    v.Features.Add(item);
            });

        }
        
    }
}
﻿#if __MOBILE__
using Material.Infrastructure.Static;

namespace Material
{
    using System.Threading.Tasks;
    public class GPSRequester
    { 
        public async Task<GPSResponse> MakeGPSRequestAsync()
        {
#if __IOS__
            var locationManager = await new GPSAuthorizationFacade()
                .AuthorizeContinuousGPSUsage()
                .ConfigureAwait(false);
            return await new iOSGPSAdapter(locationManager)
                .GetPositionAsync()
                .ConfigureAwait(false);
#elif __ANDROID__
            return await new AndroidGPSAdapter()
                .GetPositionAsync()
                .ConfigureAwait(false);
#else
            throw new NotSupportedException();
#endif

        }
    }
}
#endif

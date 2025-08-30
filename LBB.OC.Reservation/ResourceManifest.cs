using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace LBB.OC.Reservation;

    public class ResourceManifestOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {
        private static ResourceManifest _manifest;

        static ResourceManifestOptionsConfiguration ()
        {
            _manifest = new ResourceManifest();

            _manifest
                .DefineScript("modal") // this will be the name you reference
                .SetUrl("~/LBB.OC.Reservation/Resources/js/modal.js");
        }

        public void Configure(ResourceManagementOptions options)
        {
            options.ResourceManifests.Add(_manifest);
        }
    }

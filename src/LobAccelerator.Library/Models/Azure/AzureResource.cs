using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.ComponentModel;
using System.Linq;

namespace LobAccelerator.Library.Models.Azure
{
    public abstract class AzureResource
    {
        [DisplayName("Name")]
        [Description("Resource Name")]
        public string Name { get; set; }

        private string _RegionName;
        [DisplayName("Region Name")]
        [Description("Region to deploy the resource to")]
        public string RegionName
        {
            get
            {
                return _RegionName;
            }
            set
            {
                _RegionName = value;

                try
                {
                    Region = Region.Values.First(r => r.Name.Equals(value));
                }
                catch (InvalidOperationException)
                {
                    Region = null;
                }
            }
        }

        [DisplayName("Region")]
        [Description("Region to deploy the resource to")]
        public Region Region { get; private set; }

        [DisplayName("Resource Group")]
        [Description("Name of resource group to deploy the resource group to")]
        public AzureResourceGroup ResourceGroup { get; set; }
    }
}

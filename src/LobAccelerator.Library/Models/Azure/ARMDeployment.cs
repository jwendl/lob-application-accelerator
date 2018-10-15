using System;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.Azure
{
    public class ARMDeployment
        : AzureResource
    {
        [DisplayName("Template Uri")]
        [Description("Uri for ARM template to deploy to Azure")]
        public Uri TemplateUri { get; set; }

        [DisplayName("Template Content Version")]
        [Description("Content Version for ARM template to deploy to Azure")]
        public string TemplateContentVersion { get; set; }

        [DisplayName("Template Parameters Json")]
        [Description("Template parameters for ARM template to deploy to Azure")]
        public string TemplateParametersJson { get; set; }
    }
}

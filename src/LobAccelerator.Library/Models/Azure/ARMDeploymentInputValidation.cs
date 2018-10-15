using System;
using System.Collections.Generic;
using System.Text;

namespace LobAccelerator.Library.Models.Azure
{
    public enum ARMDeploymentInputValidation
    {
        OK,
        InvalidARMDeploymentConfigObject,
        ARMDeploymentNameIsNotValid,
        ARMDeploymentRegionIsNotValid,
        ARMDeploymentUriIsNotValid,
        ARMDeploymentVersionIsNotValid
    }
}

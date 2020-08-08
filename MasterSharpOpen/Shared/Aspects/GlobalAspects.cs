using System;
using System.Collections.Generic;
using System.Text;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Extensibility;

//[assembly: Log(AttributePriority = 2, AttributeTargetMemberAttributes = MulticastAttributes.Protected | MulticastAttributes.Internal | MulticastAttributes.Public)]
//[assembly: Log(AttributePriority = 3, AttributeExclude = true, AttributeTargetMembers = "get_*")]
//[assembly: Log(AttributePriority = 4, AttributeExclude = true, AttributeTargetMembers = "MasterSharpOpen.Shared.StaticAuth.*")]
//[assembly: Log(AttributePriority = 1, AttributeTargetTypes = "MasterSharpOpen.Shared.*")]
//[assembly: Log(AttributePriority = 5, AttributeExclude = true, AttributeTargetTypes = "MasterSharpOpen.Shared.AppStateService*")]
namespace MasterSharpOpen.Shared.Aspects
{
    public class GlobalAspects
    {
    }
}

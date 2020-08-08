using System;
using System.Collections.Generic;
using System.Text;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Extensibility;

[assembly: Log(AttributePriority = 1, AttributeTargetMemberAttributes = /*MulticastAttributes.Protected | MulticastAttributes.Internal |*/ MulticastAttributes.Public)]
[assembly: Log(AttributePriority = 2, AttributeExclude = true, AttributeTargetMembers = "get_*")]

namespace MasterSharpOpen.Shared.Aspects
{
    class GlobalAspects
    {
    }
}

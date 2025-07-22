// Wrapper to preserve old references temporarily
using UnityEngine;

public class MyComponent : Unity.VRTemplate.MyComponent { }
 
namespace PS01
{
    public class MyComponent : MonoBehaviour
    {
         // empty on purpose
    }
}

namespace Unity.VRTemplate
{
    public class MyComponent : PS01.MyComponent
    {
        // empty on purpose
    }
}
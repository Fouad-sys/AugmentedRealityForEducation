# About Subsystem Registration (DEPRECATED)

**The Subsystem Registration package has been deprecated for Unity Editor 2023.1 and above. It provided an API to register a subsystem implemented using [Subsystem](xref:UnityEngine.Subsystem) API. The `Subsystem` API will be deprecated in 2023.1. A subsystem need to be implemented using the new [SubsystemWithProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemWithProvider) API and registered using [SubsystemDescriptorStore](xref:UnityEngine.SubsystemsImplementation.SubsystemDescriptorStore) API.**

Provides internal API for subsystems to register themselves with the Subsystem Manager within Unity. This will allow the Subsystem Manager to keep track of and provide lifecycle management for the registered subsystem.

# How to use this package
This package is a dependency for any other packages that defines new types of subsystems.

# Requirements

This version of `com.unity.subsystemregistration` is compatible with the following versions of the Unity Editor:

* 2019.4 and later (required)

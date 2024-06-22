# Knowledge
This document contains information learned during the project 

# Burst 
The `[BurstDiscard]` attribute prevents the Burst compiler from compiling the marked method. If the method is called within Burst-compiled code, it will be skipped.
Usage: Use this attribute sparingly, as it may result in skipped logic when running in a Burst context.

The `[BurstCompile]` attribute must be applied to both the class and the static method for Burst compilation to work properly.
A Burst-compiled method makes all subsequent method calls Burst-compiled as well.  
- Static Methods: The attribute must be set on static methods, except for jobs and ISystem implementations, because the code generation process makes these methods static automatically (see Unity forum post).
Class-Level Compilation:

The `[BurstCompile]` attribute must also be set on the class. Burst first looks for classes with this attribute and then looks for methods, as checking every method individually is inefficient.  

Resource: https://forum.unity.com/threads/when-where-and-why-to-put-burstcompile-with-mild-under-the-hood-explanation.1344539/


# Rendering
Mesh       ======================  
RenderMesh				Renders a mesh with given rendering parameters.
RenderMeshIndirect			Renders multiple instances of a mesh using GPU instancing and rendering command arguments from commandBuffer.
RenderMeshInstanced			Renders multiple instances of a mesh using GPU instancing.
RenderMeshPrimitives			Renders multiple instances of a Mesh using GPU instancing and a custom shader.

Primitives ======================
RenderPrimitives			Renders non-indexed primitives with GPU instancing and a custom shader.
RenderPrimitivesIndexed			Renders indexed primitives with GPU instancing and a custom shader.
RenderPrimitivesIndexedIndirect		Renders indexed primitives with GPU instancing and a custom shader with rendering command arguments from commandBuffer.
RenderPrimitivesIndirect		Renders primitives with GPU instancing and a custom shader using rendering command arguments from commandBuffer.
# Features
The game is a tickbased planet simulator where each planet gains point every tick.
- Solar System
- Input system
- Fast rendering using indirect rendering
- UI systems using Unity Toolkit
- Calculated with Unity DOTS ECS

# Architecture
This section describes the Projects in this system and how they interact with each other 

The **Application** handles all end user interactions to the domain and rendering of domain objects, interacts with all the other.

The **Domain** handles all domain related logic  

The **Simulation** contains mathematical complex formulas and equations used in the domain. Such as calculation of planetary gravity and position on an elipse.

The **Engine** shares commonly used tools that are not domain specific. Such as a physics raycast. Rendering helper scripts. And system groups like time systems for ticks

![architecture](diagrams/architecture.png | width=300px)


## Engine
- Physics spherecollider
- Fast rendering

## Gameplay
- Discovery systen
- Solar system

## Rendering
- PositionOnlyRendering
- Position and animation renderer

## Simulation
- Orbitalmechanics
- Ellipsemechanics, math related to ellipse 
- Circlemechanics, math releated to circles


## Known bottlenecks
5k - only bodies 15 ms

Update 5.71, Orbits 5.46, Bodies 0.13  
In orbits
3.00 - 5.00 GetOrbitMatricies
0.92 Rendermesh
The RenderMeshInstanced got it down to 0.01 ms


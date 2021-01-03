<div align="center">
    <img width=25% src="https://github.com/red-owl-games/Sleipnir/raw/master/logo.png">
</div>

<div align="center">
  <!-- Version -->
  <a href="#">
    <img src="https://img.shields.io/github/package-json/v/red-owl-games/Sleipnir"
      alt="Version" />
  </a>
  <a href="https://github.com/red-owl-games/Sleipnir/issues">
    <img src="https://img.shields.io/github/issues/red-owl-games/Sleipnir.svg"
      alt="Issues" />
  </a>
  <a href="#">
    <img src="https://img.shields.io/badge/contributions-welcome-orange.svg"
      alt="Contributions Welcome" />
  </a>
  <a href="#">
    <img src="https://img.shields.io/badge/license-MIT-blue.svg"
      alt="License" />
  </a>
</div>

<div align="center">
  <sub> Built with ❤︎ by
  <a href="https://redowlgames.com/">Red Owl Games</a>
</div>

----

## Why

After surveying the landscape of graph frameworks for Unity for the past few years 
and trying to use them I found myself always painting myself into corners, having to work
in constrained ways and/or just not liking the API setup around building graph based tools.

I've also come to love Odin Inspector and the power it brings to writing editor tools.

So here we are - this is our love letter to Odin by marrying it with a graph framework that
gets out of your way and hopefully makes it trivial to build new graph based tools ontop.

## Features

 - Strong interface based design - Graphs and Nodes can be POCO's (Plain Old CLR Object - IE Classes)
 - Built ontop of Unity
   - Uses Unity's Serializer to reduce outside dependancies (leverages the new `SerializeReference` features)
   - Uses UI toolkit (UIElements) to support the future of Editor UI's
 - Natively supports Odin Inspector inside the Graph Editor Window
 - Complete control over flow/execution - Allows for writing any kind of graph, statemachine, behaviour, flow, etc
 - Batteries Included - Base classes for Graph, Node, Port and Flow to get up and working quickly
   - Also includes standard nodes and graph types that could be shared from graph tool to graph tool
    
## Quickstart

To write a new custom node quickly you inherit from the base Node class and define your ports.

```csharp
using RedOwl.Sleipnir.Engine;

[Node]
public class AddNode : Node
{
    [FlowIn(nameof(OnEnter))] public FlowPort Enter;
    [FlowOut] public FlowPort Exit;
    
    [ValueIn] public ValuePort<float> Left;
    [ValueIn] public ValuePort<float> Right;

    [ValueOut] public ValuePort<float> Output;
    
    protected IFlowPort OnEnter(IFlow flow)
    {
        Output.Value = Left.Value + Right.Value;
        return Exit;
    }
}
```

The above is a simple definition of a node that performs an "add" math operation on the
"left" and "right" value in ports and pushs the result to the value out port.

To then make use of this new node you navigate to your Project Browser in Unity right click
and then `Create` -> `Red Owl` -> `Graph`

This creates a scriptable object asset in your project that can serialize graph data using
unity's serializer.  From there you double click the asset to open the Graph Editor window.

From there you can press spacebar and a node search window will open - navigate to your new
node and that will place an instance in your graph.  You should beable to create a graph like the following image.

<img width=25% src="https://github.com/red-owl-games/Sleipnir/raw/master/example.png">

The follow graph will perform an addition on the 2 float values and log the result to the console.

Cheers! (For more examples dig into the codebase at ./Engine/Nodes)

## Installing

In *Package Manager* click *Add package from git URL* and use the following:

```
https://github.com/red-owl-games/Sleipnir.git
```
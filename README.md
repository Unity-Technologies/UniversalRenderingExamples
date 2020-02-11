# Universal Rendering Examples
###### This project contains a collection of Custom Renderer examples. This will be updated as we refine the feature and add more options.
##### Current support : 2019.3.0f6 or later with UniversalRP 7.2.0
![CustomRendererExamples][MainImg]

#### Usage of the project
- Clone the repo/Download the zip down to your computer
- Load in Unity version 2019.3.0f6 or later.
- Each scene contains a different example
- Upon loading the scene the Universal Render Pipeline asset is switched automatically to the one for the specific example

#### Project layout
- `_CompletedDemos/XXX` This contains the demo data and scene files.
- `External Assets` This folder contains shared assets such as meshes and textures.
- `Scripts` Folder that contains some custom renderer features and utility scripts

## [Click here for more details about the individual examples](https://github.com/Unity-Technologies/UniversalRenderingExamples/wiki)

# Examples
There are three examples currently in this project, they are:
## First Person Objects
This demo showcases a setup to render first person perspective objects with a FOV(Field Of View) that differs from the game scene rendering FOV, this is common in first person games where the FOV needed for the experience is too wide for the objects held in hand ends up distorted.

![FOV][FPSMain]

- Can be found at `_CompletedDemos/FPSCameraCompleted/FPSCameraDemo.unity`
- Uses the `Render Objects (Experimental)` feature that is provided with the Universal Render Pipeline Package
- Different FOV for GameObjects with the Layer set to `First Person Objects`
- Depth settings to avoid rendering inside close objects like walls
- Stencil settings to properly handle transparencies

---
### Toon Outline
Showcases a setup to create an effect of Toon styled outlines, there are two approaches in the example, a post-process one and a hull mesh approach. One example uses a custom RendererFeature and both use custom shaders.

![FOV][OutlineMain]

- Can be found at `_CompletedDemos/ToonOutlinePostprocessCompleted/ToonOutlinePost.unity`
- Uses the `Blit` custom feature(`Blit.cs` and `BlitPass.cs`) that is provided with this project for the `OutlinePostEffect` custom renderer
- Custom Sobel Filter shader with Posterize option
- Uses the `Render Objects (Experimental)` feature that is provided with the Universal Render Pipeline Package for the `OutlineHullEffect` custom renderer
- Custom Toon Outline shader based of Unity Basic Toon shader

---
### Object Occlusion
Showcases a setup useful to create effects when an object moves behind another one. Zero code needed, Shadergraph used for dither effect.

![FOV][OcclusionMain]

- Can be found at `_CompletedDemos/UnityOcclusionDemoCompleted/UnityOcclusionDemo.unity`
- Uses the `Render Objects (Experimental)` feature that is provided with the Universal Render Pipeline Package
- Shadergraph made Dither shader for the effect

[MainImg]: http://drive.google.com/uc?export=view&id=1hFiaaw4oN_dQOn9RgWE5ZWoWddh-DHjr

[FPSMain]: http://drive.google.com/uc?export=view&id=1_grNnDnCFZ8Y7EVJGgAda7fFr-PFVHfc
[OutlineMain]: http://drive.google.com/uc?export=view&id=1Dn2WqgOp8gAcfbnd-qKj6k908N4XMfHg
[OcclusionMain]: http://drive.google.com/uc?export=view&id=1EMQNnU40TQcCrd0Glrt4r0xOWqKiIwSF

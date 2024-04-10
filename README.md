# Unity Light LOD (Level of Detail) System

An Level of detail system for Unity that controlls wich settings are active depending on the distance from the light source to the light controller.

## Installation

### Via Unity Package Manager
- Follow [these](https://docs.unity3d.com/Manual/upm-ui-giturl.html) steps and by step 4 paste "https://github.com/NaxtorGames/com.naxtorgames.lightlodsystem.git".

### The manual way
1. Download this code as Zip.
2. Extract the Zip at "<**Your-Unity-Project-Name/Packages/com.naxtorgames.lightlodsystem**>"
3. Return to Unity and let it compile.

## How to use

There are three core components to setup this system:
- **Light LOD Controller**
- **Light LOD Source**
- **Light LOD Settings**

### Creating the **Light LOD Controller**
- Pick any game object in your scene wich should control the lights. Preferable you want to use your active camera.
- Add the **Light LOD Controller** component under "**Component -> NaxtorGames -> Light LOD System -> Light LOD Controller**"
- The following settings can be set:
    - **Update Tick**: The time between every tick in seconds.
    - **Consider Direction**: If the direction of the **Light LOD Controller** matters. For example: When the controller is assigned to the main camera, when enabled can this option disable any **Light LOD Source** light wich is behind the camera. (This does not affect lights that are in front of the controller but are obscured by an obstacle.)

### Creating the **Light LOD Source** 
- Go to: "**GameObject -> Light -> LOD**" and select either "**Point Light**" or "**Spot Light**" to create an pre configured **Light LOD**.    
- You can also just add the **Light LOD Source** component manualy under: "**Component -> NaxtorGames -> Light LOD System -> Light LOD Source**".
- The following settings can be set:
    - **Light Source**: The **Light** component wich should be controlled.
    - **LOD Settings**: The settings that will be applied at wich point.
    - **Range Is Threshold**: If the **Dot Threshold** should be overriden by the range of the **Light Source**.
    - **Dot Threshold**: Controlls how far the direction can differ between the active **Light LOD Controller**s forward direction and the direction between the **Light LOD Source** and the **Light LOD Controller**.
    A positive value means that the **Light LOD Controller** looks away from the **Light LOD Source**. More information about the [Vector Dot Product](https://en.wikipedia.org/wiki/Dot_product).

### Creating **Light LOD Settings**
- Create an new **Light LOD Settings** Asset under "**Create -> NaxtorGames -> Light LOD System -> Light LOD Settings**"
- In the property array "**Lod Settings**" create a new entry and set the following settings:
    - **Min Distance**: Controls at wich distance from the active **Light LOD Controller* to the **Light LOD Source** the current setting is active.
    - **Is Enabled**: Defines when this setting is selected, should the light source be enabled.
    - **Force Is Enabled State**: If the **Is Enabled** value should be enforced no matter what. In case the **Consider Direction** setting on the active **Light LOD Controller** is enabled it could happen that the light will be disabled because the light isn't in front of the controller. In this case you want to force the light to be on so no matter what the light has to be the **Is Enabled** state.
    - **Light Render Mode**: Defines how precise the light should be calculated. It can either be **Per Pixel** (High) or **Per Vertex** (Low) or it can be **Auto** so Unity itself defines this value.
    On the light component itself the values under Render Mode are represented under an different name:
        - Auto
        - Important (Per Pixel)
        - Not Important (Per Vertex)
    - **Shadow Quality**: If the light casts shadows and if so should it be filtered (blurred) (Soft Shadows) or not (Hard Shadows).
    - **Shadow Resolution**: How big the resolution for the casted shadows are.

### Final Steps

When every part is ready assign the desired **Light LOD Settings** to all **Light LOD Source**s and done.
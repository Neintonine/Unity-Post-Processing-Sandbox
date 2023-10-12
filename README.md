# Unity-Post-Processing-Sandbox
Contains some post processing effects that can be used in a game, that runs with URP.

The Post-Processing Effects are written against Unity 2022.3.10f1. They may/probably will work in older and newer versions... I didn't check yet.

## Repo-Structure
- "Scripts" contains C# scripts used to run the effect.
- "Shaders" contains the HLSL shaders to actually do the effect.
- "Sample Contents" contains the sample content from the URP and that will also include all effects, since I test them there.

- "Framework"-folders contains Scripts, that are being used across all effects. (Common would be a better name, yes...)

## Setup your project to use these effects.
1. Copy "Scripts" and "Shaders" to your project.
    - If you only want a particular effect, copy only the folder with the effect and the "Framework" folder. (That one is important!)
2. In your "Universal Renderer Data"-object add a renderer feature called "PostProcessingFeature".
3. In the feature, specify the effects you want to be active.
4. In your "Universal Render Pipeline Asset" check "Depth Texture", "Opaque Texture" and turn off "Opaque Downsampling"
(while not essecial, I had some issues with this feature in my outline effect, so I recommend turning it off.)
5. Add your desired effects to your volume.
6. Done!

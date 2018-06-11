To build, just build the .sln in VS 2017. HugsLib depedencies are fetched via NuGet and the RimWorld and Unity assemblies will be copied out of the game's Steam directory. The output will automatically be copied into the game's mod directory if you use Steam.

If you are not using Steam, create a `ThirdParty` directory at the root of the repository and copy `Assembly-CSharp.dll` and `UnityEngine.dll` from the game into there.

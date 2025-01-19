# Tokamak
A C# engine for games and virtual worlds.

This project is just starting out.  The goal is to provide the basis for doing the basic building blocks of a game engine that can be used in a variety of different ways.

# Building
Should be a mater of opening the solution in Visual Studio and hitting build.

Not yet tested it under Linux or Mac OS X, but if you have the .NET 8.0 tools installed you should be able to run:
```
dotnet build Tokamak.sln
```

The current executable is the TestBed project where all the expiramentation is going on.

# TODOS
- [ ] Abstractions
  - [ ] OpenGL (in progress)
  - [ ] DirectX
  - [ ] Metal
  - [ ] Vulkan (in progress)
	- [ ] Extension loader system
  - [ ] OS level GUI objects (e.g. the main window/screen)
  - [ ] Direct hardware input (e.g. DirectInput)
  - [ ] Sound
- [ ] Graphite (2D Graphics)
  - [X] Basic line drawing
  - [ ] Bitmap drawing (basic blits working)
  - [ ] Text rendering (in progress)
	- [ ] Basics for debugging (reworking)
	- [ ] Advanced formatting and layout
  - [ ] Complex shape drawing
  - [ ] Animation
- [ ] In engine UI

# Status
- Currently the Vulkan driver is really broken, will likely need to rewrite portions of it.
- Reorganized a good chunk of the project to clean up the code a bit.
- Created a basic GameHost type hosting system similar to how Microsoft organizes most other .NET Core apps.
- FBX Loading working (ish), still fragile and assumes the file isn't corrupt or messed up somehow.

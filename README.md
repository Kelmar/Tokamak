# Tokamak
A C# engine for games and virtual worlds.

This project is just starting out.  The goal is to provide a set up easily understood building blocks that can be combined together to build the application, or to be 
replaced, updated, and extended however the user wishes to fill their desired needs.

## Projects
- Abstractions\
  This project holds all of the key abstraction interfaces.

- Hosting\
  Base runtime classes, provides configuration and application host management.  Similar to the NET Generic Host, but with a focus on applications with real time processing.

- Graphite\
  2D Graphics library with a rich set of functions for rendering beautiful UIs and other 2D graphics.

- Mathematics\
  Additional mathematical functions for processing color, basic physics primitives and hit testing.

- Quill\
  Library for doing reading and rendering of typography.

- Readers\
  Generic list of format readers.

- Tritium\
  Base graphics hardware management and rendering.

- Utilities\
  Generic runtime utility classes and methods.

- VFS\
  A Virtual File System library

# Building
Should be a mater of opening the solution in Visual Studio and hitting build.

Not yet tested it under Linux or Mac OS X, but if you have the .NET 8.0 tools installed you should be able to run:
```
dotnet build Tokamak.sln
```

The current executable is the TestBed project where all the experimentation is going on.

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
  - [ ] Complex shape drawing (in progress, several functions working)
  - [ ] Bitmap drawing (basic blits working)
  - [ ] Text rendering (in progress)
    - [ ] Basics for debugging (reworking)
    - [ ] Advanced formatting and layout
  - [ ] Animation
- [ ] Quill (Fonts)
- [ ] In engine UI

# Status
- Currently the Vulkan driver is really broken, will likely need to rewrite portions of it.
- Reorganized a good chunk of the project to clean up the code a bit.
- Created a basic GameHost type hosting system similar to how Microsoft organizes most other .NET Core apps.
- FBX Loading working (ish), still fragile and assumes the file isn't corrupt or messed up somehow.

using UhgeEngine.Core;
using UhgeEngine.Targets.Desktop;

using var engine = new Engine(new (new DesktopWindowDriver()));
engine.Run();
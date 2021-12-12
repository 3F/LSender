# [LSender](https://github.com/3F/LSender)

Ascetic aggregative repeater for loggers etc.

[![Build status](https://ci.appveyor.com/api/projects/status/fdrrp7mgb4vsm4gv/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/lsender/branch/master)
[![NuGet package](https://img.shields.io/nuget/v/LSender.svg)](https://www.nuget.org/packages/LSender/)
[![release](https://img.shields.io/github/release/3F/LSender.svg)](https://github.com/3F/LSender/releases/latest)
[![Tests](https://img.shields.io/appveyor/tests/3Fs/lsender/master.svg)](https://ci.appveyor.com/project/3Fs/lsender/build/tests)
[![License](https://img.shields.io/badge/License-MIT-74A5C2.svg)](https://github.com/3F/LSender/blob/master/License.txt)

[![Build history](https://buildstats.info/appveyor/chart/3Fs/lsender?buildCount=20&showStats=true)](https://ci.appveyor.com/project/3Fs/lsender/history)

```r
Copyright (c) 2016-2021 Denis Kuzmin <x-3F@outlook.com> github/3F
```

[ 「 <sub>@</sub> ☕ 」 ](https://3F.github.io/Donation/)

## Features

### Control vectors

```csharp
LSender.Sent += (object sender, MsgArgs e) =>
{
    Assert.True(e.At("Module"));
    Assert.True(e.At("DepC", "Module"));
    Assert.True(e.At("DepC", "DepB", "Module"));
    Assert.True(e.At("DepC", "DepB", "DepA", "Module"));

    Assert.False(e.At("DepB", "DepC", "Module"));
    Assert.False(e.At("DepA", "DepB", "Module"));

    Assert.True(e.At("DepB", "DepA", "Module"));
};
```

### Split within the domain

It can also be splitted within the domain,

```xml
<PropertyGroup>
  <LSenderExtIncSrc>..\LSender\src\</LSenderExtIncSrc>
</PropertyGroup>
<Import Project="$(LSenderExtIncSrc)src.targets" />
```

```xml
<ItemGroup>
  <ProjectReference Include="..\ModuleA\ClassA.csproj">
    <Aliases>ModuleA,%(Aliases)</Aliases>
  </ProjectReference>
  <ProjectReference Include="..\ModuleB\ClassB.csproj">
    <Aliases>ModuleB,%(Aliases)</Aliases>
  </ProjectReference>
</ItemGroup>
```

```csharp
extern alias ModuleA;
extern alias ModuleB;
namespace UserCode
{
    class Program
    {
        static void Main()
        {
            // ModuleA.net.r_eg.Components.LSender
            // ModuleB.net.r_eg.Components.LSender
            // ...
        }
    }
}
```

### Configure vectors

```xml
<PropertyGroup>
  <DefineConstants>LSR_FEATURE_S_VECTOR;$(DefineConstants)</DefineConstants>
  <LSenderExtIncSrc>..\LSender\src\</LSenderExtIncSrc>
</PropertyGroup>
<Import Project="$(LSenderExtIncSrc)src.targets" />
```
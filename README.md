# ExtraChess
![Build and Tests](https://github.com/ExtraBB/ExtraChess/actions/workflows/ExtraChess.yml/badge.svg)

ExtraChess is a UCI-based .NET Core Chess engine. It uses .NET6 as backing framework.

## Engine
The Engine is located in the `ExtraChess` project. It uses [Bitboards](https://www.chessprogramming.org/Bitboards) to generate moves. Interfacing with the engine is done with the [UCI](http://wbec-ridderkerk.nl/html/UCIProtocol.html) protocol.

Start from Visual Studio or run using the `dotnet` CLI:
```
dotnet run --project ./ExtraChess/ExtraChess.csproj
```

## UI
Included in the solution is the `ExtraChessUI` project. It is a simple chess UI that allows you to play a game of chess using multiple engines. Just place the engine in the same folder as the executable and it will present it as option in the UI.

Start from Visual Studio or run the UI using the `dotnet` CLI:
```
dotnet build
dotnet run --project ./ExtraChessUI/ExtraChessUI.csproj
```

## Tests
Unit tests for the engine can be found in the `ExtraChessTests` project.

Run from the TestExplorer in Visual Studio or using the `dotnet` CLI:
```
dotnet test
```

## Prerequisites
- Make sure you have .NET 6 installed.
- For running from Visual Studio, install the Visual Studio 2022 Preview.

## Goals
 - Create a playable game of chess vs computer
 - Create an AI that plays around 1500 level

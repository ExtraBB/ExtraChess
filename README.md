# ExtraChess
ExtraChess is a UCI-based .NET Core Chess engine. It uses .NET6 as backing framework.

## Engine
The Engine is located in the `ExtraChess` project. It uses [Bitboards](https://www.chessprogramming.org/Bitboards) to generate moves. Interfacing with the engine is done with the [UCI](http://wbec-ridderkerk.nl/html/UCIProtocol.html) protocol.

## UI
Included in the solution is the `ExtraChessUI` project. It is a simple chess UI that allows you to play a game of chess using multiple engines. Just place the engine in the same folder as the executable and it will present it as option in the UI.

## Tests
Unit tests for the engine can be found in the `ExtraChessTests` project. They can be run using the TestExplorer in Visual Studio.

## Running the project
- Make sure you have .NET 6 installed.
- Open the solution in Visual Studio.
- Either run the ExtraChess engine directly or the ExtraChessUI project for a UI.

## Goals
 - Create a playable game of chess vs computer
 - Create an AI that plays around 1500 level

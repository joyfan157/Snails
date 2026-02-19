# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Run

```bash
dotnet build
dotnet run
```

This is a .NET 9.0 MonoGame (DesktopGL) project. No test framework is configured.

## Game Overview

"Nigiri Sushi" — a sushi kitchen game where the player moves between stations to prepare and serve dishes (Nigiri, Maki Roll, Miso Soup). Orders arrive on timers and the player scores points by fulfilling them. The core mechanic is a **ghost recording system**: the player records action sequences (press Space), which spawn autonomous ghost helpers that replay those actions in a loop.

## Architecture

**Game1.cs** — Main game class. Owns all entities, runs the update/draw loop. Stations are hardcoded in `LoadContent()` by position. Station list order matters because ghosts reference stations by index.

**Core/**
- `GameConstants` — All tuning values (speeds, timers, sizes, scoring). Central place to adjust game balance.
- `TextureManager` — Primitive rendering (colored rectangles, outlines, progress bars). No sprite assets; everything is drawn with a 1x1 white pixel texture.

**Entities/**
- `Player` — WASD movement with sprint (Shift) and stamina system. Sprint is ice-like (low friction, momentum-based) while walk is stiff/instant. Click stations to interact. Q to trash held item.
- `Station` (abstract) — Base for all workstations. Each has `Interact(ref Item? playerItem)` which swaps/transforms the player's held item. Stations are solid (player collides/bounces). Subclasses: `SalmonStation`, `RiceCookerStation`, `NoriStation`, `TofuStation`, `DashiStation`, `ChoppingStation`, `CuttingBoardStation`, `PotStation`, `OutputStation`.
- `Item` (abstract) — Carried items with `ItemType` enum. `Item.Create(ItemType?)` is a factory method. Each concrete item defines display color and name.
- `Obstacle` — Simple collision-only entity.

**Entities/Ghost/**
- `GhostRecording` — Data class holding recorded frames (position, velocity, sprinting) and interactions (station index, held item before/after).
- `GhostEntity` — Replays a recording in a loop. Waits at interaction points if the station isn't ready. Returns to start position between loops. Uses `CanGhostInteract()` to pre-check stations before interacting.

**Systems/**
- `GhostRecorder` — Records player movement frames and station interactions while in Recording state (Space toggles).
- `OrderManager` — Spawns random dish orders on a timer, tracks active orders, handles fulfillment.
- `ScoreManager` — Tracks score.

**UI/**
- `HudRenderer` — Draws top HUD bar with orders, score, stamina, held item, and recording status.

## Key Patterns

- **Station interaction** uses pass-by-ref: `Interact(ref Item? playerItem)`. Stations either give an item (source stations), transform it (processing stations), or consume it (output station).
- **Ghost system** records station interactions by index into the station list, so the station list order in `Game1.LoadContent()` must remain stable for recordings to replay correctly.
- All rendering uses `TextureManager` primitives — no sprite sheets or content pipeline images (only a SpriteFont for text).

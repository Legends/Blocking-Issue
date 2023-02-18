# Blocking-Issue

Server spawns client process

The issue happens in client.cs (line 38) on first read using ` await JsonSerializer.DeserializeAsync<T>(pipe)`
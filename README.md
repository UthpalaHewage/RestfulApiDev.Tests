# RestfulApiDev.Tests (xUnit)

This project contains automated API tests (xUnit) for the public API: https://restful-api.dev/

The tests cover:
1. Get list of all objects (GET)
2. Add an object (POST)
3. Get a single object by the created ID (GET by ID)
4. Update the created object (PUT)
5. Delete the created object (DELETE)

---

<!-- ## Prerequisites -->

- Windows 10/11
- .NET SDK 8.0 or later installed
- Verify .NET is installed:

<!-- execution instructions -->
- dotnet restore
- dotnet test
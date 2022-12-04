# Diagram as Code Example Using Structurizr and C#
## Running Locally
1. Run the Structurizr On-Premise Docker Image
  1. `docker run -it --rm -p 8080:8080 -v PATH:/usr/local/structurizr structurizr/onpremises`
  1. Replace `PATH` with your path to a directory where structurizer can save its state
1. Navigate to `localhost:8080`
1. Login with default username (`structurizr`) and password (`password`)
1. Create a new workspace.
1. Go to Settings of the workspace
1. Find the API Key and API Secret
  1. Set environment variables
    1. `STRUCTURIZR_API_KEY` - The API Key of your workspace
    1. `STRUCTURIZR_API_SECRET` - The API Secret off your workspace
    1. `STRUCTURIZR_WORKSPACE_ID` - The ID of your workspace
1. Go to `src/MyDiagrams` and run `dotnet run` command
1. Your workspace should have been updated now with the diagram defined in your C# code.
1. For the first time, your diagram's layout will be a mess, you can edit the diagram in the browser and click Auto Layout (the magic wand icon), then save the diagram. Any subsequent run of this program will then respect the layout.

## References
1. [Structurizr Getting Started](https://structurizr.com/share/18571/documentation#configuration)
1. [Structurizr C# Client](https://github.com/structurizr/dotnet/blob/master/docs/getting-started.md)
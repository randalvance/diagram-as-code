using Structurizr;
using Structurizr.Api;

var workspace = new Workspace("My Workspace", "This is a model of my software system.");
var model = workspace.Model;

var user = model.AddPerson("User", "A user of my software system.");
var softwareSystem = model.AddSoftwareSystem("Software System", "My software system.");
user.Uses(softwareSystem, "Uses");

var views = workspace.Views;
var systemContextView = views.CreateSystemContextView(softwareSystem, "SystemContext", "Example of System Context Diagram");
systemContextView.AddAllSoftwareSystems();
systemContextView.AddAllPeople();

var styles = views.Configuration.Styles;
styles.Add(new ElementStyle(Tags.Person) { Shape = Shape.Person });
styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });

var config = GetStructurizerConfig();

var structurizrClient = new StructurizrClient(config.Url, config.ApiKey, config.ApiSecret);
// If you want to save the generated diagram JSON to a file, set it here
// structurizrClient.WorkspaceArchiveLocation = new DirectoryInfo("diagrams");

structurizrClient.PutWorkspace(config.WorkspaceID, workspace);

(string Url, string ApiKey, string ApiSecret, int WorkspaceID) GetStructurizerConfig()
{
  var url = "http://localhost:8080/api";
  var apiKey = Environment.GetEnvironmentVariable("STRUCTURIZR_API_KEY");
  var apiSecret = Environment.GetEnvironmentVariable("STRUCTURIZR_API_SECRET");
  var workspaceID = Environment.GetEnvironmentVariable("STRUCTURIZR_WORKSPACE_ID");

  var missingEnvironmentVariables = new List<string>();
  if (apiKey is null) missingEnvironmentVariables.Add("STRUCTURIZR_API_KEY");
  if (apiSecret is null) missingEnvironmentVariables.Add("STRUCTURIZR_API_SECRET");
  if (workspaceID is null) missingEnvironmentVariables.Add("STRUCTURIZR_WORKSPACE_ID");

  if (missingEnvironmentVariables.Any())
  {
    throw new Exception($"Missing environment variables: {string.Join(", ", missingEnvironmentVariables)}");
  }

  return (url!, apiKey!, apiSecret!, Int32.Parse(workspaceID!));
}
using Structurizr;
using Structurizr.Api;

#region Constants
const string TagExisting = "Existing";
const string TagDatabase = "Database";
#endregion

var workspace = new Workspace("Credit Blue Sky", "Architecture Diagrams for Credit Blue Sky");
var model = workspace.Model;
model.ImpliedRelationshipsStrategy = new CreateImpliedRelationshipsUnlessAnyRelationshipExistsStrategy();
model.Enterprise = new Enterprise("GIC Pte. Ltd.");

#region Models

// Users
var userCreditOfficer = model.AddPerson("GIC Credit Officers");
var userTraders = model.AddPerson("GIC Traders");

// Software Systems
var creditBlueSky = model.AddSoftwareSystem("Credit Blue Sky", "A system used to manage counterparty limits, monitor exposures, approve counterparty onboarding, and generate reports");
var upstreamDatabases = model.AddSoftwareSystem("Upstream Databases", "iLab, Allegro, EVE, ETHSYS, GEMS, U2ID, SecLanding, CCP, Adapativ, APEX");
upstreamDatabases.AddTags(TagExisting, TagDatabase);
var upstreamSystems = model.AddSoftwareSystem("Upstream Systems", "ATLAS, Adaptiv, Stargazer");
upstreamSystems.AddTags(TagExisting);
var downstreamSystems = model.AddSoftwareSystem("Downstream System", "Stargazer, Downstream Legacy Modules");
downstreamSystems.AddTags(TagExisting);
var adfs = model.AddSoftwareSystem("ADFS", "Authentication and Authorization");
adfs.AddTags(TagExisting);
var controlM = model.AddSoftwareSystem("Control-M", "Job Scheduling");
controlM.AddTags(TagExisting);
var tableau = model.AddSoftwareSystem("Tableau", "Data Visualization Tool");
tableau.AddTags(TagExisting);
var msPowerAutomate = model.AddSoftwareSystem("MS Power Automate", "Workflow Automation Tool");
msPowerAutomate.AddTags(TagExisting);
var k2 = model.AddSoftwareSystem("K2", "Workflow Automation Tool");
k2.AddTags(TagExisting);
var microsoftExchange = model.AddSoftwareSystem("Microsoft Exchange", "Email");
microsoftExchange.AddTags(TagExisting);

// Containers
var creditBlueSkyUi = creditBlueSky.AddContainer("UI", "User Interface", "React");
var creditBlueSkyWeb = creditBlueSky.AddContainer("Internal API", "Web Application", "Python, FastAPI");
var creditBlueSkyApi = creditBlueSky.AddContainer("Downstream API", "Web Application", "Python, FastAPI");
var creditBlueSkyAppDb = creditBlueSky.AddContainer("Application Database", "Application Database", "PostgreSQL");
creditBlueSkyAppDb.AddTags(TagDatabase);
var k8sJobs = creditBlueSky.AddContainer("Kubernetes Jobs", "Kubernetes Jobs", "Kubernetes");
var awsCloudWatch = creditBlueSky.AddContainer("AWS CloudWatch", "Monitoring Tool", "AWS CloudWatch");
var awsSecretsManager = creditBlueSky.AddContainer("AWS Secrets Manager", "Secrets Management Tool", "AWS Secrets Manager");
var awsElasticache = creditBlueSky.AddContainer("AWS Elasticache", "In-Memory Data Store", "AWS Elasticache");
var awsGlue = creditBlueSky.AddContainer("AWS Glue", "ETL Tool", "AWS Glue");
#endregion

#region Relationships
userCreditOfficer.Uses(creditBlueSkyUi, "Uses", "HTTPS");
userTraders.Uses(creditBlueSkyUi, "Uses", "HTTPS");

creditBlueSkyUi.Uses(creditBlueSkyWeb, "Uses", "HTTPS");

creditBlueSkyWeb.Uses(creditBlueSkyAppDb, "Reads from and writes to", "ODBC");
creditBlueSkyWeb.Uses(awsCloudWatch, "Logs", "HTTPS");
creditBlueSkyWeb.Uses(awsSecretsManager, "Gets Secrets", "HTTPS");
creditBlueSkyWeb.Uses(awsElasticache, "Reads from and writes to", "HTTPS");
creditBlueSkyWeb.Uses(upstreamDatabases, "Reads from and writes to", "ODBC");
creditBlueSkyWeb.Uses(upstreamSystems, "Get Data", "HTTPS");
creditBlueSkyWeb.Uses(adfs, "AuthN/AuthZ", "HTTPS");
creditBlueSkyWeb.Uses(microsoftExchange, "Sends email", "SMTP");
creditBlueSkyWeb.Uses(k2, "Triggers and Gets Updates", "HTTPS");

creditBlueSkyApi.Uses(upstreamDatabases, "Reads from and writes to", "ODBC");
creditBlueSkyApi.Uses(upstreamSystems, "Get Data", "HTTPS");
creditBlueSkyApi.Uses(adfs, "AuthN/AuthZ", "HTTPS");
creditBlueSkyApi.Uses(creditBlueSkyAppDb, "Reads from and writes to", "ODBC");
creditBlueSkyApi.Uses(awsCloudWatch, "Logs", "HTTPS");
creditBlueSkyApi.Uses(awsSecretsManager, "Gets Secrets", "HTTPS");
creditBlueSkyApi.Uses(awsElasticache, "Reads from and writes to", "HTTPS");

k8sJobs.Uses(awsGlue, "Triggers", "HTTPS");

tableau.Uses(creditBlueSkyAppDb, "Get Report Data", "ODBC");
downstreamSystems.Uses(creditBlueSkyApi, "Get CP Limits", "HTTPS");
creditBlueSkyWeb.Uses(msPowerAutomate, "Trigger Workflow", "HTTPS");
controlM.Uses(k8sJobs, "Triggers", "HTTPS");
#endregion

#region Deployment Nodes

var internet = model.AddDeploymentNode("Internet");
var microsoft365 = internet.AddDeploymentNode("Microsoft 365");
microsoft365.AddDeploymentNode("Exchange Server").Add(microsoftExchange);
microsoft365.AddDeploymentNode("Power Automate").Add(msPowerAutomate);

var gicOnPremise = model.AddDeploymentNode("GIC On-premise");
gicOnPremise.AddDeploymentNode("K2 Server").Add(k2);
gicOnPremise.AddDeploymentNode("ADFS Server").Add(adfs);
gicOnPremise.AddDeploymentNode("Control-M").Add(controlM);

var aws = model.AddDeploymentNode("GIC AWS", "", "");
aws.AddDeploymentNode("AWS Glue").Add(awsGlue);
aws.AddDeploymentNode("AWS CloudWatch").Add(awsCloudWatch);
aws.AddDeploymentNode("AWS Secrets Manager").Add(awsSecretsManager);
aws.AddDeploymentNode("AWS Elasticache").Add(awsElasticache);
aws.AddDeploymentNode("AWS S3").Add(creditBlueSkyUi);

var bento = aws.AddDeploymentNode("Bento", "", "Openshift");
bento.AddDeploymentNode("Credit Blue Sky Internal API", "", "Kubernetes Pod").Add(creditBlueSkyWeb);
bento.AddDeploymentNode("Credit Blue Sky Downstream API", "", "Kubernetes Pod").Add(creditBlueSkyApi);
bento.AddDeploymentNode("Kubernetes Job", "", "Kubernetes Job").Add(creditBlueSkyUi);

var cloudFrontInfra = aws.AddInfrastructureNode("AWS CloudFront");

var awsRdsNode = aws.AddDeploymentNode("Application Database", "AWS RDS", "AWS RDS Postgres");
awsRdsNode.Add(creditBlueSkyAppDb);
#endregion

AddSystemContextView(workspace);
AddContainerView(workspace);
AddDeploymentView(workspace);

AddStyles(workspace);

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

void AddSystemContextView(Workspace workspace)
{
  var systemContextView = workspace.Views.CreateSystemContextView(creditBlueSky, "Context", "Context Diagram");
  // systemContextView.EnableAutomaticLayout(RankDirection.TopBottom, 300, 300, 50, true);
  systemContextView.AddAllElements();
}

void AddContainerView(Workspace workspace)
{
  var containerView = workspace.Views.CreateContainerView(creditBlueSky, "Container", "Container Diagram");
  // containerView.EnableAutomaticLayout();
  containerView.AddAllElements();
}

void AddDeploymentView(Workspace workspace)
{
  var deploymentView = workspace.Views.CreateDeploymentView(creditBlueSky, "Deployment", "Deployment Diagram");
  deploymentView.AddAllDeploymentNodes();
}

void AddStyles(Workspace workspace)
{
  var styles = workspace.Views.Configuration.Styles;
  styles.Add(new ElementStyle(Tags.Person) { Shape = Shape.Person });
  styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
  styles.Add(new ElementStyle(TagExisting) { Background = "#00b51e", Color = "#ffffff" });
  styles.Add(new ElementStyle(TagDatabase) { Shape = Shape.Cylinder });
}